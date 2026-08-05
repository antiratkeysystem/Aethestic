using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Server.Helper;

public class DiscordWebhook
{
	public static class FormUpload
	{
		public class FileParameter
		{
			public byte[] File { get; set; }

			public string FileName { get; set; }

			public string ContentType { get; set; }

			public FileParameter(byte[] file)
				: this(file, null)
			{
			}

			public FileParameter(byte[] file, string filename)
				: this(file, filename, null)
			{
			}

			public FileParameter(byte[] file, string filename, string contenttype)
			{
				File = file;
				FileName = filename;
				ContentType = contenttype;
			}
		}

		private static readonly Encoding encoding = Encoding.UTF8;

		public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters)
		{
			string text = $"----------{Guid.NewGuid():N}";
			string contentType = "multipart/form-data; boundary=" + text;
			byte[] multipartFormData = GetMultipartFormData(postParameters, text);
			return PostForm(postUrl, userAgent, contentType, multipartFormData);
		}

		private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
		{
			if (!(WebRequest.Create(postUrl) is HttpWebRequest httpWebRequest))
			{
				throw new NullReferenceException("request is not a http request");
			}
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = contentType;
			httpWebRequest.UserAgent = userAgent;
			httpWebRequest.CookieContainer = new CookieContainer();
			httpWebRequest.ContentLength = formData.Length;
			using (Stream requestStream = httpWebRequest.GetRequestStream())
			{
				requestStream.Write(formData, 0, formData.Length);
				requestStream.Close();
			}
			return httpWebRequest.GetResponse() as HttpWebResponse;
		}

		private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
		{
			Stream stream = new MemoryStream();
			bool flag = false;
			foreach (KeyValuePair<string, object> keyValuePair in postParameters)
			{
				if (flag)
				{
					stream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));
				}
				flag = true;
				if (keyValuePair.Value is FileParameter)
				{
					FileParameter fileParameter = (FileParameter)keyValuePair.Value;
					string s = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n", boundary, keyValuePair.Key, fileParameter.FileName ?? keyValuePair.Key, fileParameter.ContentType ?? "application/octet-stream");
					stream.Write(encoding.GetBytes(s), 0, encoding.GetByteCount(s));
					stream.Write(fileParameter.File, 0, fileParameter.File.Length);
				}
				else
				{
					string s2 = $"--{boundary}\r\nContent-Disposition: form-data; name=\"{keyValuePair.Key}\"\r\n\r\n{keyValuePair.Value}";
					stream.Write(encoding.GetBytes(s2), 0, encoding.GetByteCount(s2));
				}
			}
			string s3 = "\r\n--" + boundary + "--\r\n";
			stream.Write(encoding.GetBytes(s3), 0, encoding.GetByteCount(s3));
			stream.Position = 0L;
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			stream.Close();
			return array;
		}
	}

	private static string defaultUserAgent = "";

	private static string defaultAvatar = "";

	public static string Send(string mssgBody, string userName, string webhook)
	{
		try
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			HttpWebResponse httpWebResponse = FormUpload.MultipartFormDataPost(webhook, defaultUserAgent, new Dictionary<string, object>
			{
				{ "username", userName },
				{ "content", mssgBody },
				{ "avatar_url", defaultAvatar }
			});
			string result = new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
			httpWebResponse.Close();
			return result;
		}
		catch (WebException)
		{
			return "";
		}
		catch (Exception)
		{
			return "";
		}
	}
}
