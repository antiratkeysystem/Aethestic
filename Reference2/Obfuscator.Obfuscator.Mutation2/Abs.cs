using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.Mutation2;

public class Abs : iMutation
{
	public void Prepare(TypeDef type)
	{
	}

	public void Process(MethodDef method, ref int index)
	{
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Call, method.Module.Import(typeof(Math).GetMethod("Abs", new Type[1] { typeof(int) }))));
	}

	public bool Supported(Instruction instr)
	{
		return Utils.CheckArithmetic(instr);
	}
}
