using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using WinFormAnimation;

namespace Server.Helper;

[ToolboxItem(true)]
[ToolboxBitmap(typeof(CircularProgressBar), "CircularProgressBar.bmp")]
[DefaultBindingProperty("Value")]
public class CircularProgressBar : ProgressBar
{
	private readonly Animator _animator;

	private int? _animatedStartAngle;

	private float? _animatedValue;

	private AnimationFunctions.Function _animationFunction;

	private Brush _backBrush;

	private KnownAnimationFunctions _knownAnimationFunction;

	private ProgressBarStyle? _lastStyle;

	private int _lastValue;

	[Category("Behavior")]
	public KnownAnimationFunctions AnimationFunction
	{
		get
		{
			return _knownAnimationFunction;
		}
		set
		{
			_animationFunction = AnimationFunctions.FromKnown(value);
			_knownAnimationFunction = value;
		}
	}

	[Category("Behavior")]
	public int AnimationSpeed { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public AnimationFunctions.Function CustomAnimationFunction
	{
		private get
		{
			return _animationFunction;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			_knownAnimationFunction = KnownAnimationFunctions.None;
			_animationFunction = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public override Font Font
	{
		get
		{
			return base.Font;
		}
		set
		{
			base.Font = value;
		}
	}

	[Category("Appearance")]
	public Color InnerColor { get; set; }

	[Category("Layout")]
	public int InnerMargin { get; set; }

	[Category("Layout")]
	public int InnerWidth { get; set; }

	[Category("Appearance")]
	public Color OuterColor { get; set; }

	[Category("Layout")]
	public int OuterMargin { get; set; }

	[Category("Layout")]
	public int OuterWidth { get; set; }

	[Category("Appearance")]
	public Color ProgressColor { get; set; }

	[Category("Layout")]
	public int ProgressWidth { get; set; }

	[Category("Appearance")]
	public Font SecondaryFont { get; set; }

	[Category("Layout")]
	public int StartAngle { get; set; }

	[Category("Appearance")]
	public Color SubscriptColor { get; set; }

	[Category("Layout")]
	public Padding SubscriptMargin { get; set; }

	[Category("Appearance")]
	public string SubscriptText { get; set; }

	[Category("Appearance")]
	public Color SuperscriptColor { get; set; }

	[Category("Layout")]
	public Padding SuperscriptMargin { get; set; }

	[Category("Appearance")]
	public string SuperscriptText { get; set; }

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	[Category("Layout")]
	public Padding TextMargin { get; set; }

	public CircularProgressBar()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		_animator = (base.DesignMode ? null : new Animator());
		AnimationFunction = KnownAnimationFunctions.Liner;
		AnimationSpeed = 500;
		base.MarqueeAnimationSpeed = 2000;
		StartAngle = 270;
		_lastValue = base.Value;
		BackColor = Color.Transparent;
		ForeColor = Color.FromArgb(64, 64, 64);
		DoubleBuffered = true;
		Font = new Font(Font.FontFamily, 72f, FontStyle.Bold);
		SecondaryFont = new Font(Font.FontFamily, (float)((double)Font.Size * 0.5), FontStyle.Regular);
		OuterMargin = -25;
		OuterWidth = 26;
		OuterColor = Color.Gray;
		ProgressWidth = 25;
		ProgressColor = Color.FromArgb(255, 128, 0);
		InnerMargin = 2;
		InnerWidth = -1;
		InnerColor = Color.FromArgb(224, 224, 224);
		TextMargin = new Padding(8, 8, 0, 0);
		base.Value = 68;
		SuperscriptMargin = new Padding(10, 35, 0, 0);
		SuperscriptColor = Color.FromArgb(166, 166, 166);
		SuperscriptText = "°C";
		SubscriptMargin = new Padding(10, -35, 0, 0);
		SubscriptColor = Color.FromArgb(166, 166, 166);
		SubscriptText = ".23";
		base.Size = new Size(320, 320);
	}

	private static PointF AddPoint(PointF p, int v)
	{
		p.X += v;
		p.Y += v;
		return p;
	}

	private static SizeF AddSize(SizeF s, int v)
	{
		s.Height += v;
		s.Width += v;
		return s;
	}

	private static Rectangle ToRectangle(RectangleF rect)
	{
		return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
	}

	protected override void OnLocationChanged(EventArgs e)
	{
		base.OnLocationChanged(e);
		Invalidate();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		try
		{
			if (!base.DesignMode)
			{
				if (base.Style == ProgressBarStyle.Marquee)
				{
					InitializeMarquee(_lastStyle != base.Style);
				}
				else
				{
					InitializeContinues(_lastStyle != base.Style);
				}
				_lastStyle = base.Style;
			}
			if (_backBrush == null)
			{
				RecreateBackgroundBrush();
			}
			StartPaint(e.Graphics);
		}
		catch
		{
		}
	}

	protected override void OnParentBackColorChanged(EventArgs e)
	{
		RecreateBackgroundBrush();
	}

	protected override void OnParentBackgroundImageChanged(EventArgs e)
	{
		RecreateBackgroundBrush();
	}

	protected override void OnParentChanged(EventArgs e)
	{
		if (base.Parent != null)
		{
			base.Parent.Invalidated -= ParentOnInvalidated;
			base.Parent.Resize -= ParentOnResize;
		}
		base.OnParentChanged(e);
		if (base.Parent != null)
		{
			base.Parent.Invalidated += ParentOnInvalidated;
			base.Parent.Resize += ParentOnResize;
		}
	}

	protected override void OnStyleChanged(EventArgs e)
	{
		base.OnStyleChanged(e);
		Invalidate();
	}

	protected virtual void InitializeContinues(bool firstTime)
	{
		if (_lastValue == base.Value && !firstTime)
		{
			return;
		}
		_lastValue = base.Value;
		_animator.Stop();
		_animatedStartAngle = null;
		if (AnimationSpeed <= 0)
		{
			_animatedValue = base.Value;
			Invalidate();
			return;
		}
		_animator.Paths = new Path(_animatedValue ?? ((float)base.Value), base.Value, (ulong)AnimationSpeed, CustomAnimationFunction).ToArray();
		_animator.Repeat = false;
		_animator.Play(new SafeInvoker<float>(delegate(float v)
		{
			try
			{
				_animatedValue = v;
				Invalidate();
			}
			catch
			{
				_animator.Stop();
			}
		}, this));
	}

	protected virtual void InitializeMarquee(bool firstTime)
	{
		if (!firstTime && (_animator.ActivePath == null || (_animator.ActivePath.Duration == (ulong)base.MarqueeAnimationSpeed && _animator.ActivePath.Function == CustomAnimationFunction)))
		{
			return;
		}
		_animator.Stop();
		_animatedValue = null;
		if (AnimationSpeed <= 0)
		{
			_animatedStartAngle = 0;
			Invalidate();
			return;
		}
		_animator.Paths = new Path(0f, 359f, (ulong)base.MarqueeAnimationSpeed, CustomAnimationFunction).ToArray();
		_animator.Repeat = true;
		_animator.Play(new SafeInvoker<float>(delegate(float v)
		{
			try
			{
				_animatedStartAngle = (int)(v % 360f);
				Invalidate();
			}
			catch
			{
				_animator.Stop();
			}
		}, this));
	}

	protected virtual void ParentOnInvalidated(object sender, InvalidateEventArgs invalidateEventArgs)
	{
		RecreateBackgroundBrush();
	}

	protected virtual void ParentOnResize(object sender, EventArgs eventArgs)
	{
		RecreateBackgroundBrush();
	}

	protected virtual void RecreateBackgroundBrush()
	{
		lock (this)
		{
			_backBrush?.Dispose();
			_backBrush = new SolidBrush(BackColor);
			if (BackColor.A == byte.MaxValue)
			{
				return;
			}
			if (base.Parent != null && base.Parent.Width > 0 && base.Parent.Height > 0)
			{
				using (Bitmap bitmap = new Bitmap(base.Parent.Width, base.Parent.Height))
				{
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						PaintEventArgs e = new PaintEventArgs(graphics, new Rectangle(new Point(0, 0), bitmap.Size));
						InvokePaintBackground(base.Parent, e);
						InvokePaint(base.Parent, e);
						if (BackColor.A > 0)
						{
							graphics.FillRectangle(_backBrush, base.Bounds);
						}
					}
					_backBrush = new TextureBrush(bitmap);
					((TextureBrush)_backBrush).TranslateTransform(0f - (float)base.Bounds.X, 0f - (float)base.Bounds.Y);
					return;
				}
			}
			_backBrush = new SolidBrush(Color.FromArgb(255, BackColor));
		}
	}

	protected virtual void StartPaint(Graphics g)
	{
		try
		{
			lock (this)
			{
				g.TextRenderingHint = TextRenderingHint.AntiAlias;
				g.SmoothingMode = SmoothingMode.AntiAlias;
				PointF pointF = AddPoint(Point.Empty, 2);
				SizeF sizeF = AddSize(base.Size, -4);
				if (OuterWidth + OuterMargin < 0)
				{
					int num = Math.Abs(OuterWidth + OuterMargin);
					pointF = AddPoint(Point.Empty, num);
					sizeF = AddSize(base.Size, -2 * num);
				}
				if (OuterColor != Color.Empty && OuterColor != Color.Transparent && OuterWidth != 0)
				{
					g.FillEllipse(new SolidBrush(OuterColor), new RectangleF(pointF, sizeF));
					if (OuterWidth >= 0)
					{
						pointF = AddPoint(pointF, OuterWidth);
						sizeF = AddSize(sizeF, -2 * OuterWidth);
						g.FillEllipse(_backBrush, new RectangleF(pointF, sizeF));
					}
				}
				pointF = AddPoint(pointF, OuterMargin);
				sizeF = AddSize(sizeF, -2 * OuterMargin);
				g.FillPie(new SolidBrush(ProgressColor), ToRectangle(new RectangleF(pointF, sizeF)), _animatedStartAngle ?? StartAngle, ((_animatedValue ?? ((float)base.Value)) - (float)base.Minimum) / (float)(base.Maximum - base.Minimum) * 360f);
				if (ProgressWidth >= 0)
				{
					pointF = AddPoint(pointF, ProgressWidth);
					sizeF = AddSize(sizeF, -2 * ProgressWidth);
					g.FillEllipse(_backBrush, new RectangleF(pointF, sizeF));
				}
				pointF = AddPoint(pointF, InnerMargin);
				sizeF = AddSize(sizeF, -2 * InnerMargin);
				if (InnerColor != Color.Empty && InnerColor != Color.Transparent && InnerWidth != 0)
				{
					g.FillEllipse(new SolidBrush(InnerColor), new RectangleF(pointF, sizeF));
					if (InnerWidth >= 0)
					{
						pointF = AddPoint(pointF, InnerWidth);
						sizeF = AddSize(sizeF, -2 * InnerWidth);
						g.FillEllipse(_backBrush, new RectangleF(pointF, sizeF));
					}
				}
				if (Text == string.Empty)
				{
					return;
				}
				pointF.X += TextMargin.Left;
				pointF.Y += TextMargin.Top;
				sizeF.Width -= TextMargin.Right;
				sizeF.Height -= TextMargin.Bottom;
				StringFormat format = new StringFormat((RightToLeft == RightToLeft.Yes) ? StringFormatFlags.DirectionRightToLeft : ((StringFormatFlags)0))
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Near
				};
				SizeF size = g.MeasureString(Text, Font);
				PointF location = new PointF(pointF.X + (sizeF.Width - size.Width) / 2f, pointF.Y + (sizeF.Height - size.Height) / 2f);
				if (SubscriptText != string.Empty || SuperscriptText != string.Empty)
				{
					float num2 = 0f;
					SizeF size2 = SizeF.Empty;
					SizeF size3 = SizeF.Empty;
					if (SuperscriptText != string.Empty)
					{
						size2 = g.MeasureString(SuperscriptText, SecondaryFont);
						num2 = Math.Max(size2.Width, num2);
						size2.Width -= SuperscriptMargin.Right;
						size2.Height -= SuperscriptMargin.Bottom;
					}
					if (SubscriptText != string.Empty)
					{
						size3 = g.MeasureString(SubscriptText, SecondaryFont);
						num2 = Math.Max(size3.Width, num2);
						size3.Width -= SubscriptMargin.Right;
						size3.Height -= SubscriptMargin.Bottom;
					}
					location.X -= num2 / 4f;
					if (SuperscriptText != string.Empty)
					{
						PointF location2 = new PointF(location.X + size.Width - size2.Width / 2f, location.Y - size2.Height * 0.85f);
						location2.X += SuperscriptMargin.Left;
						location2.Y += SuperscriptMargin.Top;
						g.DrawString(SuperscriptText, SecondaryFont, new SolidBrush(SuperscriptColor), new RectangleF(location2, size2), format);
					}
					if (SubscriptText != string.Empty)
					{
						PointF location3 = new PointF(location.X + size.Width - size3.Width / 2f, location.Y + size.Height * 0.85f);
						location3.X += SubscriptMargin.Left;
						location3.Y += SubscriptMargin.Top;
						g.DrawString(SubscriptText, SecondaryFont, new SolidBrush(SubscriptColor), new RectangleF(location3, size3), format);
					}
				}
				g.DrawString(Text, Font, new SolidBrush(ForeColor), new RectangleF(location, size), format);
			}
		}
		catch
		{
		}
	}
}
