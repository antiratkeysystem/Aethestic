using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Helper;

namespace Server.Forms;

public class FormInput : FormMaterial
{
	public bool Run;

	private IContainer components;

	private RJButton rjButton2;

	private RJButton rjButton1;

	public RJTextBox rjTextBox1;

	public Label label1;

	public FormInput()
	{
		InitializeComponent();
	}

	private void FormInput_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		rjTextBox1.textBox1.KeyDown += rjTextBox1_KeyDown;
		rjTextBox1.Focus();
	}

	private void rjTextBox1_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			Run = true;
			base.DialogResult = DialogResult.OK;
			Close();
			e.SuppressKeyPress = true;
		}
		if (e.KeyCode == Keys.Escape)
		{
			base.DialogResult = DialogResult.Cancel;
			Close();
			e.SuppressKeyPress = true;
		}
	}

	private void ChangeScheme(object sender)
	{
		rjButton1.BackColor = FormMaterial.PrimaryColor;
		rjButton2.BackColor = FormMaterial.PrimaryColor;
		rjTextBox1.BorderColor = FormMaterial.PrimaryColor;
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		Run = true;
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void rjButton2_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
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
		this.label1 = new System.Windows.Forms.Label();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Arial", 10f);
		this.label1.ForeColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.label1.Location = new System.Drawing.Point(25, 66);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(46, 16);
		this.label1.TabIndex = 21;
		this.label1.Text = "Input:";
		this.rjButton2.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton2.BorderRadius = 0;
		this.rjButton2.BorderSize = 0;
		this.rjButton2.FlatAppearance.BorderSize = 0;
		this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton2.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButton2.ForeColor = System.Drawing.Color.White;
		this.rjButton2.Location = new System.Drawing.Point(144, 136);
		this.rjButton2.Name = "rjButton2";
		this.rjButton2.Size = new System.Drawing.Size(64, 31);
		this.rjButton2.TabIndex = 20;
		this.rjButton2.Text = "Cancel";
		this.rjButton2.TextColor = System.Drawing.Color.White;
		this.rjButton2.UseVisualStyleBackColor = false;
		this.rjButton2.Click += new System.EventHandler(rjButton2_Click);
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(214, 136);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(64, 31);
		this.rjButton1.TabIndex = 19;
		this.rjButton1.Text = "Run";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.rjTextBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox1.Location = new System.Drawing.Point(28, 86);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "";
		this.rjTextBox1.Size = new System.Drawing.Size(250, 31);
		this.rjTextBox1.TabIndex = 18;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(312, 187);
		base.Controls.Add(this.rjButton2);
		base.Controls.Add(this.rjButton1);
		base.Controls.Add(this.rjTextBox1);
		base.Controls.Add(this.label1);
		base.Name = "FormInput";
		this.Text = "FormInput";
		base.Load += new System.EventHandler(FormInput_Load);
		base.ResumeLayout(false);
	}
}
