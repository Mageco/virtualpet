using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusPanel : MonoBehaviour
{

    public Button[] collectButtons;
    public GameObject[] covers;
    public GameObject[] dones;
    public Text[] dayText;


    // Start is called before the first frame update
    void Start()
    {

        Load();
    }

    private void Load()
    {

        int n = 0;
        for (int i=0;i< GameManager.instance.myPlayer.dailyBonus.Count;i++)
        {
            dayText[i].text = DataHolder.Dialog(94).GetName(MageManager.instance.GetLanguage()) + " " + (i+1).ToString("00");
            int id = i;
            if(i < 6)
            {
                covers[i].SetActive(false);
                dones[i].SetActive(false);
            }
 
            collectButtons[i].interactable = false;
            collectButtons[i].onClick.AddListener(delegate { OnCollect(id); });
            if (GameManager.instance.myPlayer.dailyBonus[i].isCollected)
            {
                n++;
                if (i < 6)
                {
                    covers[i].SetActive(true);
                    dones[i].SetActive(true);
                }

            }
        }

        Debug.Log(n);
        if(n == 0)
        {
            collectButtons[n].interactable = true;
        }
        else
        {
            Debug.Log(System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived).Day);
            Debug.Log(System.DateTime.Now.Day);
            
            if(System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived).Year < System.DateTime.Now.Year || System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived).Month < System.DateTime.Now.Month || System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n-1].timeReceived).Day < System.DateTime.Now.Day)
            {
                collectButtons[n].interactable = true;
            }
        }
        
    }


    public void OnCollect(int id)
    {
        MageManager.instance.PlaySound("Collect_Achivement", false);

        if (id == 0)
        {
            GameManager.instance.AddCoin(100);
        }else if(id == 1)
        {
            if (!GameManager.instance.IsHaveItem(42))
            {
                GameManager.instance.AddItem(42);
                GameManager.instance.EquipItem(42);
            }
        }
        else if (id == 2)
        {
            if (!GameManager.instance.IsHaveItem(7))
            {
                GameManager.instance.AddItem(7);
                GameManager.instance.EquipItem(7);
            }
        }
        else if (id == 3)
        {
            if (!GameManager.instance.IsHaveItem(15))
            {
                GameManager.instance.AddItem(15);
                GameManager.instance.EquipItem(15);
            }
        }
        else if (id == 4)
        {
            if (!GameManager.instance.IsHaveItem(77))
            {
                GameManager.instance.AddItem(77);
                GameManager.instance.EquipItem(77);
            }
        }
        else if (id == 5)
        {
            if (!GameManager.instance.IsHaveItem(91))
            {
                GameManager.instance.AddItem(91);
                GameManager.instance.EquipItem(91);
            }
        }
        else if (id == 6)
        {
            if (!GameManager.instance.IsHavePet(34))
            {
                GameManager.instance.AddPet(34);
                GameManager.instance.GetPet(34).isNew = true;
                GameManager.instance.EquipPet(34);
            }
            this.Close();
        }
        GameManager.instance.myPlayer.dailyBonus[id].Collect();
        this.Load();

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
