using System;

namespace Theraot.Threading;

public static class ThreadUniqueId
{
	[ThreadStatic]
	private static UniqueId? _currentThreadId;

	public static UniqueId CurrentThreadId
	{
		get
		{
			if (_currentThreadId.HasValue)
			{
				return _currentThreadId.Value;
			}
			UniqueId nextId = RuntimeUniqueIdProvider.GetNextId();
			_currentThreadId = nextId;
			return nextId;
		}
	}
}
