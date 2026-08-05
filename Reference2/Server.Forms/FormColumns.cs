using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Server.Data;
using Server.Helper;

namespace Server.Forms;

public class FormColumns : FormMaterial
{
	private Form1 mainForm;

	private const string SettingsFile = "local\\ColumnsSettings.json";

	private IContainer components;

	public MaterialSwitch materialSwitch5;

	public MaterialSwitch materialSwitch1;

	public MaterialSwitch materialSwitch2;

	public MaterialSwitch materialSwitch3;

	public MaterialSwitch materialSwitch4;

	public MaterialSwitch materialSwitch6;

	public MaterialSwitch materialSwitch7;

	public MaterialSwitch materialSwitch8;

	public MaterialSwitch materialSwitch9;

	public MaterialSwitch materialSwitch10;

	public MaterialSwitch materialSwitch11;

	public MaterialSwitch materialSwitch12;

	public MaterialSwitch materialSwitch13;

	public MaterialSwitch materialSwitch14;

	public MaterialSwitch materialSwitch15;

	public MaterialSwitch materialSwitch16;

	public MaterialSwitch materialSwitch17;

	public FormColumns(Form1 form)
	{
		InitializeComponent();
		mainForm = form;
		LoadSettings();
		AttachEventHandlers();
		ApplyTheme();
		MaterialSkinManager.Instance.ThemeChanged += delegate
		{
			ApplyTheme();
		};
		MaterialSkinManager.Instance.ColorSchemeChanged += delegate
		{
			ApplyTheme();
		};
	}

	private void ApplyTheme()
	{
		Color primary = FormMaterial.PrimaryColor;
		if (MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK)
		{
			BackColor = Color.FromArgb(40, 40, 40);
		}
		else
		{
			BackColor = Color.White;
		}
		foreach (Control control in base.Controls)
		{
			if (control is MaterialSwitch)
			{
				control.ForeColor = primary;
			}
		}
	}

	private void LoadSettings()
	{
		ColumnsSettings settings = ((!File.Exists("local\\ColumnsSettings.json")) ? new ColumnsSettings() : JsonConvert.DeserializeObject<ColumnsSettings>(File.ReadAllText("local\\ColumnsSettings.json")));
		materialSwitch5.Checked = settings.ShowIPAddress;
		materialSwitch1.Checked = settings.ShowUserPC;
		materialSwitch2.Checked = settings.ShowFlag;
		materialSwitch3.Checked = settings.ShowCamera;
		materialSwitch8.Checked = settings.ShowCountry;
		materialSwitch7.Checked = settings.ShowCpu;
		materialSwitch6.Checked = settings.ShowGroup;
		materialSwitch4.Checked = settings.ShowGpu;
		materialSwitch12.Checked = settings.ShowNote;
		materialSwitch11.Checked = settings.ShowWindows;
		materialSwitch10.Checked = settings.ShowHwid;
		materialSwitch9.Checked = settings.ShowAntiVirus;
		materialSwitch16.Checked = settings.ShowVersion;
		materialSwitch15.Checked = settings.ShowPrivilege;
		materialSwitch14.Checked = settings.ShowTimeInstall;
		materialSwitch13.Checked = settings.ShowPing;
		materialSwitch17.Checked = settings.ShowWindow;
	}

