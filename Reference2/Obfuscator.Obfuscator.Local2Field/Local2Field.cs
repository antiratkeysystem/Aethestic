using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfuscator.Helper;

namespace Obfuscator.Obfuscator.Local2Field;

internal class Local2Field
{
	private static Dictionary<Local, FieldDef> _convertedLocals = new Dictionary<Local, FieldDef>();

	public static void Execute(ModuleDef moduleDef)
	{
		foreach (TypeDef item in moduleDef.Types.Where((TypeDef x) => x != moduleDef.GlobalType))
		{
			foreach (MethodDef meth in item.Methods.Where((MethodDef x) => x.HasBody && x.Body.HasInstructions && !x.IsConstructor))
			{
				_convertedLocals = new Dictionary<Local, FieldDef>();
				Process(moduleDef, meth);
			}
		}
	}

	private static void Process(ModuleDef module, MethodDef meth)
	{
		meth.Body.SimplifyMacros(meth.Parameters);
		foreach (Instruction instruction in meth.Body.Instructions)
		{
			if (instruction.Operand is Local local)
			{
				FieldDef fieldDef;
				if (!_convertedLocals.ContainsKey(local))
				{
					fieldDef = new FieldDefUser(Methods.GenerateString(), new FieldSig(local.Type), FieldAttributes.Public | FieldAttributes.Static);
					module.GlobalType.Fields.Add(fieldDef);
					_convertedLocals.Add(local, fieldDef);
				}
				else
				{
					fieldDef = _convertedLocals[local];
				}
				switch (instruction.OpCode?.Code)
				{
				case Code.Ldloc:
					instruction.OpCode = OpCodes.Ldsfld;
					break;
				case Code.Ldloca:
					instruction.OpCode = OpCodes.Ldsflda;
					break;
				case Code.Stloc:
					instruction.OpCode = OpCodes.Stsfld;
					break;
				default:
					instruction.OpCode = null;
					break;
				}
				instruction.Operand = fieldDef;
			}
		}
		_convertedLocals.ToList().ForEach(delegate(KeyValuePair<Local, FieldDef> x)
		{
			meth.Body.Variables.Remove(x.Key);
		});
		_convertedLocals = new Dictionary<Local, FieldDef>();
	}
}
