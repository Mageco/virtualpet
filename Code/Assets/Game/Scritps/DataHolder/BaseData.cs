
using System.IO;
using UnityEngine;

[System.Serializable]
public class BaseData
{
	public BaseData()
	{
	}

	public virtual void RemoveData(int index)
	{

	}

	public virtual void Copy(int index)
	{

	}

	public void SaveFile(string fileName,string text)
	{
		string savePath = Application.dataPath + "/Game/Resources/Data/" + fileName + ".txt";
		if (!File.Exists (savePath)) {
			FileStream file = File.Open (savePath, FileMode.Create);
			file.Close ();
		}
		//File.AppendAllText
		File.Delete(savePath);
		File.WriteAllText(savePath,text);
	}

	public string LoadFile(string fileName)
	{
		var jsonTextFile = Resources.Load<TextAsset>("Data/" + fileName);
		if(jsonTextFile != null)
			return jsonTextFile.text;
		else return "";
	}

	public virtual int GetDataCount()
	{
		return 0;
	}

	public virtual string GetName(int index)
	{
		return "";
	}

	public virtual string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		return result;
	}

	public virtual string[] GetNameListFilter(int category){
		string[] result = new string[0];
		return result;
	}

	public virtual string[] GetDescriptionListFilter(int category,int language){
		string[] result = new string[0];
		return result;
	}

	public int CheckForIndex(int index, int check)
	{
		if(check == index) check = 0;
		else if(check > index) check -= 1;
		return check;
	}
}