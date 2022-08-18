using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Equipment))]
public class EquipmentEditor : Editor
{
    GUIStyle titleStyle = new GUIStyle();
    GUIStyle dividerStyle = new GUIStyle();
    Editor gameObjectEditor;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        
        Equipment equipment = (Equipment)target;
        EditorUtility.SetDirty(equipment);

        titleStyle.fontSize = 24;
        titleStyle.normal.textColor = Color.gray;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        // Equipment Name
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(equipment.itemName, titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);
        
        // Model Preview
        if(gameObjectEditor == null)
        gameObjectEditor = Editor.CreateEditor(equipment.model);

        if(equipment.model) gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), null);

        EditorGUILayout.Space(10);

        // Stats
        equipment.equipmentType = (EquipmentType)EditorGUILayout.EnumPopup("Equipment Type", equipment.equipmentType);
        equipment.equipmentLayer = (EquipmentLayer)EditorGUILayout.EnumPopup("Equipment Layer", equipment.equipmentLayer);
        
        EditorGUILayout.Space(10);
        
        equipment.model = (Mesh)EditorGUILayout.ObjectField("Model", equipment.model, typeof(Mesh), true);
        equipment.itemName = EditorGUILayout.DelayedTextField("Name", equipment.itemName);
        equipment.visualIndex = EditorGUILayout.IntSlider("Visual", equipment.visualIndex, 0, 100);

        EditorGUILayout.Space(10);

        equipment.bluntDefense = EditorGUILayout.IntSlider("Blunt", equipment.bluntDefense, 1, 10);
        equipment.slashDefense = EditorGUILayout.IntSlider("Slash", equipment.slashDefense, 1, 10);
        equipment.pierceDefense = EditorGUILayout.IntSlider("Pierce", equipment.pierceDefense, 1, 10);

        EditorGUILayout.Space(10);

        equipment.pickup = (GameObject)EditorGUILayout.ObjectField("Pickup", equipment.pickup, typeof(GameObject), true);

        //EditorGUI.HelpBox(new Rect(100,100, 50, 50), "Test", MessageType.None);

    }

    public void WindowUpdate(int id)
    {
        GUILayout.Button("Hello");
    }

}
