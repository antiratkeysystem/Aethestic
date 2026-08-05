using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Server.Helper;

public static class FlagHelper
{
	private static readonly object CacheLock = new object();

	private static readonly Dictionary<string, Bitmap> Cache = new Dictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);

	private static Bitmap _unknownBitmap;

	private static readonly Dictionary<string, string> CountryNameToCode = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "Russian Federation", "RU" },
		{ "Russia", "RU" },
		{ "Belarus", "BY" },
		{ "Kazakhstan", "KZ" },
		{ "Kazakstan", "KZ" },
		{ "Isle of Man", "IM" },
		{ "United States", "US" },
		{ "United States of America", "US" },
		{ "USA", "US" },
		{ "Germany", "DE" },
		{ "United Kingdom", "GB" },
		{ "UK", "GB" },
		{ "France", "FR" },
		{ "Ukraine", "UA" },
		{ "Poland", "PL" },
		{ "Italy", "IT" },
		{ "Spain", "ES" },
		{ "Netherlands", "NL" },
		{ "Turkey", "TR" },
		{ "China", "CN" },
		{ "Japan", "JP" },
		{ "India", "IN" },
		{ "Brazil", "BR" },
		{ "Canada", "CA" },
		{ "Australia", "AU" },
		{ "Indonesia", "ID" },
		{ "Mexico", "MX" },
		{ "South Korea", "KR" },
		{ "Korea, Republic of", "KR" },
		{ "Thailand", "TH" },
		{ "Vietnam", "VN" },
		{ "Viet Nam", "VN" },
		{ "Egypt", "EG" },
		{ "South Africa", "ZA" },
		{ "Nigeria", "NG" },
		{ "Kenya", "KE" },
		{ "Morocco", "MA" },
		{ "Algeria", "DZ" },
		{ "Tunisia", "TN" },
		{ "Israel", "IL" },
		{ "Saudi Arabia", "SA" },
		{ "United Arab Emirates", "AE" },
		{ "UAE", "AE" },
		{ "Iran", "IR" },
		{ "Iran, Islamic Republic of", "IR" },
		{ "Iraq", "IQ" },
		{ "Pakistan", "PK" },
		{ "Bangladesh", "BD" },
		{ "Philippines", "PH" },
		{ "Malaysia", "MY" },
		{ "Singapore", "SG" },
		{ "Hong Kong", "HK" },
		{ "Taiwan", "TW" },
		{ "Argentina", "AR" },
		{ "Chile", "CL" },
		{ "Colombia", "CO" },
		{ "Peru", "PE" },
		{ "Venezuela", "VE" },
		{ "Romania", "RO" },
		{ "Czech Republic", "CZ" },
		{ "Czechia", "CZ" },
		{ "Hungary", "HU" },
		{ "Greece", "GR" },
		{ "Portugal", "PT" },
		{ "Sweden", "SE" },
		{ "Norway", "NO" },
		{ "Denmark", "DK" },
		{ "Finland", "FI" },
		{ "Austria", "AT" },
		{ "Switzerland", "CH" },
		{ "Belgium", "BE" },
		{ "Ireland", "IE" },
		{ "New Zealand", "NZ" },
		{ "Afghanistan", "AF" },
		{ "Albania", "AL" },
		{ "Armenia", "AM" },
		{ "Azerbaijan", "AZ" },
		{ "Georgia", "GE" },
		{ "Moldova", "MD" },
		{ "Moldova, Republic of", "MD" },
		{ "Lithuania", "LT" },
		{ "Latvia", "LV" },
		{ "Estonia", "EE" },
		{ "Slovakia", "SK" },
		{ "Slovenia", "SI" },
		{ "Croatia", "HR" },
		{ "Serbia", "RS" },
		{ "Bulgaria", "BG" },
		{ "North Macedonia", "MK" },
		{ "Macedonia", "MK" },
		{ "Bosnia and Herzegovina", "BA" },
		{ "Montenegro", "ME" },
		{ "Kosovo", "XK" },
		{ "Cyprus", "CY" },
		{ "Malta", "MT" },
		{ "Luxembourg", "LU" },
		{ "Iceland", "IS" },
		{ "Uzbekistan", "UZ" },
		{ "Turkmenistan", "TM" },
		{ "Tajikistan", "TJ" },
		{ "Kyrgyzstan", "KG" },
		{ "Lebanon", "LB" },
		{ "Jordan", "JO" },
		{ "Syria", "SY" },
		{ "Syrian Arab Republic", "SY" },
		{ "Yemen", "YE" },
		{ "Kuwait", "KW" },
		{ "Qatar", "QA" },
		{ "Bahrain", "BH" },
		{ "Oman", "OM" },
		{ "Sri Lanka", "LK" },
		{ "Nepal", "NP" },
		{ "Myanmar", "MM" },
		{ "Myanmar (Burma)", "MM" },
		{ "Cambodia", "KH" },
		{ "Laos", "LA" },
		{ "Lao People's Democratic Republic", "LA" },
		{ "Mongolia", "MN" },
		{ "North Korea", "KP" },
		{ "Korea, Democratic People's Republic of", "KP" },
		{ "Ecuador", "EC" },
		{ "Bolivia", "BO" },
		{ "Paraguay", "PY" },
		{ "Uruguay", "UY" },
		{ "Costa Rica", "CR" },
		{ "Panama", "PA" },
		{ "Dominican Republic", "DO" },
		{ "Cuba", "CU" },
		{ "Guatemala", "GT" },
		{ "Honduras", "HN" },
		{ "El Salvador", "SV" },
		{ "Nicaragua", "NI" },
		{ "Puerto Rico", "PR" },
		{ "Jamaica", "JM" },
		{ "Trinidad and Tobago", "TT" },
		{ "Ghana", "GH" },
		{ "Ethiopia", "ET" },
		{ "Tanzania", "TZ" },
		{ "Tanzania, United Republic of", "TZ" },
		{ "Uganda", "UG" },
		{ "Senegal", "SN" },
		{ "Ivory Coast", "CI" },
		{ "Côte d'Ivoire", "CI" },
		{ "Cote d'Ivoire", "CI" },
		{ "Cameroon", "CM" },
		{ "Zimbabwe", "ZW" },
		{ "Zambia", "ZM" },
		{ "Botswana", "BW" },
		{ "Mozambique", "MZ" },
		{ "Angola", "AO" },
		{ "Libya", "LY" },
		{ "Sudan", "SD" }
	};

	public static int FlagWidthPixels { get; set; } = 26;

	public static int FlagHeightPixels { get; set; } = 12;

	private static string GetFlagsFolder()
	{
		string baseDir = AppDomain.CurrentDomain.BaseDirectory;
		if (string.IsNullOrEmpty(baseDir))
		{
			baseDir = Application.StartupPath ?? ".";
		}
		return Path.Combine(baseDir, "Flags");
	}

	public static Image GetFlagImage(string countryCode, string countryName = null)
	{
		string code = ResolveCountryCode(countryCode, countryName);
		int w = Math.Max(8, Math.Min(64, FlagWidthPixels));
		int h = Math.Max(8, Math.Min(64, FlagHeightPixels));
		if (string.IsNullOrEmpty(code))
		{
			return GetUnknownBitmap(w, h);
		}
		string cacheKey = code.ToUpperInvariant() + ":" + w + "x" + h;
		lock (CacheLock)
		{
			if (Cache.TryGetValue(cacheKey, out var cached))
			{
				return cached;
			}
		}
		Bitmap result = LoadFromLocal(code, w, h);
		if (result != null)
		{
			lock (CacheLock)
			{
				Cache[cacheKey] = result;
				return result;
			}
		}
		result = LoadFromApi(code, w, h);
		if (result != null)
		{
			lock (CacheLock)
			{
				Cache[cacheKey] = result;
				return result;
			}
		}
		return GetUnknownBitmap(w, h);
	}

	private static Bitmap LoadFromLocal(string code, int width, int height)
	{
		string dir = GetFlagsFolder();
		string codeLower = code.ToLowerInvariant();
		string[] array = new string[4] { ".png", ".jpg", ".jpeg", ".bmp" };
		foreach (string ext in array)
		{
			string path = Path.Combine(dir, codeLower + ext);
			try
			{
				if (!File.Exists(path))
				{
					continue;
				}
				using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
				using Image img = Image.FromStream(fs);
				return ResizeTo(img, width, height);
			}
			catch
			{
			}
		}
		return null;
	}

	private static Bitmap LoadFromApi(string code, int width, int height)
	{
		string codeLower = code.ToLowerInvariant();
		string[] array = new string[3]
		{
			"https://flagcdn.com/w80/" + codeLower + ".png",
			"https://countryflagsapi.com/png/" + codeLower,
			"https://raw.githubusercontent.com/EmmanuelSty/Country-Flags/main/flags/" + codeLower + ".png"
		};
		for (int i = 0; i < array.Length; i++)
		{
			byte[] data = DownloadBytes(array[i], 5000);
			if (data == null || data.Length < 8 || !IsImageData(data))
			{
				continue;
			}
			try
			{
				using MemoryStream ms = new MemoryStream(data);
				using Image img = Image.FromStream(ms);
				return ResizeTo(img, width, height);
			}
			catch
			{
			}
		}
		return null;
	}

	private static bool IsImageData(byte[] data)
	{
		if (data.Length >= 8 && data[0] == 137 && data[1] == 80 && data[2] == 78 && data[3] == 71)
		{
			return true;
		}
		if (data.Length >= 3 && data[0] == byte.MaxValue && data[1] == 216 && data[2] == byte.MaxValue)
		{
			return true;
		}
		return false;
	}

	private static byte[] DownloadBytes(string url, int timeoutMs)
	{
		try
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(url);
			obj.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
			obj.Timeout = timeoutMs;
			obj.ReadWriteTimeout = timeoutMs;
			obj.Proxy = null;
			obj.AllowAutoRedirect = true;
			using HttpWebResponse resp = (HttpWebResponse)obj.GetResponse();
			using Stream stream = resp.GetResponseStream();
			if (stream == null)
			{
				return null;
			}
			using MemoryStream ms = new MemoryStream();
			stream.CopyTo(ms);
			return ms.ToArray();
		}
		catch
		{
			return null;
		}
	}

	private static Bitmap ResizeTo(Image img, int width, int height)
	{
		Bitmap bmp = new Bitmap(width, height);
		using Graphics g = Graphics.FromImage(bmp);
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		g.DrawImage(img, 0, 0, width, height);
		return bmp;
	}

	public static Bitmap GetUnknownFlagImageSafe()
	{
		int width = Math.Max(8, Math.Min(64, FlagWidthPixels));
		int h = Math.Max(8, Math.Min(64, FlagHeightPixels));
		return GetUnknownBitmap(width, h);
	}

	private static Bitmap GetUnknownBitmap(int width, int height)
	{
		if (_unknownBitmap != null && _unknownBitmap.Width == width && _unknownBitmap.Height == height)
		{
			return _unknownBitmap;
		}
		lock (CacheLock)
		{
			if (_unknownBitmap != null && _unknownBitmap.Width == width && _unknownBitmap.Height == height)
			{
				return _unknownBitmap;
			}
			try
			{
				Bitmap bmp = new Bitmap(width, height);
				using (Graphics g = Graphics.FromImage(bmp))
				{
					g.Clear(Color.FromArgb(140, 140, 140));
					using Font font = new Font("Tahoma", Math.Max(6, Math.Min(width, height) / 2), FontStyle.Bold);
					using SolidBrush brush = new SolidBrush(Color.White);
					StringFormat sf = new StringFormat
					{
						Alignment = StringAlignment.Center,
						LineAlignment = StringAlignment.Center
					};
					g.DrawString("?", font, brush, new RectangleF(0f, 0f, width, height), sf);
				}
				_unknownBitmap = bmp;
				return bmp;
			}
			catch
			{
				Bitmap bmp2 = new Bitmap(width, height);
				using (Graphics g2 = Graphics.FromImage(bmp2))
				{
					g2.Clear(Color.FromArgb(140, 140, 140));
				}
				return bmp2;
			}
		}
	}

	public static string ResolveCountryCode(string countryCode, string countryName = null)
	{
		if (!string.IsNullOrWhiteSpace(countryCode))
		{
			string s = countryCode.Trim();
			if (s.Length == 2 && char.IsLetter(s[0]) && char.IsLetter(s[1]))
			{
				return s.ToUpperInvariant();
			}
		}
		if (!string.IsNullOrWhiteSpace(countryName))
		{
			string code = GetCodeFromName(countryName.Trim());
			if (!string.IsNullOrEmpty(code))
			{
				return code;
			}
		}
		return null;
	}

	private static string GetCodeFromName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return null;
		}
		if (string.Equals(name, "LocalHost", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "Unknown", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}
		if (CountryNameToCode.TryGetValue(name, out var code))
		{
			return code;
		}
		foreach (KeyValuePair<string, string> kv in CountryNameToCode)
		{
			if (string.Equals(kv.Key, name, StringComparison.OrdinalIgnoreCase))
			{
				return kv.Value;
			}
			if (name.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return kv.Value;
			}
		}
		return null;
	}
}
