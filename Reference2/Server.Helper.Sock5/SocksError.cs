namespace Server.Helper.Sock5;

public enum SocksError
{
	Granted = 0,
	Failure = 1,
	NotAllowed = 2,
	Unreachable = 3,
	HostUnreachable = 4,
	Refused = 5,
	Expired = 6,
	NotSupported = 7,
	AddressNotSupported = 8,
	LoginRequired = 144,
	Fail = 255
}
