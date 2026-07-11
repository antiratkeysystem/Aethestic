using System;
using System.Threading;

namespace Intelix.Helper
{
	// Token: 0x0200005E RID: 94
	public static class MutexControl
	{
		// Token: 0x06000143 RID: 323 RVA: 0x000114AC File Offset: 0x0000F6AC
		public static bool CreateMutex(string mtx)
		{
			bool createdNew;
			MutexControl.currentApp = new Mutex(false, mtx, out createdNew);
			MutexControl.createdNew = createdNew;
			return MutexControl.createdNew;
		}

		// Token: 0x0400002E RID: 46
		public static Mutex currentApp;

		// Token: 0x0400002F RID: 47
		public static bool createdNew;
	}
}
