#define DEBUG
using System.Diagnostics;

namespace System.Text;

public static class UnicodeDebug
{
	[Conditional("DEBUG")]
	internal static void AssertIsHighSurrogateCodePoint(uint codePoint)
	{
		Debug.Assert(UnicodeUtility.IsHighSurrogateCodePoint(codePoint), "The value " + ToHexString(codePoint) + " is not a valid UTF-16 high surrogate code point.");
	}

	[Conditional("DEBUG")]
	internal static void AssertIsLowSurrogateCodePoint(uint codePoint)
	{
		Debug.Assert(UnicodeUtility.IsLowSurrogateCodePoint(codePoint), "The value " + ToHexString(codePoint) + " is not a valid UTF-16 low surrogate code point.");
	}

	[Conditional("DEBUG")]
	internal static void AssertIsValidCodePoint(uint codePoint)
	{
		Debug.Assert(UnicodeUtility.IsValidCodePoint(codePoint), "The value " + ToHexString(codePoint) + " is not a valid Unicode code point.");
	}

	[Conditional("DEBUG")]
	internal static void AssertIsValidScalar(uint scalarValue)
	{
		Debug.Assert(UnicodeUtility.IsValidUnicodeScalar(scalarValue), "The value " + ToHexString(scalarValue) + " is not a valid Unicode scalar value.");
	}

	[Conditional("DEBUG")]
	public static void AssertIsValidSupplementaryPlaneScalar(uint scalarValue)
	{
		Debug.Assert(UnicodeUtility.IsValidUnicodeScalar(scalarValue) && !UnicodeUtility.IsBmpCodePoint(scalarValue), "The value " + ToHexString(scalarValue) + " is not a valid supplementary plane Unicode scalar value.");
	}

	private static string ToHexString(uint codePoint)
	{
		return FormattableString.Invariant($"U+{codePoint:X4}");
	}
}
