using System;
using InspectorExtension;
using UnityEngine;

[CreateAssetMenu(fileName = "InspectorExtensionAttributes", menuName = "Scriptable Objects/InspectorExtensionAttributes")]
internal class InspectorExtensionAttributes : ScriptableObject
{
    [Label("テストラベル")] public int key1 = 0;
    [Readonly] public int key2 = 1;
    [Readonly] public GameObject prefab2;

    [SerializeReference, ReferenceSelector("ラベル")]
    IAttributeTest attributeTest;
}

internal interface IAttributeTest
{
}

[Serializable]
internal abstract class AttributeTestBase : IAttributeTest
{
}

[Serializable, ReferenceMeta(nameof(AttributeTest1), order: 1)]
internal class AttributeTest1 : AttributeTestBase
{
}

[Serializable, ReferenceMeta("テスト文字列")]
internal class AttributeTest2 : AttributeTestBase
{
    [Label("label1")] public int value1;
}

[Serializable, ReferenceMeta("優先度高", order: 1000)]
internal class AttributeTest3 : AttributeTestBase
{
    [Label("hoge")] public int value1;
}
