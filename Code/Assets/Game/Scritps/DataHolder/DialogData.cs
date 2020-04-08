using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogData : BaseData
{
	public Dialog[] dialogs = new Dialog[0];
	string fileName = "DialogData";

	public DialogData()
	{
		LoadData();
	}
	
	public void LoadData()
	{
		string text = LoadFile (fileName);
		if (text != "") {
			dialogs = JsonHelper.getJsonArray<Dialog> (text);
			Debug.Log(dialogs[0]);
		}

	}
	
	public void SaveData()
	{
		string text = "";
		text += "[";
		for (int i = 0; i < dialogs.Length; i++) {
			text +=  JsonUtility.ToJson(dialogs[i]).ToString(); 
			if(i < dialogs.Length - 1)
				text += ",";
		}
		text += "]";
		SaveFile (fileName, text);
	}

	public void ImportData()
	{
    #if UNITY_EDITOR
		string loadPath = EditorUtility.OpenFilePanel("Load import file", Application.dataPath + "/Data/ImportData/", "csv,txt");
		string text = File.ReadAllText(loadPath);
		string[] lines = text.Split('\n');
		int count = 0;
		Debug.Log(SplitCsvLine(lines[0]).Length);
		for (int i = 1; i < lines.Length; i++)
		{
			string[] fields = SplitCsvLine(lines[i]);
			bool isExist = false;
			Debug.Log(fields[0]);
			for (int j = 0; j < dialogs.Length; j++)
			{

				if (dialogs[j].iD.ToString() == fields[0])
				{

					int n = DataHolder.Languages().GetDataCount();
					for (int k = 0; k < n; k++)
					{
						Debug.Log(k);
						dialogs[j].SetDescription(k, fields[1 + k]);
					}
					isExist = true;
					break;
				}
			}

			if (!isExist)
			{
				//	int n = fields.Length - 8;
				Dialog item = new Dialog();
				for (int j = 0; j < DataHolder.Languages().GetDataCount(); j++)
				{
					item.AddLanguageItem();
					item.SetName(j, fields[1 + j]);
                    
				}
				this.dialogs = ArrayHelper.Add(item, dialogs);
				count++;
			}
		}


		SaveData();
		EditorUtility.DisplayDialog("Import", "Import Items Completed, There are " + count.ToString() + " new item added", "Ok");
#endif
	}

	public string[] SplitCsvLine(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
			@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
			System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
				select m.Groups[1].Value).ToArray();
	}

	public void AddDialog()
	{
		dialogs = ArrayHelper.Add(new Dialog(), dialogs);
	}

	public void AddLanguage()
	{
		for(int i=0; i<dialogs.Length; i++)
		{
			dialogs [i].AddLanguageItem ();
		}
	}

	public void RemoveLanguage(int index)
	{
		for(int i=0; i<dialogs.Length; i++)
		{
			dialogs [i].RemoveLanguage (index);
		}
	}

	
	
	public override void RemoveData(int index)
	{
		base.RemoveData(index);
		dialogs = ArrayHelper.Remove(index, dialogs);
	}

	

	public override void Copy(int index)
	{
		base.Copy(index);
		dialogs = ArrayHelper.Add(new Dialog(), dialogs);

		if (dialogs.Length < 2)
			return;

		dialogs [dialogs.Length - 1].languageItem = new LanguageItem[ dialogs [index].languageItem.Length];
		for (int i = 0; i < dialogs [index].languageItem.Length; i++) {
			dialogs [dialogs.Length - 1].languageItem[i].Description = dialogs [index].languageItem[i].Description;
			dialogs [dialogs.Length - 1].languageItem[i].Name = dialogs [index].languageItem[i].Name;
		}

	}

	public override int GetDataCount()
	{
		int val = 0;
		if(dialogs != null)
		{
			val = dialogs.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return dialogs[index].iD + " : " + dialogs[index].languageItem[0].Description;
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(dialogs != null)
		{
			result = new string[dialogs.Length];
			for(int i=0; i<dialogs.Length; i++)
			{
				if(showIDs)
				{
					result[i] = i.ToString() + ":" + dialogs[i].languageItem[0].Name;
				}
				else
				{
					result[i] =  dialogs[i].languageItem[0].Name;
				}
			}
		}
		return result;
	}

	public int GetDialogPosition(int id)
	{

		for (int i = 0; i < dialogs.Length; i++)
		{
			if (dialogs[i].iD == id)
			{
				return i;
			}
		}

		return -1;
	}

	public Dialog GetDialog(int id)
	{
		for(int i=0; i<dialogs.Length; i++)
		{
			if (dialogs[i].iD == id) {
				return dialogs[i];
			}
		}
		return null;
	}

}