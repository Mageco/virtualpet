using UnityEditor;
using UnityEngine;

public class QuestTab : BaseTab
{
	private GameObject tmpPrefab;

    public QuestTab(ProjectWindow pw) : base(pw)
    {
        this.Reload();
    }

    public void ShowTab()
    {
		int tmpSelection = selection;
        EditorGUILayout.BeginVertical();

        // buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Quest", GUILayout.Width(pw.mWidth)))
        {
            DataHolder.Quests().AddQuest();
            selection = DataHolder.Quests().GetDataCount() - 1;
            GUI.FocusControl("ID");
        }
        this.ShowCopyButton(DataHolder.Quests());
        if (selection >= 0)
        {
            this.ShowRemButton("Remove Quest", DataHolder.Quests());
        }
        this.CheckSelection(DataHolder.Quests());
        EditorGUILayout.EndHorizontal();

        // color list
        this.AddItemFilterList(DataHolder.Quests());

        // color settings

        EditorGUILayout.BeginVertical();
        SP2 = EditorGUILayout.BeginScrollView(SP2);
        if (DataHolder.Quests().GetDataCount() > 0)
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
                DataHolder.Quest(selection).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.Quest(selection).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
                DataHolder.Quest(selection).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.Quest(selection).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                EditorGUILayout.Separator();
            }

            EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Quest Requirement");
			if(fold2)
			{
				DataHolder.Quest(selection).charLevel = EditorGUILayout.IntField("Pet Level", DataHolder.Quest(selection).charLevel, GUILayout.Width(pw.mWidth));
				EditorGUILayout.Separator();
				DataHolder.Quest(selection).dialogId = EditorGUILayout.Popup("Dialog", 
						DataHolder.Quest(selection).dialogId, pw.GetDialogs(), GUILayout.Width(pw.mWidth));				
                EditorGUILayout.Separator();
                if(selection != tmpSelection) this.tmpPrefab = null;
				if(this.tmpPrefab == null && "" != DataHolder.Quest(selection).prefabName)
				{
					this.tmpPrefab = (GameObject)Resources.Load(DataHolder.Quests().GetPrefabPath()+DataHolder.Quest(selection).prefabName, typeof(GameObject));
				}
				this.tmpPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", this.tmpPrefab, typeof(GameObject), false, GUILayout.Width(pw.mWidth*2));
				if(this.tmpPrefab) DataHolder.Quest(selection).prefabName = this.tmpPrefab.name;
				else DataHolder.Quest(selection).prefabName = "";

				EditorGUILayout.Separator();
                for(int i=0;i<DataHolder.Quest(selection).requirements.Length;i++){
                    DataHolder.Quest(selection).requirements[i].requireType = (QuestRequirementType)EditorTab.EnumToolbar("Quest Type", (int)DataHolder.Quest(selection).requirements[i].requireType, typeof(QuestRequirementType));
                    if(DataHolder.Quest(selection).requirements[i].requireType == QuestRequirementType.Action){
                        EditorGUILayout.BeginHorizontal();
                        DataHolder.Quest(selection).requirements[i].actionType = (ActionType)EditorGUILayout.Popup("Action", (int)DataHolder.Quest(selection).requirements[i].actionType,System.Enum.GetNames(typeof(ActionType)),GUILayout.Width(pw.mWidth));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth * 0.3f)))
                        {
                            DataHolder.Quest(selection).RemoveRequirement(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }else if(DataHolder.Quest(selection).requirements[i].requireType == QuestRequirementType.Skill){
                        EditorGUILayout.BeginHorizontal();
                        DataHolder.Quest(selection).requirements[i].skillType = (SkillType)EditorGUILayout.Popup("Skill", (int)DataHolder.Quest(selection).requirements[i].skillType,System.Enum.GetNames(typeof(SkillType)),GUILayout.Width(pw.mWidth));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth * 0.3f)))
                        {
                            DataHolder.Quest(selection).RemoveRequirement(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }else if(DataHolder.Quest(selection).requirements[i].requireType == QuestRequirementType.Interact){
                        EditorGUILayout.BeginHorizontal();
                        DataHolder.Quest(selection).requirements[i].interactType = (InteractType)EditorGUILayout.Popup("Interact", (int)DataHolder.Quest(selection).requirements[i].interactType,System.Enum.GetNames(typeof(InteractType)),GUILayout.Width(pw.mWidth));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth * 0.3f)))
                        {
                            DataHolder.Quest(selection).RemoveRequirement(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }else if(DataHolder.Quest(selection).requirements[i].requireType == QuestRequirementType.Variable){
                        EditorGUILayout.BeginHorizontal();
                        DataHolder.Quest(selection).requirements[i].key = EditorGUILayout.TextField("Key", DataHolder.Quest(selection).requirements[i].key, GUILayout.Width(pw.mWidth));
                        DataHolder.Quest(selection).requirements[i].value = EditorGUILayout.TextField("Value", DataHolder.Quest(selection).requirements[i].value, GUILayout.Width(pw.mWidth));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth * 0.3f)))
                        {
                            DataHolder.Quest(selection).RemoveRequirement(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.Separator();
                }
                if (GUILayout.Button("Add Quest Requirement", GUILayout.Width(pw.mWidth)))
                {
                    DataHolder.Quest(selection).AddRequirement();
                }

			}
			EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Quest Reward");
			if(fold2)
			{
				DataHolder.Quest(selection).coinValue = EditorGUILayout.IntField("Coin", DataHolder.Quest(selection).coinValue, GUILayout.Width(pw.mWidth));
                DataHolder.Quest(selection).diamondValue = EditorGUILayout.IntField("Diamond", DataHolder.Quest(selection).diamondValue, GUILayout.Width(pw.mWidth));
                DataHolder.Quest(selection).expValue = EditorGUILayout.IntField("Exp", DataHolder.Quest(selection).expValue, GUILayout.Width(pw.mWidth));
				DataHolder.Quest(selection).itemId = EditorGUILayout.Popup("Item", 
						DataHolder.Quest(selection).itemId, pw.GetItems(), GUILayout.Width(pw.mWidth));
				EditorGUILayout.Separator();
			}
			EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

        }
        this.EndTab();

        
    }
}