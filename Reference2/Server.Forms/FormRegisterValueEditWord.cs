using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Microsoft.Win32;
using Server.Helper;

namespace Server.Forms;

public class FormRegisterValueEditWord : FormMaterial
{
	private readonly RegistrySeeker.RegValueData _value;

	private const string DWORD_WARNING = "The decimal value entered is greater than the maximum value of a DWORD (32-bit number). Should the value be truncated in order to continue?";

	private const string QWORD_WARNING = "The decimal value entered is greater than the maximum value of a QWORD (64-bit number). Should the value be truncated in order to continue?";

	private IContainer components;

	private RJButton rjButton4;

	private RJTextBox valueNameTxtBox;

	private RJButton rjButton1;

	private GroupBox baseBox;

	private RadioButton radioDecimal;

	private RadioButton radioHexa;

	private WordTextBox valueDataTxtBox;

	public FormRegisterValueEditWord(RegistrySeeker.RegValueData value)
	{
		_value = value;
		InitializeComponent();
		valueNameTxtBox.Text = value.Name;
		if (value.Kind == RegistryValueKind.DWord)
		{
			Text = "Register Value Edit DWORD (32-bit) Value";
			valueDataTxtBox.Type = WordTextBox.WordType.DWORD;
			valueDataTxtBox.Text = Server.Helper.ByteConverter.ToUInt32(value.Data).ToString("x");
		}
		else
		{
			Text = "Register Value Edit QWORD (64-bit) Value";
			valueDataTxtBox.Type = WordTextBox.WordType.QWORD;
			valueDataTxtBox.Text = Server.Helper.ByteConverter.ToUInt64(value.Data).ToString("x");
		}
	}

