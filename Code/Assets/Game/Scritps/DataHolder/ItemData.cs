using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemData : BaseData
{
	public Item[] items = new Item[0];
	string fileName = "ItemData";

	public ItemData()
	{
		LoadData();
	}
	
	public void LoadData()
	{
		string text = LoadFile (fileName);
		if (text != "") {
			items = JsonHelper.getJsonArray<Item> (text);
		}

	}
	
	public void SaveData()
	{
		string text = "";
		text += "[";
		for (int i = 0; i < items.Length; i++) {
			text +=  JsonUtility.ToJson(items[i]).ToString(); 
			if(i < items.Length - 1)
				text += ",";
		}
		text += "]";
		SaveFile (fileName, text);
	}

	public string GetPrefabPath() { return "Prefabs/Items/"; }
	
	public void AddItem()
	{
		items = ArrayHelper.Add(new Item(), items);
	}

	public void AddLanguage()
	{
		for(int i=0; i<items.Length; i++)
		{
			items [i].AddLanguageItem ();
		}
	}

	public void RemoveLanguage(int index)
	{
		for(int i=0; i<items.Length; i++)
		{
			items [i].RemoveLanguage (index);
		}
	}

	
	
	public override void RemoveData(int index)
	{
		base.RemoveData(index);
		items = ArrayHelper.Remove(index, items);
	}

	

	public override void Copy(int index)
	{
		base.Copy(index);
		items = ArrayHelper.Add(new Item(), items);

		if (items.Length < 2)
			return;

		items[items.Length-1].category = items[index].category;
		items [items.Length - 1].languageItem = new LanguageItem[ items [index].languageItem.Length];
		for (int i = 0; i < items [index].languageItem.Length; i++) {
			items [items.Length - 1].languageItem[i].Description = items [index].languageItem[i].Description;
			items [items.Length - 1].languageItem[i].Name = items [index].languageItem[i].Name;
		}

	}

	public override int GetDataCount()
	{
		int val = 0;
		if(items != null)
		{
			val = items.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return items[index].iD + " : " + items[index].languageItem[0].Description;
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(items != null)
		{
			result = new string[items.Length];
			for(int i=0; i<items.Length; i++)
			{
				if(showIDs)
				{
					result[i] = i.ToString() + ":" + items[i].languageItem[0].Name;
				}
				else
				{
					result[i] =  items[i].languageItem[0].Name;
				}
			}
		}
		return result;
	}

	public override string[] GetNameListFilter(int category)
	{
		List<string> result = new List<string>();
		if(items != null)
		{
			for(int i=0; i<items.Length; i++)
			{
				if (category == 0 || items [i].category == category) {
					string text = items[i].iD.ToString() + " : " + items [i].GetName(0); 
					result.Add (text);
				}
			}
		}
		return result.ToArray();
	}

	public override string[] GetDescriptionListFilter(int category,int language)
	{
		List<string> result = new List<string>();
		if(items != null)
		{
			for(int i=0; i<items.Length; i++)
			{
				if (category == 0 || items [i].category == category) {
					string text = items [i].iD.ToString(); 
					result.Add (text);
				}
			}
		}
		return result.ToArray();
	}


	public Item GetItem(int id)
	{
		for(int i=0; i<items.Length; i++)
		{
			if (items[i].iD == id) {
				return items[i];
			}
		}
		return null;
	}




	public Item GetItem(int id,int category)
	{
		List<Item> result = new List<Item>();
		if(items != null)
		{
			for(int i=0; i<items.Length; i++)
			{
				if (category == 0 || items [i].category == category) {
					result.Add (items[i]);
				}
			}
		}
		if (id < result.Count)
			return result [id];
		else
			return null;
	}

	public Item[] GetItems(int category)
	{
		List<Item> result = new List<Item>();
		if(items != null)
		{
			for(int i=0; i<items.Length; i++)
			{
				if (category == 0 || items [i].category == category) {
					result.Add (items[i]);
				}
			}
		}

		return result.ToArray();
	}



	public int GetItemOrder(int id,int category)
	{
		List<int> result = new List<int>();
		if(items != null)
		{
			for(int i=0; i<items.Length; i++)
			{
				if (category == 0 || items [i].category == category){
					result.Add (i);
				}
			}
		}
		if (id < result.Count)
			return result [id];
		else
			return 0;
	}

	public int[] GetItemOrders(int category)
	{
		List<int> result = new List<int>();
		if(items != null)
		{
			for(int i=0; i<items.Length; i++)
			{
				if (category == 0 || items [i].category == category) {
					result.Add (i);
				}
			}
		}
			
		return result.ToArray();
	}

}