using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    public int gameId = 0;
    public Text levelText;
    public Text title;
    public Image gameIcon;
    public GameObject[] stars;
    public Animator progress;
    public Text priceText;
    public int price;

    // Start is called before the first frame update
    void Awake()
    {
        price = GameManager.instance.myPlayer.minigameLevels[gameId] + 1;
        priceText.text = price.ToString();
        levelText.text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) +  " " + (GameManager.instance.myPlayer.minigameLevels[gameId] + 1).ToString();
        int n = (GameManager.instance.myPlayer.minigameLevels[gameId]+1) % 5;

        for (int i = 0; i < stars.Length; i++)
        {
            if (n == 0)
            {
                progress.Play("Active", 0);
                stars[i].SetActive(false);
            }
            else
            {
                progress.Play("Idle", 0);
                if (i < n-1)
                {
                    stars[i].SetActive(false);
                }
                else
                    stars[i].SetActive(true);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlay()
    {
        MageManager.instance.PlaySoundName("BubbleButton", false);
        if (GameManager.instance.GetHappy() < price)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
        }else
        {
            GameManager.instance.AddHappy(-price);
            UIManager.instance.OnMinigame(gameId);
            if (UIManager.instance.eventPanel != null)
            {
                UIManager.instance.eventPanel.Close();
            }
        }
    }
}
