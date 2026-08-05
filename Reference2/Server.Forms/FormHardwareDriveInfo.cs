using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using Server.Helper;

namespace Server.Forms;

public class FormHardwareDriveInfo : FormMaterial
{
	private readonly string _drive;

	private readonly string _type;

	private readonly string _total;

	private readonly string _free;

	private readonly string _files;

	private readonly string _visible;

	private IContainer components;

	public FormHardwareDriveInfo(string drive, string type, string total, string free, string files, string visible)
	{
		_drive = drive ?? "";
		_type = type ?? "HDD";
		_total = total ?? "0 B";
		_free = free ?? "0 B";
		_files = files ?? "0";
		_visible = visible ?? "Visible";
		InitializeComponent();
	}

	private void FormHardwareDriveInfo_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		Text = "Drive " + _drive + " — Information";
		base.Size = new Size(420, 320);
		base.FormBorderStyle = FormBorderStyle.Sizable;
		base.StartPosition = FormStartPosition.CenterParent;
		Font = new Font("Segoe UI", 9.75f);
		int y = 80;
		int left = 24;
		int rowHeight = 32;
		AddLabel(left, y, "Drive:", isHeader: true);
		AddLabel(left + 130, y, _drive, isHeader: false);
		y += rowHeight;
		AddLabel(left, y, "Type:", isHeader: true);
		AddLabel(left + 130, y, _type, isHeader: false);
		y += rowHeight;
		AddLabel(left, y, "Total size:", isHeader: true);
		AddLabel(left + 130, y, _total, isHeader: false);
		y += rowHeight;
		AddLabel(left, y, "Free space:", isHeader: true);
		AddLabel(left + 130, y, _free, isHeader: false);
		y += rowHeight;
		AddLabel(left, y, "Files (root):", isHeader: true);
		AddLabel(left + 130, y, _files, isHeader: false);
		y += rowHeight;
		AddLabel(left, y, "In \"This PC\":", isHeader: true);
		AddLabel(left + 130, y, _visible, isHeader: false);
		y += rowHeight;
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Button btn = new Button
		{
			Text = "OK",
			Location = new Point(base.ClientSize.Width / 2 - 45, y + 20),
			Size = new Size(90, 32),
			BackColor = (isDark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(240, 240, 240)),
			ForeColor = (isDark ? Color.WhiteSmoke : Color.Black),
			FlatStyle = FlatStyle.Flat
		};
		btn.FlatAppearance.BorderColor = (isDark ? Color.FromArgb(80, 80, 80) : Color.DarkGray);
		btn.Click += delegate
		{
			Close();
		};
		base.Controls.Add(btn);
	}

	private void ChangeScheme(object sender)
	{
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		BackColor = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
		foreach (Control c in base.Controls)
		{
			if (c is Label l)
			{
				l.BackColor = BackColor;
				if (l.Font.Bold)
				{
					l.ForeColor = (isDark ? Color.FromArgb(180, 180, 180) : Color.FromArgb(80, 80, 80));
				}
				else
				{
					l.ForeColor = (isDark ? Color.WhiteSmoke : Color.Black);
				}
			}
			else if (c is Button b)
			{
				b.BackColor = (isDark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(240, 240, 240));
				b.ForeColor = (isDark ? Color.WhiteSmoke : Color.Black);
				b.FlatAppearance.BorderColor = (isDark ? Color.FromArgb(80, 80, 80) : Color.DarkGray);
			}
		}
	}

	private void AddLabel(int x, int y, string text, bool isHeader)
	{
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Label l = new Label
		{
			Text = text,
			Location = new Point(x, y),
			AutoSize = true,
			Font = (isHeader ? new Font("Segoe UI", 10f, FontStyle.Bold) : new Font("Segoe UI", 10f)),
			ForeColor = ((!isHeader) ? (isDark ? Color.WhiteSmoke : Color.Black) : (isDark ? Color.FromArgb(180, 180, 180) : Color.FromArgb(80, 80, 80))),
			BackColor = (isDark ? Color.FromArgb(40, 40, 40) : Color.White)
		};
		base.Controls.Add(l);
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
		base.SuspendLayout();
		base.ClientSize = new System.Drawing.Size(400, 280);
		base.Name = "FormHardwareDriveInfo";
		base.Load += new System.EventHandler(FormHardwareDriveInfo_Load);
		base.ResumeLayout(false);
	}
}
