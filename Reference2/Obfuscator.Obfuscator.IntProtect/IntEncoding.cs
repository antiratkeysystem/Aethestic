using System;
using System.Security.Cryptography;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.IntProtect;

internal static class IntEncoding
{
	private static readonly RandomNumberGenerator csp = RandomNumberGenerator.Create();

	public static int Next(int minValue = 0, int maxValue = int.MaxValue)
	{
		if (minValue >= maxValue)
		{
			throw new ArgumentOutOfRangeException("minValue");
		}
		long num = (long)maxValue - (long)minValue;
		long num2 = (long)(ulong.MaxValue / (ulong)num) * num;
		uint num3;
		do
		{
			num3 = RandomUInt();
		}
		while ((ulong)num3 >= (ulong)num2);
		return (int)((long)minValue + (long)((ulong)num3 % (ulong)num));
	}

	public static string GenerateRandomString(int length)
	{
		byte[] bytes = RandomBytes(length);
		return Encoding.UTF7.GetString(bytes).Replace("\0", ".").Replace("\n", ".")
			.Replace("\r", ".");
	}

	public static int GetRandomStringLength()
	{
		return Next(30, 120);
	}

	public static int GetRandomInt32()
	{
		return BitConverter.ToInt32(RandomBytes(4), 0);
	}

	private static uint RandomUInt()
	{
		return BitConverter.ToUInt32(RandomBytes(4), 0);
	}

	private static byte[] RandomBytes(int bytes)
	{
		byte[] array = new byte[bytes];
		csp.GetBytes(array);
		return array;
	}

	public static void Execute(ModuleDef moduleDef)
	{
		IMethod method = moduleDef.Import(typeof(Math).GetMethod("Abs", new Type[1] { typeof(int) }));
		IMethod method2 = moduleDef.Import(typeof(Math).GetMethod("Min", new Type[2]
		{
			typeof(int),
			typeof(int)
		}));
		foreach (TypeDef typeDef in moduleDef.Types)
		{
			if (typeDef.IsGlobalModuleType)
			{
				continue;
			}
			foreach (MethodDef methodDef in typeDef.Methods)
			{
				if (!methodDef.HasBody)
				{
					continue;
				}
				for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
				{
					if (methodDef.Body.Instructions[i].Operand is int num && num > 0)
					{
						methodDef.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(method));
						int num2 = Next(8, GetRandomStringLength());
						if (num2 % 2 != 0)
						{
							num2++;
						}
						for (int j = 0; j < num2; j++)
						{
							methodDef.Body.Instructions.Insert(i + j + 1, Instruction.Create(OpCodes.Neg));
						}
						if (num < int.MaxValue)
						{
							methodDef.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(int.MaxValue));
							methodDef.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(method2));
						}
						i += num2 + 2;
					}
				}
				methodDef.Body.SimplifyBranches();
			}
		}
	}
}
