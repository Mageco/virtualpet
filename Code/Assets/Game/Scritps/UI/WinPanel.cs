using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    public Text exp;
    public Text coin;
    public Text diamon;
    int price = 5;
    int gameId = 0;
    public Text priceText;

    // Start is called before the first frame update
    void Start()
    {
        MageManager.instance.PlaySoundName("Win", false);
    }

    Animator animator;

    void Awake(){

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(int d, int c,int minigameId){
        
        gameId = minigameId;
        price = GameManager.instance.myPlayer.minigameLevels[gameId] + 1;
        priceText.text = price.ToString();
        animator = this.GetComponent<Animator>();
        animator.Play("Win",0);
        if(minigameId == 0)
        {
            if ((GameManager.instance.myPlayer.minigameLevels[0] + 1) % 5 == 0 || GameManager.instance.myPlayer.minigameLevels[0] == 0)
            {
                exp.transform.parent.gameObject.SetActive(true);
                GameManager.instance.AddItem(72);
                GameManager.instance.EquipItem(72);
            }
            else
                exp.transform.parent.gameObject.SetActive(false);
        }else if(minigameId == 1)
        {
            exp.transform.parent.gameObject.SetActive(false);
        }

        
        if(c > 0){
            coin.text = c.ToString();
            GameManager.instance.AddCoin(c);
        }
        else
            coin.transform.parent.gameObject.SetActive(false);

        if(d > 0){
            diamon.text = d.ToString();
            GameManager.instance.AddDiamond(d);
        }
        else
            diamon.transform.parent.gameObject.SetActive(false);

        
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
            GameManager.instance.AddHappy(-price);
            MageManager.instance.LoadScene(SceneManager.GetActiveScene().name, 0.5f);
            this.GetComponent<Popup>().Close();
        }
    }

    public void Close(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }


}
