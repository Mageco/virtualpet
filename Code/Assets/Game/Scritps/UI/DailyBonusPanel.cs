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

        //Debug.Log(n);
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
            GameManager.instance.AddDiamond(10);
        }
        else if (id == 2)
        {
            GameManager.instance.AddCoin(500);
        }
        else if (id == 3)
        {
            GameManager.instance.AddDiamond(20);
        }
        else if (id == 4)
        {
            GameManager.instance.AddCoin(1000);
        }
        else if (id == 5)
        {
            GameManager.instance.AddDiamond(50);
        }
        else if (id == 6)
        {
            GameManager.instance.AddRandomPet(RareType.Common);
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
