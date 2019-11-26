
using UnityEditor;
using UnityEngine;



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
            Debug.Log(temcategory + "  " + selection);
            DataHolder.Items().GetItem(selection,temcategory).shopOrder = EditorGUILayout.IntField("Shop Order", DataHolder.Items().GetItem(selection,temcategory).shopOrder, GUILayout.Width(pw.mWidth));
            
            for (int i = 0; i < DataHolder.Languages().GetDataCount(); i++)
            {
                EditorGUILayout.LabelField(DataHolder.Language(i));
                DataHolder.Items().GetItem(selection,temcategory).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.Items().GetItem(selection,temcategory).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
                DataHolder.Items().GetItem(selection,temcategory).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.Items().GetItem(selection,temcategory).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                EditorGUILayout.Separator();
            }

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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Icon Lock", GUILayout.MaxWidth(110));
            if(DataHolder.Items().GetItem(selection,temcategory).iconLockUrl != null){
                this.tmpLockSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Items().GetItem(selection,temcategory).iconLockUrl);
            }
            this.tmpLockSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpLockSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
            if (this.tmpLockSprites != null)
            {
                DataHolder.Items().GetItem(selection,temcategory).iconLockUrl = AssetDatabase.GetAssetPath(this.tmpLockSprites);

            }
            EditorGUILayout.LabelField(DataHolder.Items().GetItem(selection,temcategory).iconLockUrl);
            EditorGUILayout.EndHorizontal();
            
			if (this.tmpLockSprites != null)
            {
                if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
                {
                    DataHolder.Items().GetItem(selection,temcategory).iconLockUrl = "";
                    tmpLockSprites = null;
                }
            }


			EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Item Settings");
			if(fold2)
			{
				if(selection != tmpSelection) this.tmpPrefab = null;
				if(this.tmpPrefab == null && "" != DataHolder.Items().GetItem(selection,temcategory).prefabName)
				{
					this.tmpPrefab = (GameObject)Resources.Load(DataHolder.Items().GetPrefabPath()+DataHolder.Items().GetItem(selection,temcategory).prefabName, typeof(GameObject));
				}
				this.tmpPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", this.tmpPrefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth*2));
				if(this.tmpPrefab) DataHolder.Items().GetItem(selection,temcategory).prefabName = this.tmpPrefab.name;
				else DataHolder.Items().GetItem(selection,temcategory).prefabName = "";

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
                
                DataHolder.Items().GetItem(selection,temcategory).levelRequire = EditorGUILayout.IntField("RequireLevel", DataHolder.Items().GetItem(selection,temcategory).levelRequire, GUILayout.Width(pw.mWidth));
                DataHolder.Items().GetItem(selection,temcategory).isAvailable = EditorGUILayout.Toggle("Available", DataHolder.Items().GetItem(selection,temcategory).isAvailable, GUILayout.Width(pw.mWidth));


				//EditorGUILayout.EndToggleGroup();
				//this.Separate();



			}
			EditorGUILayout.EndVertical();

 			EditorGUILayout.BeginVertical("box");
			fold3 = EditorGUILayout.Foldout(fold3, "Usage Settings");
			if(fold3)
			{
				DataHolder.Items().GetItem(selection,temcategory).consume = EditorGUILayout.Toggle("Consume", DataHolder.Items().GetItem(selection,temcategory).consume, GUILayout.Width(pw.mWidth));
				EditorGUILayout.Separator();
				DataHolder.Items().GetItem(selection,temcategory).itemSkill = (ItemSkillType)EditorTab.EnumToolbar("Item skill", (int)DataHolder.Items().GetItem(selection,temcategory).itemSkill, typeof(ItemSkillType));
				if(!ItemSkillType.NONE.Equals(DataHolder.Items().GetItem(selection,temcategory).itemSkill))
				{
					DataHolder.Items().GetItem(selection,temcategory).skillID = EditorGUILayout.Popup("Skill", 
						DataHolder.Items().GetItem(selection,temcategory).skillID, pw.GetSkills(), GUILayout.Width(pw.mWidth));
				}

			}
			EditorGUILayout.EndVertical(); 

            

            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            //EditorGUILayout.EndHorizontal();


        }
        this.EndTab();
    }
}