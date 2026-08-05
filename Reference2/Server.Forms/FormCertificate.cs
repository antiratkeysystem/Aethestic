using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormCertificate : FormMaterial
{
	private IContainer components;

	private RJTextBox rjTextBox1;

	private RJButton rjButton1;

	public FormCertificate()
	{
		InitializeComponent();
	}

	private void FormAbout_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
	}

	private void ChangeScheme(object sender)
	{
		rjTextBox1.BorderColor = FormMaterial.PrimaryColor;
		rjButton1.BorderColor = FormMaterial.PrimaryColor;
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		rjButton1.Text = "Wait...";
		Certificate.CreateCertificateAuthority(rjTextBox1.Texts);
		Close();
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
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		base.SuspendLayout();
		this.rjTextBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.DarkViolet;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 2;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox1.Location = new System.Drawing.Point(7, 68);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "Ca Name";
		this.rjTextBox1.Size = new System.Drawing.Size(250, 31);
		this.rjTextBox1.TabIndex = 2;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		this.rjButton1.BackColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.BackgroundColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(6, 106);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(251, 29);
		this.rjButton1.TabIndex = 3;
		this.rjButton1.Text = "Create";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(264, 143);
		base.Controls.Add(this.rjTextBox1);
		base.Controls.Add(this.rjButton1);
		base.Name = "FormCertificate";
		this.Text = "Certificate Create";
		base.Load += new System.EventHandler(FormAbout_Load);
		base.ResumeLayout(false);
	}
}
