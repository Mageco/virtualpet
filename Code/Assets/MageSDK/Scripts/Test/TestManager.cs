using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static TestManager instance;
    public Dictionary<string, object> taskList = new Dictionary<string, object>();
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {

    }

    public void ShowVideoAd()
    {
        
    }

}
