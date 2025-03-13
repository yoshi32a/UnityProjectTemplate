using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InspectorExtension.Editor
{
[CustomPropertyDrawer(typeof(ReferenceSelectorAttribute))]
public class ReferenceSelectorDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var attr = (ReferenceSelectorAttribute)attribute;
        var root = new VisualElement();

        // attributeのついてるフィールドのTypeを取得する
        var fieldTypeInfo = property.managedReferenceFieldTypename.Split(" ");
        var searchType = Type.GetType($"{fieldTypeInfo[1]},{fieldTypeInfo[0]}");
        // 派生タイプをすべて取得する
        var typeTable = new Dictionary<ReferenceMetaAttribute, Type>();
        foreach (var t in TypeCache.GetTypesDerivedFrom(searchType))
        {
            if (!t.Attributes.HasFlag(TypeAttributes.Serializable))
            {
                Debug.Log($"[Serializable]がついてないので表示されません {t}");
                continue;
            }

            if (t.IsAbstract)
            {
                continue;
            }

            var meta = t.GetCustomAttribute<ReferenceMetaAttribute>();
            typeTable.Add(meta != null ? meta : new ReferenceMetaAttribute(t.Name), t);
        }

        var labelTable = typeTable.ToDictionary(x => x.Value, x => x.Key);
        var types = typeTable.Keys.OrderByDescending(x => x.Order).ToList();
        var popup = new PopupField<ReferenceMetaAttribute>(property.name,
            types,
            0
        );
        popup.RegisterValueChangedCallback(evt =>
        {
            property.managedReferenceValue = Activator.CreateInstance(typeTable[evt.newValue]);
            property.serializedObject.ApplyModifiedProperties();
        });

        if (property.managedReferenceValue != null)
        {
            // すでに存在する型があるなら選択してるindexに変更
            var typeName = labelTable[property.managedReferenceValue.GetType()];
            popup.index = types.IndexOf(typeName);
        }
        else
        {
            // 型が1つでもあるなら…
            if (typeTable.Count > 0)
            {
                property.managedReferenceValue = Activator.CreateInstance(typeTable[types[0]]);
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        root.Add(popup);
        // 選択されているオブジェクトを描画する
        var field = new PropertyField(property);
        if (!string.IsNullOrEmpty(attr.Label))
        {
            field.label = attr.Label;
        }

        root.Add(field);

        return root;
    }
}
}
