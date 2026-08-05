using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.CtrlFlow;

public class Block
{
	public List<Instruction> Instructions { get; set; }

	public int Number { get; set; }

	public int SubRand { get; set; }

	public int PlusRand { get; set; }

	public Block()
	{
		Instructions = new List<Instruction>();
	}
}
