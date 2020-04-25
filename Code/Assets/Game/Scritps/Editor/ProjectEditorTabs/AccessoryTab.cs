
using UnityEditor;
using UnityEngine;

public class AccessoryTab : BaseTab
{

	private GameObject tmpPrefab;
	private string[] sections = new string[] {"1"};
	
	public AccessoryTab(ProjectWindow pw) : base(pw)
	{
		this.Reload();
	}
	
	public void ShowTab()
	{
		int tmpSelection = selection;
		EditorGUILayout.BeginVertical();

		// buttons
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Accessory", GUILayout.Width(pw.mWidth)))
		{
			DataHolder.Accessories().AddAccessory();
			selection = DataHolder.Accessories().GetDataCount() - 1;
			GUI.FocusControl("ID");
		}
		//this.ShowCopyButton(DataHolder.Accessories());
		if (selection >= 0)
		{
			this.ShowRemButton("Remove Accessory", DataHolder.Accessories());
		}
		this.CheckSelection(DataHolder.Accessories());
		EditorGUILayout.EndHorizontal();

		// color list
		this.AddItemList(DataHolder.Accessories());

		// color settings

		EditorGUILayout.BeginVertical();
		SP2 = EditorGUILayout.BeginScrollView(SP2);
		if (DataHolder.Accessories().GetDataCount() > 0)
		{
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();


			EditorGUILayout.LabelField(DataHolder.Language(0));
			DataHolder.Accessory(selection).languageItem[0].Name = EditorGUILayout.TextField("Name", DataHolder.Accessory(selection).languageItem[0].Name, GUILayout.Width(pw.mWidth * 2));
			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
			if(DataHolder.Accessory(selection).iconUrl != null){
                this.tmpSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Accessory(selection).iconUrl);
            }
			this.tmpSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
			if (this.tmpSprites != null)
			{
				DataHolder.Accessory(selection).iconUrl = AssetDatabase.GetAssetPath(this.tmpSprites);

			}
			EditorGUILayout.LabelField(DataHolder.Accessory(selection).iconUrl);
			EditorGUILayout.EndHorizontal();

			if (this.tmpSprites != null)
			{
				if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
				{
					DataHolder.Accessory(selection).iconUrl = "";
					tmpSprites = null;
				}
			}

			DataHolder.Accessory(selection).priceType = (PriceType)EditorTab.EnumToolbar("Buy Price Type", (int)DataHolder.Accessory(selection).priceType, typeof(PriceType));
			DataHolder.Accessory(selection).buyPrice = EditorGUILayout.IntField("Buy price", DataHolder.Accessory(selection).buyPrice, GUILayout.Width(pw.mWidth));
			DataHolder.Accessory(selection).isAvailable = EditorGUILayout.Toggle("Available", DataHolder.Accessory(selection).isAvailable, GUILayout.Width(pw.mWidth));
			DataHolder.Accessory(selection).itemTag = (ItemTag)EditorTab.EnumToolbar("Tag", (int)DataHolder.Accessory(selection).itemTag, typeof(ItemTag));
			DataHolder.Accessory(selection).levelRequire = EditorGUILayout.IntField("Level Require", DataHolder.Accessory(selection).levelRequire, GUILayout.Width(pw.mWidth));
			DataHolder.Accessory(selection).accessoryId = EditorGUILayout.IntField("Accessory Id", DataHolder.Accessory(selection).accessoryId, GUILayout.Width(pw.mWidth));
			DataHolder.Accessory(selection).petId = EditorGUILayout.IntField("Pet Id", DataHolder.Accessory(selection).petId, GUILayout.Width(pw.mWidth));


			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}
		this.EndTab();
	}
}