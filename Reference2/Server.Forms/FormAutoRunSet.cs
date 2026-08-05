using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormAutoRunSet : FormMaterial
{
	public Clients client;

	private IContainer components;

	private RJButton rjButton1;

	private RJButton rjButton2;

	public RJTextBox rjTextBox1;

	public RJTextBox rjTextBox2;

	private RJComboBox rjComboBox2;

	public FormAutoRunSet()
	{
		InitializeComponent();
	}

	private void FormHVNCrun_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
	}

	private void ChangeScheme(object sender)
	{
		rjTextBox1.BorderColor = FormMaterial.PrimaryColor;
		rjTextBox2.BorderColor = FormMaterial.PrimaryColor;
		rjComboBox2.BorderColor = FormMaterial.PrimaryColor;
		rjButton2.BackColor = FormMaterial.PrimaryColor;
		rjButton1.BackColor = FormMaterial.PrimaryColor;
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(rjTextBox1.Texts) && !string.IsNullOrEmpty(rjTextBox2.Texts) && !string.IsNullOrEmpty(rjComboBox2.Texts))
		{
			client.Send(new object[4] { "Set", rjComboBox2.Texts, rjTextBox2.Texts, rjTextBox1.Texts });
			Close();
		}
	}

	private void rjButton2_Click(object sender, EventArgs e)
	{
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
		this.rjTextBox2 = new CustomControls.RJControls.RJTextBox();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.rjComboBox2 = new CustomControls.RJControls.RJComboBox();
		base.SuspendLayout();
		this.rjTextBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox1.Location = new System.Drawing.Point(7, 68);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "Path";
		this.rjTextBox1.Size = new System.Drawing.Size(345, 31);
		this.rjTextBox1.TabIndex = 14;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		this.rjTextBox2.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox2.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox2.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox2.BorderRadius = 0;
		this.rjTextBox2.BorderSize = 1;
		this.rjTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox2.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox2.Location = new System.Drawing.Point(7, 107);
		this.rjTextBox2.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox2.Multiline = false;
		this.rjTextBox2.Name = "rjTextBox2";
		this.rjTextBox2.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox2.PasswordChar = false;
		this.rjTextBox2.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox2.PlaceholderText = "Name";
		this.rjTextBox2.Size = new System.Drawing.Size(345, 31);
		this.rjTextBox2.TabIndex = 15;
		this.rjTextBox2.Texts = "";
		this.rjTextBox2.UnderlinedStyle = false;
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(286, 183);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(64, 31);
		this.rjButton1.TabIndex = 16;
		this.rjButton1.Text = "Set";
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
		this.rjButton2.Location = new System.Drawing.Point(216, 183);
		this.rjButton2.Name = "rjButton2";
		this.rjButton2.Size = new System.Drawing.Size(64, 31);
		this.rjButton2.TabIndex = 17;
		this.rjButton2.Text = "Cancel";
		this.rjButton2.TextColor = System.Drawing.Color.White;
		this.rjButton2.UseVisualStyleBackColor = false;
		this.rjButton2.Click += new System.EventHandler(rjButton2_Click);
		this.rjComboBox2.BackColor = System.Drawing.Color.White;
		this.rjComboBox2.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.BorderSize = 1;
		this.rjComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox2.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox2.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.Items.AddRange(new object[6] { "StartUp", "CurrentUser", "LocalMachine", "Node32", "UserInit", "Sheduler" });
		this.rjComboBox2.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox2.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox2.Location = new System.Drawing.Point(7, 145);
		this.rjComboBox2.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox2.Name = "rjComboBox2";
		this.rjComboBox2.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox2.Size = new System.Drawing.Size(343, 30);
		this.rjComboBox2.TabIndex = 22;
		this.rjComboBox2.Texts = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(359, 220);
		base.Controls.Add(this.rjComboBox2);
		base.Controls.Add(this.rjButton2);
		base.Controls.Add(this.rjButton1);
		base.Controls.Add(this.rjTextBox2);
		base.Controls.Add(this.rjTextBox1);
		base.Name = "FormAutoRunSet";
		this.Text = "Auto Run Set";
		base.Load += new System.EventHandler(FormHVNCrun_Load);
		base.ResumeLayout(false);
	}
}
