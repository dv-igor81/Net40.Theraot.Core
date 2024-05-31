using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System;

public static class ThrowHelper
{
	[DoesNotReturn]
	public static void ThrowIndexOutOfRangeException()
	{
		throw new IndexOutOfRangeException();
	}

	[DoesNotReturn]
	public static void ThrowArrayTypeMismatchException()
	{
		throw new ArrayTypeMismatchException();
	}

	[DoesNotReturn]
	public static void ThrowArgumentOutOfRangeException()
	{
		throw new ArgumentOutOfRangeException();
	}

	[DoesNotReturn]
	public static void ThrowInvalidTypeWithPointersNotSupported(Type targetType)
	{
		throw new ArgumentException("SR.Format(SR.Argument_InvalidTypeWithPointersNotSupported, targetType)");
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void ThrowForUnsupportedVectorBaseType<T>() where T : struct
	{
		if (typeof(T) != typeof(byte) && typeof(T) != typeof(sbyte) && typeof(T) != typeof(short) && typeof(T) != typeof(ushort) && typeof(T) != typeof(int) && typeof(T) != typeof(uint) && typeof(T) != typeof(long) && typeof(T) != typeof(ulong) && typeof(T) != typeof(float) && typeof(T) != typeof(double))
		{
			throw new NotSupportedException("ExceptionResource.Arg_TypeNotSupported");
		}
	}

	public static void ThrowArgumentNullException(ExceptionArgument argument)
	{
		throw GetArgumentNullException(argument);
	}

	public static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
	{
		throw GetArgumentOutOfRangeException(argument);
	}

	public static ArgumentNullException GetArgumentNullException(ExceptionArgument argument)
	{
		return new ArgumentNullException(GetArgumentName(argument));
	}

	public static ArgumentOutOfRangeException GetArgumentOutOfRangeException(ExceptionArgument argument)
	{
		return new ArgumentOutOfRangeException(GetArgumentName(argument));
	}

	[MethodImpl(MethodImplOptionsEx.NoInlining)]
	private static string GetArgumentName(ExceptionArgument argument)
	{
		return argument.ToString();
	}
}
