using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 	public static GameManager instance;

	private Hashtable variables;
	public List<ActionData> actions = new List<ActionData>();
	public float gameTime = 0;
	void Awake()
	{
		if (instance == null)
			instance = this;
        
        Application.targetFrameRate = 50;
        Load();
	}

	void Update(){
		gameTime += Time.deltaTime;
	}

    void Load(){
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		DataHolder.Instance();
		DataHolder.Instance().Init();
		//Debug.Log(DataHolder.Items().GetDataCount());
    }

	public void LogAction(ActionType t){
		ActionData a = new ActionData();
		a.actionType = t;
		a.startTime = System.DateTime.Now;
		actions.Add(a);
		Debug.Log(a.actionType + "  " + a.startTime.ToShortTimeString());
		SaveAction();
	}

	void SaveAction(){
		ES2.Save(actions,"ActionLog");
	}

	void LoadAction(){
		if(ES2.Exists("ActionLog")){
			ES2.LoadList<ActionData>("ActionLog");
		}
	}

	public void SaveVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();
		foreach (DictionaryEntry entry in variables) {
			keys.Add ((string)entry.Key);
			values.Add ((string)entry.Value);
		}
		ES2.Save(keys, "variablesKey");
		ES2.Save(values, "variablesValue");

	}

	public void LoadVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();

		if(ES2.Exists("variablesKey"))
			keys = ES2.LoadList<string>("variablesKey");
		if(ES2.Exists("variablesValue"))
			values = ES2.LoadList<string>("variablesValue");

		for (int i = 0; i < keys.Count;i++) {
			if(i < values.Count)
				variables.Add (keys [i], values [i]);

			Debug.Log ("Load Variables " + keys [i] + " " + values [i]);
		}
	}
}
