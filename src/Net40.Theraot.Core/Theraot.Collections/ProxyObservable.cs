using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace Theraot.Collections;

public sealed class ProxyObservable<T> : IObservable<T>, IObserver<T>
{
	private readonly Bucket<IObserver<T>> _observers;

	private int _index;

	public ProxyObservable()
	{
		_observers = new Bucket<IObserver<T>>();
		_index = -1;
	}

	public void OnCompleted()
	{
		foreach (IObserver<T> observer in _observers)
		{
			observer.OnCompleted();
		}
	}

	public void OnError(Exception error)
	{
		foreach (IObserver<T> observer in _observers)
		{
			observer.OnError(error);
		}
	}

	public void OnNext(T value)
	{
		foreach (IObserver<T> observer in _observers)
		{
			observer.OnNext(value);
		}
	}

	public IDisposable Subscribe(IObserver<T> observer)
	{
		int index = Interlocked.Increment(ref _index);
		_observers.Insert(index, observer);
		return Disposable.Create(delegate
		{
			_observers.RemoveAt(index);
		});
	}
}
