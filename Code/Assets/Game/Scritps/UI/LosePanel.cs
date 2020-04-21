using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LosePanel : MonoBehaviour
{
    public Text priceText;
    public Text coin;
    int gameId = 0;
    int price = 5;


    // Start is called before the first frame update
    void Start()
    {
        priceText.text = price.ToString();
        MageManager.instance.PlaySound("Lose", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(int c, int minigameId)
    {
        coin.text = c.ToString();
        gameId = minigameId;
        price = GameManager.instance.myPlayer.minigameLevels[gameId] + 1;
        priceText.text = price.ToString();
    }

    public void Close(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }

    public void OnHome(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }

    public void Replay()
    {
        if (GameManager.instance.GetHappy() < price)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
        }
        else
        {
            GameManager.instance.AddHappy(-price, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
            MageManager.instance.LoadScene(SceneManager.GetActiveScene().name, 0.5f);
            this.GetComponent<Popup>().Close();
        }
    }
}
