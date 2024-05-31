using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace System;

public static class StringEx
{
	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static string Concat(IEnumerable<string> values)
	{
		return string.Concat(values);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static string Concat<T>(IEnumerable<T> values)
	{
		return string.Concat(values);
	}

	public static string Concat<T>(IEnumerable<T> values, Func<T, string> converter)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		List<string> list = new List<string>();
		int num = 0;
		foreach (T value in values)
		{
			string text = converter(value);
			list.Add(text);
			num += text.Length;
		}
		return ConcatExtractedExtracted(list.ToArray(), 0, list.Count, num);
	}

	public static string Concat(object[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		return (arrayIndex == array.Length) ? string.Empty : ConcatExtracted(array, arrayIndex, array.Length - arrayIndex);
	}

	public static string Concat(object[] array, int arrayIndex, int countLimit)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (countLimit < 0)
		{
			throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
		}
		if (countLimit > array.Length - arrayIndex)
		{
			throw new ArgumentException("startIndex plus countLimit is greater than the number of elements in array.", "array");
		}
		return (arrayIndex == array.Length) ? string.Empty : ConcatExtracted(array, arrayIndex, countLimit);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static string Concat(params object[] values)
	{
		return string.Concat(values);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static string Concat(params string[] value)
	{
		return string.Concat(value);
	}

	public static string Concat(string[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		return (arrayIndex == array.Length) ? string.Empty : ConcatExtracted(array, arrayIndex, array.Length - arrayIndex);
	}

	public static string Concat(string[] array, int arrayIndex, int countLimit)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (countLimit < 0)
		{
			throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
		}
		if (countLimit > array.Length - arrayIndex)
		{
			throw new ArgumentException("startIndex plus countLimit is greater than the number of elements in array.", "array");
		}
		return (arrayIndex == array.Length) ? string.Empty : ConcatExtracted(array, arrayIndex, countLimit);
	}

	private static string ConcatExtracted(object[] array, int startIndex, int count)
	{
		int num = 0;
		int num2 = startIndex + count;
		string[] array2 = new string[count];
		for (int i = startIndex; i < num2; i++)
		{
			string text = array[i]?.ToString();
			if (text != null)
			{
				array2[i - startIndex] = text;
				num += text.Length;
			}
		}
		return ConcatExtractedExtracted(array2, 0, count, num);
	}

	private static string ConcatExtracted(string[] array, int startIndex, int count)
	{
		int num = 0;
		int num2 = startIndex + count;
		for (int i = startIndex; i < num2; i++)
		{
			string text = array[i];
			if (text != null)
			{
				num += text.Length;
			}
		}
		return ConcatExtractedExtracted(array, startIndex, num2, num);
	}

	private static string ConcatExtractedExtracted(string[] array, int startIndex, int maxIndex, int length)
	{
		if (length <= 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(length);
		for (int i = startIndex; i < maxIndex; i++)
		{
			string value = array[i];
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	public static string Implode(string separator, IEnumerable<string> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values);
		}
		List<string> list = values.ToList();
		return ImplodeExtracted(separator, list.ToArray(), 0, list.Count);
	}

	public static string Implode(string separator, IEnumerable<string> values, string start, string end)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values);
		}
		List<string> list = values.ToList();
		if (list.Count == 0)
		{
			return string.Empty;
		}
		if (start == null)
		{
			start = string.Empty;
		}
		if (end == null)
		{
			end = string.Empty;
		}
		return start + ImplodeExtracted(separator, list.ToArray(), 0, list.Count) + end;
	}

	public static string Implode<T>(string separator, IEnumerable<T> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values);
		}
		List<string> list = values.Select((T item) => item?.ToString()).ToList();
		return ImplodeExtracted(separator, list.ToArray(), 0, list.Count);
	}

	public static string Implode<T>(string separator, IEnumerable<T> values, Func<T, string> converter)
	{
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values, converter);
		}
		List<string> list = values.Select((T item) => item?.ToString()).ToList();
		return ImplodeExtracted(separator, list.ToArray(), 0, list.Count);
	}

	public static string Implode<T>(string separator, IEnumerable<T> values, Func<T, string> converter, string start, string end)
	{
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values, converter);
		}
		List<string> list = values.Select((T item) => item?.ToString()).ToList();
		if (list.Count == 0)
		{
			return string.Empty;
		}
		if (start == null)
		{
			start = string.Empty;
		}
		if (end == null)
		{
			end = string.Empty;
		}
		return start + ImplodeExtracted(separator, list.ToArray(), 0, list.Count) + end;
	}

	public static string Implode<T>(string separator, IEnumerable<T> values, string start, string end)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values);
		}
		List<string> list = values.Select((T item) => item?.ToString()).ToList();
		if (list.Count == 0)
		{
			return string.Empty;
		}
		if (start == null)
		{
			start = string.Empty;
		}
		if (end == null)
		{
			end = string.Empty;
		}
		return start + ImplodeExtracted(separator, list.ToArray(), 0, list.Count) + end;
	}

	public static string Implode(string separator, object[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		return ImplodeExtracted(separator ?? string.Empty, array, arrayIndex, array.Length - arrayIndex);
	}

	public static string Implode(string separator, object[] array, int arrayIndex, int countLimit)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (countLimit < 0)
		{
			throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
		}
		if (countLimit > array.Length - arrayIndex)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		return ImplodeExtracted(separator ?? string.Empty, array, arrayIndex, countLimit);
	}

	public static string Implode(string separator, params object[] values)
	{
		if (separator == null)
		{
			return string.Concat(values);
		}
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		string[] array = new string[values.Length];
		int num = 0;
		foreach (object obj in values)
		{
			array[num++] = obj?.ToString();
		}
		return ImplodeExtracted(separator, array, 0, array.Length);
	}

	public static string Implode(string separator, params string[] value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		return ImplodeExtracted(separator ?? string.Empty, value, 0, value.Length);
	}

	public static string Implode(string separator, string[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		return ImplodeExtracted(separator ?? string.Empty, array, arrayIndex, array.Length - arrayIndex);
	}

	public static string Implode(string separator, string[] array, int arrayIndex, int countLimit)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (countLimit < 0)
		{
			throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
		}
		if (countLimit > array.Length - arrayIndex)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		return ImplodeExtracted(separator ?? string.Empty, array, arrayIndex, countLimit);
	}

	private static string ImplodeExtracted(string separator, object[] array, int startIndex, int count)
	{
		int num = 0;
		int num2 = startIndex + count;
		string[] array2 = new string[count];
		for (int i = startIndex; i < num2; i++)
		{
			string text = array[i]?.ToString();
			if (text != null)
			{
				array2[i - startIndex] = text;
				num += text.Length;
			}
		}
		num += separator.Length * (count - 1);
		return ImplodeExtractedExtracted(separator, array2, 0, count, num);
	}

	private static string ImplodeExtracted(string separator, string?[] array, int startIndex, int count)
	{
		int num = 0;
		int num2 = startIndex + count;
		for (int i = startIndex; i < num2; i++)
		{
			string text = array[i];
			if (text != null)
			{
				num += text.Length;
			}
		}
		num += separator.Length * (count - 1);
		return ImplodeExtractedExtracted(separator, array, startIndex, num2, num);
	}

	private static string ImplodeExtractedExtracted(string separator, string?[] array, int startIndex, int maxIndex, int length)
	{
		if (length <= 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(length);
		bool flag = true;
		for (int i = startIndex; i < maxIndex; i++)
		{
			string value = array[i];
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsNullOrWhiteSpace(string value)
	{
		return string.IsNullOrWhiteSpace(value);
	}
}
