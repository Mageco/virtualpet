
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


			EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Pet Settings");
			if(fold2)
			{
                if(selection != tmpSelection) this.tmp3Prefab = null;
				if(this.tmp3Prefab == null && "" != DataHolder.Pet(selection).petBig)
				{
					this.tmp3Prefab = (GameObject)Resources.Load(DataHolder.Pets().GetPrefabPath()+DataHolder.Pet(selection).petBig, typeof(GameObject));
				}
				this.tmp3Prefab = (GameObject)EditorGUILayout.ObjectField("Pet Prefab", this.tmp3Prefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth*2));
				if(this.tmp3Prefab) DataHolder.Pet(selection).petBig = this.tmp3Prefab.name;
				else DataHolder.Pet(selection).petBig = "";


				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
                DataHolder.Pet(selection).priceType = (PriceType)EditorTab.EnumToolbar("Price Type", (int)DataHolder.Pet(selection).priceType, typeof(PriceType));    
				DataHolder.Pet(selection).buyPrice = EditorGUILayout.IntField("Buy price", DataHolder.Pet(selection).buyPrice, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).isAvailable = EditorGUILayout.Toggle("Available", DataHolder.Pet(selection).isAvailable, GUILayout.Width(pw.mWidth));

			}
			EditorGUILayout.EndVertical();

 			EditorGUILayout.BeginVertical("box");
			fold3 = EditorGUILayout.Foldout(fold3, "Attribute Settings");
			if(fold3)
			{
                DataHolder.Pet(selection).speed = EditorGUILayout.FloatField("Speed", DataHolder.Pet(selection).speed, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).maxFood = EditorGUILayout.FloatField("MaxFood", DataHolder.Pet(selection).maxFood, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).rateFood = EditorGUILayout.FloatField("RateFood", DataHolder.Pet(selection).rateFood, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).maxWater = EditorGUILayout.FloatField("MaxWater", DataHolder.Pet(selection).maxWater, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).rateWater = EditorGUILayout.FloatField("RateWater", DataHolder.Pet(selection).rateWater, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).maxSleep = EditorGUILayout.FloatField("MaxSleep", DataHolder.Pet(selection).maxSleep, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).rateSleep = EditorGUILayout.FloatField("RateSleep", DataHolder.Pet(selection).rateSleep, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).recoverSleep = EditorGUILayout.FloatField("RecSleep", DataHolder.Pet(selection).recoverSleep, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).maxPee = EditorGUILayout.FloatField("MaxPee", DataHolder.Pet(selection).maxPee, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).ratePee = EditorGUILayout.FloatField("RatePee", DataHolder.Pet(selection).ratePee, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).maxShit = EditorGUILayout.FloatField("MaxShit", DataHolder.Pet(selection).maxShit, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).rateShit = EditorGUILayout.FloatField("RateShit", DataHolder.Pet(selection).rateShit, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).maxDirty = EditorGUILayout.FloatField("MaxDirty", DataHolder.Pet(selection).maxDirty, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).recoverDirty = EditorGUILayout.FloatField("RecDirty", DataHolder.Pet(selection).recoverDirty, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).maxHealth = EditorGUILayout.FloatField("MaxHealth", DataHolder.Pet(selection).maxHealth, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).recoverHealth = EditorGUILayout.FloatField("RecHealth", DataHolder.Pet(selection).recoverHealth, GUILayout.Width(pw.mWidth));
                DataHolder.Pet(selection).recoverEnergy = EditorGUILayout.FloatField("RecEnergy", DataHolder.Pet(selection).recoverEnergy, GUILayout.Width(pw.mWidth));
			}
			EditorGUILayout.EndVertical(); 

 

            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
 
        }
        this.EndTab();
    }
}