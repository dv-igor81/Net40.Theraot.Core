using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Theraot.Collections;

namespace System.Collections.ObjectModel;

public static class ReadOnlyCollectionEx
{
	public static ReadOnlyCollectionEx<T> Create<T>(params T[] list)
	{
		return new ReadOnlyCollectionEx<T>(list);
	}
}
[Serializable]
[ComVisible(false)]
[DebuggerNonUserCode]
[DebuggerDisplay("Count={Count}")]
public class ReadOnlyCollectionEx<T> : ReadOnlyCollection<T>, IReadOnlyList<T>
{
	internal IList<T> Wrapped { get; }

	public ReadOnlyCollectionEx(IList<T> wrapped)
		: base(wrapped)
	{
		Wrapped = wrapped;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public void CopyTo(int sourceIndex, T[] array, int index, int count)
	{
		Extensions.CanCopyTo(base.Count - sourceIndex, array, count);
		Extensions.CopyTo(this, sourceIndex, array, index, count);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public void CopyTo(T[] array)
	{
		Wrapped.CopyTo(array, 0);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public void CopyTo(T[] array, int index, int count)
	{
		Extensions.CanCopyTo(array, index, count);
		Extensions.CopyTo(this, array, index, count);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public override int GetHashCode()
	{
		return this.ComputeHashCode();
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public T[] ToArray()
	{
		T[] array = new T[Wrapped.Count];
		CopyTo(array);
		return array;
	}

	// T IReadOnlyList<T>.get_Item(int index)
	// {
	// 	return base[index];
	// }
}
