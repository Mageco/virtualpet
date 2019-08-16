
using UnityEditor;
using UnityEngine;

public class CategoryTab : BaseTab
{
	public CategoryTab(ProjectWindow pw) : base(pw)
	{
		this.Reload();
	}
	
	public void ShowTab()
	{
		EditorGUILayout.BeginVertical();
		
		// buttons
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Add Category", GUILayout.Width(pw.mWidth)))
		{
			DataHolder.Categories().AddCategory("New Category");
			selection = DataHolder.Categories().GetDataCount()-1;
			GUI.FocusControl ("ID");
		}
		this.ShowCopyButton(DataHolder.Categories());
		if(selection >= 0)
		{
			this.ShowRemButton("Remove Category", DataHolder.Categories());
		}
		this.CheckSelection(DataHolder.Categories());
		EditorGUILayout.EndHorizontal();
		
		// color list
		this.AddItemList(DataHolder.Categories());
		
		// color settings
		EditorGUILayout.BeginVertical();
		SP2 = EditorGUILayout.BeginScrollView(SP2);
		if(DataHolder.Categories().GetDataCount() > 0)
		{
			EditorGUILayout.BeginHorizontal();
			this.AddID("Category ID");
			DataHolder.Categories().name[selection] = EditorGUILayout.TextField("Category name", DataHolder.Categories().name[selection]);
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
		}
		this.EndTab();
	}
}