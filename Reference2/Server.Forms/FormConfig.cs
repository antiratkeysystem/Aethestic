using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormConfig : FormMaterial
{
	private IContainer components;

	private Timer timer1;

	private Panel contentPanel;

	private MaterialLabel materialLabel1;

	private MaterialLabel lblHwid;

	private MaterialLabel materialLabel2;

	private MaterialLabel lblPath;

	private MaterialLabel materialLabel3;

	private MaterialLabel lblFileName;

	private MaterialLabel materialLabel4;

	private MaterialLabel lblGroup;

	private MaterialLabel materialLabel5;

	private MaterialLabel lblVersion;

	private MaterialLabel materialLabel6;

	private MaterialLabel lblAdmin;

	private MaterialLabel materialLabel7;

	private MaterialLabel lblOS;

	private MaterialLabel materialLabel8;

	private MaterialLabel lblConnType;

	private MaterialCheckbox chkPersistence;

	private MaterialCheckbox chkAntiAnalysis;

	private MaterialCheckbox chkStartup;

	private MaterialButton btnRefresh;

	private MaterialLabel lblStatus;

	public Clients Client { get; set; }

	public FormConfig()
	{
		InitializeComponent();
	}

	private void FormConfig_Load(object sender, EventArgs e)
	{
		if (Client != null)
		{
			Text = "Client Config [" + Client.IP + "]";
			if (Client.itsConnect)
			{
				Client.Send(new object[1] { "Config" });
			}
		}
		timer1.Start();
		LoadConfigFromRow();
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (Client != null && !Client.itsConnect)
		{
			Close();
		}
	}

	public void LoadConfigFromRow()
	{
		if (Client != null && Client.Tag != null && Client.Tag is DataGridViewRow)
		{
			DataGridViewRow row = (DataGridViewRow)Client.Tag;
			lblHwid.Text = GetCellValue(row, 6);
			lblOS.Text = GetCellValue(row, 11);
			lblGroup.Text = GetCellValue(row, 4);
			lblStatus.Text = "Basic info loaded. Requesting full config...";
		}
	}

	public void UpdateConfig(object[] data)
	{
		if (base.InvokeRequired)
		{
			Invoke(new Action<object[]>(UpdateConfig), new object[1] { data });
			return;
		}
		try
		{
			if (data.Length >= 10)
			{
				lblHwid.Text = data[2].ToString();
				lblPath.Text = data[3].ToString();
				lblFileName.Text = data[4].ToString();
				lblGroup.Text = data[5].ToString();
				materialLabel5.Text = "Host/Paste:";
				lblVersion.Text = data[7].ToString();
				materialLabel6.Text = "Assembly:";
				lblAdmin.Text = data[10].ToString();
				materialLabel7.Text = "Startup Key:";
				lblOS.Text = ((data.Length > 12) ? data[12].ToString() : "N/A");
				materialLabel8.Text = "Port/Key:";
				lblConnType.Text = data[8].ToString() + " / " + data[9].ToString();
				lblStatus.Text = "Full config received at " + DateTime.Now.ToLongTimeString();
			}
		}
		catch (Exception ex)
		{
			lblStatus.Text = "Parse error: " + ex.Message;
		}
	}

	private string GetCellValue(DataGridViewRow row, int index)
	{
		if (row.Cells.Count > index && row.Cells[index].Value != null)
		{
			return row.Cells[index].Value.ToString();
		}
		return "---";
	}

	private void btnRefresh_Click(object sender, EventArgs e)
	{
		if (Client != null && Client.itsConnect)
		{
			Client.Send(new object[1] { "Config" });
			lblStatus.Text = "Requesting config from client...";
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
		this.contentPanel = new System.Windows.Forms.Panel();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.lblHwid = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
		this.lblPath = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
		this.lblFileName = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
		this.lblGroup = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
		this.lblVersion = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel6 = new MaterialSkin.Controls.MaterialLabel();
		this.lblAdmin = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel7 = new MaterialSkin.Controls.MaterialLabel();
		this.lblOS = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel8 = new MaterialSkin.Controls.MaterialLabel();
		this.lblConnType = new MaterialSkin.Controls.MaterialLabel();
		this.chkPersistence = new MaterialSkin.Controls.MaterialCheckbox();
		this.chkAntiAnalysis = new MaterialSkin.Controls.MaterialCheckbox();
		this.chkStartup = new MaterialSkin.Controls.MaterialCheckbox();
		this.btnRefresh = new MaterialSkin.Controls.MaterialButton();
		this.lblStatus = new MaterialSkin.Controls.MaterialLabel();
		this.contentPanel.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.contentPanel.Controls.Add(this.lblStatus);
		this.contentPanel.Controls.Add(this.btnRefresh);
		this.contentPanel.Controls.Add(this.chkStartup);
		this.contentPanel.Controls.Add(this.chkAntiAnalysis);
		this.contentPanel.Controls.Add(this.chkPersistence);
		this.contentPanel.Controls.Add(this.lblConnType);
		this.contentPanel.Controls.Add(this.materialLabel8);
		this.contentPanel.Controls.Add(this.lblOS);
		this.contentPanel.Controls.Add(this.materialLabel7);
		this.contentPanel.Controls.Add(this.lblAdmin);
		this.contentPanel.Controls.Add(this.materialLabel6);
		this.contentPanel.Controls.Add(this.lblVersion);
		this.contentPanel.Controls.Add(this.materialLabel5);
		this.contentPanel.Controls.Add(this.lblGroup);
		this.contentPanel.Controls.Add(this.materialLabel4);
		this.contentPanel.Controls.Add(this.lblFileName);
		this.contentPanel.Controls.Add(this.materialLabel3);
		this.contentPanel.Controls.Add(this.lblPath);
		this.contentPanel.Controls.Add(this.materialLabel2);
		this.contentPanel.Controls.Add(this.lblHwid);
		this.contentPanel.Controls.Add(this.materialLabel1);
		this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.contentPanel.Location = new System.Drawing.Point(3, 64);
		this.contentPanel.Name = "contentPanel";
		this.contentPanel.Size = new System.Drawing.Size(450, 400);
		this.contentPanel.TabIndex = 0;
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(20, 20);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(49, 19);
		this.materialLabel1.TabIndex = 0;
		this.materialLabel1.Text = "HWID:";
		this.lblHwid.AutoSize = true;
		this.lblHwid.Depth = 0;
		this.lblHwid.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblHwid.Location = new System.Drawing.Point(140, 20);
		this.lblHwid.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblHwid.Name = "lblHwid";
		this.lblHwid.Size = new System.Drawing.Size(20, 19);
		this.lblHwid.TabIndex = 1;
		this.lblHwid.Text = "---";
		this.materialLabel2.AutoSize = true;
		this.materialLabel2.Depth = 0;
		this.materialLabel2.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel2.Location = new System.Drawing.Point(20, 50);
		this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel2.Name = "materialLabel2";
		this.materialLabel2.Size = new System.Drawing.Size(86, 19);
		this.materialLabel2.TabIndex = 2;
		this.materialLabel2.Text = "Install Path:";
		this.lblPath.AutoSize = true;
		this.lblPath.Depth = 0;
		this.lblPath.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblPath.Location = new System.Drawing.Point(140, 50);
		this.lblPath.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblPath.Name = "lblPath";
		this.lblPath.Size = new System.Drawing.Size(20, 19);
		this.lblPath.TabIndex = 3;
		this.lblPath.Text = "---";
		this.materialLabel3.AutoSize = true;
		this.materialLabel3.Depth = 0;
		this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel3.Location = new System.Drawing.Point(20, 80);
		this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel3.Name = "materialLabel3";
		this.materialLabel3.Size = new System.Drawing.Size(76, 19);
		this.materialLabel3.TabIndex = 4;
		this.materialLabel3.Text = "File Name:";
		this.lblFileName.AutoSize = true;
		this.lblFileName.Depth = 0;
		this.lblFileName.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblFileName.Location = new System.Drawing.Point(140, 80);
		this.lblFileName.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblFileName.Name = "lblFileName";
		this.lblFileName.Size = new System.Drawing.Size(20, 19);
		this.lblFileName.TabIndex = 5;
		this.lblFileName.Text = "---";
		this.materialLabel4.AutoSize = true;
		this.materialLabel4.Depth = 0;
		this.materialLabel4.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel4.Location = new System.Drawing.Point(20, 110);
		this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel4.Name = "materialLabel4";
		this.materialLabel4.Size = new System.Drawing.Size(48, 19);
		this.materialLabel4.TabIndex = 6;
		this.materialLabel4.Text = "Group:";
		this.lblGroup.AutoSize = true;
		this.lblGroup.Depth = 0;
		this.lblGroup.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblGroup.Location = new System.Drawing.Point(140, 110);
		this.lblGroup.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblGroup.Name = "lblGroup";
		this.lblGroup.Size = new System.Drawing.Size(20, 19);
		this.lblGroup.TabIndex = 7;
		this.lblGroup.Text = "---";
		this.materialLabel5.AutoSize = true;
		this.materialLabel5.Depth = 0;
		this.materialLabel5.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel5.Location = new System.Drawing.Point(20, 140);
		this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel5.Name = "materialLabel5";
		this.materialLabel5.Size = new System.Drawing.Size(59, 19);
		this.materialLabel5.TabIndex = 8;
		this.materialLabel5.Text = "Version:";
		this.lblVersion.AutoSize = true;
		this.lblVersion.Depth = 0;
		this.lblVersion.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblVersion.Location = new System.Drawing.Point(140, 140);
		this.lblVersion.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblVersion.Name = "lblVersion";
		this.lblVersion.Size = new System.Drawing.Size(20, 19);
		this.lblVersion.TabIndex = 9;
		this.lblVersion.Text = "---";
		this.materialLabel6.AutoSize = true;
		this.materialLabel6.Depth = 0;
		this.materialLabel6.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel6.Location = new System.Drawing.Point(20, 170);
		this.materialLabel6.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel6.Name = "materialLabel6";
		this.materialLabel6.Size = new System.Drawing.Size(73, 19);
		this.materialLabel6.TabIndex = 10;
		this.materialLabel6.Text = "Privileges:";
		this.lblAdmin.AutoSize = true;
		this.lblAdmin.Depth = 0;
		this.lblAdmin.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblAdmin.Location = new System.Drawing.Point(140, 170);
		this.lblAdmin.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblAdmin.Name = "lblAdmin";
		this.lblAdmin.Size = new System.Drawing.Size(20, 19);
		this.lblAdmin.TabIndex = 11;
		this.lblAdmin.Text = "---";
		this.materialLabel7.AutoSize = true;
		this.materialLabel7.Depth = 0;
		this.materialLabel7.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel7.Location = new System.Drawing.Point(20, 200);
		this.materialLabel7.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel7.Name = "materialLabel7";
		this.materialLabel7.Size = new System.Drawing.Size(25, 19);
		this.materialLabel7.TabIndex = 12;
		this.materialLabel7.Text = "OS:";
		this.lblOS.AutoSize = true;
		this.lblOS.Depth = 0;
		this.lblOS.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblOS.Location = new System.Drawing.Point(140, 200);
		this.lblOS.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblOS.Name = "lblOS";
		this.lblOS.Size = new System.Drawing.Size(20, 19);
		this.lblOS.TabIndex = 13;
		this.lblOS.Text = "---";
		this.materialLabel8.AutoSize = true;
		this.materialLabel8.Depth = 0;
		this.materialLabel8.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel8.Location = new System.Drawing.Point(20, 230);
		this.materialLabel8.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel8.Name = "materialLabel8";
		this.materialLabel8.Size = new System.Drawing.Size(83, 19);
		this.materialLabel8.TabIndex = 14;
		this.materialLabel8.Text = "Conn. Type:";
		this.lblConnType.AutoSize = true;
		this.lblConnType.Depth = 0;
		this.lblConnType.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblConnType.Location = new System.Drawing.Point(140, 230);
		this.lblConnType.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblConnType.Name = "lblConnType";
		this.lblConnType.Size = new System.Drawing.Size(20, 19);
		this.lblConnType.TabIndex = 15;
		this.lblConnType.Text = "---";
		this.chkPersistence.AutoSize = true;
		this.chkPersistence.Depth = 0;
		this.chkPersistence.Enabled = false;
		this.chkPersistence.Location = new System.Drawing.Point(20, 270);
		this.chkPersistence.Margin = new System.Windows.Forms.Padding(0);
		this.chkPersistence.MouseLocation = new System.Drawing.Point(-1, -1);
		this.chkPersistence.MouseState = MaterialSkin.MouseState.HOVER;
		this.chkPersistence.Name = "chkPersistence";
		this.chkPersistence.ReadOnly = false;
		this.chkPersistence.Ripple = true;
		this.chkPersistence.Size = new System.Drawing.Size(117, 37);
		this.chkPersistence.TabIndex = 16;
		this.chkPersistence.Text = "Persistence";
		this.chkPersistence.UseVisualStyleBackColor = true;
		this.chkAntiAnalysis.AutoSize = true;
		this.chkAntiAnalysis.Depth = 0;
		this.chkAntiAnalysis.Enabled = false;
		this.chkAntiAnalysis.Location = new System.Drawing.Point(150, 270);
		this.chkAntiAnalysis.Margin = new System.Windows.Forms.Padding(0);
		this.chkAntiAnalysis.MouseLocation = new System.Drawing.Point(-1, -1);
		this.chkAntiAnalysis.MouseState = MaterialSkin.MouseState.HOVER;
		this.chkAntiAnalysis.Name = "chkAntiAnalysis";
		this.chkAntiAnalysis.ReadOnly = false;
		this.chkAntiAnalysis.Ripple = true;
		this.chkAntiAnalysis.Size = new System.Drawing.Size(128, 37);
		this.chkAntiAnalysis.TabIndex = 17;
		this.chkAntiAnalysis.Text = "Anti-Analysis";
		this.chkAntiAnalysis.UseVisualStyleBackColor = true;
		this.chkStartup.AutoSize = true;
		this.chkStartup.Depth = 0;
		this.chkStartup.Enabled = false;
		this.chkStartup.Location = new System.Drawing.Point(290, 270);
		this.chkStartup.Margin = new System.Windows.Forms.Padding(0);
		this.chkStartup.MouseLocation = new System.Drawing.Point(-1, -1);
		this.chkStartup.MouseState = MaterialSkin.MouseState.HOVER;
		this.chkStartup.Name = "chkStartup";
		this.chkStartup.ReadOnly = false;
		this.chkStartup.Ripple = true;
		this.chkStartup.Size = new System.Drawing.Size(87, 37);
		this.chkStartup.TabIndex = 18;
		this.chkStartup.Text = "Startup";
		this.chkStartup.UseVisualStyleBackColor = true;
		this.btnRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.btnRefresh.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.btnRefresh.Depth = 0;
		this.btnRefresh.HighEmphasis = true;
		this.btnRefresh.Icon = null;
		this.btnRefresh.Location = new System.Drawing.Point(20, 320);
		this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.btnRefresh.MouseState = MaterialSkin.MouseState.HOVER;
		this.btnRefresh.Name = "btnRefresh";
		this.btnRefresh.NoAccentTextColor = System.Drawing.Color.Empty;
		this.btnRefresh.Size = new System.Drawing.Size(84, 36);
		this.btnRefresh.TabIndex = 19;
		this.btnRefresh.Text = "Refresh";
		this.btnRefresh.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.btnRefresh.UseAccentColor = false;
		this.btnRefresh.UseVisualStyleBackColor = true;
		this.btnRefresh.Click += new System.EventHandler(btnRefresh_Click);
		this.lblStatus.AutoSize = true;
		this.lblStatus.Depth = 0;
		this.lblStatus.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblStatus.Location = new System.Drawing.Point(20, 365);
		this.lblStatus.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblStatus.Name = "lblStatus";
		this.lblStatus.Size = new System.Drawing.Size(94, 19);
		this.lblStatus.TabIndex = 20;
		this.lblStatus.Text = "Please wait...";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(456, 467);
		base.Controls.Add(this.contentPanel);
		base.Name = "FormConfig";
		this.Text = "Client Config";
		base.Load += new System.EventHandler(FormConfig_Load);
		this.contentPanel.ResumeLayout(false);
		this.contentPanel.PerformLayout();
		base.ResumeLayout(false);
	}
}
