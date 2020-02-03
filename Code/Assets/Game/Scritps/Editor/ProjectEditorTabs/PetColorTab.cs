
using UnityEditor;
using UnityEngine;



public class PetColorTab : BaseTab
{
    private GameObject tmp1Prefab;
    private GameObject tmp2Prefab;
    private GameObject tmp3Prefab;
    private GameObject tmp4Prefab;
    private int tempId = 0;
    private int tempPetColorId = 0;
    int lastSellection = -1;

    public PetColorTab(ProjectWindow pw) : base(pw)
    {
        this.Reload();
    }

    public void ShowTab()
    {
        int tmpSelection = selection;
        EditorGUILayout.BeginVertical();

        // buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add PetColor", GUILayout.Width(pw.mWidth)))
        {
            DataHolder.PetColors().AddPetColor();
            selection = DataHolder.PetColors().GetDataCount() - 1;
            GUI.FocusControl("ID");
        }
        this.ShowCopyButton(DataHolder.PetColors());
        if (selection >= 0)
        {
            this.ShowRemButton("Remove PetColor", DataHolder.PetColors());
        }
        this.CheckSelection(DataHolder.PetColors());
        EditorGUILayout.EndHorizontal();

        // color list
        this.AddItemList(DataHolder.PetColors());

        // color settings

        EditorGUILayout.BeginVertical();
        SP2 = EditorGUILayout.BeginScrollView(SP2);
        if (DataHolder.PetColors().GetDataCount() > 0)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            DataHolder.PetColor(selection).shopOrder = EditorGUILayout.IntField("Shop Order", DataHolder.PetColor(selection).shopOrder, GUILayout.Width(pw.mWidth));

            for (int i = 0; i < DataHolder.Languages().GetDataCount(); i++)
            {
                EditorGUILayout.LabelField(DataHolder.Language(i));
                DataHolder.PetColor(selection).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.PetColor(selection).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
                DataHolder.PetColor(selection).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.PetColor(selection).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                EditorGUILayout.Separator();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
            if (DataHolder.PetColor(selection).iconUrl != null)
            {
                this.tmpSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.PetColor(selection).iconUrl);
            }
            this.tmpSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
            if (this.tmpSprites != null)
            {
                DataHolder.PetColor(selection).iconUrl = AssetDatabase.GetAssetPath(this.tmpSprites);

            }
            EditorGUILayout.LabelField(DataHolder.PetColor(selection).iconUrl);
            EditorGUILayout.EndHorizontal();

            if (this.tmpSprites != null)
            {
                if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
                {
                    DataHolder.PetColor(selection).iconUrl = "";
                    tmpSprites = null;
                }
            }



            

            EditorGUILayout.BeginVertical("box");
            fold2 = EditorGUILayout.Foldout(fold2, "PetColor Settings");
            if (fold2)
            {
                if (selection != tmpSelection) this.tmp3Prefab = null;
                if (this.tmp3Prefab == null && "" != DataHolder.PetColor(selection).prefabName)
                {
                    this.tmp3Prefab = (GameObject)Resources.Load(DataHolder.PetColors().GetPrefabPath() + DataHolder.PetColor(selection).prefabName, typeof(GameObject));
                }
                this.tmp3Prefab = (GameObject)EditorGUILayout.ObjectField("PetColor Prefab", this.tmp3Prefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth * 2));
                if (this.tmp3Prefab) DataHolder.PetColor(selection).prefabName = this.tmp3Prefab.name;
                else DataHolder.PetColor(selection).prefabName = "";


                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                DataHolder.PetColor(selection).priceType = (PriceType)EditorTab.EnumToolbar("Price Type", (int)DataHolder.PetColor(selection).priceType, typeof(PriceType));
                DataHolder.PetColor(selection).buyPrice = EditorGUILayout.IntField("Buy price", DataHolder.PetColor(selection).buyPrice, GUILayout.Width(pw.mWidth));
                DataHolder.PetColor(selection).levelRequire = EditorGUILayout.IntField("Level Require", DataHolder.PetColor(selection).levelRequire, GUILayout.Width(pw.mWidth));
                DataHolder.PetColor(selection).isAvailable = EditorGUILayout.Toggle("Available", DataHolder.PetColor(selection).isAvailable, GUILayout.Width(pw.mWidth));

            }
            EditorGUILayout.EndVertical();

            




            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

        }
        this.EndTab();
    }
}