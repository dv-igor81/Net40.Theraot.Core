namespace System.Runtime.CompilerServices;

public static class MethodImplOptionsEx
{
	public const MethodImplOptions AggressiveInlining = (MethodImplOptions)256;

	public const MethodImplOptions ForwardRef = MethodImplOptions.ForwardRef;

	public const MethodImplOptions InternalCall = MethodImplOptions.InternalCall;

	public const MethodImplOptions NoInlining = MethodImplOptions.NoInlining;

	public const MethodImplOptions NoOptimization = MethodImplOptions.NoOptimization;

	public const MethodImplOptions PreserveSig = MethodImplOptions.PreserveSig;

	public const MethodImplOptions SecurityMitigations = (MethodImplOptions)1024;

	public const MethodImplOptions Synchronized = MethodImplOptions.Synchronized;

	public const MethodImplOptions Unmanaged = MethodImplOptions.Unmanaged;
}