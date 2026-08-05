using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Server.Forms;

public class FormNotification : Form
{
	private double opacity;

	private const int FADE_SPEED = 30;

	private const int DISPLAY_TIME = 4000;

	private bool isDarkTheme = true;

	private int targetY;

	private int startY;

	private bool isDisconnect;

	private IContainer components;

	private Label lblTitle;

	private Label lblIP;

	private Label lblUsername;

	private Label lblCountry;

	private Label lblHwid;

	private PictureBox iconBox;

	private Panel mainPanel;

	private Timer slideTimer;

	private Timer fadeInTimer;

	private Timer fadeOutTimer;

	private Timer displayTimer;

	public FormNotification(string ip, string username, string country, string hwid, bool isNewUser, bool darkTheme = true, bool disconnect = false)
	{
		isDarkTheme = darkTheme;
		isDisconnect = disconnect;
		InitializeComponent();
		ApplyTheme();
		if (isDisconnect)
		{
			SetupDisconnectNotification(ip, username, country, hwid);
		}
		else
		{
			SetupNotification(ip, username, country, hwid, isNewUser);
		}
		slideTimer = new Timer();
		slideTimer.Interval = 10;
		slideTimer.Tick += SlideTimer_Tick;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		targetY = base.Location.Y;
		startY = targetY + 200;
		base.Location = new Point(base.Location.X, startY);
		StartAnimation();
	}

	private void SlideTimer_Tick(object sender, EventArgs e)
	{
		int currentY = base.Location.Y;
		int step = 15;
		if (currentY > targetY)
		{
			int newY = currentY - step;
			if (newY < targetY)
			{
				newY = targetY;
			}
			base.Location = new Point(base.Location.X, newY);
		}
		else
		{
			slideTimer.Stop();
		}
	}

	private void ApplyTheme()
	{
		if (isDarkTheme)
		{
			BackColor = Color.FromArgb(45, 45, 48);
			mainPanel.BackColor = Color.FromArgb(30, 30, 30);
			lblIP.ForeColor = Color.White;
			lblUsername.ForeColor = Color.White;
			lblCountry.ForeColor = Color.White;
			lblHwid.ForeColor = Color.FromArgb(200, 200, 200);
		}
		else
		{
			BackColor = Color.FromArgb(240, 240, 240);
			mainPanel.BackColor = Color.White;
			lblIP.ForeColor = Color.Black;
			lblUsername.ForeColor = Color.Black;
			lblCountry.ForeColor = Color.Black;
			lblHwid.ForeColor = Color.FromArgb(80, 80, 80);
		}
	}

