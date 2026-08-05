using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Helper;

namespace Server.Forms;

public class FormSearch : FormMaterial
{
	private Form1 mainForm;

	private IContainer components;

	private RJTextBox rjTextBox7;

	private RJButton rjButton14;

	private RJButton rjButton13;

	public FormSearch(Form1 form)
	{
		InitializeComponent();
		mainForm = form;
	}

	private void FormSearch_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		BeginInvoke((Action)delegate
		{
			ChangeScheme(this);
		});
		rjTextBox7.Focus();
		rjTextBox7.textBox1.KeyDown += RjTextBox7_KeyDown;
		rjTextBox7.textBox1.TextChanged += RjTextBox7_TextChanged;
		rjButton14.Click += RjButton14_Click;
		rjButton13.Click += RjButton13_Click;
	}

	private void RjTextBox7_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			e.SuppressKeyPress = true;
			Close();
		}
		else if (e.KeyCode == Keys.Escape)
		{
			Close();
		}
	}

	private void RjTextBox7_TextChanged(object sender, EventArgs e)
	{
		PerformLiveSearch();
	}

	private void RjButton14_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void RjButton13_Click(object sender, EventArgs e)
	{
		mainForm.ResetSearch();
		Close();
	}

	private void PerformLiveSearch()
	{
		string searchText = rjTextBox7.textBox1.Text.Trim();
		mainForm.SearchClients(searchText);
	}

	private void ChangeScheme(object sender)
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color text = (num ? Color.WhiteSmoke : Color.Black);
		Color styleColor = (RainbowThemeManager.IsActive() ? RainbowThemeManager.GetStyleColor() : FormMaterial.PrimaryColor);
		BackColor = back;
		rjTextBox7.BackColor = back;
		rjTextBox7.ForeColor = text;
		rjTextBox7.BorderColor = styleColor;
		rjButton14.BackColor = styleColor;
		rjButton13.BackColor = styleColor;
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
		this.rjTextBox7 = new CustomControls.RJControls.RJTextBox();
		this.rjButton14 = new CustomControls.RJControls.RJButton();
		this.rjButton13 = new CustomControls.RJControls.RJButton();
		base.SuspendLayout();
		this.rjTextBox7.BackColor = System.Drawing.Color.White;
		this.rjTextBox7.BorderColor = System.Drawing.Color.FromArgb(210, 180, 140);
		this.rjTextBox7.BorderFocusColor = System.Drawing.Color.FromArgb(210, 180, 140);
		this.rjTextBox7.BorderRadius = 0;
		this.rjTextBox7.BorderSize = 1;
		this.rjTextBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox7.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox7.Location = new System.Drawing.Point(51, 108);
		this.rjTextBox7.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox7.Multiline = false;
		this.rjTextBox7.Name = "rjTextBox7";
		this.rjTextBox7.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox7.PasswordChar = false;
		this.rjTextBox7.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox7.PlaceholderText = "Search";
		this.rjTextBox7.Size = new System.Drawing.Size(240, 31);
		this.rjTextBox7.TabIndex = 32;
		this.rjTextBox7.Texts = "";
		this.rjTextBox7.UnderlinedStyle = false;
		this.rjButton14.BackColor = System.Drawing.Color.FromArgb(210, 180, 140);
		this.rjButton14.BackgroundColor = System.Drawing.Color.FromArgb(210, 180, 140);
		this.rjButton14.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton14.BorderRadius = 0;
		this.rjButton14.BorderSize = 0;
		this.rjButton14.FlatAppearance.BorderSize = 0;
		this.rjButton14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton14.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton14.ForeColor = System.Drawing.Color.White;
		this.rjButton14.Location = new System.Drawing.Point(222, 187);
		this.rjButton14.Name = "rjButton14";
		this.rjButton14.Size = new System.Drawing.Size(118, 31);
		this.rjButton14.TabIndex = 54;
		this.rjButton14.Text = "Search";
		this.rjButton14.TextColor = System.Drawing.Color.White;
		this.rjButton14.UseVisualStyleBackColor = false;
		this.rjButton13.BackColor = System.Drawing.Color.FromArgb(210, 180, 140);
		this.rjButton13.BackgroundColor = System.Drawing.Color.FromArgb(210, 180, 140);
		this.rjButton13.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton13.BorderRadius = 0;
		this.rjButton13.BorderSize = 0;
		this.rjButton13.FlatAppearance.BorderSize = 0;
		this.rjButton13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton13.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton13.ForeColor = System.Drawing.Color.White;
		this.rjButton13.Location = new System.Drawing.Point(98, 187);
		this.rjButton13.Name = "rjButton13";
		this.rjButton13.Size = new System.Drawing.Size(118, 31);
		this.rjButton13.TabIndex = 53;
		this.rjButton13.Text = "Cancel";
		this.rjButton13.TextColor = System.Drawing.Color.White;
		this.rjButton13.UseVisualStyleBackColor = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(346, 224);
		base.Controls.Add(this.rjButton14);
		base.Controls.Add(this.rjButton13);
		base.Controls.Add(this.rjTextBox7);
		base.Name = "FormSearch";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Search";
		base.Load += new System.EventHandler(FormSearch_Load);
		base.ResumeLayout(false);
	}
}
