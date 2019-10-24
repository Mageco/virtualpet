using System.Collections;
using UnityEngine;

public class LanguageData : BaseData
{
	public int[] languageIDs;
	string fileName = "LanguageData";

	public LanguageData()
	{
		LoadData();
	}
	
	public void LoadData()
	{
		languageIDs = new int[1];
		languageIDs[0] = 0;

		string text = LoadFile (fileName);
		if (text != "") {
			string[] lgs = text.Split ('\n');
			languageIDs = new int[lgs.Length];
			for (int i = 0; i < lgs.Length; i++) {
				languageIDs[i] = int.Parse (lgs [i]);
			}
		}
	}
	
	public void SaveData()
	{
		string text = "";
		for (int i = 0; i < languageIDs.Length; i++) {
			text += languageIDs [i]; 
			if(i < languageIDs.Length - 1)
				text += "\n";
		}
		SaveFile (fileName, text);
	}
	
	public void AddLanguage(int id)
	{
		if(languageIDs == null)
		{
			languageIDs = new int[] {id};
		}
		else
		{
			languageIDs = ArrayHelper.Add(id, languageIDs);
		}
	}
	
	public override void RemoveData(int index)
	{
		languageIDs = ArrayHelper.Remove(index, languageIDs);
	}
	
	public override void Copy(int index)
	{
		this.AddLanguage(languageIDs[index]);
	}

	public override int GetDataCount()
	{
		int val = 0;
		if(languageIDs != null)
		{
			val = languageIDs.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return DataHolder.LanguageName[languageIDs[index]];
	}



	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(languageIDs != null)
		{
			result = new string[languageIDs.Length];
			for(int i=0; i<languageIDs.Length; i++)
			{
				if(showIDs)
				{
					result[i] = i.ToString() + ": " + DataHolder.LanguageName[languageIDs[i]];
				}
				else
				{
					result[i] = DataHolder.LanguageName[languageIDs[i]];
				}
			}
		}
		return result;
	}

}