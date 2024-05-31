using System.Runtime.CompilerServices;

namespace System.IO;

public static class PathInternalEx
{
	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	internal static bool IsDirectorySeparator(char c)
	{
		if (c != '\\')
		{
			return c == '/';
		}
		return true;
	}
}
