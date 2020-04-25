using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoadAccessories))]
public class LoadAccessoryInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Changed");
        ((LoadAccessories)target).petBasic = (GameObject)EditorGUILayout.ObjectField("pet basic", ((LoadAccessories)target).petBasic, typeof(GameObject), false, GUILayout.Width(300));
        if (GUILayout.Button("Load", GUILayout.Width(200)))
        {
            var sourcePath = AssetDatabase.GetAssetPath(((LoadAccessories)target).petBasic);
            sourcePath = sourcePath.Replace(".prefab", "");
            var copyPath = sourcePath + "_Accessory_" + (1).ToString() + ".prefab";
            if (GameObject.FindObjectOfType<CharController>())
            {
                GameObject.FindObjectOfType<CharController>().petPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(copyPath);
            }
        }
    }
}
