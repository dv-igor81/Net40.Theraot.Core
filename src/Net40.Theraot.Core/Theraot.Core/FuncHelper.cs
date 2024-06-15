using System;

namespace Theraot.Core;

public static partial class FuncHelper
{
    public static Func<TOutput> ChainConversion<TInput, TOutput>(this Func<TInput> source, Func<TInput, TOutput> converter)
    {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return () => converter.Invoke(source.Invoke());
        }

    public static Func<TReturn, TReturn> GetIdentityFunc<TReturn>()
    {
            return IdentityHelper<TReturn>.Instance;
        }

    private static class IdentityHelper<TReturn>
    {
        static IdentityHelper()
        {
                Instance = IdentityFunc;
            }

        public static Func<TReturn, TReturn> Instance { get; }

        private static TReturn IdentityFunc(TReturn target)
        {
                return target;
            }
    }
}
    
[System.Diagnostics.DebuggerNonUserCode]
public static partial class FuncHelper
{
    public static Func<TReturn> GetDefaultFunc<TReturn>()
    {
            return HelperDefaultFunc<TReturn>.Instance;
        }

    public static Func<T, TReturn> GetDefaultFunc<T, TReturn>()
    {
            return HelperDefaultFunc<T, TReturn>.Instance;
        }

    public static Func<T1, T2, TReturn> GetDefaultFunc<T1, T2, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, TReturn> GetDefaultFunc<T1, T2, T3, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, TReturn> GetDefaultFunc<T1, T2, T3, T4, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>()
    {
            return HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.Instance;
        }

    public static Func<bool> GetFallacyFunc()
    {
            return HelperFallacyFunc.Instance;
        }

    public static Func<T, bool> GetFallacyFunc<T>()
    {
            return HelperFallacyFunc<T>.Instance;
        }

    public static Func<T1, T2, bool> GetFallacyFunc<T1, T2>()
    {
            return HelperFallacyFunc<T1, T2>.Instance;
        }

    public static Func<T1, T2, T3, bool> GetFallacyFunc<T1, T2, T3>()
    {
            return HelperFallacyFunc<T1, T2, T3>.Instance;
        }

    public static Func<T1, T2, T3, T4, bool> GetFallacyFunc<T1, T2, T3, T4>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, bool> GetFallacyFunc<T1, T2, T3, T4, T5>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, bool> GetFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>()
    {
            return HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Instance;
        }

    public static Func<TReturn> GetReturnFunc<TReturn>(TReturn def)
    {
            return () => def;
        }

    public static Func<T, TReturn> GetReturnFunc<T, TReturn>(TReturn def)
    {
            return (_) => def;
        }

    public static Func<T1, T2, TReturn> GetReturnFunc<T1, T2, TReturn>(TReturn def)
    {
            return (_, _) => def;
        }

    public static Func<T1, T2, T3, TReturn> GetReturnFunc<T1, T2, T3, TReturn>(TReturn def)
    {
            return (_, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, TReturn> GetReturnFunc<T1, T2, T3, T4, TReturn>(TReturn def)
    {
            return (_, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, TReturn>(TReturn def)
    {
            return (_, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => def;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> GetReturnFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>(TReturn def)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => def;
        }

    public static Func<bool> GetTautologyFunc()
    {
            return HelperTautologyFunc.Instance;
        }

    public static Func<T, bool> GetTautologyFunc<T>()
    {
            return HelperTautologyFunc<T>.Instance;
        }

    public static Func<T1, T2, bool> GetTautologyFunc<T1, T2>()
    {
            return HelperTautologyFunc<T1, T2>.Instance;
        }

    public static Func<T1, T2, T3, bool> GetTautologyFunc<T1, T2, T3>()
    {
            return HelperTautologyFunc<T1, T2, T3>.Instance;
        }

    public static Func<T1, T2, T3, T4, bool> GetTautologyFunc<T1, T2, T3, T4>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, bool> GetTautologyFunc<T1, T2, T3, T4, T5>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Instance;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, bool> GetTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>()
    {
            return HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Instance;
        }

    public static Func<TReturn> GetThrowFunc<TReturn>(Exception exception)
    {
            return () => throw exception;
        }

    public static Func<T, TReturn> GetThrowFunc<T, TReturn>(Exception exception)
    {
            return (_) => throw exception;
        }

    public static Func<T1, T2, TReturn> GetThrowFunc<T1, T2, TReturn>(Exception exception)
    {
            return (_, _) => throw exception;
        }

    public static Func<T1, T2, T3, TReturn> GetThrowFunc<T1, T2, T3, TReturn>(Exception exception)
    {
            return (_, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, TReturn> GetThrowFunc<T1, T2, T3, T4, TReturn>(Exception exception)
    {
            return (_, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, TReturn>(Exception exception)
    {
            return (_, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => throw exception;
        }

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>(Exception exception)
    {
            return (_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _) => throw exception;
        }

    private static class HelperDefaultFunc<TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<TReturn> Instance { get; }

        private static TReturn DefaultFunc()
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T obj)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
                return default;
            }
    }

    private static class HelperDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>
    {
        static HelperDefaultFunc()
        {
                Instance = DefaultFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> Instance { get; }


        private static TReturn DefaultFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
                return default;
            }
    }

    private static class HelperFallacyFunc
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<bool> Instance { get; }

        private static bool FallacyFunc()
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T, bool> Instance { get; }


        private static bool FallacyFunc(T obj)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
                return false;
            }
    }

    private static class HelperFallacyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
        static HelperFallacyFunc()
        {
                Instance = FallacyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, bool> Instance { get; }


        private static bool FallacyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
                return false;
            }
    }

    private static class HelperTautologyFunc
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<bool> Instance { get; }

        private static bool TautologyFunc()
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T, bool> Instance { get; }


        private static bool TautologyFunc(T obj)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
                return true;
            }
    }

    private static class HelperTautologyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
        static HelperTautologyFunc()
        {
                Instance = TautologyFunc;
            }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, bool> Instance { get; }


        private static bool TautologyFunc(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
                return true;
            }
    }
}