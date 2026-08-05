using System;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;
using Obfuscator.Obfuscator.Strings.Runtime;

namespace Obfuscator.Obfuscator.Strings;

internal class StringEcnryption
{
	private static MethodDef InjectMethod(ModuleDef module, string methodName)
	{
		MethodDef result = (MethodDef)InjectHelper.Inject(ModuleDefMD.Load(typeof(DecryptionHelper).Module).ResolveTypeDef(MDToken.ToRID(typeof(DecryptionHelper).MetadataToken)), module.GlobalType, module).Single((IDnlibDef method) => method.Name == methodName);
		foreach (MethodDef methodDef in module.GlobalType.Methods)
		{
			if (methodDef.Name == ".ctor")
			{
				module.GlobalType.Remove(methodDef);
				break;
			}
		}
		return result;
	}

	private static string Encrypt(string dataPlain)
	{
		try
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(dataPlain));
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static void Execute(ModuleDef module)
	{
		MethodDef methodDef = InjectMethod(module, "Decrypt_Base64");
		foreach (TypeDef typeDef in module.Types)
		{
			if (typeDef.IsGlobalModuleType || typeDef.Name == "Resources" || typeDef.Name == "Settings")
			{
				continue;
			}
			foreach (MethodDef methodDef2 in typeDef.Methods)
			{
				if (!methodDef2.HasBody || methodDef2 == methodDef)
				{
					continue;
				}
				methodDef2.Body.KeepOldMaxStack = true;
				for (int i = 0; i < methodDef2.Body.Instructions.Count; i++)
				{
					if (methodDef2.Body.Instructions[i].OpCode == OpCodes.Ldstr)
					{
						string dataPlain = methodDef2.Body.Instructions[i].Operand.ToString();
						methodDef2.Body.Instructions[i].Operand = Encrypt(dataPlain);
						methodDef2.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, methodDef));
					}
				}
				methodDef2.Body.SimplifyBranches();
				methodDef2.Body.OptimizeBranches();
			}
		}
	}
}
