using System;

namespace Intelix.Helper
{
	// Token: 0x02000058 RID: 88
	public class BlobParsedData
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000122 RID: 290 RVA: 0x000108D7 File Offset: 0x0000EAD7
		// (set) Token: 0x06000123 RID: 291 RVA: 0x000108DF File Offset: 0x0000EADF
		public byte Flag { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000124 RID: 292 RVA: 0x000108E8 File Offset: 0x0000EAE8
		// (set) Token: 0x06000125 RID: 293 RVA: 0x000108F0 File Offset: 0x0000EAF0
		public byte[] Iv { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000126 RID: 294 RVA: 0x000108F9 File Offset: 0x0000EAF9
		// (set) Token: 0x06000127 RID: 295 RVA: 0x00010901 File Offset: 0x0000EB01
		public byte[] Ciphertext { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000128 RID: 296 RVA: 0x0001090A File Offset: 0x0000EB0A
		// (set) Token: 0x06000129 RID: 297 RVA: 0x00010912 File Offset: 0x0000EB12
		public byte[] Tag { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600012A RID: 298 RVA: 0x0001091B File Offset: 0x0000EB1B
		// (set) Token: 0x0600012B RID: 299 RVA: 0x00010923 File Offset: 0x0000EB23
		public byte[] EncryptedAesKey { get; set; }
	}
}
