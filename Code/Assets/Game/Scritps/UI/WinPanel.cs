using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    public Image header;
    public Sprite[] headerSprites;
    public Text exp;
    public Text coin;
    int price = 5;
    int gameId = 0;
    public Text priceText;
    int bonus = 0;
    public Button watchAd;
    public Text nextText;
    public Text replayText;
    public Text completeText;

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
        bonus = c;
        gameId = minigameId;
        price = GameManager.instance.myPlayer.minigameLevels[gameId] + 1;
        priceText.text = price.ToString();
        animator = this.GetComponent<Animator>();
        animator.Play("Win",0);


        if (isWin)
        {
            header.sprite = headerSprites[0];
            completeText.gameObject.SetActive(true);
            replayText.gameObject.SetActive(false);
            nextText.gameObject.SetActive(true);
            if (minigameId == 0)
            {
                if ((GameManager.instance.myPlayer.minigameLevels[0] + 1) % 5 == 0 || GameManager.instance.myPlayer.minigameLevels[0] == 0)
                {
                    exp.transform.parent.gameObject.SetActive(true);
                    GameManager.instance.AddItem(72);
                    GameManager.instance.EquipItem(72);
                }
                else
                    exp.transform.parent.gameObject.SetActive(false);
            }
            else if (minigameId == 1)
            {
                exp.transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            exp.transform.parent.gameObject.SetActive(false);
            header.sprite = headerSprites[1];
            completeText.gameObject.SetActive(false);
            replayText.gameObject.SetActive(true);
            nextText.gameObject.SetActive(false);
        }
            

        if (c > 0){
            coin.text = c.ToString(); 
        }
        else
            coin.transform.parent.gameObject.SetActive(false);
    }

    public void OnHome(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }

    public void Replay(){

        if (GameManager.instance.GetHappy() < price)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
        }
        else
        {
            GameManager.instance.AddCoin(bonus);
            GameManager.instance.AddHappy(-price);
            MageManager.instance.LoadScene(SceneManager.GetActiveScene().name, 0.5f);
            this.GetComponent<Popup>().Close();
        }
    }

    public void OnWatchedAd()
    {
        bonus = bonus * 2;
        coin.text = bonus.ToString();
        watchAd.interactable = false;
    }

    public void OnWatchAd()
    {
        RewardVideoAdManager.instance.ShowAd(RewardType.Minigame);
    }

    public void Close(){
        MageManager.instance.PlaySound("Collect_Achivement", false);
        GameManager.instance.AddCoin(bonus);
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }
}
