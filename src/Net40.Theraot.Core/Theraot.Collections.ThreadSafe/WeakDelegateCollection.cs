using System;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe;

[DebuggerNonUserCode]
public sealed class WeakDelegateCollection : WeakCollection<Delegate, WeakDelegateNeedle>
{
	private readonly Action<object[]> _invoke;

	private readonly Action<object[]> _invokeAndClear;

	private readonly Action<Action<Exception>, object[]> _invokeAndClearWithException;

	private readonly Action<Action<Exception>, object[]> _invokeWithException;

	public WeakDelegateCollection(bool autoRemoveDeadItems, bool freeReentry)
		: base(autoRemoveDeadItems)
	{
		if (freeReentry)
		{
			_invoke = InvokeExtracted;
			_invokeAndClear = InvokeAndClearExtracted;
			_invokeWithException = InvokeExtracted;
			_invokeAndClearWithException = InvokeAndClearExtracted;
			return;
		}
		ReentryGuard guard = new ReentryGuard();
		_invoke = delegate(object[] input)
		{
			guard.Execute(delegate
			{
				InvokeExtracted(input);
			});
		};
		_invokeAndClear = delegate(object[] input)
		{
			guard.Execute(delegate
			{
				InvokeAndClearExtracted(input);
			});
		};
		_invokeWithException = delegate(Action<Exception> onException, object[] input)
		{
			guard.Execute(delegate
			{
				InvokeExtracted(onException, input);
			});
		};
		_invokeAndClearWithException = delegate(Action<Exception> onException, object[] input)
		{
			guard.Execute(delegate
			{
				InvokeAndClearExtracted(onException, input);
			});
		};
	}

	public void Invoke(Action<Exception> onException, DelegateCollectionInvokeOptions options, params object[] args)
	{
		if ((options & DelegateCollectionInvokeOptions.RemoveDelegates) != 0)
		{
			_invokeAndClearWithException(onException, args);
		}
		else
		{
			_invokeWithException(onException, args);
		}
	}

	public void Invoke(DelegateCollectionInvokeOptions options, params object[] args)
	{
		if ((options & DelegateCollectionInvokeOptions.RemoveDelegates) != 0)
		{
			_invokeAndClear(args);
		}
		else
		{
			_invoke(args);
		}
	}

	private void InvokeAndClearExtracted(Action<Exception> onException, object[] args)
	{
		foreach (Delegate item in ClearEnumerable())
		{
			try
			{
				item?.DynamicInvoke(args);
			}
			catch (Exception obj)
			{
				onException?.Invoke(obj);
			}
		}
	}

	private void InvokeAndClearExtracted(object[] args)
	{
		foreach (Delegate item in ClearEnumerable())
		{
			item?.DynamicInvoke(args);
		}
	}

	private void InvokeExtracted(Action<Exception> onException, object[] args)
	{
		using IEnumerator<Delegate> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Delegate current = enumerator.Current;
			try
			{
				current?.DynamicInvoke(args);
			}
			catch (Exception obj)
			{
				onException?.Invoke(obj);
			}
		}
	}

	private void InvokeExtracted(object[] args)
	{
		using IEnumerator<Delegate> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current?.DynamicInvoke(args);
		}
	}
}