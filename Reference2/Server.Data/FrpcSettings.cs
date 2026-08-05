namespace Server.Data;

public class FrpcSettings
{
	public string ServerAddr { get; set; } = "";

	public string ServerPort { get; set; } = "7000";

	public string Token { get; set; } = "";

	public string LocalPort { get; set; } = "";

	public string RemotePort { get; set; } = "";

	public string Protocol { get; set; } = "tcp";
}
