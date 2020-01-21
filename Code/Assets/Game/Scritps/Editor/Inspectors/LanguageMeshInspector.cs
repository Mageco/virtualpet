
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LanguageMesh))]
public class LanguageMeshInspector : Editor
{
	int id = 2;
	public override void OnInspectorGUI()
	{
		Undo.RecordObject(target, "Changed");
		if (((LanguageMesh)target).dialogId != -1)
		{
			id = DataHolder.Dialogs().GetDialogPosition(((LanguageMesh)target).dialogId);
		}
		id = EditorGUILayout.Popup("Dialog", id, DataHolder.Dialogs().GetNameList(true), GUILayout.Width(300));
		((LanguageMesh)target).dialogId = DataHolder.Dialog(id).iD;
		((LanguageMesh)target).GetComponent<TextMesh>().text = DataHolder.GetDialog(((LanguageMesh)target).dialogId).GetName(0);
	}
}
