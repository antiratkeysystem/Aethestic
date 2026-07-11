using System;
using Microsoft.Win32;

namespace CvMega.Helper
{
	// Token: 0x02000086 RID: 134
	internal class RegeditKey
	{
		// Token: 0x0600021B RID: 539 RVA: 0x0001AF64 File Offset: 0x00019164
		public static bool CheckValue(string name)
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.CreateSubKey(RegeditKey.Regkey, false))
				{
					bool flag = registryKey2.GetValue(name) != null;
					if (flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0001AFE0 File Offset: 0x000191E0
		public static void SetValue(string name, string value)
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.CreateSubKey(RegeditKey.Regkey, true))
				{
					bool flag = RegeditKey.CheckValue(name);
					if (flag)
					{
						registryKey2.DeleteValue(name);
					}
					registryKey2.SetValue(name, value);
				}
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0001B05C File Offset: 0x0001925C
		public static string GetValue(string name)
		{
			bool flag = !RegeditKey.CheckValue(name);
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
				{
					using (RegistryKey registryKey2 = registryKey.CreateSubKey(RegeditKey.Regkey, false))
					{
						result = (string)registryKey2.GetValue(name);
					}
				}
			}
			return result;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0001B0DC File Offset: 0x000192DC
		public static string[] GetValues()
		{
			string[] valueNames;
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.CreateSubKey(RegeditKey.Regkey, false))
				{
					valueNames = registryKey2.GetValueNames();
				}
			}
			return valueNames;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0001B144 File Offset: 0x00019344
		public static void DeleteValue(string name)
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.CreateSubKey(RegeditKey.Regkey, true))
				{
					bool flag = RegeditKey.CheckValue(name);
					if (flag)
					{
						registryKey2.DeleteValue(name);
					}
				}
			}
		}

		// Token: 0x04000091 RID: 145
		public static string Regkey = "SOFTWARE\\Google\\CrashReports";
	}
}
