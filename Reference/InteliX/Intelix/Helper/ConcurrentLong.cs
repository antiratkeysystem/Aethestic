using System;
using System.Threading;

namespace Intelix.Helper
{
	// Token: 0x02000059 RID: 89
	public struct ConcurrentLong
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600012D RID: 301 RVA: 0x00010938 File Offset: 0x0000EB38
		// (set) Token: 0x0600012E RID: 302 RVA: 0x00010955 File Offset: 0x0000EB55
		public long Value
		{
			get
			{
				return Interlocked.Read(ref this._value);
			}
			set
			{
				Interlocked.Exchange(ref this._value, value);
			}
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00010965 File Offset: 0x0000EB65
		public ConcurrentLong(long initial)
		{
			this._value = initial;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00010970 File Offset: 0x0000EB70
		public static ConcurrentLong operator ++(ConcurrentLong x)
		{
			Interlocked.Increment(ref x._value);
			return x;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00010990 File Offset: 0x0000EB90
		public static ConcurrentLong operator --(ConcurrentLong x)
		{
			Interlocked.Decrement(ref x._value);
			return x;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000109B0 File Offset: 0x0000EBB0
		public static implicit operator long(ConcurrentLong x)
		{
			return x.Value;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000109CC File Offset: 0x0000EBCC
		public static implicit operator ConcurrentLong(long v)
		{
			return new ConcurrentLong(v);
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000109E4 File Offset: 0x0000EBE4
		public static ConcurrentLong operator +(ConcurrentLong x, long y)
		{
			Interlocked.Add(ref x._value, y);
			return x;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00010A08 File Offset: 0x0000EC08
		public static ConcurrentLong operator -(ConcurrentLong x, long y)
		{
			Interlocked.Add(ref x._value, -y);
			return x;
		}

		// Token: 0x04000022 RID: 34
		private long _value;
	}
}
