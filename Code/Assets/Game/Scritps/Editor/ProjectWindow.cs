
using System.Collections;
using UnityEditor;
using UnityEngine;


public class ProjectWindow : EditorWindow
{
	//private bool isInit = false;
	public int mWidth = 300;
	// section handling
	private int currentSection = 0;
	private string[] sections = new string[] {"Language","Item","Dialog","Quest","Pet","Achivement","Accessories"};
	
	private int LANGUAGES = 0;
	//private int SKILL = 1;
	private int ITEMS = 1;
	private int DIALOGS = 2;
	private int QUESTS = 3;
	private int PETS = 4;
	private int ACHIVEMENTS = 5;
	private int ACCESSORY = 6;

	// tabs
	private LanguageTab langTab = null;
	private ItemTab itemTab = null;
	//private SkillTab skillTab = null;
	private DialogTab dialogTab = null;
	private QuestTab questTab = null;
	private PetTab petTab = null;
	private AchivementTab achivementTab = null;
	private AccessoryTab accessoryTab = null;



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

		//if(skillTab == null) skillTab = new SkillTab(this);
		//else skillTab.Reload();
				
		if(dialogTab == null) dialogTab = new DialogTab(this);
		else dialogTab.Reload();
		
		if(questTab == null) questTab = new QuestTab(this);
		else questTab.Reload();

		if(petTab == null) petTab = new PetTab(this);
		else petTab.Reload();
		
		if(achivementTab == null) achivementTab = new AchivementTab(this);
		else achivementTab.Reload();

		if (accessoryTab == null) accessoryTab = new AccessoryTab(this);
		else accessoryTab.Reload();
	}
	
	public void Save()
	{
		DataHolder.Languages().SaveData();
		DataHolder.Items().SaveData();
		//DataHolder.Skills().SaveData();
		DataHolder.Dialogs().SaveData();
		DataHolder.Quests().SaveData();
		DataHolder.Pets().SaveData();
		DataHolder.Achivements().SaveData();
		DataHolder.Accessories().SaveData();
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

		if (currentSection == this.LANGUAGES)
		{
			this.langTab.ShowTab();
		}
		else if (currentSection == this.ITEMS)
		{
			this.itemTab.ShowTab();
			//}  else if (currentSection == this.SKILL) {
			//	this.skillTab.ShowTab ();
		}
		else if (currentSection == this.DIALOGS)
		{
			this.dialogTab.ShowTab();
		}
		else if (currentSection == this.QUESTS)
		{
			this.questTab.ShowTab();
		}
		else if (currentSection == this.PETS)
		{
			this.petTab.ShowTab();
		}
		else if (currentSection == this.ACHIVEMENTS)
		{
			this.achivementTab.ShowTab();
		}
		else if (currentSection == this.ACCESSORY)
		{
			this.accessoryTab.ShowTab();
		}


		EditorGUILayout.Separator ();
		EditorGUILayout.BeginHorizontal ();
		GUI.SetNextControlName("Import");
		if (GUILayout.Button("Import Items"))
		{
			DataHolder.Dialogs().ImportData();
			GUI.FocusControl("Import");
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
		DataHolder.Dialogs ().AddLanguage ();
		DataHolder.Quests ().AddLanguage ();
		DataHolder.Pets ().AddLanguage ();
		DataHolder.Achivements ().AddLanguage ();
		DataHolder.Accessories().AddLanguage();
	}
	
	public void RemoveLanguage(int lang)
	{
		DataHolder.Items ().RemoveLanguage (lang);
		DataHolder.Skills ().RemoveLanguage (lang);
		DataHolder.Dialogs ().RemoveLanguage (lang);
		DataHolder.Quests ().RemoveLanguage (lang);
		DataHolder.Pets ().RemoveLanguage (lang);
		DataHolder.Achivements ().RemoveLanguage (lang);
		DataHolder.Accessories().RemoveLanguage(lang);
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

	public int GetDialogCount()
	{
		return DataHolder.Dialogs().GetDataCount();
	}

	public string GetDialog(int index)
	{
		return DataHolder.Dialogs().GetName(index);
	}

	public string[] GetDialogs()
	{
		return DataHolder.Dialogs().GetNameList(true);
	}

	public void RemoveDialog(int item)
	{

	}

	public int GetQuestCount()
	{
		return DataHolder.Quests().GetDataCount();
	}
	
	public string GetQuest(int index)
	{
		return DataHolder.Quests().GetName(index);
	}
	
	public string[] GetQuests()
	{
		return DataHolder.Quests().GetNameList(true);
	}
	
	public void RemoveQuest(int item)
	{

	}
}