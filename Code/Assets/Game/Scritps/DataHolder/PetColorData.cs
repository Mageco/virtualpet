using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PetColorData : BaseData
{
	public PetColor[] petColors = new PetColor[0];
	string fileName = "PetColorData";

	public PetColorData()
	{
		LoadData();
	}
	
	public void LoadData()
	{
		string text = LoadFile (fileName);
		if (text != "") {
			petColors = JsonHelper.getJsonArray<PetColor> (text);
		}

	}
	
	public void SaveData()
	{
		string text = "";
		text += "[";
		for (int i = 0; i < petColors.Length; i++) {
			text +=  JsonUtility.ToJson(petColors[i]).ToString(); 
			if(i < petColors.Length - 1)
				text += ",";
		}
		text += "]";
		SaveFile (fileName, text);
	}

	public string GetPrefabPath() { return "Prefabs/Pets/"; }
	
	public void AddPetColor()
	{
		petColors = ArrayHelper.Add(new PetColor(), petColors);
	}

	public void AddLanguage()
	{
		for(int i=0; i<petColors.Length; i++)
		{
			petColors [i].AddLanguageItem ();
		}
	}

	public void RemoveLanguage(int index)
	{
		for(int i=0; i<petColors.Length; i++)
		{
			petColors [i].RemoveLanguage (index);
		}
	}

	
	
	public override void RemoveData(int index)
	{
		base.RemoveData(index);
		petColors = ArrayHelper.Remove(index, petColors);
	}

	

	public override void Copy(int index)
	{
		base.Copy(index);
		petColors = ArrayHelper.Add(new PetColor(), petColors);

		if (petColors.Length < 2)
			return;

		petColors [petColors.Length - 1].languageItem = new LanguageItem[ petColors [index].languageItem.Length];
		for (int i = 0; i < petColors [index].languageItem.Length; i++) {
			petColors [petColors.Length - 1].languageItem[i].Description = petColors [index].languageItem[i].Description;
			petColors [petColors.Length - 1].languageItem[i].Name = petColors [index].languageItem[i].Name;
		}

	}

	public override int GetDataCount()
	{
		int val = 0;
		if(petColors != null)
		{
			val = petColors.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return petColors[index].iD + " : " + petColors[index].languageItem[0].Description;
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(petColors != null)
		{
			result = new string[petColors.Length];
			for(int i=0; i<petColors.Length; i++)
			{
				if(showIDs)
				{
					result[i] = i.ToString() + ":" + petColors[i].languageItem[0].Name;
				}
				else
				{
					result[i] =  petColors[i].languageItem[0].Name;
				}
			}
		}
		return result;
	}




	public PetColor GetPetColor(int id)
	{
		for(int i=0; i<petColors.Length; i++)
		{
			if (petColors[i].iD == id) {
				return petColors[i];
			}
		}
		return null;
	}


    public int GetPetColorPosition(int id)
    {

        for (int i = 0; i < petColors.Length; i++)
        {
            if (petColors[i].iD == id)
            {
                return i;
            }
        }

        return -1;
    }



}