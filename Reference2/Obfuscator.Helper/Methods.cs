using System;
using System.Linq;

namespace Obfuscator.Helper;

internal class Methods
{
	private const string Ascii = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

	private const string number = "1234567890";

	public static readonly Random Random = new Random();

	public static string GenerateString()
	{
		return GenerateString((byte)Random.Next(10, 26));
	}

	public static string GenerateString(byte length)
	{
		char c = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"[Random.Next("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".Length)];
		char[] value = (from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", length - 1)
			select s[Random.Next(s.Length)]).ToArray();
		return c + new string(value);
	}

	public static bool GenerateBool()
	{
		return GenerateBool(2);
	}

	public static bool GenerateBool(byte probability)
	{
		return Random.Next(probability) == 1;
	}
}
