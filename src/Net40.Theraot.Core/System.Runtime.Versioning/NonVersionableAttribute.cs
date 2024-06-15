namespace System.Runtime.Versioning;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class NonVersionableAttribute : Attribute
{
}