using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace InspectorExtension.Editor
{
[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var name = (LabelAttribute)attribute;
        return new PropertyField(property, name.Label);
    }
}
}
