using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

[Serializable]
[DebuggerNonUserCode]
[DebuggerDisplay("Count={Count}")]
public sealed class ListEx<T> : List<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
{
	public ListEx()
	{
	}

	public ListEx(IEnumerable<T> collection)
		: base(collection)
	{
	}

	public ListEx(int capacity)
		: base(capacity)
	{
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public void CopyTo(T[] array, int arrayIndex, int count)
	{
		CopyTo(0, array, arrayIndex, count);
	}

	public new ReadOnlyCollectionEx<T> AsReadOnly()
	{
		return new ReadOnlyCollectionEx<T>(this);
	}
}
