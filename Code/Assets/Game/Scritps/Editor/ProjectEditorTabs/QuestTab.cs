using UnityEditor;
using UnityEngine;

public class QuestTab : BaseTab
{
	private GameObject tmpPrefab;
    private int tempId = -1;

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
			fold2 = EditorGUILayout.Foldout(fold2, "Quest Info");
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

			}
			EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
			fold2 = EditorGUILayout.Foldout(fold2, "Quest Reward");
			if(fold2)
			{
				DataHolder.Quest(selection).coinValue = EditorGUILayout.IntField("Coin", DataHolder.Quest(selection).coinValue, GUILayout.Width(pw.mWidth));
                DataHolder.Quest(selection).diamondValue = EditorGUILayout.IntField("Diamond", DataHolder.Quest(selection).diamondValue, GUILayout.Width(pw.mWidth));
                DataHolder.Quest(selection).expValue = EditorGUILayout.IntField("Exp", DataHolder.Quest(selection).expValue, GUILayout.Width(pw.mWidth));
                DataHolder.Quest(selection).haveItem = EditorGUILayout.Toggle("Item Reward", DataHolder.Quest(selection).haveItem, GUILayout.Width(pw.mWidth));
                if (DataHolder.Quest(selection).haveItem)
                {
                   if(tempId == -1)
                        tempId = DataHolder.Items().GetItemPosition(DataHolder.Quest(selection).itemId);
                   tempId = EditorGUILayout.Popup("Item",tempId, DataHolder.Items().GetNameList(true), GUILayout.Width(pw.mWidth));

                    DataHolder.Quest(selection).itemId = DataHolder.Item(tempId).iD;
                    
                }
				EditorGUILayout.Separator();
			}
			EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

        }
        this.EndTab();

        
    }
}