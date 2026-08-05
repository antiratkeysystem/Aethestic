using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server.Helper;

namespace Server.Connectings;

public class Listner
{
	private static readonly ConcurrentDictionary<string, int> _connectionsPerSecond = new ConcurrentDictionary<string, int>();

	private static readonly ConcurrentDictionary<string, DateTime> _connectionWindowStart = new ConcurrentDictionary<string, DateTime>();

	private const int MaxNewConnectionsPerIPPerSecond = 20;

	private const int MaxTotalPendingConnections = 500;

	private static int _currentPendingConnections = 0;

	private const int MaxConcurrentConnections = 2000;

	private Socket Server { get; set; }

	public int port { get; set; }

	public void Stop()
	{
		Server?.Dispose();
		Methods.AppendLogs("Server", "Stop Listner: " + port, Color.Red);
	}

	public Listner(int port)
	{
		this.port = port;
		IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
		Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		Server.ReceiveBufferSize = 131072;
		Server.SendBufferSize = 131072;
		Server.Bind(localEP);
		Server.Listen(500);
		Server.BeginAccept(EndAccept, null);
		Methods.AppendLogs("Server", "Start Listner: " + port, Color.Green);
	}

	private void EndAccept(IAsyncResult ar)
	{
		try
		{
			Socket clientSocket = Server.EndAccept(ar);
			if (Interlocked.Increment(ref _currentPendingConnections) > 2000)
			{
				Interlocked.Decrement(ref _currentPendingConnections);
				try
				{
					clientSocket.Close();
					return;
				}
				catch
				{
					return;
				}
			}
			string ip = ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();
			if (!(ip == "127.0.0.1") && !(ip == "::1") && !ip.StartsWith("127."))
			{
				DateTime now = DateTime.UtcNow;
				if (_connectionWindowStart.TryGetValue(ip, out var windowStart) && (now - windowStart).TotalSeconds < 1.0)
				{
					if (_connectionsPerSecond.AddOrUpdate(ip, 1, (string k, int v) => v + 1) > 20)
					{
						Interlocked.Decrement(ref _currentPendingConnections);
						try
						{
							clientSocket.Close();
							return;
						}
						catch
						{
							return;
						}
					}
				}
				else
				{
					_connectionWindowStart[ip] = now;
					_connectionsPerSecond[ip] = 1;
				}
			}
			new Clients(clientSocket);
		}
		catch
		{
			Interlocked.Decrement(ref _currentPendingConnections);
		}
		try
		{
			Server.BeginAccept(EndAccept, null);
		}
		catch
		{
		}
	}

	public static void OnClientDisconnected()
	{
		Interlocked.Decrement(ref _currentPendingConnections);
	}
}
