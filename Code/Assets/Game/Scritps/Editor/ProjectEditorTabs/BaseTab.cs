
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class BaseTab : EditorTab
{
	protected ProjectWindow pw;
	protected Texture2D tmpSprites;
	protected int temcategory = 0;
	protected int temorder = 0;

	public BaseTab(ProjectWindow pw) : base()
	{
		this.pw = pw;

	}
	
	public void Reload()
	{
		selection = 0;
	}

	public void AddItemFilterList(BaseData data)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical(GUILayout.Width(pw.mWidth*0.7f));
		EditorGUILayout.BeginHorizontal();
		temcategory = EditorGUILayout.Popup ("Category",temcategory, DataHolder.Categories ().GetNameList (true), GUILayout.Width (pw.mWidth * 0.7f));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator();
		EditorGUILayout.BeginVertical("box");
		SP1 = EditorGUILayout.BeginScrollView(SP1,GUILayout.Width(pw.mWidth*0.7f));

		if(data.GetDataCount() > 0)
		{
			var prev = selection;
			temorder = GUILayout.SelectionGrid(temorder, data.GetNameListFilter(temcategory), 1);
			selection =  DataHolder.Items ().GetItemOrder (temorder, temcategory);
			if(prev != temorder)
			{
				GUI.FocusControl("ID");
			}
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical();

	}
	
	public void AddItemList(BaseData data)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical(GUILayout.Width(pw.mWidth*0.7f));
		EditorGUILayout.BeginVertical("box");
		SP1 = EditorGUILayout.BeginScrollView(SP1,GUILayout.Width(pw.mWidth*0.7f));
		
		if(data.GetDataCount() > 0)
		{
			var prev = selection;
			selection = GUILayout.SelectionGrid(selection, data.GetNameList(true), 1);
			if(prev != selection)
			{
				GUI.FocusControl("ID");
			}
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical();
	}
	
	public void AddID(string text)
	{
		EditorGUILayout.BeginVertical();
		EditorGUILayout.Separator();
		GUI.SetNextControlName("ID");
		EditorGUILayout.LabelField(text, selection.ToString(), GUILayout.Width(pw.mWidth));
	}
	
	public bool ShowRemButton(string text, BaseData data)
	{
		GUI.SetNextControlName("Rem");
		bool press = GUILayout.Button(text, GUILayout.Width(pw.mWidth));
		if(press)
		{
			GUI.FocusControl("Rem");
			data.RemoveData(selection);
		}
		return press;
	}
	
	public bool ShowCopyButton(BaseData data)
	{
		GUI.SetNextControlName("Copy");
		bool press = GUILayout.Button("Copy", GUILayout.Width(pw.mWidth));
		if(press)
		{
			GUI.FocusControl("Copy");
			data.Copy(selection);
			selection = data.GetDataCount()-1;
		}
		return press;
	}
	

	public void EndTab()
	{
		EditorGUILayout.Separator();
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}
	
	public void CheckSelection(BaseData data)
	{
		if (selection > data.GetDataCount () - 1) {
			selection = data.GetDataCount () - 1;
		} 
	}

}