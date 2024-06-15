using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Theraot.Collections;

[DebuggerNonUserCode]
public sealed class EmptyCollection<T> : ReadOnlyCollectionEx<T>
{
	public static EmptyCollection<T> Instance { get; } = new EmptyCollection<T>();


	private EmptyCollection()
		: base((IList<T>)ArrayEx.Empty<T>())
	{
	}
}