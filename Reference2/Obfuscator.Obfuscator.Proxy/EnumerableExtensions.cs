using System.Collections.Generic;

namespace Obfuscator.Obfuscator.Proxy;

public static class EnumerableExtensions
{
	public static T Random<T>(this IEnumerable<T> input)
	{
		return EnumerableHelper.Random(input);
	}
}
