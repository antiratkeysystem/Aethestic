using System;
using System.Text;

namespace Server.Helper.Sock5;

public class Socks5Request
{
	public byte Version { get; set; }

	public byte Command { get; set; }

	public byte Reserved { get; set; }

	public byte AddressType { get; set; }

	public string DestinationAddress { get; set; }

	public ushort DestinationPort { get; set; }

	public static Socks5Request Parse(byte[] data)
	{
		if (data.Length < 10)
		{
			throw new ArgumentException("Invalid data length");
		}
		Socks5Request socks5Request = new Socks5Request();
		socks5Request.Version = data[0];
		socks5Request.Command = data[1];
		socks5Request.Reserved = data[2];
		socks5Request.AddressType = data[3];
		if (socks5Request.AddressType != 1)
		{
			if (socks5Request.AddressType == 3)
			{
				_ = data[4];
			}
			else if (socks5Request.AddressType != 4)
			{
				throw new ArgumentException("Invalid address type");
			}
		}
		if (socks5Request.AddressType == 3)
		{
			socks5Request.DestinationAddress = Encoding.ASCII.GetString(data, 5, data[4]);
		}
		if (socks5Request.AddressType == 1)
		{
			socks5Request.DestinationAddress = data[4] + "." + data[5] + "." + data[6] + "." + data[7];
		}
		Array.Reverse(data, data.Length - 2, 2);
		socks5Request.DestinationPort = BitConverter.ToUInt16(data, data.Length - 2);
		return socks5Request;
	}

	public override string ToString()
	{
		return $"Version: {Version}, Command: {Command}, Reserved: {Reserved}, AddressType: {AddressType}, DestinationAddress: {DestinationAddress}, DestinationPort: {DestinationPort}";
	}
}
