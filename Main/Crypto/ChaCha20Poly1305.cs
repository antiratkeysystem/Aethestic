using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Stealer.Crypto
{
    public static class ChaCha20Poly1305
    {
        public static byte[] Decrypt(byte[] key32, byte[] iv12, byte[] ciphertext, byte[] tag, byte[] aad = null)
        {
            if (key32 == null)
                throw new ArgumentNullException("key32");
            if (key32.Length != 32)
                throw new ArgumentException("Key must be 32 bytes", "key32");
            if (iv12 == null)
                throw new ArgumentNullException("iv12");
            if (iv12.Length != 12)
                throw new ArgumentException("IV must be 12 bytes", "iv12");
            if (ciphertext == null)
                throw new ArgumentNullException("ciphertext");
            if (tag == null)
                throw new ArgumentNullException("tag");
            if (tag.Length != 16)
                throw new ArgumentException("Tag must be 16 bytes", "tag");
            if (aad == null)
                aad = Array.Empty<byte>();

            byte[] array = ChaCha20Block(key32, 0U, iv12);
            byte[] array2 = new byte[32];
            Buffer.BlockCopy(array, 0, array2, 0, 32);
            byte[] msg = BuildPoly1305Message(aad, ciphertext);
            
            if (!FixedTimeEquals(Poly1305TagWithBigInteger(array2, msg), tag))
            {
                Array.Clear(array, 0, array.Length);
                Array.Clear(array2, 0, array2.Length);
                throw new CryptographicException("ChaCha20-Poly1305 authentication failed (tag mismatch).");
            }
            
            byte[] array3 = new byte[ciphertext.Length];
            ChaCha20Xor(key32, 1U, iv12, ciphertext, array3);
            Array.Clear(array, 0, array.Length);
            Array.Clear(array2, 0, array2.Length);
            return array3;
        }

        private static byte[] ChaCha20Block(byte[] key32, uint counter, byte[] nonce12)
        {
            uint[] array = new uint[]
            {
                1634760805U, 857760878U, 2036477234U, 1797285236U,
                0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U,
                0U, 0U, 0U, 0U
            };
            
            for (int i = 0; i < 8; i++)
            {
                array[4 + i] = ToUInt32Little(key32, i * 4);
            }
            
            array[12] = counter;
            array[13] = ToUInt32Little(nonce12, 0);
            array[14] = ToUInt32Little(nonce12, 4);
            array[15] = ToUInt32Little(nonce12, 8);
            
            uint[] array2 = new uint[16];
            Array.Copy(array, array2, 16);
            
            for (int j = 0; j < 10; j++)
            {
                QuarterRound(ref array2[0], ref array2[4], ref array2[8], ref array2[12]);
                QuarterRound(ref array2[1], ref array2[5], ref array2[9], ref array2[13]);
                QuarterRound(ref array2[2], ref array2[6], ref array2[10], ref array2[14]);
                QuarterRound(ref array2[3], ref array2[7], ref array2[11], ref array2[15]);
                QuarterRound(ref array2[0], ref array2[5], ref array2[10], ref array2[15]);
                QuarterRound(ref array2[1], ref array2[6], ref array2[11], ref array2[12]);
                QuarterRound(ref array2[2], ref array2[7], ref array2[8], ref array2[13]);
                QuarterRound(ref array2[3], ref array2[4], ref array2[9], ref array2[14]);
            }
            
            byte[] array3 = new byte[64];
            for (int k = 0; k < 16; k++)
            {
                LittleEndian(array2[k] + array[k], array3, k * 4);
            }
            return array3;
        }

        private static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d)
        {
            a += b;
            d ^= a;
            d = Rol(d, 16);
            c += d;
            b ^= c;
            b = Rol(b, 12);
            a += b;
            d ^= a;
            d = Rol(d, 8);
            c += d;
            b ^= c;
            b = Rol(b, 7);
        }

        private static uint Rol(uint x, int n)
        {
            return x << n | x >> 32 - n;
        }

        private static uint ToUInt32Little(byte[] bs, int off)
        {
            return (uint)((int)bs[off] | (int)bs[off + 1] << 8 | (int)bs[off + 2] << 16 | (int)bs[off + 3] << 24);
        }

        private static void LittleEndian(uint v, byte[] outbuf, int off)
        {
            outbuf[off] = (byte)(v & 255U);
            outbuf[off + 1] = (byte)(v >> 8 & 255U);
            outbuf[off + 2] = (byte)(v >> 16 & 255U);
            outbuf[off + 3] = (byte)(v >> 24 & 255U);
        }

        private static void ChaCha20Xor(byte[] key, uint counter, byte[] nonce, byte[] input, byte[] output)
        {
            if (input == null || input.Length == 0)
                return;

            int i = 0;
            uint num = counter;
            while (i < input.Length)
            {
                byte[] array = ChaCha20Block(key, num, nonce);
                num += 1U;
                int num2 = Math.Min(64, input.Length - i);
                for (int j = 0; j < num2; j++)
                {
                    output[i + j] = (byte)(input[i + j] ^ array[j]);
                }
                i += num2;
            }
        }

        private static byte[] BuildPoly1305Message(byte[] aad, byte[] ciphertext)
        {
            int num = (aad != null) ? aad.Length : 0;
            int num2 = (ciphertext != null) ? ciphertext.Length : 0;
            int num3 = (16 - num % 16) % 16;
            int num4 = (16 - num2 % 16) % 16;
            byte[] array = new byte[num + num3 + num2 + num4 + 8 + 8];
            int num5 = 0;
            
            if (num > 0)
            {
                Buffer.BlockCopy(aad, 0, array, num5, num);
                num5 += num;
            }
            if (num3 > 0)
            {
                num5 += num3;
            }
            if (num2 > 0)
            {
                Buffer.BlockCopy(ciphertext, 0, array, num5, num2);
                num5 += num2;
            }
            if (num4 > 0)
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

        private static byte[] Poly1305TagWithBigInteger(byte[] oneTimeKey32, byte[] msg)
        {
            byte[] array = new byte[16];
            Buffer.BlockCopy(oneTimeKey32, 0, array, 0, 16);
            array[3] &= 15;
            array[7] &= 15;
            array[11] &= 15;
            array[15] &= 15;
            array[4] &= 252;
            array[8] &= 252;
            array[12] &= 252;
            
            byte[] array9 = new byte[16];
            Buffer.BlockCopy(oneTimeKey32, 16, array9, 0, 16);
            
            BigInteger right = new BigInteger(AppendZero(array));
            BigInteger right2 = new BigInteger(AppendZero(array9));
            BigInteger divisor = (BigInteger.One << 130) - 5;
            BigInteger left = BigInteger.Zero;
            
            for (int i = 0; i < msg.Length; i += 16)
            {
                int num8 = Math.Min(16, msg.Length - i);
                byte[] array10 = new byte[num8];
                Buffer.BlockCopy(msg, i, array10, 0, num8);
                BigInteger bigInteger = new BigInteger(AppendZero(array10));
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

        private static byte[] AppendZero(byte[] b)
        {
            byte[] array = new byte[b.Length + 1];
            Buffer.BlockCopy(b, 0, array, 0, b.Length);
            array[array.Length - 1] = 0;
            return array;
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int num = 0;
            for (int i = 0; i < a.Length; i++)
            {
                num |= (int)(a[i] ^ b[i]);
            }
            return (num == 0);
        }
    }
}
