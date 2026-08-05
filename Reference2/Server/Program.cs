using System;
using System.Windows.Forms;

namespace Server;

internal static class Program
{
	public static Form1 form;

	[STAThread]
	private static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		form = new Form1();
		Application.Run(form);
	}
}
