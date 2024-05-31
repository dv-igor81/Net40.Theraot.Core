using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe;

[DebuggerNonUserCode]
public sealed class StrongDelegateCollection : ICollection<Delegate>, IEnumerable<Delegate>, IEnumerable
{
	private readonly Action<object?[]> _invoke;

	private readonly Action<object?[]> _invokeAndClear;

	private readonly Action<Action<Exception>?, object?[]> _invokeAndClearWithException;

	private readonly Action<Action<Exception>?, object?[]> _invokeWithException;

	private readonly ThreadSafeCollection<Delegate> _wrapped;

	public int Count => _wrapped.Count;

	bool ICollection<Delegate>.IsReadOnly => false;

	public StrongDelegateCollection(bool freeReentry)
	{
		IEqualityComparer<Delegate> @default = EqualityComparer<Delegate>.Default;
		_wrapped = new ThreadSafeCollection<Delegate>(@default);
		if (freeReentry)
		{
			_invoke = InvokeExtracted;
			_invokeAndClear = InvokeAndClearExtracted;
			_invokeWithException = InvokeExtracted;
			_invokeAndClearWithException = InvokeAndClearExtracted;
			return;
		}
		ReentryGuard guard = new ReentryGuard();
		_invoke = delegate(object?[] input)
		{
			guard.Execute(delegate
			{
				InvokeExtracted(input);
			});
		};
		_invokeAndClear = delegate(object?[] input)
		{
			guard.Execute(delegate
			{
				InvokeAndClearExtracted(input);
			});
		};
		_invokeWithException = delegate(Action<Exception>? onException, object?[] input)
		{
			Action<Exception> onException3 = onException;
			guard.Execute(delegate
			{
				InvokeExtracted(onException3, input);
			});
		};
		_invokeAndClearWithException = delegate(Action<Exception>? onException, object?[] input)
		{
			Action<Exception> onException2 = onException;
			guard.Execute(delegate
			{
				InvokeAndClearExtracted(onException2, input);
			});
		};
	}

	public void Add(Delegate item)
	{
		if ((object)item != null)
		{
			_wrapped.Add(item);
		}
	}

	public void Clear()
	{
		_wrapped.Clear();
	}

	public bool Contains(Delegate item)
	{
		return _wrapped.Contains(item);
	}

	public void CopyTo(Delegate[] array, int arrayIndex)
	{
		_wrapped.CopyTo(array, arrayIndex);
	}

	public IEnumerator<Delegate> GetEnumerator()
	{
		return _wrapped.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Invoke(Action<Exception>? onException, DelegateCollectionInvokeOptions options, params object?[] args)
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

	public void Invoke(DelegateCollectionInvokeOptions options, params object?[] args)
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

	public bool Remove(Delegate item)
	{
		return _wrapped.Remove(item);
	}

	private void InvokeAndClearExtracted(Action<Exception>? onException, object?[] args)
	{
		foreach (Delegate item in _wrapped.ClearEnumerable())
		{
			try
			{
				item.DynamicInvoke(args);
			}
			catch (Exception obj)
			{
				onException?.Invoke(obj);
			}
		}
	}

	private void InvokeAndClearExtracted(object?[] args)
	{
		foreach (Delegate item in _wrapped.ClearEnumerable())
		{
			item.DynamicInvoke(args);
		}
	}

	private void InvokeExtracted(Action<Exception>? onException, object?[] args)
	{
		foreach (Delegate item in _wrapped)
		{
			try
			{
				item.DynamicInvoke(args);
			}
			catch (Exception obj)
			{
				onException?.Invoke(obj);
			}
		}
	}

	private void InvokeExtracted(object?[] args)
	{
		foreach (Delegate item in _wrapped)
		{
			item.DynamicInvoke(args);
		}
	}
}
