using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public class WeakNeedle<T> : IEquatable<WeakNeedle<T>>, IRecyclable, ICacheNeedle<T>, INeedle<T>, IReadOnlyNeedle<T>, IPromise<T>, IPromise where T : class
{
	private readonly int _hashCode;

	private readonly bool _trackResurrection;

	private WeakReference<T>? _handle;

	public Exception? Exception { get; private set; }

	public bool IsAlive
	{
		get
		{
			T target;
			return Exception == null && (_handle?.TryGetTarget(out target) ?? false);
		}
	}

	bool IPromise.IsCompleted => true;

	public bool IsFaulted => Exception != null;

	public virtual bool TrackResurrection => _trackResurrection;

	public virtual T Value
	{
		get
		{
			if (Exception == null && _handle != null && _handle.TryGetTarget(out T target))
			{
				return target;
			}
			throw new InvalidOperationException();
		}
		set
		{
			SetTargetValue(value);
		}
	}

	public WeakNeedle()
		: this(trackResurrection: false)
	{
	}

	public WeakNeedle(bool trackResurrection)
	{
		_trackResurrection = trackResurrection;
		_hashCode = base.GetHashCode();
	}

	public WeakNeedle(T? target)
		: this(target, trackResurrection: false)
	{
	}

	public WeakNeedle(T? target, bool trackResurrection)
	{
		if (target == null)
		{
			_hashCode = base.GetHashCode();
		}
		else
		{
			SetTargetValue(target);
			_hashCode = target.GetHashCode();
		}
		_trackResurrection = trackResurrection;
	}

	public static bool operator !=(WeakNeedle<T> left, WeakNeedle<T> right)
	{
		if ((object)left == null)
		{
			return (object)right != null;
		}
		return (object)right == null || !EqualsExtractedExtracted(left, right);
	}

	public static bool operator ==(WeakNeedle<T> left, WeakNeedle<T> right)
	{
		if ((object)left == null)
		{
			return (object)right == null;
		}
		return (object)right != null && EqualsExtractedExtracted(left, right);
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is WeakNeedle<T> weakNeedle))
		{
			if (obj is T val)
			{
				T y = val;
				if (TryGetValue(out var value))
				{
					return EqualityComparer<T>.Default.Equals(value, y);
				}
			}
			return false;
		}
		WeakNeedle<T> right = weakNeedle;
		return EqualsExtractedExtracted(this, right);
	}

	public bool Equals(WeakNeedle<T>? other)
	{
		return (object)other != null && EqualsExtractedExtracted(this, other);
	}

	public void Free()
	{
		SetTargetValue(null);
	}

	public override int GetHashCode()
	{
		return _hashCode;
	}

	public override string ToString()
	{
		if (Exception != null)
		{
			return $"<Faulted: {Exception}>";
		}
		T target;
		return (_handle != null && _handle.TryGetTarget(out target)) ? (target.ToString() ?? "<?>") : "<Dead Needle>";
	}

	public virtual bool TryGetValue(out T value)
	{
		if (Exception == null && _handle != null && _handle.TryGetTarget(out T target))
		{
			value = target;
			return true;
		}
		value = null;
		return false;
	}

	protected void SetTargetError(Exception error)
	{
		Exception = error;
		_handle = null;
	}

	protected void SetTargetValue(T? value)
	{
		if (value == null)
		{
			_handle = null;
		}
		else if (_handle == null)
		{
			_handle = new WeakReference<T>(value, _trackResurrection);
		}
		else
		{
			_handle.SetTarget(value);
		}
		Exception = null;
	}

	private static bool EqualsExtractedExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
	{
		Exception exception = left.Exception;
		Exception exception2 = right.Exception;
		if (left.Exception != null || right.Exception != null)
		{
			return EqualityComparer<System.Exception>.Default.Equals(exception, exception2);
		}
		if (left.TryGetValue(out var value) && right.TryGetValue(out var value2))
		{
			return EqualityComparer<T>.Default.Equals(value, value2);
		}
		return false;
	}
}
