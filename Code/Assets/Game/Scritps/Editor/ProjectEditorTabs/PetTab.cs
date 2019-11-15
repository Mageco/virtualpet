
using UnityEditor;
using UnityEngine;



public class PetTab : BaseTab
{
	private GameObject tmp1Prefab;
    private GameObject tmp2Prefab;
    private GameObject tmp3Prefab;
    private GameObject tmp4Prefab;

    public PetTab(ProjectWindow pw) : base(pw)
    {
        this.Reload();
    }

    public void ShowTab()
    {
		int tmpSelection = selection;
        EditorGUILayout.BeginVertical();

        // buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Pet", GUILayout.Width(pw.mWidth)))
        {
            DataHolder.Pets().AddPet();
            selection = DataHolder.Pets().GetDataCount() - 1;
            GUI.FocusControl("ID");
        }
        this.ShowCopyButton(DataHolder.Pets());
        if (selection >= 0)
        {
            this.ShowRemButton("Remove Pet", DataHolder.Pets());
        }
        this.CheckSelection(DataHolder.Pets());
        EditorGUILayout.EndHorizontal();

        // color list
        this.AddItemList(DataHolder.Pets());

        // color settings

        EditorGUILayout.BeginVertical();
        SP2 = EditorGUILayout.BeginScrollView(SP2);
        if (DataHolder.Pets().GetDataCount() > 0)
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
                DataHolder.Pet(selection).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.Pet(selection).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
                DataHolder.Pet(selection).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.Pet(selection).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                EditorGUILayout.Separator();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
            if(DataHolder.Pet(selection).iconUrl != null){
                this.tmpSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Pet(selection).iconUrl);
            }
            this.tmpSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
            if (this.tmpSprites != null)
            {
                DataHolder.Pet(selection).iconUrl = AssetDatabase.GetAssetPath(this.tmpSprites);

            }
            EditorGUILayout.LabelField(DataHolder.Pet(selection).iconUrl);
            EditorGUILayout.EndHorizontal();
            
			if (this.tmpSprites != null)
            {
                if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
                {
                    DataHolder.Pet(selection).iconUrl = "";
                    tmpSprites = null;
                }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Icon Lock", GUILayout.MaxWidth(110));
            if(DataHolder.Pet(selection).iconLockUrl != null){
                this.tmpLockSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Pet(selection).iconLockUrl);
            }
            this.tmpLockSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpLockSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
            if (this.tmpLockSprites != null)
            {
                DataHolder.Pet(selection).iconLockUrl = AssetDatabase.GetAssetPath(this.tmpLockSprites);

            }
            EditorGUILayout.LabelField(DataHolder.Pet(selection).iconLockUrl);
            EditorGUILayout.EndHorizontal();
            
			if (this.tmpLockSprites != null)
            {
                if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
                {
                    DataHolder.Pet(selection).iconLockUrl = "";
                    tmpLockSprites = null;
                }
            }

			EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Pet Settings");
			if(fold2)
			{
				if(selection != tmpSelection) this.tmp1Prefab = null;
				if(this.tmp1Prefab == null && "" != DataHolder.Pet(selection).petSmall)
				{
					this.tmp1Prefab = (GameObject)Resources.Load(DataHolder.Pets().GetPrefabPath()+DataHolder.Pet(selection).petSmall, typeof(GameObject));
				}
				this.tmp1Prefab = (GameObject)EditorGUILayout.ObjectField("Pet Small Prefab", this.tmp1Prefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth*2));
				if(this.tmp1Prefab) DataHolder.Pet(selection).petSmall = this.tmp1Prefab.name;
				else DataHolder.Pet(selection).petSmall = "";

                if(selection != tmpSelection) this.tmp2Prefab = null;
				if(this.tmp2Prefab == null && "" != DataHolder.Pet(selection).petMiddle)
				{
					this.tmp2Prefab = (GameObject)Resources.Load(DataHolder.Pets().GetPrefabPath()+DataHolder.Pet(selection).petMiddle, typeof(GameObject));
				}
				this.tmp2Prefab = (GameObject)EditorGUILayout.ObjectField("Pet Middle Prefab", this.tmp2Prefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth*2));
				if(this.tmp2Prefab) DataHolder.Pet(selection).petMiddle = this.tmp2Prefab.name;
				else DataHolder.Pet(selection).petMiddle = "";

                if(selection != tmpSelection) this.tmp3Prefab = null;
				if(this.tmp3Prefab == null && "" != DataHolder.Pet(selection).petBig)
				{
					this.tmp3Prefab = (GameObject)Resources.Load(DataHolder.Pets().GetPrefabPath()+DataHolder.Pet(selection).petBig, typeof(GameObject));
				}
				this.tmp3Prefab = (GameObject)EditorGUILayout.ObjectField("Pet Big Prefab", this.tmp3Prefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth*2));
				if(this.tmp3Prefab) DataHolder.Pet(selection).petBig = this.tmp3Prefab.name;
				else DataHolder.Pet(selection).petBig = "";

                if(selection != tmpSelection) this.tmp4Prefab = null;
				if(this.tmp4Prefab == null && "" != DataHolder.Pet(selection).petMiniGame1)
				{
					this.tmp4Prefab = (GameObject)Resources.Load(DataHolder.Pets().GetPrefabPath()+DataHolder.Pet(selection).petMiniGame1, typeof(GameObject));
				}
				this.tmp4Prefab = (GameObject)EditorGUILayout.ObjectField("Pet Big Prefab", this.tmp4Prefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth*2));
				if(this.tmp4Prefab) DataHolder.Pet(selection).petMiniGame1 = this.tmp4Prefab.name;
				else DataHolder.Pet(selection).petBig = "";

				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
                DataHolder.Pet(selection).priceType = (PriceType)EditorTab.EnumToolbar("Price Type", (int)DataHolder.Pet(selection).priceType, typeof(PriceType));    
				DataHolder.Pet(selection).buyPrice = EditorGUILayout.IntField("Buy price", DataHolder.Pet(selection).buyPrice, GUILayout.Width(pw.mWidth));
			}
			EditorGUILayout.EndVertical();

 			EditorGUILayout.BeginVertical("box");
			fold3 = EditorGUILayout.Foldout(fold3, "Attribute Settings");
			if(fold3)
			{
                DataHolder.Pet(selection).speed = EditorGUILayout.FloatField("Speed", DataHolder.Pet(selection).speed, GUILayout.Width(pw.mWidth));
			}
			EditorGUILayout.EndVertical(); 

 

            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
 
        }
        this.EndTab();
    }
}