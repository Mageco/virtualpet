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
    public Button buttonCost;
    System.DateTime timeSpin = System.DateTime.Now;
    public Image[] icons;
    int price = 2;
    public Text priceText;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        priceText.text = price.ToString();
    }


    void LoadTime()
    {


        if (GameManager.instance.myPlayer.spinedTime != "")
        {
            timeSpin = System.DateTime.Parse(GameManager.instance.myPlayer.spinedTime);
        }
        else
            GameManager.instance.myPlayer.spinCount = 1;



        if (GameManager.instance.IsYesterDay(timeSpin))
        {
            GameManager.instance.myPlayer.spinCount = 1;
        }



        LoadButton();
    }

    void LoadButton()
    {
        if (GameManager.instance.myPlayer.spinCount >= 1)
        {
            buttonFree.gameObject.SetActive(true);
            buttonAd.gameObject.SetActive(false);
            buttonFree.interactable = true;
            buttonCost.gameObject.SetActive(false);
        }
        else
        {
            buttonFree.gameObject.SetActive(false);
            buttonAd.gameObject.SetActive(true);
            buttonAd.interactable = true;
            buttonCost.gameObject.SetActive(false);
            buttonCost.interactable = true;
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
        if (!ApiManager.instance.IsLogin())
        {
            MageManager.instance.OnNotificationPopup("Network error");
            return;
        }

        MageManager.instance.PlaySound("BubbleButton", false);
        StartCoroutine(DoSpin());
        buttonFree.interactable = false;
    }

    private IEnumerator DoSpin()
    {
        int soundId = MageManager.instance.PlaySound("Wheel_Loop", false);
        GameManager.instance.myPlayer.spinedTime = MageEngine.instance.GetServerTimeStamp().ToString();
        GameManager.instance.myPlayer.spinCount -= 1;
        speed = Random.Range(2f, 5f);
        float a = Random.Range(0.5f, 1f);
        Debug.Log(speed);
        bool isStop = false;

        while (speed > 0)
        {
            if (speed < 1.2f && !isStop)
            {
                isStop = true;
                MageManager.instance.StopSound(soundId);
                MageManager.instance.PlaySound("Wheel_Stop", false);
            }



            speed -= a * Time.deltaTime;
            if (speed < 0.1f && spinArrow.transform.rotation.eulerAngles.z > 0)
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



        LoadButton();
        List<int> items = new List<int>();
        int id = 0;
        int value = 2;

        if (n == 4)
            MageManager.instance.PlaySound("Win", false);
        else
            MageManager.instance.PlaySound("collect_item_02", false);

        if (n == 0)
        {
            value = 100;
            GameManager.instance.AddCoin(value, GetKey());
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString(), DataHolder.Dialog(6).GetName(MageManager.instance.GetLanguage()));
        }
        else if (n == 1)
        {

            for (int i = 231; i <= 238; i++)
            {
                items.Add(i);
            }
            items.Add(244);
            id = Random.Range(0, items.Count);
            value = (int)DataHolder.GetItem(items[id]).value;
            GameManager.instance.AddItem(items[id], value, GetKey());
            string url = DataHolder.GetItem(items[id]).iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            string itemName = DataHolder.GetItem(items[id]).GetName(MageManager.instance.GetLanguage());
            UIManager.instance.OnSpinRewardPanel(Resources.Load<Sprite>(url) as Sprite, value.ToString(), itemName);
        }
        else if (n == 2)
        {
            value = 5;
            GameManager.instance.AddDiamond(value, GetKey());
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString(), DataHolder.Dialog(7).GetName(MageManager.instance.GetLanguage()));
        }
        else if (n == 3)
        {

            for (int i = 204; i <= 215; i++)
            {
                items.Add(i);
            }
            id = Random.Range(0, items.Count);
            value = (int)DataHolder.GetItem(items[id]).value;
            GameManager.instance.AddItem(items[id], value, GetKey());
            string url = DataHolder.GetItem(items[id]).iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            string itemName = DataHolder.GetItem(items[id]).GetName(MageManager.instance.GetLanguage());
            UIManager.instance.OnSpinRewardPanel(Resources.Load<Sprite>(url) as Sprite, value.ToString(), itemName);
        }
        else if (n == 4)
        {

            for (int i = 239; i <= 241; i++)
            {
                items.Add(i);
            }

            id = Random.Range(0, items.Count);
            value = (int)DataHolder.GetItem(items[id]).value;
            GameManager.instance.AddItem(items[id], value, GetKey());
            string url = DataHolder.GetItem(items[id]).iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            string itemName = DataHolder.GetItem(items[id]).GetName(MageManager.instance.GetLanguage());
            UIManager.instance.OnSpinRewardPanel(Resources.Load<Sprite>(url) as Sprite, value.ToString(), itemName);
        }
        else if (n == 5)
        {
            value = 50;
            GameManager.instance.AddExp(value, GetKey());
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString(), "Exp");
        }
        else if (n == 6)
        {
            value = 500;
            GameManager.instance.AddCoin(value, GetKey());
            UIManager.instance.OnSpinRewardPanel(icons[n].sprite, value.ToString(), DataHolder.Dialog(6).GetName(MageManager.instance.GetLanguage()));
        }
        else if (n == 7)
        {
            for (int i = 216; i <= 230; i++)
            {
                items.Add(i);
            }
            items.Add(242);
            items.Add(243);
            items.Add(245);

            id = Random.Range(0, items.Count);
            value = (int)DataHolder.GetItem(items[id]).value;
            GameManager.instance.AddItem(items[id], value, GetKey());
            string url = DataHolder.GetItem(items[id]).iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            string itemName = DataHolder.GetItem(items[id]).GetName(MageManager.instance.GetLanguage());
            UIManager.instance.OnSpinRewardPanel(Resources.Load<Sprite>(url) as Sprite, value.ToString(), itemName);
        }
    }

    public void OnWatch()
    {
        RewardVideoAdManager.instance.ShowVideoAd(RewardType.SpinWheel);
    }

    public void OnCost()
    {
        if (GameManager.instance.GetDiamond() >= price)
        {
            buttonAd.interactable = false;
            buttonCost.interactable = false;
            GameManager.instance.AddDiamond(-price, GetKey());
            StartCoroutine(DoSpin());
        }
        else
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(7).GetDescription(MageManager.instance.GetLanguage()));
        }

    }

    public void OnWatched()
    {
        buttonAd.interactable = false;
        buttonCost.interactable = false;
        StartCoroutine(DoSpin());
    }

    public void Close()
    {
        StopAllCoroutines();
        this.GetComponent<Popup>().Close();
    }

    string GetKey()
    {
        return Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013");
    }
}
