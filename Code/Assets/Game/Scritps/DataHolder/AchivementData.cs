using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AchivementData : BaseData
{
	public Achivement[] achivements = new Achivement[0];
	string fileName = "AchivementData";

	public AchivementData()
	{
		LoadData();
	}
	
	public void LoadData()
	{
		string text = LoadFile (fileName);
		if (text != "") {
			achivements = JsonHelper.getJsonArray<Achivement> (text);
		}

	}
	
	public void SaveData()
	{
		string text = "";
		text += "[";
		for (int i = 0; i < achivements.Length; i++) {
			text +=  JsonUtility.ToJson(achivements[i]).ToString(); 
			if(i < achivements.Length - 1)
				text += ",";
		}
		text += "]";
		SaveFile (fileName, text);
	}

	public string GetPrefabPath() { return "Prefabs/achivements/"; }
	
	public void AddAchivement()
	{
		achivements = ArrayHelper.Add(new Achivement(), achivements);
	}
	
	public override void RemoveData(int index)
	{
		base.RemoveData(index);
		achivements = ArrayHelper.Remove(index, achivements);
	}

	public void AddLanguage()
	{
		for(int i=0; i<achivements.Length; i++)
		{
			achivements [i].AddLanguageItem ();
		}
	}

	public void RemoveLanguage(int index)
	{
		for(int i=0; i<achivements.Length; i++)
		{
			achivements [i].RemoveLanguage (index);
		}
	}


	public override void Copy(int index)
	{
		base.Copy(index);
		achivements = ArrayHelper.Add(new Achivement(), achivements);

		if (achivements.Length < 2)
			return;

		achivements[achivements.Length-1].category = achivements[index].category;
		achivements [achivements.Length - 1].languageItem = new LanguageItem[ achivements [index].languageItem.Length];
		for (int i = 0; i < achivements [index].languageItem.Length; i++) {
			achivements [achivements.Length - 1].languageItem[i].Description = achivements [index].languageItem[i].Description;
			achivements [achivements.Length - 1].languageItem[i].Name = achivements [index].languageItem[i].Name;
		}

	}

	public override int GetDataCount()
	{
		int val = 0;
		if(achivements != null)
		{
			val = achivements.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return achivements[index].iD + " : " + achivements[index].languageItem[0].Description;
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(achivements != null)
		{
			result = new string[achivements.Length];
			for(int i=0; i<achivements.Length; i++)
			{
				if(showIDs)
				{
					//Debug.Log(achivements[i].actionType);
					result[i] = achivements[i].iD.ToString() + ":" + achivements[i].languageItem[0].Name;
				}
				else
				{
					result[i] =  achivements[i].languageItem[0].Name;
				}
			}
		}
		return result;
	}

	public override string[] GetNameListFilter(int category)
	{
		List<string> result = new List<string>();
		if(achivements != null)
		{
			for(int i=0; i<achivements.Length; i++)
			{
				if (category == 0 || achivements [i].category == category) {
					string text = achivements[i].iD.ToString() + " : " + achivements [i].GetName(0); 
					result.Add (text);
				}
			}
		}
		return result.ToArray();
	}

	public override string[] GetDescriptionListFilter(int category,int language)
	{
		List<string> result = new List<string>();
		if(achivements != null)
		{
			for(int i=0; i<achivements.Length; i++)
			{
				if (category == 0 || achivements [i].category == category) {
					string text = achivements [i].iD.ToString(); 
					result.Add (text);
				}
			}
		}
		return result.ToArray();
	}


	public Achivement GetAchivement(int id)
	{
		for(int i=0; i<achivements.Length; i++)
		{
			if (achivements[i].iD == id) {
				return achivements[i];
			}
		}
		return null;
	}




	public Achivement GetAchivement(int id,int category)
	{
		List<Achivement> result = new List<Achivement>();
		if(achivements != null)
		{
			for(int i=0; i<achivements.Length; i++)
			{
				if (category == 0 || achivements [i].category == category) {
					result.Add (achivements[i]);
				}
			}
		}
		if (id < result.Count)
			return result [id];
		else
			return null;
	}

	public Achivement[] GetAchivements(int category)
	{
		List<Achivement> result = new List<Achivement>();
		if(achivements != null)
		{
			for(int i=0; i<achivements.Length; i++)
			{
				if (category == 0 || achivements [i].category == category) {
					result.Add (achivements[i]);
				}
			}
		}

		return result.ToArray();
	}



	public int GetAchivementOrder(int id,int category)
	{
		List<int> result = new List<int>();
		if(achivements != null)
		{
			for(int i=0; i<achivements.Length; i++)
			{
				if (category == 0 || achivements [i].category == category){
					result.Add (i);
				}
			}
		}
		if (id < result.Count)
			return result [id];
		else
			return 0;
	}

	public int[] GetAchivementOrders(int category)
	{
		List<int> result = new List<int>();
		if(achivements != null)
		{
			for(int i=0; i<achivements.Length; i++)
			{
				if (category == 0 || achivements [i].category == category) {
					result.Add (i);
				}
			}
		}
			
		return result.ToArray();
	}

}