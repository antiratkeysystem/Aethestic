using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using Toolbelt.Drawing;

namespace Server.Helper;

internal class Methods
{
	private static readonly ConcurrentDictionary<string, string> _checksumCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private const int MaxLogRows = 3000;

	private const int MaxLogsPerClientPerSecond = 5;

	private const int MaxTotalLogsPerSecond = 30;

	private static readonly ConcurrentDictionary<string, int> _logCountPerClient = new ConcurrentDictionary<string, int>();

	private static readonly ConcurrentDictionary<string, DateTime> _logWindowPerClient = new ConcurrentDictionary<string, DateTime>();

	private static int _globalLogCount = 0;

	private static DateTime _globalLogWindow = DateTime.UtcNow;

	private static readonly object _logRateLock = new object();

	public static byte[] getIcon(string hash, object[] list)
	{
		for (int i = 1; i < list.Length; i += 2)
		{
			if ((string)list[i] == hash)
			{
				return (byte[])list[i - 1];
			}
		}
		return null;
	}

	public static string Shuffle(string str)
	{
		char[] array = str.ToCharArray();
		Random random = new Random();
		int i = array.Length;
		while (i > 1)
		{
			i--;
			int num = random.Next(i + 1);
			char c = array[num];
			array[num] = array[i];
			array[i] = c;
		}
		return new string(array);
	}

	public static string GetPublicIpAsync()
	{
		try
		{
			using WebClient webClient = new WebClient();
			return webClient.DownloadString("http://icanhazip.com").Replace("\n", "");
		}
		catch
		{
		}
		return "127.0.0.1";
	}

	public static string GetIcon(string path)
	{
		try
		{
			string text = Path.GetTempFileName() + ".ico";
			using (FileStream fileStream = new FileStream(text, FileMode.Create))
			{
				IconExtractor.Extract1stIconTo(path, fileStream);
			}
			return text;
		}
		catch
		{
		}
		return "";
	}

	public static string GetChecksum(string file)
	{
		if (!File.Exists(file))
		{
			return string.Empty;
		}
		string fullPath = Path.GetFullPath(file);
		string cacheKey = fullPath + "|" + File.GetLastWriteTimeUtc(file).Ticks;
		return _checksumCache.GetOrAdd(cacheKey, delegate
		{
			using FileStream inputStream = File.OpenRead(fullPath);
			return BitConverter.ToString(new SHA256Managed().ComputeHash(inputStream)).Replace("-", string.Empty);
		});
	}

	public static void AppendLogs(string client, string message, Color color)
	{
		DateTime now = DateTime.UtcNow;
		lock (_logRateLock)
		{
			if ((now - _globalLogWindow).TotalSeconds >= 1.0)
			{
				_globalLogWindow = now;
				_globalLogCount = 0;
			}
			_globalLogCount++;
			if (_globalLogCount > 30)
			{
				return;
			}
		}
		if (!string.IsNullOrEmpty(client) && client != "Server" && client != "AntiSpam" && client != "AntiDDoS")
		{
			if (_logWindowPerClient.TryGetValue(client, out var clientWindow) && (now - clientWindow).TotalSeconds < 1.0)
			{
				if (_logCountPerClient.AddOrUpdate(client, 1, (string k, int v) => v + 1) > 5)
				{
					return;
				}
			}
			else
			{
				_logWindowPerClient[client] = now;
				_logCountPerClient[client] = 1;
			}
		}
		if (message != null && message.Length > 500)
		{
			message = message.Substring(0, 500) + "...";
		}
		DataGridViewRow Item = new DataGridViewRow();
		Item.DefaultCellStyle = new DataGridViewCellStyle
		{
			Alignment = DataGridViewContentAlignment.MiddleLeft,
			ForeColor = color,
			SelectionForeColor = Color.White,
			Font = new Font("Segoe UI", 11f, FontStyle.Regular, GraphicsUnit.Pixel),
			WrapMode = DataGridViewTriState.False
		};
		Item.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = client
		});
		Item.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = DateTime.Now.ToString("HH:mm:ss")
		});
		Item.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = message
		});
		Program.form.GridLogs.Invoke((MethodInvoker)delegate
		{
			Program.form.GridLogs.Rows.Insert(0, Item);
			if (Program.form.GridLogs.Rows.Count > 3000)
			{
				for (int i = 0; i < 500; i++)
				{
					if (Program.form.GridLogs.Rows.Count <= 2500)
					{
						break;
					}
					Program.form.GridLogs.Rows.RemoveAt(Program.form.GridLogs.Rows.Count - 1);
				}
			}
		});
	}

	public static string BytesToString(long byteCount)
	{
		string[] array = new string[7] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
		if (byteCount == 0L)
		{
			return "0" + array[0];
		}
		long num = Math.Abs(byteCount);
		int num2 = Convert.ToInt32(Math.Floor(Math.Log(num, 1024.0)));
		double num3 = Math.Round((double)num / Math.Pow(1024.0, num2), 1);
		return (double)Math.Sign(byteCount) * num3 + " " + array[num2];
	}

	public static Bitmap ByteArrayToBitmap(byte[] byteArray)
	{
		using MemoryStream memoryStream = new MemoryStream(byteArray);
		return new Bitmap(memoryStream);
	}
}
