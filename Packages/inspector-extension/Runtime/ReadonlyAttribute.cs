using System;
using System.Diagnostics;
using UnityEngine;

namespace InspectorExtension
{
/// <summary>
/// インスペクターのフィールドを読み取り専用にします
/// </summary>
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class ReadonlyAttribute : PropertyAttribute
{
}
}
