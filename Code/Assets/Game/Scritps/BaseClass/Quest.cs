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
    //Rewards
    public bool haveItem = false;
	public bool havePet = false;
	public int itemId = 0;
	public int petId = 0;
	public int coinValue = 0;
    public int diamondValue = 0;
    public int expValue = 0;
	public int itemNumber = 0;
	//Dialog

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




