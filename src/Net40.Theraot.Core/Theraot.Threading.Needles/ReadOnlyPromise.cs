using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public class ReadOnlyPromise : IWaitablePromise, IPromise, INotifyCompletion
{
	private readonly IWaitablePromise _promised;

	public bool IsCompleted => _promised.IsCompleted;

	public ReadOnlyPromise(IWaitablePromise promised)
	{
		_promised = promised;
	}

	public override int GetHashCode()
	{
		return _promised.GetHashCode();
	}

	public void OnCompleted(Action continuation)
	{
		_promised.OnCompleted(continuation);
	}

	public override string ToString()
	{
		return $"{{Promise: {_promised}}}";
	}

	public void Wait()
	{
		_promised.Wait();
	}
}
