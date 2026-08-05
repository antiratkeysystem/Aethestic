using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Helper;

namespace Server.Forms;

public class FormPumpSettings : FormMaterial
{
	private IContainer components;

	private Label labelSize;

	private RJTextBox rjTextBoxSize;

	private RJComboBox rjComboBoxUnit;

	private RJButton rjButtonOk;

	private RJButton rjButtonCancel;

	public long? PumpSizeBytes { get; private set; }

	public FormPumpSettings()
	{
		InitializeComponent();
	}

	private void FormPumpSettings_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		if (rjComboBoxUnit.Texts != "MB" && string.IsNullOrEmpty(rjComboBoxUnit.Texts))
		{
			rjComboBoxUnit.Texts = "MB";
		}
	}

	private void ChangeScheme(object sender)
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color fore = (num ? Color.WhiteSmoke : Color.Black);
		labelSize.ForeColor = fore;
		labelSize.BackColor = Color.Transparent;
		rjTextBoxSize.BorderColor = FormMaterial.PrimaryColor;
		rjTextBoxSize.BackColor = back;
		rjTextBoxSize.ForeColor = fore;
		rjComboBoxUnit.BorderColor = FormMaterial.PrimaryColor;
		rjComboBoxUnit.BackColor = back;
		rjComboBoxUnit.ForeColor = fore;
		rjComboBoxUnit.ListBackColor = back;
		rjComboBoxUnit.ListTextColor = fore;
		rjComboBoxUnit.IconColor = FormMaterial.PrimaryColor;
		rjButtonOk.BackColor = FormMaterial.PrimaryColor;
		rjButtonCancel.BackColor = FormMaterial.PrimaryColor;
	}

	private void rjButtonOk_Click(object sender, EventArgs e)
	{
		if (!long.TryParse(rjTextBoxSize.Texts.Trim(), out var value) || value <= 0)
		{
			MessageBox.Show("Enter a valid positive number.", "Pump size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string unit = (rjComboBoxUnit.Texts ?? "MB").ToUpperInvariant();
		long multiplier = 1048576L;
		switch (unit)
		{
		case "KB":
			multiplier = 1024L;
			break;
		case "MB":
			multiplier = 1048576L;
			break;
		case "GB":
			multiplier = 1073741824L;
			break;
		}
		try
		{
			PumpSizeBytes = value * multiplier;
		}
		catch (OverflowException)
		{
			MessageBox.Show("Size is too large.", "Pump size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void rjButtonCancel_Click(object sender, EventArgs e)
	{
		PumpSizeBytes = null;
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
		this.labelSize = new System.Windows.Forms.Label();
		this.rjTextBoxSize = new CustomControls.RJControls.RJTextBox();
		this.rjComboBoxUnit = new CustomControls.RJControls.RJComboBox();
		this.rjButtonOk = new CustomControls.RJControls.RJButton();
		this.rjButtonCancel = new CustomControls.RJControls.RJButton();
		base.SuspendLayout();
		this.labelSize.AutoSize = true;
		this.labelSize.BackColor = System.Drawing.Color.Transparent;
		this.labelSize.Location = new System.Drawing.Point(34, 83);
		this.labelSize.Name = "labelSize";
		this.labelSize.Size = new System.Drawing.Size(58, 13);
		this.labelSize.TabIndex = 0;
		this.labelSize.Text = "Pump size:";
		this.rjTextBoxSize.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBoxSize.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxSize.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxSize.BorderRadius = 0;
		this.rjTextBoxSize.BorderSize = 1;
		this.rjTextBoxSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBoxSize.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBoxSize.Location = new System.Drawing.Point(37, 100);
		this.rjTextBoxSize.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxSize.Multiline = false;
		this.rjTextBoxSize.Name = "rjTextBoxSize";
		this.rjTextBoxSize.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxSize.PasswordChar = false;
		this.rjTextBoxSize.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxSize.PlaceholderText = "e.g. 500";
		this.rjTextBoxSize.Size = new System.Drawing.Size(180, 31);
		this.rjTextBoxSize.TabIndex = 1;
		this.rjTextBoxSize.Texts = "500";
		this.rjTextBoxSize.UnderlinedStyle = false;
		this.rjComboBoxUnit.BackColor = System.Drawing.Color.White;
		this.rjComboBoxUnit.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBoxUnit.BorderSize = 1;
		this.rjComboBoxUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.rjComboBoxUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBoxUnit.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBoxUnit.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBoxUnit.Items.AddRange(new object[3] { "KB", "MB", "GB" });
		this.rjComboBoxUnit.ListBackColor = System.Drawing.Color.White;
		this.rjComboBoxUnit.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBoxUnit.Location = new System.Drawing.Point(227, 100);
		this.rjComboBoxUnit.MinimumSize = new System.Drawing.Size(80, 30);
		this.rjComboBoxUnit.Name = "rjComboBoxUnit";
		this.rjComboBoxUnit.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBoxUnit.Size = new System.Drawing.Size(100, 31);
		this.rjComboBoxUnit.TabIndex = 2;
		this.rjComboBoxUnit.Texts = "MB";
		this.rjButtonOk.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonOk.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonOk.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonOk.BorderRadius = 0;
		this.rjButtonOk.BorderSize = 0;
		this.rjButtonOk.FlatAppearance.BorderSize = 0;
		this.rjButtonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonOk.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButtonOk.ForeColor = System.Drawing.Color.White;
		this.rjButtonOk.Location = new System.Drawing.Point(227, 136);
		this.rjButtonOk.Name = "rjButtonOk";
		this.rjButtonOk.Size = new System.Drawing.Size(100, 32);
		this.rjButtonOk.TabIndex = 4;
		this.rjButtonOk.Text = "Build";
		this.rjButtonOk.TextColor = System.Drawing.Color.White;
		this.rjButtonOk.UseVisualStyleBackColor = false;
		this.rjButtonOk.Click += new System.EventHandler(rjButtonOk_Click);
		this.rjButtonCancel.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonCancel.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonCancel.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonCancel.BorderRadius = 0;
		this.rjButtonCancel.BorderSize = 0;
		this.rjButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.rjButtonCancel.FlatAppearance.BorderSize = 0;
		this.rjButtonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonCancel.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButtonCancel.ForeColor = System.Drawing.Color.White;
		this.rjButtonCancel.Location = new System.Drawing.Point(37, 137);
		this.rjButtonCancel.Name = "rjButtonCancel";
		this.rjButtonCancel.Size = new System.Drawing.Size(180, 31);
		this.rjButtonCancel.TabIndex = 3;
		this.rjButtonCancel.Text = "Cancel";
		this.rjButtonCancel.TextColor = System.Drawing.Color.White;
		this.rjButtonCancel.UseVisualStyleBackColor = false;
		this.rjButtonCancel.Click += new System.EventHandler(rjButtonCancel_Click);
		base.AcceptButton = this.rjButtonOk;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.rjButtonCancel;
		base.ClientSize = new System.Drawing.Size(360, 191);
		base.Controls.Add(this.rjButtonCancel);
		base.Controls.Add(this.rjButtonOk);
		base.Controls.Add(this.rjComboBoxUnit);
		base.Controls.Add(this.rjTextBoxSize);
		base.Controls.Add(this.labelSize);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FormPumpSettings";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Build Pump";
		base.Load += new System.EventHandler(FormPumpSettings_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
