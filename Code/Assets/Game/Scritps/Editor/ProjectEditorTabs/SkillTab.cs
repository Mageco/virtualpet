
using UnityEditor;
using UnityEngine;

public class SkillTab : BaseTab
{

	private GameObject tmpPrefab;


	private int levelSelection = 0;
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

			EditorGUILayout.Separator();
			DataHolder.Skill(selection).category = EditorGUILayout.Popup("Category", DataHolder.Skill(selection).category, DataHolder.Categories().GetNameList(true), GUILayout.Width(pw.mWidth * 2));
			EditorGUILayout.Separator();

			DataHolder.Skill(selection).skillType = (SkillType)EditorGUILayout.EnumPopup("Skill Type", DataHolder.Skill(selection).skillType, GUILayout.Width (pw.mWidth * 2));
			EditorGUILayout.Separator();

			
			EditorGUILayout.BeginVertical("box");
			fold3 = EditorGUILayout.Foldout(fold3, "User Settings");
			if(fold3)
			{
				GUILayout.Label ("Consume Status Value", EditorStyles.boldLabel);

				EditorGUILayout.Separator();
				for(int i=0; i<pw.GetStatusValueCount(); i++)
				{
					if(pw.IsStatusValueConsumable(i))
					{
						if(DataHolder.Skill(selection).valueChange[i] == null)
							DataHolder.Skill(selection).valueChange[i] = new ValueChange();
						DataHolder.Skill(selection).valueChange[i] = EditorHelper.ValueChangeSettings(i,DataHolder.Skill(selection).valueChange[i]);
					}
				}
				this.Separate();
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}
		this.EndTab();
	}
}