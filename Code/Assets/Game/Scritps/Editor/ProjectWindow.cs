
using System.Collections;
using UnityEditor;
using UnityEngine;


public class ProjectWindow : EditorWindow
{
	//private bool isInit = false;
	public int mWidth = 300;
	// section handling
	private int currentSection = 0;
	private string[] sections = new string[] {"Language","Skill","Item"};
	
	private int LANGUAGES = 0;
	private int SKILL = 1;
	private int ITEMS = 2;

	// tabs
	private LanguageTab langTab = null;
	private ItemTab itemTab = null;
	private SkillTab skillTab = null;

	[MenuItem("Mage Kit/Item Editor")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		ProjectWindow window = (ProjectWindow)EditorWindow.GetWindow(typeof(ProjectWindow), false, "Item Editor");
		window.Reload();
		window.Show();

	}
	
	public void Reload()
	{
		DataHolder.Instance().Init();
		if(langTab == null) langTab = new LanguageTab(this);
		else langTab.Reload();
		
		if(itemTab == null) itemTab = new ItemTab(this);
		else itemTab.Reload();

		if(skillTab == null) skillTab = new SkillTab(this);
		else skillTab.Reload();
	}
	
	public void Save()
	{
		DataHolder.Languages().SaveData();
		DataHolder.Items().SaveData();
		DataHolder.Skills().SaveData();
	}

	void OnGUI()
	{
		if (this.langTab == null)
			Init ();


		GUI.SetNextControlName ("Toolbar");
		var prevSection = currentSection;
		currentSection = GUILayout.SelectionGrid (currentSection, sections, 7);
		if (prevSection != currentSection) {
			GUI.FocusControl ("Toolbar");
		}
		GUILayout.Box (" ", GUILayout.ExpandWidth (true));
	
		if (currentSection == this.LANGUAGES) {
			this.langTab.ShowTab ();
		} else if (currentSection == this.ITEMS) {
			this.itemTab.ShowTab ();
		}  else if (currentSection == this.SKILL) {
			this.skillTab.ShowTab ();
		} 


		EditorGUILayout.Separator ();
		EditorGUILayout.BeginHorizontal ();
		GUI.SetNextControlName ("Reload");
		if (GUILayout.Button ("Reload Settings")) {
			GUI.FocusControl ("Reload");
			this.Reload ();
		}
		GUI.SetNextControlName ("Save");
		if (GUILayout.Button ("Save Settings")) {
			GUI.FocusControl ("Save");
			this.Save ();
		}

		EditorGUILayout.EndHorizontal ();

	}
	
	// add/remove
	
	public void AddLanguage(int lang)
	{
		DataHolder.Items ().AddLanguage ();
		DataHolder.Skills ().AddLanguage ();
	}
	
	public void RemoveLanguage(int lang)
	{
		DataHolder.Items ().RemoveLanguage (lang);
		DataHolder.Skills ().RemoveLanguage (lang);
	}


	
	public int GetLangCount()
	{
		return DataHolder.Languages().GetDataCount();
	}
	
	public string GetLang(int index)
	{
		return DataHolder.Languages().GetName(index);
	}

	

	
	public int GetItemCount()
	{
		return DataHolder.Items().GetDataCount();
	}
	
	public string GetItem(int index)
	{
		return DataHolder.Items().GetName(index);
	}
	
	public string[] GetItems()
	{
		return DataHolder.Items().GetNameList(true);
	}
	
	public void RemoveItem(int item)
	{

	}

	public int GetSkillCount()
	{
		return DataHolder.Skills().GetDataCount();
	}

	public string GetSkill(int index)
	{
		return DataHolder.Skills().GetName(index);
	}

	public string[] GetSkills()
	{
		return DataHolder.Skills().GetNameList(true);
	}

	public void RemoveSkill(int item)
	{

	}
}