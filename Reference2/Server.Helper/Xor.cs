using System;

namespace Server.Helper;

internal class Xor
{
	public static byte[] DecodEncod(byte[] data, byte[] key)
	{
		int[] array = new int[256];
		for (int i = 0; i < 256; i++)
		{
			array[i] = i;
		}
		int[] array2 = new int[256];
		if (key.Length == 256)
		{
			Buffer.BlockCopy(key, 0, array2, 0, key.Length);
		}
		else
		{
			for (int j = 0; j < 256; j++)
			{
				array2[j] = key[j % key.Length];
			}
		}
		int num = 0;
		for (int k = 0; k < 256; k++)
		{
			num = (num + array[k] + array2[k]) % 256;
			int num2 = array[k];
			array[k] = array[num];
			array[num] = num2;
		}
		int num4;
		int num3 = (num4 = 0);
		byte[] array3 = new byte[data.Length];
		for (int l = 0; l < data.Length; l++)
		{
			num4 = (num4 + 1) % 256;
			num3 = (num3 + array[num4]) % 256;
			int num5 = array[num4];
			array[num4] = array[num3];
			array[num3] = num5;
			int num6 = array[(array[num4] + array[num3]) % 256];
			array3[l] = Convert.ToByte(data[l] ^ num6);
		}
		return array3;
	}
}
