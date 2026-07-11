using System;
using System.Security.Cryptography;

namespace Intelix.Helper.Hashing
{
	// Token: 0x0200006A RID: 106
	public class PBKDF2
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600018F RID: 399 RVA: 0x00013D60 File Offset: 0x00011F60
		private HMAC Algorithm { get; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000190 RID: 400 RVA: 0x00013D68 File Offset: 0x00011F68
		private byte[] Salt { get; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000191 RID: 401 RVA: 0x00013D70 File Offset: 0x00011F70
		private int IterationCount { get; }

		// Token: 0x06000192 RID: 402 RVA: 0x00013D78 File Offset: 0x00011F78
		public PBKDF2(HMAC algorithm, byte[] password, byte[] salt, int iterations)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm", "Algorithm cannot be null.");
			}
			this.Algorithm = algorithm;
			KeyedHashAlgorithm algorithm2 = this.Algorithm;
			if (password == null)
			{
				throw new ArgumentNullException("password", "Password cannot be null.");
			}
			algorithm2.Key = password;
			if (salt == null)
			{
				throw new ArgumentNullException("salt", "Salt cannot be null.");
			}
			this.Salt = salt;
			this.IterationCount = iterations;
			this._blockSize = this.Algorithm.HashSize / 8;
			this._bufferBytes = new byte[this._blockSize];
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00013E18 File Offset: 0x00012018
		public byte[] GetBytes(int count)
		{
			byte[] array = new byte[count];
			int i = 0;
			int num = this._bufferEndIndex - this._bufferStartIndex;
			bool flag = num > 0;
			if (flag)
			{
				bool flag2 = count < num;
				if (flag2)
				{
					Buffer.BlockCopy(this._bufferBytes, this._bufferStartIndex, array, 0, count);
					this._bufferStartIndex += count;
					return array;
				}
				Buffer.BlockCopy(this._bufferBytes, this._bufferStartIndex, array, 0, num);
				this._bufferStartIndex = (this._bufferEndIndex = 0);
				i += num;
			}
			while (i < count)
			{
				int num2 = count - i;
				this._bufferBytes = this.Func();
				bool flag3 = num2 > this._blockSize;
				if (!flag3)
				{
					Buffer.BlockCopy(this._bufferBytes, 0, array, i, num2);
					this._bufferStartIndex = num2;
					this._bufferEndIndex = this._blockSize;
					return array;
				}
				Buffer.BlockCopy(this._bufferBytes, 0, array, i, this._blockSize);
				i += this._blockSize;
			}
			return array;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00013F28 File Offset: 0x00012128
		private byte[] Func()
		{
			byte[] array = new byte[this.Salt.Length + 4];
			Buffer.BlockCopy(this.Salt, 0, array, 0, this.Salt.Length);
			Buffer.BlockCopy(PBKDF2.GetBytesFromInt(this._blockIndex), 0, array, this.Salt.Length, 4);
			byte[] array2 = this.Algorithm.ComputeHash(array);
			byte[] array3 = array2;
			for (int i = 2; i <= this.IterationCount; i++)
			{
				array2 = this.Algorithm.ComputeHash(array2, 0, array2.Length);
				for (int j = 0; j < this._blockSize; j++)
				{
					byte[] array4 = array3;
					int num = j;
					array4[num] ^= array2[j];
				}
			}
			bool flag = this._blockIndex == uint.MaxValue;
			if (flag)
			{
				throw new InvalidOperationException("Derived key too long.");
			}
			this._blockIndex += 1U;
			return array3;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00014014 File Offset: 0x00012214
		private static byte[] GetBytesFromInt(uint i)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			bool flag = !BitConverter.IsLittleEndian;
			byte[] result;
			if (flag)
			{
				result = bytes;
			}
			else
			{
				result = new byte[]
				{
					bytes[3],
					bytes[2],
					bytes[1],
					bytes[0]
				};
			}
			return result;
		}

		// Token: 0x04000043 RID: 67
		private readonly int _blockSize;

		// Token: 0x04000044 RID: 68
		private uint _blockIndex = 1U;

		// Token: 0x04000045 RID: 69
		private byte[] _bufferBytes;

		// Token: 0x04000046 RID: 70
		private int _bufferStartIndex;

		// Token: 0x04000047 RID: 71
		private int _bufferEndIndex;
	}
}
