using System;
using System.IO;
using System.Text;

namespace Intelix.Helper.Sql
{
	// Token: 0x02000068 RID: 104
	public class SqLite
	{
		// Token: 0x0600017C RID: 380 RVA: 0x00012C28 File Offset: 0x00010E28
		public SqLite(string fileName)
		{
			this._fileBytes = File.ReadAllBytes(fileName);
			this._pageSize = this.ConvertToULong(16, 2);
			this._dbEncoding = this.ConvertToULong(56, 4);
			this.ReadMasterTable(100L);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00012C8C File Offset: 0x00010E8C
		public SqLite(byte[] basedata)
		{
			this._fileBytes = basedata;
			this._pageSize = this.ConvertToULong(16, 2);
			this._dbEncoding = this.ConvertToULong(56, 4);
			this.ReadMasterTable(100L);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00012CE8 File Offset: 0x00010EE8
		public string GetValue(int rowNum, int field)
		{
			string result;
			try
			{
				bool flag = rowNum >= this._tableEntries.Length;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = ((field >= this._tableEntries[rowNum].Content.Length) ? null : this._tableEntries[rowNum].Content[field]);
				}
			}
			catch
			{
				result = "";
			}
			return result;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00012D58 File Offset: 0x00010F58
		public int GetRowCount()
		{
			return this._tableEntries.Length;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00012D74 File Offset: 0x00010F74
		private bool ReadTableFromOffset(ulong offset)
		{
			bool result;
			try
			{
				byte b = this._fileBytes[(int)(checked((IntPtr)offset))];
				byte b2 = b;
				if (b2 != 5)
				{
					if (b2 == 13)
					{
						uint num = (uint)(this.ConvertToULong((int)offset + 3, 2) - 1UL);
						int num2 = 0;
						bool flag = this._tableEntries != null;
						if (flag)
						{
							num2 = this._tableEntries.Length;
							Array.Resize<SqLite.TableEntry>(ref this._tableEntries, this._tableEntries.Length + (int)num + 1);
						}
						else
						{
							this._tableEntries = new SqLite.TableEntry[num + 1U];
						}
						for (uint num3 = 0U; num3 <= num; num3 += 1U)
						{
							ulong num4 = this.ConvertToULong((int)offset + 8 + (int)(num3 * 2U), 2);
							bool flag2 = offset != 100UL;
							if (flag2)
							{
								num4 += offset;
							}
							int num5 = this.Gvl((int)num4);
							this.Cvl((int)num4, num5);
							int num6 = this.Gvl((int)(num4 + (ulong)((long)num5 - (long)num4) + 1UL));
							this.Cvl((int)(num4 + (ulong)((long)num5 - (long)num4) + 1UL), num6);
							ulong num7 = num4 + (ulong)((long)num6 - (long)num4 + 1L);
							int num8 = this.Gvl((int)num7);
							int num9 = num8;
							long num10 = this.Cvl((int)num7, num8);
							SqLite.RecordHeaderField[] array = null;
							long num11 = (long)(num7 - (ulong)((long)num8) + 1UL);
							int num12 = 0;
							while (num11 < num10)
							{
								Array.Resize<SqLite.RecordHeaderField>(ref array, num12 + 1);
								int num13 = num9 + 1;
								num9 = this.Gvl(num13);
								array[num12].Type = this.Cvl(num13, num9);
								array[num12].Size = (long)((array[num12].Type <= 9L) ? ((ulong)this._sqlDataTypeSize[(int)(checked((IntPtr)array[num12].Type))]) : ((ulong)((!SqLite.IsOdd(array[num12].Type)) ? ((array[num12].Type - 12L) / 2L) : ((array[num12].Type - 13L) / 2L))));
								num11 = num11 + (long)(num9 - num13) + 1L;
								num12++;
							}
							bool flag3 = array == null;
							if (!flag3)
							{
								this._tableEntries[num2 + (int)num3].Content = new string[array.Length];
								int num14 = 0;
								for (int i = 0; i <= array.Length - 1; i++)
								{
									bool flag4 = array[i].Type > 9L;
									if (flag4)
									{
										bool flag5 = !SqLite.IsOdd(array[i].Type);
										if (flag5)
										{
											long dbEncoding = (long)this._dbEncoding;
											long num15 = dbEncoding - 1L;
											bool flag6 = num15 <= 2L;
											if (flag6)
											{
												long num16 = num15;
												long num17 = num16;
												long num18 = num17;
												if (num18 <= 2L)
												{
													switch ((uint)num18)
													{
													case 0U:
														this._tableEntries[num2 + (int)num3].Content[i] = Encoding.Default.GetString(this._fileBytes, (int)(num7 + (ulong)num10 + (ulong)((long)num14)), (int)array[i].Size);
														break;
													case 1U:
														this._tableEntries[num2 + (int)num3].Content[i] = Encoding.Unicode.GetString(this._fileBytes, (int)(num7 + (ulong)num10 + (ulong)((long)num14)), (int)array[i].Size);
														break;
													case 2U:
														this._tableEntries[num2 + (int)num3].Content[i] = Encoding.BigEndianUnicode.GetString(this._fileBytes, (int)(num7 + (ulong)num10 + (ulong)((long)num14)), (int)array[i].Size);
														break;
													}
												}
											}
										}
										else
										{
											this._tableEntries[num2 + (int)num3].Content[i] = Encoding.Default.GetString(this._fileBytes, (int)(num7 + (ulong)num10 + (ulong)((long)num14)), (int)array[i].Size);
										}
									}
									else
									{
										this._tableEntries[num2 + (int)num3].Content[i] = Convert.ToString(this.ConvertToULong((int)(num7 + (ulong)num10 + (ulong)((long)num14)), (int)array[i].Size));
									}
									num14 += (int)array[i].Size;
								}
							}
						}
					}
				}
				else
				{
					uint num19 = (uint)(this.ConvertToULong((int)(offset + 3UL), 2) - 1UL);
					for (uint num20 = 0U; num20 <= num19; num20 += 1U)
					{
						uint num21 = (uint)this.ConvertToULong((int)offset + 12 + (int)(num20 * 2U), 2);
						this.ReadTableFromOffset((this.ConvertToULong((int)(offset + (ulong)num21), 4) - 1UL) * this._pageSize);
					}
					this.ReadTableFromOffset((this.ConvertToULong((int)(offset + 8UL), 4) - 1UL) * this._pageSize);
				}
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00013290 File Offset: 0x00011490
		private void ReadMasterTable(long offset)
		{
			byte b2;
			for (;;)
			{
				byte b = this._fileBytes[(int)(checked((IntPtr)offset))];
				b2 = b;
				if (b2 != 5)
				{
					break;
				}
				uint num = (uint)(this.ConvertToULong((int)offset + 3, 2) - 1UL);
				for (int i = 0; i <= (int)num; i++)
				{
					uint num2 = (uint)this.ConvertToULong((int)offset + 12 + i * 2, 2);
					bool flag = offset == 100L;
					if (flag)
					{
						this.ReadMasterTable((long)((this.ConvertToULong((int)num2, 4) - 1UL) * this._pageSize));
					}
					else
					{
						this.ReadMasterTable((long)((this.ConvertToULong((int)(offset + (long)((ulong)num2)), 4) - 1UL) * this._pageSize));
					}
				}
				offset = (long)((this.ConvertToULong((int)offset + 8, 4) - 1UL) * this._pageSize);
			}
			if (b2 == 13)
			{
				ulong num3 = this.ConvertToULong((int)offset + 3, 2) - 1UL;
				int num4 = 0;
				bool flag2 = this._masterTableEntries != null;
				if (flag2)
				{
					num4 = this._masterTableEntries.Length;
					Array.Resize<SqLite.SqliteMasterEntry>(ref this._masterTableEntries, this._masterTableEntries.Length + (int)num3 + 1);
				}
				else
				{
					this._masterTableEntries = new SqLite.SqliteMasterEntry[num3 + 1UL];
				}
				for (ulong num5 = 0UL; num5 <= num3; num5 += 1UL)
				{
					ulong num6 = this.ConvertToULong((int)offset + 8 + (int)num5 * 2, 2);
					bool flag3 = offset != 100L;
					if (flag3)
					{
						num6 += (ulong)offset;
					}
					int num7 = this.Gvl((int)num6);
					this.Cvl((int)num6, num7);
					int num8 = this.Gvl((int)(num6 + (ulong)((long)num7 - (long)num6) + 1UL));
					this.Cvl((int)(num6 + (ulong)((long)num7 - (long)num6) + 1UL), num8);
					ulong num9 = num6 + (ulong)((long)num8 - (long)num6 + 1L);
					int num10 = this.Gvl((int)num9);
					int num11 = num10;
					long num12 = this.Cvl((int)num9, num10);
					long[] array = new long[5];
					for (int j = 0; j <= 4; j++)
					{
						int startIdx = num11 + 1;
						num11 = this.Gvl(startIdx);
						array[j] = this.Cvl(startIdx, num11);
						array[j] = (long)((array[j] <= 9L) ? ((ulong)this._sqlDataTypeSize[(int)(checked((IntPtr)array[j]))]) : ((ulong)((!SqLite.IsOdd(array[j])) ? ((array[j] - 12L) / 2L) : ((array[j] - 13L) / 2L))));
					}
					bool flag4 = this._dbEncoding == 1UL || this._dbEncoding == 2UL;
					long dbEncoding;
					if (flag4)
					{
						dbEncoding = (long)this._dbEncoding;
						long num13 = dbEncoding - 1L;
						bool flag5 = num13 <= 2L;
						if (flag5)
						{
							long num14 = num13;
							long num15 = num14;
							long num16 = num15;
							if (num16 <= 2L)
							{
								switch ((uint)num16)
								{
								case 0U:
									this._masterTableEntries[num4 + (int)num5].ItemName = Encoding.Default.GetString(this._fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0]), (int)array[1]);
									break;
								case 1U:
									this._masterTableEntries[num4 + (int)num5].ItemName = Encoding.Unicode.GetString(this._fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0]), (int)array[1]);
									break;
								case 2U:
									this._masterTableEntries[num4 + (int)num5].ItemName = Encoding.BigEndianUnicode.GetString(this._fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0]), (int)array[1]);
									break;
								}
							}
						}
					}
					this._masterTableEntries[num4 + (int)num5].RootNum = (long)this.ConvertToULong((int)(num9 + (ulong)num12 + (ulong)array[0] + (ulong)array[1] + (ulong)array[2]), (int)array[3]);
					dbEncoding = (long)this._dbEncoding;
					long num17 = dbEncoding - 1L;
					bool flag6 = num17 <= 2L;
					if (flag6)
					{
						long num18 = num17;
						long num19 = num18;
						long num20 = num19;
						if (num20 <= 2L)
						{
							switch ((uint)num20)
							{
							case 0U:
								this._masterTableEntries[num4 + (int)num5].SqlStatement = Encoding.Default.GetString(this._fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0] + (ulong)array[1] + (ulong)array[2] + (ulong)array[3]), (int)array[4]);
								break;
							case 1U:
								this._masterTableEntries[num4 + (int)num5].SqlStatement = Encoding.Unicode.GetString(this._fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0] + (ulong)array[1] + (ulong)array[2] + (ulong)array[3]), (int)array[4]);
								break;
							case 2U:
								this._masterTableEntries[num4 + (int)num5].SqlStatement = Encoding.BigEndianUnicode.GetString(this._fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0] + (ulong)array[1] + (ulong)array[2] + (ulong)array[3]), (int)array[4]);
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0001378C File Offset: 0x0001198C
		public bool ReadTable(string tableName)
		{
			int num = -1;
			for (int i = 0; i <= this._masterTableEntries.Length; i++)
			{
				bool flag = string.Compare(this._masterTableEntries[i].ItemName.ToLower(), tableName.ToLower(), StringComparison.Ordinal) == 0;
				if (flag)
				{
					num = i;
					break;
				}
			}
			bool flag2 = num == -1;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				string[] array = this._masterTableEntries[num].SqlStatement.Substring(this._masterTableEntries[num].SqlStatement.IndexOf("(", StringComparison.Ordinal) + 1).Split(new char[]
				{
					','
				});
				for (int j = 0; j <= array.Length - 1; j++)
				{
					array[j] = array[j].TrimStart(Array.Empty<char>());
					int num2 = array[j].IndexOf(' ');
					bool flag3 = num2 > 0;
					if (flag3)
					{
						array[j] = array[j].Substring(0, num2);
					}
					bool flag4 = array[j].IndexOf("UNIQUE", StringComparison.Ordinal) != 0;
					if (flag4)
					{
						Array.Resize<string>(ref this._fieldNames, j + 1);
						this._fieldNames[j] = array[j];
					}
				}
				result = this.ReadTableFromOffset((ulong)((this._masterTableEntries[num].RootNum - 1L) * (long)this._pageSize));
			}
			return result;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000138FC File Offset: 0x00011AFC
		private ulong ConvertToULong(int startIndex, int size)
		{
			ulong result;
			try
			{
				bool flag = size > 8 || size == 0;
				if (flag)
				{
					result = 0UL;
				}
				else
				{
					ulong num = 0UL;
					for (int i = 0; i <= size - 1; i++)
					{
						num = (num << 8 | (ulong)this._fileBytes[startIndex + i]);
					}
					result = num;
				}
			}
			catch
			{
				result = 0UL;
			}
			return result;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00013968 File Offset: 0x00011B68
		private int Gvl(int startIdx)
		{
			int result;
			try
			{
				bool flag = startIdx > this._fileBytes.Length;
				if (flag)
				{
					result = 0;
				}
				else
				{
					for (int i = startIdx; i <= startIdx + 8; i++)
					{
						bool flag2 = i > this._fileBytes.Length - 1;
						if (flag2)
						{
							return 0;
						}
						bool flag3 = (this._fileBytes[i] & 128) != 128;
						if (flag3)
						{
							return i;
						}
					}
					result = startIdx + 8;
				}
			}
			catch
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x000139FC File Offset: 0x00011BFC
		private long Cvl(int startIdx, int endIdx)
		{
			long result;
			try
			{
				endIdx++;
				byte[] array = new byte[8];
				int num = endIdx - startIdx;
				bool flag = false;
				bool flag2 = num == 0 || num > 9;
				if (flag2)
				{
					result = 0L;
				}
				else
				{
					int num2 = num;
					int num3 = num2;
					if (num3 != 1)
					{
						if (num3 == 9)
						{
							flag = true;
						}
						int num4 = 1;
						int num5 = 7;
						int num6 = 0;
						bool flag3 = flag;
						if (flag3)
						{
							array[0] = this._fileBytes[endIdx - 1];
							endIdx--;
							num6 = 1;
						}
						for (int i = endIdx - 1; i >= startIdx; i += -1)
						{
							bool flag4 = i - 1 >= startIdx;
							if (flag4)
							{
								array[num6] = (byte)((this._fileBytes[i] >> num4 - 1 & 255 >> num4) | (int)this._fileBytes[i - 1] << num5);
								num4++;
								num6++;
								num5--;
							}
							else
							{
								bool flag5 = !flag;
								if (flag5)
								{
									array[num6] = (byte)(this._fileBytes[i] >> num4 - 1 & 255 >> num4);
								}
							}
						}
						result = BitConverter.ToInt64(array, 0);
					}
					else
					{
						array[0] = (byte)(this._fileBytes[startIdx] & 127);
						result = BitConverter.ToInt64(array, 0);
					}
				}
			}
			catch
			{
				result = 0L;
			}
			return result;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00013B6C File Offset: 0x00011D6C
		private static bool IsOdd(long value)
		{
			return (value & 1L) == 1L;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00013B88 File Offset: 0x00011D88
		public static SqLite ReadTable(string database, string table)
		{
			SqLite result;
			try
			{
				string text = Path.GetTempFileName() + ".tmpdb";
				File.Copy(database, text);
				SqLite sqLite = new SqLite(text);
				sqLite.ReadTable(table);
				File.Delete(text);
				result = ((sqLite.GetRowCount() == 65536) ? null : sqLite);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04000037 RID: 55
		private readonly ulong _dbEncoding;

		// Token: 0x04000038 RID: 56
		private readonly byte[] _fileBytes;

		// Token: 0x04000039 RID: 57
		private readonly ulong _pageSize;

		// Token: 0x0400003A RID: 58
		private readonly byte[] _sqlDataTypeSize = new byte[]
		{
			0,
			1,
			2,
			3,
			4,
			6,
			8,
			8,
			0,
			0
		};

		// Token: 0x0400003B RID: 59
		private string[] _fieldNames;

		// Token: 0x0400003C RID: 60
		private SqLite.SqliteMasterEntry[] _masterTableEntries;

		// Token: 0x0400003D RID: 61
		private SqLite.TableEntry[] _tableEntries;

		// Token: 0x020000EF RID: 239
		private struct RecordHeaderField
		{
			// Token: 0x04000208 RID: 520
			public long Size;

			// Token: 0x04000209 RID: 521
			public long Type;
		}

		// Token: 0x020000F0 RID: 240
		private struct TableEntry
		{
			// Token: 0x0400020A RID: 522
			public string[] Content;
		}

		// Token: 0x020000F1 RID: 241
		private struct SqliteMasterEntry
		{
			// Token: 0x0400020B RID: 523
			public string ItemName;

			// Token: 0x0400020C RID: 524
			public long RootNum;

			// Token: 0x0400020D RID: 525
			public string SqlStatement;
		}
	}
}
