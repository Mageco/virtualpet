
using System.Collections;
using UnityEngine;

public class CategoryData : BaseData
{
	public string[] name;
	string fileName = "CategoryData";

	public CategoryData()
	{
		LoadData ();
	}


	public void SaveData()
	{
		string text = "";
		for (int i = 0; i < name.Length; i++) {
			text += name [i]; 
			if(i < name.Length - 1)
				text += "\n";
		}
		SaveFile (fileName, text);
	}

	public void LoadData()
	{
		name = new string[1];
		name[0] = "New Category";

		string text = LoadFile (fileName);
		if (text != "") {
			string[] lgs = text.Split ('\n');
			name = new string[lgs.Length];
			for (int i = 0; i < lgs.Length; i++) {
				name[i] = lgs [i];
			}
		}
	}

	public int GetCategory(string c)
	{
		for (int i = 0; i < name.Length; i++) {
			if (name [i] == c)
				return i;
		}
		return -1;
	}
	
	public void AddCategory(string n)
	{
		if(name == null)
		{
			name = new string[] {n};
		}
		else
		{
			name = ArrayHelper.Add(n, name);
		}
	}

	public override void RemoveData(int index)
	{
		name = ArrayHelper.Remove(index, name);
	}

	public override void Copy(int index)
	{
		this.AddCategory(name[index]);
	}
	
	public override int GetDataCount()
	{
		int val = 0;
		if(name != null)
		{
			val = name.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return name[index];
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(name != null)
		{
			result = new string[name.Length];
			for(int i=0; i<name.Length; i++)
			{
				if(showIDs)
				{
					result[i] = i.ToString() + ": " + name[i];
				}
				else
				{
					result[i] = name[i];
				}
			}
		}
		return result;
	}


}