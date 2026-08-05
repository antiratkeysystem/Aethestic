using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.CtrlFlow;

public class BlockParser
{
	public static List<Block> ParseMethod(MethodDef method)
	{
		List<Block> list = new List<Block>();
		Block block = new Block();
		int num = 0;
		int num2 = 0;
		block.Number = num;
		block.Instructions.Add(Instruction.Create(OpCodes.Nop));
		list.Add(block);
		block = new Block();
		Stack<ExceptionHandler> stack = new Stack<ExceptionHandler>();
		foreach (Instruction instruction in method.Body.Instructions)
		{
			foreach (ExceptionHandler exceptionHandler in method.Body.ExceptionHandlers)
			{
				if (exceptionHandler.HandlerStart == instruction || exceptionHandler.TryStart == instruction || exceptionHandler.FilterStart == instruction)
				{
					stack.Push(exceptionHandler);
				}
			}
			foreach (ExceptionHandler exceptionHandler2 in method.Body.ExceptionHandlers)
			{
				if (exceptionHandler2.HandlerEnd == instruction || exceptionHandler2.TryEnd == instruction)
				{
					stack.Pop();
				}
			}
			instruction.CalculateStackUsage(out var num3, out var num4);
			block.Instructions.Add(instruction);
			num2 += num3 - num4;
			if (num3 == 0 && instruction.OpCode != OpCodes.Nop && (num2 == 0 || instruction.OpCode == OpCodes.Ret) && stack.Count == 0)
			{
				int num5 = (block.Number = num + 1);
				num = num5;
				list.Add(block);
				block = new Block();
			}
		}
		return list;
	}
}
