
using UnityEngine;

[System.Serializable]
public class Item
{
	public int iD = 0;
	public int category = 0;
	public string iconUrl = "";
	public LanguageItem[] languageItem = new LanguageItem[0];

	public string prefabName = "";
	public ItemType itemType = ItemType.Diamond;
	public PriceType priceType = PriceType.Coin;
    public ItemState itemState = ItemState.OnShop;
	public int buyPrice = 0;
	public int sellPrice = 0;
	public bool consume = true;
	public ItemSkillType itemSkill = ItemSkillType.NONE;
	public int skillID = 0;

	public bool isAvailable = true;
	public int shopOrder = 0;


	public Item()
	{
		iD = DataHolder.LastItemID() + 1;
		languageItem = new LanguageItem[DataHolder.Languages ().GetDataCount ()];
		for (int i = 0; i < languageItem.Length; i++) {
			languageItem[i] = new LanguageItem();
		}
		languageItem[0].Name = "New Item";
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




