using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraShaker))]
public class CameraShakerEditor : Editor 
{
    private SerializedProperty camera;
    private SerializedProperty strength;
    private SerializedProperty duration;
    private SerializedProperty frequency;
    private SerializedProperty shakeType;

    private GUIStyle titleStyle = new GUIStyle();
    private GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

    private void OnEnable() 
    {
        camera = serializedObject.FindProperty("camera");
        strength = serializedObject.FindProperty("strength");
        duration = serializedObject.FindProperty("duration");
        frequency = serializedObject.FindProperty("frequency");
        shakeType = serializedObject.FindProperty("shakeType");
    }

    public override void OnInspectorGUI() 
    {
        CameraShaker shaker = (CameraShaker)target;

        titleStyle.fontSize = 24;
        titleStyle.normal.textColor = Color.gray;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;

        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Camera Reference", titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);

        EditorGUILayout.ObjectField(camera);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug", titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(strength);
        EditorGUILayout.PropertyField(duration);
        EditorGUILayout.PropertyField(frequency);
        EditorGUILayout.PropertyField(shakeType);
        
        EditorGUILayout.Space(10);

        GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
                if(GUILayout.Button("Shake", buttonStyle, new GUILayoutOption[] {GUILayout.Width(96), GUILayout.Height(60)}))
                {
                    if(Application.isPlaying)
                        shaker.StartShake(strength.floatValue, duration.floatValue, frequency.floatValue);
                }
            GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal ();
        
        serializedObject.ApplyModifiedProperties();
    }
}