using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public static class GCMonitor
{
    [DebuggerNonUserCode]
    private sealed class GCProbe : CriticalFinalizerObject
    {
        ~GCProbe()
        {
                try
                {
                }
                finally
                {
                    try
                    {
                        if (Volatile.Read(ref _status) == 0)
                        {
                            GC.ReRegisterForFinalize(this);
                            Internal.Invoke();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
    }

    private static class Internal
    {
        private static readonly WaitCallback _work = delegate { RaiseCollected(); };

        public static WeakDelegateCollection CollectedEventHandlers { get; } =
            new WeakDelegateCollection(autoRemoveDeadItems: false, freeReentry: false);


        public static void Invoke()
        {
                ThreadPool.QueueUserWorkItem(_work);
            }

        private static void RaiseCollected()
        {
                if (Volatile.Read(ref _status) == 0)
                {
                    try
                    {
                        CollectedEventHandlers.RemoveDeadItems();
                        CollectedEventHandlers.Invoke(ActionHelper.GetNoopAction<Exception>(),
                            DelegateCollectionInvokeOptions.None, null, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                    }

                    Volatile.Write(ref _status, 0);
                }
            }
    }

    private const int _statusNotReady = -2;

    private const int _statusPending = -1;

    private const int _statusReady = 0;

    private static int _status;

    private const int _statusFinished = 1;

    public static bool FinalizingForUnload => AppDomain.CurrentDomain.IsFinalizingForUnload();

    public static event EventHandler Collected
    {
        add
        {
                try
                {
                    Initialize();
                    Internal.CollectedEventHandlers.Add(value);
                }
                catch (NullReferenceException) when (value == null)
                {
                }
            }
        remove
        {
                if (Volatile.Read(ref _status) != 0)
                {
                    return;
                }

                try
                {
                    Internal.CollectedEventHandlers.Remove(value);
                }
                catch (NullReferenceException) when (value == null)
                {
                }
            }
    }

    private static void Initialize()
    {
            switch (Interlocked.CompareExchange(ref _status, -1, -2))
            {
                case -2:
                    GC.KeepAlive(new GCProbe());
                    Volatile.Write(ref _status, 0);
                    break;
                case -1:
                    ThreadingHelper.SpinWaitUntil(ref _status, 0);
                    break;
            }
        }

    static GCMonitor()
    {
            _status = -2;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.ProcessExit += ReportApplicationDomainExit;
            currentDomain.DomainUnload += ReportApplicationDomainExit;
        }

    private static void ReportApplicationDomainExit(object sender, EventArgs e)
    {
            Volatile.Write(ref _status, 1);
        }
}