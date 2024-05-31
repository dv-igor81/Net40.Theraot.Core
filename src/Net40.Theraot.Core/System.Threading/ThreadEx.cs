using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Threading;

public static class ThreadEx
{
	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void MemoryBarrier()
	{
		Thread.MemoryBarrier();
	}

	[MethodImpl(MethodImplOptionsEx.NoInlining)]
	public static bool VolatileRead(ref bool address)
	{
		bool result = address;
		Thread.MemoryBarrier();
		return result;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static sbyte VolatileRead(ref sbyte address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static byte VolatileRead(ref byte address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileRead(ref short address)
	{
		Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ushort VolatileRead(ref ushort address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static int VolatileRead(ref int address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint VolatileRead(ref uint address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static long VolatileRead(ref long address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ulong VolatileRead(ref ulong address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static IntPtr VolatileRead(ref IntPtr address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static UIntPtr VolatileRead(ref UIntPtr address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static float VolatileRead(ref float address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static double VolatileRead(ref double address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static T VolatileRead<T>(ref T address) where T : class?
	{
		T result = address;
		Thread.MemoryBarrier();
		return result;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[return: NotNullIfNotNull("address")]
	public static object? VolatileRead(ref object? address)
	{
		return Thread.VolatileRead(ref address);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void VolatileWrite(ref bool address, bool value)
	{
		GC.KeepAlive(address);
		Thread.MemoryBarrier();
		address = value;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref sbyte address, sbyte value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileWrite(ref byte address, byte value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileWrite(ref short address, short value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref ushort address, ushort value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileWrite(ref int address, int value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref uint address, uint value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileWrite(ref long address, long value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref ulong address, ulong value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileWrite(ref IntPtr address, IntPtr value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref UIntPtr address, UIntPtr value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileWrite(ref float address, float value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileWrite(ref double address, double value)
	{
		Thread.VolatileWrite(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileWrite<T>(ref T address, T value) where T : class?
	{
		Interlocked.Exchange(ref address, value);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static void VolatileWrite(ref object? address, object? value)
	{
		Thread.VolatileWrite(ref address, value);
	}
}
