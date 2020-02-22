using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PetData : BaseData
{
	public Pet[] pets = new Pet[0];
	string fileName = "PetData";

	public PetData()
	{
		LoadData();
	}
	
	public void LoadData()
	{
		string text = LoadFile (fileName);
		if (text != "") {
			pets = JsonHelper.getJsonArray<Pet> (text);
		}

	}
	
	public void SaveData()
	{
		string text = "";
		text += "[";
		for (int i = 0; i < pets.Length; i++) {
			text +=  JsonUtility.ToJson(pets[i]).ToString(); 
			if(i < pets.Length - 1)
				text += ",";
		}
		text += "]";
		SaveFile (fileName, text);
	}

	public string GetPrefabPath() { return "Prefabs/Pets/"; }
	
	public void AddPet()
	{
		pets = ArrayHelper.Add(new Pet(), pets);
	}

	public void AddLanguage()
	{
		for(int i=0; i<pets.Length; i++)
		{
			pets [i].AddLanguageItem ();
		}
	}

	public void RemoveLanguage(int index)
	{
		for(int i=0; i<pets.Length; i++)
		{
			pets [i].RemoveLanguage (index);
		}
	}

	
	
	public override void RemoveData(int index)
	{
		base.RemoveData(index);
		pets = ArrayHelper.Remove(index, pets);
	}

	

	public override void Copy(int index)
	{
		base.Copy(index);
		pets = ArrayHelper.Add(new Pet(), pets);

		if (pets.Length < 2)
			return;

		pets [pets.Length - 1].languageItem = new LanguageItem[ pets [index].languageItem.Length];
		for (int i = 0; i < pets [index].languageItem.Length; i++) {
			pets [pets.Length - 1].languageItem[i].Description = pets [index].languageItem[i].Description;
			pets [pets.Length - 1].languageItem[i].Name = pets [index].languageItem[i].Name;
		}

	}

	public override int GetDataCount()
	{
		int val = 0;
		if(pets != null)
		{
			val = pets.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return pets[index].iD + " : " + pets[index].languageItem[0].Description;
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(pets != null)
		{
			result = new string[pets.Length];
			for(int i=0; i<pets.Length; i++)
			{
				if(showIDs)
				{
					result[i] = pets[i].iD.ToString() + ":" + pets[i].languageItem[0].Name;
				}
				else
				{
					result[i] =  pets[i].languageItem[0].Name;
				}
			}
		}
		return result;
	}




	public Pet GetPet(int id)
	{
		for(int i=0; i<pets.Length; i++)
		{
			if (pets[i].iD == id) {
				return pets[i];
			}
		}
		return null;
	}


    public int GetPetPosition(int id)
    {

        for (int i = 0; i < pets.Length; i++)
        {
            if (pets[i].iD == id)
            {
                return i;
            }
        }

        return -1;
    }



}