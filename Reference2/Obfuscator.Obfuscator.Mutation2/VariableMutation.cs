using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.Mutation2;

public class VariableMutation : iMutation
{
	public void Prepare(TypeDef type)
	{
	}

	public void Process(MethodDef method, ref int index)
	{
		int ldcI4Value = method.Body.Instructions[index].GetLdcI4Value();
		Local local = new Local(method.Module.CorLibTypes.Int32);
		method.Body.Variables.Add(local);
		method.Body.Instructions.Insert(0, new Instruction(OpCodes.Stloc, local));
		method.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldc_I4, ldcI4Value));
		index += 2;
		method.Body.Instructions[index] = new Instruction(OpCodes.Ldloc, local);
	}

	public bool Supported(Instruction instr)
	{
		return Utils.CheckArithmetic(instr);
	}
}
