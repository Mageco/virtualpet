using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 public static GameManager instance;

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

}
