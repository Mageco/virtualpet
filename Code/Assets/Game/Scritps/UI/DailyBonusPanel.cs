using System.Collections;
using System.Collections.Generic;
using MageSDK.Client;
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
        }else if (n == 7)
        {
            if (GameManager.instance.IsYesterDay(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived))
            {
                for (int i = 0; i < GameManager.instance.myPlayer.dailyBonus.Count; i++)
                {
                    GameManager.instance.myPlayer.dailyBonus[i].isCollected = false;
                    GameManager.instance.myPlayer.dailyBonus[i].timeReceived = System.DateTime.Now.ToString();
                    Load();
                }
            }
        }
        else
        {
           
            Debug.Log(MageEngine.instance.GetServerTimeStamp().Day);

            if (GameManager.instance.IsYesterDay(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived))
            {
                collectButtons[n].interactable = true;
            }
        }


        
    }

    string GetKey()
    {
        return Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013");
    }

    public void OnCollect(int id)
    {
        if (!ApiManager.instance.IsLogin())
        {
            MageManager.instance.OnNotificationPopup("Network error");
            return;
        }

        MageManager.instance.PlaySound("Collect_Achivement", false);

        if (id == 0)
        {
            GameManager.instance.AddCoin(100,GetKey());
        }else if(id == 1)
        {
            GameManager.instance.AddDiamond(5, GetKey());
        }
        else if (id == 2)
        {
            GameManager.instance.AddCoin(500, GetKey());
        }
        else if (id == 3)
        {
            GameManager.instance.AddDiamond(10, GetKey());
        }
        else if (id == 4)
        {
            GameManager.instance.AddCoin(1000, GetKey());
        }
        else if (id == 5)
        {
            GameManager.instance.AddDiamond(15, GetKey());
        }
        else if (id == 6)
        {
            GameManager.instance.AddRandomPet(RareType.Common, GetKey());
            GameManager.instance.myPlayer.dailyBonus[id].Collect();
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
