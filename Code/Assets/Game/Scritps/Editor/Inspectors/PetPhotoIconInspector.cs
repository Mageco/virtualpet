using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PetPhotoIcon))]
public class PetPhotoIconInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Changed");

        if (GUILayout.Button("Add Pet", GUILayout.Width(200)))
        {
            ((PetPhotoIcon)target).petBasic = ArrayHelper.Add(null, ((PetPhotoIcon)target).petBasic);
        }

        for (int i=0;i< ((PetPhotoIcon)target).petBasic.Length; i++)
        {
            GUILayout.BeginHorizontal();
            ((PetPhotoIcon)target).petBasic[i] = (GameObject)EditorGUILayout.ObjectField("pet " + (i+1).ToString(), ((PetPhotoIcon)target).petBasic[i], typeof(GameObject), false, GUILayout.Width(300));
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                ((PetPhotoIcon)target).petBasic = ArrayHelper.Remove(i, ((PetPhotoIcon)target).petBasic);
            }
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Load Accessory", GUILayout.Width(200)))
        {
            for(int i=0;i< ((PetPhotoIcon)target).petBasic.Length; i++)
            {
                var sourcePath = AssetDatabase.GetAssetPath(((PetPhotoIcon)target).petBasic[i]);
                sourcePath = sourcePath.Replace(".prefab", "");
                for (int n = 0; n < 20; n++)
                {
                    var copyPath = sourcePath + "_Accessory_" + (n + 1).ToString() + ".prefab";
                    Debug.Log(copyPath);
                    if (GameObject.Find(((PetPhotoIcon)target).petBasic[i].name + "_Accessory_" + (n + 1).ToString()))
                        DestroyImmediate(GameObject.Find(((PetPhotoIcon)target).petBasic[i].name + "_Accessory_" + (n + 1).ToString()));
                    GameObject sourceObject = AssetDatabase.LoadAssetAtPath<GameObject>(copyPath);
                    if(sourceObject != null)
                    {
                        GameObject copyObject = Instantiate(sourceObject) as GameObject;
                        copyObject.name = ((PetPhotoIcon)target).petBasic[i].name + "_Accessory_" + (n + 1).ToString();
                    }
                }
            }
        }
    }
}