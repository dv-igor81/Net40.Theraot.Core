/*
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

public sealed class ExceptionDispatchInfo
{
    private readonly Exception _exception;

    private readonly Exception.DispatchState _dispatchState;

    public Exception SourceException => _exception;

    private ExceptionDispatchInfo(Exception exception)
    {
        _exception = exception;
        _dispatchState = exception.CaptureDispatchState();
    }

    public static ExceptionDispatchInfo Capture(Exception source)
    {
        if (source == null)
        {
            throw new ArgumentNullException("source");
        }
        return new ExceptionDispatchInfo(source);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    public void Throw()
    {
        _exception.RestoreDispatchState(in _dispatchState);
        throw _exception;
    }

    [DoesNotReturn]
    public static void Throw(Exception source)
    {
        Capture(source).Throw();
    }
}*/


using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace System.Runtime.ExceptionServices;

public sealed class ExceptionDispatchInfo
{
    private static FieldInfo _remoteStackTraceString;

    private readonly object _stackTrace;

    public Exception SourceException { get; }

    private ExceptionDispatchInfo(Exception exception)
    {
        SourceException = exception;
        _stackTrace = SourceException.StackTrace;
        if (_stackTrace != null)
        {
            _stackTrace = _stackTrace?.ToString() + Environment.NewLine +
                          "---End of stack trace from previous location where exception was thrown ---" +
                          Environment.NewLine;
        }
        else
        {
            _stackTrace = string.Empty;
        }
    }

    public static ExceptionDispatchInfo Capture(Exception source)
    {
        if (source == null)
        {
            throw new ArgumentNullException("source");
        }

        return new ExceptionDispatchInfo(source);
    }

    [DoesNotReturn]
    public static void Throw(Exception source)
    {
        Capture(source).Throw();
    }

    public void Throw()
    {
        try
        {
            throw SourceException;
        }
        catch (Exception)
        {
            string value = _stackTrace?.ToString() + BuildStackTrace(Environment.StackTrace);
            SetStackTrace(SourceException, value);
            throw;
        }

        static string BuildStackTrace(string trace)
        {
            string[] array = trace.Split(new string[1] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = false;
            string[] array2 = array;
            foreach (string text in array2)
            {
                if (text.Contains(":"))
                {
                    if (text.Contains("System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()"))
                    {
                        break;
                    }

                    if (flag)
                    {
                        stringBuilder.Append(Environment.NewLine);
                    }

                    flag = true;
                    stringBuilder.Append(text);
                }
                else if (flag)
                {
                    break;
                }
            }

            return stringBuilder.ToString();
        }
    }

    private static FieldInfo GetFieldInfo()
    {
        if (_remoteStackTraceString != null)
        {
            return _remoteStackTraceString;
        }

        _remoteStackTraceString =
            typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic) ??
            typeof(Exception).GetField("remote_stack_trace", BindingFlags.Instance | BindingFlags.NonPublic);
        return _remoteStackTraceString;
    }

    private static void SetStackTrace(Exception exception, object value)
    {
        FieldInfo fieldInfo = GetFieldInfo();
        fieldInfo.SetValue(exception, value);
    }
}