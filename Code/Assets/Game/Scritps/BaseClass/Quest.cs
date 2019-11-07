using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest 
{
    public int iD =0;
    public int category = 0;
    public LanguageItem[] languageItem = new LanguageItem[0];
    //Requirement
    public int charLevel;
    public QuestRequirement[] requirements = new QuestRequirement[0];
	public string prefabName = "";
    //Rewards
    public int itemId = 0;
    public int coinValue = 0;
    public int diamondValue = 0;
    public int expValue = 0;
    //Dialog
    public int dialogId = 0;

    public Quest()
	{
		iD = DataHolder.LastQuestID() + 1;
		languageItem = new LanguageItem[DataHolder.Languages ().GetDataCount ()];
		for (int i = 0; i < languageItem.Length; i++) {
			languageItem[i] = new LanguageItem();
		}
		languageItem[0].Name = "New Quest";
	}


    public void AddLanguageItem()
	{
		this.languageItem = ArrayHelper.Add(new LanguageItem(), this.languageItem);
	}

	public void RemoveLanguage(int index)
	{
		this.languageItem = ArrayHelper.Remove (index, this.languageItem);
	}

    public void AddRequirement()
	{
		this.requirements = ArrayHelper.Add(new QuestRequirement(), this.requirements);
	}

	public void RemoveRequirement(int index)
	{
		this.requirements = ArrayHelper.Remove (index, this.requirements);
	}

	public string GetDescription(int languageID)
	{
		return languageItem[languageID].Description;  
	}

	public void SetDescription(int languageID,string text)
	{
		if (this.languageItem.Length > languageID) {
			languageItem [languageID].Description = text;
		} else {
			LanguageItem item = new LanguageItem ();
			item.Description = text;
			this.languageItem = ArrayHelper.Add (item, this.languageItem);
		}
	}

	public string GetName(int languageID)
	{
		return languageItem[languageID].Name;  
	}

	public void SetName(int languageID,string text)
	{
		if (this.languageItem.Length > languageID) {
			languageItem [languageID].Name = text;
		} else {
			LanguageItem item = new LanguageItem ();
			item.Name = text;
			this.languageItem = ArrayHelper.Add (item, this.languageItem);
		}
	}
}

[System.Serializable]
public class QuestRequirement{

     public QuestRequirementType requireType;
     public ActionType actionType;
     public InteractType interactType;
     public SkillType skillType;
     public string key = "";
	 public string value = "";

}


