using System;
using System.Diagnostics;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public sealed class NoOpDisposable : IDisposable
{
	public static IDisposable Instance { get; } = new NoOpDisposable();


	private NoOpDisposable()
	{
	}

	public void Dispose()
	{
	}
}