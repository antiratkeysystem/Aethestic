using System;
using System.Threading;

namespace Server.Connectings;

public class LastPing
{
	private Timer timer;

	public Clients client;

	private DateTime lastPing;

	public LastPing(Clients client)
	{
		this.client = client;
		lastPing = DateTime.Now;
		timer = new Timer(Check, null, 1, 2000);
	}

	private double DiffSeconds(DateTime startTime, DateTime endTime)
	{
		return Math.Abs(new TimeSpan(endTime.Ticks - startTime.Ticks).TotalSeconds);
	}

	private void Check(object obj)
	{
		if (DiffSeconds(lastPing, DateTime.Now) > (double)Program.form.settings.second)
		{
			client.Disconnect();
		}
	}

	public void Disconnect()
	{
		this.timer?.Dispose();
	}

	public void Last()
	{
		lastPing = DateTime.Now;
	}
}
