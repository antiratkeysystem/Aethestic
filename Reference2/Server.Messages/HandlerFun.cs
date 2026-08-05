using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerFun
{
	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			if (client != null)
			{
				client.Disconnect();
			}
		}
		else
		{
			if (!((string)objects[1] == "Connect"))
			{
				return;
			}
			FormFun form = (FormFun)Application.OpenForms["Fun:" + (string)objects[2]];
			if (form == null)
			{
				client.Disconnect();
				return;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.Text = "Fun [" + (string)objects[2] + "]";
				form.client = client;
				form.rjTextBox1.Enabled = true;
				form.rjTextBox2.Enabled = true;
				form.rjTextBox3.Enabled = true;
				form.rjComboBox2.Enabled = true;
				form.rjComboBox3.Enabled = true;
				form.rjComboBox1.Enabled = true;
				form.rjButton1.Enabled = true;
				form.rjButton2.Enabled = true;
				form.rjButton3.Enabled = true;
				form.rjButton4.Enabled = true;
				form.rjButton5.Enabled = true;
				form.rjButton6.Enabled = true;
				form.rjButton7.Enabled = true;
				form.rjButton8.Enabled = true;
				form.rjButton9.Enabled = true;
				form.rjButton10.Enabled = true;
				form.rjButton11.Enabled = true;
				form.rjButton12.Enabled = true;
				form.rjButton13.Enabled = true;
				form.rjButton14.Enabled = true;
				form.rjButton15.Enabled = true;
				form.rjButton16.Enabled = true;
				form.rjButton17.Enabled = true;
				form.rjButton18.Enabled = true;
				form.rjButton19.Enabled = true;
				form.rjButton20.Enabled = true;
				form.rjButton21.Enabled = true;
				form.rjButton22.Enabled = true;
				form.rjButton23.Enabled = true;
				form.rjButton24.Enabled = true;
				form.rjButton25.Enabled = true;
				form.rjButton26.Enabled = true;
				form.rjButton27.Enabled = true;
				form.rjButton28.Enabled = true;
				form.rjButton29.Enabled = true;
				form.rjButton30.Enabled = true;
				form.rjButton31.Enabled = true;
				form.rjButton32.Enabled = true;
				form.rjButton33.Enabled = true;
				form.rjButton34.Enabled = true;
				form.rjButton35.Enabled = true;
				form.rjButton36.Enabled = true;
				form.rjButton37.Enabled = true;
				form.rjButton38.Enabled = true;
				form.rjButton39.Enabled = true;
				form.rjButton40.Enabled = true;
				form.rjButton41.Enabled = true;
				form.rjButton42.Enabled = true;
				form.rjButton43.Enabled = true;
				form.rjButton44.Enabled = true;
				form.rjButton45.Enabled = true;
				form.rjButton46.Enabled = true;
				form.rjButton47.Enabled = true;
			});
			client.Tag = form;
			client.Hwid = (string)objects[2];
		}
	}
}
