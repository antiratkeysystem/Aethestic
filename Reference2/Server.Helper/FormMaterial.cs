using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Server.Data;

namespace Server.Helper;

public class FormMaterial : MaterialForm
{
	private class DarkThemeToolStripRenderer : ToolStripProfessionalRenderer
	{
		public DarkThemeToolStripRenderer()
			: base(new DarkThemeColorTable())
		{
			base.RoundedEdges = false;
		}
	}

	private class DarkThemeColorTable : ProfessionalColorTable
	{
		public override Color ToolStripDropDownBackground => DarkBackColor;

		public override Color ImageMarginGradientBegin => DarkBackColor;

		public override Color ImageMarginGradientMiddle => DarkBackColor;

		public override Color ImageMarginGradientEnd => DarkBackColor;

		public override Color MenuBorder => DarkSelectionBorderColor;

		public override Color MenuItemBorder => DarkSelectionBorderColor;

		public override Color MenuItemSelected => DarkSelectionBackColor;

		public override Color MenuItemSelectedGradientBegin => DarkSelectionBackColor;

		public override Color MenuItemSelectedGradientEnd => DarkSelectionBackColor;

		public override Color MenuStripGradientBegin => DarkBackColor;

		public override Color MenuStripGradientEnd => DarkBackColor;

		public override Color ToolStripGradientBegin => DarkBackColor;

		public override Color ToolStripGradientMiddle => DarkBackColor;

		public override Color ToolStripGradientEnd => DarkBackColor;

		public override Color MenuItemPressedGradientBegin => DarkSelectionBackColor;

		public override Color MenuItemPressedGradientEnd => DarkSelectionBackColor;

		public override Color MenuItemPressedGradientMiddle => DarkSelectionBackColor;
	}

	public static class RainbowThemeManager
	{
		private static Timer rainbowTimer;

		private static float rainbowHue;

		private static bool isRainbowActive;

		private static bool speedUp;

		private static Color originalPrimaryColor;

		private static bool originalColorSaved;

		private static Color styleColor;

		public static void Initialize()
		{
			if (rainbowTimer == null)
			{
				rainbowTimer = new Timer();
				rainbowTimer.Tick += RainbowTimer_Tick;
			}
			if (!File.Exists("local\\Settings.json"))
			{
				return;
			}
			Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
			if (settings.RainbowTheme)
			{
				if (!originalColorSaved)
				{
					originalPrimaryColor = PrimaryColor;
					styleColor = PrimaryColor;
					originalColorSaved = true;
				}
				Timer initTimer = new Timer();
				initTimer.Interval = 100;
				initTimer.Tick += delegate
				{
					initTimer.Stop();
					initTimer.Dispose();
					StartRainbowTheme(settings.SpeedUPTheme);
				};
				initTimer.Start();
			}
		}

		public static void StartRainbowTheme(bool speedUpMode)
		{
			if (isRainbowActive)
			{
				rainbowTimer.Stop();
			}
			if (!originalColorSaved)
			{
				originalPrimaryColor = PrimaryColor;
				styleColor = PrimaryColor;
				originalColorSaved = true;
			}
			isRainbowActive = true;
			speedUp = speedUpMode;
			rainbowTimer.Interval = (speedUp ? 15 : 50);
			ApplyRainbowColor();
			rainbowTimer.Start();
		}

		public static Color GetStyleColor()
		{
			if (isRainbowActive && originalColorSaved)
			{
				return styleColor;
			}
			return PrimaryColor;
		}

		public static void UpdateStyleColor(Color newColor)
		{
			styleColor = newColor;
			if (!isRainbowActive)
			{
				PrimaryColor = newColor;
			}
		}

		private static void ApplyRainbowColor()
		{
			if (!isRainbowActive)
			{
				return;
			}
			try
			{
				PrimaryColor = HslToRgb(rainbowHue, 1f, 0.5f);
				UpdateGridClientsOnly();
			}
			catch (Exception)
			{
			}
		}

