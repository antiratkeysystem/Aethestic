using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.ProtectStack;

internal class ProtectStack
{
	public static void Execute(ModuleDef mod)
	{
		foreach (TypeDef type in mod.Types)
		{
			foreach (MethodDef methodDef in type.Methods)
			{
				if (!methodDef.HasBody)
				{
					break;
				}
				CilBody body = methodDef.Body;
				Instruction instruction = body.Instructions[0];
				Instruction instruction2 = Instruction.Create(OpCodes.Br_S, instruction);
				Instruction item = Instruction.Create(OpCodes.Pop);
				Instruction randomInstruction = GetRandomInstruction(new Random());
				body.Instructions.Insert(0, randomInstruction);
				body.Instructions.Insert(1, item);
				body.Instructions.Insert(2, instruction2);
				if (body.ExceptionHandlers == null)
				{
					continue;
				}
				foreach (ExceptionHandler exceptionHandler in body.ExceptionHandlers)
				{
					if (exceptionHandler.TryStart == instruction)
					{
						exceptionHandler.TryStart = instruction2;
					}
					else if (exceptionHandler.HandlerStart == instruction)
					{
						exceptionHandler.HandlerStart = instruction2;
					}
					else if (exceptionHandler.FilterStart == instruction)
					{
						exceptionHandler.FilterStart = instruction2;
					}
				}
			}
		}
	}

	private static Instruction GetRandomInstruction(Random random)
	{
		return random.Next(0, 5) switch
		{
			0 => Instruction.Create(OpCodes.Ldnull), 
			1 => Instruction.Create(OpCodes.Ldc_I4_0), 
			2 => Instruction.Create(OpCodes.Ldstr, "Isolator"), 
			3 => Instruction.Create(OpCodes.Ldc_I8, (long)random.Next()), 
			_ => Instruction.Create(OpCodes.Ldc_I8, (long)random.Next()), 
		};
	}
}
