
using UnityEditor;
using UnityEngine;



public class DialogTab : BaseTab
{
	private GameObject tmpPrefab;

    public DialogTab(ProjectWindow pw) : base(pw)
    {
        this.Reload();
    }

    public void ShowTab()
    {
		int tmpSelection = selection;
        EditorGUILayout.BeginVertical();

        // buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Dialog", GUILayout.Width(pw.mWidth)))
        {
            DataHolder.Dialogs().AddDialog();
            selection = DataHolder.Dialogs().GetDataCount() - 1;
            GUI.FocusControl("ID");
        }
        this.ShowCopyButton(DataHolder.Dialogs());
        if (selection >= 0)
        {
            this.ShowRemButton("Remove Dialog", DataHolder.Dialogs());
        }
        this.CheckSelection(DataHolder.Dialogs());
        EditorGUILayout.EndHorizontal();

        // color list
        this.AddItemList(DataHolder.Dialogs());
        // color settings

        EditorGUILayout.BeginVertical();
        SP2 = EditorGUILayout.BeginScrollView(SP2);
        if (DataHolder.Dialogs().GetDataCount() > 0)
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
                DataHolder.Dialog(selection).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.Dialog(selection).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
                DataHolder.Dialog(selection).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.Dialog(selection).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                EditorGUILayout.Separator();
            }

        
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            //EditorGUILayout.EndHorizontal();


        }
        if (GUILayout.Button("Clear All", GUILayout.Width(pw.mWidth)))
        {
            for (int i = 1; i < DataHolder.Languages().GetDataCount(); i++)
            {
                DataHolder.Dialog(selection).languageItem[i].Name = "";
                DataHolder.Dialog(selection).languageItem[i].Description = "";
            }
        }
        this.EndTab();
    }
}