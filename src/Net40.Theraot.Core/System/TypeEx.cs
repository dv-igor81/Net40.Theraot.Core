using System.Runtime.CompilerServices;

namespace System;

public static class TypeEx
{
	public static readonly Type[] EmptyTypes = Type.EmptyTypes;

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static TypeCode GetTypeCode(Type type)
	{
		return Type.GetTypeCode(type);
	}
}
