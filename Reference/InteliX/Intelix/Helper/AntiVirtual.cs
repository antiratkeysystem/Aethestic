using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace Intelix.Helper
{
	// Token: 0x02000057 RID: 87
	public static class AntiVirtual
	{
		// Token: 0x06000118 RID: 280 RVA: 0x00010670 File Offset: 0x0000E870
		public static void CheckOrExit()
		{
			bool flag = AntiVirtual.ProccessorCheck();
			if (flag)
			{
				throw new Exception();
			}
			bool flag2 = AntiVirtual.CheckDebugger();
			if (flag2)
			{
				throw new Exception();
			}
			bool flag3 = AntiVirtual.CheckMemory();
			if (flag3)
			{
				throw new Exception();
			}
			bool flag4 = AntiVirtual.CheckDriveSpace();
			if (flag4)
			{
				throw new Exception();
			}
			bool flag5 = AntiVirtual.CheckUserConditions();
			if (flag5)
			{
				throw new Exception();
			}
			bool flag6 = AntiVirtual.CheckCache();
			if (flag6)
			{
				throw new Exception();
			}
			bool flag7 = AntiVirtual.CheckFileName();
			if (flag7)
			{
				throw new Exception();
			}
			bool flag8 = AntiVirtual.CheckCim();
			if (flag8)
			{
				throw new Exception();
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00010708 File Offset: 0x0000E908
		public static bool CheckFileName()
		{
			return Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName).ToLower().Contains("sandbox");
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00010740 File Offset: 0x0000E940
		public static bool ProccessorCheck()
		{
			return Environment.ProcessorCount <= 1;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00010760 File Offset: 0x0000E960
		public static bool CheckDebugger()
		{
			return Debugger.IsAttached;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00010778 File Offset: 0x0000E978
		public static bool CheckDriveSpace()
		{
			return new DriveInfo("C").TotalSize / 1073741824L < 50L;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x000107A8 File Offset: 0x0000E9A8
		public static bool CheckCache()
		{
			return AntiVirtual.CheckCount("Select * from Win32_CacheMemory");
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000107C4 File Offset: 0x0000E9C4
		public static bool CheckCim()
		{
			return AntiVirtual.CheckCount("Select * from CIM_Memory");
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000107E0 File Offset: 0x0000E9E0
		public static bool CheckCount(string selector)
		{
			return new ManagementObjectSearcher(selector).Get().Count == 0;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00010808 File Offset: 0x0000EA08
		public static bool CheckMemory()
		{
			return Convert.ToDouble(new ManagementObjectSearcher("Select * From Win32_ComputerSystem").Get().Cast<ManagementObject>().FirstOrDefault<ManagementObject>()["TotalPhysicalMemory"]) / 1048576.0 < 2048.0;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00010858 File Offset: 0x0000EA58
		public static bool CheckUserConditions()
		{
			string a = Environment.UserName.ToLower();
			string text = Environment.MachineName.ToLower();
			bool flag = (!(a == "frank") || !text.Contains("desktop")) && !(a == "WDAGUtilityAccount");
			bool result;
			if (flag)
			{
				bool flag2 = a == "robert";
				result = (flag2 && text.Contains("22h2"));
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
