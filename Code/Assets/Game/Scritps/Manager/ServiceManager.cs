using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;
    float time;
    float maxTimeCheck = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (time > maxTimeCheck)
        {
            CheckService();
            time = 0;
        }
        else
            time += Time.deltaTime;
    }

    void CheckService()
    {
        foreach (PlayerService s in GameManager.instance.myPlayer.playerServices)
        {
            if (s.isActive && (System.DateTime.Now - s.timeStart).TotalSeconds > 1800)
            {
                s.StopService();
            }
        }
    }

    public void StartService(ServiceType type)
    {
        foreach(PlayerService s in GameManager.instance.myPlayer.playerServices)
        {
            if(s.type == type)
            {
                s.StartService();
            }
        }
    }
}


