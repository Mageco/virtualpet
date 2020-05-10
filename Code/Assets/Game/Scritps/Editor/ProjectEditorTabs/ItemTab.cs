
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public class ItemTab : BaseTab
{
	private GameObject tmpPrefab;

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


            //EditorGUILayout.BeginVertical("box");
            //fold4 = EditorGUILayout.Foldout(fold4, "Item Name");
            //if (fold4)
            //{
            //    for (int i = 0; i < DataHolder.Languages().GetDataCount(); i++)
            //    {
                    EditorGUILayout.LabelField(DataHolder.Language(0));
                    DataHolder.Items().GetItem(selection, temcategory).languageItem[0].Name = EditorGUILayout.TextField("Name", DataHolder.Items().GetItem(selection, temcategory).languageItem[0].Name, GUILayout.Width(pw.mWidth * 2));
                    //DataHolder.Items().GetItem(selection, temcategory).languageItem[0].Description = EditorGUILayout.TextField("Description", DataHolder.Items().GetItem(selection, temcategory).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                    EditorGUILayout.Separator();
            //    }
            //}
            //EditorGUILayout.EndVertical();

            


            EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Item Settings");
			if(fold2)
			{
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
                if (DataHolder.Items().GetItem(selection, temcategory).iconUrl != null)
                {
                    this.tmpSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Items().GetItem(selection, temcategory).iconUrl);
                }
                this.tmpSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
                if (this.tmpSprites != null)
                {
                    DataHolder.Items().GetItem(selection, temcategory).iconUrl = AssetDatabase.GetAssetPath(this.tmpSprites);

                }
                EditorGUILayout.LabelField(DataHolder.Items().GetItem(selection, temcategory).iconUrl);
                EditorGUILayout.EndHorizontal();

                if (this.tmpSprites != null)
                {
                    if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
                    {
                        DataHolder.Items().GetItem(selection, temcategory).iconUrl = "";
                        tmpSprites = null;
                    }
                }


                if (selection != tmpSelection || lastCategory != temcategory)
                {
                    this.tmpPrefab = null;
                }
                if (this.tmpPrefab == null && "" != DataHolder.Items().GetItem(selection, temcategory).prefabName)
                {
                    this.tmpPrefab = (GameObject)Resources.Load(DataHolder.Items().GetPrefabPath() + DataHolder.Items().GetItem(selection, temcategory).prefabName, typeof(GameObject));
                }
                this.tmpPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", this.tmpPrefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth * 2));
                if (this.tmpPrefab) DataHolder.Items().GetItem(selection, temcategory).prefabName = this.tmpPrefab.name;
                else DataHolder.Items().GetItem(selection, temcategory).prefabName = "";


                EditorGUILayout.Separator();
                DataHolder.Items().GetItem(selection, temcategory).levelRequire = EditorGUILayout.IntField("Level Require", DataHolder.Items().GetItem(selection, temcategory).levelRequire, GUILayout.Width(pw.mWidth));
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
                DataHolder.Items().GetItem(selection, temcategory).itemTag = (ItemTag)EditorTab.EnumToolbar("Tag", (int)DataHolder.Items().GetItem(selection, temcategory).itemTag, typeof(ItemTag));
                DataHolder.Items().GetItem(selection,temcategory).consume = EditorGUILayout.Toggle("Consume", DataHolder.Items().GetItem(selection,temcategory).consume, GUILayout.Width(pw.mWidth));

			}
			EditorGUILayout.EndVertical();



            EditorGUILayout.Separator();
            if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Food){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Food Amount", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
            }else if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Drink){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Water Amount", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
            }else if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Toilet){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Clean per 1 second", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
            }
            else if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Bath){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Clean after take bath", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
            }
            else if(DataHolder.Items().GetItem(selection,temcategory).itemType == ItemType.Clean){
                DataHolder.Items().GetItem(selection,temcategory).value = EditorGUILayout.FloatField("Clean per second", DataHolder.Items().GetItem(selection,temcategory).value, GUILayout.Width(pw.mWidth));
            }
            else if (DataHolder.Items().GetItem(selection, temcategory).itemType == ItemType.Bed)
            {
                DataHolder.Items().GetItem(selection, temcategory).value = EditorGUILayout.FloatField("Sleep rate", DataHolder.Items().GetItem(selection, temcategory).value, GUILayout.Width(pw.mWidth));
            }
            else if (DataHolder.Items().GetItem(selection, temcategory).itemType == ItemType.MedicineBox)
            {
                DataHolder.Items().GetItem(selection, temcategory).value = EditorGUILayout.FloatField("+Health", DataHolder.Items().GetItem(selection, temcategory).value, GUILayout.Width(pw.mWidth));
            }
            else if (DataHolder.Items().GetItem(selection, temcategory).itemType == ItemType.Toy)
            {
                DataHolder.Items().GetItem(selection, temcategory).value = EditorGUILayout.FloatField("+Happy", DataHolder.Items().GetItem(selection, temcategory).value, GUILayout.Width(pw.mWidth));
            }
            else if (DataHolder.Items().GetItem(selection, temcategory).itemType == ItemType.Fruit)
            {
                DataHolder.Items().GetItem(selection, temcategory).value = EditorGUILayout.FloatField("Time grow", DataHolder.Items().GetItem(selection, temcategory).value, GUILayout.Width(pw.mWidth));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();



        }
        this.EndTab();
    }
}