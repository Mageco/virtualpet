
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler
{
	#region Init
	private static GameHandler instance;
	private bool pauseGame = false;

	// ingame
	private float gameTime = 0;
	private bool isLock = false;


	// handlers
	private Hashtable variables;
	private Hashtable numberVariables;

	private GameHandler()
	{
		if(instance != null)
		{
			Debug.Log("There is already an instance of GameHandler!");
			return;
		}
		instance = this;
		this.Init();
	}
	
	public void Init()
	{
		this.ClearData();
		LoadVariables();
		LoadNumberVariables ();
	}
	
	public static GameHandler Instance()
	{
		if(instance == null)
		{
			new GameHandler();
		}
		return instance;
	}
	
	public void ClearData()
	{
		this.variables = new Hashtable();
		this.numberVariables = new Hashtable();
	}

	#endregion

	#region check lock
	public static bool IsLock()
	{
		return GameHandler.Instance ().isLock;
	}

	public static void SetLock(bool l)
	{
		GameHandler.instance.isLock = l;
	}


	#endregion


	
	#region Time functions
	public static void PauseGame(bool pause)
	{
		GameHandler.Instance().pauseGame = pause;
	}

	public static bool IsGamePaused()
	{
		return GameHandler.Instance().pauseGame;
	}

	public static float GetGameTime()
	{
		return GameHandler.Instance().gameTime;
	}
	
	public static void SetGameTime(float t)
	{
		GameHandler.Instance().gameTime = t;
	}
	
	public static void AddGameTime(float t)
	{
		GameHandler.Instance().gameTime += t;
	}
	
	public static string GetTimeString()
	{
		return GameHandler.GetTimeString(GameHandler.GetGameTime());
	}
	
	public static string GetTimeString(float t)
	{
		int h = (int)(t / 3600);
		t -= h * 3600;
		int m = (int)(t / 60);
		t -= m * 60;
		int s = (int)t;
		string time = h.ToString() + ":";
		if(m < 10) time += "0";
		time += m.ToString() + ":";
		if(s < 10) time += "0";
		time += s.ToString();
		return time;
	}
	#endregion

	#region Hashtable handling
	private static int GetCount(int id , Hashtable ht)
	{
		int count = 0;
		if(ht.ContainsKey(id))
		{
			count = (int)ht[id];
		}
		return count;
	}
	
	public static int AddCount(int id, int n, Hashtable ht)
	{
		int count = GameHandler.GetCount(id, ht);
		if(ht.ContainsKey(id))
		{
			ht.Remove(id);
		}
		count += n;
		ht.Add(id, count);
		return count;
	}
	
	public static int RemoveCount(int id, int n, Hashtable ht)
	{
		int count = GameHandler.GetCount(id, ht);
		if(ht.ContainsKey(id))
		{
			ht.Remove(id);
		}
		count -= n;
		if(count > 0)
		{
			ht.Add(id, count);
		}
		return count;
	}
	#endregion
	
	#region Variable handling
	public static string GetVariable(string key)
	{
		return GameHandler.Instance().variables[key] as string;
	}
	
	public static void SetVariable(string key, string value)
	{
		if(GameHandler.Instance().variables.ContainsKey(key))
		{
			GameHandler.Instance().variables[key] = value;
		}
		else
		{
			GameHandler.Instance().variables.Add(key, value);
		}

		Debug.Log ("Set Variable");

		SaveVariables();
	}
	
	public static void RemoveVariable(string key)
	{
		GameHandler.Instance().variables.Remove(key);
		SaveVariables();
	}
	
	public static bool CheckVariable(string key, string value)
	{
		bool check = false;
		if(GameHandler.Instance().variables.ContainsKey(key) && 
			GameHandler.Instance().variables[key] as string == value)
		{
			check = true;
		}
		return check;
	}
	#endregion

	#region Number variable handling
	public static float GetNumberVariable(string key)
	{
		float value = 0;
		if(GameHandler.Instance().numberVariables.ContainsKey(key))
		{
			value = (float)GameHandler.Instance().numberVariables[key];
		}
		return value;
	}
	
	public static void SetNumberVariable(string key, float value)
	{
		if(GameHandler.Instance().numberVariables.ContainsKey(key))
		{
			GameHandler.Instance().numberVariables[key] = value;
		}
		else
		{
			GameHandler.Instance().numberVariables.Add(key, value);
		}
		SaveNumberVariables ();
	}
	
	public static void RemoveNumberVariable(string key)
	{
		GameHandler.Instance().numberVariables.Remove(key);
		SaveNumberVariables ();
	}
	
	public static bool CheckNumberVariable(string key, float value, ValueCheck type)
	{
		bool check = false;
		if(GameHandler.Instance().numberVariables.ContainsKey(key) &&
			((ValueCheck.EQUALS.Equals(type) && (float)GameHandler.Instance().numberVariables[key] == value) ||
			(ValueCheck.LESS.Equals(type) && (float)GameHandler.Instance().numberVariables[key] < value) ||
			(ValueCheck.GREATER.Equals(type) && (float)GameHandler.Instance().numberVariables[key] > value)))
		{
			check = true;
		}
		return check;
	}
	#endregion


	#region Save Load	

	public static void SaveVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();
		foreach (DictionaryEntry entry in GameHandler.Instance().variables) {
			keys.Add ((string)entry.Key);
			values.Add ((string)entry.Value);
		}
		ES2.Save(keys, "variablesKey");
		ES2.Save(values, "variablesValue");

	}

	public static void LoadVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();

		if(ES2.Exists("variablesKey"))
			keys = ES2.LoadList<string>("variablesKey");
		if(ES2.Exists("variablesValue"))
			values = ES2.LoadList<string>("variablesValue");

		for (int i = 0; i < keys.Count;i++) {
			if(i < values.Count)
				GameHandler.Instance ().variables.Add (keys [i], values [i]);

			Debug.Log ("Load Variables " + keys [i] + " " + values [i]);
		}

	}

	public static void SaveNumberVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();
		foreach (DictionaryEntry entry in GameHandler.Instance().numberVariables) {
			keys.Add ((string)entry.Key);
			values.Add ((string)entry.Value);
		}
		ES2.Save(keys, "numberVariablesKey");
		ES2.Save(values, "numberVariablesValue");

	}

	public static void LoadNumberVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();

		if(ES2.Exists("numberVariablesKey"))
			keys = ES2.LoadList<string>("numberVariablesKey");
		if(ES2.Exists("numberVariablesValue"))
			values = ES2.LoadList<string>("numberVariablesValue");

		for (int i = 0; i < keys.Count;i++) {
			if(i < values.Count)
				GameHandler.Instance ().numberVariables.Add (keys [i], values [i]);

			Debug.Log ("Load Number Variables" + keys [i] + " " + values [i]);
		}

	}

	#endregion
}