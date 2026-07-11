using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Device
{
	// Token: 0x0200002E RID: 46
	public class InstalledBrowsers : ITarget
	{
		// Token: 0x0600007F RID: 127 RVA: 0x00006378 File Offset: 0x00004578
		public void Collect(InMemoryZip zip, Counter counter)
		{
			List<InstalledBrowsers.Browser> list = (from g in InstalledBrowsers.GetBrowsers().GroupBy((InstalledBrowsers.Browser b) => b.Name, StringComparer.OrdinalIgnoreCase)
			select g.First<InstalledBrowsers.Browser>()).ToList<InstalledBrowsers.Browser>();
			int maxName = Math.Max("Name".Length, list.Max((InstalledBrowsers.Browser b) => b.Name.Length));
			int maxVersion = Math.Max("Version".Length, list.Max((InstalledBrowsers.Browser b) => b.Version.Length));
			int length = "In Use".Length;
			List<string> list2 = new List<string>
			{
				"Name".PadRight(maxName) + " | " + "Version".PadRight(maxVersion),
				new string('-', maxName + maxVersion + length + 6)
			};
			list2.AddRange(list.Select(delegate(InstalledBrowsers.Browser b)
			{
				InstalledBrowsers.SafeGetExeName(b.Path);
				return b.Name.PadRight(maxName) + " | " + b.Version.PadRight(maxVersion);
			}));
			bool flag = list.Count > 0;
			if (flag)
			{
				zip.AddTextFile("InstalledBrowsers.txt", string.Join("\n", list2));
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000064FC File Offset: 0x000046FC
		private static IEnumerable<InstalledBrowsers.Browser> GetBrowsers()
		{
			string[] array = new string[]
			{
				"SOFTWARE\\WOW6432Node\\Clients\\StartMenuInternet",
				"SOFTWARE\\Clients\\StartMenuInternet"
			};
			List<InstalledBrowsers.Browser> list = new List<InstalledBrowsers.Browser>();
			string[] array2 = array;
			foreach (string keyPath in array2)
			{
				list.AddRange(InstalledBrowsers.GetBrowsersFromRegistry(keyPath, Registry.LocalMachine));
				list.AddRange(InstalledBrowsers.GetBrowsersFromRegistry(keyPath, Registry.CurrentUser));
			}
			InstalledBrowsers.Browser edgeLegacyVersion = InstalledBrowsers.GetEdgeLegacyVersion();
			bool flag = edgeLegacyVersion != null;
			if (flag)
			{
				list.Add(edgeLegacyVersion);
			}
			return list;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00006590 File Offset: 0x00004790
		private static IEnumerable<InstalledBrowsers.Browser> GetBrowsersFromRegistry(string keyPath, RegistryKey root)
		{
			List<InstalledBrowsers.Browser> browsers = new List<InstalledBrowsers.Browser>();
			try
			{
				using (RegistryKey key = root.OpenSubKey(keyPath))
				{
					if (key != null)
					{
						foreach (string browserName in key.GetSubKeyNames())
						{
							try
							{
								using (RegistryKey browserKey = key.OpenSubKey(browserName))
								{
									if (browserKey != null)
									{
										string command = browserKey.GetValue("") as string;
										string path = InstalledBrowsers.StripQuotesFromCommand(command);
										string version = browserKey.GetValue("Version") as string ?? "Unknown";
										if (!string.IsNullOrEmpty(path) && File.Exists(path))
										{
											browsers.Add(new InstalledBrowsers.Browser
											{
												Name = browserName,
												Path = path,
												Version = version
											});
										}
									}
								}
							}
							catch
							{
							}
						}
					}
				}
			}
			catch
			{
			}
			return browsers;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000065A8 File Offset: 0x000047A8
		private static InstalledBrowsers.Browser GetEdgeLegacyVersion()
		{
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\SystemAppData\\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\\Schemas"))
			{
				string text = ((registryKey != null) ? registryKey.GetValue("PackageFullName") : null) as string;
				bool flag = text != null;
				if (flag)
				{
					Match match = Regex.Match(text, "\\d+(\\.\\d+)+");
					bool success = match.Success;
					if (success)
					{
						return new InstalledBrowsers.Browser
						{
							Name = "Microsoft Edge (Legacy)",
							Path = null,
							Version = match.Value
						};
					}
				}
			}
			return null;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00006650 File Offset: 0x00004850
		private static string StripQuotesFromCommand(string command)
		{
			bool flag = string.IsNullOrWhiteSpace(command);
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				command = command.Trim();
				bool flag2 = command.StartsWith("\"");
				if (flag2)
				{
					int num = command.IndexOf('"', 1);
					bool flag3 = num > 1;
					if (flag3)
					{
						result = command.Substring(1, num - 1);
					}
					else
					{
						result = null;
					}
				}
				else
				{
					int num2 = command.IndexOf(' ');
					bool flag4 = num2 <= 0;
					if (flag4)
					{
						result = command;
					}
					else
					{
						result = command.Substring(0, num2);
					}
				}
			}
			return result;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000066D8 File Offset: 0x000048D8
		private static string SafeGetExeName(string path)
		{
			string result;
			try
			{
				bool flag = string.IsNullOrWhiteSpace(path);
				if (flag)
				{
					result = null;
				}
				else
				{
					string fileName = Path.GetFileName(path);
					result = ((fileName != null) ? fileName.ToUpperInvariant() : null);
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0200009C RID: 156
		private class Browser
		{
			// Token: 0x1700001C RID: 28
			// (get) Token: 0x06000255 RID: 597 RVA: 0x0001BE3B File Offset: 0x0001A03B
			// (set) Token: 0x06000256 RID: 598 RVA: 0x0001BE43 File Offset: 0x0001A043
			public string Name { get; set; }

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x06000257 RID: 599 RVA: 0x0001BE4C File Offset: 0x0001A04C
			// (set) Token: 0x06000258 RID: 600 RVA: 0x0001BE54 File Offset: 0x0001A054
			public string Path { get; set; }

			// Token: 0x1700001E RID: 30
			// (get) Token: 0x06000259 RID: 601 RVA: 0x0001BE5D File Offset: 0x0001A05D
			// (set) Token: 0x0600025A RID: 602 RVA: 0x0001BE65 File Offset: 0x0001A065
			public string Version { get; set; }
		}
	}
}
