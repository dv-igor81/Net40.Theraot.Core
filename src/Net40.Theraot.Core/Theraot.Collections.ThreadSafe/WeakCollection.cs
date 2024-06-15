using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe;

[DebuggerNonUserCode]
[DebuggerDisplay("Count={Count}")]
public class WeakCollection<T, TNeedle> : ICollection<T>, IEnumerable<T>, IEnumerable
    where T : class where TNeedle : WeakNeedle<T>, new()
{
    private readonly IEqualityComparer<T> _comparer;

    private readonly ThreadSafeCollection<TNeedle> _wrapped;

    private WeakNeedle<EventHandler> _eventHandler;

    public bool AutoRemoveDeadItems
    {
        get { return _eventHandler.IsAlive; }
        set
        {
                if (value != _eventHandler.IsAlive)
                {
                    if (value)
                    {
                        RegisterForAutoRemoveDeadItems();
                    }
                    else
                    {
                        UnRegisterForAutoRemoveDeadItems();
                    }
                }
            }
    }

    public int Count => _wrapped.Count;

    bool ICollection<T>.IsReadOnly => false;

    public WeakCollection()
        : this((IEqualityComparer<T>)null, autoRemoveDeadItems: true)
    {
        }

    public WeakCollection(IEqualityComparer<T> comparer)
        : this(comparer, autoRemoveDeadItems: true)
    {
        }

    public WeakCollection(bool autoRemoveDeadItems)
        : this((IEqualityComparer<T>)null, autoRemoveDeadItems)
    {
        }

    public WeakCollection(IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
    {
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _wrapped = new ThreadSafeCollection<TNeedle>();
            _eventHandler = new WeakNeedle<EventHandler>(null);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

    ~WeakCollection()
    {
            UnRegisterForAutoRemoveDeadItemsExtracted();
        }

    public void Add(T item)
    {
            TNeedle val = new TNeedle();
            if (item != null)
            {
                val.Value = item;
            }

            _wrapped.Add(val);
        }

    public void Clear()
    {
            foreach (TNeedle item in _wrapped.ClearEnumerable())
            {
                item.Free();
            }
        }

    public IEnumerable<T> ClearEnumerable()
    {
            foreach (TNeedle needle in _wrapped.ClearEnumerable())
            {
                if (needle.TryGetValue(out var result))
                {
                    yield return result;
                }

                needle.Free();
                result = null;
            }
        }

    public bool Contains(T item)
    {
            Predicate<TNeedle> check = Check(item);
            return _wrapped.Where(check).Any();
        }

    public bool Contains(Predicate<T> itemCheck)
    {
            if (itemCheck == null)
            {
                throw new ArgumentNullException("itemCheck");
            }

            Predicate<TNeedle> check = Check(itemCheck);
            return _wrapped.Where(check).Any();
        }

    public void CopyTo(T[] array, int arrayIndex)
    {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            this.CopyTo<T>(array, arrayIndex);
        }

    public IEnumerator<T> GetEnumerator()
    {
            foreach (TNeedle needle in _wrapped)
            {
                if (needle.TryGetValue(out var result))
                {
                    yield return result;
                }

                result = null;
            }
        }

    IEnumerator IEnumerable.GetEnumerator()
    {
            return GetEnumerator();
        }

    public bool Remove(T item)
    {
            Predicate<TNeedle> check = Check(item);
            using (IEnumerator<TNeedle> enumerator = _wrapped.RemoveWhereEnumerable(check).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    TNeedle current = enumerator.Current;
                    current.Free();
                    return true;
                }
            }

            return false;
        }

    public void RemoveDeadItems()
    {
            _wrapped.RemoveWhere((TNeedle input) => !input.IsAlive);
        }

    public int RemoveWhere(Predicate<T> itemCheck)
    {
            Predicate<TNeedle> check = Check(itemCheck);
            return _wrapped.RemoveWhere(check);
        }

    public IEnumerable<T> RemoveWhereEnumerable(Predicate<T> itemCheck)
    {
            Predicate<TNeedle> check = Check(itemCheck);
            foreach (TNeedle removed in _wrapped.RemoveWhereEnumerable(check))
            {
                if (removed.TryGetValue(out var value))
                {
                    yield return value;
                }

                removed.Free();
                value = null;
            }
        }

    protected void Add(TNeedle needle)
    {
            _wrapped.Add(needle);
        }

    protected bool Contains(Predicate<TNeedle> needleCheck)
    {
            return _wrapped.Where(needleCheck).Any();
        }

    protected IEnumerable<T> RemoveWhereEnumerable(Predicate<TNeedle> needleCheck)
    {
            foreach (TNeedle removed in _wrapped.RemoveWhereEnumerable(needleCheck))
            {
                if (removed.TryGetValue(out var value))
                {
                    yield return value;
                }

                removed.Free();
                value = null;
            }
        }

    private static Predicate<TNeedle> Check(Predicate<T> itemCheck)
    {
            T value;
            return (TNeedle input) => input.TryGetValue(out value) && itemCheck(value);
        }

    private Predicate<TNeedle> Check(T item)
    {
            T item2 = item;
            T value;
            return (TNeedle input) => input.TryGetValue(out value) && _comparer.Equals(item2, value);
        }

    private void GarbageCollected(object sender, EventArgs e)
    {
            RemoveDeadItems();
        }

    private void RegisterForAutoRemoveDeadItems()
    {
            if (RegisterForAutoRemoveDeadItemsExtracted())
            {
                GC.ReRegisterForFinalize(this);
            }
        }

    private bool RegisterForAutoRemoveDeadItemsExtracted()
    {
            bool result = false;
            EventHandler eventHandler = _eventHandler.Value;
            if (eventHandler == null)
            {
                eventHandler = GarbageCollected;
                _eventHandler = new WeakNeedle<EventHandler>(eventHandler);
                result = true;
            }

            GCMonitor.Collected += eventHandler;
            return result;
        }

    private void UnRegisterForAutoRemoveDeadItems()
    {
            if (UnRegisterForAutoRemoveDeadItemsExtracted())
            {
                GC.SuppressFinalize(this);
            }
        }

    private bool UnRegisterForAutoRemoveDeadItemsExtracted()
    {
            if (NeedleHelper.TryGetValue(_eventHandler, out var target))
            {
                GCMonitor.Collected -= target;
                _eventHandler.Free();
                return true;
            }

            _eventHandler.Free();
            return false;
        }
}