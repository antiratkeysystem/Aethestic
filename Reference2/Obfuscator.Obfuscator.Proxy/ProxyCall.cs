using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Proxy;

public class ProxyCall
{
	private static bool Analyse(ModuleDef module, MethodDef method)
	{
		if (method.FullName.ToLower().Contains("get_") || method.FullName.ToLower().Contains("get_"))
		{
			return false;
		}
		if (method.FullName.ToLower().Contains("datetime"))
		{
			return false;
		}
		if (method.Attributes.ToString().Replace("PrivateScope", "").Contains("Private"))
		{
			method.Attributes = MethodAttributes.Public | MethodAttributes.Static;
		}
		for (int i = 0; i < method.Body.Instructions.Count(); i++)
		{
			if (method.Body.Instructions[i].OpCode == OpCodes.Call && method.Body.Instructions[i].Operand is MethodDef)
			{
				MethodAttributes attributes = ((MethodDef)method.Body.Instructions[i].Operand).Attributes;
				if (attributes.ToString().Contains("Private") || !attributes.ToString().Contains("Static"))
				{
					return false;
				}
			}
			else if (method.Body.Instructions[i].OpCode == OpCodes.Ldsfld && method.Body.Instructions[i].Operand is FieldDef)
			{
				FieldAttributes attributes2 = ((FieldDef)method.Body.Instructions[i].Operand).Attributes;
				if (attributes2.ToString().Contains("Private") || !attributes2.ToString().Contains("Static"))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static void Execute(ModuleDef module)
	{
		for (int i = 0; i < Methods.Random.Next(4, 8); i++)
		{
			foreach (TypeDef typeDef in module.Types)
			{
				if (typeDef.IsGlobalModuleType)
				{
					continue;
				}
				MethodDef[] array = typeDef.Methods.ToArray();
				foreach (MethodDef methodDef in array)
				{
					if (!methodDef.HasBody)
					{
						continue;
					}
					for (int k = 0; k < methodDef.Body.Instructions.Count; k++)
					{
						if (methodDef.Body.Instructions[k].OpCode != OpCodes.Call)
						{
							continue;
						}
						try
						{
							if (!(methodDef.Body.Instructions[k].Operand is MethodDef))
							{
								continue;
							}
							MethodDef methodDef2 = methodDef.Body.Instructions[k].Operand as MethodDef;
							if (!Analyse(module, methodDef2) || methodDef2.Parameters.Count > 4)
							{
								continue;
							}
							MethodDefUser methodDefUser = new MethodDefUser(Methods.GenerateString(16), methodDef2.MethodSig, methodDef2.ImplAttributes, methodDef2.Attributes);
							methodDefUser.Body = methodDef2.Body;
							foreach (ParamDef paramDef in methodDef2.ParamDefs)
							{
								methodDefUser.ParamDefs.Add(new ParamDefUser(Methods.GenerateString(10), paramDef.Sequence));
							}
							CilBody cilBody = new CilBody();
							cilBody.Instructions.Add(OpCodes.Nop.ToInstruction());
							for (int l = 0; l < methodDef2.Parameters.Count; l++)
							{
								switch (l)
								{
								case 0:
									cilBody.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
									break;
								case 1:
									cilBody.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
									break;
								case 2:
									cilBody.Instructions.Add(OpCodes.Ldarg_2.ToInstruction());
									break;
								case 3:
									cilBody.Instructions.Add(OpCodes.Ldarg_3.ToInstruction());
									break;
								}
							}
							cilBody.Instructions.Add(OpCodes.Call.ToInstruction(methodDefUser));
							cilBody.Instructions.Add(OpCodes.Nop.ToInstruction());
							cilBody.Instructions.Add(OpCodes.Ret.ToInstruction());
							methodDef2.Body = cilBody;
							typeDef.Methods.Add(methodDefUser);
						}
						catch
						{
						}
					}
				}
			}
		}
	}
}
