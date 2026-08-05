using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Obfuscator.Obfuscator.Anti.Runtime;

internal class EofAntiTamper
{
	private static void Initializer()
	{
		string location = Assembly.GetExecutingAssembly().Location;
		Stream baseStream = new StreamReader(location).BaseStream;
		BinaryReader binaryReader = new BinaryReader(baseStream);
		string text = BitConverter.ToString(SHA256.Create().ComputeHash(binaryReader.ReadBytes(File.ReadAllBytes(location).Length - 32)));
		baseStream.Seek(-32L, SeekOrigin.End);
		string b = BitConverter.ToString(binaryReader.ReadBytes(32));
		if (text != b)
		{
			Process.Start(new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del \"" + Assembly.GetExecutingAssembly().Location + "\"")
			{
				WindowStyle = ProcessWindowStyle.Hidden
			})?.Dispose();
			Process.GetCurrentProcess().Kill();
		}
	}
}
