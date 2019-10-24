using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SkillData : BaseData
{
	public Skill[] skills = new Skill[0];
	string fileName = "SkillData";

	public SkillData()
	{
		LoadData();
	}
	
	public void LoadData()
	{
		string text = LoadFile (fileName);
		if (text != "") {
			skills = JsonHelper.getJsonArray<Skill> (text);
		}

	}
	
	public void SaveData()
	{
		string text = "";
		text += "[";
		for (int i = 0; i < skills.Length; i++) {
			text +=  JsonUtility.ToJson(skills[i]).ToString(); 
			if(i < skills.Length - 1)
				text += ",";
		}
		text += "]";
		SaveFile (fileName, text);
	}

	public string GetPrefabPath() { return "Prefabs/skills/"; }
	
	public void AddSkill()
	{
		skills = ArrayHelper.Add(new Skill(), skills);
	}
	
	public override void RemoveData(int index)
	{
		base.RemoveData(index);
		skills = ArrayHelper.Remove(index, skills);
	}

	public void AddLanguage()
	{
		for(int i=0; i<skills.Length; i++)
		{
			skills [i].AddLanguageItem ();
		}
	}

	public void RemoveLanguage(int index)
	{
		for(int i=0; i<skills.Length; i++)
		{
			skills [i].RemoveLanguage (index);
		}
	}


	public override void Copy(int index)
	{
		base.Copy(index);
		skills = ArrayHelper.Add(new Skill(), skills);

		if (skills.Length < 2)
			return;

		skills[skills.Length-1].category = skills[index].category;
		skills [skills.Length - 1].languageItem = new LanguageItem[ skills [index].languageItem.Length];
		for (int i = 0; i < skills [index].languageItem.Length; i++) {
			skills [skills.Length - 1].languageItem[i].Description = skills [index].languageItem[i].Description;
			skills [skills.Length - 1].languageItem[i].Name = skills [index].languageItem[i].Name;
		}

	}

	public override int GetDataCount()
	{
		int val = 0;
		if(skills != null)
		{
			val = skills.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return skills[index].iD + " : " + skills[index].languageItem[0].Description;
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(skills != null)
		{
			result = new string[skills.Length];
			for(int i=0; i<skills.Length; i++)
			{
				if(showIDs)
				{
					result[i] = i.ToString() + ":" + skills[i].languageItem[0].Name;
				}
				else
				{
					result[i] =  skills[i].languageItem[0].Name;
				}
			}
		}
		return result;
	}

	public override string[] GetNameListFilter(int category)
	{
		List<string> result = new List<string>();
		if(skills != null)
		{
			for(int i=0; i<skills.Length; i++)
			{
				if (category == 0 || skills [i].category == category) {
					string text = skills[i].iD.ToString() + " : " + skills [i].GetName(0); 
					result.Add (text);
				}
			}
		}
		return result.ToArray();
	}

	public override string[] GetDescriptionListFilter(int category,int language)
	{
		List<string> result = new List<string>();
		if(skills != null)
		{
			for(int i=0; i<skills.Length; i++)
			{
				if (category == 0 || skills [i].category == category) {
					string text = skills [i].iD.ToString(); 
					result.Add (text);
				}
			}
		}
		return result.ToArray();
	}


	public Skill GetSkill(int id)
	{
		for(int i=0; i<skills.Length; i++)
		{
			if (skills[i].iD == id) {
				return skills[i];
			}
		}
		return null;
	}




	public Skill GetSkill(int id,int category)
	{
		List<Skill> result = new List<Skill>();
		if(skills != null)
		{
			for(int i=0; i<skills.Length; i++)
			{
				if (category == 0 || skills [i].category == category) {
					result.Add (skills[i]);
				}
			}
		}
		if (id < result.Count)
			return result [id];
		else
			return null;
	}

	public Skill[] Getskills(int category)
	{
		List<Skill> result = new List<Skill>();
		if(skills != null)
		{
			for(int i=0; i<skills.Length; i++)
			{
				if (category == 0 || skills [i].category == category) {
					result.Add (skills[i]);
				}
			}
		}

		return result.ToArray();
	}



	public int GetSkillOrder(int id,int category)
	{
		List<int> result = new List<int>();
		if(skills != null)
		{
			for(int i=0; i<skills.Length; i++)
			{
				if (category == 0 || skills [i].category == category){
					result.Add (i);
				}
			}
		}
		if (id < result.Count)
			return result [id];
		else
			return 0;
	}

	public int[] GetSkillOrders(int category)
	{
		List<int> result = new List<int>();
		if(skills != null)
		{
			for(int i=0; i<skills.Length; i++)
			{
				if (category == 0 || skills [i].category == category) {
					result.Add (i);
				}
			}
		}
			
		return result.ToArray();
	}

}