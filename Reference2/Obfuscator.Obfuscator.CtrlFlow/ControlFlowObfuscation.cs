using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.CtrlFlow;

internal class ControlFlowObfuscation
{
	public static Random rnd = new Random();

	public static void Execute(ModuleDefMD md)
	{
		foreach (TypeDef typeDef in md.Types)
		{
			if (typeDef == md.GlobalType || typeDef.Name == "FakeError")
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

	public static TypeSig RandomSig(MethodDef method)
	{
		return rnd.Next(0, 3) switch
		{
			0 => method.Module.CorLibTypes.Int32, 
			1 => method.Module.CorLibTypes.Int64, 
			2 => method.Module.CorLibTypes.Double, 
			_ => method.Module.CorLibTypes.Int32, 
		};
	}

	public static void ExecuteMethod(MethodDef method)
	{
		foreach (ParamDef paramDef in method.ParamDefs)
		{
			paramDef.Name = Methods.GenerateString(16);
		}
		method.Body.SimplifyMacros(method.Parameters);
		List<Block> blocks = BlockParser.ParseMethod(method);
		int num = rnd.Next(1, 10);
		int num2 = 0;
		foreach (Block block3 in blocks)
		{
			block3.SubRand = num;
			num = (block3.PlusRand = rnd.Next(num + 1, num + 10));
			if (blocks.Count - 2 == block3.Number)
			{
				num2 = num;
			}
		}
		blocks = Randomize(blocks);
		method.Body.Instructions.Clear();
		Local local = new Local(RandomSig(method));
		method.Body.Variables.Add(local);
		local.Name = Methods.GenerateString();
		Instruction instruction = Instruction.Create(OpCodes.Nop);
		Instruction instruction2 = Instruction.Create(OpCodes.Br, instruction);
		int num4 = rnd.Next(0, 10000);
		method.Body.Instructions.Add(Calc(num4, local.Type));
		method.Body.Instructions.Add(OpCodes.Stloc.ToInstruction(local));
		method.Body.Instructions.Add(Instruction.Create(OpCodes.Br, instruction2));
		method.Body.Instructions.Add(instruction);
		Block lastBlock = blocks.Single((Block x) => x.Number == blocks.Count - 1);
		foreach (Block block4 in blocks.Where((Block block5) => block5 != lastBlock))
		{
			method.Body.Instructions.Add(OpCodes.Ldloc.ToInstruction(local));
			if (block4.Number == 0)
			{
				method.Body.Instructions.Add(Calc(num4, local.Type));
			}
			else
			{
				method.Body.Instructions.Add(Calc(block4.SubRand + num4, local.Type));
			}
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Ceq));
			Instruction instruction3 = Instruction.Create(OpCodes.Nop);
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse, instruction3));
			foreach (Instruction item in block4.Instructions)
			{
				method.Body.Instructions.Add(item);
			}
			method.Body.Instructions.Add(Calc(block4.PlusRand + num4, local.Type));
			method.Body.Instructions.Add(OpCodes.Stloc.ToInstruction(local));
			method.Body.Instructions.Add(instruction3);
		}
		method.Body.Instructions.Add(OpCodes.Ldloc.ToInstruction(local));
		method.Body.Instructions.Add(Calc(num2 + num4, local.Type));
		method.Body.Instructions.Add(Instruction.Create(OpCodes.Ceq));
		method.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse, instruction2));
		method.Body.Instructions.Add(Instruction.Create(OpCodes.Br, blocks.Single((Block x) => x.Number == blocks.Count - 1).Instructions[0]));
		method.Body.Instructions.Add(instruction2);
		foreach (Instruction item2 in blocks.Single((Block x) => x.Number == blocks.Count - 1).Instructions)
		{
			method.Body.Instructions.Add(item2);
		}
		method.Body.InitLocals = true;
	}

	public static List<Block> Randomize(List<Block> input)
	{
		List<Block> list = new List<Block>();
		foreach (Block item in input)
		{
			list.Insert(rnd.Next(0, list.Count), item);
		}
		return list;
	}

	public static Instruction Calc(int value, TypeSig sig)
	{
		if (sig == sig.Module.CorLibTypes.Double)
		{
			return Instruction.Create(OpCodes.Ldc_R8, (double)value);
		}
		return Instruction.Create(OpCodes.Ldc_I4, value);
	}
}
