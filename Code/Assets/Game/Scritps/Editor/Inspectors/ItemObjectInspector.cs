//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//
//[CustomEditor(typeof(ItemObject))]
//public class ItemObjectInspector : Editor
//{
//	int tempId;
//	public override void OnInspectorGUI()
//	{
//		Undo.RecordObject (target, "Changed");
//		tempId = EditorGUILayout.Popup("Item", tempId, DataHolder.Items ().GetNameList(true), GUILayout.Width(250));
//		((ItemObject)target).itemID = DataHolder.Item (tempId).iD;
//	}
//}
