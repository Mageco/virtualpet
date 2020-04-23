using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DuplicateCharacter))]
public class DuplicateCharacterInspector : Editor
{
    public override void OnInspectorGUI()
    {
    	Undo.RecordObject (target, "Changed");
        ((DuplicateCharacter)target).count = EditorGUILayout.IntField("Total Accessory", ((DuplicateCharacter)target).count, GUILayout.Width(200));
        ((DuplicateCharacter)target).petBasic = (GameObject)EditorGUILayout.ObjectField("pet basic", ((DuplicateCharacter)target).petBasic, typeof(GameObject), false, GUILayout.Width(300));
        ((DuplicateCharacter)target).petTarget = (GameObject)EditorGUILayout.ObjectField("pet color", ((DuplicateCharacter)target).petTarget, typeof(GameObject), false, GUILayout.Width(300));

        if (GUILayout.Button("Duplicate", GUILayout.Width(200)))
        {
            var sourcePath = AssetDatabase.GetAssetPath(((DuplicateCharacter)target).petBasic);
            sourcePath = sourcePath.Replace(".prefab", "");

            var targetPath = AssetDatabase.GetAssetPath(((DuplicateCharacter)target).petTarget);
            targetPath = targetPath.Replace(".prefab", "");

            Debug.Log(sourcePath);
            for(int n = 1;n< ((DuplicateCharacter)target).count + 1; n++)
            {
                var copyPath = sourcePath + "_Accessory_" + n.ToString() + ".prefab";
                Debug.Log(copyPath);
                GameObject sourceObject = AssetDatabase.LoadAssetAtPath<GameObject>(copyPath);
                GameObject copyObject = GameObject.Instantiate(sourceObject) as GameObject;
                SpriteRenderer[] targetSprites = ((DuplicateCharacter)target).petTarget.GetComponentsInChildren<SpriteRenderer>(true);
                SpriteRenderer[] copySprites = copyObject.GetComponentsInChildren<SpriteRenderer>(true);

                for (int i = 0; i < targetSprites.Length; i++)
                {
                    for (int j = 0; j < copySprites.Length; j++)
                    {
                        if (targetSprites[i].name == copySprites[j].name && targetSprites[i].sharedMaterial.name != "Sprites-Default")
                        {
                            copySprites[j].sharedMaterial = targetSprites[i].sharedMaterial;
                        }
                    }
                }

                copyObject.name = ((DuplicateCharacter)target).petTarget.name + "_Accessory_" + n.ToString();
                var pastePath = targetPath + "_Accessory_" + n.ToString() + ".prefab";
                Debug.Log(pastePath);
                bool isSuccess = false;
                PrefabUtility.SaveAsPrefabAsset(copyObject, pastePath,out isSuccess);
                if (isSuccess)
                {
                    Debug.Log("Success Copy Accessory " + n.ToString());
                    GameObject.DestroyImmediate(copyObject);
                }
            }

            /*
            var path = AssetDatabase.GetAssetPath(((DuplicateCharacter)target).accessories);
            if (string.IsNullOrEmpty(path))
                return;

            Debug.Log(path);

            string newPath = path.Replace(".", "(Clone).");
            if (!AssetDatabase.CopyAsset(path, newPath))
            {
                Debug.LogError("Couldn't Clone Asset");
                return;
            }*/
        }

        
    }
}
