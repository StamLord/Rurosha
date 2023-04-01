using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(WorldDialogueManager))]
public class WorldDialogueManagerEditor : Editor
{
    private GUIStyle titleStyle = new GUIStyle();
    
    private string debugMessage = "Test";
    private float debugDuration = 20;
    private Transform debugTarget;

    public override void OnInspectorGUI()
    {
        WorldDialogueManager manager = (WorldDialogueManager)target;
        EditorUtility.SetDirty(manager);

        titleStyle.fontSize = 24;
        titleStyle.normal.textColor = Color.gray;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.LabelField("Prefab", titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);

        manager.messagePrefab = (GameObject)EditorGUILayout.ObjectField("TextMesh", manager.messagePrefab, typeof(GameObject), true);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Pixel Offset", titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);

        manager.verticalPixelOffset = EditorGUILayout.FloatField("Vertical Pixel Offset", manager.verticalPixelOffset);
        manager.verticalTransformOffset = EditorGUILayout.FloatField("Vertical Transform Offset", manager.verticalTransformOffset);
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Distance Scaling", titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);

        manager.minScale = EditorGUILayout.FloatField("Minimum Scale", manager.minScale);
        manager.maxScale = EditorGUILayout.FloatField("Maximum Scale", manager.maxScale);

        manager.minDistance = EditorGUILayout.FloatField("Minimum Distance", manager.minDistance);
        manager.maxDistance = EditorGUILayout.FloatField("Maximum Distance", manager.maxDistance);

        // Debug 
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug", titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);

        debugMessage = EditorGUILayout.TextField("Debug Message", debugMessage);
        debugDuration = EditorGUILayout.FloatField("Duration", debugDuration);
        debugTarget = (Transform)EditorGUILayout.ObjectField("Target", debugTarget, typeof(Transform), true);

        EditorGUILayout.Space(10);
        
        if(GUILayout.Button("Debug Message"))
            manager.NewMessage(debugMessage, debugDuration, debugTarget);
    }
}
