using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;
    float time;
    float maxTimeCheck = 1.2f;
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

        if(GameManager.instance.myPlayer.startGameTime == null || GameManager.instance.myPlayer.startGameTime == "")
        {
            GameManager.instance.myPlayer.startGameTime = System.DateTime.Now.ToString();

        }

        if(GameManager.instance.myPlayer.dailyBonus.Count == 0)
        {
            for (int i = 0; i < 7; i++)
            {
                PlayerBonus b = new PlayerBonus();
                GameManager.instance.myPlayer.dailyBonus.Add(b);
            }
        }

        LoadServiceUI();
    }

    void LoadServiceUI()
    {
        if (GameManager.instance.GetPets().Count >= 3)
        {
            servicePanel.SetActive(true);
            for (int i = 0; i < serviceUIs.Length; i++)
            {
                serviceUIs[i].Load();
            }
        }
        else if(GameManager.instance.myPlayer.level > 1)
        {
            servicePanel.SetActive(true);
            for (int i = 0; i < serviceUIs.Length; i++)
            {
                if (i < serviceUIs.Length - 1)
                    serviceUIs[i].gameObject.SetActive(false);
                else
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
            if (s.type == ServiceType.Instructor)
                s.duration = 600;
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
        LoadServiceUI();

        foreach (PlayerService s in GameManager.instance.myPlayer.playerServices)
        {
            if (s.isActive && (System.DateTime.Now - System.DateTime.Parse(s.timeStart)).TotalSeconds > s.duration)
            {
                s.StopService();
                if (s.type == ServiceType.Instructor)
                    GameManager.instance.expScale = 1;
                LoadServiceUI();
            }

            if (s.isActive)
            {
                if (s.type == ServiceType.Instructor)
                    GameManager.instance.expScale = 2;
                RunService(s.type);
            }
        }


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

    void RunService(ServiceType type)
    {
        if(type == ServiceType.Chef)
        {
            foreach(Pet p in GameManager.instance.GetPets())
            {
                if(p.Food < p.MaxFood * 0.3f)
                {
                    p.Food = p.MaxFood;
                    if(ItemManager.instance != null && p.character != null)
                           ItemManager.instance.SpawnHeart(p.rateHappy, p.character.transform.position);
                }

                if (p.Water < p.MaxWater * 0.3f)
                {
                    p.Water = p.MaxWater;
                    if (ItemManager.instance != null && p.character != null)
                        ItemManager.instance.SpawnHeart(p.rateHappy, p.character.transform.position);
                }


            }
        }else if(type == ServiceType.HouseKeeper)
        {
            ItemDirty[] dirties = GameObject.FindObjectsOfType<ItemDirty>();
            for(int i = 0; i < dirties.Length; i++)
            {
                dirties[i].OnClean(dirties[i].dirty);
            }
        }
        else if (type == ServiceType.PetSitter)
        {
            foreach (Pet p in GameManager.instance.GetPets())
            {
                if (p.Dirty > p.MaxDirty * 0.7f)
                {
                    p.Dirty = 0;
                    if (ItemManager.instance != null)
                        ItemManager.instance.SpawnHeart(p.rateHappy, p.character.transform.position);
                }

                if (p.Sleep < p.MaxSleep * 0.3f)
                {
                    p.Sleep = p.MaxSleep;
                    if (ItemManager.instance != null)
                        ItemManager.instance.SpawnHeart(p.rateHappy, p.character.transform.position);
                }
            }
        }
        else if (type == ServiceType.Doctor)
        {
            foreach (Pet p in GameManager.instance.GetPets())
            {
                if (p.Damage > p.MaxDamage * 0.7f)
                {
                    p.Damage = 0;
                }

                if (p.Health < p.MaxHealth * 0.3f)
                {
                    p.Health = p.MaxHealth;
                }
            }
        }
    }





}


