using System.Collections;
using System.Collections.Generic;
using MageSDK.Client;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheelPanel : MonoBehaviour
{
    // This animation curve drives the spin wheel motion.
    //public AnimationCurve AnimationCurve;
    Animator animator;
    float speed = 0;
    public GameObject spinArrow;
    public GameObject spin;
    public Button buttonFree;
    public Button buttonAd;
    int spinCount = 0;
    int rewardId = 0;
    System.DateTime timeSpin = System.DateTime.Now;
    public Image[] icons;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
       
    }


    void LoadTime()
    {
        if (ES2.Exists("TimeSpin"))
        {
            timeSpin = ES2.Load<System.DateTime>("TimeSpin");
        }
        else
            spinCount = 20;

        if (timeSpin.Year < MageEngine.instance.GetServerTimeStamp().Year && timeSpin.Month < MageEngine.instance.GetServerTimeStamp().Month && timeSpin.Day < MageEngine.instance.GetServerTimeStamp().Day)
        {
            spinCount = 2;
        }

        LoadButton();
    }

    void LoadButton()
    {
        if (spinCount >= 2)
        {
            buttonFree.gameObject.SetActive(true);
            buttonAd.gameObject.SetActive(false);
            buttonFree.interactable = true;
        }
        else if (spinCount == 1)
        {
            buttonFree.gameObject.SetActive(false);
            buttonAd.gameObject.SetActive(true);
            buttonAd.interactable = true;
        }
        else
        {
            buttonFree.gameObject.SetActive(true);
            buttonAd.gameObject.SetActive(false);
            buttonFree.interactable = false;
        }
    }

    IEnumerator Start()
    {
        LoadTime();
        yield return new WaitForSeconds(1);
        animator.Play("Active", 0);
        animator.speed = 0;
    }

   

    public void Spin()
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        StartCoroutine(DoSpin());
        buttonFree.interactable = false;
    }

    private IEnumerator DoSpin()
    {
        int soundId = MageManager.instance.PlaySound("CollectItem", true);

        spinCount -= 1;
        speed = Random.Range(2f,5f);
        float a = Random.Range(0.5f, 1f);
        Debug.Log(speed);
       
        while (speed > 0)
        {
            
            speed -= a * Time.deltaTime;
            if(speed < 0.1f && spinArrow.transform.rotation.eulerAngles.z > 0)
            {
                speed = 0.1f;
            }
            if (speed < 0)
                speed = 0;
            animator.speed = speed;
            yield return new WaitForEndOfFrame();
        }

        int n = (int)((spin.transform.rotation.eulerAngles.z + 22.5f) / 45f);
        if (n == 8)
            n = 0;
        Debug.Log(n);

        MageManager.instance.StopSound(soundId);

        LoadButton();

        if(n==4)
            MageManager.instance.PlaySound("Win", false);
        else
            MageManager.instance.PlaySound("collect_item_02", false);

        if (n == 0)
        {
            int value = 100;
            GameManager.instance.AddCoin(value);
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString());
        }else if(n == 1)
        {
            int value = 2;
            GameManager.instance.AddDiamond(value);
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString());
        }
        else if (n == 2)
        {
            int value = 300;
            GameManager.instance.AddHappy(value);
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString());
        }
        else if (n == 3)
        {
            int value = 10;
            GameManager.instance.AddDiamond(value);
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString());
        }
        else if (n == 4)
        {
            int value = 1;
            List<Item> items = new List<Item>();
            for(int i = 0; i < DataHolder.Items().GetDataCount(); i++)
            {
                if (DataHolder.Item(i).isAvailable && DataHolder.Item(i).itemType != ItemType.Animal && DataHolder.Item(i).itemType != ItemType.Diamond
                    && DataHolder.Item(i).itemType != ItemType.Chest && DataHolder.Item(i).itemType != ItemType.Coin && DataHolder.Item(i).itemType != ItemType.MagicBox)
                {
                    bool isAdd = true;
                    foreach (PlayerItem p in GameManager.instance.myPlayer.items)
                    {
                        if (p.itemId == DataHolder.Item(i).iD && p.state != ItemState.OnShop)
                        {
                            isAdd = false;
                        }
                    }
                    if (isAdd)
                    {
                        items.Add(DataHolder.Item(i));
                    }
                }
            }
            items.Sort((p1, p2) => (p1.levelRequire).CompareTo(p2.levelRequire));
            if(items.Count > 1)
            {
                bool isAdopt = false;
                int id = 0;
                while (!isAdopt)
                {
                    int ran = Random.Range(0, 100);
                    if (ran > 60)
                    {
                        isAdopt = true;
                    }
                    else
                    {
                        id++;
                        if (id >= items.Count)
                            id = 0;
                    }
                }
                GameManager.instance.AddItem(items[id].iD);
                GameManager.instance.EquipItem(items[id].iD);
                string url = items[id].iconUrl.Replace("Assets/Game/Resources/", "");
                url = url.Replace(".png", "");
                UIManager.instance.OnSpinRewardPanel(Resources.Load<Sprite>(url) as Sprite, value.ToString());
            }
        }
        else if (n == 5)
        {
            int value = 5;
            GameManager.instance.AddExp(value);
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString());
        }
        else if (n == 6)
        {
            int value = 500;
            GameManager.instance.AddCoin(value);
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString());
        }
        else if (n == 7)
        {
            int value = 50;
            GameManager.instance.AddHappy(value);
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString());
        }

    }

    public void OnWatch()
    {
        RewardVideoAdManager.instance.ShowAd(RewardType.SpinWheel);
    }

    public void OnWatched()
    {
        buttonAd.interactable = false;
        StartCoroutine(DoSpin());
    }

    public void Close()
    {
        StopAllCoroutines();
        this.GetComponent<Popup>().Close();
    }
}
