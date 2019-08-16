
using UnityEditor;
using UnityEngine;

public class StatusValueTab : BaseTab
{
	public StatusValueTab(ProjectWindow pw) : base(pw)
	{
		this.Reload();
	}
	
	public void ShowTab()
	{
		int tmpSelection = selection;
		EditorGUILayout.BeginVertical();

		// buttons
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Status Value", GUILayout.Width(pw.mWidth)))
		{
			DataHolder.StatusValues().AddValue();
			selection = DataHolder.StatusValues().GetDataCount() - 1;
			GUI.FocusControl("ID");
			pw.AddStatusValue (selection);
		}
		this.ShowCopyButton(DataHolder.StatusValues());
		if (selection >= 0)
		{
			this.ShowRemButton("Remove Status Value", DataHolder.StatusValues());
		}
		this.CheckSelection(DataHolder.StatusValues());
		EditorGUILayout.EndHorizontal();

		// color list
		this.AddItemList(DataHolder.StatusValues());

		// color settings

		EditorGUILayout.BeginVertical();
		SP2 = EditorGUILayout.BeginScrollView(SP2);
		if (DataHolder.StatusValues().GetDataCount() > 0)
		{
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();

			for (int i = 0; i < DataHolder.Languages().GetDataCount(); i++)
			{
				if (DataHolder.StatusValue(selection).languageItem[i] == null)
					DataHolder.StatusValue(selection).AddLanguageItem();

				EditorGUILayout.LabelField(DataHolder.Language(i));
				DataHolder.StatusValue(selection).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.StatusValue(selection).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
				DataHolder.StatusValue(selection).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.StatusValue(selection).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
				EditorGUILayout.Separator();
			}

			fold2 = EditorGUILayout.Foldout(fold2, "Settings");
			if(fold2)
			{
				DataHolder.StatusValue(selection).minValue = EditorGUILayout.IntField("Minimum Value", DataHolder.StatusValue(selection).minValue, GUILayout.Width(pw.mWidth));
				DataHolder.StatusValue(selection).maxValue = EditorGUILayout.IntField("Maximum Value", DataHolder.StatusValue(selection).maxValue, GUILayout.Width(pw.mWidth));
				if(GUILayout.Button("Apply Changes", GUILayout.Width(pw.mWidth)))
				{
					pw.StatusValueMinMaxChanged(selection, DataHolder.StatusValue(selection).minValue, DataHolder.StatusValue(selection).maxValue);
				}

				EditorGUILayout.Separator();
				var prev = DataHolder.StatusValue(selection).type;
				DataHolder.StatusValue(selection).type = (StatusValueType)EditorTab.EnumToolbar("Type", (int)DataHolder.StatusValue(selection).type, typeof(StatusValueType), pw.mWidth*2);
				EditorGUILayout.Separator();
				if(prev != DataHolder.StatusValue(selection).type)
				{
					pw.SetStatusValueType(selection, DataHolder.StatusValue(selection).type);
				}

				if(StatusValueType.CONSUMABLE.Equals(DataHolder.StatusValue(selection).type))
				{
					DataHolder.StatusValue(selection).maxStatus = EditorGUILayout.Popup("Max Status Value", DataHolder.StatusValue(selection).maxStatus, DataHolder.StatusValues().GetNameList(true), GUILayout.Width(pw.mWidth));
					DataHolder.StatusValue(selection).killChar = EditorGUILayout.Toggle("Death on min.", DataHolder.StatusValue(selection).killChar, GUILayout.Width(pw.mWidth));
				}
				else if(StatusValueType.EXPERIENCE.Equals(DataHolder.StatusValue(selection).type))
				{
					var prev2 = DataHolder.StatusValue(selection).levelUp;
					DataHolder.StatusValue(selection).levelUp = EditorGUILayout.Toggle("Causes level up", DataHolder.StatusValue(selection).levelUp, GUILayout.Width(pw.mWidth));
					if(prev2 != DataHolder.StatusValue(selection).levelUp && DataHolder.StatusValue(selection).levelUp)
					{
						for(int i=0; i<DataHolder.StatusValues().GetDataCount(); i++)
						{
							if(selection != i)
							{
								DataHolder.StatusValues().statusValues[i].levelUp = false;
							}
						}
					}
				}
			}

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}
		this.EndTab();






	}
}