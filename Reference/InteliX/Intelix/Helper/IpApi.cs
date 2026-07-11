using System;
using System.Net;

namespace Intelix.Helper
{
	// Token: 0x0200005D RID: 93
	public static class IpApi
	{
		// Token: 0x06000141 RID: 321 RVA: 0x000113B4 File Offset: 0x0000F5B4
		public static string GetPublicIp()
		{
			bool flag = !string.IsNullOrEmpty(IpApi._cachedIp);
			string cachedIp;
			if (flag)
			{
				cachedIp = IpApi._cachedIp;
			}
			else
			{
				object @lock = IpApi._lock;
				lock (@lock)
				{
					bool flag3 = !string.IsNullOrEmpty(IpApi._cachedIp);
					if (flag3)
					{
						cachedIp = IpApi._cachedIp;
					}
					else
					{
						try
						{
							using (WebClient webClient = new WebClient())
							{
								string text = webClient.DownloadString("http://icanhazip.com");
								bool flag4 = !string.IsNullOrEmpty(text);
								if (flag4)
								{
									IpApi._cachedIp = text.Trim();
								}
							}
						}
						catch
						{
							IpApi._cachedIp = "Request failed";
						}
						cachedIp = IpApi._cachedIp;
					}
				}
			}
			return cachedIp;
		}

		// Token: 0x0400002C RID: 44
		private static string _cachedIp;

		// Token: 0x0400002D RID: 45
		private static readonly object _lock = new object();
	}
}
