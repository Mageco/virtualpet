using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 public static GameManager instance;

    public ItemData[] datas;

	void Awake()
	{
		if (instance == null)
			instance = this;
        
        Load();
	}

    void Load(){
        for(int i=0;i<datas.Length;i++){
            datas[i].itemID = i;
        }
    }

}
