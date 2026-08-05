using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Server.Helper;

public class DiscordRPC
{
	private static bool _isRunning = false;

	private static Thread _rpcThread = null;

	private static NamedPipeClientStream _currentPipe = null;

	private static readonly object _lock = new object();

	private const string APPLICATION_ID = "1475131145053278440";

	private const string DESCRIPTION = "Coded by: @liberiumSeller";

	private const string VERSION = "3.1 [RECODE]";

	private static string _gifUrl = "https://i.imgur.com/gDKF63C.gif";

	public static void Initialize()
	{
		lock (_lock)
		{
			if (!_isRunning)
			{
				_isRunning = true;
				_rpcThread = new Thread(RunRPC)
				{
					IsBackground = true
				};
				_rpcThread.Start();
			}
		}
	}

	public static void Shutdown()
	{
		lock (_lock)
		{
			if (!_isRunning)
			{
				return;
			}
			_isRunning = false;
			if (_currentPipe != null && _currentPipe.IsConnected)
			{
				try
				{
					_currentPipe.Close();
				}
				catch
				{
				}
				_currentPipe = null;
			}
			_rpcThread = null;
		}
	}

	private static void RunRPC()
	{
		while (_isRunning)
		{
			try
			{
				if (ConnectToDiscord())
				{
					UpdatePresence();
				}
				Thread.Sleep(15000);
			}
			catch
			{
				Thread.Sleep(5000);
			}
		}
	}

	private static bool ConnectToDiscord()
	{
		lock (_lock)
		{
			if (_currentPipe != null && _currentPipe.IsConnected)
			{
				return true;
			}
			if (_currentPipe != null)
			{
				try
				{
					_currentPipe.Close();
				}
				catch
				{
				}
				_currentPipe = null;
			}
			for (int i = 0; i < 10; i++)
			{
				try
				{
					NamedPipeClientStream pipe = new NamedPipeClientStream(".", $"discord-ipc-{i}", PipeDirection.InOut);
					pipe.Connect(1000);
					pipe.ReadMode = PipeTransmissionMode.Byte;
					string authJson = JsonConvert.SerializeObject(new
					{
						v = 1,
						client_id = "1475131145053278440"
					});
					SendFrame(pipe, 0u, authJson);
					byte[] header = new byte[8];
					if (pipe.Read(header, 0, header.Length) != header.Length)
					{
						throw new IOException("Incomplete RPC header");
					}
					uint length = BitConverter.ToUInt32(header, 4);
					if (length != 0)
					{
						byte[] payload = new byte[length];
						int chunk;
						for (int offset = 0; offset < length; offset += chunk)
						{
							chunk = pipe.Read(payload, offset, (int)length - offset);
							if (chunk <= 0)
							{
								throw new IOException("Failed to read RPC payload");
							}
						}
					}
					_currentPipe = pipe;
					return true;
				}
				catch (TimeoutException)
				{
				}
				catch
				{
				}
			}
			return false;
		}
	}

	private static void UpdatePresence()
	{
		lock (_lock)
		{
			if (_currentPipe == null || !_currentPipe.IsConnected)
			{
				return;
			}
			try
			{
				string image = _gifUrl;
				var activity = new
				{
					details = "Coded by: @liberiumSeller",
					state = "Version: 3.1 [RECODE]",
					timestamps = new
					{
						start = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
					},
					assets = new
					{
						large_image = image,
						large_text = "LiberiumRAT Server"
					}
				};
				string json = JsonConvert.SerializeObject(new
				{
					cmd = "SET_ACTIVITY",
					args = new
					{
						pid = Process.GetCurrentProcess().Id,
						activity = activity
					},
					nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
				});
				SendFrame(_currentPipe, 1u, json);
			}
			catch
			{
				if (_currentPipe != null)
				{
					try
					{
						_currentPipe.Close();
					}
					catch
					{
					}
					_currentPipe = null;
				}
			}
		}
	}

	private static void SendFrame(NamedPipeClientStream pipe, uint opcode, string json)
	{
		byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
		byte[] opcodeBytes = BitConverter.GetBytes(opcode);
		byte[] lengthBytes = BitConverter.GetBytes((uint)jsonBytes.Length);
		if (!BitConverter.IsLittleEndian)
		{
			Array.Reverse(opcodeBytes);
			Array.Reverse(lengthBytes);
		}
		pipe.Write(opcodeBytes, 0, 4);
		pipe.Write(lengthBytes, 0, 4);
		pipe.Write(jsonBytes, 0, jsonBytes.Length);
		pipe.Flush();
	}
}
