using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace InspectorExtension.Editor
{
[CustomPropertyDrawer(typeof(SelectAssetAttribute))]
public class SelectAssetDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var selectFieldAttribute = (SelectAssetAttribute)attribute;
        var root = new VisualElement();

        var label = property.displayName;
        if (fieldInfo.IsDefined(typeof(LabelAttribute)))
        {
            var labelAttribute = (LabelAttribute)fieldInfo.GetCustomAttribute(typeof(LabelAttribute));
            label = labelAttribute.Label;
        }

        var popupField = new PopupField<string>(label: label);
        var assets = AssetDatabase.FindAssets($"t:{selectFieldAttribute.Type.Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(path => AssetDatabase.LoadAssetAtPath(path, selectFieldAttribute.Type))
            .ToArray();

        var choiceList = assets.Select(asset => asset.name).ToList();
        popupField.choices = choiceList;
        popupField.index = ArrayUtility.IndexOf(assets, property.objectReferenceValue);
        popupField.RegisterValueChangedCallback(callback: evt =>
        {
            property.objectReferenceValue = assets[choiceList.IndexOf(evt.newValue)];
            property.serializedObject.ApplyModifiedProperties();
        });
        root.Add(popupField);

        return root;
    }
}
}
