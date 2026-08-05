using System.Threading.Tasks;

namespace Server.Data;

internal class SocketData
{
	private static object syncSent = new object();

	private static object syncSents = new object();

	private static object syncRecives = new object();

	private static object syncRecive = new object();

	private static object syncConnects = new object();

	public static long Connects { get; private set; }

	public static long Sents { get; private set; }

	public static long Recives { get; private set; }

	public static long Sent { get; private set; }

	public static long Recive { get; private set; }

	public static void ConnectsPluse()
	{
		Task.Run(delegate
		{
			lock (syncConnects)
			{
				Connects++;
			}
		});
	}

	public static void ConnectsMinuse()
	{
		Task.Run(delegate
		{
			lock (syncConnects)
			{
				Connects--;
			}
		});
	}

	public static void SentData(long count)
	{
		Task.Run(delegate
		{
			lock (syncSents)
			{
				Sents += count;
			}
		});
		Task.Run(delegate
		{
			lock (syncSent)
			{
				Sent += count;
			}
		});
	}

	public static void ReciveData(long count)
	{
		Task.Run(delegate
		{
			lock (syncRecives)
			{
				Recives += count;
			}
		});
		Task.Run(delegate
		{
			lock (syncRecive)
			{
				Recive += count;
			}
		});
	}

	public static void Clear()
	{
		Task.Run(delegate
		{
			lock (syncSent)
			{
				Sent = 0L;
			}
			lock (syncRecive)
			{
				Recive = 0L;
			}
		});
	}
}
