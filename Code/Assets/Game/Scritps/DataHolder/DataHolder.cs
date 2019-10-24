
using UnityEngine;

public class DataHolder
{
	private static DataHolder instance;
	
	// data
	private ItemData items;
	private LanguageData languages;
	private SkillData skills;

	private DataHolder()
	{
		if(instance != null)
		{
			Debug.Log("There is already an instance of DataHolder!");
			return;
		}
		instance = this;
		Init();
	}
	
	public void Init()
	{
		// first init languages
		languages = new LanguageData();
		items = new  ItemData();
		skills = new SkillData ();
	}



		
	
	public static DataHolder Instance()
	{
		if(instance == null)
		{
			new DataHolder();
		}
		return instance;
	}
		


	public static ItemData Items()
	{
		return DataHolder.Instance().items;
	}
	
	public static Item Item(int index)
	{
		return DataHolder.Instance().items.items[index];
	}

	public static Item GetItem(int id)
	{
		return DataHolder.Instance().items.GetItem(id);
	}

	public static int GetItemIndex(int id){
		for(int i=0;i<DataHolder.Items().GetDataCount();i++){
			if(DataHolder.Item(i).iD == id)
				return i;
		}
		return 0;
	}

	public static int LastItemID(){
		if(DataHolder.Instance().items != null && DataHolder.Instance().items.GetDataCount() > 0)
			return DataHolder.Instance().items.items[DataHolder.Instance().items.items.Length - 1].iD;
		else 
			return 0;
	}

	public static SkillData Skills()
	{
		return DataHolder.Instance().skills;
	}

	public static Skill Skill(int index)
	{
		return DataHolder.Instance().skills.skills[index];
	}

	public static Skill GetSkill(int id)
	{
		return DataHolder.Instance().skills.GetSkill(id);
	}

	public static int GetSkillIndex(int id){
		for(int i=0;i<DataHolder.Skills().GetDataCount();i++){
			if(DataHolder.Skill(i).iD == id)
				return i;
		}
		return 0;
	}

	public static int LastSkillID(){
		if(DataHolder.Instance().skills != null && DataHolder.Instance().skills.GetDataCount() > 0)
			return DataHolder.Instance().skills.skills[DataHolder.Instance().skills.skills.Length - 1].iD;
		else 
			return 0;
	}


	public static LanguageData Languages()
	{
		return DataHolder.Instance().languages;
	}
	
	public static string Language(int index)
	{
		return  DataHolder.Instance().languages.GetName(index);
	}

	public static string[] LanguageName = new string[] {"Afrikaans","Arabic","Armenian","Belarusian","Bulgarian","Catalan"," Chinese (Simplified)","Chinese (Traditional)","Croatian"
		,"Czech","Danish","Dutch","English","Esperanto","Estonian","Filipino","Finnish","French","German","Greek","Hebrew","Hindi","Hungarian","Icelandic"
		,"Indonesian","Italian","Japanese","Korean","Latvian","Lithuanian","Norwegian","Persian","Polish","Portuguese","Romanian","Russian","Serbian"
		,"Slovak","Slovenian","Spanish","Swahili","Swedish","Thai","Turkish","Ukrainian","Vietnamese"};
	public static string[] LanguageCode = new string[] {"af","ar","hy","be","bg","ca","zh-CN","zh-TW","hr","cs","da","nl","en","eo","et","tl","fi","fr"
		,"de","el","iw","hi","hu","is","id","it","ja","ko","lv","lt","no","fa","pl","pt","ro","ru","sr","sk","sl","es","sw","sv","th","tr","uk","vi"};
	

}