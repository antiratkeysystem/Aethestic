using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Mutation2;

public class Add : iMutation
{
	public void Prepare(TypeDef type)
	{
	}

	public void Process(MethodDef method, ref int index)
	{
		int ldcI4Value = method.Body.Instructions[index].GetLdcI4Value();
		int num = Methods.Random.Next((int)((double)ldcI4Value / 1.5));
		method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4;
		method.Body.Instructions[index].Operand = ldcI4Value - num;
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldc_I4, num));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Add));
	}

	public bool Supported(Instruction instr)
	{
		return Utils.CheckArithmetic(instr);
	}
}
