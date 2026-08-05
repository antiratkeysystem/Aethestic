using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Mutation2;

public class CharMutations : iMutation
{
	public MethodDef Converter { get; set; }

	public void Prepare(TypeDef type)
	{
		MethodDef methodDef = ModuleDefMD.Load(typeof(FuncMutation).Module).ResolveTypeDef(MDToken.ToRID(typeof(FuncMutation).MetadataToken)).FindMethod("CharToInt");
		methodDef.Name = Methods.GenerateString();
		methodDef.DeclaringType = null;
		type.Methods.Add(methodDef);
		Converter = methodDef;
	}

	public void Process(MethodDef method, ref int index)
	{
		method.Body.Instructions.Insert(++index, new Instruction(OpCodes.Call, Converter));
	}

	public bool Supported(Instruction instr)
	{
		return Utils.CheckArithmetic(instr);
	}
}
