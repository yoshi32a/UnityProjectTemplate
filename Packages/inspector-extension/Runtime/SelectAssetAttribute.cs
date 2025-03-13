using System;
using System.Diagnostics;
using UnityEngine;

namespace InspectorExtension
{
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class SelectAssetAttribute : PropertyAttribute
{
    public readonly Type Type;

    public SelectAssetAttribute(Type type)
    {
        Type = type;
    }
}
}
