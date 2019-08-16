using UnityEngine;
[System.Serializable]
public class StatusValue
{
	public int iD = 0;
	public LanguageItem[] languageItem = new LanguageItem[0];
	// settings
	public int minValue = 0;
	public int maxValue  = 99999;
	public StatusValueType type = StatusValueType.NORMAL;
	
	// type consumable
	public int maxStatus = 0;
	public bool killChar = false;
	
	// type experience
	public bool levelUp = false;
	
	// ingame
	private int baseValue = 0;
	private int currentValue = 0;

	public StatusValue()
	{
		iD = DataHolder.LastStatusValueID() + 1;
		languageItem = new LanguageItem[DataHolder.Languages ().GetDataCount ()];
		for (int i = 0; i < languageItem.Length; i++) {
			languageItem[i] = new LanguageItem();
		}
		languageItem[0].Name = "New Status";
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
	
	/*
	============================================================================
	Init functions
	============================================================================
	*/
	public void Init(int index)
	{
		StatusValue tmp = DataHolder.StatusValues().statusValues[index];
		this.minValue = tmp.minValue;
		this.maxValue = tmp.maxValue;
		this.type = tmp.type;
		this.maxStatus = tmp.maxStatus;
		this.killChar = tmp.killChar;
		this.levelUp = tmp.levelUp;
		this.iD = tmp.iD;
	}
	
	public void Init(int index, int val)
	{
		this.Init(index);
		this.InitValue(val);
	}
	
	public void InitValue(int val)
	{
		this.baseValue = val;
		this.currentValue = val;
	}
	
	/*
	============================================================================
	Get functions
	============================================================================
	*/
	public int GetValue()
	{
		
		return this.currentValue;
	}
	
	public int GetBaseValue()
	{
		return this.baseValue;
	}
	
	/*
	============================================================================
	Change functions
	============================================================================
	*/
	public void SetValue(int val)
	{
		this.currentValue = val;
	}
	
	public void AddValue(int add)
	{
		this.currentValue += add;
	}
	
	public void ResetValue()
	{
		this.currentValue = this.baseValue;
	}
	
	public void SetBaseValue(int val)
	{
		this.baseValue = val;
	}

	public void AddBaseValue(int add)
	{
		this.baseValue += add;
	}
	
	/*
	============================================================================
	Check functions
	============================================================================
	*/

	
	public bool IsNormal()
	{
		return StatusValueType.NORMAL.Equals(type);
	}
	
	public bool IsConsumable()
	{
		return StatusValueType.CONSUMABLE.Equals(type);
	}
	
	public bool IsExperience()
	{
		return StatusValueType.EXPERIENCE.Equals(type);
	}
	

}