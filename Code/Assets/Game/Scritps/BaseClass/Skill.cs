
using UnityEngine;
using System.Collections;
[System.Serializable]
public class Skill
{
	public int iD = 0;
	public int category = 0;
	public string iconUrl = "";
	public LanguageItem[] languageItem = new LanguageItem[0];

	public SkillType skillType = SkillType.NONE;
	public ValueChange[] valueChange = new ValueChange[0];

	public bool useable = false;
	public ItemVariableType itemVariable = ItemVariableType.NONE;
	public string variableKey = "";
	public string variableValue = "";

	public Skill()
	{
		iD = DataHolder.LastSkillID() + 1;
		languageItem = new LanguageItem[DataHolder.Languages ().GetDataCount ()];
		for (int i = 0; i < languageItem.Length; i++) {
			languageItem[i] = new LanguageItem();
		}
		languageItem[0].Name = "New Skill";
		valueChange = new ValueChange[DataHolder.StatusValues ().GetDataCount ()];
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