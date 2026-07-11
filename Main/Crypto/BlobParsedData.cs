using System;

namespace Stealer.Crypto
{
    public class BlobParsedData
    {
        public byte Flag { get; set; }
        public byte[] Iv { get; set; }
        public byte[] Ciphertext { get; set; }
        public byte[] Tag { get; set; }
        public byte[] EncryptedAesKey { get; set; }
    }
}
