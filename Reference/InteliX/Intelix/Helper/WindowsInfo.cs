using System;
using Microsoft.Win32;

namespace Intelix.Helper
{
	// Token: 0x02000066 RID: 102
	public static class WindowsInfo
	{
		// Token: 0x06000174 RID: 372 RVA: 0x0001267C File Offset: 0x0001087C
		public static string GetProductName()
		{
			string result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion"))
				{
					bool flag = registryKey == null;
					if (flag)
					{
						result = "Unknown";
					}
					else
					{
						string str = (registryKey.GetValue("ProductName") as string) ?? "Unknown";
						string text;
						if ((text = (registryKey.GetValue("ReleaseId") as string)) == null)
						{
							text = ((registryKey.GetValue("DisplayVersion") as string) ?? "");
						}
						string str2 = text;
						result = (str + " " + str2).Trim();
					}
				}
			}
			catch
			{
				result = "Unknown";
			}
			return result;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00012740 File Offset: 0x00010940
		public static string GetBuildNumber()
		{
			string result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion"))
				{
					bool flag = registryKey == null;
					if (flag)
					{
						result = "Unknown";
					}
					else
					{
						object value = registryKey.GetValue("CurrentBuild");
						string text;
						if ((text = ((value != null) ? value.ToString() : null)) == null)
						{
							object value2 = registryKey.GetValue("CurrentBuildNumber");
							text = (((value2 != null) ? value2.ToString() : null) ?? "Unknown");
						}
						result = text;
					}
				}
			}
			catch
			{
				result = "Unknown";
			}
			return result;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x000127E0 File Offset: 0x000109E0
		public static string GetArchitecture()
		{
			string result;
			try
			{
				result = (Environment.Is64BitOperatingSystem ? "x64" : "x86");
			}
			catch
			{
				result = "Unknown";
			}
			return result;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00012820 File Offset: 0x00010A20
		public static string GetVersion()
		{
			string result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion"))
				{
					bool flag = registryKey == null;
					if (flag)
					{
						result = "Unknown";
					}
					else
					{
						object value = registryKey.GetValue("CurrentMajorVersionNumber");
						object value2 = registryKey.GetValue("CurrentMinorVersionNumber");
						bool flag2 = value != null && value2 != null;
						if (flag2)
						{
							result = string.Format("{0}.{1}", value, value2);
						}
						else
						{
							string text = registryKey.GetValue("CurrentVersion") as string;
							bool flag3 = !string.IsNullOrEmpty(text);
							if (flag3)
							{
								result = text;
							}
							else
							{
								result = "Unknown";
							}
						}
					}
				}
			}
			catch
			{
				result = "Unknown";
			}
			return result;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000128F0 File Offset: 0x00010AF0
		public static string GetFullInfo()
		{
			return string.Concat(new string[]
			{
				"OS Product: ",
				WindowsInfo.GetProductName(),
				"\nOS Build: ",
				WindowsInfo.GetBuildNumber(),
				"\nOS Arch: ",
				WindowsInfo.GetArchitecture()
			});
		}
	}
}
