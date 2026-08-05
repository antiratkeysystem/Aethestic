using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Obfuscator.Obfuscator.FakeAttributes;

internal class FakeAttributes
{
	public static void Execute(ModuleDef module)
	{
		foreach (string s in new List<string> { "ZYXDNGuarder", "HVMRuntm.dll" })
		{
			TypeDef typeDef = new TypeDefUser("Attributes", s, module.Import(typeof(Attribute)));
			typeDef.Attributes = TypeAttributes.NotPublic;
			module.Types.Add(typeDef);
		}
		_ = module.Types[new Random().Next(0, module.Types.Count)];
	}
}
