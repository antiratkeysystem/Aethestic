using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;
using WinFormAnimation;

namespace Server.Forms;

public class FormPerformance : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private System.Windows.Forms.Timer timer1;

	private Panel panel1;

	private PictureBox pictureBox1;

	public CircularProgressBar circularProgressBar1;

	public CircularProgressBar circularProgressBar2;

	public Label label1;

	public Label label2;

	public Label label3;

	public Label label4;

	public Label label5;

	public Label label7;

	public Label label6;

	public FormPerformance()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormPerformance_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
	}

	private void ChangeScheme(object sender)
	{
		circularProgressBar1.ProgressColor = FormMaterial.PrimaryColor;
		circularProgressBar2.ProgressColor = FormMaterial.PrimaryColor;
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormPerformance));
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.panel1 = new System.Windows.Forms.Panel();
		this.label3 = new System.Windows.Forms.Label();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.label7 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.circularProgressBar2 = new Server.Helper.CircularProgressBar();
		this.circularProgressBar1 = new Server.Helper.CircularProgressBar();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.label3);
		this.panel1.Controls.Add(this.pictureBox1);
		this.panel1.Controls.Add(this.label7);
		this.panel1.Controls.Add(this.label6);
		this.panel1.Controls.Add(this.label5);
		this.panel1.Controls.Add(this.label4);
		this.panel1.Controls.Add(this.label2);
		this.panel1.Controls.Add(this.label1);
		this.panel1.Controls.Add(this.circularProgressBar2);
		this.panel1.Controls.Add(this.circularProgressBar1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(497, 301);
		this.panel1.TabIndex = 0;
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label3.Location = new System.Drawing.Point(60, 257);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(84, 15);
		this.label3.TabIndex = 5;
		this.label3.Text = "System Time: ";
		this.pictureBox1.Image = (System.Drawing.Image)componentResourceManager.GetObject("pictureBox1.Image");
		this.pictureBox1.Location = new System.Drawing.Point(35, 252);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(25, 25);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 10;
		this.pictureBox1.TabStop = false;
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label7.Location = new System.Drawing.Point(306, 182);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(53, 15);
		this.label7.TabIndex = 9;
		this.label7.Text = "Logical: ";
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label6.Location = new System.Drawing.Point(306, 155);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(45, 15);
		this.label6.TabIndex = 8;
		this.label6.Text = "Cores: ";
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label5.Location = new System.Drawing.Point(27, 169);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(49, 15);
		this.label5.TabIndex = 7;
		this.label5.Text = "Speed: ";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label4.Location = new System.Drawing.Point(306, 125);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(49, 15);
		this.label4.TabIndex = 6;
		this.label4.Text = "Speed: ";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label2.Location = new System.Drawing.Point(27, 16);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(58, 15);
		this.label2.TabIndex = 4;
		this.label2.Text = "Memory: ";
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label1.Location = new System.Drawing.Point(150, 79);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(68, 15);
		this.label1.TabIndex = 3;
		this.label1.Text = "Processor: ";
		this.circularProgressBar2.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
		this.circularProgressBar2.AnimationSpeed = 500;
		this.circularProgressBar2.BackColor = System.Drawing.Color.Transparent;
		this.circularProgressBar2.Font = new System.Drawing.Font("Arial", 13f);
		this.circularProgressBar2.ForeColor = System.Drawing.Color.Black;
		this.circularProgressBar2.InnerColor = System.Drawing.Color.White;
		this.circularProgressBar2.InnerMargin = 2;
		this.circularProgressBar2.InnerWidth = -1;
		this.circularProgressBar2.Location = new System.Drawing.Point(31, 45);
		this.circularProgressBar2.MarqueeAnimationSpeed = 2000;
		this.circularProgressBar2.Name = "circularProgressBar2";
		this.circularProgressBar2.OuterColor = System.Drawing.Color.Gray;
		this.circularProgressBar2.OuterMargin = -12;
		this.circularProgressBar2.OuterWidth = 12;
		this.circularProgressBar2.ProgressColor = System.Drawing.Color.FromArgb(255, 128, 0);
		this.circularProgressBar2.ProgressWidth = 12;
		this.circularProgressBar2.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36f);
		this.circularProgressBar2.Size = new System.Drawing.Size(100, 100);
		this.circularProgressBar2.StartAngle = 270;
		this.circularProgressBar2.SubscriptColor = System.Drawing.Color.FromArgb(166, 166, 166);
		this.circularProgressBar2.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
		this.circularProgressBar2.SubscriptText = "";
		this.circularProgressBar2.SuperscriptColor = System.Drawing.Color.FromArgb(166, 166, 166);
		this.circularProgressBar2.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
		this.circularProgressBar2.SuperscriptText = "";
		this.circularProgressBar2.TabIndex = 2;
		this.circularProgressBar2.Text = "0 %";
		this.circularProgressBar2.TextMargin = new System.Windows.Forms.Padding(8, 8, 0, 0);
		this.circularProgressBar2.Value = 68;
		this.circularProgressBar1.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
		this.circularProgressBar1.AnimationSpeed = 500;
		this.circularProgressBar1.BackColor = System.Drawing.Color.Transparent;
		this.circularProgressBar1.Font = new System.Drawing.Font("Arial", 13f);
		this.circularProgressBar1.ForeColor = System.Drawing.Color.Black;
		this.circularProgressBar1.InnerColor = System.Drawing.Color.White;
		this.circularProgressBar1.InnerMargin = 2;
		this.circularProgressBar1.InnerWidth = -1;
		this.circularProgressBar1.Location = new System.Drawing.Point(181, 102);
		this.circularProgressBar1.MarqueeAnimationSpeed = 2000;
		this.circularProgressBar1.Name = "circularProgressBar1";
		this.circularProgressBar1.OuterColor = System.Drawing.Color.Gray;
		this.circularProgressBar1.OuterMargin = -12;
		this.circularProgressBar1.OuterWidth = 12;
		this.circularProgressBar1.ProgressColor = System.Drawing.Color.Lime;
		this.circularProgressBar1.ProgressWidth = 12;
		this.circularProgressBar1.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36f);
		this.circularProgressBar1.Size = new System.Drawing.Size(100, 100);
		this.circularProgressBar1.StartAngle = 270;
		this.circularProgressBar1.SubscriptColor = System.Drawing.Color.FromArgb(166, 166, 166);
		this.circularProgressBar1.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
		this.circularProgressBar1.SubscriptText = "";
		this.circularProgressBar1.SuperscriptColor = System.Drawing.Color.FromArgb(166, 166, 166);
		this.circularProgressBar1.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
		this.circularProgressBar1.SuperscriptText = "";
		this.circularProgressBar1.TabIndex = 1;
		this.circularProgressBar1.Text = "0 %";
		this.circularProgressBar1.TextMargin = new System.Windows.Forms.Padding(8, 8, 0, 0);
		this.circularProgressBar1.Value = 68;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(503, 368);
		base.Controls.Add(this.panel1);
		base.Name = "FormPerformance";
		this.Text = "Performance";
		base.Load += new System.EventHandler(FormPerformance_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
	}
}
