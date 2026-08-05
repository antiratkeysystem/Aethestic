using System;
using System.Text;

namespace Obfuscator.Obfuscator.Strings.Runtime;

internal static class DecryptionHelper
{
	public static string Decrypt_Base64(string dataEnc)
	{
		return Encoding.UTF8.GetString(Convert.FromBase64String(dataEnc));
	}
}
