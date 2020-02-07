using Mage.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skin : BaseModel
{
    public string iconUrl = "";
    public LanguageItem[] languageItem = new LanguageItem[0];
    public PriceType priceType = PriceType.Coin;
    public int levelRequire = 0;
    public string prefabName = "";
    public int buyPrice = 0;
    public ItemState itemState = ItemState.OnShop;

	public Skin()
	{
		languageItem = new LanguageItem[DataHolder.Languages().GetDataCount()];
		for (int i = 0; i < languageItem.Length; i++)
		{
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
		this.languageItem = ArrayHelper.Remove(index, this.languageItem);
	}

	public string GetDescription(int languageID)
	{
		return languageItem[languageID].Description;
	}

	public void SetDescription(int languageID, string text)
	{
		if (this.languageItem.Length > languageID)
		{
			languageItem[languageID].Description = text;
		}
		else
		{
			LanguageItem item = new LanguageItem();
			item.Description = text;
			this.languageItem = ArrayHelper.Add(item, this.languageItem);
		}
	}

	public string GetName(int languageID)
	{
		return languageItem[languageID].Name;
	}

	public void SetName(int languageID, string text)
	{
		if (this.languageItem.Length > languageID)
		{
			languageItem[languageID].Name = text;
		}
		else
		{
			LanguageItem item = new LanguageItem();
			item.Name = text;
			this.languageItem = ArrayHelper.Add(item, this.languageItem);
		}
	}

}
