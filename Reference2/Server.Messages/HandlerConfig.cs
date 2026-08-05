using System;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerConfig
{
	public static void Read(Clients client, object[] objects)
	{
		try
		{
			if (client != null && objects != null && objects.Length >= 2 && objects[1] as string == "Info")
			{
				string hwid = objects[2].ToString();
				((FormConfig)Application.OpenForms["Config:" + hwid])?.UpdateConfig(objects);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error in HandlerConfig: " + ex.Message);
		}
	}
}
