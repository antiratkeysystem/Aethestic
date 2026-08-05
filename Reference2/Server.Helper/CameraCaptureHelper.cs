using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Server.Helper;

internal class CameraCaptureHelper
{
	private VideoCaptureDevice _device;

	private readonly object _jpegLock = new object();

	private static readonly ImageCodecInfo _jpegEncoder = ImageCodecInfo.GetImageEncoders().First((ImageCodecInfo c) => c.FormatID == ImageFormat.Jpeg.Guid);

	public static string[] GetDeviceNames()
	{
		try
		{
			FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			string[] names = new string[devices.Count];
			for (int i = 0; i < devices.Count; i++)
			{
				names[i] = devices[i].Name;
			}
			return names;
		}
		catch
		{
			return new string[0];
		}
	}

	public void Start(int deviceIndex, int quality, Action<byte[]> onFrame)
	{
		Stop();
		try
		{
			FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			if (deviceIndex < 0 || deviceIndex >= devices.Count)
			{
				return;
			}
			_device = new VideoCaptureDevice(devices[deviceIndex].MonikerString);
			_device.NewFrame += delegate(object sender, NewFrameEventArgs e)
			{
				try
				{
					using Bitmap bitmap = (Bitmap)e.Frame.Clone();
					byte[] array = BitmapToJpeg(bitmap, quality);
					if (array != null && onFrame != null)
					{
						onFrame(array);
					}
				}
				catch
				{
				}
			};
			_device.Start();
		}
		catch
		{
		}
	}

	public void Stop()
	{
		try
		{
			if (_device != null && _device.IsRunning)
			{
				_device.SignalToStop();
				_device.WaitForStop();
				_device.NewFrame -= null;
				_device = null;
			}
		}
		catch
		{
		}
	}

	private static byte[] BitmapToJpeg(Bitmap bitmap, int quality)
	{
		if (bitmap == null)
		{
			return null;
		}
		int w = bitmap.Width;
		int h = bitmap.Height;
		if (w > 640 || h > 480)
		{
			float scale = Math.Min(640f / (float)w, 480f / (float)h);
			w = (int)((float)w * scale);
			h = (int)((float)h * scale);
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
