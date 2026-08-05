using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.Mutation1;

internal class Mutation1
{
	private static ModuleDefMD _moduleDefMd;

	public static void Execute(ModuleDefMD moduleDefMd)
	{
		_moduleDefMd = moduleDefMd;
		MutationHelper.CryptoRandom cryptoRandom = new MutationHelper.CryptoRandom();
		foreach (TypeDef typeDef in moduleDefMd.GetTypes())
		{
			List<MethodDef> list = new List<MethodDef>();
			foreach (MethodDef methodDef in typeDef.Methods.Where((MethodDef x) => x.HasBody))
			{
				IList<Instruction> instructions = methodDef.Body.Instructions;
				for (int i = 0; i < instructions.Count; i++)
				{
					if (instructions[i].IsLdcI4() && IsSafe(instructions.ToList(), i))
					{
						MethodDef methodDef2 = null;
						int ldcI4Value = instructions[i].GetLdcI4Value();
						instructions[i].OpCode = OpCodes.Ldc_R8;
						switch (cryptoRandom.Next(0, 3))
						{
						case 0:
							methodDef2 = GenerateRefMethod("Floor");
							instructions[i].Operand = Convert.ToDouble((double)ldcI4Value + cryptoRandom.NextDouble());
							break;
						case 1:
							methodDef2 = GenerateRefMethod("Sqrt");
							instructions[i].Operand = Math.Pow(Convert.ToDouble(ldcI4Value), 2.0);
							break;
						case 2:
							methodDef2 = GenerateRefMethod("Round");
							instructions[i].Operand = Convert.ToDouble(ldcI4Value);
							break;
						}
						instructions.Insert(i + 1, OpCodes.Call.ToInstruction(methodDef2));
						instructions.Insert(i + 2, OpCodes.Conv_I4.ToInstruction());
						i += 2;
						list.Add(methodDef2);
					}
				}
				methodDef.Body.SimplifyMacros(methodDef.Parameters);
			}
			foreach (MethodDef item in list)
			{
				typeDef.Methods.Add(item);
			}
		}
	}

	private static MethodDef GenerateRefMethod(string methodName)
	{
		MethodDefUser methodDefUser = new MethodDefUser("_" + Guid.NewGuid().ToString("D").ToUpper()
			.Substring(2, 5), MethodSig.CreateStatic(_moduleDefMd.ImportAsTypeSig(typeof(double))), MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig)
		{
			Signature = new MethodSig
			{
				Params = { _moduleDefMd.ImportAsTypeSig(typeof(double)) },
				RetType = _moduleDefMd.ImportAsTypeSig(typeof(double))
			}
		};
		CilBody cilBody = new CilBody();
		cilBody.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
		cilBody.Instructions.Add(OpCodes.Call.ToInstruction(GetMethod(typeof(Math), methodName, new Type[1] { typeof(double) })));
		cilBody.Instructions.Add(OpCodes.Stloc_0.ToInstruction());
		cilBody.Instructions.Add(OpCodes.Ldloc_0.ToInstruction());
		cilBody.Instructions.Add(OpCodes.Ret.ToInstruction());
		CilBody body = cilBody;
		methodDefUser.Body = body;
		methodDefUser.Body.Variables.Add(new Local(_moduleDefMd.ImportAsTypeSig(typeof(double))));
		return methodDefUser.ResolveMethodDef();
	}

	private static bool IsSafe(List<Instruction> instructions, int i)
	{
		return !new int[5] { -2, -1, 0, 1, 2 }.Contains(instructions[i].GetLdcI4Value());
	}

	private static IMethod GetMethod(Type type, string methodName, Type[] types)
	{
		return _moduleDefMd.Import(type.GetMethod(methodName, types));
	}
}
