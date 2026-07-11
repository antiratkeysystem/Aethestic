using System;
using Intelix.Helper.Data;

namespace Intelix.Targets
{
	// Token: 0x02000006 RID: 6
	public interface ITarget
	{
		// Token: 0x0600001F RID: 31
		void Collect(InMemoryZip zip, Counter counter);
	}
}
