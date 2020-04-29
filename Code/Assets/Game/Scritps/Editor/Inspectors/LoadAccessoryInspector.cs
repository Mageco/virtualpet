using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoadAccessories))]
public class LoadAccessoryInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Changed");
        if (GUILayout.Button("Add Pet", GUILayout.Width(200)))
        {
            ((LoadAccessories)target).petBasic = ArrayHelper.Add(null, ((LoadAccessories)target).petBasic);
        }

        for (int i = 0; i < ((LoadAccessories)target).petBasic.Length; i++)
        {
            GUILayout.BeginHorizontal();
            ((LoadAccessories)target).petBasic[i] = (GameObject)EditorGUILayout.ObjectField("pet " + (i + 1).ToString(), ((LoadAccessories)target).petBasic[i], typeof(GameObject), false, GUILayout.Width(300));
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                ((LoadAccessories)target).petBasic = ArrayHelper.Remove(i, ((LoadAccessories)target).petBasic);
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Load Accessory Skin", GUILayout.Width(200)))
        {
            for (int i = 0; i < ((LoadAccessories)target).petBasic.Length; i++)
            {
                var sourcePath = AssetDatabase.GetAssetPath(((LoadAccessories)target).petBasic[i]);
                sourcePath = sourcePath.Replace(".prefab", "");

                GameObject go = GameObject.Find(((LoadAccessories)target).petBasic[i].name);
                if (go != null)
                {
                    //var pastePath = PrefabUtility.A.GetAssetPath(go);
                    //Debug.Log(pastePath);
                    go.GetComponent<CharController>().skinPrefabs.Clear();
                    go.GetComponent<CharController>().skinPrefabs.Add(((LoadAccessories)target).petBasic[i]);
                    for (int j = 0; j < 20; j++)
                    {
                        var copyPath = sourcePath + "_Accessory_" + (j + 1).ToString() + ".prefab";
                        GameObject sourceObject = AssetDatabase.LoadAssetAtPath<GameObject>(copyPath);
                        if (sourceObject != null)
                        {
                            go.GetComponent<CharController>().skinPrefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(copyPath));
                        }
                    }
                    PrefabUtility.ApplyPrefabInstance(go, InteractionMode.AutomatedAction);
                    Debug.Log("Save prefab ok " + go.name);
                }
            }
        }

        if (GUILayout.Button("Import Accessory Data", GUILayout.Width(200)))
        {
 
            for (int i = 0; i < ((LoadAccessories)target).petBasic.Length; i++)
            {
               

                GameObject go = GameObject.Find(((LoadAccessories)target).petBasic[i].name);
                if (go != null)
                {
                    for (int j = 0; j < go.GetComponent<CharController>().skinPrefabs.Count; j++)
                    {
                        bool isExist = false;
                        string name = ((LoadAccessories)target).petBasic[i].name + "_Accessory_" + j.ToString();
                        for (int k = 0; k < DataHolder.Accessories().GetDataCount(); k++)
                        {
                            if(DataHolder.Accessory(k).GetName(0) == name)
                            {
                                Debug.Log("Existed " + name);
                                isExist = true;
                                for (int m = 0; m < DataHolder.Pets().GetDataCount(); m++)
                                {
                                    if (DataHolder.Pet(m).prefabName == ((LoadAccessories)target).petBasic[i].name)
                                    {

                                        DataHolder.Accessory(k).petId = DataHolder.Pet(m).iD;
                                        Debug.Log(DataHolder.Pet(m).iD + " : " + DataHolder.Pet(m).prefabName);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        if (!isExist)
                        {
                            Accessory a = new Accessory();
                            a.iD = DataHolder.LastAccessoryID() + 1;
                            a.SetName(0, name);
                            a.accessoryId = j;
                            a.levelRequire = 3*j;
                            
                            Random.InitState(a.iD);

                            for (int m = 0; m < DataHolder.Pets().GetDataCount(); m++)
                            {
                                if (DataHolder.Pet(m).prefabName == ((LoadAccessories)target).petBasic[i].name)
                                {

                                    a.petId = DataHolder.Pet(m).iD;
                                    Debug.Log(a.petId + " : " + DataHolder.Pet(m).prefabName);
                                    break;
                                }
                            }

                            if (j == 0)
                            {
                                string url = DataHolder.GetPet(a.petId).iconUrl;
                                a.iconUrl = url;
                                a.priceType = PriceType.Coin;
                                a.buyPrice = 0;
                            }
                            else
                            {
                                string url = "Assets/Game/Resources/icons/Accessory/" + name + ".png";
                                a.iconUrl = url;
                                int n = Random.Range(0, 100);
                                if (n > 80)
                                {
                                    a.priceType = PriceType.Coin;
                                    a.buyPrice = 999;
                                }
                                else
                                {
                                    a.priceType = PriceType.Diamond;
                                    a.buyPrice = 10;
                                }
                            }
 

                            DataHolder.Accessories().accessories = ArrayHelper.Add(a, DataHolder.Accessories().accessories);
                            Debug.Log("Add " + name);
                        }
                    }
                }

            }

            DataHolder.Accessories().SaveData();
        }
    }
}
