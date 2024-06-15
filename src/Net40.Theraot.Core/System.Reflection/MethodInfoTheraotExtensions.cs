using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace System.Reflection;

public static class MethodInfoTheraotExtensions
{
	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType)
	{
		if (methodInfo == null)
		{
			throw new NullReferenceException();
		}
		if (methodInfo is DynamicMethod dynamicMethod)
		{
			return dynamicMethod.CreateDelegate(delegateType);
		}
		return Delegate.CreateDelegate(delegateType, methodInfo);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object target)
	{
		if (methodInfo == null)
		{
			throw new NullReferenceException();
		}
		if (methodInfo is DynamicMethod dynamicMethod)
		{
			return dynamicMethod.CreateDelegate(delegateType, target);
		}
		return Delegate.CreateDelegate(delegateType, target, methodInfo);
	}
}