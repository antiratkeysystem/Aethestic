using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormHostsFile : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	public RichTextBox richTextBox1;

	public FormHostsFile()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormProcess_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		richTextBox1.KeyDown += RichTextBox1_KeyDown;
	}

	private void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Control && e.KeyCode == Keys.S)
		{
			client.Send(new object[2] { "Save", richTextBox1.Text });
			e.SuppressKeyPress = true;
		}
	}

	private void ChangeScheme(object sender)
	{
		richTextBox1.ForeColor = FormMaterial.PrimaryColor;
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
		this.richTextBox1.Size = new System.Drawing.Size(519, 451);
		this.richTextBox1.TabIndex = 1;
		this.richTextBox1.Text = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(525, 518);
		base.Controls.Add(this.richTextBox1);
		base.Name = "FormHostsFile";
		this.Text = "Hosts File Edit";
		base.Load += new System.EventHandler(FormProcess_Load);
		base.ResumeLayout(false);
	}
}
