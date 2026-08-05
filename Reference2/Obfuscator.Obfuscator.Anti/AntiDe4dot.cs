using dnlib.DotNet;

namespace Obfuscator.Obfuscator.Anti;

internal class AntiDe4dot
{
	public static void Execute(AssemblyDef mod)
	{
		foreach (ModuleDef moduleDef in mod.Modules)
		{
			InterfaceImplUser item = new InterfaceImplUser(moduleDef.GlobalType);
			for (int i = 0; i < 1; i++)
			{
				TypeDefUser typeDefUser = new TypeDefUser(string.Empty, $"Form{i}", moduleDef.CorLibTypes.GetTypeRef("System", "Attribute"));
				InterfaceImplUser item2 = new InterfaceImplUser(typeDefUser);
				moduleDef.Types.Add(typeDefUser);
				typeDefUser.Interfaces.Add(item2);
				typeDefUser.Interfaces.Add(item);
			}
		}
	}
}
