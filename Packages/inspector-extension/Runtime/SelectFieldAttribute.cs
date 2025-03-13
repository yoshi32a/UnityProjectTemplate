using System;
using System.Diagnostics;
using UnityEngine;

namespace InspectorExtension
{
/// <summary>
/// 指定した型のフィールドを一覧として表示し設定します。
/// </summary>
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class SelectFieldAttribute : PropertyAttribute
{
    public readonly Type Type;

    public SelectFieldAttribute(Type type)
    {
        Type = type;
    }
}
}
