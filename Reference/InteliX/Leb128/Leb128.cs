using System;
using System.Collections.Generic;
using System.IO;

namespace Leb128
{
	// Token: 0x02000004 RID: 4
	public class Leb128
	{
		// Token: 0x06000003 RID: 3 RVA: 0x0000206C File Offset: 0x0000026C
		public static object[] Read(byte[] data)
		{
			List<object> list = new List<object>();
			object[] result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				int num;
				for (;;)
				{
					num = memoryStream.ReadByte();
					switch (num)
					{
					case -1:
						goto IL_175;
					case 0:
						list.Add(Leb128Coding.ReadLebString(memoryStream));
						continue;
					case 1:
						list.Add(Leb128Coding.ReadLebBool(memoryStream));
						continue;
					case 2:
						list.Add(Leb128Coding.ReadLebByte(memoryStream));
						continue;
					case 3:
						list.Add(Leb128Coding.ReadLebShort(memoryStream));
						continue;
					case 4:
						list.Add(Leb128Coding.ReadLebInt(memoryStream));
						continue;
					case 5:
						list.Add(Leb128Coding.ReadLebLong(memoryStream));
						continue;
					case 6:
						list.Add(Leb128Coding.ReadLebFloat(memoryStream));
						continue;
					case 7:
						list.Add(Leb128Coding.ReadLebDouble(memoryStream));
						continue;
					case 8:
						list.Add(Leb128Coding.ReadLebArray(memoryStream));
						continue;
					case 9:
						list.Add(Leb128Coding.ReadLebUshort(memoryStream));
						continue;
					case 10:
						list.Add(Leb128Coding.ReadLebUint(memoryStream));
						continue;
					case 11:
						list.Add(Leb128Coding.ReadLebUlong(memoryStream));
						continue;
					case 12:
						list.Add(Leb128.Read(Leb128Coding.ReadLebArray(memoryStream)));
						continue;
					}
					break;
				}
				throw new Exception(num.ToString());
				IL_175:
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002238 File Offset: 0x00000438
		public static byte[] Write(object[] data)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				foreach (object obj in data)
				{
					bool flag = obj is string;
					if (flag)
					{
						memoryStream.WriteByte(0);
						Leb128Coding.WriteLeb(memoryStream, (string)obj);
					}
					else
					{
						bool flag2 = obj is bool;
						if (flag2)
						{
							memoryStream.WriteByte(1);
							Leb128Coding.WriteLeb(memoryStream, (bool)obj);
						}
						else
						{
							bool flag3 = obj is byte;
							if (flag3)
							{
								memoryStream.WriteByte(2);
								Leb128Coding.WriteLeb(memoryStream, (byte)obj);
							}
							else
							{
								bool flag4 = obj is short;
								if (flag4)
								{
									memoryStream.WriteByte(3);
									Leb128Coding.WriteLeb(memoryStream, (short)obj);
								}
								else
								{
									bool flag5 = obj is int;
									if (flag5)
									{
										memoryStream.WriteByte(4);
										Leb128Coding.WriteLeb(memoryStream, (int)obj);
									}
									else
									{
										bool flag6 = obj is long;
										if (flag6)
										{
											memoryStream.WriteByte(5);
											Leb128Coding.WriteLeb(memoryStream, (long)obj);
										}
										else
										{
											bool flag7 = obj is float;
											if (flag7)
											{
												memoryStream.WriteByte(6);
												Leb128Coding.WriteLeb(memoryStream, (float)obj);
											}
											else
											{
												bool flag8 = obj is double;
												if (flag8)
												{
													memoryStream.WriteByte(7);
													Leb128Coding.WriteLeb(memoryStream, (double)obj);
												}
												else
												{
													bool flag9 = obj is byte[];
													if (flag9)
													{
														memoryStream.WriteByte(8);
														Leb128Coding.WriteLeb(memoryStream, (byte[])obj);
													}
													else
													{
														bool flag10 = obj is ushort;
														if (flag10)
														{
															memoryStream.WriteByte(9);
															Leb128Coding.WriteLeb(memoryStream, (ushort)obj);
														}
														else
														{
															bool flag11 = obj is uint;
															if (flag11)
															{
																memoryStream.WriteByte(10);
																Leb128Coding.WriteLeb(memoryStream, (uint)obj);
															}
															else
															{
																bool flag12 = obj is ulong;
																if (flag12)
																{
																	memoryStream.WriteByte(11);
																	Leb128Coding.WriteLeb(memoryStream, (ulong)obj);
																}
																else
																{
																	bool flag13 = obj is object[];
																	if (!flag13)
																	{
																		throw new Exception(obj.GetType().Name);
																	}
																	memoryStream.WriteByte(12);
																	Leb128Coding.WriteLeb(memoryStream, Leb128.Write((object[])obj));
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				result = memoryStream.ToArray();
			}
			return result;
		}
	}
}
