using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormDownload : FormMaterial
{
	public string NameFile;

	public string pathto;

	public long Recevied;

	public long Secound;

	public long SizeFile;

	public Clients client;

	public Clients parrent;

	private IContainer components;

	private MaterialProgressBar materialProgressBar1;

	private MaterialLabel materialLabel1;

	private MaterialLabel materialLabel2;

	private MaterialLabel materialLabel3;

	private MaterialLabel materialLabel4;

	public Timer timer2;

	private MaterialLabel materialLabel5;

	public Timer timer1;

	public FormDownload()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
		}
	}

	private void FormUpload_Load(object sender, EventArgs e)
	{
		timer1.Start();
		timer2.Start();
		materialProgressBar1.Maximum = (int)SizeFile;
		materialLabel3.Text = "File: " + NameFile;
		materialLabel4.Text = "Size: " + Methods.BytesToString(SizeFile);
		Clients clients = client;
		clients.Recevied = (Clients.Delegate)Delegate.Combine(clients.Recevied, (Clients.Delegate)delegate(long item)
		{
			Program.form.Invoke((MethodInvoker)delegate
			{
				Recevied += item;
				Secound += item;
				if (Recevied <= SizeFile)
				{
					materialProgressBar1.Value = (int)Recevied;
					materialLabel5.Text = "Size Recive: " + Methods.BytesToString(Recevied);
					client.lastPing.Last();
				}
			});
		});
	}

	private void timer2_Tick(object sender, EventArgs e)
	{
		materialLabel2.Text = Methods.BytesToString(Secound) + "\\sec";
		Secound = 0L;
		materialLabel1.Text = GetPercent(SizeFile, Recevied) + "% done";
	}

	public static long GetPercent(long b, long a)
	{
		if (b == 0L)
		{
			return 0L;
		}
		return (long)((decimal)a / ((decimal)b / 100m));
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
		this.materialProgressBar1 = new MaterialSkin.Controls.MaterialProgressBar();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
		this.timer2 = new System.Windows.Forms.Timer(this.components);
		this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		base.SuspendLayout();
		this.materialProgressBar1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.materialProgressBar1.Depth = 0;
		this.materialProgressBar1.Location = new System.Drawing.Point(6, 121);
		this.materialProgressBar1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialProgressBar1.Name = "materialProgressBar1";
		this.materialProgressBar1.Size = new System.Drawing.Size(323, 5);
		this.materialProgressBar1.TabIndex = 0;
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(6, 129);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(61, 19);
		this.materialLabel1.TabIndex = 1;
		this.materialLabel1.Text = "0% done";
		this.materialLabel2.AutoSize = true;
		this.materialLabel2.Depth = 0;
		this.materialLabel2.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel2.Location = new System.Drawing.Point(243, 129);
		this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel2.Name = "materialLabel2";
		this.materialLabel2.Size = new System.Drawing.Size(50, 19);
		this.materialLabel2.TabIndex = 2;
		this.materialLabel2.Text = "0b/sec";
		this.materialLabel3.AutoSize = true;
		this.materialLabel3.Depth = 0;
		this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel3.Location = new System.Drawing.Point(6, 95);
		this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel3.Name = "materialLabel3";
		this.materialLabel3.Size = new System.Drawing.Size(91, 19);
		this.materialLabel3.TabIndex = 3;
		this.materialLabel3.Text = "File: none.txt";
		this.materialLabel4.AutoSize = true;
		this.materialLabel4.Depth = 0;
		this.materialLabel4.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel4.Location = new System.Drawing.Point(6, 73);
		this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel4.Name = "materialLabel4";
		this.materialLabel4.Size = new System.Drawing.Size(71, 19);
		this.materialLabel4.TabIndex = 4;
		this.materialLabel4.Text = "Size: 0mb";
		this.timer2.Interval = 1000;
		this.timer2.Tick += new System.EventHandler(timer2_Tick);
		this.materialLabel5.AutoSize = true;
		this.materialLabel5.Depth = 0;
		this.materialLabel5.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel5.Location = new System.Drawing.Point(119, 73);
		this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel5.Name = "materialLabel5";
		this.materialLabel5.Size = new System.Drawing.Size(121, 19);
		this.materialLabel5.TabIndex = 5;
		this.materialLabel5.Text = "Size Recive: 0mb";
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(335, 162);
		base.Controls.Add(this.materialLabel5);
		base.Controls.Add(this.materialLabel4);
		base.Controls.Add(this.materialLabel3);
		base.Controls.Add(this.materialLabel2);
		base.Controls.Add(this.materialLabel1);
		base.Controls.Add(this.materialProgressBar1);
		this.MaximumSize = new System.Drawing.Size(335, 162);
		this.MinimumSize = new System.Drawing.Size(335, 162);
		base.Name = "FormDownload";
		this.Text = "Download";
		base.Load += new System.EventHandler(FormUpload_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
