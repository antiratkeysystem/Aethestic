using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.Proxy;

public static class ProxyMeth
{
	public static Random rand = new Random();

	public static List<MemberRef> MemberRefList = new List<MemberRef>();

	public static void ScanMemberRef(ModuleDef module)
	{
		foreach (TypeDef type in module.Types)
		{
			foreach (MethodDef methodDef in type.Methods)
			{
				if (!methodDef.HasBody || !methodDef.Body.HasInstructions)
				{
					continue;
				}
				for (int i = 0; i < methodDef.Body.Instructions.Count - 1; i++)
				{
					if (methodDef.Body.Instructions[i].OpCode != OpCodes.Call)
					{
						continue;
					}
					try
					{
						MemberRef memberRef = (MemberRef)methodDef.Body.Instructions[i].Operand;
						if (!memberRef.HasThis)
						{
							MemberRefList.Add(memberRef);
						}
					}
					catch
					{
					}
				}
			}
		}
	}

	public static MethodDef GenerateSwitch(MemberRef original, ModuleDef md)
	{
		try
		{
			List<TypeSig> list = original.MethodSig.Params.ToList();
			list.Add(md.CorLibTypes.Int32);
			MethodImplAttributes implFlags = MethodImplAttributes.IL;
			MethodAttributes flags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
			MethodDef methodDef = new MethodDefUser($"ProxyMeth{rand.Next(0, int.MaxValue)}", MethodSig.CreateStatic(original.MethodSig.RetType, list.ToArray()), implFlags, flags)
			{
				Body = new CilBody()
			};
			methodDef.Body.Variables.Add(new Local(md.CorLibTypes.Int32));
			methodDef.Body.Variables.Add(new Local(md.CorLibTypes.Int32));
			methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
			List<Instruction> list2 = new List<Instruction>();
			Instruction instruction = new Instruction(OpCodes.Switch);
			methodDef.Body.Instructions.Add(instruction);
			Instruction instruction2 = new Instruction(OpCodes.Br_S);
			methodDef.Body.Instructions.Add(instruction2);
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j <= original.MethodSig.Params.Count - 1; j++)
				{
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, methodDef.Parameters[j]));
					if (j == 0)
					{
						list2.Add(Instruction.Create(OpCodes.Ldarg, methodDef.Parameters[j]));
					}
				}
				Instruction item = Instruction.Create(OpCodes.Ldc_I4, i);
				methodDef.Body.Instructions.Add(item);
				methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			}
			Instruction instruction3 = Instruction.Create(OpCodes.Ldnull);
			methodDef.Body.Instructions.Add(instruction3);
			methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			instruction2.Operand = instruction3;
			instruction.Operand = list2;
			return methodDef;
		}
		catch
		{
			return null;
		}
	}

	public static void Execute(ModuleDef module)
	{
		ScanMemberRef(module);
		foreach (TypeDef typeDef in module.Types)
		{
			if (typeDef.IsGlobalModuleType)
			{
				continue;
			}
			MethodDef[] array = typeDef.Methods.ToArray();
			foreach (MethodDef methodDef in array)
			{
				if (!methodDef.HasBody || methodDef.Name.Contains("ProxyMeth"))
				{
					continue;
				}
				IList<Instruction> instructions = methodDef.Body.Instructions;
				for (int j = 0; j < instructions.Count; j++)
				{
					if (methodDef.Body.Instructions[j].OpCode != OpCodes.Call)
					{
						continue;
					}
					try
					{
						MemberRef original = (MemberRef)methodDef.Body.Instructions[j].Operand;
						if (original.HasThis)
						{
							continue;
						}
						MethodDef methodDef2 = GenerateSwitch(original, module);
						methodDef.DeclaringType.Methods.Add(methodDef2);
						instructions[j].OpCode = OpCodes.Call;
						instructions[j].Operand = methodDef2;
						int value = rand.Next(0, 5);
						for (int k = 0; k < methodDef2.Body.Instructions.Count - 1; k++)
						{
							if (methodDef2.Body.Instructions[k].OpCode != OpCodes.Ldc_I4)
							{
								continue;
							}
							if (string.Compare(methodDef2.Body.Instructions[k].Operand.ToString(), value.ToString(), StringComparison.Ordinal) != 0)
							{
								methodDef2.Body.Instructions[k].OpCode = OpCodes.Call;
								methodDef2.Body.Instructions[k].Operand = MemberRefList.Where((MemberRef m) => m.MethodSig.Params.Count == original.MethodSig.Params.Count).ToList().Random();
							}
							else
							{
								methodDef2.Body.Instructions[k].OpCode = OpCodes.Call;
								methodDef2.Body.Instructions[k].Operand = original;
							}
						}
						methodDef.Body.Instructions.Insert(j, Instruction.CreateLdcI4(value));
					}
					catch
					{
					}
				}
			}
		}
	}
}
