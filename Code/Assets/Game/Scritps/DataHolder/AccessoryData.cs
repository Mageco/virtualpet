using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AccessoryData : BaseData
{
	public Accessory[] accessories = new Accessory[0];
	string fileName = "AccessoryData";

	public AccessoryData()
	{
		LoadData();
	}

	public void LoadData()
	{
		string text = LoadFile(fileName);
		if (text != "")
		{
			accessories = JsonHelper.getJsonArray<Accessory>(text);
		}
	}

	public void SaveData()
	{
		string text = "";
		text += "[";
		for (int i = 0; i < accessories.Length; i++)
		{
			text += JsonUtility.ToJson(accessories[i]).ToString();
			if (i < accessories.Length - 1)
				text += ",";
		}
		text += "]";
		SaveFile(fileName, text);
	}

	public string GetPrefabPath() { return "Prefabs/Accessorys/"; }

	public void AddAccessory()
	{
		accessories = ArrayHelper.Add(new Accessory(), accessories);
	}

	public void AddLanguage()
	{
		for (int i = 0; i < accessories.Length; i++)
		{
			accessories[i].AddLanguageItem();
		}
	}

	public void RemoveLanguage(int index)
	{
		for (int i = 0; i < accessories.Length; i++)
		{
			accessories[i].RemoveLanguage(index);
		}
	}



	public override void RemoveData(int index)
	{
		base.RemoveData(index);
		accessories = ArrayHelper.Remove(index, accessories);
	}



	public override void Copy(int index)
	{
		base.Copy(index);
		accessories = ArrayHelper.Add(new Accessory(), accessories);

		if (accessories.Length < 2)
			return;

		accessories[accessories.Length - 1].languageItem = new LanguageItem[accessories[index].languageItem.Length];
		for (int i = 0; i < accessories[index].languageItem.Length; i++)
		{
			accessories[accessories.Length - 1].languageItem[i].Description = accessories[index].languageItem[i].Description;
			accessories[accessories.Length - 1].languageItem[i].Name = accessories[index].languageItem[i].Name;
		}

	}

	public override int GetDataCount()
	{
		int val = 0;
		if (accessories != null)
		{
			val = accessories.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return accessories[index].iD + " : " + accessories[index].languageItem[0].Description;
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if (accessories != null)
		{
			result = new string[accessories.Length];
			for (int i = 0; i < accessories.Length; i++)
			{
				if (showIDs)
				{
					result[i] = accessories[i].iD.ToString() + ":" + accessories[i].GetName(0);
				}
				else
				{
					result[i] = accessories[i].languageItem[0].Name;
				}
			}
		}
		return result;
	}




	public Accessory GetAccessory(int id)
	{
		for (int i = 0; i < accessories.Length; i++)
		{
			if (accessories[i].iD == id)
			{
				return accessories[i];
			}
		}
		return null;
	}


	public int GetAccessoryPosition(int id)
	{

		for (int i = 0; i < accessories.Length; i++)
		{
			if (accessories[i].iD == id)
			{
				return i;
			}
		}

		return -1;
	}



}