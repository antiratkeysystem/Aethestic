using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Junk;

internal class Junks
{
	public static Random random = new Random();

	public static List<MethodDef> AllVoid = new List<MethodDef>();

	public static MethodDefUser MethodGenerator(ModuleDef module, CilBody instr, MethodAttributes attributes)
	{
		MethodImplAttributes implFlags = MethodImplAttributes.IL;
		return new MethodDefUser(Methods.GenerateString(), MethodSig.CreateStatic(module.CorLibTypes.Void), implFlags, attributes)
		{
			Body = instr
		};
	}

	public static MethodDefUser GetStringFunction(ModuleDef module, CilBody cilBody, MethodAttributes attributes)
	{
		MethodImplAttributes implFlags = MethodImplAttributes.IL;
		return new MethodDefUser(Methods.GenerateString(), MethodSig.CreateStatic(module.CorLibTypes.String), implFlags, attributes)
		{
			Body = cilBody
		};
	}

	public static MethodDefUser GetIntFunction(ModuleDef module, CilBody instr, MethodAttributes attributes)
	{
		MethodImplAttributes implFlags = MethodImplAttributes.IL;
		return new MethodDefUser(Methods.GenerateString(), MethodSig.CreateStatic(module.CorLibTypes.Int32), implFlags, attributes)
		{
			Body = instr
		};
	}

	public static MethodDefUser GetLongFunction(ModuleDef module, CilBody instr, MethodAttributes attributes)
	{
		MethodImplAttributes implFlags = MethodImplAttributes.IL;
		return new MethodDefUser(Methods.GenerateString(), MethodSig.CreateStatic(module.CorLibTypes.Int64), implFlags, attributes)
		{
			Body = instr
		};
	}

	public static FieldDef CreateString(ModuleDef module, FieldAttributes attributes)
	{
		FieldSig signature = new FieldSig(module.CorLibTypes.String);
		return new FieldDefUser(Methods.GenerateString(), signature, attributes);
	}

	public static FieldDef CreateInt32(ModuleDef module, FieldAttributes attributes)
	{
		FieldSig signature = new FieldSig(module.CorLibTypes.Int32);
		return new FieldDefUser(Methods.GenerateString(), signature, attributes);
	}

	public static FieldDef CreateInt64(ModuleDef module, FieldAttributes attributes)
	{
		FieldSig signature = new FieldSig(module.CorLibTypes.Int64);
		return new FieldDefUser(Methods.GenerateString(), signature, attributes);
	}

	public static FieldDef CreateBool(ModuleDef module, FieldAttributes attributes)
	{
		FieldSig signature = new FieldSig(module.CorLibTypes.Boolean);
		return new FieldDefUser(Methods.GenerateString(), signature, attributes);
	}

	public static TypeDef CreateNewClass()
	{
		return new TypeDefUser(Methods.GenerateString())
		{
			Attributes = (TypeAttributes.Abstract | TypeAttributes.BeforeFieldInit),
			Namespace = Methods.GenerateString()
		};
	}

