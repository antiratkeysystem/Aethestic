using System;
using System.Text;

namespace CvMega.Helper
{
	// Token: 0x02000085 RID: 133
	internal class RandomPathGenerator
	{
		// Token: 0x06000217 RID: 535 RVA: 0x0001AE04 File Offset: 0x00019004
		public static string GenerateCompletelyRandomPath()
		{
			int num = RandomPathGenerator.random.Next(1, 11);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < num; i++)
			{
				int length = RandomPathGenerator.random.Next(3, 20);
				stringBuilder.Append('/');
				stringBuilder.Append(RandomPathGenerator.GenerateRandomString(length));
			}
			bool flag = RandomPathGenerator.random.Next(2) == 1;
			if (flag)
			{
				stringBuilder.Append(RandomPathGenerator.extensions[RandomPathGenerator.random.Next(RandomPathGenerator.extensions.Length)]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0001AEA0 File Offset: 0x000190A0
		private static string GenerateRandomString(int length)
		{
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				stringBuilder.Append(RandomPathGenerator.chars[RandomPathGenerator.random.Next(RandomPathGenerator.chars.Length)]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0400008E RID: 142
		private static readonly Random random = new Random();

		// Token: 0x0400008F RID: 143
		private static readonly string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		// Token: 0x04000090 RID: 144
		private static readonly string[] extensions = new string[]
		{
			"",
			".txt",
			".png",
			".jpg",
			".html",
			".json",
			""
		};
	}
}