		public static void StopRainbowTheme()
		{
			isRainbowActive = false;
			if (rainbowTimer != null)
			{
				rainbowTimer.Stop();
			}
			if (originalColorSaved)
			{
				PrimaryColor = originalPrimaryColor;
				originalColorSaved = false;
			}
			try
			{
				MaterialSkinManager instance = MaterialSkinManager.Instance;
				bool isDark = instance.Theme == MaterialSkinManager.Themes.DARK;
				foreach (Form form in Application.OpenForms)
				{
					if (form == null || !form.IsHandleCreated || form.IsDisposed)
					{
						continue;
					}
					try
					{
						if (form.InvokeRequired)
						{
							form.BeginInvoke((Action)delegate
							{
								try
								{
									RestoreNormalStyleToGrids(form, isDark);
									if (form is FormMaterial formMaterial)
									{
										formMaterial.ApplyThemeRecursive(form, isDark);
									}
									form.Invalidate(invalidateChildren: true);
									form.Refresh();
									form.Update();
								}
								catch
								{
								}
							});
						}
						else
						{
							RestoreNormalStyleToGrids(form, isDark);
							if (form is FormMaterial materialForm)
							{
								materialForm.ApplyThemeRecursive(form, isDark);
							}
							form.Invalidate(invalidateChildren: true);
							form.Refresh();
							form.Update();
						}
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}

		private static void RestoreNormalStyleToGrids(Form form, bool isDark)
		{
			try
			{
				Color gridBack = (isDark ? DarkBackColor : LightBackColor);
				Color gridFore = (isDark ? DarkForeColor : LightForeColor);
				Color selBack = PrimaryColor;
				Color selFore = GetContrastingTextColor(selBack);
				if (FindControlByName(form, "GridClients") is DataGridView gridClients)
				{
					RestoreSingleGrid(gridClients, gridBack, PrimaryColor, selBack, selFore);
				}
				if (FindControlByName(form, "GridLogs") is DataGridView gridLogs)
				{
					gridLogs.BackgroundColor = gridBack;
					if (gridLogs.ColumnHeadersDefaultCellStyle != null)
					{
						gridLogs.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryColor;
					}
					if (gridLogs.DefaultCellStyle != null)
					{
						gridLogs.DefaultCellStyle.SelectionBackColor = selBack;
						gridLogs.DefaultCellStyle.SelectionForeColor = selFore;
					}
					gridLogs.Refresh();
				}
				if (FindControlByName(form, "dataGridView2") is DataGridView gridTasks)
				{
					RestoreSingleGrid(gridTasks, gridBack, gridFore, selBack, selFore);
				}
			}
			catch
			{
			}
		}

		private static void RestoreNormalStyleToGrids(Form form)
		{
			try
			{
				bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
				Color gridBack = (num ? DarkBackColor : LightBackColor);
				Color gridFore = (num ? DarkForeColor : LightForeColor);
				Color selBack = PrimaryColor;
				Color selFore = GetContrastingTextColor(selBack);
				if (FindControlByName(form, "GridClients") is DataGridView gridClients)
				{
					RestoreSingleGrid(gridClients, gridBack, gridFore, selBack, selFore);
				}
				if (FindControlByName(form, "GridLogs") is DataGridView gridLogs)
				{
					RestoreSingleGrid(gridLogs, gridBack, gridFore, selBack, selFore);
				}
				if (FindControlByName(form, "dataGridView2") is DataGridView gridTasks)
				{
					RestoreSingleGrid(gridTasks, gridBack, gridFore, selBack, selFore);
				}
			}
			catch
			{
			}
		}

		private static void RestoreSingleGrid(DataGridView grid, Color gridBack, Color gridFore, Color selBack, Color selFore)
		{
			try
			{
				grid.BackgroundColor = gridBack;
				if (grid.DefaultCellStyle != null)
				{
					grid.DefaultCellStyle.BackColor = gridBack;
					grid.DefaultCellStyle.ForeColor = gridFore;
					grid.DefaultCellStyle.SelectionBackColor = selBack;
					grid.DefaultCellStyle.SelectionForeColor = selFore;
				}
				if (grid.AlternatingRowsDefaultCellStyle != null)
				{
					grid.AlternatingRowsDefaultCellStyle.BackColor = gridBack;
					grid.AlternatingRowsDefaultCellStyle.ForeColor = gridFore;
					grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = selBack;
					grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = selFore;
				}
				if (grid.ColumnHeadersDefaultCellStyle != null)
				{
					grid.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryColor;
				}
				grid.Refresh();
			}
			catch
			{
			}
		}

		public static void SetSpeedUp(bool speedUpMode)
		{
			speedUp = speedUpMode;
			if (isRainbowActive)
			{
				rainbowTimer.Interval = (speedUp ? 15 : 50);
			}
		}

		public static bool IsActive()
		{
			return isRainbowActive;
		}

		private static void RainbowTimer_Tick(object sender, EventArgs e)
		{
			if (!isRainbowActive)
			{
				return;
			}
			try
			{
				rainbowHue += 1.5f;
				if (rainbowHue >= 360f)
				{
					rainbowHue = 0f;
				}
				ApplyRainbowColor();
			}
			catch (Exception)
			{
			}
		}

		private static void UpdateGridClientsOnly()
		{
			try
			{
				foreach (Form form in Application.OpenForms)
				{
					if (form == null || !form.IsHandleCreated || form.IsDisposed || !(form.Name == "Form1"))
					{
						continue;
					}
					try
					{
						if (form.InvokeRequired)
						{
							form.BeginInvoke((Action)delegate
							{
								try
								{
									UpdateGridInForm(form);
								}
								catch
								{
								}
							});
						}
						else
						{
							UpdateGridInForm(form);
						}
					}
					catch
					{
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private static void UpdateGridInForm(Form form)
		{
			try
			{
				Color gridBack = ((MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK) ? DarkBackColor : LightBackColor);
				Color gridFore = PrimaryColor;
				Color selBack = PrimaryColor;
				Color selFore = Color.White;
				if (FindControlByName(form, "GridClients") is DataGridView gridClients)
				{
					UpdateSingleGrid(gridClients, gridBack, gridFore, selBack, selFore);
				}
				if (FindControlByName(form, "GridLogs") is DataGridView gridLogs)
				{
					UpdateSingleGrid(gridLogs, gridBack, gridFore, selBack, selFore);
				}
				if (FindControlByName(form, "dataGridView2") is DataGridView gridTasks)
				{
					UpdateSingleGrid(gridTasks, gridBack, gridFore, selBack, selFore);
				}
			}
			catch
			{
			}
		}

		private static void UpdateSingleGrid(DataGridView grid, Color gridBack, Color gridFore, Color selBack, Color selFore)
		{
			try
			{
				if (grid.DefaultCellStyle != null)
				{
					grid.DefaultCellStyle.BackColor = gridBack;
					grid.DefaultCellStyle.ForeColor = gridFore;
					grid.DefaultCellStyle.SelectionBackColor = selBack;
					grid.DefaultCellStyle.SelectionForeColor = selFore;
				}
				if (grid.AlternatingRowsDefaultCellStyle != null)
				{
					grid.AlternatingRowsDefaultCellStyle.BackColor = gridBack;
					grid.AlternatingRowsDefaultCellStyle.ForeColor = gridFore;
					grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = selBack;
					grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = selFore;
				}
				if (grid.ColumnHeadersDefaultCellStyle != null)
				{
					grid.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryColor;
				}
				grid.Refresh();
			}
			catch
			{
			}
		}

		private static Control FindControlByName(Control parent, string name)
		{
			if (parent.Name == name)
			{
				return parent;
			}
			foreach (Control control in parent.Controls)
			{
				Control found = FindControlByName(control, name);
				if (found != null)
				{
					return found;
				}
			}
			return null;
		}

		private static Color HslToRgb(float h, float s, float l)
		{
			float r;
			float g;
			float b;
			if (s == 0f)
			{
				r = (g = (b = l));
			}
			else
			{
				float q = ((l < 0.5f) ? (l * (1f + s)) : (l + s - l * s));
				float p = 2f * l - q;
				r = HueToRgb(p, q, h / 360f + 1f / 3f);
				g = HueToRgb(p, q, h / 360f);
				b = HueToRgb(p, q, h / 360f - 1f / 3f);
			}
			return Color.FromArgb((int)(r * 255f), (int)(g * 255f), (int)(b * 255f));
		}

		private static float HueToRgb(float p, float q, float t)
		{
			if (t < 0f)
			{
				t += 1f;
			}
			if (t > 1f)
			{
				t -= 1f;
			}
			if (t < 1f / 6f)
			{
				return p + (q - p) * 6f * t;
			}
			if (t < 0.5f)
			{
				return q;
			}
			if (t < 2f / 3f)
			{
				return p + (q - p) * (2f / 3f - t) * 6f;
			}
			return p;
		}

		public static void Dispose()
		{
			if (rainbowTimer != null)
			{
				rainbowTimer.Stop();
				rainbowTimer.Dispose();
				rainbowTimer = null;
			}
		}
	}

	private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

	private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

	private static readonly Color DarkBackColor = Color.FromArgb(40, 40, 40);

	private static readonly Color DarkForeColor = Color.WhiteSmoke;

	private static readonly Color LightBackColor = Color.White;

	private static readonly Color LightForeColor = Color.Black;

	private static readonly Color DarkSelectionBackColor = Color.FromArgb(70, 70, 70);

	private static readonly Color DarkSelectionBorderColor = Color.FromArgb(90, 90, 90);

	private static readonly ToolStripRenderer DarkToolStripRenderer = new DarkThemeToolStripRenderer();

	private static readonly ConcurrentDictionary<Type, PropertyInfo> _borderColorProps = new ConcurrentDictionary<Type, PropertyInfo>();

	private static readonly ConcurrentDictionary<Type, PropertyInfo> _borderFocusProps = new ConcurrentDictionary<Type, PropertyInfo>();

	private static readonly ConcurrentDictionary<Type, PropertyInfo> _listBackProps = new ConcurrentDictionary<Type, PropertyInfo>();

	private static readonly ConcurrentDictionary<Type, PropertyInfo> _listTextProps = new ConcurrentDictionary<Type, PropertyInfo>();

	private static readonly ConcurrentDictionary<Type, PropertyInfo> _iconColorProps = new ConcurrentDictionary<Type, PropertyInfo>();

	private static readonly ConcurrentDictionary<Type, PropertyInfo> _bgColorProps = new ConcurrentDictionary<Type, PropertyInfo>();

	private static readonly ConcurrentDictionary<Type, PropertyInfo> _textColorProps = new ConcurrentDictionary<Type, PropertyInfo>();

	public static Color PrimaryColor;

	private IContainer components;

	[DllImport("uxtheme.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

	[DllImport("dwmapi.dll")]
	private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

	public FormMaterial()
	{
		FormMaterial formMaterial = this;
		InitializeComponent();
		base.Sizable = true;
		MaterialSkinManager instance = MaterialSkinManager.Instance;
		instance.ColorSchemeChanged += delegate
		{
			formMaterial.ApplyThemeRecursive(formMaterial, instance.Theme == MaterialSkinManager.Themes.DARK);
			formMaterial.Refresh();
		};
		instance.ThemeChanged += delegate
		{
			bool isDark = instance.Theme == MaterialSkinManager.Themes.DARK;
			formMaterial.ApplyThemeRecursive(formMaterial, isDark);
			formMaterial.ApplyThemeToAllOpenForms(isDark);
		};
		if (GetType().Name == "Form1")
		{
			if (File.Exists("local\\Settings.json"))
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				RainbowThemeManager.Initialize();
				GetColorScheme(settings.Style, instance);
				instance.Theme = (settings.DarkTheme ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT);
			}
			else
			{
				GetColorScheme(Randomizer.random.Next(29), instance);
				instance.Theme = MaterialSkinManager.Themes.LIGHT;
				RainbowThemeManager.Initialize();
			}
		}
		base.Load += delegate
		{
			formMaterial.ApplyThemeRecursive(formMaterial, instance.Theme == MaterialSkinManager.Themes.DARK);
		};
	}

	private void ApplyThemeToAllOpenForms(bool isDark)
	{
		try
		{
			foreach (Form openForm in Application.OpenForms)
			{
				if (openForm is FormMaterial fm)
				{
					fm.ApplyThemeRecursive(fm, isDark);
					fm.Invalidate(invalidateChildren: true);
				}
			}
		}
		catch
		{
		}
	}

	public static void GetColorScheme(int index, MaterialSkinManager materialSkinManager)
	{
		Color selectedStyleColor = (index % 29) switch
		{
			0 => PrimaryColor = Color.FromArgb(120, 81, 169), 
			1 => PrimaryColor = Color.FromArgb(0, 51, 102), 
			2 => PrimaryColor = Color.FromArgb(128, 0, 0), 
			3 => PrimaryColor = Color.FromArgb(0, 128, 0), 
			4 => PrimaryColor = Color.FromArgb(255, 95, 31), 
			5 => PrimaryColor = Color.FromArgb(255, 212, 59), 
			6 => PrimaryColor = Color.FromArgb(48, 25, 52), 
			7 => PrimaryColor = Color.FromArgb(0, 100, 60), 
			8 => PrimaryColor = Color.FromArgb(100, 200, 235), 
			9 => PrimaryColor = Color.FromArgb(2, 168, 244), 
			10 => PrimaryColor = Color.FromArgb(176, 255, 0), 
			11 => PrimaryColor = Color.FromArgb(75, 0, 130), 
			12 => PrimaryColor = Color.FromArgb(207, 16, 32), 
			13 => PrimaryColor = Color.FromArgb(212, 175, 55), 
			14 => PrimaryColor = Color.FromArgb(255, 145, 175), 
			15 => PrimaryColor = Color.FromArgb(76, 187, 23), 
			16 => PrimaryColor = Color.FromArgb(111, 78, 55), 
			17 => PrimaryColor = Color.FromArgb(113, 121, 126), 
			18 => PrimaryColor = Color.FromArgb(255, 0, 127), 
			19 => PrimaryColor = Color.FromArgb(255, 0, 255), 
			20 => PrimaryColor = Color.FromArgb(80, 200, 120), 
			21 => PrimaryColor = Color.FromArgb(237, 145, 33), 
			22 => PrimaryColor = Color.FromArgb(43, 0, 80), 
			23 => PrimaryColor = Color.FromArgb(135, 206, 235), 
			24 => PrimaryColor = Color.FromArgb(205, 38, 38), 
			25 => PrimaryColor = Color.FromArgb(53, 56, 57), 
			26 => PrimaryColor = Color.FromArgb(240, 248, 255), 
			27 => PrimaryColor = Color.FromArgb(115, 130, 118), 
			28 => PrimaryColor = Color.FromArgb(18, 18, 18), 
			_ => PrimaryColor = Color.FromArgb(120, 81, 169), 
		};
		RainbowThemeManager.UpdateStyleColor(selectedStyleColor);
		Color primary = selectedStyleColor;
		Color primaryDark;
		Color primaryDarker;
		Color accent;
		TextShade textShade;
		if (index % 29 == 9)
		{
			primaryDark = Color.FromArgb(2, 117, 172);
			primaryDarker = Color.FromArgb(1, 85, 130);
			accent = Color.FromArgb(80, 200, 255);
			textShade = TextShade.WHITE;
		}
		else
		{
			primaryDark = Color.FromArgb(Math.Max(0, primary.R - 30), Math.Max(0, primary.G - 30), Math.Max(0, primary.B - 30));
			primaryDarker = Color.FromArgb(Math.Max(0, primary.R - 60), Math.Max(0, primary.G - 60), Math.Max(0, primary.B - 60));
			accent = Color.FromArgb(Math.Min(255, primary.R + 50), Math.Min(255, primary.G + 50), Math.Min(255, primary.B + 50));
			textShade = (((double)(int)primary.R * 0.299 + (double)(int)primary.G * 0.587 + (double)(int)primary.B * 0.114 > 186.0) ? TextShade.BLACK : TextShade.WHITE);
		}
		materialSkinManager.ColorScheme = new ColorScheme(primary, primaryDark, primaryDarker, accent, textShade);
	}

	private static Color ToColor(int argb)
	{
		return Color.FromArgb((argb & 0xFF0000) >> 16, (argb & 0xFF00) >> 8, argb & 0xFF);
	}

	private Color GetStyleColor()
	{
		return RainbowThemeManager.GetStyleColor();
	}

	public void ApplyThemeRecursive(Control control, bool isDark)
	{
		bool isRainbow = RainbowThemeManager.IsActive();
		Color styleColor = (isRainbow ? RainbowThemeManager.GetStyleColor() : PrimaryColor);
		HashSet<ToolStrip> processedStrips = new HashSet<ToolStrip>();
		if (control is Form form)
		{
			try
			{
				form.Refresh();
				int useImmersiveDarkMode = (isDark ? 1 : 0);
				if (DwmSetWindowAttribute(form.Handle, 20, ref useImmersiveDarkMode, 4) != 0)
				{
					DwmSetWindowAttribute(form.Handle, 19, ref useImmersiveDarkMode, 4);
				}
			}
			catch
			{
			}
		}
		ApplyThemeRecursiveCore(control, isDark, isRainbow, styleColor, processedStrips);
		try
		{
			FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo field in fields)
			{
				if (typeof(ContextMenuStrip).IsAssignableFrom(field.FieldType) && field.GetValue(this) is ContextMenuStrip cms && processedStrips.Add(cms))
				{
					ApplyThemeToToolStrip(cms, isDark);
				}
			}
		}
		catch
		{
		}
	}

	private static PropertyInfo GetCachedProp(ConcurrentDictionary<Type, PropertyInfo> cache, Type t, string name)
	{
		return cache.GetOrAdd(t, (Type _) => t.GetProperty(name));
	}

	private void ApplyThemeRecursiveCore(Control control, bool isDark, bool isRainbow, Color styleColor, HashSet<ToolStrip> processedStrips)
	{
		if (control == null)
		{
			return;
		}
		try
		{
			if (control is ScrollableControl || control is ListView || control is TreeView || control is TextBoxBase || control is ListBox || control is DataGridView)
			{
				if (isDark)
				{
					SetWindowTheme(control.Handle, "DarkMode_Explorer", null);
					if (control is DataGridView || control is ListView || control is TreeView)
					{
						SetWindowTheme(control.Handle, "Explorer", null);
						SetWindowTheme(control.Handle, "DarkMode_Explorer", null);
					}
				}
				else
				{
					SetWindowTheme(control.Handle, "Explorer", null);
				}
				foreach (Control child in control.Controls)
				{
					if (child.GetType().Name.Contains("ScrollBar"))
					{
						if (isDark)
						{
							SetWindowTheme(child.Handle, "DarkMode_Explorer", null);
						}
						else
						{
							SetWindowTheme(child.Handle, "Explorer", null);
						}
					}
				}
			}
			if (control is MaterialSlider slider)
			{
				slider.BackColor = (isDark ? DarkBackColor : LightBackColor);
				slider.ForeColor = (isDark ? DarkForeColor : LightForeColor);
				try
				{
					slider.Invalidate();
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
		if (control.ContextMenuStrip != null && processedStrips.Add(control.ContextMenuStrip))
		{
			ApplyThemeToToolStrip(control.ContextMenuStrip, isDark);
		}
		string typeName = control.GetType().Name;
		if (!(control is MaterialForm) && !(control is MaterialButton) && !(control is MaterialLabel) && !(control is MaterialSwitch) && !(control is MaterialCheckbox) && !(control is MaterialTextBox))
		{
			if (control is TrackBar tb)
			{
				tb.BackColor = (isDark ? DarkBackColor : LightBackColor);
			}
			else if (control is PictureBox pb)
			{
				if (isDark && (pb.BackColor == SystemColors.Control || pb.BackColor == Color.White))
				{
					pb.BackColor = DarkBackColor;
				}
				else if (!isDark && pb.BackColor == DarkBackColor)
				{
					pb.BackColor = SystemColors.Control;
				}
			}
			else if (typeName == "RJTextBox")
			{
				Control parent = control.Parent;
				while (parent != null && !(parent is Form))
				{
					parent = parent.Parent;
				}
				string formTypeName = parent?.GetType().Name ?? "";
				if (formTypeName != "FormBulider" && formTypeName != "FormFun")
				{
					control.BackColor = (isDark ? DarkBackColor : LightBackColor);
					control.ForeColor = (isDark ? DarkForeColor : LightForeColor);
					Type t = control.GetType();
					GetCachedProp(_borderColorProps, t, "BorderColor")?.SetValue(control, styleColor, null);
					GetCachedProp(_borderFocusProps, t, "BorderFocusColor")?.SetValue(control, styleColor, null);
				}
			}
			else if (typeName == "RJComboBox")
			{
				Control parent2 = control.Parent;
				while (parent2 != null && !(parent2 is Form))
				{
					parent2 = parent2.Parent;
				}
				string formTypeName2 = parent2?.GetType().Name ?? "";
				if (formTypeName2 != "FormBulider" && formTypeName2 != "FormFun")
				{
					control.BackColor = (isDark ? DarkBackColor : LightBackColor);
					control.ForeColor = (isDark ? DarkForeColor : LightForeColor);
					Type t2 = control.GetType();
					GetCachedProp(_listBackProps, t2, "ListBackColor")?.SetValue(control, isDark ? DarkBackColor : LightBackColor, null);
					GetCachedProp(_listTextProps, t2, "ListTextColor")?.SetValue(control, isDark ? DarkForeColor : LightForeColor, null);
					GetCachedProp(_iconColorProps, t2, "IconColor")?.SetValue(control, styleColor, null);
					GetCachedProp(_borderColorProps, t2, "BorderColor")?.SetValue(control, styleColor, null);
				}
			}
			else if (control is GroupBox || control is Panel || control is TabPage)
			{
				if (control is TabPage page)
				{
					page.UseVisualStyleBackColor = false;
				}
				if (isDark && (control.BackColor == Color.White || control.BackColor == SystemColors.Control))
				{
					control.BackColor = DarkBackColor;
				}
				else if (!isDark && control.BackColor == DarkBackColor)
				{
					control.BackColor = LightBackColor;
				}
				else if (!(control is TabPage))
				{
					control.BackColor = (isDark ? DarkBackColor : LightBackColor);
				}
				control.ForeColor = ((control is GroupBox) ? styleColor : (isDark ? DarkForeColor : LightForeColor));
			}
			else if (control is Label label)
			{
				Control parent3 = control.Parent;
				while (parent3 != null && !(parent3 is Form))
				{
					parent3 = parent3.Parent;
				}
				if (!(parent3?.GetType().Name == "FormAbout") || !(label.Name == "label1"))
				{
					label.ForeColor = (isDark ? DarkForeColor : LightForeColor);
					if (isDark && (label.BackColor == Color.White || label.BackColor == SystemColors.Control))
					{
						label.BackColor = Color.Transparent;
					}
					else if (!isDark && label.BackColor == DarkBackColor)
					{
						label.BackColor = Color.Transparent;
					}
				}
			}
			else if (control is CheckBox || control is RadioButton)
			{
				control.ForeColor = (isDark ? DarkForeColor : LightForeColor);
			}
			else if (control is TreeView tv)
			{
				tv.BackColor = (isDark ? DarkBackColor : LightBackColor);
				tv.ForeColor = (isDark ? DarkForeColor : LightForeColor);
				if (isDark)
				{
					tv.LineColor = Color.WhiteSmoke;
				}
			}
			else if (control is ListView lv)
			{
				lv.BackColor = (isDark ? DarkBackColor : LightBackColor);
				lv.ForeColor = (isDark ? DarkForeColor : LightForeColor);
			}
			else if (control is TextBoxBase || control is ComboBox || control is ListBox || typeName == "HexEditor")
			{
				control.BackColor = (isDark ? DarkBackColor : LightBackColor);
				control.ForeColor = (isDark ? DarkForeColor : LightForeColor);
			}
			else if (control is DataGridView grid)
			{
				Form form = grid.FindForm();
				string gridName = grid.Name;
				bool isForm1 = form?.Name == "Form1";
				if (!(gridName == "GridLogs" && isForm1) || isRainbow)
				{
					ResetGridLocalStyles(grid);
				}
				Color selFore = GetContrastingTextColor(styleColor);
				if (gridName == "GridClients" && isForm1)
				{
					Color gridBack = (grid.BackgroundColor = (isDark ? DarkBackColor : LightBackColor));
					if (grid.DefaultCellStyle != null)
					{
						grid.DefaultCellStyle.BackColor = gridBack;
						grid.DefaultCellStyle.ForeColor = styleColor;
						grid.DefaultCellStyle.SelectionBackColor = styleColor;
						grid.DefaultCellStyle.SelectionForeColor = selFore;
					}
					if (grid.AlternatingRowsDefaultCellStyle != null)
					{
						grid.AlternatingRowsDefaultCellStyle.BackColor = gridBack;
						grid.AlternatingRowsDefaultCellStyle.ForeColor = styleColor;
						grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = styleColor;
						grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = selFore;
					}
					if (grid.ColumnHeadersDefaultCellStyle != null)
					{
						grid.ColumnHeadersDefaultCellStyle.ForeColor = styleColor;
						grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
						grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = grid.ColumnHeadersDefaultCellStyle.ForeColor;
					}
				}
				else if ((gridName == "GridLogs" || gridName == "dataGridView2") && isForm1)
				{
					Color gridBack2 = (isDark ? DarkBackColor : LightBackColor);
					Color gridFore = (isDark ? DarkForeColor : LightForeColor);
					grid.BackgroundColor = gridBack2;
					if (grid.DefaultCellStyle != null)
					{
						grid.DefaultCellStyle.BackColor = gridBack2;
						grid.DefaultCellStyle.ForeColor = (isRainbow ? styleColor : gridFore);
						grid.DefaultCellStyle.SelectionBackColor = styleColor;
						grid.DefaultCellStyle.SelectionForeColor = selFore;
					}
					if (grid.ColumnHeadersDefaultCellStyle != null)
					{
						grid.ColumnHeadersDefaultCellStyle.BackColor = gridBack2;
						grid.ColumnHeadersDefaultCellStyle.ForeColor = styleColor;
						grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
						grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = grid.ColumnHeadersDefaultCellStyle.ForeColor;
					}
					if (grid.AlternatingRowsDefaultCellStyle != null)
					{
						grid.AlternatingRowsDefaultCellStyle.BackColor = gridBack2;
						grid.AlternatingRowsDefaultCellStyle.ForeColor = (isRainbow ? styleColor : gridFore);
						grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = styleColor;
						grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = selFore;
					}
				}
				else
				{
					Color gridBack3 = (isDark ? DarkBackColor : LightBackColor);
					if (!isDark)
					{
						_ = LightForeColor;
					}
					else
					{
						_ = DarkForeColor;
					}
					grid.BackgroundColor = gridBack3;
					if (grid.DefaultCellStyle != null)
					{
						grid.DefaultCellStyle.BackColor = gridBack3;
						grid.DefaultCellStyle.ForeColor = styleColor;
						grid.DefaultCellStyle.SelectionBackColor = styleColor;
						grid.DefaultCellStyle.SelectionForeColor = selFore;
					}
					if (grid.ColumnHeadersDefaultCellStyle != null)
					{
						grid.ColumnHeadersDefaultCellStyle.BackColor = gridBack3;
						grid.ColumnHeadersDefaultCellStyle.ForeColor = styleColor;
						grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
						grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = grid.ColumnHeadersDefaultCellStyle.ForeColor;
					}
					if (grid.AlternatingRowsDefaultCellStyle != null)
					{
						grid.AlternatingRowsDefaultCellStyle.BackColor = gridBack3;
						grid.AlternatingRowsDefaultCellStyle.ForeColor = styleColor;
						grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = styleColor;
						grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = selFore;
					}
				}
			}
			else if (typeName == "RJButton")
			{
				string ownerName = control.FindForm()?.GetType().Name ?? "";
				if (ownerName != "FormBulider" && ownerName != "FormFun")
				{
					control.BackColor = styleColor;
					Type t3 = control.GetType();
					GetCachedProp(_bgColorProps, t3, "BackgroundColor")?.SetValue(control, styleColor, null);
					GetCachedProp(_textColorProps, t3, "TextColor")?.SetValue(control, Color.White, null);
				}
			}
			else if (control is NumericUpDown numeric)
			{
				numeric.BackColor = (isDark ? DarkBackColor : LightBackColor);
				numeric.ForeColor = styleColor;
			}
			else if (control is SplitContainer split)
			{
				split.BackColor = (isDark ? DarkBackColor : LightBackColor);
				split.Panel1.BackColor = (isDark ? DarkBackColor : LightBackColor);
				split.Panel2.BackColor = (isDark ? DarkBackColor : LightBackColor);
			}
			else if (control is TabControl tabCtrl)
			{
				tabCtrl.BackColor = (isDark ? DarkBackColor : LightBackColor);
			}
			else if (control is TrackBar trackBar)
			{
				trackBar.BackColor = (isDark ? DarkBackColor : LightBackColor);
			}
			else if (control is PictureBox pictureBox)
			{
				pictureBox.BackColor = (isDark ? DarkBackColor : LightBackColor);
			}
			else if (control is StatusStrip statusStrip)
			{
				ApplyThemeToToolStrip(statusStrip, isDark);
			}
			else if (control is MenuStrip menuStrip)
			{
				ApplyThemeToToolStrip(menuStrip, isDark);
			}
			else if (control is ToolStrip toolStrip)
			{
				ApplyThemeToToolStrip(toolStrip, isDark);
			}
		}
		if (control.ContextMenuStrip != null && processedStrips.Add(control.ContextMenuStrip))
		{
			ApplyThemeToToolStrip(control.ContextMenuStrip, isDark);
		}
		if (control is DataGridView)
		{
			try
			{
				control.BeginInvoke((Action)delegate
				{
					control.Refresh();
				});
				return;
			}
			catch
			{
				return;
			}
		}
		foreach (Control child2 in control.Controls)
		{
			ApplyThemeRecursiveCore(child2, isDark, isRainbow, styleColor, processedStrips);
		}
	}

	private static Color GetContrastingTextColor(Color back)
	{
		if (!(0.299 * (double)(int)back.R + 0.587 * (double)(int)back.G + 0.114 * (double)(int)back.B > 160.0))
		{
			return Color.White;
		}
		return Color.Black;
	}

	private static void ResetGridLocalStyles(DataGridView grid)
	{
		if (grid == null)
		{
			return;
		}
		try
		{
			if (grid.Rows == null || grid.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataGridViewRow row in (IEnumerable)grid.Rows)
			{
				if (row == null)
				{
					continue;
				}
				row.DefaultCellStyle = new DataGridViewCellStyle();
				foreach (DataGridViewCell cell in row.Cells)
				{
					if (cell != null)
					{
						cell.Style = new DataGridViewCellStyle();
					}
				}
			}
		}
		catch
		{
		}
	}

	private void ApplyThemeToToolStrip(ToolStrip strip, bool isDark)
	{
		if (strip == null)
		{
			return;
		}
		strip.BackColor = (isDark ? DarkBackColor : LightBackColor);
		strip.ForeColor = (isDark ? DarkForeColor : LightForeColor);
		strip.Renderer = (isDark ? DarkToolStripRenderer : null);
		foreach (ToolStripItem item in strip.Items)
		{
			ApplyThemeToToolStripItem(item, isDark);
		}
		if (!(strip is ContextMenuStrip contextMenu))
		{
			return;
		}
		if (contextMenu.OwnerItem != null)
		{
			ApplyThemeToToolStripItem(contextMenu.OwnerItem, isDark);
		}
		foreach (ToolStripItem item2 in contextMenu.Items)
		{
			if (item2 is ToolStripSeparator separator)
			{
				separator.BackColor = (isDark ? DarkBackColor : LightBackColor);
				separator.ForeColor = (isDark ? DarkSelectionBorderColor : Color.DarkGray);
			}
		}
	}

	private void ApplyThemeToToolStripItem(ToolStripItem item, bool isDark)
	{
		if (item == null)
		{
			return;
		}
		item.BackColor = (isDark ? DarkBackColor : LightBackColor);
		item.ForeColor = (isDark ? DarkForeColor : LightForeColor);
		if (!(item is ToolStripDropDownItem { DropDown: not null } dropDownItem))
		{
			return;
		}
		dropDownItem.DropDown.BackColor = (isDark ? DarkBackColor : LightBackColor);
		dropDownItem.DropDown.ForeColor = (isDark ? DarkForeColor : LightForeColor);
		foreach (ToolStripItem subItem in dropDownItem.DropDownItems)
		{
			ApplyThemeToToolStripItem(subItem, isDark);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Helper.FormMaterial));
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "FormMaterial";
		this.Text = "FormMaterial";
		base.ResumeLayout(false);
	}
}
