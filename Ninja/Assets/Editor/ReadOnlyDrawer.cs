using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var prevGUIState = GUI.enabled;
        GUI.enabled = false;

        EditorGUI.PropertyField(position, property, label);

        GUI.enabled = prevGUIState;
    }
}
