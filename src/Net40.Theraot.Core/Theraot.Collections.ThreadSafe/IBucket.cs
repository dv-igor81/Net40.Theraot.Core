using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Theraot.Collections.ThreadSafe;

public interface IBucket<T> : IEnumerable<T>, IEnumerable
{
	int Count { get; }

	void CopyTo(T[] array, int arrayIndex);

	bool Exchange(int index, T item, [MaybeNullWhen(true)] out T previous);

	bool Insert(int index, T item);

	bool Insert(int index, T item, out T previous);

	bool RemoveAt(int index);

	bool RemoveAt(int index, out T previous);

	bool RemoveAt(int index, Predicate<T> check);

	void Set(int index, T item, out bool isNew);

	bool TryGet(int index, out T value);

	bool Update(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty);

	IEnumerable<T> Where(Predicate<T> check);

	IEnumerable<KeyValuePair<int, T>> WhereIndexed(Predicate<T> check);
}
