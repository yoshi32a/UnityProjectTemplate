using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace InspectorExtension.Editor
{
[CustomPropertyDrawer(typeof(SelectFieldAttribute))]
public class SelectFieldDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var selectFieldAttribute = (SelectFieldAttribute)attribute;
        var root = new VisualElement();

        if (!property.type.Equals("String", StringComparison.OrdinalIgnoreCase))
        {
            var box = new Box();
            box.Add(new PropertyField(property));
            box.Add(new HelpBox("SelectFieldAttributeはStringのみサポートしています。", HelpBoxMessageType.Error));
            root.Add(box);

            return root;
        }

        var label = property.displayName;
        if (fieldInfo.IsDefined(typeof(LabelAttribute)))
        {
            var labelAttribute = (LabelAttribute)fieldInfo.GetCustomAttribute(typeof(LabelAttribute));
            label = labelAttribute.Label;
        }

        var popupField = new PopupField<string>(label: label);
        var fields = selectFieldAttribute.Type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
        var choiceList = fields
            .Where(f => string.Equals(f.FieldType.Name, property.type, StringComparison.OrdinalIgnoreCase))
            .Select(f => f.Name)
            .ToList();
        popupField.choices = choiceList;
        popupField.index = choiceList.IndexOf(property.stringValue);
        popupField.RegisterValueChangedCallback(callback: evt =>
        {
            property.stringValue = evt.newValue;
            property.serializedObject.ApplyModifiedProperties();
        });
        root.Add(popupField);

        return root;
    }
}
}
