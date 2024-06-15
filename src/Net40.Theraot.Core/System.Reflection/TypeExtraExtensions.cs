using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace System.Reflection;

public static class TypeExtraExtensions
{
	private static readonly CacheDict<Type, bool> _binaryPortableCache = new CacheDict<Type, bool>(GetBinaryPortableResult, 256);

	private static readonly CacheDict<Type, bool> _blittableCache = new CacheDict<Type, bool>(GetBlittableResult, 256);

	private static readonly CacheDict<Type, bool> _valueTypeRecursiveCache = new CacheDict<Type, bool>(GetValueTypeRecursiveResult, 256);

	public static Type[] GenericTypeArguments(this Type self)
	{
		return self.GetGenericArguments();
	}

	public static bool IsAssignableTo(this Type self, [NotNullWhen(true)] Type? targetType)
	{
		return targetType?.IsAssignableFrom(self) ?? false;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static TypeCode GetTypeCode(this Type type)
	{
		return Type.GetTypeCode(type);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsSealed(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return typeInfo.IsSealed;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Assembly Assembly(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return typeInfo.Assembly;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsAbstract(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return typeInfo.IsAbstract;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Type BaseType(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return typeInfo.BaseType;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool CanBeNull(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return !typeInfo.IsValueType || Nullable.GetUnderlyingType(type) != null;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsValueType(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return typeInfo.IsValueType;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsEnum(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return typeInfo.IsEnum;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsGenericType(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return typeInfo.IsGenericType;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool DelegateEquals(this Delegate @delegate, MethodInfo? method, object target)
	{
		if ((object)@delegate == null)
		{
			throw new ArgumentNullException("delegate");
		}
		return DelegateEqualsExtracted(RuntimeReflectionExtensions.GetMethodInfo(@delegate), @delegate.Target, method, target);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static TAttribute[] GetAttributes<TAttribute>(this Assembly item) where TAttribute : Attribute
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), inherit: true);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static TAttribute[] GetAttributes<TAttribute>(this MemberInfo item, bool inherit) where TAttribute : Attribute
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), inherit);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static TAttribute[] GetAttributes<TAttribute>(this Module item) where TAttribute : Attribute
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), inherit: true);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static TAttribute[] GetAttributes<TAttribute>(this ParameterInfo item, bool inherit) where TAttribute : Attribute
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), inherit);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static TAttribute[] GetAttributes<TAttribute>(this Type type, bool inherit) where TAttribute : Attribute
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return (TAttribute[])typeInfo.GetCustomAttributes(typeof(TAttribute), inherit);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Type GetNonNullable(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return Nullable.GetUnderlyingType(type) ?? type;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Type GetNonRefType(this ParameterInfo parameterInfo)
	{
		if (parameterInfo == null)
		{
			throw new ArgumentNullException("parameterInfo");
		}
		Type type = parameterInfo.ParameterType;
		Type elementType = type.GetElementType();
		if (elementType != null && type.IsByRef)
		{
			type = elementType;
		}
		return type;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Type GetNonRefType(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return type.GetNonRefTypeInternal();
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Type GetNotNullable(this Type type)
	{
		Type underlyingType = Nullable.GetUnderlyingType(type);
		return underlyingType ?? type;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Type GetNullable(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsValueType && !type.IsNullable())
		{
			return typeof(Nullable<>).MakeGenericType(type);
		}
		return type;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Type GetReturnType(this MethodBase methodInfo)
	{
		if (methodInfo == null)
		{
			throw new ArgumentNullException("methodInfo");
		}
		return methodInfo.IsConstructor ? methodInfo.DeclaringType : ((MethodInfo)methodInfo).ReturnType;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool HasAttribute<TAttribute>(this Assembly item) where TAttribute : Attribute
	{
		TAttribute[] attributes = item.GetAttributes<TAttribute>();
		return attributes != null && attributes.Length != 0;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool HasAttribute<TAttribute>(this MemberInfo item) where TAttribute : Attribute
	{
		TAttribute[] attributes = item.GetAttributes<TAttribute>(inherit: true);
		return attributes != null && attributes.Length != 0;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool HasAttribute<TAttribute>(this Module item) where TAttribute : Attribute
	{
		TAttribute[] attributes = item.GetAttributes<TAttribute>();
		return attributes != null && attributes.Length != 0;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool HasAttribute<TAttribute>(this ParameterInfo item) where TAttribute : Attribute
	{
		TAttribute[] attributes = item.GetAttributes<TAttribute>(inherit: true);
		return attributes != null && attributes.Length != 0;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool HasAttribute<TAttribute>(this Type item) where TAttribute : Attribute
	{
		TAttribute[] attributes = item.GetAttributes<TAttribute>(inherit: true);
		return attributes != null && attributes.Length != 0;
	}

	public static bool HasIdentityPrimitiveOrNullableConversionTo(this Type source, Type target)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		return source.HasIdentityPrimitiveOrNullableConversionToInternal(target);
	}

	public static bool HasIdentityPrimitiveOrNullableConversionToInternal(this Type source, Type target)
	{
		if (source == target)
		{
			return true;
		}
		if (source.IsNullable() && target == source.GetNonNullable())
		{
			return true;
		}
		if (target.IsNullable() && source == target.GetNonNullable())
		{
			return true;
		}
		return source.IsConvertible() && target.IsConvertible() && (target.GetNonNullable() != typeof(bool) || (IntrospectionExtensions.GetTypeInfo(source).IsEnum && source.GetUnderlyingSystemType() == typeof(bool)));
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsArithmetic(this Type type)
	{
		type = type.GetNonNullable();
		return type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);
	}

	public static bool IsBinaryPortable(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return IsBinaryPortableExtracted(type);
	}

	public static bool IsBlittable(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return IsBlittableExtracted(type);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsBool(this Type type)
	{
		return type.GetNonNullable() == typeof(bool);
	}

	public static bool IsByRefParameter(this ParameterInfo parameterInfo)
	{
		if (parameterInfo == null)
		{
			throw new ArgumentNullException("parameterInfo");
		}
		return parameterInfo.IsByRefParameterInternal();
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsConstructedGenericType(this Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsConvertible(this Type type)
	{
		type = type.GetNonNullable();
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsEnum)
		{
			return true;
		}
		return type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong) || type == typeof(float) || type == typeof(double) || type == typeof(char);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsGenericInstanceOf(this Type type, Type genericTypeDefinition)
	{
		if (genericTypeDefinition == null)
		{
			throw new ArgumentNullException("genericTypeDefinition");
		}
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (!typeInfo.IsGenericType)
		{
			return false;
		}
		return type.GetGenericTypeDefinition() == genericTypeDefinition;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsInteger(this Type type)
	{
		type = type.GetNonNullable();
		return type.IsPrimitiveInteger();
	}

	public static bool IsInteger64(this Type type)
	{
		type = type.GetNonNullable();
		return !type.IsSameOrSubclassOfInternal(typeof(Enum)) && (type == typeof(long) || type == typeof(ulong));
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsIntegerOrBool(this Type type)
	{
		type = type.GetNonNullable();
		return type == typeof(bool) || type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsNullable(this Type type)
	{
		return Nullable.GetUnderlyingType(type) != null;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsNumeric(this Type type)
	{
		type = type.GetNonNullable();
		return type == typeof(char) || type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsNumericOrBool(this Type type)
	{
		type = type.GetNonNullable();
		return type == typeof(char) || type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong) || type == typeof(bool);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsPrimitiveInteger(this Type type)
	{
		return type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool IsSafeArray(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return type.IsArray && type.GetElementType()?.MakeArrayType() == type;
	}

	public static bool IsSameOrSubclassOf(this Type type, Type baseType)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (baseType == null)
		{
			throw new ArgumentNullException("baseType");
		}
		return type.IsSameOrSubclassOfInternal(baseType);
	}

	public static bool IsValueTypeRecursive(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return IsValueTypeRecursiveExtracted(type);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Type MakeNullable(this Type self)
	{
		return typeof(Nullable<>).MakeGenericType(self);
	}

	internal static Type GetNonRefTypeInternal(this Type type)
	{
		Type elementType = type.GetElementType();
		return (elementType != null && type.IsByRef) ? elementType : type;
	}

	internal static MethodInfo[] GetStaticMethodsInternal(this Type type)
	{
		MethodInfo[] array = RuntimeReflectionExtensions.GetRuntimeMethods(type).AsArray();
		List<MethodInfo> list = new List<MethodInfo>(array.Length);
		list.AddRange(array.Where((MethodInfo method) => method.IsStatic));
		return list.ToArray();
	}

	internal static bool IsByRefParameterInternal(this ParameterInfo parameterInfo)
	{
		if (parameterInfo.ParameterType.IsByRef)
		{
			return true;
		}
		return (parameterInfo.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out;
	}

	internal static bool IsFloatingPoint(this Type type)
	{
		type = type.GetNonNullable();
		return type == typeof(float) || type == typeof(double);
	}

	internal static bool IsSameOrSubclassOfInternal(this Type type, Type baseType)
	{
		return type == baseType || IntrospectionExtensions.GetTypeInfo(type).IsSubclassOf(baseType);
	}

	internal static bool IsUnsigned(this Type type)
	{
		type = type.GetNonNullable();
		return type == typeof(byte) || type == typeof(char) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);
	}

	internal static bool IsUnsignedInteger(this Type type)
	{
		type = type.GetNonNullable();
		return type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong);
	}

	private static bool DelegateEqualsExtracted(MethodInfo? delegateMethodInfo, object delegateTarget, MethodInfo? method, object target)
	{
		return EqualityComparer<MethodInfo>.Default.Equals(method, delegateMethodInfo) && delegateTarget == target;
	}

	private static bool GetBinaryPortableResult(Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsPrimitive)
		{
			return type != typeof(IntPtr) && type != typeof(UIntPtr) && type != typeof(char) && type != typeof(bool);
		}
		if (!typeInfo.IsValueType)
		{
			return false;
		}
		if (RuntimeReflectionExtensions.GetRuntimeFields(type).Any((FieldInfo field) => !IsBinaryPortableExtracted(field.FieldType)))
		{
			return false;
		}
		int result;
		if (!typeInfo.IsAutoLayout)
		{
			StructLayoutAttribute? structLayoutAttribute = type.GetStructLayoutAttribute();
			result = ((structLayoutAttribute != null && structLayoutAttribute.Pack > 0) ? 1 : 0);
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	private static bool GetBlittableResult(Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsPrimitive)
		{
			return type != typeof(char) && type != typeof(bool);
		}
		return typeInfo.IsValueType && RuntimeReflectionExtensions.GetRuntimeFields(type).All((FieldInfo field) => IsBlittableExtracted(field.FieldType));
	}

	private static bool GetValueTypeRecursiveResult(Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsPrimitive)
		{
			return true;
		}
		return typeInfo.IsValueType && RuntimeReflectionExtensions.GetRuntimeFields(type).All((FieldInfo field) => IsValueTypeRecursiveExtracted(field.FieldType));
	}

	private static bool IsBinaryPortableExtracted(Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (!typeInfo.IsValueType)
		{
			return false;
		}
		return _binaryPortableCache[type];
	}

	private static bool IsBlittableExtracted(Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (!typeInfo.IsValueType)
		{
			return false;
		}
		return _blittableCache[type];
	}

	private static bool IsValueTypeRecursiveExtracted(Type type)
	{
		TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (!typeInfo.IsValueType)
		{
			return false;
		}
		return _valueTypeRecursiveCache[type];
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static StructLayoutAttribute? GetStructLayoutAttribute(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return type.StructLayoutAttribute;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Type GetUnderlyingSystemType(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return type.UnderlyingSystemType;
	}
}