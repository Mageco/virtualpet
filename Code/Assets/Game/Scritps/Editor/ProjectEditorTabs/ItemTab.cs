
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

            for (int i = 0; i < DataHolder.Languages().GetDataCount(); i++)
            {
                EditorGUILayout.LabelField(DataHolder.Language(i));
                DataHolder.Item(selection).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.Item(selection).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
                DataHolder.Item(selection).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.Item(selection).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                EditorGUILayout.Separator();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
            if(DataHolder.Item(selection).iconUrl != null){
                this.tmpSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Item(selection).iconUrl);
            }
            this.tmpSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
            if (this.tmpSprites != null)
            {
                DataHolder.Item(selection).iconUrl = AssetDatabase.GetAssetPath(this.tmpSprites);

            }
            EditorGUILayout.LabelField(DataHolder.Item(selection).iconUrl);
            EditorGUILayout.EndHorizontal();
            
			if (this.tmpSprites != null)
            {
                if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
                {
                    DataHolder.Item(selection).iconUrl = "";
                    tmpSprites = null;
                }
            }


			EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Item Settings");
			if(fold2)
			{
				if(selection != tmpSelection) this.tmpPrefab = null;
				if(this.tmpPrefab == null && "" != DataHolder.Item(selection).prefabName)
				{
					this.tmpPrefab = (GameObject)Resources.Load(DataHolder.Items().GetPrefabPath()+DataHolder.Item(selection).prefabName, typeof(GameObject));
				}
				this.tmpPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", this.tmpPrefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth*2));
				if(this.tmpPrefab) DataHolder.Item(selection).prefabName = this.tmpPrefab.name;
				else DataHolder.Item(selection).prefabName = "";

				EditorGUILayout.Separator();
				DataHolder.Item(selection).itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", DataHolder.Item(selection).itemType, GUILayout.Width (pw.mWidth * 2));
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
				DataHolder.Item(selection).buyPrice = EditorGUILayout.IntField("Buy price", DataHolder.Item(selection).buyPrice, GUILayout.Width(pw.mWidth));
                DataHolder.Item(selection).priceType = (PriceType)EditorTab.EnumToolbar("Price Type", (int)DataHolder.Item(selection).priceType, typeof(PriceType));    


				//EditorGUILayout.EndToggleGroup();
				//this.Separate();



			}
			EditorGUILayout.EndVertical();

 			EditorGUILayout.BeginVertical("box");
			fold3 = EditorGUILayout.Foldout(fold3, "Usage Settings");
			if(fold3)
			{
				DataHolder.Item(selection).consume = EditorGUILayout.Toggle("Consume", DataHolder.Item(selection).consume, GUILayout.Width(pw.mWidth));
				EditorGUILayout.Separator();
				DataHolder.Item(selection).itemSkill = (ItemSkillType)EditorTab.EnumToolbar("Item skill", (int)DataHolder.Item(selection).itemSkill, typeof(ItemSkillType));
				if(!ItemSkillType.NONE.Equals(DataHolder.Item(selection).itemSkill))
				{
					DataHolder.Item(selection).skillID = EditorGUILayout.Popup("Skill", 
						DataHolder.Item(selection).skillID, pw.GetSkills(), GUILayout.Width(pw.mWidth));
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