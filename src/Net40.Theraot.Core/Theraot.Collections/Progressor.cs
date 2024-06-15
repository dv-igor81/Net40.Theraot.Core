using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections;

[DebuggerTypeProxy(typeof(ProgressorProxy))]
public sealed class Progressor<T> : IObservable<T>, IEnumerable<T>, IEnumerable, IClosable
{
	private ProxyObservable<T>? _proxy;

	private TryTake<T>? _tryTake;

	public bool IsClosed => Volatile.Read(ref _tryTake) == null;

	private Progressor(ProxyObservable<T> proxy, TryTake<T> tryTake)
	{
		_proxy = proxy;
		_tryTake = tryTake;
	}

	public static Progressor<T> CreateFromArray(T[] array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		int index = -1;
		ProxyObservable<T> proxy = new ProxyObservable<T>();
		return new Progressor<T>(proxy, delegate(out T value)
		{
			return Take(out value);
		});
		bool Take(out T value)
		{
			value = default(T);
			int num = Interlocked.Increment(ref index);
			if (num >= array.Length)
			{
				return false;
			}
			value = array[num];
			return true;
		}
	}

	public static Progressor<T> CreateFromIEnumerable(IEnumerable<T> enumerable)
	{
		if (enumerable == null)
		{
			throw new ArgumentNullException("enumerable");
		}
		if (!(enumerable is T[] array))
		{
			if (!(enumerable is IList<T> list))
			{
				if (enumerable is IReadOnlyList<T> list2)
				{
					return CreateFromIReadOnlyList(list2);
				}
				IEnumerator<T> enumerator = enumerable.GetEnumerator();
				return CreateFromIEnumerableExtracted(enumerator);
			}
			return CreateFromIList(list);
		}
		return CreateFromArray(array);
	}

	public static Progressor<T> CreateFromIList(IList<T> list)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		int index = -1;
		ProxyObservable<T> proxy = new ProxyObservable<T>();
		return new Progressor<T>(proxy, delegate(out T value)
		{
			return Take(out value);
		});
		bool Take(out T value)
		{
			value = default(T);
			int num = Interlocked.Increment(ref index);
			if (num >= list.Count)
			{
				return false;
			}
			value = list[num];
			return true;
		}
	}

	public static Progressor<T> CreateFromIObservable(IObservable<T> observable, Action? exhaustedCallback = null, CancellationToken token = default(CancellationToken))
	{
		Action exhaustedCallback2 = exhaustedCallback;
		if (observable == null)
		{
			throw new ArgumentNullException("observable");
		}
		if (exhaustedCallback2 == null)
		{
			exhaustedCallback2 = ActionHelper.GetNoopAction();
		}
		ThreadSafeQueue<T> buffer = new ThreadSafeQueue<T>();
		SemaphoreSlim semaphore = new SemaphoreSlim(0);
		CancellationTokenSource source = new CancellationTokenSource();
		IDisposable?[] subscription = new IDisposable[1] { observable.Subscribe(new CustomObserver<T>(source.Cancel, OnError, OnNext)) };
		ProxyObservable<T> proxy = new ProxyObservable<T>();
		TryTake<T>[] tryTake = new TryTake<T>[1]
		{
			delegate(out T val)
			{
				val = default(T);
				return false;
			}
		};
		tryTake[0] = TakeInitial;
		return new Progressor<T>(proxy, Take);
		void OnError(Exception _)
		{
			source.Cancel();
		}
		void OnNext(T item)
		{
			buffer.Add(item);
			semaphore.Release();
		}
		bool Take(out T value)
		{
			return tryTake[0](out value);
		}
		bool TakeInitial(out T value)
		{
			if (source.IsCancellationRequested || token.IsCancellationRequested)
			{
				if (Interlocked.CompareExchange(ref tryTake[0], delegate(out T value1)
				{
					return TakeReplacement(out value1);
				}, tryTake[0]) == tryTake[0])
				{
					Interlocked.Exchange(ref subscription[0], null)?.Dispose();
					semaphore.Dispose();
					source.Dispose();
				}
			}
			else if (exhaustedCallback2 != null)
			{
				SpinWait spinWait = default(SpinWait);
				while (semaphore.CurrentCount == 0 && !source.IsCancellationRequested && !token.IsCancellationRequested)
				{
					exhaustedCallback2();
					spinWait.SpinOnce();
				}
			}
			if (source.IsCancellationRequested || token.IsCancellationRequested)
			{
				return TakeReplacement(out value);
			}
			try
			{
				semaphore.Wait(source.Token);
			}
			catch (OperationCanceledException)
			{
			}
			return TakeReplacement(out value);
		}
		bool TakeReplacement(out T value)
		{
			if (buffer.TryTake(out value))
			{
				return true;
			}
			value = default(T);
			return false;
		}
	}

	public static Progressor<T> CreateFromIReadOnlyList(IReadOnlyList<T> list)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		int index = -1;
		ProxyObservable<T> proxy = new ProxyObservable<T>();
		return new Progressor<T>(proxy, delegate(out T value)
		{
			return Take(out value);
		});
		bool Take(out T value)
		{
			value = default(T);
			int num = Interlocked.Increment(ref index);
			if (num >= list.Count)
			{
				return false;
			}
			value = list[num];
			return true;
		}
	}

	public void Close()
	{
		Volatile.Write(ref _tryTake, null);
		Interlocked.Exchange(ref _proxy, null)?.OnCompleted();
	}

	public IEnumerator<T> GetEnumerator()
	{
		T item;
		while (TryTake(out item))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IDisposable Subscribe(IObserver<T> observer)
	{
		ProxyObservable<T> proxyObservable = Volatile.Read(ref _proxy);
		if (proxyObservable != null)
		{
			return proxyObservable.Subscribe(observer);
		}
		observer?.OnCompleted();
		return NoOpDisposable.Instance;
	}

	public bool TryTake(out T item)
	{
		TryTake<T> tryTake = Volatile.Read(ref _tryTake);
		ProxyObservable<T> proxyObservable = Volatile.Read(ref _proxy);
		if (tryTake != null && proxyObservable != null)
		{
			if (tryTake(out item))
			{
				proxyObservable.OnNext(item);
				return true;
			}
			Close();
		}
		item = default(T);
		return false;
	}

	public IEnumerable<T> While(Func<bool> condition)
	{
		if (condition == null)
		{
			throw new ArgumentNullException("condition");
		}
		return WhileExtracted();
		IEnumerable<T> WhileExtracted()
		{
			while (true)
			{
				TryTake<T> tryTake = Volatile.Read(ref _tryTake);
				if (tryTake == null || !tryTake(out var item) || !condition())
				{
					break;
				}
				yield return item;
				item = default(T);
			}
		}
	}

	private static Progressor<T> CreateFromIEnumerableExtracted(IEnumerator<T> enumerator)
	{
		ProxyObservable<T> proxy = new ProxyObservable<T>();
		return new Progressor<T>(proxy, delegate(out T value)
		{
			return Take(new IEnumerator<T>[1] { enumerator }, out value);
		});
		static bool Take(IEnumerator<T>?[] enumeratorBox, out T value)
		{
			IEnumerator<T> enumerator2 = Volatile.Read(ref enumeratorBox[0]);
			if (enumerator2 != null)
			{
				lock (enumerator2)
				{
					if (enumerator2 == Volatile.Read(ref enumeratorBox[0]))
					{
						if (enumerator2.MoveNext())
						{
							value = enumerator2.Current;
							return true;
						}
						Interlocked.Exchange(ref enumeratorBox[0], null)?.Dispose();
					}
				}
			}
			value = default(T);
			return false;
		}
	}
}