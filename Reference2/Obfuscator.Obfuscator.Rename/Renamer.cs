using System.Collections.Generic;
using dnlib.DotNet;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Rename;

internal class Renamer
{
	private static readonly Dictionary<TypeDef, bool> TypeRename = new Dictionary<TypeDef, bool>();

	private static readonly List<string> TypeNewName = new List<string>();

	private static readonly Dictionary<MethodDef, bool> MethodRename = new Dictionary<MethodDef, bool>();

	private static readonly List<string> MethodNewName = new List<string>();

	private static readonly Dictionary<FieldDef, bool> FieldRename = new Dictionary<FieldDef, bool>();

	private static readonly List<string> FieldNewName = new List<string>();

	private static int RenameTypes;

	private static int RenameFields;

	private static int RenameMethods;

	private static int RenameNameSpaces;

	public static void Execute(ModuleDefMD module)
	{
		RenameTypes = 0;
		RenameFields = 0;
		RenameMethods = 0;
		RenameNameSpaces = 1;
		string s = Methods.GenerateString();
		foreach (TypeDef typeDef in module.Types)
		{
			if (typeDef.Name == "FakeError")
			{
				continue;
			}
			if (Methods.GenerateBool(4))
			{
				s = Methods.GenerateString();
				RenameNameSpaces++;
			}
			if (TypeRename.TryGetValue(typeDef, out var flag))
			{
				if (flag)
				{
					InternalRename(typeDef);
				}
			}
			else
			{
				InternalRename(typeDef);
			}
			typeDef.Namespace = s;
			foreach (TypeDef typeDef2 in typeDef.NestedTypes)
			{
				if (TypeRename.TryGetValue(typeDef2, out var flag2))
				{
					if (flag2)
					{
						InternalRename(typeDef2);
					}
				}
				else
				{
					InternalRename(typeDef2);
				}
				foreach (MethodDef method in typeDef2.Methods)
				{
					foreach (ParamDef paramDef in method.ParamDefs)
					{
						paramDef.Name = Methods.GenerateString(16);
					}
				}
			}
			foreach (MethodDef methodDef2 in typeDef.Methods)
			{
				if (MethodRename.TryGetValue(methodDef2, out var flag3))
				{
					if (flag3 && !methodDef2.IsConstructor && !methodDef2.IsSpecialName)
					{
						InternalRename(methodDef2);
					}
				}
				else if (!methodDef2.IsConstructor && !methodDef2.IsSpecialName)
				{
					InternalRename(methodDef2);
				}
			}
			MethodNewName.Clear();
			foreach (FieldDef fieldDef in typeDef.Fields)
			{
				if (FieldRename.TryGetValue(fieldDef, out var flag4))
				{
					if (flag4)
					{
						InternalRename(fieldDef);
					}
				}
				else
				{
					InternalRename(fieldDef);
				}
			}
			FieldNewName.Clear();
		}
		TypeRename.Clear();
		MethodRename.Clear();
		FieldRename.Clear();
	}

	public static string Info()
	{
		return $"Rename Obfuscator [Types: [{RenameTypes}]  Namespaces: [{RenameNameSpaces}]  Methods: [{RenameMethods}]  Fields: [{RenameFields}]]";
	}

	private static void InternalRename(TypeDef type)
	{
		string text = Methods.GenerateString();
		while (TypeNewName.Contains(text))
		{
			text = Methods.GenerateString();
		}
		TypeNewName.Add(text);
		type.Name = text;
		RenameTypes++;
	}

	private static void InternalRename(MethodDef method)
	{
		string text = Methods.GenerateString();
		while (MethodNewName.Contains(text))
		{
			text = Methods.GenerateString();
		}
		MethodNewName.Add(text);
		method.Name = text;
		RenameMethods++;
	}

	private static void InternalRename(FieldDef field)
	{
		string text = Methods.GenerateString();
		while (FieldNewName.Contains(text))
		{
			text = Methods.GenerateString();
		}
		FieldNewName.Add(text);
		field.Name = text;
		RenameFields++;
	}
}
