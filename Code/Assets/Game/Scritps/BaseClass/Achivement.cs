using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achivement
{
    public int iD = 0;
	public int category = 0;
	public string iconUrl = "";
	public LanguageItem[] languageItem = new LanguageItem[0];

	public AchivementType achivementType = AchivementType.Do_Action;
    public ActionType actionType;
    public AnimalType animalType;
    public int itemId = 0;    
    public int collectPointNumber = 3;
	public int[] maxProgress = new int[0];
    public string[] levelDescription = new string[0];

	//Rewards
    public int[] coinValue = new int[0];
    public int[] diamondValue = new int[0];
	public int order = 0;


	public Achivement()
	{
		iD = DataHolder.LastAchivementID() + 1;
		languageItem = new LanguageItem[DataHolder.Languages ().GetDataCount ()];
		for (int i = 0; i < languageItem.Length; i++) {
			languageItem[i] = new LanguageItem();
		}
		languageItem[0].Name = "New Achivement";
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
