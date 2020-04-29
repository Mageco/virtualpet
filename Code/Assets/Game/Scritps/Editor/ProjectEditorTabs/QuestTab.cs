using UnityEditor;
using UnityEngine;

public class QuestTab : BaseTab
{
	private GameObject tmpPrefab;
    private int tempId = 0;
    int lastSellection = -1;

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
        this.AddItemList(DataHolder.Quests());

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

            //EditorGUILayout.LabelField("Quest ID: " + DataHolder.Quests().GetQuest(selection).iD.ToString(), GUILayout.Width(pw.mWidth * 2));

            for (int i = 0; i < DataHolder.Languages().GetDataCount(); i++)
            {
                EditorGUILayout.LabelField(DataHolder.Language(i));
                DataHolder.Quest(selection).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.Quest(selection).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
                DataHolder.Quest(selection).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.Quest(selection).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                EditorGUILayout.Separator();
            }

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
                   if(lastSellection != selection)
                        tempId = DataHolder.Items().GetItemPosition(DataHolder.Quest(selection).itemId);

                    if(tempId == -1)
                        tempId = 0;
                 
                   tempId = EditorGUILayout.Popup("Item",tempId, DataHolder.Items().GetNameList(true), GUILayout.Width(pw.mWidth));

                   if(DataHolder.Item(tempId) != null)
                        DataHolder.Quest(selection).itemId = DataHolder.Item(tempId).iD;
                   lastSellection = selection;
                    
                }
                DataHolder.Quest(selection).havePet = EditorGUILayout.Toggle("Item Reward", DataHolder.Quest(selection).havePet, GUILayout.Width(pw.mWidth));
                if (DataHolder.Quest(selection).havePet)
                {
                    DataHolder.Quest(selection).petId = EditorGUILayout.Popup("Pet", DataHolder.Quest(selection).petId, DataHolder.Pets().GetNameList(true), GUILayout.Width(pw.mWidth));
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