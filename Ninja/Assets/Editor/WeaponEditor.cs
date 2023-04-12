using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    GUIStyle titleStyle = new GUIStyle();
    GUIStyle dividerStyle = new GUIStyle();
    Editor gameObjectEditor;

    private SerializedProperty sprite;
    private SerializedProperty shape;
    private SerializedProperty canDrop;

    private void OnEnable() 
    {
        sprite = serializedObject.FindProperty("_sprite");
        shape = serializedObject.FindProperty("_shape");
        canDrop = serializedObject.FindProperty("_canDrop");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        
        Weapon weapon = (Weapon)target;
        EditorUtility.SetDirty(weapon);

        serializedObject.Update();
        
        titleStyle.fontSize = 24;
        titleStyle.normal.textColor = Color.gray;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        // Weapon Name
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(weapon.itemName, titleStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);
        
        // Model Preview
        if(gameObjectEditor == null)
        gameObjectEditor = Editor.CreateEditor(weapon.model);

        if(weapon.model) gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), null);

        EditorGUILayout.Space(10);
        
        weapon.model = (Mesh)EditorGUILayout.ObjectField("Model", weapon.model, typeof(Mesh), true);
        weapon.material = (Material)EditorGUILayout.ObjectField("Material", weapon.material, typeof(Material), true);
        
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(sprite);
        EditorGUILayout.PropertyField(shape);
        EditorGUILayout.PropertyField(canDrop);

        weapon.itemName = EditorGUILayout.DelayedTextField("Name", weapon.itemName);

        EditorGUILayout.Space(10);

        weapon.cost = EditorGUILayout.IntField("Cost", weapon.cost);
        weapon.damage = EditorGUILayout.IntField("Damage", weapon.damage);
        weapon.amount = EditorGUILayout.IntSlider("Ammo", weapon.amount, 1, 99);
        weapon.durability = EditorGUILayout.Slider("Durability", weapon.durability, 0, 1);
        weapon.stackable = EditorGUILayout.Toggle("Stackable", weapon.stackable);
        weapon.WeaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weapon.WeaponType);
        //EditorGUI.HelpBox(new Rect(100,100, 50, 50), "Test", MessageType.None);

        EditorGUILayout.Space(10);

        weapon.pickup = (GameObject)EditorGUILayout.ObjectField("Pickup", weapon.pickup, typeof(GameObject), true);

        EditorGUILayout.Space(10);

        weapon.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", weapon.projectile, typeof(GameObject), true);

        serializedObject.ApplyModifiedProperties();
    }

    public void WindowUpdate(int id)
    {
        GUILayout.Button("Hello");
    }

}
