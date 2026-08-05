using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Mutation2;

public class StringLen : iMutation
{
	public void Prepare(TypeDef type)
	{
	}

	public void Process(MethodDef method, ref int index)
	{
		if (method.DeclaringType == method.Module.GlobalType)
		{
			index--;
			return;
		}
		int ldcI4Value = method.Body.Instructions[index].GetLdcI4Value();
		int num = Methods.Random.Next(4, 15);
		string operand = Methods.GenerateString((byte)num);
		method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4;
		method.Body.Instructions[index].Operand = ldcI4Value - num;
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldstr, operand));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Call, method.Module.Import(typeof(string).GetMethod("get_Length"))));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Add));
	}

	public bool Supported(Instruction instr)
	{
		return Utils.CheckArithmetic(instr);
	}
}
