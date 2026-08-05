using System;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Obfuscator.IntProtect;

namespace Obfuscator.Obfuscator.CtrlFlow;

internal class CflowV2
{
	private static readonly Type[] types = new Type[7]
	{
		typeof(uint),
		typeof(int),
		typeof(long),
		typeof(ulong),
		typeof(ushort),
		typeof(short),
		typeof(double)
	};

	private static readonly int[] sizes = new int[7] { 4, 4, 8, 8, 2, 2, 8 };

	public static void Execute(ModuleDefMD md)
	{
		foreach (TypeDef typeDef in md.Types)
		{
			if (typeDef == md.GlobalType)
			{
				continue;
			}
			foreach (MethodDef methodDef in typeDef.Methods)
			{
				if (!methodDef.Name.StartsWith("get_") && !methodDef.Name.StartsWith("set_") && methodDef.HasBody && !methodDef.IsConstructor)
				{
					methodDef.Body.SimplifyBranches();
					ExecuteMethod(methodDef);
				}
			}
		}
	}

	public static void ExecuteMethod(MethodDef method)
	{
		Console.WriteLine(string.Concat(method.Name, method.Body.Instructions.Count().ToString()));
		for (int i = 0; i < method.Body.Instructions.Count(); i++)
		{
			if (method.Body.Instructions[i].IsLdcI4())
			{
				int num = IntEncoding.Next();
				int num2 = IntEncoding.Next();
				int value = num ^ num2;
				int num3 = IntEncoding.Next(types.Length);
				Type type = types[num3];
				Instruction instruction = OpCodes.Nop.ToInstruction();
				Local local = new Local(method.Module.ImportAsTypeSig(type));
				Instruction item = OpCodes.Stloc.ToInstruction(local);
				method.Body.Variables.Add(local);
				method.Body.Instructions.Insert(i + 1, item);
				method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldc_I4, method.Body.Instructions[i].GetLdcI4Value() - sizes[num3]));
				method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Ldc_I4, value));
				method.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Ldc_I4, num2));
				method.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Xor));
				method.Body.Instructions.Insert(i + 6, Instruction.Create(OpCodes.Ldc_I4, num));
				method.Body.Instructions.Insert(i + 7, Instruction.Create(OpCodes.Bne_Un, instruction));
				method.Body.Instructions.Insert(i + 8, Instruction.Create(OpCodes.Ldc_I4, 2));
				method.Body.Instructions.Insert(i + 9, item);
				method.Body.Instructions.Insert(i + 10, Instruction.Create(OpCodes.Sizeof, method.Module.Import(type)));
				method.Body.Instructions.Insert(i + 11, Instruction.Create(OpCodes.Add));
				method.Body.Instructions.Insert(i + 12, instruction);
				i += method.Body.Instructions.Count - i;
			}
		}
	}
}