	private void SetupNotification(string ip, string username, string country, string hwid, bool isNewUser)
	{
		lblTitle.Text = (isNewUser ? "Новый пользователь!" : "Подключился пользователь!");
		if (isNewUser)
		{
			lblTitle.ForeColor = Color.FromArgb(0, 255, 127);
		}
		else
		{
			lblTitle.ForeColor = Color.FromArgb(100, 200, 255);
		}
		lblIP.Text = "IP: " + ip;
		lblUsername.Text = "User: " + username;
		lblCountry.Text = "Country: " + country;
		string shortHwid = hwid;
		if (!string.IsNullOrEmpty(hwid) && hwid.Length > 32)
		{
			shortHwid = hwid.Substring(0, 32) + "...";
		}
		lblHwid.Text = "HWID: " + shortHwid;
		try
		{
			if (isNewUser)
			{
				Bitmap icon = new Bitmap(48, 48);
				using (Graphics g = Graphics.FromImage(icon))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.Clear(Color.Transparent);
					using (SolidBrush brush = new SolidBrush(Color.FromArgb(0, 255, 127)))
					{
						g.FillEllipse(brush, 4, 4, 40, 40);
					}
					using Pen pen = new Pen(Color.White, 3f);
					g.DrawLine(pen, 14, 24, 22, 32);
					g.DrawLine(pen, 22, 32, 34, 16);
				}
				iconBox.Image = icon;
				return;
			}
			Bitmap icon2 = new Bitmap(48, 48);
			using (Graphics g2 = Graphics.FromImage(icon2))
			{
				g2.SmoothingMode = SmoothingMode.AntiAlias;
				g2.Clear(Color.Transparent);
				using (SolidBrush brush2 = new SolidBrush(Color.FromArgb(100, 200, 255)))
				{
					g2.FillEllipse(brush2, 4, 4, 40, 40);
				}
				using Pen pen2 = new Pen(Color.White, 3f);
				g2.DrawArc(pen2, 12, 12, 24, 24, 0, 270);
				g2.DrawLine(pen2, 36, 16, 36, 24);
				g2.DrawLine(pen2, 36, 16, 28, 16);
			}
			iconBox.Image = icon2;
		}
		catch
		{
		}
	}

	private void SetupDisconnectNotification(string ip, string username, string country, string hwid)
	{
		lblTitle.Text = "Пользователь отключился!";
		lblTitle.ForeColor = Color.FromArgb(255, 69, 58);
		lblIP.Text = "IP: " + ip;
		lblUsername.Text = "User: " + username;
		lblCountry.Text = "Country: " + country;
		string shortHwid = hwid;
		if (!string.IsNullOrEmpty(hwid) && hwid.Length > 32)
		{
			shortHwid = hwid.Substring(0, 32) + "...";
		}
		lblHwid.Text = "HWID: " + shortHwid;
		try
		{
			Bitmap icon = new Bitmap(48, 48);
			using (Graphics g = Graphics.FromImage(icon))
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.Clear(Color.Transparent);
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 69, 58)))
				{
					g.FillEllipse(brush, 4, 4, 40, 40);
				}
				using Pen pen = new Pen(Color.White, 3f);
				g.DrawLine(pen, 16, 16, 32, 32);
				g.DrawLine(pen, 32, 16, 16, 32);
			}
			iconBox.Image = icon;
		}
		catch
		{
		}
	}

	private void StartAnimation()
	{
		opacity = 0.0;
		base.Opacity = 0.0;
		fadeInTimer.Start();
		slideTimer.Start();
	}

	private void FadeInTimer_Tick(object sender, EventArgs e)
	{
		opacity += 0.1;
		if (opacity >= 1.0)
		{
			opacity = 1.0;
			base.Opacity = opacity;
			fadeInTimer.Stop();
			displayTimer.Start();
		}
		else
		{
			base.Opacity = opacity;
		}
	}

	private void DisplayTimer_Tick(object sender, EventArgs e)
	{
		displayTimer.Stop();
		StartFadeOut();
	}

	private void StartFadeOut()
	{
		if (!fadeOutTimer.Enabled)
		{
			fadeInTimer.Stop();
			displayTimer.Stop();
			fadeOutTimer.Start();
		}
	}

	private void FadeOutTimer_Tick(object sender, EventArgs e)
	{
		opacity -= 0.1;
		if (opacity <= 0.0)
		{
			opacity = 0.0;
			base.Opacity = 0.0;
			fadeOutTimer.Stop();
			Close();
		}
		else
		{
			base.Opacity = opacity;
		}
	}

	private void FormNotification_Click(object sender, EventArgs e)
	{
		fadeInTimer.Stop();
		fadeOutTimer.Stop();
		displayTimer.Stop();
		slideTimer.Stop();
		Close();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		using Pen pen = new Pen(isDarkTheme ? Color.FromArgb(95, 158, 160) : Color.FromArgb(70, 130, 180), 2f);
		e.Graphics.DrawRectangle(pen, 0, 0, base.Width - 1, base.Height - 1);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			if (slideTimer != null)
			{
				slideTimer.Stop();
				slideTimer.Dispose();
			}
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.mainPanel = new System.Windows.Forms.Panel();
		this.lblHwid = new System.Windows.Forms.Label();
		this.lblCountry = new System.Windows.Forms.Label();
		this.lblUsername = new System.Windows.Forms.Label();
		this.lblIP = new System.Windows.Forms.Label();
		this.lblTitle = new System.Windows.Forms.Label();
		this.iconBox = new System.Windows.Forms.PictureBox();
		this.fadeInTimer = new System.Windows.Forms.Timer(this.components);
		this.fadeOutTimer = new System.Windows.Forms.Timer(this.components);
		this.displayTimer = new System.Windows.Forms.Timer(this.components);
		this.mainPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.iconBox).BeginInit();
		base.SuspendLayout();
		this.mainPanel.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
		this.mainPanel.Controls.Add(this.lblHwid);
		this.mainPanel.Controls.Add(this.lblCountry);
		this.mainPanel.Controls.Add(this.lblUsername);
		this.mainPanel.Controls.Add(this.lblIP);
		this.mainPanel.Controls.Add(this.lblTitle);
		this.mainPanel.Controls.Add(this.iconBox);
		this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainPanel.Location = new System.Drawing.Point(0, 0);
		this.mainPanel.Name = "mainPanel";
		this.mainPanel.Padding = new System.Windows.Forms.Padding(2);
		this.mainPanel.Size = new System.Drawing.Size(350, 150);
		this.mainPanel.TabIndex = 0;
		this.mainPanel.Click += new System.EventHandler(FormNotification_Click);
		this.lblHwid.BackColor = System.Drawing.Color.Transparent;
		this.lblHwid.Font = new System.Drawing.Font("Segoe UI", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.lblHwid.ForeColor = System.Drawing.Color.FromArgb(150, 150, 150);
		this.lblHwid.Location = new System.Drawing.Point(75, 105);
		this.lblHwid.Name = "lblHwid";
		this.lblHwid.Size = new System.Drawing.Size(260, 20);
		this.lblHwid.TabIndex = 5;
		this.lblHwid.Text = "HWID: Unknown";
		this.lblHwid.Click += new System.EventHandler(FormNotification_Click);
		this.lblCountry.BackColor = System.Drawing.Color.Transparent;
		this.lblCountry.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.lblCountry.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
		this.lblCountry.Location = new System.Drawing.Point(75, 85);
		this.lblCountry.Name = "lblCountry";
		this.lblCountry.Size = new System.Drawing.Size(260, 20);
		this.lblCountry.TabIndex = 4;
		this.lblCountry.Text = "Country: Unknown";
		this.lblCountry.Click += new System.EventHandler(FormNotification_Click);
		this.lblUsername.BackColor = System.Drawing.Color.Transparent;
		this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
		this.lblUsername.Location = new System.Drawing.Point(75, 65);
		this.lblUsername.Name = "lblUsername";
		this.lblUsername.Size = new System.Drawing.Size(260, 20);
		this.lblUsername.TabIndex = 3;
		this.lblUsername.Text = "User: Unknown";
		this.lblUsername.Click += new System.EventHandler(FormNotification_Click);
		this.lblIP.BackColor = System.Drawing.Color.Transparent;
		this.lblIP.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.lblIP.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
		this.lblIP.Location = new System.Drawing.Point(75, 45);
		this.lblIP.Name = "lblIP";
		this.lblIP.Size = new System.Drawing.Size(260, 20);
		this.lblIP.TabIndex = 2;
		this.lblIP.Text = "IP: 0.0.0.0";
		this.lblIP.Click += new System.EventHandler(FormNotification_Click);
		this.lblTitle.BackColor = System.Drawing.Color.Transparent;
		this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
		this.lblTitle.ForeColor = System.Drawing.Color.White;
		this.lblTitle.Location = new System.Drawing.Point(75, 15);
		this.lblTitle.Name = "lblTitle";
		this.lblTitle.Size = new System.Drawing.Size(260, 25);
		this.lblTitle.TabIndex = 1;
		this.lblTitle.Text = "Новый пользователь";
		this.lblTitle.Click += new System.EventHandler(FormNotification_Click);
		this.iconBox.BackColor = System.Drawing.Color.Transparent;
		this.iconBox.Location = new System.Drawing.Point(15, 15);
		this.iconBox.Name = "iconBox";
		this.iconBox.Size = new System.Drawing.Size(48, 48);
		this.iconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.iconBox.TabIndex = 0;
		this.iconBox.TabStop = false;
		this.iconBox.Click += new System.EventHandler(FormNotification_Click);
		this.fadeInTimer.Interval = 30;
		this.fadeInTimer.Tick += new System.EventHandler(FadeInTimer_Tick);
		this.fadeOutTimer.Interval = 30;
		this.fadeOutTimer.Tick += new System.EventHandler(FadeOutTimer_Tick);
		this.displayTimer.Interval = 4000;
		this.displayTimer.Tick += new System.EventHandler(DisplayTimer_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
		base.ClientSize = new System.Drawing.Size(350, 150);
		base.Controls.Add(this.mainPanel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "FormNotification";
		base.Opacity = 0.0;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
		base.TopMost = true;
		base.Click += new System.EventHandler(FormNotification_Click);
		this.mainPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.iconBox).EndInit();
		base.ResumeLayout(false);
	}
}
