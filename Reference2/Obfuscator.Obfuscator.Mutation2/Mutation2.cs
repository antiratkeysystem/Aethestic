using System.Collections.Generic;
using dnlib.DotNet;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Mutation2;

public class Mutation2
{
	private static List<iMutation> Tasks = new List<iMutation>
	{
		new Abs(),
		new VariableMutation(),
		new ComparerMutation(),
		new MulToShift()
	};

	public static void Execute(ModuleDef Module)
	{
		foreach (TypeDef typeDef in Module.Types)
		{
			foreach (iMutation task in Tasks)
			{
				task.Prepare(typeDef);
			}
			for (int i = 0; i < typeDef.Methods.Count; i++)
			{
				MethodDef methodDef = typeDef.Methods[i];
				if (!methodDef.HasBody || methodDef.IsConstructor)
				{
					continue;
				}
				methodDef.Body.SimplifyBranches();
				for (int j = 0; j < methodDef.Body.Instructions.Count; j++)
				{
					iMutation iMutation2 = Tasks[Methods.Random.Next(Tasks.Count)];
					if (iMutation2.Supported(methodDef.Body.Instructions[j]))
					{
						iMutation2.Process(methodDef, ref j);
					}
				}
			}
		}
	}
}
