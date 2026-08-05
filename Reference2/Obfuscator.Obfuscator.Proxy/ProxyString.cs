using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Proxy;

internal class ProxyString
{
	public static void Execute(ModuleDef module)
	{
		foreach (TypeDef typeDef in module.GetTypes())
		{
			if (typeDef.IsGlobalModuleType || typeDef.Name == "FakeError")
			{
				continue;
			}
			foreach (MethodDef methodDef in typeDef.Methods)
			{
				if (!methodDef.HasBody)
				{
					continue;
				}
				foreach (Instruction instruction in methodDef.Body.Instructions)
				{
					if (instruction.OpCode == OpCodes.Ldstr)
					{
						MethodImplAttributes implFlags = MethodImplAttributes.IL;
						MethodAttributes flags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
						MethodDefUser methodDefUser = new MethodDefUser(Methods.GenerateString(), MethodSig.CreateStatic(module.CorLibTypes.String), implFlags, flags);
						module.GlobalType.Methods.Add(methodDefUser);
						methodDefUser.Body = new CilBody();
						methodDefUser.Body.Variables.Add(new Local(module.CorLibTypes.String));
						methodDefUser.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, instruction.Operand.ToString()));
						methodDefUser.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
						instruction.OpCode = OpCodes.Call;
						instruction.Operand = methodDefUser;
					}
				}
			}
		}
	}
}
