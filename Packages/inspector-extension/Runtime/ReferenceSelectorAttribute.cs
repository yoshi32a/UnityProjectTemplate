using System;
using System.Diagnostics;
using UnityEngine;

namespace InspectorExtension
{
/// <summary>
/// SerializedReferenceと一緒に使い、インターフェースの継承先のインスタンスを選択できるようにします。
/// </summary>
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class ReferenceSelectorAttribute : PropertyAttribute
{
    public readonly string Label;

    public ReferenceSelectorAttribute(string label)
    {
        Label = label;
    }
}
}
