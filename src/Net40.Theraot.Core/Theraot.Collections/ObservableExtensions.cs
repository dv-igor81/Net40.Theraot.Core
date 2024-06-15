using System;

namespace Theraot.Collections;

public static class ObservableExtensions
{
	public static IDisposable SubscribeAction<T>(this IObservable<T> observable, Action<T> listener)
	{
		if (observable == null)
		{
			throw new ArgumentNullException("observable");
		}
		return observable.Subscribe(listener.ToObserver());
	}

	public static IDisposable SubscribeProgress<T>(this IObservable<T> observable, IProgress<T> listener)
	{
		if (observable == null)
		{
			throw new ArgumentNullException("observable");
		}
		return observable.Subscribe(listener.ToObserver());
	}

	public static IObserver<T> ToObserver<T>(this Action<T> listener)
	{
		if (listener == null)
		{
			throw new ArgumentNullException("listener");
		}
		return new CustomObserver<T>(listener);
	}

	public static IObserver<T> ToObserver<T>(this IProgress<T> listener)
	{
		if (listener == null)
		{
			throw new ArgumentNullException("listener");
		}
		return new CustomObserver<T>(listener.Report);
	}
}