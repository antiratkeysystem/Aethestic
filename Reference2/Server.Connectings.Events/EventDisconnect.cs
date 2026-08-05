using System;

namespace Server.Connectings.Events;

public class EventDisconnect : EventArgs
{
	public Clients clients;
}
