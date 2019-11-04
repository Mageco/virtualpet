
using UnityEditor;
using UnityEngine;

public class SkillTab : BaseTab
{

	private GameObject tmpPrefab;
	private string[] sections = new string[] {"1"};
	
	public SkillTab(ProjectWindow pw) : base(pw)
	{
		this.Reload();
	}
	
	public void ShowTab()
	{
		int tmpSelection = selection;
		EditorGUILayout.BeginVertical();

		// buttons
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Skill", GUILayout.Width(pw.mWidth)))
		{
			DataHolder.Skills().AddSkill();
			selection = DataHolder.Skills().GetDataCount() - 1;
			GUI.FocusControl("ID");
		}
		this.ShowCopyButton(DataHolder.Skills());
		if (selection >= 0)
		{
			this.ShowRemButton("Remove Skill", DataHolder.Skills());
		}
		this.CheckSelection(DataHolder.Skills());
		EditorGUILayout.EndHorizontal();

		// color list
		this.AddItemFilterList(DataHolder.Skills());

		// color settings

		EditorGUILayout.BeginVertical();
		SP2 = EditorGUILayout.BeginScrollView(SP2);
		if (DataHolder.Skills().GetDataCount() > 0)
		{
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();

			for (int i = 0; i < DataHolder.Languages().GetDataCount(); i++)
			{
				if (DataHolder.Skill(selection).languageItem[i] == null)
					DataHolder.Skill(selection).AddLanguageItem();

				EditorGUILayout.LabelField(DataHolder.Language(i));
				DataHolder.Skill(selection).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.Skill(selection).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
				DataHolder.Skill(selection).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.Skill(selection).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
				EditorGUILayout.Separator();
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
			if(DataHolder.Skill(selection).iconUrl != null){
                this.tmpSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Skill(selection).iconUrl);
            }
			this.tmpSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
			if (this.tmpSprites != null)
			{
				DataHolder.Skill(selection).iconUrl = AssetDatabase.GetAssetPath(this.tmpSprites);

			}
			EditorGUILayout.LabelField(DataHolder.Skill(selection).iconUrl);
			EditorGUILayout.EndHorizontal();

			if (this.tmpSprites != null)
			{
				if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
				{
					DataHolder.Skill(selection).iconUrl = "";
					tmpSprites = null;
				}
			}

			DataHolder.Skill(selection).skillType = (SkillType)EditorGUILayout.EnumPopup("Skill Type", DataHolder.Skill(selection).skillType, GUILayout.Width (pw.mWidth * 2));
			DataHolder.Skill(selection).maxProgress = EditorGUILayout.IntField("Max Progress", DataHolder.Skill(selection).maxProgress, GUILayout.Width (pw.mWidth * 2));
			EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Skill Reward");
			if(fold2)
			{
				DataHolder.Skill(selection).coinValue = EditorGUILayout.IntField("Coin", DataHolder.Skill(selection).coinValue, GUILayout.Width(pw.mWidth));
                DataHolder.Skill(selection).diamondValue = EditorGUILayout.IntField("Diamond", DataHolder.Skill(selection).diamondValue, GUILayout.Width(pw.mWidth));
				DataHolder.Skill(selection).itemId = EditorGUILayout.Popup("Item", 
						DataHolder.Skill(selection).itemId, pw.GetItems(), GUILayout.Width(pw.mWidth));
				EditorGUILayout.Separator();
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}
		this.EndTab();
	}
}