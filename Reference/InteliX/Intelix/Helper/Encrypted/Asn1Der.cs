using System;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x0200006D RID: 109
	public class Asn1Der
	{
		// Token: 0x060001A7 RID: 423 RVA: 0x00014E28 File Offset: 0x00013028
		public Asn1DerObject Parse(byte[] toParse)
		{
			Asn1DerObject asn1DerObject = new Asn1DerObject();
			for (int i = 0; i < toParse.Length; i++)
			{
				Asn1Der.Type type = (Asn1Der.Type)toParse[i];
				Asn1Der.Type type2 = type;
				switch (type2)
				{
				case Asn1Der.Type.Integer:
				{
					asn1DerObject.Objects.Add(new Asn1DerObject
					{
						Type = Asn1Der.Type.Integer,
						Lenght = (int)toParse[i + 1]
					});
					byte[] array = new byte[(int)toParse[i + 1]];
					int length = (i + 2 + (int)toParse[i + 1] > toParse.Length) ? (toParse.Length - (i + 2)) : ((int)toParse[i + 1]);
					Array.Copy(toParse, i + 2, array, 0, length);
					Asn1DerObject[] array2 = asn1DerObject.Objects.ToArray();
					asn1DerObject.Objects[array2.Length - 1].Data = array;
					i = i + 1 + asn1DerObject.Objects[array2.Length - 1].Lenght;
					break;
				}
				case (Asn1Der.Type)3:
				case (Asn1Der.Type)5:
					break;
				case Asn1Der.Type.OctetString:
				{
					asn1DerObject.Objects.Add(new Asn1DerObject
					{
						Type = Asn1Der.Type.OctetString,
						Lenght = (int)toParse[i + 1]
					});
					byte[] array3 = new byte[(int)toParse[i + 1]];
					int length2 = (i + 2 + (int)toParse[i + 1] > toParse.Length) ? (toParse.Length - (i + 2)) : ((int)toParse[i + 1]);
					Array.Copy(toParse, i + 2, array3, 0, length2);
					Asn1DerObject[] array4 = asn1DerObject.Objects.ToArray();
					asn1DerObject.Objects[array4.Length - 1].Data = array3;
					i = i + 1 + asn1DerObject.Objects[array4.Length - 1].Lenght;
					break;
				}
				case Asn1Der.Type.ObjectIdentifier:
				{
					asn1DerObject.Objects.Add(new Asn1DerObject
					{
						Type = Asn1Der.Type.ObjectIdentifier,
						Lenght = (int)toParse[i + 1]
					});
					byte[] array5 = new byte[(int)toParse[i + 1]];
					int length3 = (i + 2 + (int)toParse[i + 1] > toParse.Length) ? (toParse.Length - (i + 2)) : ((int)toParse[i + 1]);
					Array.Copy(toParse, i + 2, array5, 0, length3);
					Asn1DerObject[] array6 = asn1DerObject.Objects.ToArray();
					asn1DerObject.Objects[array6.Length - 1].Data = array5;
					i = i + 1 + asn1DerObject.Objects[array6.Length - 1].Lenght;
					break;
				}
				default:
					if (type2 == Asn1Der.Type.Sequence)
					{
						bool flag = asn1DerObject.Lenght == 0;
						byte[] array7;
						if (flag)
						{
							asn1DerObject.Type = Asn1Der.Type.Sequence;
							asn1DerObject.Lenght = toParse.Length - (i + 2);
							array7 = new byte[asn1DerObject.Lenght];
						}
						else
						{
							asn1DerObject.Objects.Add(new Asn1DerObject
							{
								Type = Asn1Der.Type.Sequence,
								Lenght = (int)toParse[i + 1]
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

		// Token: 0x020000F2 RID: 242
		public enum Type
		{
			// Token: 0x0400020F RID: 527
			Sequence = 48,
			// Token: 0x04000210 RID: 528
			Integer = 2,
			// Token: 0x04000211 RID: 529
			OctetString = 4,
			// Token: 0x04000212 RID: 530
			ObjectIdentifier = 6
		}
	}
}
