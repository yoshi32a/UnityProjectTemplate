using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace InspectorExtension.Editor
{
[CustomPropertyDrawer(typeof(ReadonlyAttribute))]
public class ReadonlyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var filed = new PropertyField(property);
        filed.SetEnabled(false);
        return filed;
    }
}
}
