using System;
using System.Threading;

namespace CvMega.Helper
{
	// Token: 0x02000084 RID: 132
	public static class MutexControl
	{
		// Token: 0x06000216 RID: 534 RVA: 0x0001ADDC File Offset: 0x00018FDC
		public static bool CreateMutex(string mtx)
		{
			bool createdNew;
			MutexControl.currentApp = new Mutex(false, mtx, out createdNew);
			MutexControl.createdNew = createdNew;
			return MutexControl.createdNew;
		}

		// Token: 0x0400008C RID: 140
		public static Mutex currentApp;

		// Token: 0x0400008D RID: 141
		public static bool createdNew;
	}
}
