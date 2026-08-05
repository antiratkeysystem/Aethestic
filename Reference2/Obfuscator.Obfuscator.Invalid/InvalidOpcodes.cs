using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.Invalid;

internal class InvalidOpcodes
{
	public static void Execute(ModuleDef module)
	{
		foreach (TypeDef type in module.GetTypes())
		{
			foreach (MethodDef methodDef in type.Methods)
			{
				if (methodDef.HasBody || methodDef.Body.HasInstructions)
				{
					methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Box, methodDef.Module.Import(typeof(Math))));
				}
			}
		}
	}
}