	private void AttachEventHandlers()
	{
		materialSwitch5.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnIP, materialSwitch5.Checked);
		};
		materialSwitch1.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnUserPC, materialSwitch1.Checked);
		};
		materialSwitch2.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnFlag, materialSwitch2.Checked);
		};
		materialSwitch3.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnCamera, materialSwitch3.Checked);
		};
		materialSwitch8.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnCountry, materialSwitch8.Checked);
		};
		materialSwitch7.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnCpu, materialSwitch7.Checked);
		};
		materialSwitch6.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnGroup, materialSwitch6.Checked);
		};
		materialSwitch4.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnGpu, materialSwitch4.Checked);
		};
		materialSwitch12.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.Column6, materialSwitch12.Checked);
		};
		materialSwitch11.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnOs, materialSwitch11.Checked);
		};
		materialSwitch10.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnHwid, materialSwitch10.Checked);
		};
		materialSwitch9.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnAntiVirus, materialSwitch9.Checked);
		};
		materialSwitch16.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnVersion, materialSwitch16.Checked);
		};
		materialSwitch15.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnPrivilege, materialSwitch15.Checked);
		};
		materialSwitch14.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnTimeInstall, materialSwitch14.Checked);
		};
		materialSwitch13.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnPing, materialSwitch13.Checked);
		};
		materialSwitch17.CheckedChanged += delegate
		{
			UpdateColumn(mainForm.ColumnWindow, materialSwitch17.Checked);
		};
	}

	private void UpdateColumn(DataGridViewColumn column, bool visible)
	{
		if (column != null)
		{
			column.Visible = visible;
			SaveSettings();
		}
	}

	private void SaveSettings()
	{
		ColumnsSettings settings = new ColumnsSettings
		{
			ShowIPAddress = materialSwitch5.Checked,
			ShowUserPC = materialSwitch1.Checked,
			ShowFlag = materialSwitch2.Checked,
			ShowCamera = materialSwitch3.Checked,
			ShowCountry = materialSwitch8.Checked,
			ShowCpu = materialSwitch7.Checked,
			ShowGroup = materialSwitch6.Checked,
			ShowGpu = materialSwitch4.Checked,
			ShowNote = materialSwitch12.Checked,
			ShowWindows = materialSwitch11.Checked,
			ShowHwid = materialSwitch10.Checked,
			ShowAntiVirus = materialSwitch9.Checked,
			ShowVersion = materialSwitch16.Checked,
			ShowPrivilege = materialSwitch15.Checked,
			ShowTimeInstall = materialSwitch14.Checked,
			ShowPing = materialSwitch13.Checked,
			ShowWindow = materialSwitch17.Checked
		};
		File.WriteAllText("local\\ColumnsSettings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
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
		this.materialSwitch5 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch1 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch2 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch3 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch4 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch6 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch7 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch8 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch9 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch10 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch11 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch12 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch13 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch14 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch15 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch16 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch17 = new MaterialSkin.Controls.MaterialSwitch();
		base.SuspendLayout();
		this.materialSwitch5.AutoSize = true;
		this.materialSwitch5.Depth = 0;
		this.materialSwitch5.Location = new System.Drawing.Point(12, 77);
		this.materialSwitch5.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch5.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch5.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch5.Name = "materialSwitch5";
		this.materialSwitch5.Ripple = true;
		this.materialSwitch5.Size = new System.Drawing.Size(133, 37);
		this.materialSwitch5.TabIndex = 64;
		this.materialSwitch5.Text = "IP-Address";
		this.materialSwitch5.UseVisualStyleBackColor = true;
		this.materialSwitch1.AutoSize = true;
		this.materialSwitch1.Depth = 0;
		this.materialSwitch1.Location = new System.Drawing.Point(163, 77);
		this.materialSwitch1.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch1.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch1.Name = "materialSwitch1";
		this.materialSwitch1.Ripple = true;
		this.materialSwitch1.Size = new System.Drawing.Size(171, 37);
		this.materialSwitch1.TabIndex = 65;
		this.materialSwitch1.Text = "Username @ PC";
		this.materialSwitch1.UseVisualStyleBackColor = true;
		this.materialSwitch2.AutoSize = true;
		this.materialSwitch2.Depth = 0;
		this.materialSwitch2.Location = new System.Drawing.Point(12, 114);
		this.materialSwitch2.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch2.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch2.Name = "materialSwitch2";
		this.materialSwitch2.Ripple = true;
		this.materialSwitch2.Size = new System.Drawing.Size(89, 37);
		this.materialSwitch2.TabIndex = 66;
		this.materialSwitch2.Text = "Flag";
		this.materialSwitch2.UseVisualStyleBackColor = true;
		this.materialSwitch3.AutoSize = true;
		this.materialSwitch3.Depth = 0;
		this.materialSwitch3.Location = new System.Drawing.Point(163, 114);
		this.materialSwitch3.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch3.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch3.Name = "materialSwitch3";
		this.materialSwitch3.Ripple = true;
		this.materialSwitch3.Size = new System.Drawing.Size(113, 37);
		this.materialSwitch3.TabIndex = 67;
		this.materialSwitch3.Text = "Camera";
		this.materialSwitch3.UseVisualStyleBackColor = true;
		this.materialSwitch4.AutoSize = true;
		this.materialSwitch4.Depth = 0;
		this.materialSwitch4.Location = new System.Drawing.Point(163, 190);
		this.materialSwitch4.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch4.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch4.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch4.Name = "materialSwitch4";
		this.materialSwitch4.Ripple = true;
		this.materialSwitch4.Size = new System.Drawing.Size(87, 37);
		this.materialSwitch4.TabIndex = 71;
		this.materialSwitch4.Text = "Gpu";
		this.materialSwitch4.UseVisualStyleBackColor = true;
		this.materialSwitch6.AutoSize = true;
		this.materialSwitch6.Depth = 0;
		this.materialSwitch6.Location = new System.Drawing.Point(12, 190);
		this.materialSwitch6.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch6.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch6.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch6.Name = "materialSwitch6";
		this.materialSwitch6.Ripple = true;
		this.materialSwitch6.Size = new System.Drawing.Size(101, 37);
		this.materialSwitch6.TabIndex = 70;
		this.materialSwitch6.Text = "Group";
		this.materialSwitch6.UseVisualStyleBackColor = true;
		this.materialSwitch7.AutoSize = true;
		this.materialSwitch7.Depth = 0;
		this.materialSwitch7.Location = new System.Drawing.Point(163, 153);
		this.materialSwitch7.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch7.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch7.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch7.Name = "materialSwitch7";
		this.materialSwitch7.Ripple = true;
		this.materialSwitch7.Size = new System.Drawing.Size(86, 37);
		this.materialSwitch7.TabIndex = 69;
		this.materialSwitch7.Text = "Cpu";
		this.materialSwitch7.UseVisualStyleBackColor = true;
		this.materialSwitch8.AutoSize = true;
		this.materialSwitch8.Depth = 0;
		this.materialSwitch8.Location = new System.Drawing.Point(12, 153);
		this.materialSwitch8.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch8.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch8.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch8.Name = "materialSwitch8";
		this.materialSwitch8.Ripple = true;
		this.materialSwitch8.Size = new System.Drawing.Size(113, 37);
		this.materialSwitch8.TabIndex = 68;
		this.materialSwitch8.Text = "Country";
		this.materialSwitch8.UseVisualStyleBackColor = true;
		this.materialSwitch9.AutoSize = true;
		this.materialSwitch9.Depth = 0;
		this.materialSwitch9.Location = new System.Drawing.Point(163, 271);
		this.materialSwitch9.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch9.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch9.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch9.Name = "materialSwitch9";
		this.materialSwitch9.Ripple = true;
		this.materialSwitch9.Size = new System.Drawing.Size(122, 37);
		this.materialSwitch9.TabIndex = 75;
		this.materialSwitch9.Text = "AntiVirus";
		this.materialSwitch9.UseVisualStyleBackColor = true;
		this.materialSwitch10.AutoSize = true;
		this.materialSwitch10.Depth = 0;
		this.materialSwitch10.Location = new System.Drawing.Point(12, 271);
		this.materialSwitch10.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch10.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch10.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch10.Name = "materialSwitch10";
		this.materialSwitch10.Ripple = true;
		this.materialSwitch10.Size = new System.Drawing.Size(94, 37);
		this.materialSwitch10.TabIndex = 74;
		this.materialSwitch10.Text = "Hwid";
		this.materialSwitch10.UseVisualStyleBackColor = true;
		this.materialSwitch11.AutoSize = true;
		this.materialSwitch11.Depth = 0;
		this.materialSwitch11.Location = new System.Drawing.Point(163, 234);
		this.materialSwitch11.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch11.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch11.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch11.Name = "materialSwitch11";
		this.materialSwitch11.Ripple = true;
		this.materialSwitch11.Size = new System.Drawing.Size(123, 37);
		this.materialSwitch11.TabIndex = 73;
		this.materialSwitch11.Text = "Windows";
		this.materialSwitch11.UseVisualStyleBackColor = true;
		this.materialSwitch12.AutoSize = true;
		this.materialSwitch12.Depth = 0;
		this.materialSwitch12.Location = new System.Drawing.Point(12, 234);
		this.materialSwitch12.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch12.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch12.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch12.Name = "materialSwitch12";
		this.materialSwitch12.Ripple = true;
		this.materialSwitch12.Size = new System.Drawing.Size(91, 37);
		this.materialSwitch12.TabIndex = 72;
		this.materialSwitch12.Text = "Note";
		this.materialSwitch12.UseVisualStyleBackColor = true;
		this.materialSwitch13.AutoSize = true;
		this.materialSwitch13.Depth = 0;
		this.materialSwitch13.Location = new System.Drawing.Point(164, 347);
		this.materialSwitch13.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch13.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch13.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch13.Name = "materialSwitch13";
		this.materialSwitch13.Ripple = true;
		this.materialSwitch13.Size = new System.Drawing.Size(90, 37);
		this.materialSwitch13.TabIndex = 79;
		this.materialSwitch13.Text = "Ping";
		this.materialSwitch13.UseVisualStyleBackColor = true;
		this.materialSwitch14.AutoSize = true;
		this.materialSwitch14.Depth = 0;
		this.materialSwitch14.Location = new System.Drawing.Point(13, 347);
		this.materialSwitch14.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch14.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch14.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch14.Name = "materialSwitch14";
		this.materialSwitch14.Ripple = true;
		this.materialSwitch14.Size = new System.Drawing.Size(141, 37);
		this.materialSwitch14.TabIndex = 78;
		this.materialSwitch14.Text = "Time Install";
		this.materialSwitch14.UseVisualStyleBackColor = true;
		this.materialSwitch15.AutoSize = true;
		this.materialSwitch15.Depth = 0;
		this.materialSwitch15.Location = new System.Drawing.Point(164, 310);
		this.materialSwitch15.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch15.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch15.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch15.Name = "materialSwitch15";
		this.materialSwitch15.Ripple = true;
		this.materialSwitch15.Size = new System.Drawing.Size(118, 37);
		this.materialSwitch15.TabIndex = 77;
		this.materialSwitch15.Text = "Privilege";
		this.materialSwitch15.UseVisualStyleBackColor = true;
		this.materialSwitch16.AutoSize = true;
		this.materialSwitch16.Depth = 0;
		this.materialSwitch16.Location = new System.Drawing.Point(13, 310);
		this.materialSwitch16.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch16.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch16.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch16.Name = "materialSwitch16";
		this.materialSwitch16.Ripple = true;
		this.materialSwitch16.Size = new System.Drawing.Size(111, 37);
		this.materialSwitch16.TabIndex = 76;
		this.materialSwitch16.Text = "Version";
		this.materialSwitch16.UseVisualStyleBackColor = true;
		this.materialSwitch17.AutoSize = true;
		this.materialSwitch17.Depth = 0;
		this.materialSwitch17.Location = new System.Drawing.Point(105, 384);
		this.materialSwitch17.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch17.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch17.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch17.Name = "materialSwitch17";
		this.materialSwitch17.Ripple = true;
		this.materialSwitch17.Size = new System.Drawing.Size(115, 37);
		this.materialSwitch17.TabIndex = 80;
		this.materialSwitch17.Text = "Window";
		this.materialSwitch17.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(354, 439);
		base.Controls.Add(this.materialSwitch17);
		base.Controls.Add(this.materialSwitch13);
		base.Controls.Add(this.materialSwitch14);
		base.Controls.Add(this.materialSwitch15);
		base.Controls.Add(this.materialSwitch16);
		base.Controls.Add(this.materialSwitch9);
		base.Controls.Add(this.materialSwitch10);
		base.Controls.Add(this.materialSwitch11);
		base.Controls.Add(this.materialSwitch12);
		base.Controls.Add(this.materialSwitch4);
		base.Controls.Add(this.materialSwitch6);
		base.Controls.Add(this.materialSwitch7);
		base.Controls.Add(this.materialSwitch8);
		base.Controls.Add(this.materialSwitch3);
		base.Controls.Add(this.materialSwitch2);
		base.Controls.Add(this.materialSwitch1);
		base.Controls.Add(this.materialSwitch5);
		base.Name = "FormColumns";
		this.Text = "Columns";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
