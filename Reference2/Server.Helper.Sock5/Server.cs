using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Server.Connectings;
using Server.Connectings.Events;

namespace Server.Helper.Sock5;

public class Server
{
	public delegate void SHandler(Client client);

	public delegate void AHandler(Server server);

	public delegate void BHandler(Client client, long Sent, long Recive);

	public List<Clients> ClientReverse = new List<Clients>();

	public List<Clients> ClientReverses = new List<Clients>();

	public List<Client> clients = new List<Client>();

	public Socket server;

	public int port;

	public bool fullclose;

	public object Tag;

	public bool Stoped;

	public bool Disconnected;

	public long Recives;

	public long Sents;

	public event BHandler ResponceEvent;

	public event SHandler DisconnectEvent;

	public event SHandler ConnectEvent;

	public event AHandler StopedServer;

	public event AHandler DisconnectClientEvent;

	public event AHandler StartedServer;

	public event AHandler ResponceServerEvent;

	public Server(List<Clients> ClientReverse)
	{
		Stoped = true;
		this.ClientReverse = ClientReverse;
	}

	public Server(Clients ClientReverse)
	{
		Stoped = true;
		Disconnected = false;
		this.ClientReverse.Add(ClientReverse);
		ClientReverse.eventDisconnect += Stop;
	}

	public void Responce(Client client, long Sent, long Recive)
	{
		Recives += Recive;
		Sents += Sent;
		if (this.ResponceServerEvent != null)
		{
			this.ResponceServerEvent(this);
		}
	}

	public void Start(int port)
	{
		if (Stoped)
		{
			Stoped = false;
			this.port = port;
			IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
			server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			server.Bind(localEP);
			server.Listen(2500);
			server.BeginAccept(EndAccept, null);
			if (this.StartedServer != null)
			{
				this.StartedServer(this);
			}
		}
	}

	public void Stop(object sender, EventDisconnect disconnect)
	{
		Stop();
		if (!Disconnected)
		{
			Disconnected = true;
			if (this.DisconnectClientEvent != null)
			{
				this.DisconnectClientEvent(this);
			}
		}
		lock (ClientReverse)
		{
			ClientReverse.Remove(disconnect.clients);
		}
	}

	public void Stop()
	{
		if (!Stoped)
		{
			Stoped = true;
			if (this.StopedServer != null)
			{
				this.StopedServer(this);
			}
			server?.Dispose();
			Client[] array = clients.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Disconnect();
			}
			Clients[] array2 = ClientReverses.ToArray();
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].Disconnect();
			}
		}
	}

	public Client Search(int connectid)
	{
		Client[] array = clients.ToArray();
		foreach (Client client in array)
		{
			if (client.ConnectionId == connectid)
			{
				return client;
			}
		}
		return null;
	}

	private void EndAccept(IAsyncResult ar)
	{
		try
		{
			Client client = new Client(server.EndAccept(ar), this);
			if (this.ResponceEvent != null)
			{
				client.ResponceEvent += delegate(Client cl, long ls, long rs)
				{
					this.ResponceEvent(cl, ls, rs);
				};
			}
			if (this.DisconnectEvent != null)
			{
				client.DisconnectEvent += delegate(Client cl)
				{
					this.DisconnectEvent(cl);
				};
			}
			if (this.ConnectEvent != null)
			{
				client.ConnectEvent += delegate(Client cl)
				{
					this.ConnectEvent(cl);
				};
			}
			client.ResponceEvent += Responce;
			lock (clients)
			{
				clients.Add(client);
			}
		}
		catch (Exception)
		{
		}
		try
		{
			server.BeginAccept(EndAccept, null);
		}
		catch
		{
		}
	}
}
