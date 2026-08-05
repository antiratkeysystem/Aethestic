using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormMap : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	public Timer timer1;

	public GMapControl gMapControl1;

	public FormMap()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormMap_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		gMapControl1.DragButton = MouseButtons.Left;
		gMapControl1.MapProvider = GMapProviders.GoogleMap;
	}

	private void ChangeScheme(object sender)
	{
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
		this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		base.SuspendLayout();
		this.gMapControl1.Bearing = 0f;
		this.gMapControl1.CanDragMap = true;
		this.gMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
		this.gMapControl1.Enabled = false;
		this.gMapControl1.GrayScaleMode = false;
		this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
		this.gMapControl1.LevelsKeepInMemory = 5;
		this.gMapControl1.Location = new System.Drawing.Point(3, 64);
		this.gMapControl1.MarkersEnabled = true;
		this.gMapControl1.MaxZoom = 15;
		this.gMapControl1.MinZoom = 5;
		this.gMapControl1.MouseWheelZoomEnabled = true;
		this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
		this.gMapControl1.Name = "gMapControl1";
		this.gMapControl1.NegativeMode = false;
		this.gMapControl1.PolygonsEnabled = true;
		this.gMapControl1.RetryLoadTile = 0;
		this.gMapControl1.RoutesEnabled = true;
		this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
		this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(33, 65, 105, 225);
		this.gMapControl1.ShowTileGridLines = false;
		this.gMapControl1.Size = new System.Drawing.Size(794, 383);
		this.gMapControl1.TabIndex = 0;
		this.gMapControl1.Zoom = 12.0;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.gMapControl1);
		base.Name = "FormMap";
		this.Text = "Map";
		base.Load += new System.EventHandler(FormMap_Load);
		base.ResumeLayout(false);
	}
}
