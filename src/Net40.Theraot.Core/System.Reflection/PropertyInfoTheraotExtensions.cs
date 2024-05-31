using System.Runtime.CompilerServices;

namespace System.Reflection;

public static class PropertyInfoTheraotExtensions
{
	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static object GetValue(this PropertyInfo info, object obj)
	{
		if (info == null)
		{
			throw new NullReferenceException();
		}
		return info.GetValue(obj, null);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void SetValue(this PropertyInfo info, object? obj, object? value)
	{
		if (info == null)
		{
			throw new NullReferenceException();
		}
		info.SetValue(obj, value, null);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static MethodInfo GetMethod(this PropertyInfo info)
	{
		return info.GetMethod(nonPublic: true);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static MethodInfo SetMethod(this PropertyInfo info)
	{
		return info.SetMethod(nonPublic: true);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static MethodInfo GetMethod(this PropertyInfo info, bool nonPublic)
	{
		if (info == null)
		{
			throw new NullReferenceException();
		}
		return info.GetGetMethod(nonPublic);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static MethodInfo SetMethod(this PropertyInfo info, bool nonPublic)
	{
		if (info == null)
		{
			throw new NullReferenceException();
		}
		return info.GetSetMethod(nonPublic);
	}
}
