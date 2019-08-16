using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StatusValueData : BaseData
{
	public StatusValue[] statusValues = new StatusValue[0];
	string fileName = "StatusValuesData";

	public StatusValueData()
	{
		LoadData();
	}
	
	public void LoadData()
	{
		string text = LoadFile (fileName);
		if (text != "") {
			statusValues = JsonHelper.getJsonArray<StatusValue> (text);
		}

	}

	public void SaveData()
	{
		string text = "";
		text += "[";
		for (int i = 0; i < statusValues.Length; i++) {
			text +=  JsonUtility.ToJson(statusValues[i]).ToString(); 
			if(i < statusValues.Length - 1)
				text += ",";
		}
		text += "]";
		SaveFile (fileName, text);
	}
	
	public StatusValue GetStatusValueData(int index)
	{
		return statusValues[index];
	}
	
	public StatusValue[] GetStatusValuesData()
	{
		return statusValues;
	}

	public void AddLanguage()
	{
		for(int i=0; i<statusValues.Length; i++)
		{
			statusValues [i].AddLanguageItem ();
		}
	}

	public void RemoveLanguage(int index)
	{
		for(int i=0; i<statusValues.Length; i++)
		{
			statusValues [i].RemoveLanguage (index);
		}
	}
	
	public void AddValue()
	{
		statusValues = ArrayHelper.Add(new StatusValue(), statusValues);
		Debug.Log (statusValues.Length);
	}
	
	public override void RemoveData(int index)
	{
		base.RemoveData(index);
		statusValues = ArrayHelper.Remove(index, statusValues);
	}
	
	public override void Copy(int index)
	{
		base.Copy(index);
		
		statusValues = ArrayHelper.Add(new StatusValue(), statusValues);
		statusValues[statusValues.Length-1].iD = statusValues[index].iD;
		statusValues[statusValues.Length-1].minValue = statusValues[index].minValue;
		statusValues[statusValues.Length-1].maxValue = statusValues[index].maxValue;
		statusValues[statusValues.Length-1].type = statusValues[index].type;
		statusValues[statusValues.Length-1].maxStatus = statusValues[index].maxStatus;
		statusValues[statusValues.Length-1].killChar = statusValues[index].killChar;
		statusValues[statusValues.Length-1].levelUp = statusValues[index].levelUp;
	}
	
	public void RemoveStatusValue(int index)
	{
		for(int i=0; i<statusValues.Length; i++)
		{
			if(statusValues[i].maxStatus == index)
			{
				statusValues[i].maxStatus = 0;
			}
			else if(statusValues[i].maxStatus > index)
			{
				statusValues[i].maxStatus -= 1;
			}
		}
	}

	public override int GetDataCount()
	{
		int val = 0;
		if(statusValues != null)
		{
			val = statusValues.Length;
		}
		return val;
	}

	public override string GetName(int index)
	{
		return index.ToString () + " : " + statusValues[index].languageItem[0].Name;
	}

	public override string[] GetNameList(bool showIDs)
	{
		string[] result = new string[0];
		if(statusValues != null)
		{
			result = new string[statusValues.Length];
			for(int i=0; i<statusValues.Length; i++)
			{
				if(showIDs)
				{
					result[i] = i.ToString() + ":" + statusValues[i].languageItem[0].Name;
				}
				else
				{
					result[i] =  statusValues[i].languageItem[0].Name;
				}
			}
		}
		return result;
	}
}