	private void FormRegisterValueEditString_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
	}

	private void ChangeScheme(object sender)
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color backColor = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color foreColor = (num ? Color.WhiteSmoke : Color.Black);
		valueDataTxtBox.BackColor = backColor;
		valueDataTxtBox.ForeColor = foreColor;
		valueNameTxtBox.BackColor = backColor;
		valueNameTxtBox.ForeColor = foreColor;
		valueNameTxtBox.BorderColor = FormMaterial.PrimaryColor;
		rjButton1.BackColor = FormMaterial.PrimaryColor;
		rjButton4.BackColor = FormMaterial.PrimaryColor;
		baseBox.ForeColor = FormMaterial.PrimaryColor;
		radioHexa.ForeColor = FormMaterial.PrimaryColor;
		radioDecimal.ForeColor = FormMaterial.PrimaryColor;
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		if (valueDataTxtBox.IsConversionValid() || IsOverridePossible())
		{
			_value.Data = ((_value.Kind == RegistryValueKind.DWord) ? Server.Helper.ByteConverter.GetBytes(valueDataTxtBox.UIntValue) : Server.Helper.ByteConverter.GetBytes(valueDataTxtBox.ULongValue));
			base.Tag = _value;
			base.DialogResult = DialogResult.OK;
		}
		else
		{
			base.DialogResult = DialogResult.None;
		}
		Close();
	}

	private DialogResult ShowWarning(string msg, string caption)
	{
		return MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
	}

	private bool IsOverridePossible()
	{
		return ShowWarning((_value.Kind == RegistryValueKind.DWord) ? "The decimal value entered is greater than the maximum value of a DWORD (32-bit number). Should the value be truncated in order to continue?" : "The decimal value entered is greater than the maximum value of a QWORD (64-bit number). Should the value be truncated in order to continue?", "Overflow") == DialogResult.Yes;
	}

	private void radioHexa_CheckedChanged(object sender, EventArgs e)
	{
		if (valueDataTxtBox.IsHexNumber != radioHexa.Checked)
		{
			if (valueDataTxtBox.IsConversionValid() || IsOverridePossible())
			{
				valueDataTxtBox.IsHexNumber = radioHexa.Checked;
			}
			else
			{
				radioDecimal.Checked = true;
			}
		}
	}

	private void rjButton4_Click(object sender, EventArgs e)
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
		this.rjButton4 = new CustomControls.RJControls.RJButton();
		this.valueNameTxtBox = new CustomControls.RJControls.RJTextBox();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.baseBox = new System.Windows.Forms.GroupBox();
		this.radioDecimal = new System.Windows.Forms.RadioButton();
		this.radioHexa = new System.Windows.Forms.RadioButton();
		this.valueDataTxtBox = new Server.Helper.WordTextBox();
		this.baseBox.SuspendLayout();
		base.SuspendLayout();
		this.rjButton4.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton4.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton4.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton4.BorderRadius = 0;
		this.rjButton4.BorderSize = 0;
		this.rjButton4.FlatAppearance.BorderSize = 0;
		this.rjButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton4.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton4.ForeColor = System.Drawing.Color.White;
		this.rjButton4.Location = new System.Drawing.Point(280, 155);
		this.rjButton4.Name = "rjButton4";
		this.rjButton4.Size = new System.Drawing.Size(74, 31);
		this.rjButton4.TabIndex = 50;
		this.rjButton4.Text = "Cancel";
		this.rjButton4.TextColor = System.Drawing.Color.White;
		this.rjButton4.UseVisualStyleBackColor = false;
		this.rjButton4.Click += new System.EventHandler(rjButton4_Click);
		this.valueNameTxtBox.BackColor = System.Drawing.SystemColors.Window;
		this.valueNameTxtBox.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.valueNameTxtBox.BorderFocusColor = System.Drawing.Color.HotPink;
		this.valueNameTxtBox.BorderRadius = 0;
		this.valueNameTxtBox.BorderSize = 1;
		this.valueNameTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.valueNameTxtBox.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.valueNameTxtBox.Location = new System.Drawing.Point(7, 117);
		this.valueNameTxtBox.Margin = new System.Windows.Forms.Padding(4);
		this.valueNameTxtBox.Multiline = false;
		this.valueNameTxtBox.Name = "valueNameTxtBox";
		this.valueNameTxtBox.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.valueNameTxtBox.PasswordChar = false;
		this.valueNameTxtBox.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.valueNameTxtBox.PlaceholderText = "Name";
		this.valueNameTxtBox.Size = new System.Drawing.Size(347, 31);
		this.valueNameTxtBox.TabIndex = 49;
		this.valueNameTxtBox.Texts = "";
		this.valueNameTxtBox.UnderlinedStyle = false;
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(200, 155);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(74, 31);
		this.rjButton1.TabIndex = 52;
		this.rjButton1.Text = "Save";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.baseBox.Controls.Add(this.radioDecimal);
		this.baseBox.Controls.Add(this.radioHexa);
		this.baseBox.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.baseBox.Location = new System.Drawing.Point(7, 155);
		this.baseBox.Name = "baseBox";
		this.baseBox.Size = new System.Drawing.Size(156, 68);
		this.baseBox.TabIndex = 53;
		this.baseBox.TabStop = false;
		this.baseBox.Text = "Base";
		this.radioDecimal.AutoSize = true;
		this.radioDecimal.Location = new System.Drawing.Point(14, 43);
		this.radioDecimal.Name = "radioDecimal";
		this.radioDecimal.Size = new System.Drawing.Size(63, 17);
		this.radioDecimal.TabIndex = 4;
		this.radioDecimal.Text = "Decimal";
		this.radioDecimal.UseVisualStyleBackColor = true;
		this.radioHexa.AutoSize = true;
		this.radioHexa.Checked = true;
		this.radioHexa.Location = new System.Drawing.Point(14, 18);
		this.radioHexa.Name = "radioHexa";
		this.radioHexa.Size = new System.Drawing.Size(86, 17);
		this.radioHexa.TabIndex = 3;
		this.radioHexa.TabStop = true;
		this.radioHexa.Text = "Hexadecimal";
		this.radioHexa.UseVisualStyleBackColor = true;
		this.radioHexa.CheckedChanged += new System.EventHandler(radioHexa_CheckedChanged);
		this.valueDataTxtBox.BackColor = System.Drawing.Color.White;
		this.valueDataTxtBox.ForeColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.valueDataTxtBox.IsHexNumber = false;
		this.valueDataTxtBox.Location = new System.Drawing.Point(7, 81);
		this.valueDataTxtBox.MaxLength = 8;
		this.valueDataTxtBox.Multiline = true;
		this.valueDataTxtBox.Name = "valueDataTxtBox";
		this.valueDataTxtBox.Size = new System.Drawing.Size(347, 29);
		this.valueDataTxtBox.TabIndex = 54;
		this.valueDataTxtBox.Type = Server.Helper.WordTextBox.WordType.DWORD;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(378, 240);
		base.Controls.Add(this.valueDataTxtBox);
		base.Controls.Add(this.baseBox);
		base.Controls.Add(this.rjButton1);
		base.Controls.Add(this.rjButton4);
		base.Controls.Add(this.valueNameTxtBox);
		base.Name = "FormRegisterValueEditWord";
		this.Text = "Register Value Edit Word";
		base.Load += new System.EventHandler(FormRegisterValueEditString_Load);
		this.baseBox.ResumeLayout(false);
		this.baseBox.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
