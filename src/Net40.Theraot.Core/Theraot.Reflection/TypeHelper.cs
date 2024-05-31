using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Theraot.Reflection;

[DebuggerNonUserCode]
public static class TypeHelper
{
	public static MethodInfo GetDelegateMethodInfo(Type delegateType)
	{
		if (delegateType == null)
		{
			throw new ArgumentNullException("delegateType");
		}
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(delegateType);
		if (typeInfo.BaseType != typeof(MulticastDelegate))
		{
			throw new ArgumentException("Not a delegate.", "delegateType");
		}
		MethodInfo methodInfo = RuntimeReflectionExtensions.GetRuntimeMethods(delegateType).FirstOrDefault((MethodInfo info) => string.Equals(info.Name, "Invoke", StringComparison.Ordinal));
		if (methodInfo == null)
		{
			throw new ArgumentException("Not a delegate.", "delegateType");
		}
		return methodInfo;
	}

	public static ParameterInfo[] GetDelegateParameters(Type delegateType)
	{
		return GetDelegateMethodInfo(delegateType).GetParameters();
	}

	public static Type GetDelegateReturnType(Type delegateType)
	{
		return GetDelegateMethodInfo(delegateType).ReturnType;
	}

	public static TTarget As<TTarget>(object source) where TTarget : class
	{
		return As(source, (Func<TTarget>)delegate
		{
			throw new InvalidOperationException("Cannot convert to " + typeof(TTarget).Name);
		});
	}

	public static TTarget As<TTarget>(object source, Func<TTarget> alternative) where TTarget : class
	{
		if (alternative == null)
		{
			throw new ArgumentNullException("alternative");
		}
		return (source is TTarget val) ? val : alternative();
	}

	public static TTarget As<TTarget>(object source, TTarget def) where TTarget : class
	{
		return As(source, () => def);
	}

	public static TTarget Cast<TTarget>(object source)
	{
		return Cast(source, (Func<TTarget>)delegate
		{
			throw new InvalidOperationException("Cannot convert to " + typeof(TTarget).Name);
		});
	}

	public static TTarget Cast<TTarget>(object source, Func<TTarget> alternative)
	{
		if (alternative == null)
		{
			throw new ArgumentNullException("alternative");
		}
		try
		{
			return (TTarget)source;
		}
		catch (Exception)
		{
			return alternative();
		}
	}

	public static TTarget Cast<TTarget>(object source, TTarget def)
	{
		return Cast(source, () => def);
	}

	public static MethodInfo? FindConversionOperator(IEnumerable<MethodInfo> methods, Type typeFrom, Type typeTo, bool implicitOnly)
	{
		return (from method in methods
			where string.Equals(method.Name, "op_Implicit", StringComparison.Ordinal) || (!implicitOnly && string.Equals(method.Name, "op_Explicit", StringComparison.Ordinal))
			where method.ReturnType == typeTo
			let parameters = method.GetParameters()
			where parameters[0].ParameterType == typeFrom
			select method).FirstOrDefault();
	}

	public static bool IsImplicitBoxingConversion(Type source, Type target)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(source);
		if (typeInfo.IsValueType && (target == typeof(object) || target == typeof(ValueType)))
		{
			return true;
		}
		return typeInfo.IsEnum && target == typeof(Enum);
	}

	public static bool IsImplicitNumericConversion(Type source, Type target)
	{
		if (source == typeof(sbyte))
		{
			if (target == typeof(short) || target == typeof(int) || target == typeof(long) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(byte))
		{
			if (target == typeof(short) || target == typeof(ushort) || target == typeof(int) || target == typeof(uint) || target == typeof(long) || target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(short))
		{
			if (target == typeof(int) || target == typeof(long) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(ushort))
		{
			if (target == typeof(int) || target == typeof(uint) || target == typeof(long) || target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(int))
		{
			if (target == typeof(long) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(uint))
		{
			if (target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(long) || target == typeof(ulong))
		{
			if (target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(char))
		{
			if (target == typeof(ushort) || target == typeof(int) || target == typeof(uint) || target == typeof(long) || target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(float))
		{
			return target == typeof(double);
		}
		return false;
	}

	[return: NotNull]
	public static T LazyCreate<T>([NotNull] ref T? target) where T : class
	{
		T val = target;
		if (val != null)
		{
			T val2 = val;
			if (true)
			{
				target = val2;
				return val2;
			}
		}
		T val3;
		try
		{
			val3 = Activator.CreateInstance<T>();
		}
		catch
		{
			throw new MissingMemberException("The type being lazily initialized does not have a public, parameterless constructor.");
		}
		val = Interlocked.CompareExchange(ref target, val3, null);
		return val ?? val3;
	}

	[return: NotNull]
	public static T LazyCreate<T>([NotNull] ref T? target, Func<T> valueFactory) where T : class
	{
		T val = target;
		if (val != null)
		{
			T val2 = val;
			if (true)
			{
				target = val2;
				return val2;
			}
		}
		if (valueFactory == null)
		{
			throw new ArgumentNullException("valueFactory");
		}
		T val3 = valueFactory();
		if (val3 == null)
		{
			throw new InvalidOperationException("valueFactory returned null");
		}
		val = Interlocked.CompareExchange(ref target, val3, null);
		return val ?? val3;
	}

	public static T LazyCreate<T>([NotNull] ref T? target, Func<T> valueFactory, object syncRoot) where T : class
	{
		T val = target;
		if (val != null)
		{
			T val2 = val;
			if (true)
			{
				target = val2;
				return val2;
			}
		}
		lock (syncRoot)
		{
			return LazyCreate(ref target, valueFactory);
		}
	}

	[return: NotNull]
	public static T LazyCreate<T>([NotNull] ref T? target, object syncRoot) where T : class
	{
		T val = target;
		if (val != null)
		{
			T val2 = val;
			if (true)
			{
				target = val2;
				return val2;
			}
		}
		lock (syncRoot)
		{
			return LazyCreate(ref target);
		}
	}

	[return: NotNull]
	public static T LazyCreateNew<T>([NotNull] ref T? target) where T : class, new()
	{
		T val = target;
		if (val != null)
		{
			T val2 = val;
			if (true)
			{
				target = val2;
				return val2;
			}
		}
		T val3 = new T();
		val = Interlocked.CompareExchange(ref target, val3, null);
		return val ?? val3;
	}

	[return: NotNull]
	public static T LazyCreateNew<T>([NotNull] ref T? target, object syncRoot) where T : class, new()
	{
		T val = target;
		if (val != null)
		{
			T val2 = val;
			if (true)
			{
				target = val2;
				return val2;
			}
		}
		lock (syncRoot)
		{
			return LazyCreateNew(ref target);
		}
	}
}
