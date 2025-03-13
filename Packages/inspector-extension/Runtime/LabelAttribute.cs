using System;
using System.Diagnostics;
using UnityEngine;

namespace InspectorExtension
{
/// <summary>
/// インスペクターのラベルに文字列を設定します。
/// </summary>
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class LabelAttribute : PropertyAttribute
{
    public readonly string Label;

    public LabelAttribute(string label)
    {
        Label = label;
    }
}
}
