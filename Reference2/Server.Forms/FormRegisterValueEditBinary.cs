using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Helper;
using Server.Helper.HexEditor;

namespace Server.Forms;

public class FormRegisterValueEditBinary : FormMaterial
{
	private readonly RegistrySeeker.RegValueData _value;

	private const string INVALID_BINARY_ERROR = "The binary value was invalid and could not be converted correctly.";

	private IContainer components;

	private RJButton rjButton4;

	private RJTextBox valueNameTxtBox;

	private RJButton rjButton1;

	private HexEditor hexEditor;

	public FormRegisterValueEditBinary(RegistrySeeker.RegValueData value)
	{
		_value = value;
		InitializeComponent();
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
		hexEditor.BackColor = backColor;
		hexEditor.ForeColor = FormMaterial.PrimaryColor;
		valueNameTxtBox.BackColor = backColor;
		valueNameTxtBox.ForeColor = foreColor;
		valueNameTxtBox.BorderColor = FormMaterial.PrimaryColor;
		rjButton1.BackColor = FormMaterial.PrimaryColor;
		rjButton4.BackColor = FormMaterial.PrimaryColor;
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		byte[] hexTable = hexEditor.HexTable;
		if (hexTable != null)
		{
			try
			{
				_value.Data = hexTable;
				base.DialogResult = DialogResult.OK;
				base.Tag = _value;
			}
			catch
			{
				MessageBox.Show("The binary value was invalid and could not be converted correctly.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				base.DialogResult = DialogResult.None;
			}
		}
		Close();
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
		this.hexEditor = new Server.Helper.HexEditor.HexEditor();
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
		this.rjButton4.Location = new System.Drawing.Point(280, 448);
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
		this.valueNameTxtBox.Location = new System.Drawing.Point(7, 410);
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
		this.rjButton1.Location = new System.Drawing.Point(200, 448);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(74, 31);
		this.rjButton1.TabIndex = 52;
		this.rjButton1.Text = "Save";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.hexEditor.BorderColor = System.Drawing.Color.Empty;
		this.hexEditor.Cursor = System.Windows.Forms.Cursors.IBeam;
		this.hexEditor.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.hexEditor.Location = new System.Drawing.Point(7, 67);
		this.hexEditor.Name = "hexEditor";
		this.hexEditor.SelectionBackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.hexEditor.Size = new System.Drawing.Size(347, 336);
		this.hexEditor.TabIndex = 53;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(367, 495);
		base.Controls.Add(this.hexEditor);
		base.Controls.Add(this.rjButton1);
		base.Controls.Add(this.rjButton4);
		base.Controls.Add(this.valueNameTxtBox);
		base.Name = "FormRegisterValueEditBinary";
		this.Text = "Register Value Edit Binary";
		base.Load += new System.EventHandler(FormRegisterValueEditString_Load);
		base.ResumeLayout(false);
	}
}
