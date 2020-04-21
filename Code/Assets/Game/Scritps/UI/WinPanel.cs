using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    public Text exp;
    public Text coin;
    public Text item;
    public Text priceText;
    int bonus = 0;
    public Button watchAd;
    public GameObject nextText;
    public GameObject replayText;
    public Text completeText;
    int minigameId = 0;

    // Start is called before the first frame update
    void Start()
    {
        MageManager.instance.PlaySound("Win", false);
    }

    Animator animator;

    void Awake(){
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(int c,int minigameId,bool isWin){
        this.minigameId = minigameId;
        bonus = c;
        animator = this.GetComponent<Animator>();
       


        if (isWin)
        {
            animator.Play("Minigame_Win_Open", 0);
            completeText.gameObject.SetActive(true);
            replayText.gameObject.SetActive(false);
            nextText.gameObject.SetActive(true);
            if (minigameId == 0)
            {
                if ((GameManager.instance.myPlayer.minigameLevels[0] + 1) % 5 == 0 || GameManager.instance.myPlayer.minigameLevels[0] == 0)
                {
                    item.transform.parent.gameObject.SetActive(true);
                    GameManager.instance.AddItem(72, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
                    GameManager.instance.EquipItem(72);
                }
                else
                    item.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                item.transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            animator.Play("Minigame_Lose_Open", 0);
            item.transform.parent.gameObject.SetActive(false);
           completeText.gameObject.SetActive(false);
            replayText.gameObject.SetActive(true);
            nextText.gameObject.SetActive(false);
        }
            

        if (c > 0){
            coin.text = c.ToString();
            exp.text = (c/10).ToString();
        }
        else
        {
            coin.transform.parent.gameObject.SetActive(false);
            exp.transform.parent.gameObject.SetActive(false);
        }

        if (!isWin)
        {
            int r = Random.Range(0, 100);
            if(r > 50 && (minigameId == 0 || minigameId == 1))
                RewardVideoAdManager.instance.ShowIntetestial();

            if (r > 70 && (minigameId == 2))
                RewardVideoAdManager.instance.ShowIntetestial();

            if (r > 60 && (minigameId == 3))
                RewardVideoAdManager.instance.ShowIntetestial();
        }
            
    }

    public void OnHome(){
        MageManager.instance.PlaySound("Collect_Achivement", false);
        GameManager.instance.AddCoin(bonus, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        GameManager.instance.AddExp(bonus / 10, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        if (minigameId == 2 || minigameId == 3)
            UIManager.instance.OnMap(MapType.Forest);
        else
            Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }

    public void Replay(){

        MageManager.instance.PlaySound("Collect_Achivement", false);
        GameManager.instance.AddCoin(bonus, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        GameManager.instance.AddExp(bonus / 10, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        MageManager.instance.LoadScene(SceneManager.GetActiveScene().name, 0.5f);
            this.GetComponent<Popup>().Close();
    }

    public void OnWatchedAd()
    {
        bonus = bonus * 2;
        coin.text = bonus.ToString();
        exp.text = (bonus / 10).ToString();
        watchAd.interactable = false;
    }

    public void OnWatchAd()
    {
        RewardVideoAdManager.instance.ShowAd(RewardType.Minigame);
    }

    public void OnLeaderBoard()
    {
        UIManager.instance.OnLeaderBoardPanel(this.minigameId+1);
    }

    public void Close(){
        MageManager.instance.PlaySound("Collect_Achivement", false);
        GameManager.instance.AddCoin(bonus,Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        GameManager.instance.AddExp(bonus/10, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }
}
