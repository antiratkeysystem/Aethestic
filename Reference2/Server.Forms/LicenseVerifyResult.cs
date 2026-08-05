using System;

namespace Server.Forms;

internal class LicenseVerifyResult
{
	public bool Success { get; set; }

	public string ErrorMessage { get; set; }

	public int DaysLeft { get; set; }

	public DateTime ExpiresAt { get; set; }
}
