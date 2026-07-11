using System;
using System.IO;

namespace Intelix.Helper
{
	// Token: 0x02000060 RID: 96
	public static class ParseKeyBlob
	{
		// Token: 0x06000160 RID: 352 RVA: 0x000114D4 File Offset: 0x0000F6D4
		public static BlobParsedData Parse(byte[] blobData)
		{
			BlobParsedData result;
			using (MemoryStream memoryStream = new MemoryStream(blobData))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					uint count = binaryReader.ReadUInt32();
					binaryReader.ReadBytes((int)count);
					uint num = binaryReader.ReadUInt32();
					long position = memoryStream.Position;
					byte[] iv = null;
					byte[] ciphertext = null;
					byte[] tag = null;
					bool flag = num == 32U;
					if (flag)
					{
						byte[] encryptedAesKey = binaryReader.ReadBytes(32);
						result = new BlobParsedData
						{
							Flag = 32,
							Iv = iv,
							Ciphertext = ciphertext,
							Tag = tag,
							EncryptedAesKey = encryptedAesKey
						};
					}
					else
					{
						byte b = binaryReader.ReadByte();
						byte b2 = b;
						byte b3 = b2;
						if (b3 - 1 > 1)
						{
							if (b3 != 3 && b3 != 35)
							{
								throw new Exception();
							}
							byte[] encryptedAesKey = binaryReader.ReadBytes(32);
							iv = binaryReader.ReadBytes(12);
							ciphertext = binaryReader.ReadBytes(32);
							tag = binaryReader.ReadBytes(16);
							result = new BlobParsedData
							{
								Flag = b,
								Iv = iv,
								Ciphertext = ciphertext,
								Tag = tag,
								EncryptedAesKey = encryptedAesKey
							};
						}
						else
						{
							iv = binaryReader.ReadBytes(12);
							ciphertext = binaryReader.ReadBytes(32);
							tag = binaryReader.ReadBytes(16);
							result = new BlobParsedData
							{
								Flag = b,
								Iv = iv,
								Ciphertext = ciphertext,
								Tag = tag,
								EncryptedAesKey = null
							};
						}
					}
				}
			}
			return result;
		}
	}
}
