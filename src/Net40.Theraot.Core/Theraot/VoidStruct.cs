using System;
using System.Runtime.InteropServices;

namespace Theraot;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct VoidStruct : IEquatable<VoidStruct>
{
	public static bool operator !=(VoidStruct left, VoidStruct right)
	{
		return false;
	}

	public static bool operator ==(VoidStruct left, VoidStruct right)
	{
		return true;
	}

	public bool Equals(VoidStruct other)
	{
		return true;
	}

	public override bool Equals(object? obj)
	{
		return obj is VoidStruct;
	}

	public override int GetHashCode()
	{
		return 0;
	}
}
