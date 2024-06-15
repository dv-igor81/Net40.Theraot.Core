using System.Diagnostics;
using System.Security;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

internal struct AsyncMethodBuilderCore
{
    private sealed class MoveNextRunner
    {
        internal IAsyncStateMachine StateMachine;

        [SecurityCritical] private static ContextCallback _invokeMoveNext;

        private readonly ExecutionContext _context;

        [SecurityCritical]
        internal MoveNextRunner(ExecutionContext context)
        {
            _context = context;
        }

        [SecuritySafeCritical]
        internal void Run()
        {
            if (_context == null)
            {
                StateMachine.MoveNext();
                return;
            }

            ContextCallback invokeMoveNext = GetInvokeMoveNext();
            ExecutionContext.Run(_context, invokeMoveNext, StateMachine);
        }

        private static ContextCallback GetInvokeMoveNext()
        {
            ContextCallback invokeMoveNext = _invokeMoveNext;
            if (invokeMoveNext != null)
            {
                return invokeMoveNext;
            }

            return _invokeMoveNext = InvokeMoveNext;

            static void InvokeMoveNext(object stateMachine)
            {
                ((IAsyncStateMachine)stateMachine).MoveNext();
            }
        }
    }

    internal IAsyncStateMachine StateMachine;

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        if (StateMachine != null)
        {
            throw new InvalidOperationException("The builder was not properly initialized.");
        }

        StateMachine = stateMachine ?? throw new ArgumentNullException("stateMachine");
    }

    [SecuritySafeCritical]
    [DebuggerStepThrough]
    internal static void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        if (stateMachine == null)
        {
            throw new ArgumentNullException("stateMachine");
        }

        stateMachine.MoveNext();
    }

    internal static void ThrowOnContext(Exception exception, SynchronizationContext targetContext)
    {
        if (targetContext != null)
        {
            try
            {
                targetContext.Post(
                    delegate(object state) { throw TaskAwaiter.PrepareExceptionForRethrow((Exception)state); },
                    exception);
                return;
            }
            catch (Exception ex)
            {
                exception = new AggregateException(exception, ex);
            }
        }

        ThreadPool.QueueUserWorkItem(
            delegate(object state) { throw TaskAwaiter.PrepareExceptionForRethrow((Exception)state); }, exception);
    }

    [SecuritySafeCritical]
    internal Action GetCompletionAction<TMethodBuilder, TStateMachine>(ref TMethodBuilder builder,
        ref TStateMachine stateMachine) where TMethodBuilder : IAsyncMethodBuilder
        where TStateMachine : IAsyncStateMachine
    {
        MoveNextRunner moveNextRunner = new MoveNextRunner(ExecutionContext.Capture());
        Action result = moveNextRunner.Run;
        if (StateMachine == null)
        {
            builder.PreBoxInitialization();
            StateMachine = stateMachine;
            StateMachine.SetStateMachine(StateMachine);
        }

        moveNextRunner.StateMachine = StateMachine;
        return result;
    }
}