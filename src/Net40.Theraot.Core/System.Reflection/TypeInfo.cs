using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Reflection;

public class TypeInfo : Type, IReflectableType
{
    private static readonly MethodInfo _methodGetAttributeFlagsImpl =
        typeof(Type).GetMethod("GetAttributeFlagsImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodGetConstructorImpl =
        typeof(Type).GetMethod("GetConstructorImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodGetMember = typeof(Type).GetMethod("GetMember",
        BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[3]
            {
                typeof(string),
                typeof(MemberTypes),
                typeof(BindingFlags)
            }, null);

    private static readonly MethodInfo _methodGetMethodImpl =
        typeof(Type).GetMethod("GetMethodImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodGetPropertyImpl =
        typeof(Type).GetMethod("GetPropertyImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodHasElementTypeImpl =
        typeof(Type).GetMethod("HasElementTypeImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodIsArrayImpl =
        typeof(Type).GetMethod("IsArrayImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodIsByRefImpl =
        typeof(Type).GetMethod("IsByRefImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodIsComObjectImpl =
        typeof(Type).GetMethod("IsCOMObjectImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodIsContextfulImpl =
        typeof(Type).GetMethod("IsContextfulImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodIsMarshalByRefImpl =
        typeof(Type).GetMethod("IsMarshalByRefImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodIsPointerImpl =
        typeof(Type).GetMethod("IsPointerImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodIsPrimitiveImpl =
        typeof(Type).GetMethod("IsPrimitiveImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo _methodIsValueTypeImpl =
        typeof(Type).GetMethod("IsValueTypeImpl", BindingFlags.Instance | BindingFlags.NonPublic);

    private readonly Type _type;

    public override Assembly Assembly => _type.Assembly;

    public override string AssemblyQualifiedName => _type.AssemblyQualifiedName;

    public override Type BaseType => _type.BaseType;

    public override bool ContainsGenericParameters => _type.ContainsGenericParameters;

    public virtual IEnumerable<ConstructorInfo> DeclaredConstructors { get; }

    public virtual IEnumerable<EventInfo> DeclaredEvents { get; }

    public virtual IEnumerable<FieldInfo> DeclaredFields { get; }

    public virtual IEnumerable<MemberInfo> DeclaredMembers { get; }

    public virtual IEnumerable<MethodInfo> DeclaredMethods { get; }

    public virtual IEnumerable<TypeInfo> DeclaredNestedTypes { get; }

    public virtual IEnumerable<PropertyInfo> DeclaredProperties { get; }

    public override MethodBase DeclaringMethod => _type.DeclaringMethod;

    public override Type DeclaringType => _type.DeclaringType;

    public override string FullName => _type.FullName;

    public override GenericParameterAttributes GenericParameterAttributes => _type.GenericParameterAttributes;

    public override int GenericParameterPosition => _type.GenericParameterPosition;

    public new IEnumerable<Type> GenericTypeArguments => _type.GetGenericArguments();

    public virtual Type[] GenericTypeParameters { get; }

    public override Guid GUID => _type.GUID;

    public virtual IEnumerable<Type> ImplementedInterfaces { get; }

    public override bool IsGenericParameter => _type.IsGenericParameter;

    public override bool IsGenericType => _type.IsGenericType;

    public override bool IsGenericTypeDefinition => _type.IsGenericTypeDefinition;

    public override MemberTypes MemberType => _type.MemberType;

    public override int MetadataToken => _type.MetadataToken;

    public override Module Module => _type.Module;

    public override string Name => _type.Name;

    public override string Namespace => _type.Namespace;

    public override Type ReflectedType => _type.ReflectedType;

    public override StructLayoutAttribute StructLayoutAttribute => _type.StructLayoutAttribute;

    public override RuntimeTypeHandle TypeHandle => _type.TypeHandle;

    public new ConstructorInfo TypeInitializer => _type.TypeInitializer;

    public override Type UnderlyingSystemType => _type.UnderlyingSystemType;

    internal TypeInfo(Type type)
    {
            _type = type;
            DeclaredConstructors = type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                        BindingFlags.Static | BindingFlags.Public |
                                                        BindingFlags.NonPublic);
            DeclaredEvents = type.GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static |
                                            BindingFlags.Public | BindingFlags.NonPublic);
            DeclaredFields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static |
                                            BindingFlags.Public | BindingFlags.NonPublic);
            DeclaredMembers = type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static |
                                              BindingFlags.Public | BindingFlags.NonPublic);
            DeclaredMethods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static |
                                              BindingFlags.Public | BindingFlags.NonPublic);
            DeclaredNestedTypes = (from nt in type.GetNestedTypes(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                                  BindingFlags.Static | BindingFlags.Public |
                                                                  BindingFlags.NonPublic)
                select new TypeInfo(nt)).ToList().AsReadOnly();
            DeclaredProperties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            GenericTypeParameters = type.GetGenericArguments();
            ImplementedInterfaces = type.GetInterfaces();
        }

    public virtual Type AsType()
    {
            return _type;
        }

    public override bool Equals(object other)
    {
            return _type.Equals(other);
        }


    /*public static bool operator ==(TypeInfo left, TypeInfo right)
    {
        if (left == null && right == null)
        {
            return true;
        }
        return left._type.Equals(right._type);
    }


    public static bool operator !=(TypeInfo left, TypeInfo right)
    {
        return left._type != right._type;
    }*/

    public override Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
    {
            return _type.FindInterfaces(filter, filterCriteria);
        }

    public override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter,
        object filterCriteria)
    {
            return _type.FindMembers(memberType, bindingAttr, filter, filterCriteria);
        }

    public override int GetArrayRank()
    {
            return _type.GetArrayRank();
        }

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
    {
            return _type.GetConstructors(bindingAttr);
        }

    public override object[] GetCustomAttributes(bool inherit)
    {
            return _type.GetCustomAttributes(inherit);
        }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
            return _type.GetCustomAttributes(attributeType, inherit);
        }

    public virtual EventInfo? GetDeclaredEvent(string name)
    {
            return DeclaredEvents.FirstOrDefault((EventInfo e) =>
                string.Equals(e.Name, name, StringComparison.Ordinal));
        }

    public virtual FieldInfo? GetDeclaredField(string name)
    {
            return DeclaredFields.FirstOrDefault((FieldInfo f) =>
                string.Equals(f.Name, name, StringComparison.Ordinal));
        }

    public virtual MethodInfo? GetDeclaredMethod(string name)
    {
            return DeclaredMethods.FirstOrDefault((MethodInfo m) =>
                string.Equals(m.Name, name, StringComparison.Ordinal));
        }

    public virtual IEnumerable<MethodInfo> GetDeclaredMethods(string name)
    {
            return DeclaredMethods.Where((MethodInfo m) => string.Equals(m.Name, name, StringComparison.Ordinal));
        }

    public virtual TypeInfo? GetDeclaredNestedType(string name)
    {
            return (from nt in _type.GetNestedTypes()
                where string.Equals(nt.Name, name, StringComparison.Ordinal)
                select new TypeInfo(nt)).FirstOrDefault();
        }

    public virtual PropertyInfo? GetDeclaredProperty(string name)
    {
            return DeclaredProperties.FirstOrDefault((PropertyInfo p) =>
                string.Equals(p.Name, name, StringComparison.Ordinal));
        }

    public override MemberInfo[] GetDefaultMembers()
    {
            return _type.GetDefaultMembers();
        }

    public override Type GetElementType()
    {
            return _type.GetElementType();
        }

    public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
    {
            return _type.GetEvent(name, bindingAttr);
        }

    public override EventInfo[] GetEvents(BindingFlags bindingAttr)
    {
            return _type.GetEvents(bindingAttr);
        }

    public override EventInfo[] GetEvents()
    {
            return _type.GetEvents();
        }

    public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
    {
            return _type.GetField(name, bindingAttr);
        }

    public override FieldInfo[] GetFields(BindingFlags bindingAttr)
    {
            return _type.GetFields(bindingAttr);
        }

    public override Type[] GetGenericArguments()
    {
            return _type.GetGenericArguments();
        }

    public override Type[] GetGenericParameterConstraints()
    {
            return _type.GetGenericParameterConstraints();
        }

    public override Type GetGenericTypeDefinition()
    {
            return _type.GetGenericTypeDefinition();
        }

    public override int GetHashCode()
    {
            return _type.GetHashCode();
        }

    public override Type GetInterface(string name, bool ignoreCase)
    {
            return _type.GetInterface(name, ignoreCase);
        }

    public override InterfaceMapping GetInterfaceMap(Type interfaceType)
    {
            return _type.GetInterfaceMap(interfaceType);
        }

    public override Type[] GetInterfaces()
    {
            return _type.GetInterfaces();
        }

    public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
    {
            return _type.GetMember(name, bindingAttr);
        }

    public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
    {
            return (MemberInfo[])_methodGetMember.Invoke(type, new object[3] { name, type, bindingAttr });
        }

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
    {
            return _type.GetMembers(bindingAttr);
        }

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
    {
            return _type.GetMethods(bindingAttr);
        }

    public override Type GetNestedType(string name, BindingFlags bindingAttr)
    {
            return _type.GetNestedType(name, bindingAttr);
        }

    public override Type[] GetNestedTypes(BindingFlags bindingAttr)
    {
            return _type.GetNestedTypes(bindingAttr);
        }

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
    {
            return _type.GetProperties(bindingAttr);
        }

    public TypeInfo GetTypeInfo()
    {
            return this;
        }

    public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
        object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
    {
            return _type.InvokeMember(name, invokeAttr, binder, target, args, CultureInfo.CurrentCulture);
        }

    public override bool IsAssignableFrom(Type c)
    {
            return _type.IsAssignableFrom(c);
        }

    public virtual bool IsAssignableFrom(TypeInfo? typeInfo)
    {
            return (object)typeInfo != null && _type.IsAssignableFrom(typeInfo._type);
        }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
            return _type.IsDefined(attributeType, inherit);
        }

    public override bool IsInstanceOfType(object o)
    {
            return _type.IsInstanceOfType(o);
        }

    public override bool IsSubclassOf(Type c)
    {
            return _type.IsSubclassOf(c);
        }

    public override Type MakeArrayType()
    {
            return _type.MakeArrayType();
        }

    public override Type MakeArrayType(int rank)
    {
            return _type.MakeArrayType(rank);
        }

    public override Type MakeByRefType()
    {
            return _type.MakeByRefType();
        }

    public override Type MakeGenericType(params Type[] typeArguments)
    {
            return _type.MakeGenericType(typeArguments);
        }

    public override Type MakePointerType()
    {
            return _type.MakePointerType();
        }

    public override string ToString()
    {
            return _type.ToString();
        }

    protected override TypeAttributes GetAttributeFlagsImpl()
    {
            return (TypeAttributes)_methodGetAttributeFlagsImpl.Invoke(_type, null);
        }

    protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder,
        CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
    {
            return (ConstructorInfo)_methodGetConstructorImpl.Invoke(_type,
                new object[5] { bindingAttr, binder, callConvention, _type, modifiers });
        }

    protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder,
        CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
    {
            return (MethodInfo)_methodGetMethodImpl.Invoke(_type,
                new object[6] { name, bindingAttr, binder, callConvention, _type, modifiers });
        }

    protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder,
        Type returnType, Type[] types, ParameterModifier[] modifiers)
    {
            return (PropertyInfo)_methodGetPropertyImpl.Invoke(_type,
                new object[6] { name, bindingAttr, binder, returnType, _type, modifiers });
        }

    protected override bool HasElementTypeImpl()
    {
            return (bool)_methodHasElementTypeImpl.Invoke(_type, null);
        }

    protected override bool IsArrayImpl()
    {
            return (bool)_methodIsArrayImpl.Invoke(_type, null);
        }

    protected override bool IsByRefImpl()
    {
            return (bool)_methodIsByRefImpl.Invoke(_type, null);
        }

    protected override bool IsCOMObjectImpl()
    {
            return (bool)_methodIsComObjectImpl.Invoke(_type, null);
        }

    protected override bool IsContextfulImpl()
    {
            return (bool)_methodIsContextfulImpl.Invoke(_type, null);
        }

    protected override bool IsMarshalByRefImpl()
    {
            return (bool)_methodIsMarshalByRefImpl.Invoke(_type, null);
        }

    protected override bool IsPointerImpl()
    {
            return (bool)_methodIsPointerImpl.Invoke(_type, null);
        }

    protected override bool IsPrimitiveImpl()
    {
            return (bool)_methodIsPrimitiveImpl.Invoke(_type, null);
        }

    protected override bool IsValueTypeImpl()
    {
            return (bool)_methodIsValueTypeImpl.Invoke(_type, null);
        }
}