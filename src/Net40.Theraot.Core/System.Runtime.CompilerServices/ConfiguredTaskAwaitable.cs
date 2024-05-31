using System.Security;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

public readonly struct ConfiguredTaskAwaitable<TResult>
{
	public readonly struct ConfiguredTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
	{
		private readonly bool _continueOnCapturedContext;

		private readonly Task<TResult> _task;

		public bool IsCompleted => _task.IsCompleted;

		internal ConfiguredTaskAwaiter(Task<TResult> task, bool continueOnCapturedContext)
		{
			_task = task;
			_continueOnCapturedContext = continueOnCapturedContext;
		}

		public TResult GetResult()
		{
			TaskAwaiter.ValidateEnd(_task);
			return _task.Result;
		}

		public void OnCompleted(Action continuation)
		{
			TaskAwaiter.OnCompletedInternal(_task, continuation, _continueOnCapturedContext);
		}

		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation)
		{
			TaskAwaiter.OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
		}
	}

	private readonly ConfiguredTaskAwaiter _configuredTaskAwaiter;

	internal ConfiguredTaskAwaitable(Task<TResult> task, bool continueOnCapturedContext)
	{
		_configuredTaskAwaiter = new ConfiguredTaskAwaiter(task, continueOnCapturedContext);
	}

	public ConfiguredTaskAwaiter GetAwaiter()
	{
		return _configuredTaskAwaiter;
	}
}
public readonly struct ConfiguredTaskAwaitable
{
	public readonly struct ConfiguredTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
	{
		private readonly bool _continueOnCapturedContext;

		private readonly Task _task;

		public bool IsCompleted => _task.IsCompleted;

		internal ConfiguredTaskAwaiter(Task task, bool continueOnCapturedContext)
		{
			_task = task;
			_continueOnCapturedContext = continueOnCapturedContext;
		}

		public void GetResult()
		{
			TaskAwaiter.ValidateEnd(_task);
		}

		public void OnCompleted(Action continuation)
		{
			TaskAwaiter.OnCompletedInternal(_task, continuation, _continueOnCapturedContext);
		}

		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation)
		{
			TaskAwaiter.OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
		}
	}

	private readonly ConfiguredTaskAwaiter _configuredTaskAwaiter;

	internal ConfiguredTaskAwaitable(Task task, bool continueOnCapturedContext)
	{
		_configuredTaskAwaiter = new ConfiguredTaskAwaiter(task, continueOnCapturedContext);
	}

	public ConfiguredTaskAwaiter GetAwaiter()
	{
		return _configuredTaskAwaiter;
	}
}
