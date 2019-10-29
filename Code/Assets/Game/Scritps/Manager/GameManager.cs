using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 public static GameManager instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
        
        Application.targetFrameRate = 50;
        Load();
	}

    void Load(){
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		DataHolder.Instance();
		Debug.Log(DataHolder.Items().GetDataCount());
    }

}
