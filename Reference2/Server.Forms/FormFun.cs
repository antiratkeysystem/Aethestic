using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormFun : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	private Label label1;

	private Label label2;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private Label label7;

	private Label label8;

	private Label label9;

	private Label label10;

	private Label label11;

	private Label label12;

	private Label label15;

	private Label label16;

	private Label label17;

	private Label label18;

	private Label label13;

	private Label label14;

	private Label label19;

	private Label label20;

	private Label label21;

	private Label label22;

	private Label label23;

	private Label label24;

	private Label label25;

	private Label label26;

	private Label label27;

	private Label label28;

	private Label label29;

	private Label label30;

	public RJButton rjButton1;

	public RJButton rjButton2;

	public RJButton rjButton3;

	public RJButton rjButton4;

	public RJButton rjButton5;

	public RJButton rjButton6;

	public RJButton rjButton7;

	public RJButton rjButton8;

	public RJButton rjButton9;

	public RJButton rjButton10;

	public RJButton rjButton11;

	public RJButton rjButton12;

	public RJButton rjButton13;

	public RJButton rjButton14;

	public RJComboBox rjComboBox2;

	public RJButton rjButton15;

	public RJButton rjButton16;

	public RJButton rjButton17;

	public RJButton rjButton20;

	public RJButton rjButton21;

	public RJButton rjButton22;

	public RJButton rjButton23;

	public RJButton rjButton18;

	public RJButton rjButton19;

	public RJButton rjButton24;

	public RJButton rjButton25;

	public RJButton rjButton26;

	public RJButton rjButton27;

	public RJButton rjButton28;

	public RJButton rjButton29;

	public RJButton rjButton30;

	public RJButton rjButton31;

	public RJTextBox rjTextBox1;

	public RJButton rjButton32;

	public RJButton rjButton33;

	public RJButton rjButton34;

	public RJButton rjButton35;

	public RJButton rjButton36;

	public RJButton rjButton37;

	public RJButton rjButton38;

	public RJButton rjButton39;

	public RJButton rjButton40;

	public RJButton rjButton41;

	public RJButton rjButton42;

	private Label label31;

	public RJTextBox rjTextBox2;

	public RJTextBox rjTextBox3;

	public RJButton rjButton43;

	public RJButton rjButton44;

	public RJComboBox rjComboBox1;

	public RJComboBox rjComboBox3;

	public RJButton rjButton45;

	private Label label32;

	private Label label33;

	public RJButton rjButton46;

	private Label label34;

	public RJButton rjButton47;

	private Label label35;

	public FormFun()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormFun_Load(object sender, EventArgs e)
	{
		rjComboBox2.SelectedIndex = 0;
		rjComboBox1.SelectedIndex = 0;
		rjComboBox3.SelectedIndex = 0;
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
	}

	private void ChangeScheme(object sender)
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color primary = FormMaterial.PrimaryColor;
		Color back = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color text = (num ? Color.WhiteSmoke : Color.Black);
		BackColor = back;
		foreach (Control ctrl in base.Controls)
		{
			if (ctrl is Panel)
			{
				ctrl.BackColor = back;
			}
		}
		rjTextBox1.BorderColor = primary;
		rjTextBox2.BorderColor = primary;
		rjTextBox3.BorderColor = primary;
		rjTextBox1.BackColor = back;
		rjTextBox2.BackColor = back;
		rjTextBox3.BackColor = back;
		rjTextBox1.ForeColor = text;
		rjTextBox2.ForeColor = text;
		rjTextBox3.ForeColor = text;
		rjComboBox1.BorderColor = primary;
		rjComboBox2.BorderColor = primary;
		rjComboBox3.BorderColor = primary;
		rjComboBox1.IconColor = primary;
		rjComboBox2.IconColor = primary;
		rjComboBox3.IconColor = primary;
		rjComboBox1.BackColor = back;
		rjComboBox2.BackColor = back;
		rjComboBox3.BackColor = back;
		rjComboBox1.ForeColor = text;
		rjComboBox2.ForeColor = text;
		rjComboBox3.ForeColor = text;
		rjComboBox1.ListBackColor = back;
		rjComboBox2.ListBackColor = back;
		rjComboBox3.ListBackColor = back;
		rjComboBox1.ListTextColor = text;
		rjComboBox2.ListTextColor = text;
		rjComboBox3.ListTextColor = text;
		foreach (Control ctrl2 in base.Controls)
		{
			if (ctrl2 is Label)
			{
				((Label)ctrl2).ForeColor = text;
			}
		}
		rjButton1.BackColor = primary;
		rjButton2.BackColor = primary;
		rjButton3.BackColor = primary;
		rjButton4.BackColor = primary;
		rjButton5.BackColor = primary;
		rjButton6.BackColor = primary;
		rjButton7.BackColor = primary;
		rjButton8.BackColor = primary;
		rjButton9.BackColor = primary;
		rjButton10.BackColor = primary;
		rjButton11.BackColor = primary;
		rjButton12.BackColor = primary;
		rjButton13.BackColor = primary;
		rjButton14.BackColor = primary;
		rjButton15.BackColor = primary;
		rjButton16.BackColor = primary;
		rjButton17.BackColor = primary;
		rjButton18.BackColor = primary;
		rjButton19.BackColor = primary;
		rjButton20.BackColor = primary;
		rjButton21.BackColor = primary;
		rjButton22.BackColor = primary;
		rjButton23.BackColor = primary;
		rjButton24.BackColor = primary;
		rjButton25.BackColor = primary;
		rjButton26.BackColor = primary;
		rjButton27.BackColor = primary;
		rjButton28.BackColor = primary;
		rjButton29.BackColor = primary;
		rjButton30.BackColor = primary;
		rjButton31.BackColor = primary;
		rjButton32.BackColor = primary;
		rjButton33.BackColor = primary;
		rjButton34.BackColor = primary;
		rjButton35.BackColor = primary;
		rjButton36.BackColor = primary;
		rjButton37.BackColor = primary;
		rjButton38.BackColor = primary;
		rjButton39.BackColor = primary;
		rjButton40.BackColor = primary;
		rjButton41.BackColor = primary;
		rjButton42.BackColor = primary;
		rjButton43.BackColor = primary;
		rjButton44.BackColor = primary;
		rjButton45.BackColor = primary;
		rjButton46.BackColor = primary;
		rjButton47.BackColor = primary;
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (!parrent.itsConnect)
		{
			Close();
		}
		if (client != null && !client.itsConnect)
		{
			Close();
		}
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		if (rjButton1.Text == "Hidden")
		{
			rjButton1.Text = "Show";
			client.Send(new object[1] { "Taskbar-" });
		}
		else
		{
			rjButton1.Text = "Hidden";
			client.Send(new object[1] { "Taskbar+" });
		}
	}

	private void rjButton2_Click(object sender, EventArgs e)
	{
		if (rjButton2.Text == "Hidden")
		{
			rjButton2.Text = "Show";
			client.Send(new object[1] { "Desktop-" });
		}
		else
		{
			rjButton2.Text = "Hidden";
			client.Send(new object[1] { "Desktop+" });
		}
	}

	private void rjButton14_Click(object sender, EventArgs e)
	{
		if (rjButton14.Text == "Start")
		{
			rjButton14.Text = "Stop";
			client.Send(new object[1] { "blankscreen+" });
		}
		else
		{
			rjButton14.Text = "Start";
			client.Send(new object[1] { "blankscreen-" });
		}
	}

	private void rjButton3_Click(object sender, EventArgs e)
	{
		if (rjButton3.Text == "Start")
		{
			rjButton3.Text = "Stop";
			client.Send(new object[1] { "holdMouse+" });
		}
		else
		{
			rjButton3.Text = "Start";
			client.Send(new object[1] { "holdMouse-" });
		}
	}

	private void rjButton4_Click(object sender, EventArgs e)
	{
		if (rjButton4.Text == "Start")
		{
			rjButton4.Text = "Stop";
			client.Send(new object[1] { "blockInput+" });
		}
		else
		{
			rjButton4.Text = "Start";
			client.Send(new object[1] { "blockInput-" });
		}
	}

	private void rjButton5_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "openCD+" });
	}

	private void rjButton13_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "openCD-" });
	}

	private void rjButton41_Click(object sender, EventArgs e)
	{
		if (rjButton41.Text == "Swap")
		{
			rjButton41.Text = "Restore";
			client.Send(new object[1] { "swapMouseButtons" });
		}
		else
		{
			rjButton41.Text = "Swap";
			client.Send(new object[1] { "restoreMouseButtons" });
		}
	}

	private void rjButton6_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "monitorOff" });
	}

	private void rjButton15_Click(object sender, EventArgs e)
	{
		if (rjComboBox2.SelectedIndex != -1)
		{
			client.Send(new object[2] { "ScreenS", rjComboBox2.Texts });
		}
	}

	private void rjButton42_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "hangSystem" });
	}

	private void rjButton32_Click(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(rjTextBox1.Texts))
		{
			client.Send(new object[2] { "OpenLink", rjTextBox1.Texts });
		}
	}

	private void rjButton9_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "Powershell" });
	}

	private void rjButton10_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "Taskmgr" });
	}

	private void rjButton11_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "Notepad" });
	}

	private void rjButton8_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "Calc" });
	}

	private void rjButton7_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "Cmd" });
	}

	private void rjButton12_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "Explorer" });
	}

	private void rjButton37_Click(object sender, EventArgs e)
	{
		client.Send(new object[2] { "ScreenColor", "Default" });
	}

	private void rjButton40_Click(object sender, EventArgs e)
	{
		client.Send(new object[2] { "ScreenColor", "Red" });
	}

	private void rjButton38_Click(object sender, EventArgs e)
	{
		client.Send(new object[2] { "ScreenColor", "Purple" });
	}

	private void rjButton39_Click(object sender, EventArgs e)
	{
		client.Send(new object[2] { "ScreenColor", "Yellow" });
	}

	private void rjButton33_Click(object sender, EventArgs e)
	{
		client.Send(new object[2] { "ScreenColor", "Dark" });
	}

	private void rjButton36_Click(object sender, EventArgs e)
	{
		client.Send(new object[2] { "ScreenColor", "Cyan" });
	}

	private void rjButton34_Click(object sender, EventArgs e)
	{
		client.Send(new object[2] { "ScreenColor", "Blue" });
	}

	private void rjButton35_Click(object sender, EventArgs e)
	{
		client.Send(new object[2] { "ScreenColor", "Green" });
	}

	private void rjButton19_Click(object sender, EventArgs e)
	{
		if (rjButton19.Text == "Start")
		{
			rjButton19.Text = "Stop";
			client.Send(new object[1] { "Smelt+" });
		}
		else
		{
			rjButton19.Text = "Start";
			client.Send(new object[1] { "Smelt-" });
		}
	}

	private void rjButton16_Click(object sender, EventArgs e)
	{
		if (rjButton16.Text == "Start")
		{
			rjButton16.Text = "Stop";
			client.Send(new object[1] { "InvertSmelt+" });
		}
		else
		{
			rjButton16.Text = "Start";
			client.Send(new object[1] { "InvertSmelt-" });
		}
	}

	private void rjButton18_Click(object sender, EventArgs e)
	{
		if (rjButton18.Text == "Start")
		{
			rjButton18.Text = "Stop";
			client.Send(new object[1] { "VerticalWide+" });
		}
		else
		{
			rjButton18.Text = "Start";
			client.Send(new object[1] { "VerticalWide-" });
		}
	}

	private void rjButton17_Click(object sender, EventArgs e)
	{
		if (rjButton17.Text == "Start")
		{
			rjButton17.Text = "Stop";
			client.Send(new object[1] { "Wide+" });
		}
		else
		{
			rjButton17.Text = "Start";
			client.Send(new object[1] { "Wide-" });
		}
	}

	private void rjButton23_Click(object sender, EventArgs e)
	{
		if (rjButton23.Text == "Start")
		{
			rjButton23.Text = "Stop";
			client.Send(new object[1] { "Train1+" });
		}
		else
		{
			rjButton23.Text = "Start";
			client.Send(new object[1] { "Train1-" });
		}
	}

	private void rjButton22_Click(object sender, EventArgs e)
	{
		if (rjButton22.Text == "Start")
		{
			rjButton22.Text = "Stop";
			client.Send(new object[1] { "Train2+" });
		}
		else
		{
			rjButton22.Text = "Start";
			client.Send(new object[1] { "Train2-" });
		}
	}

	private void rjButton21_Click(object sender, EventArgs e)
	{
		if (rjButton21.Text == "Start")
		{
			rjButton21.Text = "Stop";
			client.Send(new object[1] { "Train3+" });
		}
		else
		{
			rjButton21.Text = "Start";
			client.Send(new object[1] { "Train3-" });
		}
	}

	private void rjButton20_Click(object sender, EventArgs e)
	{
		if (rjButton20.Text == "Start")
		{
			rjButton20.Text = "Stop";
			client.Send(new object[1] { "Rgbtrain+" });
		}
		else
		{
			rjButton20.Text = "Start";
			client.Send(new object[1] { "Rgbtrain-" });
		}
	}

	private void rjButton27_Click(object sender, EventArgs e)
	{
		if (rjButton27.Text == "Start")
		{
			rjButton27.Text = "Stop";
			client.Send(new object[1] { "Sinewaves+" });
		}
		else
		{
			rjButton27.Text = "Start";
			client.Send(new object[1] { "Sinewaves-" });
		}
	}

	private void rjButton26_Click(object sender, EventArgs e)
	{
		if (rjButton26.Text == "Start")
		{
			rjButton26.Text = "Stop";
			client.Send(new object[1] { "Shake+" });
		}
		else
		{
			rjButton26.Text = "Start";
			client.Send(new object[1] { "Shake-" });
		}
	}

	private void rjButton25_Click(object sender, EventArgs e)
	{
		if (rjButton25.Text == "Start")
		{
			rjButton25.Text = "Stop";
			client.Send(new object[1] { "Setpixel+" });
		}
		else
		{
			rjButton25.Text = "Start";
			client.Send(new object[1] { "Setpixel-" });
		}
	}

	private void rjButton24_Click(object sender, EventArgs e)
	{
		if (rjButton24.Text == "Start")
		{
			rjButton24.Text = "Stop";
			client.Send(new object[1] { "DumpVD+" });
		}
		else
		{
			rjButton24.Text = "Start";
			client.Send(new object[1] { "DumpVD-" });
		}
	}

	private void rjButton31_Click(object sender, EventArgs e)
	{
		if (rjButton31.Text == "Start")
		{
			rjButton31.Text = "Stop";
			client.Send(new object[1] { "Dark+" });
		}
		else
		{
			rjButton31.Text = "Start";
			client.Send(new object[1] { "Dark-" });
		}
	}

	private void rjButton30_Click(object sender, EventArgs e)
	{
		if (rjButton30.Text == "Start")
		{
			rjButton30.Text = "Stop";
			client.Send(new object[1] { "Stripes+" });
		}
		else
		{
			rjButton30.Text = "Start";
			client.Send(new object[1] { "Stripes-" });
		}
	}

	private void rjButton29_Click(object sender, EventArgs e)
	{
		if (rjButton29.Text == "Start")
		{
			rjButton29.Text = "Stop";
			client.Send(new object[1] { "Tunnel+" });
		}
		else
		{
			rjButton29.Text = "Start";
			client.Send(new object[1] { "Tunnel-" });
		}
	}

	private void rjButton28_Click(object sender, EventArgs e)
	{
		if (rjButton28.Text == "Start")
		{
			rjButton28.Text = "Stop";
			client.Send(new object[1] { "InvertColor+" });
		}
		else
		{
			rjButton28.Text = "Start";
			client.Send(new object[1] { "InvertColor-" });
		}
	}

	private void rjButton44_Click(object sender, EventArgs e)
	{
		if (!(rjButton28.Text == "Spam"))
		{
			rjButton28.Text = "Spam";
			client.Send(new object[1] { "MessageBoxSpam-" });
		}
		else if (!string.IsNullOrEmpty(rjTextBox2.Texts) && !string.IsNullOrEmpty(rjTextBox3.Texts) && rjComboBox1.SelectedIndex != -1 && rjComboBox3.SelectedIndex != -1)
		{
			rjButton28.Text = "Stop";
			client.Send(new object[5]
			{
				"MessageBoxSpam+",
				rjTextBox3.Texts,
				rjTextBox2.Texts,
				rjComboBox1.SelectedIndex,
				rjComboBox3.SelectedIndex * 16
			});
		}
	}

	private void rjButton43_Click(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(rjTextBox2.Texts) && !string.IsNullOrEmpty(rjTextBox3.Texts) && rjComboBox1.SelectedIndex != -1 && rjComboBox3.SelectedIndex != -1)
		{
			client.Send(new object[5]
			{
				"MessageBox",
				rjTextBox3.Texts,
				rjTextBox2.Texts,
				rjComboBox1.SelectedIndex,
				rjComboBox3.SelectedIndex * 16
			});
		}
	}

	private void rjButton45_Click(object sender, EventArgs e)
	{
		if (rjButton45.Text == "Start")
		{
			rjButton45.Text = "Stop";
			client.Send(new object[1] { "FuckScreen+" });
		}
		else
		{
			rjButton45.Text = "Start";
			client.Send(new object[1] { "FuckScreen-" });
		}
	}

	private void rjButton46_Click(object sender, EventArgs e)
	{
		if (rjButton46.Text == "Start")
		{
			rjButton46.Text = "Stop";
			client.Send(new object[1] { "Led+" });
		}
		else
		{
			rjButton46.Text = "Start";
			client.Send(new object[1] { "Led-" });
		}
	}

	private void rjButton47_Click(object sender, EventArgs e)
	{
		if (rjButton47.Text == "Start")
		{
			rjButton47.Text = "Stop";
			client.Send(new object[1] { "Keyboard+" });
		}
		else
		{
			rjButton47.Text = "Start";
			client.Send(new object[1] { "Keyboard-" });
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.label3 = new System.Windows.Forms.Label();
		this.rjButton3 = new CustomControls.RJControls.RJButton();
		this.label4 = new System.Windows.Forms.Label();
		this.rjButton4 = new CustomControls.RJControls.RJButton();
		this.label5 = new System.Windows.Forms.Label();
		this.rjButton5 = new CustomControls.RJControls.RJButton();
		this.label6 = new System.Windows.Forms.Label();
		this.rjButton6 = new CustomControls.RJControls.RJButton();
		this.label7 = new System.Windows.Forms.Label();
		this.rjButton7 = new CustomControls.RJControls.RJButton();
		this.rjButton8 = new CustomControls.RJControls.RJButton();
		this.rjButton9 = new CustomControls.RJControls.RJButton();
		this.rjButton10 = new CustomControls.RJControls.RJButton();
		this.rjButton11 = new CustomControls.RJControls.RJButton();
		this.rjButton12 = new CustomControls.RJControls.RJButton();
		this.rjButton13 = new CustomControls.RJControls.RJButton();
		this.label8 = new System.Windows.Forms.Label();
		this.rjButton14 = new CustomControls.RJControls.RJButton();
		this.label9 = new System.Windows.Forms.Label();
		this.rjComboBox2 = new CustomControls.RJControls.RJComboBox();
		this.label10 = new System.Windows.Forms.Label();
		this.rjButton15 = new CustomControls.RJControls.RJButton();
		this.label11 = new System.Windows.Forms.Label();
		this.rjButton16 = new CustomControls.RJControls.RJButton();
		this.label12 = new System.Windows.Forms.Label();
		this.rjButton17 = new CustomControls.RJControls.RJButton();
		this.label15 = new System.Windows.Forms.Label();
		this.rjButton20 = new CustomControls.RJControls.RJButton();
		this.label16 = new System.Windows.Forms.Label();
		this.rjButton21 = new CustomControls.RJControls.RJButton();
		this.label17 = new System.Windows.Forms.Label();
		this.rjButton22 = new CustomControls.RJControls.RJButton();
		this.label18 = new System.Windows.Forms.Label();
		this.rjButton23 = new CustomControls.RJControls.RJButton();
		this.label13 = new System.Windows.Forms.Label();
		this.rjButton18 = new CustomControls.RJControls.RJButton();
		this.label14 = new System.Windows.Forms.Label();
		this.rjButton19 = new CustomControls.RJControls.RJButton();
		this.label19 = new System.Windows.Forms.Label();
		this.rjButton24 = new CustomControls.RJControls.RJButton();
		this.label20 = new System.Windows.Forms.Label();
		this.rjButton25 = new CustomControls.RJControls.RJButton();
		this.label21 = new System.Windows.Forms.Label();
		this.rjButton26 = new CustomControls.RJControls.RJButton();
		this.label22 = new System.Windows.Forms.Label();
		this.rjButton27 = new CustomControls.RJControls.RJButton();
		this.label23 = new System.Windows.Forms.Label();
		this.rjButton28 = new CustomControls.RJControls.RJButton();
		this.label24 = new System.Windows.Forms.Label();
		this.rjButton29 = new CustomControls.RJControls.RJButton();
		this.label25 = new System.Windows.Forms.Label();
		this.rjButton30 = new CustomControls.RJControls.RJButton();
		this.label26 = new System.Windows.Forms.Label();
		this.rjButton31 = new CustomControls.RJControls.RJButton();
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.label27 = new System.Windows.Forms.Label();
		this.rjButton32 = new CustomControls.RJControls.RJButton();
		this.rjButton33 = new CustomControls.RJControls.RJButton();
		this.rjButton34 = new CustomControls.RJControls.RJButton();
		this.rjButton35 = new CustomControls.RJControls.RJButton();
		this.rjButton36 = new CustomControls.RJControls.RJButton();
		this.label28 = new System.Windows.Forms.Label();
		this.rjButton37 = new CustomControls.RJControls.RJButton();
		this.rjButton38 = new CustomControls.RJControls.RJButton();
		this.rjButton39 = new CustomControls.RJControls.RJButton();
		this.rjButton40 = new CustomControls.RJControls.RJButton();
		this.label29 = new System.Windows.Forms.Label();
		this.rjButton41 = new CustomControls.RJControls.RJButton();
		this.label30 = new System.Windows.Forms.Label();
		this.rjButton42 = new CustomControls.RJControls.RJButton();
		this.label31 = new System.Windows.Forms.Label();
		this.rjTextBox2 = new CustomControls.RJControls.RJTextBox();
		this.rjTextBox3 = new CustomControls.RJControls.RJTextBox();
		this.rjButton43 = new CustomControls.RJControls.RJButton();
		this.rjButton44 = new CustomControls.RJControls.RJButton();
		this.rjComboBox1 = new CustomControls.RJControls.RJComboBox();
		this.rjComboBox3 = new CustomControls.RJControls.RJComboBox();
		this.rjButton45 = new CustomControls.RJControls.RJButton();
		this.label32 = new System.Windows.Forms.Label();
		this.label33 = new System.Windows.Forms.Label();
		this.rjButton46 = new CustomControls.RJControls.RJButton();
		this.label34 = new System.Windows.Forms.Label();
		this.rjButton47 = new CustomControls.RJControls.RJButton();
		this.label35 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.Enabled = false;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(20, 100);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(74, 31);
		this.rjButton1.TabIndex = 49;
		this.rjButton1.Text = "Hidden";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label1.Location = new System.Drawing.Point(17, 81);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(105, 16);
		this.label1.TabIndex = 50;
		this.label1.Text = "Taskbar Hidden";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label2.Location = new System.Drawing.Point(17, 138);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(105, 16);
		this.label2.TabIndex = 52;
		this.label2.Text = "Desktop Hidden";
		this.rjButton2.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton2.BorderRadius = 0;
		this.rjButton2.BorderSize = 0;
		this.rjButton2.Enabled = false;
		this.rjButton2.FlatAppearance.BorderSize = 0;
		this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton2.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton2.ForeColor = System.Drawing.Color.White;
		this.rjButton2.Location = new System.Drawing.Point(20, 157);
		this.rjButton2.Name = "rjButton2";
		this.rjButton2.Size = new System.Drawing.Size(74, 31);
		this.rjButton2.TabIndex = 51;
		this.rjButton2.Text = "Hidden";
		this.rjButton2.TextColor = System.Drawing.Color.White;
		this.rjButton2.UseVisualStyleBackColor = false;
		this.rjButton2.Click += new System.EventHandler(rjButton2_Click);
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label3.Location = new System.Drawing.Point(130, 81);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(77, 16);
		this.label3.TabIndex = 54;
		this.label3.Text = "Mouse hold";
		this.rjButton3.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton3.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton3.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton3.BorderRadius = 0;
		this.rjButton3.BorderSize = 0;
		this.rjButton3.Enabled = false;
		this.rjButton3.FlatAppearance.BorderSize = 0;
		this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton3.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton3.ForeColor = System.Drawing.Color.White;
		this.rjButton3.Location = new System.Drawing.Point(133, 100);
		this.rjButton3.Name = "rjButton3";
		this.rjButton3.Size = new System.Drawing.Size(74, 31);
		this.rjButton3.TabIndex = 53;
		this.rjButton3.Text = "Start";
		this.rjButton3.TextColor = System.Drawing.Color.White;
		this.rjButton3.UseVisualStyleBackColor = false;
		this.rjButton3.Click += new System.EventHandler(rjButton3_Click);
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label4.Location = new System.Drawing.Point(130, 138);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(80, 16);
		this.label4.TabIndex = 56;
		this.label4.Text = "Action block";
		this.rjButton4.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton4.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton4.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton4.BorderRadius = 0;
		this.rjButton4.BorderSize = 0;
		this.rjButton4.Enabled = false;
		this.rjButton4.FlatAppearance.BorderSize = 0;
		this.rjButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton4.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton4.ForeColor = System.Drawing.Color.White;
		this.rjButton4.Location = new System.Drawing.Point(133, 157);
		this.rjButton4.Name = "rjButton4";
		this.rjButton4.Size = new System.Drawing.Size(74, 31);
		this.rjButton4.TabIndex = 55;
		this.rjButton4.Text = "Start";
		this.rjButton4.TextColor = System.Drawing.Color.White;
		this.rjButton4.UseVisualStyleBackColor = false;
		this.rjButton4.Click += new System.EventHandler(rjButton4_Click);
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label5.Location = new System.Drawing.Point(243, 81);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(97, 16);
		this.label5.TabIndex = 58;
		this.label5.Text = "CD Drive Open";
		this.rjButton5.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton5.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton5.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton5.BorderRadius = 0;
		this.rjButton5.BorderSize = 0;
		this.rjButton5.Enabled = false;
		this.rjButton5.FlatAppearance.BorderSize = 0;
		this.rjButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton5.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton5.ForeColor = System.Drawing.Color.White;
		this.rjButton5.Location = new System.Drawing.Point(246, 100);
		this.rjButton5.Name = "rjButton5";
		this.rjButton5.Size = new System.Drawing.Size(74, 31);
		this.rjButton5.TabIndex = 57;
		this.rjButton5.Text = "Open";
		this.rjButton5.TextColor = System.Drawing.Color.White;
		this.rjButton5.UseVisualStyleBackColor = false;
		this.rjButton5.Click += new System.EventHandler(rjButton5_Click);
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label6.Location = new System.Drawing.Point(243, 199);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(110, 16);
		this.label6.TabIndex = 60;
		this.label6.Text = "Monitor shutdown";
		this.rjButton6.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton6.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton6.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton6.BorderRadius = 0;
		this.rjButton6.BorderSize = 0;
		this.rjButton6.Enabled = false;
		this.rjButton6.FlatAppearance.BorderSize = 0;
		this.rjButton6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton6.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton6.ForeColor = System.Drawing.Color.White;
		this.rjButton6.Location = new System.Drawing.Point(246, 218);
		this.rjButton6.Name = "rjButton6";
		this.rjButton6.Size = new System.Drawing.Size(74, 31);
		this.rjButton6.TabIndex = 59;
		this.rjButton6.Text = "Shutdown";
		this.rjButton6.TextColor = System.Drawing.Color.White;
		this.rjButton6.UseVisualStyleBackColor = false;
		this.rjButton6.Click += new System.EventHandler(rjButton6_Click);
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label7.Location = new System.Drawing.Point(30, 345);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(110, 16);
		this.label7.TabIndex = 61;
		this.label7.Text = "Open Application";
		this.rjButton7.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton7.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton7.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton7.BorderRadius = 0;
		this.rjButton7.BorderSize = 0;
		this.rjButton7.Enabled = false;
		this.rjButton7.FlatAppearance.BorderSize = 0;
		this.rjButton7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton7.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton7.ForeColor = System.Drawing.Color.White;
		this.rjButton7.Location = new System.Drawing.Point(128, 401);
		this.rjButton7.Name = "rjButton7";
		this.rjButton7.Size = new System.Drawing.Size(79, 31);
		this.rjButton7.TabIndex = 62;
		this.rjButton7.Text = "Cmd";
		this.rjButton7.TextColor = System.Drawing.Color.White;
		this.rjButton7.UseVisualStyleBackColor = false;
		this.rjButton7.Click += new System.EventHandler(rjButton7_Click);
		this.rjButton8.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton8.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton8.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton8.BorderRadius = 0;
		this.rjButton8.BorderSize = 0;
		this.rjButton8.Enabled = false;
		this.rjButton8.FlatAppearance.BorderSize = 0;
		this.rjButton8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton8.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton8.ForeColor = System.Drawing.Color.White;
		this.rjButton8.Location = new System.Drawing.Point(43, 401);
		this.rjButton8.Name = "rjButton8";
		this.rjButton8.Size = new System.Drawing.Size(79, 31);
		this.rjButton8.TabIndex = 63;
		this.rjButton8.Text = "Calc";
		this.rjButton8.TextColor = System.Drawing.Color.White;
		this.rjButton8.UseVisualStyleBackColor = false;
		this.rjButton8.Click += new System.EventHandler(rjButton8_Click);
		this.rjButton9.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton9.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton9.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton9.BorderRadius = 0;
		this.rjButton9.BorderSize = 0;
		this.rjButton9.Enabled = false;
		this.rjButton9.FlatAppearance.BorderSize = 0;
		this.rjButton9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton9.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton9.ForeColor = System.Drawing.Color.White;
		this.rjButton9.Location = new System.Drawing.Point(43, 364);
		this.rjButton9.Name = "rjButton9";
		this.rjButton9.Size = new System.Drawing.Size(79, 31);
		this.rjButton9.TabIndex = 64;
		this.rjButton9.Text = "Powershell";
		this.rjButton9.TextColor = System.Drawing.Color.White;
		this.rjButton9.UseVisualStyleBackColor = false;
		this.rjButton9.Click += new System.EventHandler(rjButton9_Click);
		this.rjButton10.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton10.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton10.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton10.BorderRadius = 0;
		this.rjButton10.BorderSize = 0;
		this.rjButton10.Enabled = false;
		this.rjButton10.FlatAppearance.BorderSize = 0;
		this.rjButton10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton10.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton10.ForeColor = System.Drawing.Color.White;
		this.rjButton10.Location = new System.Drawing.Point(128, 364);
		this.rjButton10.Name = "rjButton10";
		this.rjButton10.Size = new System.Drawing.Size(79, 31);
		this.rjButton10.TabIndex = 65;
		this.rjButton10.Text = "Taskmgr";
		this.rjButton10.TextColor = System.Drawing.Color.White;
		this.rjButton10.UseVisualStyleBackColor = false;
		this.rjButton10.Click += new System.EventHandler(rjButton10_Click);
		this.rjButton11.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton11.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton11.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton11.BorderRadius = 0;
		this.rjButton11.BorderSize = 0;
		this.rjButton11.Enabled = false;
		this.rjButton11.FlatAppearance.BorderSize = 0;
		this.rjButton11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton11.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton11.ForeColor = System.Drawing.Color.White;
		this.rjButton11.Location = new System.Drawing.Point(213, 364);
		this.rjButton11.Name = "rjButton11";
		this.rjButton11.Size = new System.Drawing.Size(79, 31);
		this.rjButton11.TabIndex = 66;
		this.rjButton11.Text = "Notepad";
		this.rjButton11.TextColor = System.Drawing.Color.White;
		this.rjButton11.UseVisualStyleBackColor = false;
		this.rjButton11.Click += new System.EventHandler(rjButton11_Click);
		this.rjButton12.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton12.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton12.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton12.BorderRadius = 0;
		this.rjButton12.BorderSize = 0;
		this.rjButton12.Enabled = false;
		this.rjButton12.FlatAppearance.BorderSize = 0;
		this.rjButton12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton12.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton12.ForeColor = System.Drawing.Color.White;
		this.rjButton12.Location = new System.Drawing.Point(213, 401);
		this.rjButton12.Name = "rjButton12";
		this.rjButton12.Size = new System.Drawing.Size(79, 31);
		this.rjButton12.TabIndex = 67;
		this.rjButton12.Text = "Explorer";
		this.rjButton12.TextColor = System.Drawing.Color.White;
		this.rjButton12.UseVisualStyleBackColor = false;
		this.rjButton12.Click += new System.EventHandler(rjButton12_Click);
		this.rjButton13.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton13.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton13.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton13.BorderRadius = 0;
		this.rjButton13.BorderSize = 0;
		this.rjButton13.Enabled = false;
		this.rjButton13.FlatAppearance.BorderSize = 0;
		this.rjButton13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton13.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton13.ForeColor = System.Drawing.Color.White;
		this.rjButton13.Location = new System.Drawing.Point(326, 100);
		this.rjButton13.Name = "rjButton13";
		this.rjButton13.Size = new System.Drawing.Size(74, 31);
		this.rjButton13.TabIndex = 68;
		this.rjButton13.Text = "Close";
		this.rjButton13.TextColor = System.Drawing.Color.White;
		this.rjButton13.UseVisualStyleBackColor = false;
		this.rjButton13.Click += new System.EventHandler(rjButton13_Click);
		this.label8.AutoSize = true;
		this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label8.Location = new System.Drawing.Point(17, 200);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(87, 16);
		this.label8.TabIndex = 70;
		this.label8.Text = "Black Screen";
		this.rjButton14.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton14.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton14.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton14.BorderRadius = 0;
		this.rjButton14.BorderSize = 0;
		this.rjButton14.Enabled = false;
		this.rjButton14.FlatAppearance.BorderSize = 0;
		this.rjButton14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton14.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton14.ForeColor = System.Drawing.Color.White;
		this.rjButton14.Location = new System.Drawing.Point(20, 219);
		this.rjButton14.Name = "rjButton14";
		this.rjButton14.Size = new System.Drawing.Size(74, 31);
		this.rjButton14.TabIndex = 69;
		this.rjButton14.Text = "Start";
		this.rjButton14.TextColor = System.Drawing.Color.White;
		this.rjButton14.UseVisualStyleBackColor = false;
		this.rjButton14.Click += new System.EventHandler(rjButton14_Click);
		this.label9.AutoSize = true;
		this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label9.Location = new System.Drawing.Point(667, 81);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(54, 16);
		this.label9.TabIndex = 71;
		this.label9.Text = "GDI+ >:)";
		this.rjComboBox2.BackColor = System.Drawing.Color.White;
		this.rjComboBox2.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.BorderSize = 1;
		this.rjComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox2.Enabled = false;
		this.rjComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox2.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox2.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.Items.AddRange(new object[4] { "0", "90", "180", "270" });
		this.rjComboBox2.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox2.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox2.Location = new System.Drawing.Point(365, 186);
		this.rjComboBox2.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox2.Name = "rjComboBox2";
		this.rjComboBox2.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox2.Size = new System.Drawing.Size(200, 30);
		this.rjComboBox2.TabIndex = 72;
		this.rjComboBox2.Texts = "";
		this.label10.AutoSize = true;
		this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label10.Location = new System.Drawing.Point(362, 167);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(53, 16);
		this.label10.TabIndex = 73;
		this.label10.Text = "Screen ";
		this.rjButton15.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton15.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton15.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton15.BorderRadius = 0;
		this.rjButton15.BorderSize = 0;
		this.rjButton15.Enabled = false;
		this.rjButton15.FlatAppearance.BorderSize = 0;
		this.rjButton15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton15.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton15.ForeColor = System.Drawing.Color.White;
		this.rjButton15.Location = new System.Drawing.Point(365, 222);
		this.rjButton15.Name = "rjButton15";
		this.rjButton15.Size = new System.Drawing.Size(74, 31);
		this.rjButton15.TabIndex = 74;
		this.rjButton15.Text = "Rotate";
		this.rjButton15.TextColor = System.Drawing.Color.White;
		this.rjButton15.UseVisualStyleBackColor = false;
		this.rjButton15.Click += new System.EventHandler(rjButton15_Click);
		this.label11.AutoSize = true;
		this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label11.Location = new System.Drawing.Point(648, 104);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(51, 16);
		this.label11.TabIndex = 76;
		this.label11.Text = "HSmelt";
		this.rjButton16.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton16.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton16.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton16.BorderRadius = 0;
		this.rjButton16.BorderSize = 0;
		this.rjButton16.Enabled = false;
		this.rjButton16.FlatAppearance.BorderSize = 0;
		this.rjButton16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton16.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton16.ForeColor = System.Drawing.Color.White;
		this.rjButton16.Location = new System.Drawing.Point(651, 123);
		this.rjButton16.Name = "rjButton16";
		this.rjButton16.Size = new System.Drawing.Size(74, 31);
		this.rjButton16.TabIndex = 75;
		this.rjButton16.Text = "Start";
		this.rjButton16.TextColor = System.Drawing.Color.White;
		this.rjButton16.UseVisualStyleBackColor = false;
		this.rjButton16.Click += new System.EventHandler(rjButton16_Click);
		this.label12.AutoSize = true;
		this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label12.Location = new System.Drawing.Point(808, 104);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(49, 16);
		this.label12.TabIndex = 78;
		this.label12.Text = "HWide";
		this.rjButton17.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton17.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton17.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton17.BorderRadius = 0;
		this.rjButton17.BorderSize = 0;
		this.rjButton17.Enabled = false;
		this.rjButton17.FlatAppearance.BorderSize = 0;
		this.rjButton17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton17.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton17.ForeColor = System.Drawing.Color.White;
		this.rjButton17.Location = new System.Drawing.Point(811, 123);
		this.rjButton17.Name = "rjButton17";
		this.rjButton17.Size = new System.Drawing.Size(74, 31);
		this.rjButton17.TabIndex = 77;
		this.rjButton17.Text = "Start";
		this.rjButton17.TextColor = System.Drawing.Color.White;
		this.rjButton17.UseVisualStyleBackColor = false;
		this.rjButton17.Click += new System.EventHandler(rjButton17_Click);
		this.label15.AutoSize = true;
		this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label15.Location = new System.Drawing.Point(808, 166);
		this.label15.Name = "label15";
		this.label15.Size = new System.Drawing.Size(58, 16);
		this.label15.TabIndex = 90;
		this.label15.Text = "Rgbtrain";
		this.rjButton20.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton20.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton20.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton20.BorderRadius = 0;
		this.rjButton20.BorderSize = 0;
		this.rjButton20.Enabled = false;
		this.rjButton20.FlatAppearance.BorderSize = 0;
		this.rjButton20.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton20.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton20.ForeColor = System.Drawing.Color.White;
		this.rjButton20.Location = new System.Drawing.Point(811, 185);
		this.rjButton20.Name = "rjButton20";
		this.rjButton20.Size = new System.Drawing.Size(74, 31);
		this.rjButton20.TabIndex = 89;
		this.rjButton20.Text = "Start";
		this.rjButton20.TextColor = System.Drawing.Color.White;
		this.rjButton20.UseVisualStyleBackColor = false;
		this.rjButton20.Click += new System.EventHandler(rjButton20_Click);
		this.label16.AutoSize = true;
		this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label16.Location = new System.Drawing.Point(728, 166);
		this.label16.Name = "label16";
		this.label16.Size = new System.Drawing.Size(45, 16);
		this.label16.TabIndex = 88;
		this.label16.Text = "Train3";
		this.rjButton21.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton21.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton21.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton21.BorderRadius = 0;
		this.rjButton21.BorderSize = 0;
		this.rjButton21.Enabled = false;
		this.rjButton21.FlatAppearance.BorderSize = 0;
		this.rjButton21.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton21.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton21.ForeColor = System.Drawing.Color.White;
		this.rjButton21.Location = new System.Drawing.Point(731, 185);
		this.rjButton21.Name = "rjButton21";
		this.rjButton21.Size = new System.Drawing.Size(74, 31);
		this.rjButton21.TabIndex = 87;
		this.rjButton21.Text = "Start";
		this.rjButton21.TextColor = System.Drawing.Color.White;
		this.rjButton21.UseVisualStyleBackColor = false;
		this.rjButton21.Click += new System.EventHandler(rjButton21_Click);
		this.label17.AutoSize = true;
		this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label17.Location = new System.Drawing.Point(648, 166);
		this.label17.Name = "label17";
		this.label17.Size = new System.Drawing.Size(45, 16);
		this.label17.TabIndex = 86;
		this.label17.Text = "Train2";
		this.rjButton22.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton22.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton22.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton22.BorderRadius = 0;
		this.rjButton22.BorderSize = 0;
		this.rjButton22.Enabled = false;
		this.rjButton22.FlatAppearance.BorderSize = 0;
		this.rjButton22.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton22.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton22.ForeColor = System.Drawing.Color.White;
		this.rjButton22.Location = new System.Drawing.Point(651, 185);
		this.rjButton22.Name = "rjButton22";
		this.rjButton22.Size = new System.Drawing.Size(74, 31);
		this.rjButton22.TabIndex = 85;
		this.rjButton22.Text = "Start";
		this.rjButton22.TextColor = System.Drawing.Color.White;
		this.rjButton22.UseVisualStyleBackColor = false;
		this.rjButton22.Click += new System.EventHandler(rjButton22_Click);
		this.label18.AutoSize = true;
		this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label18.Location = new System.Drawing.Point(568, 166);
		this.label18.Name = "label18";
		this.label18.Size = new System.Drawing.Size(45, 16);
		this.label18.TabIndex = 84;
		this.label18.Text = "Train1";
		this.rjButton23.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton23.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton23.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton23.BorderRadius = 0;
		this.rjButton23.BorderSize = 0;
		this.rjButton23.Enabled = false;
		this.rjButton23.FlatAppearance.BorderSize = 0;
		this.rjButton23.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton23.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton23.ForeColor = System.Drawing.Color.White;
		this.rjButton23.Location = new System.Drawing.Point(571, 185);
		this.rjButton23.Name = "rjButton23";
		this.rjButton23.Size = new System.Drawing.Size(74, 31);
		this.rjButton23.TabIndex = 83;
		this.rjButton23.Text = "Start";
		this.rjButton23.TextColor = System.Drawing.Color.White;
		this.rjButton23.UseVisualStyleBackColor = false;
		this.rjButton23.Click += new System.EventHandler(rjButton23_Click);
		this.label13.AutoSize = true;
		this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label13.Location = new System.Drawing.Point(728, 104);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(48, 16);
		this.label13.TabIndex = 92;
		this.label13.Text = "VWide";
		this.rjButton18.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton18.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton18.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton18.BorderRadius = 0;
		this.rjButton18.BorderSize = 0;
		this.rjButton18.Enabled = false;
		this.rjButton18.FlatAppearance.BorderSize = 0;
		this.rjButton18.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton18.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton18.ForeColor = System.Drawing.Color.White;
		this.rjButton18.Location = new System.Drawing.Point(731, 123);
		this.rjButton18.Name = "rjButton18";
		this.rjButton18.Size = new System.Drawing.Size(74, 31);
		this.rjButton18.TabIndex = 91;
		this.rjButton18.Text = "Start";
		this.rjButton18.TextColor = System.Drawing.Color.White;
		this.rjButton18.UseVisualStyleBackColor = false;
		this.rjButton18.Click += new System.EventHandler(rjButton18_Click);
		this.label14.AutoSize = true;
		this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label14.Location = new System.Drawing.Point(568, 104);
		this.label14.Name = "label14";
		this.label14.Size = new System.Drawing.Size(50, 16);
		this.label14.TabIndex = 94;
		this.label14.Text = "VSmelt";
		this.rjButton19.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton19.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton19.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton19.BorderRadius = 0;
		this.rjButton19.BorderSize = 0;
		this.rjButton19.Enabled = false;
		this.rjButton19.FlatAppearance.BorderSize = 0;
		this.rjButton19.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton19.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton19.ForeColor = System.Drawing.Color.White;
		this.rjButton19.Location = new System.Drawing.Point(571, 123);
		this.rjButton19.Name = "rjButton19";
		this.rjButton19.Size = new System.Drawing.Size(74, 31);
		this.rjButton19.TabIndex = 93;
		this.rjButton19.Text = "Start";
		this.rjButton19.TextColor = System.Drawing.Color.White;
		this.rjButton19.UseVisualStyleBackColor = false;
		this.rjButton19.Click += new System.EventHandler(rjButton19_Click);
		this.label19.AutoSize = true;
		this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label19.Location = new System.Drawing.Point(808, 225);
		this.label19.Name = "label19";
		this.label19.Size = new System.Drawing.Size(62, 16);
		this.label19.TabIndex = 102;
		this.label19.Text = "DumpVD";
		this.rjButton24.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton24.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton24.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton24.BorderRadius = 0;
		this.rjButton24.BorderSize = 0;
		this.rjButton24.Enabled = false;
		this.rjButton24.FlatAppearance.BorderSize = 0;
		this.rjButton24.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton24.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton24.ForeColor = System.Drawing.Color.White;
		this.rjButton24.Location = new System.Drawing.Point(811, 244);
		this.rjButton24.Name = "rjButton24";
		this.rjButton24.Size = new System.Drawing.Size(74, 31);
		this.rjButton24.TabIndex = 101;
		this.rjButton24.Text = "Start";
		this.rjButton24.TextColor = System.Drawing.Color.White;
		this.rjButton24.UseVisualStyleBackColor = false;
		this.rjButton24.Click += new System.EventHandler(rjButton24_Click);
		this.label20.AutoSize = true;
		this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label20.Location = new System.Drawing.Point(728, 225);
		this.label20.Name = "label20";
		this.label20.Size = new System.Drawing.Size(55, 16);
		this.label20.TabIndex = 100;
		this.label20.Text = "Setpixel";
		this.rjButton25.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton25.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton25.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton25.BorderRadius = 0;
		this.rjButton25.BorderSize = 0;
		this.rjButton25.Enabled = false;
		this.rjButton25.FlatAppearance.BorderSize = 0;
		this.rjButton25.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton25.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton25.ForeColor = System.Drawing.Color.White;
		this.rjButton25.Location = new System.Drawing.Point(731, 244);
		this.rjButton25.Name = "rjButton25";
		this.rjButton25.Size = new System.Drawing.Size(74, 31);
		this.rjButton25.TabIndex = 99;
		this.rjButton25.Text = "Start";
		this.rjButton25.TextColor = System.Drawing.Color.White;
		this.rjButton25.UseVisualStyleBackColor = false;
		this.rjButton25.Click += new System.EventHandler(rjButton25_Click);
		this.label21.AutoSize = true;
		this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label21.Location = new System.Drawing.Point(648, 225);
		this.label21.Name = "label21";
		this.label21.Size = new System.Drawing.Size(46, 16);
		this.label21.TabIndex = 98;
		this.label21.Text = "Shake";
		this.rjButton26.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton26.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton26.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton26.BorderRadius = 0;
		this.rjButton26.BorderSize = 0;
		this.rjButton26.Enabled = false;
		this.rjButton26.FlatAppearance.BorderSize = 0;
		this.rjButton26.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton26.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton26.ForeColor = System.Drawing.Color.White;
		this.rjButton26.Location = new System.Drawing.Point(651, 244);
		this.rjButton26.Name = "rjButton26";
		this.rjButton26.Size = new System.Drawing.Size(74, 31);
		this.rjButton26.TabIndex = 97;
		this.rjButton26.Text = "Start";
		this.rjButton26.TextColor = System.Drawing.Color.White;
		this.rjButton26.UseVisualStyleBackColor = false;
		this.rjButton26.Click += new System.EventHandler(rjButton26_Click);
		this.label22.AutoSize = true;
		this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label22.Location = new System.Drawing.Point(568, 225);
		this.label22.Name = "label22";
		this.label22.Size = new System.Drawing.Size(73, 16);
		this.label22.TabIndex = 96;
		this.label22.Text = "Sinewaves";
		this.rjButton27.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton27.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton27.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton27.BorderRadius = 0;
		this.rjButton27.BorderSize = 0;
		this.rjButton27.Enabled = false;
		this.rjButton27.FlatAppearance.BorderSize = 0;
		this.rjButton27.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton27.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton27.ForeColor = System.Drawing.Color.White;
		this.rjButton27.Location = new System.Drawing.Point(571, 244);
		this.rjButton27.Name = "rjButton27";
		this.rjButton27.Size = new System.Drawing.Size(74, 31);
		this.rjButton27.TabIndex = 95;
		this.rjButton27.Text = "Start";
		this.rjButton27.TextColor = System.Drawing.Color.White;
		this.rjButton27.UseVisualStyleBackColor = false;
		this.rjButton27.Click += new System.EventHandler(rjButton27_Click);
		this.label23.AutoSize = true;
		this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label23.Location = new System.Drawing.Point(808, 284);
		this.label23.Name = "label23";
		this.label23.Size = new System.Drawing.Size(71, 16);
		this.label23.TabIndex = 110;
		this.label23.Text = "InvertColor";
		this.rjButton28.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton28.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton28.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton28.BorderRadius = 0;
		this.rjButton28.BorderSize = 0;
		this.rjButton28.Enabled = false;
		this.rjButton28.FlatAppearance.BorderSize = 0;
		this.rjButton28.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton28.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton28.ForeColor = System.Drawing.Color.White;
		this.rjButton28.Location = new System.Drawing.Point(811, 303);
		this.rjButton28.Name = "rjButton28";
		this.rjButton28.Size = new System.Drawing.Size(74, 31);
		this.rjButton28.TabIndex = 109;
		this.rjButton28.Text = "Start";
		this.rjButton28.TextColor = System.Drawing.Color.White;
		this.rjButton28.UseVisualStyleBackColor = false;
		this.rjButton28.Click += new System.EventHandler(rjButton28_Click);
		this.label24.AutoSize = true;
		this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label24.Location = new System.Drawing.Point(728, 284);
		this.label24.Name = "label24";
		this.label24.Size = new System.Drawing.Size(48, 16);
		this.label24.TabIndex = 108;
		this.label24.Text = "Tunnel";
		this.rjButton29.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton29.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton29.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton29.BorderRadius = 0;
		this.rjButton29.BorderSize = 0;
		this.rjButton29.Enabled = false;
		this.rjButton29.FlatAppearance.BorderSize = 0;
		this.rjButton29.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton29.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton29.ForeColor = System.Drawing.Color.White;
		this.rjButton29.Location = new System.Drawing.Point(731, 303);
		this.rjButton29.Name = "rjButton29";
		this.rjButton29.Size = new System.Drawing.Size(74, 31);
		this.rjButton29.TabIndex = 107;
		this.rjButton29.Text = "Start";
		this.rjButton29.TextColor = System.Drawing.Color.White;
		this.rjButton29.UseVisualStyleBackColor = false;
		this.rjButton29.Click += new System.EventHandler(rjButton29_Click);
		this.label25.AutoSize = true;
		this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label25.Location = new System.Drawing.Point(648, 284);
		this.label25.Name = "label25";
		this.label25.Size = new System.Drawing.Size(49, 16);
		this.label25.TabIndex = 106;
		this.label25.Text = "Stripes";
		this.rjButton30.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton30.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton30.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton30.BorderRadius = 0;
		this.rjButton30.BorderSize = 0;
		this.rjButton30.Enabled = false;
		this.rjButton30.FlatAppearance.BorderSize = 0;
		this.rjButton30.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton30.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton30.ForeColor = System.Drawing.Color.White;
		this.rjButton30.Location = new System.Drawing.Point(651, 303);
		this.rjButton30.Name = "rjButton30";
		this.rjButton30.Size = new System.Drawing.Size(74, 31);
		this.rjButton30.TabIndex = 105;
		this.rjButton30.Text = "Start";
		this.rjButton30.TextColor = System.Drawing.Color.White;
		this.rjButton30.UseVisualStyleBackColor = false;
		this.rjButton30.Click += new System.EventHandler(rjButton30_Click);
		this.label26.AutoSize = true;
		this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label26.Location = new System.Drawing.Point(577, 284);
		this.label26.Name = "label26";
		this.label26.Size = new System.Drawing.Size(36, 16);
		this.label26.TabIndex = 104;
		this.label26.Text = "Dark";
		this.rjButton31.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton31.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton31.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton31.BorderRadius = 0;
		this.rjButton31.BorderSize = 0;
		this.rjButton31.Enabled = false;
		this.rjButton31.FlatAppearance.BorderSize = 0;
		this.rjButton31.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton31.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton31.ForeColor = System.Drawing.Color.White;
		this.rjButton31.Location = new System.Drawing.Point(571, 303);
		this.rjButton31.Name = "rjButton31";
		this.rjButton31.Size = new System.Drawing.Size(74, 31);
		this.rjButton31.TabIndex = 103;
		this.rjButton31.Text = "Start";
		this.rjButton31.TextColor = System.Drawing.Color.White;
		this.rjButton31.UseVisualStyleBackColor = false;
		this.rjButton31.Click += new System.EventHandler(rjButton31_Click);
		this.rjTextBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Enabled = false;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox1.Location = new System.Drawing.Point(33, 310);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "Url";
		this.rjTextBox1.Size = new System.Drawing.Size(174, 31);
		this.rjTextBox1.TabIndex = 111;
		this.rjTextBox1.Texts = "https://google.com";
		this.rjTextBox1.UnderlinedStyle = false;
		this.label27.AutoSize = true;
		this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label27.Location = new System.Drawing.Point(30, 290);
		this.label27.Name = "label27";
		this.label27.Size = new System.Drawing.Size(67, 16);
		this.label27.TabIndex = 112;
		this.label27.Text = "Open Link";
		this.rjButton32.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton32.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton32.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton32.BorderRadius = 0;
		this.rjButton32.BorderSize = 0;
		this.rjButton32.Enabled = false;
		this.rjButton32.FlatAppearance.BorderSize = 0;
		this.rjButton32.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton32.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton32.ForeColor = System.Drawing.Color.White;
		this.rjButton32.Location = new System.Drawing.Point(213, 311);
		this.rjButton32.Name = "rjButton32";
		this.rjButton32.Size = new System.Drawing.Size(79, 31);
		this.rjButton32.TabIndex = 113;
		this.rjButton32.Text = "Open";
		this.rjButton32.TextColor = System.Drawing.Color.White;
		this.rjButton32.UseVisualStyleBackColor = false;
		this.rjButton32.Click += new System.EventHandler(rjButton32_Click);
		this.rjButton33.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton33.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton33.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton33.BorderRadius = 0;
		this.rjButton33.BorderSize = 0;
		this.rjButton33.Enabled = false;
		this.rjButton33.FlatAppearance.BorderSize = 0;
		this.rjButton33.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton33.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton33.ForeColor = System.Drawing.Color.White;
		this.rjButton33.Location = new System.Drawing.Point(309, 401);
		this.rjButton33.Name = "rjButton33";
		this.rjButton33.Size = new System.Drawing.Size(74, 31);
		this.rjButton33.TabIndex = 117;
		this.rjButton33.Text = "Dark";
		this.rjButton33.TextColor = System.Drawing.Color.White;
		this.rjButton33.UseVisualStyleBackColor = false;
		this.rjButton33.Click += new System.EventHandler(rjButton33_Click);
		this.rjButton34.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton34.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton34.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton34.BorderRadius = 0;
		this.rjButton34.BorderSize = 0;
		this.rjButton34.Enabled = false;
		this.rjButton34.FlatAppearance.BorderSize = 0;
		this.rjButton34.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton34.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton34.ForeColor = System.Drawing.Color.White;
		this.rjButton34.Location = new System.Drawing.Point(469, 401);
		this.rjButton34.Name = "rjButton34";
		this.rjButton34.Size = new System.Drawing.Size(74, 31);
		this.rjButton34.TabIndex = 116;
		this.rjButton34.Text = "Blue";
		this.rjButton34.TextColor = System.Drawing.Color.White;
		this.rjButton34.UseVisualStyleBackColor = false;
		this.rjButton34.Click += new System.EventHandler(rjButton34_Click);
		this.rjButton35.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton35.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton35.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton35.BorderRadius = 0;
		this.rjButton35.BorderSize = 0;
		this.rjButton35.Enabled = false;
		this.rjButton35.FlatAppearance.BorderSize = 0;
		this.rjButton35.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton35.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton35.ForeColor = System.Drawing.Color.White;
		this.rjButton35.Location = new System.Drawing.Point(549, 401);
		this.rjButton35.Name = "rjButton35";
		this.rjButton35.Size = new System.Drawing.Size(74, 31);
		this.rjButton35.TabIndex = 115;
		this.rjButton35.Text = "Green";
		this.rjButton35.TextColor = System.Drawing.Color.White;
		this.rjButton35.UseVisualStyleBackColor = false;
		this.rjButton35.Click += new System.EventHandler(rjButton35_Click);
		this.rjButton36.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton36.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton36.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton36.BorderRadius = 0;
		this.rjButton36.BorderSize = 0;
		this.rjButton36.Enabled = false;
		this.rjButton36.FlatAppearance.BorderSize = 0;
		this.rjButton36.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton36.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton36.ForeColor = System.Drawing.Color.White;
		this.rjButton36.Location = new System.Drawing.Point(389, 401);
		this.rjButton36.Name = "rjButton36";
		this.rjButton36.Size = new System.Drawing.Size(74, 31);
		this.rjButton36.TabIndex = 114;
		this.rjButton36.Text = "Cyan";
		this.rjButton36.TextColor = System.Drawing.Color.White;
		this.rjButton36.UseVisualStyleBackColor = false;
		this.rjButton36.Click += new System.EventHandler(rjButton36_Click);
		this.label28.AutoSize = true;
		this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label28.Location = new System.Drawing.Point(385, 345);
		this.label28.Name = "label28";
		this.label28.Size = new System.Drawing.Size(112, 16);
		this.label28.TabIndex = 118;
		this.label28.Text = "Monitor Settings :)";
		this.rjButton37.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton37.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton37.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton37.BorderRadius = 0;
		this.rjButton37.BorderSize = 0;
		this.rjButton37.Enabled = false;
		this.rjButton37.FlatAppearance.BorderSize = 0;
		this.rjButton37.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton37.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton37.ForeColor = System.Drawing.Color.White;
		this.rjButton37.Location = new System.Drawing.Point(309, 364);
		this.rjButton37.Name = "rjButton37";
		this.rjButton37.Size = new System.Drawing.Size(74, 31);
		this.rjButton37.TabIndex = 122;
		this.rjButton37.Text = "Default";
		this.rjButton37.TextColor = System.Drawing.Color.White;
		this.rjButton37.UseVisualStyleBackColor = false;
		this.rjButton37.Click += new System.EventHandler(rjButton37_Click);
		this.rjButton38.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton38.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton38.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton38.BorderRadius = 0;
		this.rjButton38.BorderSize = 0;
		this.rjButton38.Enabled = false;
		this.rjButton38.FlatAppearance.BorderSize = 0;
		this.rjButton38.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton38.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton38.ForeColor = System.Drawing.Color.White;
		this.rjButton38.Location = new System.Drawing.Point(469, 364);
		this.rjButton38.Name = "rjButton38";
		this.rjButton38.Size = new System.Drawing.Size(74, 31);
		this.rjButton38.TabIndex = 121;
		this.rjButton38.Text = "Purple";
		this.rjButton38.TextColor = System.Drawing.Color.White;
		this.rjButton38.UseVisualStyleBackColor = false;
		this.rjButton38.Click += new System.EventHandler(rjButton38_Click);
		this.rjButton39.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton39.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton39.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton39.BorderRadius = 0;
		this.rjButton39.BorderSize = 0;
		this.rjButton39.Enabled = false;
		this.rjButton39.FlatAppearance.BorderSize = 0;
		this.rjButton39.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton39.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton39.ForeColor = System.Drawing.Color.White;
		this.rjButton39.Location = new System.Drawing.Point(549, 364);
		this.rjButton39.Name = "rjButton39";
		this.rjButton39.Size = new System.Drawing.Size(74, 31);
		this.rjButton39.TabIndex = 120;
		this.rjButton39.Text = "Yellow";
		this.rjButton39.TextColor = System.Drawing.Color.White;
		this.rjButton39.UseVisualStyleBackColor = false;
		this.rjButton39.Click += new System.EventHandler(rjButton39_Click);
		this.rjButton40.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton40.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton40.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton40.BorderRadius = 0;
		this.rjButton40.BorderSize = 0;
		this.rjButton40.Enabled = false;
		this.rjButton40.FlatAppearance.BorderSize = 0;
		this.rjButton40.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton40.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton40.ForeColor = System.Drawing.Color.White;
		this.rjButton40.Location = new System.Drawing.Point(389, 364);
		this.rjButton40.Name = "rjButton40";
		this.rjButton40.Size = new System.Drawing.Size(74, 31);
		this.rjButton40.TabIndex = 119;
		this.rjButton40.Text = "Red";
		this.rjButton40.TextColor = System.Drawing.Color.White;
		this.rjButton40.UseVisualStyleBackColor = false;
		this.rjButton40.Click += new System.EventHandler(rjButton40_Click);
		this.label29.AutoSize = true;
		this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label29.Location = new System.Drawing.Point(243, 138);
		this.label29.Name = "label29";
		this.label29.Size = new System.Drawing.Size(85, 16);
		this.label29.TabIndex = 124;
		this.label29.Text = "Mouse Swap";
		this.rjButton41.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton41.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton41.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton41.BorderRadius = 0;
		this.rjButton41.BorderSize = 0;
		this.rjButton41.Enabled = false;
		this.rjButton41.FlatAppearance.BorderSize = 0;
		this.rjButton41.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton41.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton41.ForeColor = System.Drawing.Color.White;
		this.rjButton41.Location = new System.Drawing.Point(246, 157);
		this.rjButton41.Name = "rjButton41";
		this.rjButton41.Size = new System.Drawing.Size(74, 31);
		this.rjButton41.TabIndex = 123;
		this.rjButton41.Text = "Swap";
		this.rjButton41.TextColor = System.Drawing.Color.White;
		this.rjButton41.UseVisualStyleBackColor = false;
		this.rjButton41.Click += new System.EventHandler(rjButton41_Click);
		this.label30.AutoSize = true;
		this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label30.Location = new System.Drawing.Point(130, 200);
		this.label30.Name = "label30";
		this.label30.Size = new System.Drawing.Size(90, 16);
		this.label30.TabIndex = 126;
		this.label30.Text = "Crash System";
		this.rjButton42.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton42.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton42.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton42.BorderRadius = 0;
		this.rjButton42.BorderSize = 0;
		this.rjButton42.Enabled = false;
		this.rjButton42.FlatAppearance.BorderSize = 0;
		this.rjButton42.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton42.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton42.ForeColor = System.Drawing.Color.White;
		this.rjButton42.Location = new System.Drawing.Point(133, 219);
		this.rjButton42.Name = "rjButton42";
		this.rjButton42.Size = new System.Drawing.Size(74, 31);
		this.rjButton42.TabIndex = 125;
		this.rjButton42.Text = "Crash!";
		this.rjButton42.TextColor = System.Drawing.Color.White;
		this.rjButton42.UseVisualStyleBackColor = false;
		this.rjButton42.Click += new System.EventHandler(rjButton42_Click);
		this.label31.AutoSize = true;
		this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label31.Location = new System.Drawing.Point(959, 100);
		this.label31.Name = "label31";
		this.label31.Size = new System.Drawing.Size(90, 16);
		this.label31.TabIndex = 127;
		this.label31.Text = "Message Box";
		this.rjTextBox2.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox2.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox2.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox2.BorderRadius = 0;
		this.rjTextBox2.BorderSize = 1;
		this.rjTextBox2.Enabled = false;
		this.rjTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox2.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox2.Location = new System.Drawing.Point(904, 123);
		this.rjTextBox2.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox2.Multiline = false;
		this.rjTextBox2.Name = "rjTextBox2";
		this.rjTextBox2.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox2.PasswordChar = false;
		this.rjTextBox2.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox2.PlaceholderText = "Caption";
		this.rjTextBox2.Size = new System.Drawing.Size(217, 31);
		this.rjTextBox2.TabIndex = 128;
		this.rjTextBox2.Texts = "";
		this.rjTextBox2.UnderlinedStyle = false;
		this.rjTextBox3.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox3.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox3.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox3.BorderRadius = 0;
		this.rjTextBox3.BorderSize = 1;
		this.rjTextBox3.Enabled = false;
		this.rjTextBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox3.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox3.Location = new System.Drawing.Point(904, 162);
		this.rjTextBox3.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox3.Multiline = false;
		this.rjTextBox3.Name = "rjTextBox3";
		this.rjTextBox3.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox3.PasswordChar = false;
		this.rjTextBox3.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox3.PlaceholderText = "Text";
		this.rjTextBox3.Size = new System.Drawing.Size(217, 31);
		this.rjTextBox3.TabIndex = 129;
		this.rjTextBox3.Texts = "";
		this.rjTextBox3.UnderlinedStyle = false;
		this.rjButton43.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton43.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton43.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton43.BorderRadius = 0;
		this.rjButton43.BorderSize = 0;
		this.rjButton43.Enabled = false;
		this.rjButton43.FlatAppearance.BorderSize = 0;
		this.rjButton43.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton43.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton43.ForeColor = System.Drawing.Color.White;
		this.rjButton43.Location = new System.Drawing.Point(1045, 277);
		this.rjButton43.Name = "rjButton43";
		this.rjButton43.Size = new System.Drawing.Size(74, 31);
		this.rjButton43.TabIndex = 130;
		this.rjButton43.Text = "Send";
		this.rjButton43.TextColor = System.Drawing.Color.White;
		this.rjButton43.UseVisualStyleBackColor = false;
		this.rjButton43.Click += new System.EventHandler(rjButton43_Click);
		this.rjButton44.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton44.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton44.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton44.BorderRadius = 0;
		this.rjButton44.BorderSize = 0;
		this.rjButton44.Enabled = false;
		this.rjButton44.FlatAppearance.BorderSize = 0;
		this.rjButton44.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton44.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton44.ForeColor = System.Drawing.Color.White;
		this.rjButton44.Location = new System.Drawing.Point(962, 277);
		this.rjButton44.Name = "rjButton44";
		this.rjButton44.Size = new System.Drawing.Size(74, 31);
		this.rjButton44.TabIndex = 131;
		this.rjButton44.Text = "Spam";
		this.rjButton44.TextColor = System.Drawing.Color.White;
		this.rjButton44.UseVisualStyleBackColor = false;
		this.rjButton44.Click += new System.EventHandler(rjButton44_Click);
		this.rjComboBox1.BackColor = System.Drawing.Color.White;
		this.rjComboBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.BorderSize = 1;
		this.rjComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox1.Enabled = false;
		this.rjComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox1.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.Items.AddRange(new object[6] { "OK", "OKCancel", "AbortRetryIgnore", "YesNoCancel", "YesNo", "RetryCancel" });
		this.rjComboBox1.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox1.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.Location = new System.Drawing.Point(903, 204);
		this.rjComboBox1.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox1.Name = "rjComboBox1";
		this.rjComboBox1.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox1.Size = new System.Drawing.Size(218, 30);
		this.rjComboBox1.TabIndex = 132;
		this.rjComboBox1.Texts = "";
		this.rjComboBox3.BackColor = System.Drawing.Color.White;
		this.rjComboBox3.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox3.BorderSize = 1;
		this.rjComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox3.Enabled = false;
		this.rjComboBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox3.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox3.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox3.Items.AddRange(new object[5] { "None", "Error", "Question", "Warning", "Information" });
		this.rjComboBox3.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox3.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox3.Location = new System.Drawing.Point(903, 241);
		this.rjComboBox3.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox3.Name = "rjComboBox3";
		this.rjComboBox3.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox3.Size = new System.Drawing.Size(218, 30);
		this.rjComboBox3.TabIndex = 133;
		this.rjComboBox3.Texts = "";
		this.rjButton45.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton45.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton45.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton45.BorderRadius = 0;
		this.rjButton45.BorderSize = 0;
		this.rjButton45.Enabled = false;
		this.rjButton45.FlatAppearance.BorderSize = 0;
		this.rjButton45.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton45.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton45.ForeColor = System.Drawing.Color.White;
		this.rjButton45.Location = new System.Drawing.Point(389, 311);
		this.rjButton45.Name = "rjButton45";
		this.rjButton45.Size = new System.Drawing.Size(74, 31);
		this.rjButton45.TabIndex = 134;
		this.rjButton45.Text = "Start";
		this.rjButton45.TextColor = System.Drawing.Color.White;
		this.rjButton45.UseVisualStyleBackColor = false;
		this.rjButton45.Click += new System.EventHandler(rjButton45_Click);
		this.label32.AutoSize = true;
		this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label32.Location = new System.Drawing.Point(386, 293);
		this.label32.Name = "label32";
		this.label32.Size = new System.Drawing.Size(50, 16);
		this.label32.TabIndex = 135;
		this.label32.Text = "Screen";
		this.label33.AutoSize = true;
		this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label33.Location = new System.Drawing.Point(306, 292);
		this.label33.Name = "label33";
		this.label33.Size = new System.Drawing.Size(30, 16);
		this.label33.TabIndex = 137;
		this.label33.Text = "Led";
		this.rjButton46.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton46.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton46.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton46.BorderRadius = 0;
		this.rjButton46.BorderSize = 0;
		this.rjButton46.Enabled = false;
		this.rjButton46.FlatAppearance.BorderSize = 0;
		this.rjButton46.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton46.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton46.ForeColor = System.Drawing.Color.White;
		this.rjButton46.Location = new System.Drawing.Point(309, 310);
		this.rjButton46.Name = "rjButton46";
		this.rjButton46.Size = new System.Drawing.Size(74, 31);
		this.rjButton46.TabIndex = 136;
		this.rjButton46.Text = "Start";
		this.rjButton46.TextColor = System.Drawing.Color.White;
		this.rjButton46.UseVisualStyleBackColor = false;
		this.rjButton46.Click += new System.EventHandler(rjButton46_Click);
		this.label34.AutoSize = true;
		this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label34.Location = new System.Drawing.Point(407, 269);
		this.label34.Name = "label34";
		this.label34.Size = new System.Drawing.Size(36, 16);
		this.label34.TabIndex = 139;
		this.label34.Text = "Fuck";
		this.rjButton47.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton47.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton47.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton47.BorderRadius = 0;
		this.rjButton47.BorderSize = 0;
		this.rjButton47.Enabled = false;
		this.rjButton47.FlatAppearance.BorderSize = 0;
		this.rjButton47.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton47.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton47.ForeColor = System.Drawing.Color.White;
		this.rjButton47.Location = new System.Drawing.Point(469, 310);
		this.rjButton47.Name = "rjButton47";
		this.rjButton47.Size = new System.Drawing.Size(74, 31);
		this.rjButton47.TabIndex = 138;
		this.rjButton47.Text = "Start";
		this.rjButton47.TextColor = System.Drawing.Color.White;
		this.rjButton47.UseVisualStyleBackColor = false;
		this.rjButton47.Click += new System.EventHandler(rjButton47_Click);
		this.label35.AutoSize = true;
		this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.6f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label35.Location = new System.Drawing.Point(466, 293);
		this.label35.Name = "label35";
		this.label35.Size = new System.Drawing.Size(70, 16);
		this.label35.TabIndex = 140;
		this.label35.Text = "Key Board";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1238, 464);
		base.Controls.Add(this.label35);
		base.Controls.Add(this.label34);
		base.Controls.Add(this.rjButton47);
		base.Controls.Add(this.label33);
		base.Controls.Add(this.rjButton46);
		base.Controls.Add(this.label32);
		base.Controls.Add(this.rjButton45);
		base.Controls.Add(this.rjComboBox3);
		base.Controls.Add(this.rjComboBox1);
		base.Controls.Add(this.rjButton44);
		base.Controls.Add(this.rjButton43);
		base.Controls.Add(this.rjTextBox3);
		base.Controls.Add(this.rjTextBox2);
		base.Controls.Add(this.label31);
		base.Controls.Add(this.label30);
		base.Controls.Add(this.rjButton42);
		base.Controls.Add(this.label29);
		base.Controls.Add(this.rjButton41);
		base.Controls.Add(this.rjButton37);
		base.Controls.Add(this.rjButton38);
		base.Controls.Add(this.rjButton39);
		base.Controls.Add(this.rjButton40);
		base.Controls.Add(this.label28);
		base.Controls.Add(this.rjButton33);
		base.Controls.Add(this.rjButton34);
		base.Controls.Add(this.rjButton35);
		base.Controls.Add(this.rjButton36);
		base.Controls.Add(this.rjButton32);
		base.Controls.Add(this.label27);
		base.Controls.Add(this.rjTextBox1);
		base.Controls.Add(this.label23);
		base.Controls.Add(this.rjButton28);
		base.Controls.Add(this.label24);
		base.Controls.Add(this.rjButton29);
		base.Controls.Add(this.label25);
		base.Controls.Add(this.rjButton30);
		base.Controls.Add(this.label26);
		base.Controls.Add(this.rjButton31);
		base.Controls.Add(this.label19);
		base.Controls.Add(this.rjButton24);
		base.Controls.Add(this.label20);
		base.Controls.Add(this.rjButton25);
		base.Controls.Add(this.label21);
		base.Controls.Add(this.rjButton26);
		base.Controls.Add(this.label22);
		base.Controls.Add(this.rjButton27);
		base.Controls.Add(this.label14);
		base.Controls.Add(this.rjButton19);
		base.Controls.Add(this.label13);
		base.Controls.Add(this.rjButton18);
		base.Controls.Add(this.label15);
		base.Controls.Add(this.rjButton20);
		base.Controls.Add(this.label16);
		base.Controls.Add(this.rjButton21);
		base.Controls.Add(this.label17);
		base.Controls.Add(this.rjButton22);
		base.Controls.Add(this.label18);
		base.Controls.Add(this.rjButton23);
		base.Controls.Add(this.label12);
		base.Controls.Add(this.rjButton17);
		base.Controls.Add(this.label11);
		base.Controls.Add(this.rjButton16);
		base.Controls.Add(this.rjButton15);
		base.Controls.Add(this.label10);
		base.Controls.Add(this.rjComboBox2);
		base.Controls.Add(this.label9);
		base.Controls.Add(this.label8);
		base.Controls.Add(this.rjButton14);
		base.Controls.Add(this.rjButton13);
		base.Controls.Add(this.rjButton12);
		base.Controls.Add(this.rjButton11);
		base.Controls.Add(this.rjButton10);
		base.Controls.Add(this.rjButton9);
		base.Controls.Add(this.rjButton8);
		base.Controls.Add(this.rjButton7);
		base.Controls.Add(this.label7);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.rjButton6);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.rjButton5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.rjButton4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.rjButton3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.rjButton2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.rjButton1);
		base.Name = "FormFun";
		this.Text = "Fun";
		base.Load += new System.EventHandler(FormFun_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
