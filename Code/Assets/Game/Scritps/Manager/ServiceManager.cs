using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterService(ServiceType type)
    {
        foreach(PlayerService s in GameManager.instance.myPlayer.playerServices)
        {
            if(s.type == type)
            {

            }
        }
    }
}


