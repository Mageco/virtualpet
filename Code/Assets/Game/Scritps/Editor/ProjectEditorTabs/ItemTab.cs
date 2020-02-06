
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public class ItemTab : BaseTab
{
	private GameObject tmpPrefab;
    List<Texture2D> tempSprites = new List<Texture2D>();
    List<GameObject> tempPrefabs = new List<GameObject>();

    public ItemTab(ProjectWindow pw) : base(pw)
    {
        this.Reload();
    }

    public void ShowTab()
    {
		int tmpSelection = selection;
        int lastCategory = temcategory;
        EditorGUILayout.BeginVertical();

        // buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Item", GUILayout.Width(pw.mWidth)))
        {
            DataHolder.Items().AddItem();
            selection = DataHolder.Items().GetDataCount() - 1;
            GUI.FocusControl("ID");
        }
        this.ShowCopyButton(DataHolder.Items());
        if (selection >= 0)
        {
            this.ShowRemButton("Remove Item", DataHolder.Items());
        }
        this.CheckSelection(DataHolder.Items());
        EditorGUILayout.EndHorizontal();

        // color list
        this.AddItemFilterList(DataHolder.Items());

        // color settings

        EditorGUILayout.BeginVertical();
        SP2 = EditorGUILayout.BeginScrollView(SP2);
        if (DataHolder.Items().GetDataCount() > 0)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Item ID: " + DataHolder.Items().GetItem(selection,temcategory).iD.ToString(), GUILayout.Width(pw.mWidth * 2));
            DataHolder.Items().GetItem(selection,temcategory).shopOrder = EditorGUILayout.IntField("Shop Order", DataHolder.Items().GetItem(selection,temcategory).shopOrder, GUILayout.Width(pw.mWidth));


            EditorGUILayout.BeginVertical("box");
            fold4 = EditorGUILayout.Foldout(fold4, "Item Name");
            if (fold4)
            {
                for (int i = 0; i < DataHolder.Languages().GetDataCount(); i++)
                {
                    EditorGUILayout.LabelField(DataHolder.Language(i));
                    DataHolder.Items().GetItem(selection, temcategory).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.Items().GetItem(selection, temcategory).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
                    DataHolder.Items().GetItem(selection, temcategory).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.Items().GetItem(selection, temcategory).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                    EditorGUILayout.Separator();
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
            if(DataHolder.Items().GetItem(selection,temcategory).iconUrl != null){
                this.tmpSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Items().GetItem(selection,temcategory).iconUrl);
            }
            this.tmpSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
            if (this.tmpSprites != null)
            {
                DataHolder.Items().GetItem(selection,temcategory).iconUrl = AssetDatabase.GetAssetPath(this.tmpSprites);

            }
            EditorGUILayout.LabelField(DataHolder.Items().GetItem(selection,temcategory).iconUrl);
            EditorGUILayout.EndHorizontal();

            if (this.tmpSprites != null)
            {
                if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
                {
                    DataHolder.Items().GetItem(selection,temcategory).iconUrl = "";
                    tmpSprites = null;
                }
            }

            

			EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Item Settings");
			if(fold2)
			{
				EditorGUILayout.Separator();
				DataHolder.Items().GetItem(selection,temcategory).itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", DataHolder.Items().GetItem(selection,temcategory).itemType, GUILayout.Width (pw.mWidth * 2));
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
				DataHolder.Items().GetItem(selection,temcategory).buyPrice = EditorGUILayout.IntField("Buy price", DataHolder.Items().GetItem(selection,temcategory).buyPrice, GUILayout.Width(pw.mWidth));
                DataHolder.Items().GetItem(selection,temcategory).priceType = (PriceType)EditorTab.EnumToolbar("Price Type", (int)DataHolder.Items().GetItem(selection,temcategory).priceType, typeof(PriceType));    
                if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Diamond){
                    DataHolder.Items().GetItem(selection,temcategory).sellPrice = EditorGUILayout.IntField("Diamond Amount", DataHolder.Items().GetItem(selection,temcategory).sellPrice, GUILayout.Width(pw.mWidth));
                }else if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Coin){
                    DataHolder.Items().GetItem(selection,temcategory).sellPrice = EditorGUILayout.IntField("Coin Amount", DataHolder.Items().GetItem(selection,temcategory).sellPrice, GUILayout.Width(pw.mWidth));
                }
                DataHolder.Items().GetItem(selection,temcategory).isAvailable = EditorGUILayout.Toggle("Available", DataHolder.Items().GetItem(selection,temcategory).isAvailable, GUILayout.Width(pw.mWidth));
                //DataHolder.Items().GetItem(selection,temcategory).consume = EditorGUILayout.Toggle("Consume", DataHolder.Items().GetItem(selection,temcategory).consume, GUILayout.Width(pw.mWidth));

			}
			EditorGUILayout.EndVertical();


				
			EditorGUILayout.Separator();
            if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Food){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Food Amount", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
            }else if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Drink){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Water Amount", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
            }else if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Toilet){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Clean per 1 second", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
                DataHolder.Items().GetItem(selection, temcategory).happy = EditorGUILayout.FloatField("+Happy", DataHolder.Items().GetItem(selection, temcategory).happy, GUILayout.Width(pw.mWidth));
            }
            else if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Bath){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Clean after take bath", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
                DataHolder.Items().GetItem(selection, temcategory).injured = EditorGUILayout.FloatField("+Injured", DataHolder.Items().GetItem(selection, temcategory).injured, GUILayout.Width(pw.mWidth));
            }
            else if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Clean){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Clean per second", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
            }
            else if (DataHolder.Items().GetItem(selection, temcategory).itemType == ItemType.Bed)
            {
                DataHolder.Items().GetItem(selection, temcategory).value = EditorGUILayout.FloatField("Clean per second", DataHolder.Items().GetItem(selection, temcategory).value, GUILayout.Width(pw.mWidth));
                DataHolder.Items().GetItem(selection, temcategory).health = EditorGUILayout.FloatField("+Health", DataHolder.Items().GetItem(selection, temcategory).health, GUILayout.Width(pw.mWidth));
            }
			



            if (GUILayout.Button("Add Skin", GUILayout.Width(pw.mWidth * 0.7f)))
            {
                DataHolder.Items().GetItem(selection, temcategory).skins.Add(new Skin());
                tempSprites.Add(new Texture2D(256, 256));
                tempPrefabs.Add(null);
            }

            if(DataHolder.Items().GetItem(selection, temcategory).skins.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                fold2 = EditorGUILayout.Foldout(fold2, "Skin");
                if (fold2)
                {


                    for (int i = 0; i < DataHolder.Items().GetItem(selection, temcategory).skins.Count; i++)
                    {
                        if (tempPrefabs.Count == i)
                        {
                            tempSprites.Add(new Texture2D(256, 256));
                            tempPrefabs.Add(null);
                        }
                    }


                    for (int i = 0; i < DataHolder.Items().GetItem(selection, temcategory).skins.Count; i++)
                    {

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
                        if (DataHolder.Items().GetItem(selection, temcategory).skins[i].iconUrl != null)
                        {

                            this.tempSprites[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Items().GetItem(selection, temcategory).skins[i].iconUrl);
                        }
                        this.tempSprites[i] = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tempSprites[i], typeof(Texture2D), false, GUILayout.MaxWidth(100));
                        if (this.tempSprites[i] != null)
                        {
                            DataHolder.Items().GetItem(selection, temcategory).skins[i].iconUrl = AssetDatabase.GetAssetPath(this.tempSprites[i]);
                        }
                        EditorGUILayout.LabelField(DataHolder.Items().GetItem(selection, temcategory).iconUrl);
                        EditorGUILayout.EndHorizontal();

                        if (this.tempSprites[i] != null)
                        {
                            if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
                            {
                                DataHolder.Items().GetItem(selection, temcategory).skins[i].iconUrl = "";
                                this.tempSprites[i] = null;
                            }
                        }

                        if (selection != tmpSelection)
                        {
                            this.tempPrefabs[i] = null;
                        }
                        if (this.tempPrefabs[i] == null && "" != DataHolder.Items().GetItem(selection, temcategory).skins[i].prefabName)
                        {
                            this.tempPrefabs[i] = (GameObject)Resources.Load(DataHolder.Pets().GetPrefabPath() + DataHolder.Items().GetItem(selection, temcategory).skins[i].prefabName, typeof(GameObject));
                        }
                        this.tempPrefabs[i] = (GameObject)EditorGUILayout.ObjectField("Prefab", this.tempPrefabs[i], typeof(GameObject), false, GUILayout.Width(pw.mWidth * 2));
                        if (this.tempPrefabs[i]) DataHolder.Items().GetItem(selection, temcategory).skins[i].prefabName = this.tempPrefabs[i].name;
                        else DataHolder.Items().GetItem(selection, temcategory).skins[i].prefabName = "";


                        DataHolder.Items().GetItem(selection, temcategory).skins[i].levelRequire = EditorGUILayout.IntField("Level Require", DataHolder.Items().GetItem(selection, temcategory).skins[i].levelRequire, GUILayout.Width(pw.mWidth));
                        DataHolder.Items().GetItem(selection, temcategory).skins[i].priceType = (PriceType)EditorTab.EnumToolbar("Price Type", (int)DataHolder.Items().GetItem(selection, temcategory).skins[i].priceType, typeof(PriceType));
                        DataHolder.Items().GetItem(selection, temcategory).skins[i].buyPrice = EditorGUILayout.IntField("Buy price", DataHolder.Items().GetItem(selection, temcategory).skins[i].buyPrice, GUILayout.Width(pw.mWidth));

                        if (GUILayout.Button("Remove Skin", GUILayout.Width(pw.mWidth * 0.7f)))
                        {
                            DataHolder.Items().GetItem(selection, temcategory).skins.RemoveAt(i);
                            GameObject.DestroyImmediate(tempSprites[i]);
                            this.tempSprites.RemoveAt(i);
                        }
                        EditorGUILayout.Separator();
                        EditorGUILayout.Separator();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            //EditorGUILayout.EndHorizontal();


        }
        this.EndTab();
    }
}