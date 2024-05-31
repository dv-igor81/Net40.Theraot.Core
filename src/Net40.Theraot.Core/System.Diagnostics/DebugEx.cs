#define DEBUG
using System.Runtime.CompilerServices;

namespace System.Diagnostics;

public static class DebugEx
{
	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[Conditional("DEBUG")]
	public static void Fail(string message)
	{
		Debug.Fail(message);
	}
}
