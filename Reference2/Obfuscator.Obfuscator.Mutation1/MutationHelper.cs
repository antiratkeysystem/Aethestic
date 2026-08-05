using System;
using System.Security.Cryptography;

namespace Obfuscator.Obfuscator.Mutation1;

internal class MutationHelper
{
	public sealed class CryptoRandom : Random, IDisposable
	{
		private RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();

		private byte[] uint32Buffer = new byte[4];

		public CryptoRandom()
		{
		}

		public CryptoRandom(int seedIgnored)
		{
		}

		public override int Next()
		{
			cryptoProvider.GetBytes(uint32Buffer);
			return BitConverter.ToInt32(uint32Buffer, 0) & 0x7FFFFFFF;
		}

		public override int Next(int maxValue)
		{
			if (maxValue < 0)
			{
				throw new ArgumentOutOfRangeException("maxValue");
			}
			return Next(0, maxValue);
		}

		public override int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue)
			{
				throw new ArgumentOutOfRangeException("minValue");
			}
			if (minValue == maxValue)
			{
				return minValue;
			}
			long num = maxValue - minValue;
			long num2 = 4294967296L;
			long num3 = num2 % num;
			uint num4;
			do
			{
				cryptoProvider.GetBytes(uint32Buffer);
				num4 = BitConverter.ToUInt32(uint32Buffer, 0);
			}
			while ((ulong)num4 >= (ulong)(num2 - num3));
			return (int)((long)minValue + (long)((ulong)num4 % (ulong)num));
		}

		public override double NextDouble()
		{
			cryptoProvider.GetBytes(uint32Buffer);
			return (double)BitConverter.ToUInt32(uint32Buffer, 0) / 4294967296.0;
		}

		public override void NextBytes(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			cryptoProvider.GetBytes(buffer);
		}

		public void Dispose()
		{
			InternalDispose();
		}

		~CryptoRandom()
		{
			InternalDispose();
		}

		private void InternalDispose()
		{
			if (cryptoProvider != null)
			{
				cryptoProvider.Dispose();
				cryptoProvider = null;
			}
		}
	}
}
