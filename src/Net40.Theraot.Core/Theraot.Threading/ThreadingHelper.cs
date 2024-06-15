using System;
using System.Diagnostics;
using System.Threading;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public static class ThreadingHelper
{
    internal const int SleepCountHint = 10;

    internal static long Milliseconds(long ticks)
    {
            return ticks / 10000;
        }

    internal static long TicksNow()
    {
            return DateTime.Now.Ticks;
        }

    public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue,
        out int lastValue)
    {
            SpinWait spinWait = default(SpinWait);
            while (true)
            {
                lastValue = Volatile.Read(ref check);
                if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue ||
                    lastValue > maxValue - value)
                {
                    return false;
                }

                int value2 = lastValue + value;
                int num = Interlocked.CompareExchange(ref check, value2, lastValue);
                if (num == lastValue)
                {
                    break;
                }

                spinWait.SpinOnce();
            }

            return true;
        }

    public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue)
    {
            SpinWait spinWait = default(SpinWait);
            while (true)
            {
                lastValue = Volatile.Read(ref check);
                if (lastValue < 0 || lastValue < -value)
                {
                    return false;
                }

                int value2 = lastValue + value;
                int num = Interlocked.CompareExchange(ref check, value2, lastValue);
                if (num == lastValue)
                {
                    break;
                }

                spinWait.SpinOnce();
            }

            return true;
        }

    public static bool SpinWaitRelativeSet(ref int check, int value)
    {
            SpinWait spinWait = default(SpinWait);
            while (true)
            {
                int num = Volatile.Read(ref check);
                int num2 = Interlocked.CompareExchange(ref check, num + value, num);
                if (num2 == num)
                {
                    break;
                }

                spinWait.SpinOnce();
            }

            return true;
        }

    public static bool SpinWaitRelativeSet(ref int check, int value, int milliseconds)
    {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }

            if (milliseconds == -1)
            {
                return SpinWaitRelativeSet(ref check, value);
            }

            SpinWait spinWait = default(SpinWait);
            long num = TicksNow();
            while (true)
            {
                int num2 = Volatile.Read(ref check);
                int num3 = Interlocked.CompareExchange(ref check, num2 + value, num2);
                if (num3 == num2)
                {
                    return true;
                }

                if (Milliseconds(TicksNow() - num) >= milliseconds)
                {
                    break;
                }

                spinWait.SpinOnce();
            }

            return false;
        }

    public static void SpinWaitSet(ref int check, int value, int comparand)
    {
            SpinWait spinWait = default(SpinWait);
            while (Interlocked.CompareExchange(ref check, value, comparand) != comparand)
            {
                spinWait.SpinOnce();
            }
        }

    public static bool SpinWaitSet(ref int check, int value, int comparand, int milliseconds)
    {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }

            if (milliseconds == -1)
            {
                SpinWaitSet(ref check, value, comparand);
                return true;
            }

            SpinWait spinWait = default(SpinWait);
            long num = TicksNow();
            while (true)
            {
                if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
                {
                    return true;
                }

                if (Milliseconds(TicksNow() - num) >= milliseconds)
                {
                    break;
                }

                spinWait.SpinOnce();
            }

            return false;
        }

    public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless)
    {
            SpinWait spinWait = default(SpinWait);
            while (true)
            {
                int num = Volatile.Read(ref check);
                if (num == unless)
                {
                    return false;
                }

                int num2 = Interlocked.CompareExchange(ref check, value, comparand);
                if (num2 == comparand)
                {
                    break;
                }

                spinWait.SpinOnce();
            }

            return true;
        }

    public static void SpinWaitUntil(Func<bool> verification)
    {
            if (verification == null)
            {
                throw new ArgumentNullException("verification");
            }

            SpinWait spinWait = default(SpinWait);
            while (!verification())
            {
                spinWait.SpinOnce();
            }
        }

    public static bool SpinWaitUntil(Func<bool> verification, TimeSpan timeout)
    {
            if (verification == null)
            {
                throw new ArgumentNullException("verification");
            }

            long num = (long)timeout.TotalMilliseconds;
            if (num < -1 || num > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }

            if (num == -1)
            {
                SpinWaitUntil(verification);
                return true;
            }

            SpinWait spinWait = default(SpinWait);
            long num2 = TicksNow();
            while (true)
            {
                if (verification())
                {
                    return true;
                }

                if (Milliseconds(TicksNow() - num2) >= num)
                {
                    break;
                }

                spinWait.SpinOnce();
            }

            return false;
        }

    public static void SpinWaitUntil(ref int check, int comparand)
    {
            SpinWait spinWait = default(SpinWait);
            while (Volatile.Read(ref check) != comparand)
            {
                spinWait.SpinOnce();
            }
        }

    public static void SpinWaitWhile(ref int check, int comparand)
    {
            SpinWait spinWait = default(SpinWait);
            while (Volatile.Read(ref check) == comparand)
            {
                spinWait.SpinOnce();
            }
        }

    public static void SpinWaitWhileNull<T>(ref T check) where T : class
    {
            SpinWait spinWait = default(SpinWait);
            while (Volatile.Read(ref check) == null)
            {
                spinWait.SpinOnce();
            }
        }
}