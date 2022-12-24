using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(Spawner))]
[CanEditMultipleObjects]
public class SpawnerEditor : Editor 
{
    SerializedProperty player;
    SerializedProperty camera;
    SerializedProperty encounters;
    SerializedProperty encounterChance;
    SerializedProperty tickRate;
    SerializedProperty gracePeriod;

    private float[] oldEncounterChance;

    private void OnEnable() 
    {
        player = serializedObject.FindProperty("player");
        camera = serializedObject.FindProperty("camera");
        encounters = serializedObject.FindProperty("encounters");
        encounterChance = serializedObject.FindProperty("encounterChance");
        tickRate = serializedObject.FindProperty("tickRate");
        gracePeriod = serializedObject.FindProperty("gracePeriod");
    }

    public override void OnInspectorGUI() 
    {
        // Get our edited object
        Spawner spawner = (Spawner)target;

        // Check if encounterChange length needs to change
        if(spawner.encounterChance.Length != spawner.encounters.Length)
            Array.Resize<float>(ref spawner.encounterChance, spawner.encounters.Length);

        // Instantiate oldEncounterChance if needed
        if(oldEncounterChance == null)
            oldEncounterChance = new float[spawner.encounterChance.Length];
        
        // Recopy if encounters length doesn't match
        if(spawner.encounterChance.Length != oldEncounterChance.Length)
        {
            oldEncounterChance = new float[spawner.encounterChance.Length];
            Array.Copy(spawner.encounterChance, oldEncounterChance, spawner.encounterChance.Length);
        }

        // Make sure no member is above 1 (100%) and round to 2 decimal points
        for (int i = 0; i < spawner.encounterChance.Length; i++)
            spawner.encounterChance[i] = Mathf.Clamp01(spawner.encounterChance[i]);

        // Check if sum of encounterChange members is more than 1 (100%)
        float sum = 0;
        for (int i = 0; i < spawner.encounterChance.Length; i++)
            sum += spawner.encounterChance[i];

        // Resize all values except current edited
        if(sum > 1)
        {
            float diff = sum - 1;
            // We divide the diff by length minus 1 since they will be substracted from all except the one changed
            float subtract = diff / (spawner.encounterChance.Length -1); 
            
            // We find which value was changed
            int i = 0;
            for (; i < spawner.encounterChance.Length; i++)
            {
                if(spawner.encounterChance[i] != oldEncounterChance[i])
                    continue;
                else
                    spawner.encounterChance[i] -= subtract;
            }

            // Update oldEncounterChance for next update
            Array.Copy(spawner.encounterChance, oldEncounterChance, spawner.encounterChance.Length);
        }

        serializedObject.Update();

        EditorGUILayout.PropertyField(player);
        EditorGUILayout.PropertyField(camera);
        EditorGUILayout.PropertyField(tickRate);
        EditorGUILayout.PropertyField(gracePeriod);
        EditorGUILayout.PropertyField(encounters);
        //EditorGUILayout.PropertyField(encounterChance);
        
        // Display members of encounterChance as sliders
        for (int i = 0; i < spawner.encounterChance.Length; i++)
        {
            SerializedProperty prop = encounterChance.GetArrayElementAtIndex(i);
            prop.floatValue = EditorGUILayout.Slider(prop.floatValue, 0, 1);
        }

        // Display chance of nothing happening
        EditorGUILayout.LabelField("Chance of nothing happening: " + (1f - sum));

        serializedObject.ApplyModifiedProperties();
    }
}