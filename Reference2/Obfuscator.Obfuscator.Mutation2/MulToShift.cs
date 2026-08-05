using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.Mutation2;

public class MulToShift : iMutation
{
	public void Prepare(TypeDef type)
	{
	}

	public void Process(MethodDef method, ref int index)
	{
		if (!method.Body.Instructions[index - 1].IsLdcI4() || !method.Body.Instructions[index - 2].IsLdcI4())
		{
			return;
		}
		int ldcI4Value = method.Body.Instructions[index - 2].GetLdcI4Value();
		int ldcI4Value2 = method.Body.Instructions[index - 1].GetLdcI4Value();
		if (ldcI4Value2 < 3)
		{
			return;
		}
		Local local = new Local(method.Module.CorLibTypes.Int32);
		method.Body.Variables.Add(local);
		method.Body.Instructions.Insert(0, new Instruction(OpCodes.Stloc, local));
		method.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldc_I4, ldcI4Value));
		index += 2;
		method.Body.Instructions[index - 2].OpCode = OpCodes.Ldloc;
		method.Body.Instructions[index - 2].Operand = local;
		method.Body.Instructions[index - 1].OpCode = OpCodes.Nop;
		method.Body.Instructions[index].OpCode = OpCodes.Nop;
		int num = 0;
		for (int i = ldcI4Value2; i > 0; i >>= 1)
		{
			if ((i & 1) == 1 && num != 0)
			{
				method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldloc, local));
				method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldc_I4, num));
				method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Shl));
				method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Add));
			}
			num++;
		}
		if ((ldcI4Value2 & 1) == 0)
		{
			method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldloc, local));
			method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Sub));
		}
	}

	public bool Supported(Instruction instr)
	{
		return instr.OpCode == OpCodes.Mul;
	}
}
