using System;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.Mutation2;

public class Func : iMutation
{
	public FieldDef Decryptor { get; set; }

	public void Process(MethodDef method, ref int index)
	{
		int ldcI4Value = method.Body.Instructions[index].GetLdcI4Value();
		method.Body.Instructions[index].OpCode = OpCodes.Ldsfld;
		method.Body.Instructions[index].Operand = Decryptor;
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Ldc_I4, ldcI4Value));
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Callvirt, method.Module.Import(typeof(Func<int, int>).GetMethod("Invoke"))));
		index -= 2;
	}

	public FieldDef CreateProperField(TypeDef type)
	{
		TypeDef typeDef = ModuleDefMD.Load(typeof(FuncMutation).Module).ResolveTypeDef(MDToken.ToRID(typeof(FuncMutation).MetadataToken));
		FieldDef fieldDef = typeDef.Fields.FirstOrDefault((FieldDef x) => x.Name == "prao");
		fieldDef.DeclaringType = null;
		type.Fields.Add(fieldDef);
		MethodDef methodDef = typeDef.FindMethod("RET");
		methodDef.DeclaringType = null;
		type.Methods.Add(methodDef);
		MethodDef methodDef2 = type.FindOrCreateStaticConstructor();
		methodDef2.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldnull));
		methodDef2.Body.Instructions.Insert(1, new Instruction(OpCodes.Ldftn, methodDef));
		methodDef2.Body.Instructions.Insert(2, new Instruction(OpCodes.Newobj, type.Module.Import(typeof(Func<int, int>).GetConstructors().First())));
		methodDef2.Body.Instructions.Insert(3, new Instruction(OpCodes.Stsfld, fieldDef));
		methodDef2.Body.Instructions.Insert(4, new Instruction(OpCodes.Nop));
		return fieldDef;
	}

	public void Prepare(TypeDef type)
	{
		Decryptor = CreateProperField(type);
	}

	public bool Supported(Instruction instr)
	{
		return Utils.CheckArithmetic(instr);
	}
}
