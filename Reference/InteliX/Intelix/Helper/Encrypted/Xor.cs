using System;
using System.Text;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x0200007A RID: 122
	public static class Xor
	{
		// Token: 0x060001EC RID: 492 RVA: 0x000182FC File Offset: 0x000164FC
		public static string DecryptString(string input, byte key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			for (int i = 0; i < bytes.Length; i++)
			{
				byte[] array = bytes;
				int num = i;
				array[num] ^= key;
			}
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
