using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading;

public sealed class RootedTimeout : IPromise
{
    private const int _canceled = 5;

    private const int _canceling = 4;

    private const int _changing = 6;

    private const int _created = 0;

    private const int _executed = 3;

    private const int _executing = 2;

    private const int _started = 1;

    private static readonly Bucket<RootedTimeout> _root = new Bucket<RootedTimeout>();

    private static int _lastRootIndex = -1;

    private readonly int _hashcode;

    private Action _callback;

    private int _rootIndex = -1;

    private long _startTime;

    private int _status;

    private long _targetTime;

    private Timer _wrapped;

    public bool IsCanceled => Volatile.Read(ref _status) == 5;

    public bool IsCompleted => Volatile.Read(ref _status) == 3;

    private RootedTimeout()
    {
            _hashcode = (int)DateTime.Now.Ticks;
        }

    ~RootedTimeout()
    {
            Timer.Donate(ref _wrapped);
        }

    public static RootedTimeout Launch(Action callback, long dueTime)
    {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException("dueTime");
            }

            RootedTimeout timeout = new RootedTimeout();
            Root(timeout);
            timeout._callback = delegate
            {
                try
                {
                    callback();
                }
                finally
                {
                    UnRoot(timeout);
                }
            };
            timeout.Start(dueTime);
            return timeout;
        }

    public static RootedTimeout Launch(Action callback, long dueTime, CancellationToken token)
    {
            return Launch(callback, ActionHelper.GetNoopAction(), dueTime, token);
        }

    public static RootedTimeout Launch(Action callback, Action cancelledCallback, long dueTime,
        CancellationToken token)
    {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (cancelledCallback == null)
            {
                throw new ArgumentNullException("cancelledCallback");
            }

            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException("dueTime");
            }

            RootedTimeout timeout = new RootedTimeout();
            if (token.CanBeCanceled)
            {
                RegisterCancellation(cancelledCallback, timeout, ref token);
            }

            Root(timeout);
            if (dueTime == -1)
            {
                return timeout;
            }

            timeout._callback = delegate
            {
                try
                {
                    callback();
                }
                finally
                {
                    UnRoot(timeout);
                }
            };
            timeout.Start(dueTime);
            return timeout;
        }

    public static RootedTimeout Launch(Action callback, TimeSpan dueTime)
    {
            return Launch(callback, (long)dueTime.TotalMilliseconds);
        }

    public static RootedTimeout Launch(Action callback, TimeSpan dueTime, CancellationToken token)
    {
            return Launch(callback, (long)dueTime.TotalMilliseconds, token);
        }

    public bool Cancel()
    {
            if (Interlocked.CompareExchange(ref _status, 4, 0) == 0 ||
                Interlocked.CompareExchange(ref _status, 4, 1) == 1)
            {
                Close();
            }

            if (Interlocked.CompareExchange(ref _status, 5, 4) != 4)
            {
                return false;
            }

            Volatile.Write(ref _status, 5);
            return true;
        }

    public bool Change(long dueTime)
    {
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException("dueTime");
            }

            if (Interlocked.CompareExchange(ref _status, 6, 1) != 1)
            {
                return false;
            }

            _startTime = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
            Timer timer = Interlocked.CompareExchange(ref _wrapped, null, null);
            if (timer == null)
            {
                return false;
            }

            if (dueTime == -1)
            {
                _targetTime = -1L;
            }
            else
            {
                _targetTime = _startTime + dueTime;
                timer.Change(Finish, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1.0));
            }

            Volatile.Write(ref _status, 1);
            return true;
        }

    public void Change(TimeSpan dueTime)
    {
            Change((long)dueTime.TotalMilliseconds);
        }

    public long CheckRemaining()
    {
            if (_targetTime == -1)
            {
                return -1L;
            }

            long num = _targetTime - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
            if (num > 0)
            {
                return num;
            }

            Finish();
            return 0L;
        }

    public override bool Equals(object obj)
    {
            if (obj is RootedTimeout)
            {
                return this == obj;
            }

            return false;
        }

    public override int GetHashCode()
    {
            return _hashcode;
        }

    private static void RegisterCancellation(Action cancelledCallback, RootedTimeout timeout,
        ref CancellationToken token)
    {
            token.Register(delegate
            {
                if (timeout.Cancel())
                {
                    cancelledCallback();
                }
            });
        }

    private static void Root(RootedTimeout rootedTimeout)
    {
            rootedTimeout._rootIndex = Interlocked.Increment(ref _lastRootIndex);
            _root.Set(rootedTimeout._rootIndex, rootedTimeout);
        }

    private static void UnRoot(RootedTimeout rootedTimeout)
    {
            int num = Interlocked.Exchange(ref rootedTimeout._rootIndex, -1);
            if (num != -1)
            {
                _root.RemoveAt(num);
            }
        }

    private void Close()
    {
            Timer.Donate(ref _wrapped);
            GC.SuppressFinalize(this);
        }

    private void Finish()
    {
            ThreadingHelper.SpinWaitWhile(ref _status, 6);
            if (Interlocked.CompareExchange(ref _status, 2, 1) == 1)
            {
                Action action = Volatile.Read(ref _callback);
                if (action != null)
                {
                    action();
                    Volatile.Write(ref _status, 3);
                    Close();
                }
            }
        }

    private void Start(long dueTime)
    {
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException("dueTime");
            }

            if (Interlocked.CompareExchange(ref _status, 1, 0) == 0)
            {
                _startTime = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
                _targetTime = ((dueTime == -1) ? (-1) : (_startTime + dueTime));
                _wrapped = Timer.GetTimer(Finish, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1.0));
            }
        }
}