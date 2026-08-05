using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Mutation2;

public class ComparerMutation : iMutation
{
	public void Prepare(TypeDef type)
	{
		if (type == type.Module.GlobalType)
		{
			return;
		}
		for (int i = 0; i < type.Methods.Count; i++)
		{
			MethodDef methodDef = type.Methods[i];
			if (!methodDef.HasBody || methodDef.IsConstructor)
			{
				continue;
			}
			methodDef.Body.SimplifyBranches();
			for (int j = 0; j < methodDef.Body.Instructions.Count; j++)
			{
				if (Utils.CheckArithmetic(methodDef.Body.Instructions[j]))
				{
					Execute(methodDef, ref j);
				}
			}
		}
	}

	public void Execute(MethodDef method, ref int index)
	{
		if (method.Body.Instructions[index].OpCode == OpCodes.Call)
		{
			return;
		}
		int ldcI4Value = method.Body.Instructions[index].GetLdcI4Value();
		Local local = new Local(method.Module.CorLibTypes.Int32);
		Local local2 = new Local(method.Module.CorLibTypes.Int32);
		method.Body.Variables.Add(local);
		method.Body.Variables.Add(local2);
		int num = Methods.Random.Next();
		int num2 = Methods.Random.Next();
		bool flag = Convert.ToBoolean(Methods.Random.Next(2));
		int num3;
		if (flag)
		{
			num3 = num2 - num;
		}
		else
		{
			num3 = Methods.Random.Next();
			while (num3 + num == num2)
			{
				num3 = Methods.Random.Next();
			}
		}
		method.Body.Instructions[index] = Instruction.CreateLdcI4(num);
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Stloc, local));
		method.Body.Instructions.Insert(++index, Instruction.CreateLdcI4(num3));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Stloc, local2));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldloc, local));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldloc, local2));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Add));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldc_I4, num2));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ceq));
		Instruction instruction = OpCodes.Nop.ToInstruction();
		method.Body.Instructions.Insert(++index, new Instruction(flag ? OpCodes.Brfalse : OpCodes.Brtrue, instruction));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Nop));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldc_I4, ldcI4Value));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Stloc, local));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Nop));
		Instruction instruction2 = OpCodes.Ldloc_S.ToInstruction(local);
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Br, instruction2));
		method.Body.Instructions.Insert(++index, instruction);
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldc_I4, Methods.Random.Next()));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Stloc, local));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Nop));
		method.Body.Instructions.Insert(++index, instruction2);
	}

	public void Process(MethodDef method, ref int index)
	{
	}

	public static void InsertInstructions(IList<Instruction> instructions, Dictionary<Instruction, int> keyValuePairs)
	{
		foreach (KeyValuePair<Instruction, int> keyValuePair in keyValuePairs)
		{
			Instruction key = keyValuePair.Key;
			int value = keyValuePair.Value;
			instructions.Insert(value, key);
		}
	}

	public bool Supported(Instruction instr)
	{
		return Utils.CheckArithmetic(instr);
	}
}