	public static MethodDef cctor(ModuleDef module)
	{
		return new MethodDefUser(".cctor", MethodSig.CreateInstance(module.CorLibTypes.Void), MethodImplAttributes.IL, MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
	}

	public static MethodDef ctor(ModuleDef module)
	{
		return new MethodDefUser(".ctor", MethodSig.CreateInstance(module.CorLibTypes.Void), MethodImplAttributes.IL, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
	}

	public static FieldAttributes RandomFieldAttributes(bool Static)
	{
		FieldAttributes fieldAttributes = FieldAttributes.Private;
		switch (random.Next(0, 3))
		{
		case 0:
			fieldAttributes = FieldAttributes.Public;
			break;
		case 1:
			fieldAttributes = FieldAttributes.Private;
			break;
		case 2:
			fieldAttributes = FieldAttributes.Literal;
			break;
		}
		if (Static)
		{
			fieldAttributes |= FieldAttributes.Static;
		}
		return fieldAttributes;
	}

	public static MethodAttributes RandomMethodAttributes(bool Static)
	{
		MethodAttributes methodAttributes = MethodAttributes.Private;
		switch (random.Next(0, 3))
		{
		case 0:
			methodAttributes = MethodAttributes.Public;
			break;
		case 1:
			methodAttributes = MethodAttributes.Private;
			break;
		case 2:
			methodAttributes = MethodAttributes.Virtual;
			break;
		}
		if (Static)
		{
			methodAttributes |= MethodAttributes.Static;
		}
		return methodAttributes | MethodAttributes.HideBySig | MethodAttributes.PrivateScope;
	}

	public static CilBody GeneratorBody(ModuleDef module, string TypeMethod)
	{
		CilBody cilBody = new CilBody();
		if (TypeMethod == "String")
		{
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Nop));
			cilBody.Variables.Add(new Local(module.CorLibTypes.String, Methods.GenerateString()));
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Ldstr, Methods.GenerateString()));
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Stloc_0));
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
		}
		if (TypeMethod == "Int32")
		{
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Nop));
			cilBody.Variables.Add(new Local(module.CorLibTypes.Int32, Methods.GenerateString()));
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, random.Next(int.MinValue, int.MaxValue)));
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Stloc_0));
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
		}
		if (TypeMethod == "Int64")
		{
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Nop));
			cilBody.Variables.Add(new Local(module.CorLibTypes.Int64, Methods.GenerateString()));
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, random.Next(int.MinValue, int.MaxValue)));
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Stloc_0));
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
		}
		if (TypeMethod != "Void")
		{
			cilBody.Instructions.Add(Instruction.Create(OpCodes.Ret));
		}
		return cilBody;
	}

	public static void Execute(ModuleDef module)
	{
		int num = random.Next(20, 100);
		for (int i = 0; i < num; i++)
		{
			TypeDef typeDef = CreateNewClass();
			bool flag = false;
			MethodDef methodDef;
			if (random.Next(0, 3) == 1)
			{
				flag = true;
				methodDef = cctor(module);
			}
			else
			{
				methodDef = ctor(module);
			}
			if (random.Next(0, 8) == 1)
			{
				typeDef.Namespace = Methods.GenerateString();
			}
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < random.Next(0, 2); k++)
				{
					typeDef.Fields.Add(CreateInt32(module, RandomFieldAttributes(flag)));
				}
				for (int l = 0; l < random.Next(0, 2); l++)
				{
					typeDef.Methods.Add(GetIntFunction(module, GeneratorBody(module, "Int32"), RandomMethodAttributes(flag)));
				}
				for (int m = 0; m < random.Next(0, 2); m++)
				{
					typeDef.Fields.Add(CreateBool(module, RandomFieldAttributes(flag)));
				}
				for (int n = 0; n < random.Next(0, 2); n++)
				{
					typeDef.Methods.Add(GetStringFunction(module, GeneratorBody(module, "String"), RandomMethodAttributes(flag)));
				}
				for (int num2 = 0; num2 < random.Next(0, 2); num2++)
				{
					typeDef.Fields.Add(CreateString(module, RandomFieldAttributes(flag)));
				}
				for (int num3 = 0; num3 < random.Next(0, 2); num3++)
				{
					typeDef.Fields.Add(CreateInt64(module, RandomFieldAttributes(flag)));
				}
				for (int num4 = 0; num4 < random.Next(0, 2); num4++)
				{
					MethodDef item = MethodGenerator(module, GeneratorBody(module, "Void"), RandomMethodAttributes(flag));
					if (flag)
					{
						AllVoid.Add(item);
					}
					typeDef.Methods.Add(item);
				}
			}
			typeDef.Methods.Add(methodDef);
			methodDef.Body = new CilBody();
			if (!flag)
			{
				MemberRefUser mr = new MemberRefUser(module, ".ctor", MethodSig.CreateInstance(module.CorLibTypes.Void), module.CorLibTypes.Object.TypeDefOrRef);
				methodDef.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
				methodDef.Body.Instructions.Add(OpCodes.Call.ToInstruction(mr));
			}
			foreach (FieldDef fieldDef in typeDef.Fields)
			{
				if (fieldDef.FieldType.TypeName == "Int64")
				{
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I8, (long)random.Next(int.MinValue, int.MaxValue)));
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, fieldDef));
				}
				if (fieldDef.FieldType.TypeName == "Boolean")
				{
					if (random.Next(0, 2) == 1)
					{
						methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4_1));
					}
					else
					{
						methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
					}
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, fieldDef));
				}
				if (fieldDef.FieldType.TypeName == "String")
				{
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, Methods.GenerateString()));
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, fieldDef));
				}
				if (fieldDef.FieldType.TypeName == "Int32")
				{
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, random.Next(int.MinValue, int.MaxValue)));
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, fieldDef));
				}
			}
			methodDef.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
			module.Types.Add(typeDef);
		}
		for (int num5 = 0; num5 < random.Next(AllVoid.Count); num5++)
		{
			MethodDef methodDef2 = AllVoid[random.Next(AllVoid.Count)];
			MethodDef method = AllVoid[random.Next(AllVoid.Count)];
			methodDef2.Body.Instructions.Add(Instruction.Create(OpCodes.Call, method));
			methodDef2.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
		}
		AllVoid.Clear();
	}
}
