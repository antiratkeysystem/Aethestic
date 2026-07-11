using System;
using System.Linq;

namespace Intelix.Helper
{
	// Token: 0x02000063 RID: 99
	public static class RandomStrings
	{
		// Token: 0x0600016B RID: 363 RVA: 0x00011DDC File Offset: 0x0000FFDC
		public static string GenerateHashTag()
		{
			return " #" + RandomStrings.GenerateString();
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00011E00 File Offset: 0x00010000
		public static string GenerateString()
		{
			return RandomStrings.GenerateString(5);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00011E18 File Offset: 0x00010018
		public static string GenerateString(int length)
		{
			char c = "abcdefghijklmnopqrstuvwxyz"[RandomStrings.Random.Next("abcdefghijklmnopqrstuvwxyz".Length)];
			char[] value = (from s in Enumerable.Repeat<string>("abcdefghijklmnopqrstuvwxyz", length - 1)
			select s[RandomStrings.Random.Next(s.Length)]).ToArray<char>();
			return c.ToString() + new string(value);
		}

		// Token: 0x04000034 RID: 52
		private const string Ascii = "abcdefghijklmnopqrstuvwxyz";

		// Token: 0x04000035 RID: 53
		private static readonly Random Random = new Random();
	}
}
