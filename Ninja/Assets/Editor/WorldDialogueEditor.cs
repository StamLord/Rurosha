using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(WorldDialogue))]
public class WorldDialogueEditor : Editor
{
    private GUIStyle titleStyle = new GUIStyle();
    private string debugMessage;
    private float debugDuration;

    public override void OnInspectorGUI()
    {
        WorldDialogue dialogue = (WorldDialogue)target;
        EditorUtility.SetDirty(dialogue);

        titleStyle.fontSize = 24;
        titleStyle.normal.textColor = Color.gray;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        // References
        dialogue.text = (TextMeshProUGUI)EditorGUILayout.ObjectField("TextMesh", dialogue.text, typeof(Mesh), true);
        dialogue.animator = (Animator)EditorGUILayout.ObjectField("Animator", dialogue.animator, typeof(Material), true);

        // Debug 
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug", titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);

        debugMessage = EditorGUILayout.TextField("Debug Message");
        debugDuration = EditorGUILayout.FloatField("Duration", 1f);

        if(GUILayout.Button("Test"))
            dialogue.NewMessage(debugMessage, debugDuration);
    }
}
