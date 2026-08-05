namespace Server.Helper.Bulider;

internal class EncryptString
{
	public string enc = "%enc%";

	public string dec = "%dec%";

	public string Encrypt(string text)
	{
		string text2 = "";
		foreach (char c in text)
		{
			for (int j = 0; j < enc.Length; j++)
			{
				if (enc[j] == c)
				{
					text2 += dec[j];
					break;
				}
			}
		}
		return text2;
	}
}
