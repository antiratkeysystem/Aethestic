using System;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.Calli;

internal class Calli
{
	public static void Execute(ModuleDef module)
	{
		TypeDef[] array = module.Types.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			MethodDef[] array2 = array[i].Methods.ToArray();
			foreach (MethodDef methodDef in array2)
			{
				if (!methodDef.HasBody || !methodDef.Body.HasInstructions || methodDef.FullName.Contains("My.") || methodDef.FullName.Contains(".My") || methodDef.FullName.Contains("Costura") || methodDef.IsConstructor || methodDef.DeclaringType.IsGlobalModuleType)
				{
					continue;
				}
				for (int k = 0; k < methodDef.Body.Instructions.Count - 1; k++)
				{
					try
					{
						if (!methodDef.Body.Instructions[k].ToString().Contains("ISupportInitialize") && (methodDef.Body.Instructions[k].OpCode == OpCodes.Call || methodDef.Body.Instructions[k].OpCode == OpCodes.Callvirt || methodDef.Body.Instructions[k].OpCode == OpCodes.Ldloc_S) && !methodDef.Body.Instructions[k].ToString().Contains("Object") && (methodDef.Body.Instructions[k].OpCode == OpCodes.Call || methodDef.Body.Instructions[k].OpCode == OpCodes.Callvirt || methodDef.Body.Instructions[k].OpCode == OpCodes.Ldloc_S))
						{
							try
							{
								MemberRef memberRef = (MemberRef)methodDef.Body.Instructions[k].Operand;
								methodDef.Body.Instructions[k].OpCode = OpCodes.Calli;
								methodDef.Body.Instructions[k].Operand = memberRef.MethodSig;
								methodDef.Body.Instructions.Insert(k, Instruction.Create(OpCodes.Ldftn, memberRef));
							}
							catch (Exception)
							{
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
			foreach (MethodDef methodDef2 in module.GlobalType.Methods)
			{
				if (!(methodDef2.Name != ".ctor"))
				{
					module.GlobalType.Remove(methodDef2);
					break;
				}
			}
		}
	}
}
