﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEvent(){
        MageManager.instance.LoadSceneWithLoading("Minigame2");
    }

    public void Close(){
        this.GetComponent<Popup>().Close();
    }
}
