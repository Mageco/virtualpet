
using System;
using System.IO;
using MageApi;
using UnityEngine;

[System.Serializable]
public class BaseData
{
	string key = "lXkGhszNZZ5A08IjNTSml1hkAACju7deo6gg6Msf7aJIDQCQPFyCuSAHekvWDMSqvn5D16bUafqOOuBD43ZQG4rOmTt9RUyokepQKasG7rGP5Ahznq8s4numu4alFngUtr9Ie7H63XR6gvGKhs1V6RlVYAbMTfbSPlphCNA4yptBnIOs35O9iIf0pIRhzlDQCjCBYj7ojuTxXQWEdhCrVRXkcDTluTv8DbC0RAuSxJatBuAHxEXpQ5ECOWXMw7eC";
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
        text = ApiUtils.GetInstance().EncryptStringWithKey(text, key);
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
		//Debug.Log(jsonTextFile.text);
		string text = ApiUtils.GetInstance().DecryptStringWithKey(jsonTextFile.text, key);
		if (jsonTextFile != null)
			return text;
		else return "";
	}

	public string ToEncryptedJson(string key)
	{
		return ApiUtils.GetInstance().EncryptStringWithKey(JsonUtility.ToJson(this), key);
	}

	public static TResult CreatFromEncryptJson<TResult>(string encryptedString, string key) where TResult : BaseData
	{
		try
		{
			string jsonString = ApiUtils.GetInstance().DecryptStringWithKey(encryptedString, key);
			return JsonUtility.FromJson<TResult>(jsonString);
		}
		catch (Exception e)
		{
			return default(TResult);
		}
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