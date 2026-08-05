using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.ImportProtect;

internal class ImportProtect
{
	public static FieldDefUser CreateField(FieldSig sig)
	{
		return new FieldDefUser(Methods.GenerateString(), sig, FieldAttributes.Public | FieldAttributes.Static);
	}

	public static void Execute(ModuleDef module)
	{
		Dictionary<IMethod, MethodDef> dictionary = new Dictionary<IMethod, MethodDef>();
		Dictionary<IMethod, TypeDef> dictionary2 = new Dictionary<IMethod, TypeDef>();
		FieldDefUser fieldDefUser = CreateField(new FieldSig(module.ImportAsTypeSig(typeof(object[]))));
		MethodDef methodDef = module.GlobalType.FindOrCreateStaticConstructor();
		TypeDef[] array = module.GetTypes().ToArray();
		foreach (TypeDef typeDef in array)
		{
			if (typeDef.IsDelegate || typeDef.IsGlobalModuleType || typeDef.Namespace == "Costura")
			{
				continue;
			}
			MethodDef[] array2 = typeDef.Methods.ToArray();
			foreach (MethodDef methodDef2 in array2)
			{
				if (!methodDef2.HasBody || !methodDef2.Body.HasInstructions || methodDef2.IsConstructor)
				{
					continue;
				}
				IList<Instruction> instructions = methodDef2.Body.Instructions;
				for (int k = 0; k < instructions.Count; k++)
				{
					if ((instructions[k].OpCode != OpCodes.Call && instructions[k].OpCode == OpCodes.Callvirt) || !(instructions[k].Operand is IMethod { IsMethodDef: not false } method))
					{
						continue;
					}
					MethodDef methodDef3 = method.ResolveMethodDef();
					if (methodDef3 == null || methodDef3.HasThis)
					{
						continue;
					}
					if (dictionary.ContainsKey(method))
					{
						instructions[k].OpCode = OpCodes.Call;
						instructions[k].Operand = dictionary[method];
						continue;
					}
					MethodSig methodSig = CreateProxySignature(module, methodDef3);
					TypeDef typeDef2 = CreateDelegateType(module, methodSig);
					module.Types.Add(typeDef2);
					MethodImplAttributes implFlags = MethodImplAttributes.IL;
					MethodAttributes flags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
					MethodDefUser methodDefUser = new MethodDefUser(Methods.GenerateString(), methodSig, implFlags, flags);
					methodDefUser.Body = new CilBody();
					methodDefUser.Body.Instructions.Add(OpCodes.Ldsfld.ToInstruction(fieldDefUser));
					methodDefUser.Body.Instructions.Add(OpCodes.Ldc_I4.ToInstruction(dictionary2.Count));
					methodDefUser.Body.Instructions.Add(OpCodes.Ldelem_Ref.ToInstruction());
					foreach (Parameter parameter in methodDefUser.Parameters)
					{
						parameter.Name = Methods.GenerateString();
						methodDefUser.Body.Instructions.Add(OpCodes.Ldarg.ToInstruction(parameter));
					}
					methodDefUser.Body.Instructions.Add(OpCodes.Call.ToInstruction(typeDef2.Methods[1]));
					methodDefUser.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
					typeDef2.Methods.Add(methodDefUser);
					instructions[k].OpCode = OpCodes.Call;
					instructions[k].Operand = methodDefUser;
					if (method.IsMethodDef)
					{
						dictionary2.Add(methodDef3, typeDef2);
					}
					else if (method.IsMemberRef)
					{
						dictionary2.Add(method as MemberRef, typeDef2);
					}
					dictionary.Add(method, methodDefUser);
				}
			}
		}
		module.GlobalType.Fields.Add(fieldDefUser);
		List<Instruction> list = new List<Instruction>();
		List<Instruction> list2 = methodDef.Body.Instructions.ToList();
		methodDef.Body.Instructions.Clear();
		list.Add(OpCodes.Ldc_I4.ToInstruction(dictionary2.Count));
		list.Add(OpCodes.Newarr.ToInstruction(module.CorLibTypes.Object));
		list.Add(OpCodes.Dup.ToInstruction());
		int num = 0;
		foreach (KeyValuePair<IMethod, TypeDef> keyValuePair in dictionary2)
		{
			list.Add(OpCodes.Ldc_I4.ToInstruction(num));
			list.Add(OpCodes.Ldnull.ToInstruction());
			list.Add(OpCodes.Ldftn.ToInstruction(keyValuePair.Key));
			list.Add(OpCodes.Newobj.ToInstruction(keyValuePair.Value.Methods[0]));
			list.Add(OpCodes.Stelem_Ref.ToInstruction());
			list.Add(OpCodes.Dup.ToInstruction());
			num++;
		}
		list.Add(OpCodes.Pop.ToInstruction());
		list.Add(OpCodes.Stsfld.ToInstruction(fieldDefUser));
		foreach (Instruction item in list)
		{
			methodDef.Body.Instructions.Add(item);
		}
		foreach (Instruction item2 in list2)
		{
			methodDef.Body.Instructions.Add(item2);
		}
	}

	public static TypeDef CreateDelegateType(ModuleDef module, MethodSig sig)
	{
		TypeDefUser obj = new TypeDefUser(Methods.GenerateString(), module.CorLibTypes.GetTypeRef("System", "MulticastDelegate"))
		{
			Attributes = (TypeAttributes.Public | TypeAttributes.Sealed)
		};
		MethodDefUser methodDefUser = new MethodDefUser(".ctor", MethodSig.CreateInstance(module.CorLibTypes.Void, module.CorLibTypes.Object, module.CorLibTypes.IntPtr))
		{
			Attributes = (MethodAttributes.Assembly | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName),
			ImplAttributes = MethodImplAttributes.CodeTypeMask
		};
		obj.Methods.Add(methodDefUser);
		MethodDefUser methodDefUser2 = new MethodDefUser("Invoke", sig.Clone())
		{
			MethodSig = 
			{
				HasThis = true
			},
			Attributes = (MethodAttributes.Assembly | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask),
			ImplAttributes = MethodImplAttributes.CodeTypeMask
		};
		obj.Methods.Add(methodDefUser2);
		return obj;
	}

	public static MethodSig CreateProxySignature(ModuleDef module, IMethod method)
	{
		IEnumerable<TypeSig> enumerable = method.MethodSig.Params.Select((TypeSig type) => (type.IsClassSig && method.MethodSig.HasThis) ? module.CorLibTypes.Object : type);
		if (method.MethodSig.HasThis && !method.MethodSig.ExplicitThis)
		{
			TypeDef typeDef = method.DeclaringType.ResolveTypeDefThrow();
			enumerable = (typeDef.IsValueType ? new TypeSig[1] { typeDef.ToTypeSig() }.Concat(enumerable) : new CorLibTypeSig[1] { module.CorLibTypes.Object }.Concat(enumerable));
		}
		TypeSig typeSig = method.MethodSig.RetType;
		if (typeSig.IsClassSig)
		{
			typeSig = module.CorLibTypes.Object;
		}
		return MethodSig.CreateStatic(typeSig, enumerable.ToArray());
	}
}
