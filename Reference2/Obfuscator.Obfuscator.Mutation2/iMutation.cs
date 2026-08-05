using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfuscator.Obfuscator.Mutation2;

public interface iMutation
{
	void Process(MethodDef method, ref int index);

	void Prepare(TypeDef type);

	bool Supported(Instruction instr);
}
