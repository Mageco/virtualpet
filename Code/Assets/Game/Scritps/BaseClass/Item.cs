
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
	public bool isAvailable = true;
	public int shopOrder = 0;

	//All item properties
	public float value = 0; //Value can be food amount, cleaness 
	public float sleep = 0;
	public float energy = 0;
	public float health = 0;
	public float happy = 0;
	public float dirty = 0;
	public float itchi = 0;
	public float rateSleep = 0;
	public float ratePee = 0;
	public float rateShit = 0;
	public float rateEat = 0;
	public float rateDrink = 0;


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




