using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.ConstMelting;

internal class ConstMelting
{
	public static void Execute(ModuleDef moduleDef)
	{
		TypeDef[] array = moduleDef.Types.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			MethodDef[] array2 = array[i].Methods.ToArray();
			foreach (MethodDef methodDef in array2)
			{
				ReplaceStringLiterals(methodDef);
				ReplaceIntLiterals(methodDef);
			}
		}
	}

	private static void ReplaceStringLiterals(MethodDef methodDef)
	{
		if (!CanObfuscate(methodDef))
		{
			return;
		}
		foreach (Instruction instruction in methodDef.Body.Instructions)
		{
			if (instruction.OpCode == OpCodes.Ldstr)
			{
				MethodDef methodDef2 = new MethodDefUser(Methods.GenerateString(), MethodSig.CreateStatic(methodDef.DeclaringType.Module.CorLibTypes.String), MethodImplAttributes.IL, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
				{
					Body = new CilBody()
				};
				methodDef2.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, instruction.Operand.ToString()));
				methodDef2.Body.Instructions.Add(new Instruction(OpCodes.Ret));
				methodDef.DeclaringType.Methods.Add(methodDef2);
				instruction.OpCode = OpCodes.Call;
				instruction.Operand = methodDef2;
			}
		}
	}

	private static void ReplaceIntLiterals(MethodDef methodDef)
	{
		if (!CanObfuscate(methodDef))
		{
			return;
		}
		foreach (Instruction instruction in methodDef.Body.Instructions)
		{
			if (instruction.OpCode == OpCodes.Ldc_I4)
			{
				MethodDef methodDef2 = new MethodDefUser(Methods.GenerateString(), MethodSig.CreateStatic(methodDef.DeclaringType.Module.CorLibTypes.Int32), MethodImplAttributes.IL, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
				{
					Body = new CilBody()
				};
				methodDef2.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, instruction.GetLdcI4Value()));
				methodDef2.Body.Instructions.Add(new Instruction(OpCodes.Ret));
				methodDef.DeclaringType.Methods.Add(methodDef2);
				instruction.OpCode = OpCodes.Call;
				instruction.Operand = methodDef2;
			}
		}
	}

	public static bool CanObfuscate(MethodDef methodDef)
	{
		if (methodDef.HasBody && methodDef.Body.HasInstructions)
		{
			return !methodDef.DeclaringType.IsGlobalModuleType;
		}
		return false;
	}
}
