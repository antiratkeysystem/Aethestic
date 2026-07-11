using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Intelix.Helper
{
	// Token: 0x0200005C RID: 92
	public static class ImpersonationHelper
	{
		// Token: 0x0600013F RID: 319 RVA: 0x0001114C File Offset: 0x0000F34C
		public static IDisposable ImpersonateWinlogon()
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			IDisposable result;
			try
			{
				ImpersonationHelper.EnableDebugPrivilege();
				Process process = Process.GetProcessesByName("winlogon").FirstOrDefault<Process>();
				if (process == null)
				{
					throw new Exception("Процесс winlogon.exe не найден");
				}
				bool flag = !NativeMethods.OpenProcessToken(process.Handle, 14U, out zero);
				if (flag)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error(), "Ошибка OpenProcessToken");
				}
				bool flag2 = !NativeMethods.DuplicateTokenEx(zero, 12U, IntPtr.Zero, 2U, 2U, out zero2);
				if (flag2)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error(), "Ошибка DuplicateTokenEx");
				}
				bool flag3 = !NativeMethods.ImpersonateLoggedOnUser(zero2);
				if (flag3)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error(), "Ошибка ImpersonateLoggedOnUser");
				}
				result = new ImpersonationHelper.ImpersonationContext();
			}
			catch
			{
				bool flag4 = zero2 != IntPtr.Zero;
				if (flag4)
				{
					NativeMethods.CloseHandle(zero2);
				}
				bool flag5 = zero != IntPtr.Zero;
				if (flag5)
				{
					NativeMethods.CloseHandle(zero);
				}
				NativeMethods.RevertToSelf();
				throw;
			}
			return result;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00011254 File Offset: 0x0000F454
		private static void EnableDebugPrivilege()
		{
			IntPtr zero = IntPtr.Zero;
			try
			{
				bool flag = !NativeMethods.OpenProcessToken(NativeMethods.GetCurrentProcess(), 40U, out zero);
				if (flag)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					throw new Win32Exception(lastWin32Error, string.Format("Ошибка OpenProcessToken: Код ошибки {0}", lastWin32Error));
				}
				NativeMethods.LUID luid = default(NativeMethods.LUID);
				bool flag2 = !NativeMethods.LookupPrivilegeValue(null, "SeDebugPrivilege", ref luid);
				if (flag2)
				{
					int lastWin32Error2 = Marshal.GetLastWin32Error();
					throw new Win32Exception(lastWin32Error2, string.Format("Ошибка LookupPrivilegeValue: Код ошибки {0}", lastWin32Error2));
				}
				NativeMethods.TOKEN_PRIVILEGES token_PRIVILEGES = new NativeMethods.TOKEN_PRIVILEGES
				{
					PrivilegeCount = 1U,
					Luid = luid,
					Attributes = 2U
				};
				bool flag3 = !NativeMethods.AdjustTokenPrivileges(zero, false, ref token_PRIVILEGES, (uint)Marshal.SizeOf(typeof(NativeMethods.TOKEN_PRIVILEGES)), IntPtr.Zero, IntPtr.Zero);
				if (flag3)
				{
					int lastWin32Error3 = Marshal.GetLastWin32Error();
					throw new Win32Exception(lastWin32Error3, string.Format("Ошибка AdjustTokenPrivileges: Код ошибки {0}", lastWin32Error3));
				}
				int lastWin32Error4 = Marshal.GetLastWin32Error();
				bool flag4 = lastWin32Error4 != 0;
				if (flag4)
				{
					throw new Win32Exception(lastWin32Error4, string.Format("AdjustTokenPrivileges вернул успех, но установил код ошибки {0}", lastWin32Error4));
				}
			}
			finally
			{
				bool flag5 = zero != IntPtr.Zero;
				if (flag5)
				{
					NativeMethods.CloseHandle(zero);
				}
			}
		}

		// Token: 0x04000025 RID: 37
		private const uint TOKEN_DUPLICATE = 2U;

		// Token: 0x04000026 RID: 38
		private const uint TOKEN_IMPERSONATE = 4U;

		// Token: 0x04000027 RID: 39
		private const uint TOKEN_QUERY = 8U;

		// Token: 0x04000028 RID: 40
		private const uint TOKEN_ADJUST_PRIVILEGES = 32U;

		// Token: 0x04000029 RID: 41
		private const uint SecurityImpersonation = 2U;

		// Token: 0x0400002A RID: 42
		private const uint TokenImpersonation = 2U;

		// Token: 0x0400002B RID: 43
		private const uint SE_PRIVILEGE_ENABLED = 2U;

		// Token: 0x020000DE RID: 222
		private class ImpersonationContext : IDisposable
		{
			// Token: 0x0600031C RID: 796 RVA: 0x0001EC02 File Offset: 0x0001CE02
			public void Dispose()
			{
				NativeMethods.RevertToSelf();
			}
		}
	}
}
