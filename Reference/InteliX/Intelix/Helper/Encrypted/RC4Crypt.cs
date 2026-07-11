using System;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x02000078 RID: 120
	public class RC4Crypt
	{
		// Token: 0x060001DB RID: 475 RVA: 0x00017A60 File Offset: 0x00015C60
		public static byte[] Decrypt(byte[] key, byte[] data)
		{
			bool flag = key == null;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = key.Length == 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = data == null;
					if (flag3)
					{
						result = null;
					}
					else
					{
						byte[] array = new byte[256];
						for (int i = 0; i < 256; i++)
						{
							array[i] = (byte)i;
						}
						int num = 0;
						for (int j = 0; j < 256; j++)
						{
							num = (num + (int)array[j] + (int)key[j % key.Length] & 255);
							RC4Crypt.Swap(array, j, num);
						}
						byte[] array2 = new byte[data.Length];
						int num2 = 0;
						num = 0;
						for (int k = 0; k < data.Length; k++)
						{
							num2 = (num2 + 1 & 255);
							num = (num + (int)array[num2] & 255);
							RC4Crypt.Swap(array, num2, num);
							byte b = array[(int)(array[num2] + array[num] & byte.MaxValue)];
							array2[k] = (byte)(data[k] ^ b);
						}
						result = array2;
					}
				}
			}
			return result;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00017B80 File Offset: 0x00015D80
		private static void Swap(byte[] arr, int a, int b)
		{
			byte b2 = arr[a];
			arr[a] = arr[b];
			arr[b] = b2;
		}
	}
}
