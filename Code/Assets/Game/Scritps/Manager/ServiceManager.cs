using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;
    float time;
    float maxTimeCheck = 1;
    public ServiceUI[] serviceUIs;
    public GameObject servicePanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (!GameManager.instance.isLoad)
        {
            yield return new WaitForEndOfFrame();
        }

        if(GameManager.instance.myPlayer.playerServices.Count == 0)
        {
            AddService();
        }

        LoadServiceUI();
    }

    void LoadServiceUI()
    {
        if (GameManager.instance.GetPets().Count >= 2)
        {
            servicePanel.SetActive(true);
            for (int i = 0; i < serviceUIs.Length; i++)
            {
                serviceUIs[i].Load();
            }
        }
        else
            servicePanel.SetActive(false);

    }

    void AddService()
    {
        for (int i = 0; i < 5; i++)
        {
            PlayerService s = new PlayerService();
            s.type = (ServiceType)i;
            s.isActive = false;
            GameManager.instance.myPlayer.playerServices.Add(s);
        }
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
        if (GameManager.instance.GetPets().Count >= 2)
        {
            servicePanel.SetActive(true);
            foreach (PlayerService s in GameManager.instance.myPlayer.playerServices)
            {
                if (s.isActive && (System.DateTime.Now - s.timeStart).TotalSeconds > 1800)
                {
                    s.StopService();
                }
            }
        }else
            servicePanel.SetActive(false);

    }

    public PlayerService GetService(ServiceType type)
    {
        foreach (PlayerService s in GameManager.instance.myPlayer.playerServices)
        {
            if (s.type == type)
            {
                return s;
            }
        }
        return null;
    }


    public void StartService(ServiceType type)
    {
        foreach(PlayerService s in GameManager.instance.myPlayer.playerServices)
        {
            if(s.type == type)
            {
                s.StartService();
                Debug.Log(s.type + " " + s.isActive);
            }
        }
        GameManager.instance.SavePlayer();
        this.LoadServiceUI();
    }
}


