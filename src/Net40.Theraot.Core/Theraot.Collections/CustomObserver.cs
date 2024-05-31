using System;
using Theraot.Core;

namespace Theraot.Collections;

public sealed class CustomObserver<T> : IObserver<T>
{
	private readonly Action _onCompleted;

	private readonly Action<Exception> _onError;

	private readonly Action<T> _onNext;

	public CustomObserver(Action onCompleted, Action<Exception> onError, Action<T> onNext)
	{
		_onCompleted = onCompleted ?? ActionHelper.GetNoopAction();
		_onError = onError ?? ActionHelper.GetNoopAction<Exception>();
		_onNext = onNext ?? ActionHelper.GetNoopAction<T>();
	}

	public CustomObserver(Action<T> onNext)
	{
		_onCompleted = ActionHelper.GetNoopAction();
		_onError = ActionHelper.GetNoopAction<Exception>();
		_onNext = onNext ?? ActionHelper.GetNoopAction<T>();
	}

	public void OnCompleted()
	{
		Action onCompleted = _onCompleted;
		onCompleted();
	}

	public void OnError(Exception error)
	{
		Action<Exception> onError = _onError;
		onError(error);
	}

	public void OnNext(T value)
	{
		Action<T> onNext = _onNext;
		onNext(value);
	}
}
