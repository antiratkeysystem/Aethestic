using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Leb128;
using Server.Connectings;
using Server.Connectings.Events;

namespace Server.Helper.Sock5;

public class Client
{
	public delegate void RHandler(Client client);

	public delegate void BHandler(Client client, long Sent, long Recive);

	public const int BufferSize = 12000;

	public const int TimeOut = 5000;

	private byte[] Buffer;

	private Socket socket;

	public Server server;

	public Clients ClientReverse;

	private Timer TimeOutTimer;

	private DateTime lastPing;

	private bool Auth;

	public object Tag;

	public bool ItsConnect { get; private set; }

	public int ConnectionId => socket.Handle.ToInt32();

	public string IPAddress { get; private set; }

	public string Port { get; private set; }

	public long Sents { get; private set; }

	public long Recives { get; private set; }

	public event BHandler ResponceEvent;

	public event RHandler DisconnectEvent;

	public event RHandler ConnectEvent;

	public Client(Socket socket, Server server)
	{
		this.socket = socket;
		this.server = server;
		ItsConnect = true;
		Auth = false;
		Sents = 0L;
		Recives = 0L;
		Last();
		TimeOutTimer = new Timer(Check, null, 1, 5000);
		server.ClientReverse[Randomizer.random.Next(server.ClientReverse.Count)].Send(new object[3] { "Accept", ConnectionId, server.port });
	}

	public void Accept(Clients ClientReverse)
	{
		this.ClientReverse = ClientReverse;
		this.ClientReverse.eventDisconnect += Disconnect;
		Buffer = new byte[12000];
		socket.BeginReceive(Buffer, 0, 12000, SocketFlags.None, ReadAsyncConnect, null);
	}

	public void Disconnect(object sender, EventDisconnect eventDisconnect)
	{
		if (ItsConnect)
		{
			ItsConnect = false;
			TimeOutTimer?.Dispose();
			this.socket?.Dispose();
			lock (server.clients)
			{
				server.clients.Remove(this);
			}
			if (this.DisconnectEvent != null)
			{
				this.DisconnectEvent(this);
			}
		}
	}

	public void Disconnect()
	{
		if (ItsConnect)
		{
			ItsConnect = false;
			TimeOutTimer?.Dispose();
			this.socket?.Dispose();
			ClientReverse?.Disconnect();
			lock (server.clients)
			{
				server.clients.Remove(this);
			}
			if (this.DisconnectEvent != null)
			{
				this.DisconnectEvent(this);
			}
		}
	}

	public void Send(byte[] Data)
	{
		if (ItsConnect)
		{
			Last();
			socket.Send(Data);
			Recives += Data.Length;
			if (this.ResponceEvent != null)
			{
				this.ResponceEvent(this, 0L, Data.Length);
			}
		}
	}

	public void ReadAsyncConnect(IAsyncResult ar)
	{
		if (!ItsConnect || !ClientReverse.itsConnect)
		{
			return;
		}
		try
		{
			int num = socket.EndReceive(ar);
			if (num > 0)
			{
				byte[] array = new byte[num];
				Array.Copy(Buffer, array, num);
				Last();
				if (array[0] == 5)
				{
					if (Auth)
					{
						if (array.Length >= 6)
						{
							if (array[1] == 1)
							{
								if (array[3] == 4)
								{
									Send(new byte[2] { 5, 7 });
									Disconnect();
									return;
								}
								Socks5Request socks5Request = Socks5Request.Parse(array);
								byte[] array2 = LEB128.Write(new object[3]
								{
									"Connect",
									socks5Request.DestinationAddress,
									(int)socks5Request.DestinationPort
								});
								IPAddress = socks5Request.DestinationAddress;
								Port = socks5Request.DestinationPort.ToString();
								ClientReverse.Send(array2);
								Sents += array2.Length;
								if (this.ResponceEvent != null)
								{
									this.ResponceEvent(this, array2.Length, 0L);
								}
							}
							else
							{
								Send(new byte[2] { 5, 7 });
								Disconnect();
							}
						}
						else
						{
							socket.BeginReceive(Buffer, 0, 12000, SocketFlags.None, ReadAsyncConnect, null);
						}
					}
					else if (Auth)
					{
						Send(new byte[2] { 5, 255 });
						Disconnect();
					}
					else
					{
						Auth = true;
						Send(new byte[2] { 5, 0 });
						socket.BeginReceive(Buffer, 0, 12000, SocketFlags.None, ReadAsyncConnect, null);
					}
				}
				else
				{
					Send(new byte[2] { 5, 7 });
					Disconnect();
				}
			}
			else
			{
				Disconnect();
			}
		}
		catch
		{
			Disconnect();
		}
	}

	public void ReadAsyncData(IAsyncResult ar)
	{
		if (!ItsConnect || !ClientReverse.itsConnect)
		{
			return;
		}
		try
		{
			int num = socket.EndReceive(ar);
			if (num > 0)
			{
				byte[] array = new byte[num];
				Array.Copy(Buffer, array, num);
				Last();
				ClientReverse.Send(new object[2] { "Data", array });
				Sents += array.Length;
				if (this.ResponceEvent != null)
				{
					this.ResponceEvent(this, array.Length, 0L);
				}
				socket.BeginReceive(Buffer, 0, 12000, SocketFlags.None, ReadAsyncData, null);
			}
			else
			{
				Disconnect();
			}
		}
		catch
		{
			Disconnect();
		}
	}

	public void HandleCommandResponse(object[] response)
	{
		if (!ItsConnect || !ClientReverse.itsConnect)
		{
			return;
		}
		byte b = 2;
		if (Convert.ToBoolean(response[b]))
		{
			try
			{
				List<byte> list = new List<byte>();
				list.Add(5);
				list.Add(0);
				list.Add(0);
				list.Add(1);
				list.AddRange((byte[])response[b + 1]);
				list.Add((byte)Math.Floor((decimal)(int)response[b + 2] / 256m));
				list.Add((byte)((int)response[b + 2] % 256));
				Send(list.ToArray());
				Last();
				socket.BeginReceive(Buffer, 0, 12000, SocketFlags.None, ReadAsyncData, null);
				if (this.ConnectEvent != null)
				{
					this.ConnectEvent(this);
				}
				return;
			}
			catch (Exception)
			{
				Send(new byte[2] { 5, 1 });
				Disconnect();
				return;
			}
		}
		Send(new byte[2] { 5, 4 });
		Disconnect();
	}

	private double DiffSeconds(DateTime startTime, DateTime endTime)
	{
		return Math.Abs(new TimeSpan(endTime.Ticks - startTime.Ticks).TotalSeconds);
	}

	private void Check(object obj)
	{
		if (DiffSeconds(lastPing, DateTime.Now) > 30.0)
		{
			Disconnect();
		}
	}

	public void Last()
	{
		lastPing = DateTime.Now;
	}
}
