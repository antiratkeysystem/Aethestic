using System;
using Microsoft.Win32;

namespace Intelix.Helper
{
	// Token: 0x0200005A RID: 90
	public static class CpuInfo
	{
		// Token: 0x06000136 RID: 310 RVA: 0x00010A2C File Offset: 0x0000EC2C
		public static string GetName()
		{
			string result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0"))
				{
					string text;
					if ((text = (((registryKey != null) ? registryKey.GetValue("ProcessorNameString") : null) as string)) == null)
					{
						text = ((((registryKey != null) ? registryKey.GetValue("VendorIdentifier") : null) as string) ?? "Unknown");
					}
					result = text;
				}
			}
			catch
			{
				result = "Unknown";
			}
			return result;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00010ABC File Offset: 0x0000ECBC
		public static int GetLogicalCores()
		{
			int result;
			try
			{
				result = Environment.ProcessorCount;
			}
			catch
			{
				result = 0;
			}
			return result;
		}
	}
}
