using System;
using System.Diagnostics;

namespace Theraot.Core;

[DebuggerNonUserCode]
public static class ActionHelper
{
	private static class HelperNullAction
	{
		public static Action Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction()
		{
		}
	}

	private static class HelperNullAction<T>
	{
		public static Action<T> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T obj)
		{
		}
	}

	private static class HelperNullAction<T1, T2>
	{
		public static Action<T1, T2> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3>
	{
		public static Action<T1, T2, T3> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4>
	{
		public static Action<T1, T2, T3, T4> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5>
	{
		public static Action<T1, T2, T3, T4, T5> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6>
	{
		public static Action<T1, T2, T3, T4, T5, T6> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
	{
		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Instance { get; }

		static HelperNullAction()
		{
			Instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
		}
	}

	public static Action GetNoopAction()
	{
		return HelperNullAction.Instance;
	}

	public static Action<T> GetNoopAction<T>()
	{
		return HelperNullAction<T>.Instance;
	}

	public static Action<T1, T2> GetNoopAction<T1, T2>()
	{
		return HelperNullAction<T1, T2>.Instance;
	}

	public static Action<T1, T2, T3> GetNoopAction<T1, T2, T3>()
	{
		return HelperNullAction<T1, T2, T3>.Instance;
	}

	public static Action<T1, T2, T3, T4> GetNoopAction<T1, T2, T3, T4>()
	{
		return HelperNullAction<T1, T2, T3, T4>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5> GetNoopAction<T1, T2, T3, T4, T5>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6> GetNoopAction<T1, T2, T3, T4, T5, T6>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7> GetNoopAction<T1, T2, T3, T4, T5, T6, T7>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Instance;
	}
}