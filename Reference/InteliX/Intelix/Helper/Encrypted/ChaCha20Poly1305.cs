using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x02000070 RID: 112
	public static class ChaCha20Poly1305
	{
		// Token: 0x060001B6 RID: 438 RVA: 0x00015A48 File Offset: 0x00013C48
		public static byte[] Decrypt(byte[] key32, byte[] iv12, byte[] ciphertext, byte[] tag, byte[] aad = null)
		{
			bool flag = key32 == null;
			if (flag)
			{
				throw new ArgumentNullException("key32");
			}
			bool flag2 = key32.Length != 32;
			if (flag2)
			{
				throw new ArgumentException("Key must be 32 bytes", "key32");
			}
			bool flag3 = iv12 == null;
			if (flag3)
			{
				throw new ArgumentNullException("iv12");
			}
			bool flag4 = iv12.Length != 12;
			if (flag4)
			{
				throw new ArgumentException("IV must be 12 bytes", "iv12");
			}
			bool flag5 = ciphertext == null;
			if (flag5)
			{
				throw new ArgumentNullException("ciphertext");
			}
			bool flag6 = tag == null;
			if (flag6)
			{
				throw new ArgumentNullException("tag");
			}
			bool flag7 = tag.Length != 16;
			if (flag7)
			{
				throw new ArgumentException("Tag must be 16 bytes", "tag");
			}
			bool flag8 = aad == null;
			if (flag8)
			{
				aad = Array.Empty<byte>();
			}
			byte[] array = ChaCha20Poly1305.ChaCha20Block(key32, 0U, iv12);
			byte[] array2 = new byte[32];
			Buffer.BlockCopy(array, 0, array2, 0, 32);
			byte[] msg = ChaCha20Poly1305.BuildPoly1305Message(aad, ciphertext);
			bool flag9 = !ChaCha20Poly1305.FixedTimeEquals(ChaCha20Poly1305.Poly1305TagWithBigInteger(array2, msg), tag);
			if (flag9)
			{
				Array.Clear(array, 0, array.Length);
				Array.Clear(array2, 0, array2.Length);
				throw new CryptographicException("ChaCha20-Poly1305 authentication failed (tag mismatch).");
			}
			byte[] array3 = new byte[ciphertext.Length];
			ChaCha20Poly1305.ChaCha20Xor(key32, 1U, iv12, ciphertext, array3);
			Array.Clear(array, 0, array.Length);
			Array.Clear(array2, 0, array2.Length);
			return array3;
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00015BB4 File Offset: 0x00013DB4
		private static byte[] ChaCha20Block(byte[] key32, uint counter, byte[] nonce12)
		{
			uint[] array = new uint[]
			{
				1634760805U,
				857760878U,
				2036477234U,
				1797285236U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U
			};
			for (int i = 0; i < 8; i++)
			{
				array[4 + i] = ChaCha20Poly1305.ToUInt32Little(key32, i * 4);
			}
			array[12] = counter;
			array[13] = ChaCha20Poly1305.ToUInt32Little(nonce12, 0);
			array[14] = ChaCha20Poly1305.ToUInt32Little(nonce12, 4);
			array[15] = ChaCha20Poly1305.ToUInt32Little(nonce12, 8);
			uint[] array2 = new uint[16];
			Array.Copy(array, array2, 16);
			for (int j = 0; j < 10; j++)
			{
				ChaCha20Poly1305.QuarterRound(ref array2[0], ref array2[4], ref array2[8], ref array2[12]);
				ChaCha20Poly1305.QuarterRound(ref array2[1], ref array2[5], ref array2[9], ref array2[13]);
				ChaCha20Poly1305.QuarterRound(ref array2[2], ref array2[6], ref array2[10], ref array2[14]);
				ChaCha20Poly1305.QuarterRound(ref array2[3], ref array2[7], ref array2[11], ref array2[15]);
				ChaCha20Poly1305.QuarterRound(ref array2[0], ref array2[5], ref array2[10], ref array2[15]);
				ChaCha20Poly1305.QuarterRound(ref array2[1], ref array2[6], ref array2[11], ref array2[12]);
				ChaCha20Poly1305.QuarterRound(ref array2[2], ref array2[7], ref array2[8], ref array2[13]);
				ChaCha20Poly1305.QuarterRound(ref array2[3], ref array2[4], ref array2[9], ref array2[14]);
			}
			byte[] array3 = new byte[64];
			for (int k = 0; k < 16; k++)
			{
				ChaCha20Poly1305.LittleEndian(array2[k] + array[k], array3, k * 4);
			}
			return array3;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00015DA8 File Offset: 0x00013FA8
		private static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d)
		{
			a += b;
			d ^= a;
			d = ChaCha20Poly1305.Rol(d, 16);
			c += d;
			b ^= c;
			b = ChaCha20Poly1305.Rol(b, 12);
			a += b;
			d ^= a;
			d = ChaCha20Poly1305.Rol(d, 8);
			c += d;
			b ^= c;
			b = ChaCha20Poly1305.Rol(b, 7);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00015E18 File Offset: 0x00014018
		private static uint Rol(uint x, int n)
		{
			return x << n | x >> 32 - n;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00015E3C File Offset: 0x0001403C
		private static uint ToUInt32Little(byte[] bs, int off)
		{
			return (uint)((int)bs[off] | (int)bs[off + 1] << 8 | (int)bs[off + 2] << 16 | (int)bs[off + 3] << 24);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00015E6B File Offset: 0x0001406B
		private static void LittleEndian(uint v, byte[] outbuf, int off)
		{
			outbuf[off] = (byte)(v & 255U);
			outbuf[off + 1] = (byte)(v >> 8 & 255U);
			outbuf[off + 2] = (byte)(v >> 16 & 255U);
			outbuf[off + 3] = (byte)(v >> 24 & 255U);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00015EA8 File Offset: 0x000140A8
		private static void ChaCha20Xor(byte[] key, uint counter, byte[] nonce, byte[] input, byte[] output)
		{
			bool flag = input == null || input.Length == 0;
			if (!flag)
			{
				int i = 0;
				uint num = counter;
				while (i < input.Length)
				{
					byte[] array = ChaCha20Poly1305.ChaCha20Block(key, num, nonce);
					num += 1U;
					int num2 = Math.Min(64, input.Length - i);
					for (int j = 0; j < num2; j++)
					{
						output[i + j] = (byte)(input[i + j] ^ array[j]);
					}
					i += num2;
				}
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00015F28 File Offset: 0x00014128
		private static byte[] BuildPoly1305Message(byte[] aad, byte[] ciphertext)
		{
			int num = (aad != null) ? aad.Length : 0;
			int num2 = (ciphertext != null) ? ciphertext.Length : 0;
			int num3 = (16 - num % 16) % 16;
			int num4 = (16 - num2 % 16) % 16;
			byte[] array = new byte[num + num3 + num2 + num4 + 8 + 8];
			int num5 = 0;
			bool flag = num > 0;
			if (flag)
			{
				Buffer.BlockCopy(aad, 0, array, num5, num);
				num5 += num;
			}
			bool flag2 = num3 > 0;
			if (flag2)
			{
				num5 += num3;
			}
			bool flag3 = num2 > 0;
			if (flag3)
			{
				Buffer.BlockCopy(ciphertext, 0, array, num5, num2);
				num5 += num2;
			}
			bool flag4 = num4 > 0;
			if (flag4)
			{
				num5 += num4;
			}
			byte[] bytes = BitConverter.GetBytes((ulong)((long)num));
			byte[] bytes2 = BitConverter.GetBytes((ulong)((long)num2));
			Buffer.BlockCopy(bytes, 0, array, num5, 8);
			num5 += 8;
			Buffer.BlockCopy(bytes2, 0, array, num5, 8);
			num5 += 8;
			return array;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0001601C File Offset: 0x0001421C
		private static byte[] Poly1305TagWithBigInteger(byte[] oneTimeKey32, byte[] msg)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(oneTimeKey32, 0, array, 0, 16);
			byte[] array2 = array;
			int num = 3;
			array2[num] &= 15;
			byte[] array3 = array;
			int num2 = 7;
			array3[num2] &= 15;
			byte[] array4 = array;
			int num3 = 11;
			array4[num3] &= 15;
			byte[] array5 = array;
			int num4 = 15;
			array5[num4] &= 15;
			byte[] array6 = array;
			int num5 = 4;
			array6[num5] &= 252;
			byte[] array7 = array;
			int num6 = 8;
			array7[num6] &= 252;
			byte[] array8 = array;
			int num7 = 12;
			array8[num7] &= 252;
			byte[] array9 = new byte[16];
			Buffer.BlockCopy(oneTimeKey32, 16, array9, 0, 16);
			BigInteger right = new BigInteger(ChaCha20Poly1305.AppendZero(array));
			BigInteger right2 = new BigInteger(ChaCha20Poly1305.AppendZero(array9));
			BigInteger divisor = (BigInteger.One << 130) - 5;
			BigInteger left = BigInteger.Zero;
			for (int i = 0; i < msg.Length; i += 16)
			{
				int num8 = Math.Min(16, msg.Length - i);
				byte[] array10 = new byte[num8];
				Buffer.BlockCopy(msg, i, array10, 0, num8);
				BigInteger bigInteger = new BigInteger(ChaCha20Poly1305.AppendZero(array10));
				BigInteger right3 = BigInteger.One << 8 * num8;
				bigInteger += right3;
				left += bigInteger;
				left = left * right % divisor;
			}
			byte[] array11 = (left + right2).ToByteArray();
			byte[] array12 = new byte[16];
			int num9 = 0;
			while (num9 < 16 && num9 < array11.Length)
			{
				array12[num9] = array11[num9];
				num9++;
			}
			return array12;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x000161DC File Offset: 0x000143DC
		private static byte[] AppendZero(byte[] b)
		{
			byte[] array = new byte[b.Length + 1];
			Buffer.BlockCopy(b, 0, array, 0, b.Length);
			array[array.Length - 1] = 0;
			return array;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00016210 File Offset: 0x00014410
		private static bool FixedTimeEquals(byte[] a, byte[] b)
		{
			bool flag = a == null || b == null || a.Length != b.Length;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < a.Length; i++)
				{
					num |= (int)(a[i] ^ b[i]);
				}
				result = (num == 0);
			}
			return result;
		}
	}
}
