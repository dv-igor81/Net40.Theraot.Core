using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public sealed class WeakDelegateNeedle : WeakNeedle<Delegate>, IEquatable<Delegate>, IEquatable<WeakDelegateNeedle>
{
	public MethodInfo Method
	{
		get
		{
			Delegate value;
			return TryGetValue(out value) ? RuntimeReflectionExtensions.GetMethodInfo(value) : null;
		}
	}

	public WeakDelegateNeedle()
	{
	}

	public WeakDelegateNeedle(Delegate handler)
		: base(handler)
	{
	}

	public bool Equals(Delegate other)
	{
		return (object)other != null && Equals(RuntimeReflectionExtensions.GetMethodInfo(other), other.Target);
	}

	public bool Equals(MethodInfo method, object target)
	{
		Delegate value;
		return TryGetValue(out value) && value.DelegateEquals(method, target);
	}

	public bool Equals(WeakDelegateNeedle other)
	{
		if ((object)other == null)
		{
			return false;
		}
		Delegate value;
		bool flag = TryGetValue(out value);
		Delegate value2;
		bool flag2 = TryGetValue(out value2);
		if (!flag)
		{
			return !flag2;
		}
		if (!flag2)
		{
			return false;
		}
		MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo(value2);
		return EqualityComparer<MethodInfo>.Default.Equals(RuntimeReflectionExtensions.GetMethodInfo(value), methodInfo) && value.Target != value2.Target;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is WeakDelegateNeedle other))
		{
			if (!(obj is INeedle<Delegate> needle))
			{
				if (obj is Delegate other2)
				{
					return Equals(other2);
				}
				return false;
			}
			if (needle.TryGetValue(out var target))
			{
				return Equals(target);
			}
			return !IsAlive;
		}
		return Equals(other);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public void Invoke(object[] args)
	{
		TryInvoke(args);
	}

	public bool TryInvoke(object[] args)
	{
		if (!TryGetValue(out var value))
		{
			return false;
		}
		value.DynamicInvoke(args);
		return true;
	}

	public bool TryInvoke(object[] args, out object result)
	{
		if (TryGetValue(out var value))
		{
			result = value.DynamicInvoke(args);
			return true;
		}
		result = null;
		return false;
	}

	public bool TryInvoke<TResult>(object[] args, out TResult result)
	{
		if (TryGetValue(out var value))
		{
			result = (TResult)value.DynamicInvoke(args);
			return true;
		}
		result = default(TResult);
		return false;
	}
}