
using UnityEditor;
using UnityEngine;

public class LanguageTab : BaseTab
{
	public LanguageTab(ProjectWindow pw) : base(pw)
	{
		this.Reload();
	}
	
	public void ShowTab()
	{
		EditorGUILayout.BeginVertical();
		
		// buttons
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Add Language", GUILayout.Width(pw.mWidth)))
		{
			DataHolder.Languages().AddLanguage(0);
			selection = DataHolder.Languages().GetDataCount()-1;
			pw.AddLanguage(selection);
			GUI.FocusControl ("ID");
		}
		if(this.ShowCopyButton(DataHolder.Languages()))
		{
			pw.AddLanguage(selection);
		}
		if(DataHolder.Languages().GetDataCount() > 0)
		{
			if(this.ShowRemButton("Remove Language", DataHolder.Languages()))
			{
				pw.RemoveLanguage(selection);
			}
		}
		this.CheckSelection(DataHolder.Languages());
		EditorGUILayout.EndHorizontal();
		
		// status value list
		this.AddItemList(DataHolder.Languages());
		
		// value settings
		EditorGUILayout.BeginVertical();
		SP2 = EditorGUILayout.BeginScrollView(SP2);
		if(DataHolder.Languages().GetDataCount() > 0)
		{
			this.AddID("Language ID");
			DataHolder.Languages().languageIDs[selection] = EditorGUILayout.Popup("Language name", DataHolder.Languages().languageIDs[selection],DataHolder.LanguageName, GUILayout.Width (pw.mWidth * 2));
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Language Code ", GUILayout.Width (pw.mWidth * 0.5f));
			EditorGUILayout.LabelField(DataHolder.LanguageCode [DataHolder.Languages ().languageIDs [selection]], GUILayout.Width (pw.mWidth * 0.5f));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical();
		}
		this.EndTab();
	}
}