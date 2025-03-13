using System;
using System.Diagnostics;

namespace InspectorExtension
{
/// <summary>
/// ReferenceSlectorで表示名につかうメタ情報
/// </summary>
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class ReferenceMetaAttribute : Attribute, IEquatable<ReferenceMetaAttribute>
{
    /// <summary>表示用</summary>
    public string Name { get; }

    /// <summary>表示順</summary>
    public int Order { get; }

    public ReferenceMetaAttribute(string name, int order = 0)
    {
        Name = name;
        Order = order;
    }

    public bool Equals(ReferenceMetaAttribute other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Name == other.Name && Order == other.Order;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is ReferenceMetaAttribute other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Name, Order);
    }

    public override string ToString()
    {
        return $"{Name}";
    }
}
}
