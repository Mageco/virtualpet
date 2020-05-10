using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeBackPanel : MonoBehaviour
{
    public Text coinText;
    public Text happyText;
    public Text expText;
    int coin = 1;
    int happy = 1;
    int exp = 1;
    public Button AdButton;
    // Start is called before the first frame update

    public void Load(int c,int h,int e)
    {
        coin = c;
        happy = h;
        exp = e;
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = coin.ToString();
        happyText.text = happy.ToString();
        expText.text = exp.ToString();
        if (RewardVideoAdManager.instance.isUnityVideoLoaded)
            AdButton.interactable = true;
        else
            AdButton.interactable = false;
    }


    public void WatchAd()
    {
        AdButton.gameObject.SetActive(false);
        MageManager.instance.PlaySound("BubbleButton", false);
        RewardVideoAdManager.instance.ShowAd(RewardType.Welcome);
    }

    public void WatchedAd()
    {
        coin = coin * 2;
        happy = happy * 2;
        exp = exp * 2;
    }

    public void Close()
    {
        MageManager.instance.PlaySound("Collect_Achivement", false);
        GameManager.instance.AddCoin(coin,Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        GameManager.instance.AddHappy(happy, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        GameManager.instance.AddExp(exp, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));

        this.GetComponent<Popup>().Close();
    }
}
