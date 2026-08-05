using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Server.Data;
using Server.Helper;

namespace Server.Forms;

public class FormCustomSounds : FormMaterial
{
	private IContainer components;

	private RJTextBox rjTextBox7;

	private RJComboBox rjComboBoxCmdlineDir;

	private CheckBox checkBoxProcessCritical;

	private RJButton rjButton4;

	private RJButton rjButton1;

	private CheckBox checkBox1;

	private RJComboBox rjComboBox1;

	private RJTextBox rjTextBox1;

	private MaterialSlider materialSlider1;

	public FormCustomSounds()
	{
		InitializeComponent();
		base.Load += FormCustomSounds_Load;
		base.FormClosing += FormCustomSounds_FormClosing;
		rjComboBoxCmdlineDir.OnSelectedIndexChanged += UpdateConnectState;
		rjComboBox1.OnSelectedIndexChanged += UpdateStartState;
		checkBoxProcessCritical.CheckedChanged += UpdateConnectState;
		checkBox1.CheckedChanged += UpdateStartState;
		rjButton4.Click += rjButton4_Click;
		rjButton1.Click += rjButton1_Click;
	}

	private void FormCustomSounds_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		MaterialSkinManager.Instance.ColorSchemeChanged += ChangeScheme;
		ApplyMaterialColors();
		try
		{
			MaterialSlider slider = materialSlider1;
			PropertyInfo valueProperty = slider.GetType().GetProperty("Value");
			PropertyInfo minProperty = slider.GetType().GetProperty("RangeMin");
			PropertyInfo maxProperty = slider.GetType().GetProperty("RangeMax");
			if (minProperty != null)
			{
				minProperty.SetValue(slider, 0);
			}
			if (maxProperty != null)
			{
				maxProperty.SetValue(slider, 100);
			}
			if (valueProperty != null)
			{
				valueProperty.SetValue(slider, 100);
			}
		}
		catch
		{
		}
		if (File.Exists("local\\Settings.json"))
		{
			try
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				checkBoxProcessCritical.Checked = settings.EnableSoundOnConnect;
				checkBox1.Checked = settings.EnableSoundOnStart;
				rjComboBoxCmdlineDir.SelectedIndex = Math.Max(0, Math.Min(settings.SoundTypeConnect, 2));
				rjComboBox1.SelectedIndex = Math.Max(0, Math.Min(settings.SoundTypeStart, 2));
				rjTextBox7.Texts = settings.CustomSoundPathConnect ?? "";
				rjTextBox1.Texts = settings.CustomSoundPathStart ?? "";
				materialSlider1.Value = Math.Max(0, Math.Min(settings.SoundVolume, 100));
			}
			catch
			{
				rjComboBoxCmdlineDir.SelectedIndex = 0;
				rjComboBox1.SelectedIndex = 0;
			}
		}
		else
		{
			rjComboBoxCmdlineDir.SelectedIndex = 0;
			rjComboBox1.SelectedIndex = 0;
		}
		UpdateConnectState(null, null);
		UpdateStartState(null, null);
	}

	private void UpdateConnectState(object sender, EventArgs e)
	{
		bool sectionEnabled = checkBoxProcessCritical.Checked;
		bool pathEnabled = sectionEnabled && rjComboBoxCmdlineDir.SelectedIndex == 1;
		rjComboBoxCmdlineDir.Enabled = sectionEnabled;
		rjTextBox7.Enabled = pathEnabled;
		rjButton4.Enabled = pathEnabled;
	}

	private void UpdateStartState(object sender, EventArgs e)
	{
		bool sectionEnabled = checkBox1.Checked;
		bool pathEnabled = sectionEnabled && rjComboBox1.SelectedIndex == 1;
		rjComboBox1.Enabled = sectionEnabled;
		rjTextBox1.Enabled = pathEnabled;
		rjButton1.Enabled = pathEnabled;
	}

	private void ApplyMaterialColors()
	{
		Color primary = FormMaterial.PrimaryColor;
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (num ? Color.FromArgb(40, 40, 40) : Color.WhiteSmoke);
		Color text = (num ? Color.WhiteSmoke : Color.DimGray);
		BackColor = back;
		materialSlider1.BackColor = back;
		materialSlider1.ForeColor = text;
		rjButton4.BackColor = primary;
		rjButton4.BackgroundColor = primary;
		rjButton4.BorderColor = primary;
		rjButton4.TextColor = Color.White;
		rjButton1.BackColor = primary;
		rjButton1.BackgroundColor = primary;
		rjButton1.BorderColor = primary;
		rjButton1.TextColor = Color.White;
		rjComboBoxCmdlineDir.BorderColor = primary;
		rjComboBoxCmdlineDir.IconColor = primary;
		rjComboBoxCmdlineDir.BackColor = back;
		rjComboBoxCmdlineDir.ForeColor = text;
		rjComboBoxCmdlineDir.ListBackColor = back;
		rjComboBoxCmdlineDir.ListTextColor = text;
		rjComboBox1.BorderColor = primary;
		rjComboBox1.IconColor = primary;
		rjComboBox1.BackColor = back;
		rjComboBox1.ForeColor = text;
		rjComboBox1.ListBackColor = back;
		rjComboBox1.ListTextColor = text;
		rjTextBox7.BorderColor = primary;
		rjTextBox7.BorderFocusColor = primary;
		rjTextBox7.BackColor = back;
		rjTextBox7.ForeColor = text;
		rjTextBox1.BorderColor = primary;
		rjTextBox1.BorderFocusColor = primary;
		rjTextBox1.BackColor = back;
		rjTextBox1.ForeColor = text;
		checkBoxProcessCritical.ForeColor = text;
		checkBox1.ForeColor = text;
	}

	private void ChangeScheme(object sender)
	{
		ApplyMaterialColors();
	}

	private void rjButton4_Click(object sender, EventArgs e)
	{
		using OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = "WAV files (*.wav)|*.wav|All files (*.*)|*.*";
		ofd.Title = "Select Connect Sound";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			rjTextBox7.Texts = ofd.FileName;
		}
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		using OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = "WAV files (*.wav)|*.wav|All files (*.*)|*.*";
		ofd.Title = "Select Start Sound";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			rjTextBox1.Texts = ofd.FileName;
		}
	}

	private void FormCustomSounds_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveSettings();
	}

	private void SaveSettings()
	{
		try
		{
			Settings settings = (File.Exists("local\\Settings.json") ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json")) : new Settings());
			settings.EnableSoundOnConnect = checkBoxProcessCritical.Checked;
			settings.EnableSoundOnStart = checkBox1.Checked;
			settings.SoundTypeConnect = Math.Max(0, Math.Min(rjComboBoxCmdlineDir.SelectedIndex, 2));
			settings.SoundTypeStart = Math.Max(0, Math.Min(rjComboBox1.SelectedIndex, 2));
			settings.CustomSoundPathConnect = rjTextBox7.Texts ?? "";
			settings.CustomSoundPathStart = rjTextBox1.Texts ?? "";
			int vol = 100;
			try
			{
				vol = materialSlider1.Value;
			}
			catch
			{
				try
				{
					PropertyInfo vp = materialSlider1.GetType().GetProperty("Value");
					if (vp != null)
					{
						vol = (int)vp.GetValue(materialSlider1);
					}
				}
				catch
				{
				}
			}
			settings.SoundVolume = Math.Max(0, Math.Min(vol, 100));
			if (Program.form != null)
			{
				Program.form.settings = settings;
			}
			File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
		}
		catch (Exception)
		{
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
		this.rjTextBox7 = new CustomControls.RJControls.RJTextBox();
		this.rjComboBoxCmdlineDir = new CustomControls.RJControls.RJComboBox();
		this.checkBoxProcessCritical = new System.Windows.Forms.CheckBox();
		this.rjButton4 = new CustomControls.RJControls.RJButton();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.rjComboBox1 = new CustomControls.RJControls.RJComboBox();
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.materialSlider1 = new MaterialSkin.Controls.MaterialSlider();
		base.SuspendLayout();
		this.rjTextBox7.BackColor = System.Drawing.Color.White;
		this.rjTextBox7.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox7.BorderFocusColor = System.Drawing.Color.DarkViolet;
		this.rjTextBox7.BorderRadius = 0;
		this.rjTextBox7.BorderSize = 1;
		this.rjTextBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f);
		this.rjTextBox7.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox7.Location = new System.Drawing.Point(7, 129);
		this.rjTextBox7.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox7.Multiline = false;
		this.rjTextBox7.Name = "rjTextBox7";
		this.rjTextBox7.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox7.PasswordChar = false;
		this.rjTextBox7.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox7.PlaceholderText = "path to sound";
		this.rjTextBox7.Size = new System.Drawing.Size(240, 31);
		this.rjTextBox7.TabIndex = 32;
		this.rjTextBox7.Texts = "";
		this.rjTextBox7.UnderlinedStyle = false;
		this.rjComboBoxCmdlineDir.BackColor = System.Drawing.Color.WhiteSmoke;
		this.rjComboBoxCmdlineDir.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjComboBoxCmdlineDir.BorderSize = 1;
		this.rjComboBoxCmdlineDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.rjComboBoxCmdlineDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBoxCmdlineDir.ForeColor = System.Drawing.Color.Black;
		this.rjComboBoxCmdlineDir.IconColor = System.Drawing.Color.MediumSlateBlue;
		this.rjComboBoxCmdlineDir.Items.AddRange(new object[3] { "Standard", "Custom", "Dota" });
		this.rjComboBoxCmdlineDir.ListBackColor = System.Drawing.Color.White;
		this.rjComboBoxCmdlineDir.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBoxCmdlineDir.Location = new System.Drawing.Point(7, 92);
		this.rjComboBoxCmdlineDir.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBoxCmdlineDir.Name = "rjComboBoxCmdlineDir";
		this.rjComboBoxCmdlineDir.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBoxCmdlineDir.Size = new System.Drawing.Size(240, 30);
		this.rjComboBoxCmdlineDir.TabIndex = 33;
		this.rjComboBoxCmdlineDir.Texts = "Standard";
		this.checkBoxProcessCritical.AutoSize = true;
		this.checkBoxProcessCritical.ForeColor = System.Drawing.Color.Black;
		this.checkBoxProcessCritical.Location = new System.Drawing.Point(7, 69);
		this.checkBoxProcessCritical.Name = "checkBoxProcessCritical";
		this.checkBoxProcessCritical.Size = new System.Drawing.Size(104, 17);
		this.checkBoxProcessCritical.TabIndex = 35;
		this.checkBoxProcessCritical.Text = "Sound on Connect";
		this.checkBoxProcessCritical.UseVisualStyleBackColor = true;
		this.rjButton4.BackColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton4.BackgroundColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton4.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton4.BorderRadius = 0;
		this.rjButton4.BorderSize = 0;
		this.rjButton4.FlatAppearance.BorderSize = 0;
		this.rjButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton4.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButton4.ForeColor = System.Drawing.Color.White;
		this.rjButton4.Location = new System.Drawing.Point(254, 129);
		this.rjButton4.Name = "rjButton4";
		this.rjButton4.Size = new System.Drawing.Size(83, 31);
		this.rjButton4.TabIndex = 49;
		this.rjButton4.Text = "Browse";
		this.rjButton4.TextColor = System.Drawing.Color.White;
		this.rjButton4.UseVisualStyleBackColor = false;
		this.rjButton1.BackColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.BackgroundColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.Font = new System.Drawing.Font("Arial", 9f);
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(254, 227);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(83, 31);
		this.rjButton1.TabIndex = 50;
		this.rjButton1.Text = "Browse";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.checkBox1.AutoSize = true;
		this.checkBox1.ForeColor = System.Drawing.Color.Black;
		this.checkBox1.Location = new System.Drawing.Point(7, 167);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(86, 17);
		this.checkBox1.TabIndex = 51;
		this.checkBox1.Text = "Sound on Start";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.rjComboBox1.BackColor = System.Drawing.Color.WhiteSmoke;
		this.rjComboBox1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjComboBox1.BorderSize = 1;
		this.rjComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.rjComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox1.ForeColor = System.Drawing.Color.Black;
		this.rjComboBox1.IconColor = System.Drawing.Color.MediumSlateBlue;
		this.rjComboBox1.Items.AddRange(new object[3] { "Standard", "Custom", "Dota" });
		this.rjComboBox1.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox1.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBox1.Location = new System.Drawing.Point(7, 190);
		this.rjComboBox1.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox1.Name = "rjComboBox1";
		this.rjComboBox1.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox1.Size = new System.Drawing.Size(240, 30);
		this.rjComboBox1.TabIndex = 53;
		this.rjComboBox1.Texts = "Standard";
		this.rjTextBox1.BackColor = System.Drawing.Color.White;
		this.rjTextBox1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.DarkViolet;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f);
		this.rjTextBox1.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox1.Location = new System.Drawing.Point(7, 227);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "path to sound";
		this.rjTextBox1.Size = new System.Drawing.Size(240, 31);
		this.rjTextBox1.TabIndex = 52;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		this.materialSlider1.Depth = 0;
		this.materialSlider1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider1.Location = new System.Drawing.Point(7, 265);
		this.materialSlider1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider1.Name = "materialSlider1";
		this.materialSlider1.Size = new System.Drawing.Size(330, 40);
		this.materialSlider1.TabIndex = 54;
		this.materialSlider1.Text = "Volume";
		this.materialSlider1.Value = 100;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(345, 313);
		base.Controls.Add(this.materialSlider1);
		base.Controls.Add(this.rjComboBox1);
		base.Controls.Add(this.rjTextBox1);
		base.Controls.Add(this.checkBox1);
		base.Controls.Add(this.rjButton1);
		base.Controls.Add(this.rjButton4);
		base.Controls.Add(this.checkBoxProcessCritical);
		base.Controls.Add(this.rjComboBoxCmdlineDir);
		base.Controls.Add(this.rjTextBox7);
		base.Name = "FormCustomSounds";
		base.Sizable = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "CustomSounds";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
