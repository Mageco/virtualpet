using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(LanguageUI))]
public class LanguageUIInspector : Editor
{
	int id = 1;
	public override void OnInspectorGUI()
	{
		Undo.RecordObject(target, "Changed");
		if (((LanguageUI)target).dialogId != -1)
		{
			id = DataHolder.Dialogs().GetDialogPosition(((LanguageUI)target).dialogId);
		}
		//Debug.Log(id);
		//((TextVoiceOver)target).text = (Text)EditorGUILayout.ObjectField ("Description",((TextVoiceOver)target).text, typeof(Text), true, GUILayout.MaxWidth (300));
		id = EditorGUILayout.Popup("Dialog", id, DataHolder.Dialogs().GetNameList(true), GUILayout.Width(300));
		((LanguageUI)target).dialogId = DataHolder.Dialog(id).iD;
		((LanguageUI)target).GetComponent<Text>().text = DataHolder.GetDialog(((LanguageUI)target).dialogId).GetName(0);
	}
}
