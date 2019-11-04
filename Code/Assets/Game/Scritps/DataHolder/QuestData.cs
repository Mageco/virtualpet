using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestData : BaseData
{
	public Quest[] quests = new Quest[0];
	string fileName = "QuestData";

	public QuestData()
	{
		LoadData();
	}
	
	public void LoadData()
	{
		string text = LoadFile (fileName);
		if (text != "") {
			quests = JsonHelper.getJsonArray<Quest> (text);
		}

	}
	
	public void SaveData()
	{
		string text = "";
		text += "[";
		for (int i = 0; i < quests.Length; i++) {
			text +=  JsonUtility.ToJson(quests[i]).ToString(); 
			if(i < quests.Length - 1)
				text += ",";
		}
		text += "]";
		SaveFile (fileName, text);
	}

	
	public void AddQuest()
	{
		quests = ArrayHelper.Add(new Quest(), quests);
	}

	public void AddLanguage()
	{
		for(int i=0; i<quests.Length; i++)
		{
			quests [i].AddLanguageItem ();
		}
	}

	public void RemoveLanguage(int index)
	{
		for(int i=0; i<quests.Length; i++)
		{
			quests [i].RemoveLanguage (index);
		}
	}

	
	
	public override void RemoveData(int index)
	{
		base.RemoveData(index);
		quests = ArrayHelper.Remove(index, quests);
	}
	

	

	public override void Copy(int index)
	{
		base.Copy(index);
		quests = ArrayHelper.Add(new Quest(), quests);

		if (quests.Length < 2)
			return;

		quests[quests.Length-1].category = quests[index].category;
		quests [quests.Length - 1].languageItem = new LanguageItem[ quests [index].languageItem.Length];
		for (int i = 0; i < quests [index].languageItem.Length; i++) {
			quests [quests.Length - 1].languageItem[i].Description = quests [index].languageItem[i].Description;
			quests [quests.Length - 1].languageItem[i].Name = quests [index].languageItem[i].Name;
		}

	}

	public override int GetDataCount()
	{
		int val = 0;
		if(quests != null)
		{
			val = quests.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return quests[index].iD + " : " + quests[index].languageItem[0].Description;
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(quests != null)
		{
			result = new string[quests.Length];
			for(int i=0; i<quests.Length; i++)
			{
				if(showIDs)
				{
					result[i] = i.ToString() + ":" + quests[i].languageItem[0].Name;
				}
				else
				{
					result[i] =  quests[i].languageItem[0].Name;
				}
			}
		}
		return result;
	}

	public override string[] GetNameListFilter(int category)
	{
		List<string> result = new List<string>();
		if(quests != null)
		{
			for(int i=0; i<quests.Length; i++)
			{
				if (category == 0 || quests [i].category == category) {
					string text = quests[i].iD.ToString() + " : " + quests [i].GetName(0); 
					result.Add (text);
				}
			}
		}
		return result.ToArray();
	}

	public override string[] GetDescriptionListFilter(int category,int language)
	{
		List<string> result = new List<string>();
		if(quests != null)
		{
			for(int i=0; i<quests.Length; i++)
			{
				if (category == 0 || quests [i].category == category) {
					string text = quests [i].iD.ToString(); 
					result.Add (text);
				}
			}
		}
		return result.ToArray();
	}


	public Quest GetQuest(int id)
	{
		for(int i=0; i<quests.Length; i++)
		{
			if (quests[i].iD == id) {
				return quests[i];
			}
		}
		return null;
	}




	public Quest GetQuest(int id,int category)
	{
		List<Quest> result = new List<Quest>();
		if(quests != null)
		{
			for(int i=0; i<quests.Length; i++)
			{
				if (category == 0 || quests [i].category == category) {
					result.Add (quests[i]);
				}
			}
		}
		if (id < result.Count)
			return result [id];
		else
			return null;
	}

	public Quest[] Getquests(int category)
	{
		List<Quest> result = new List<Quest>();
		if(quests != null)
		{
			for(int i=0; i<quests.Length; i++)
			{
				if (category == 0 || quests [i].category == category) {
					result.Add (quests[i]);
				}
			}
		}

		return result.ToArray();
	}



	public int GetQuestOrder(int id,int category)
	{
		List<int> result = new List<int>();
		if(quests != null)
		{
			for(int i=0; i<quests.Length; i++)
			{
				if (category == 0 || quests [i].category == category){
					result.Add (i);
				}
			}
		}
		if (id < result.Count)
			return result [id];
		else
			return 0;
	}

	public int[] GetQuestOrders(int category)
	{
		List<int> result = new List<int>();
		if(quests != null)
		{
			for(int i=0; i<quests.Length; i++)
			{
				if (category == 0 || quests [i].category == category) {
					result.Add (i);
				}
			}
		}
			
		return result.ToArray();
	}

}