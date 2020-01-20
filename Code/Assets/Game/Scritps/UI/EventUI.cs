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
    int playCount = 5;
    
    public Text playText;
    public Text timeText;
    float time = 2;
    float maxTimeUpdate = 1;
    System.DateTime startTime;
    int maxPlayCount = 5;
    public Image[] playCountIcon;
    public GameObject adButton;
    public GameObject playButton;

    // Start is called before the first frame update
    void Awake()
    {
        levelText.text = "Level " + (GameManager.instance.myPlayer.minigameLevels[gameId] + 1).ToString();
        int n = (GameManager.instance.myPlayer.minigameLevels[0]+1) % 5;

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

        if (ES2.Exists("MinigameWait" + gameId.ToString()))
        {
            startTime = ES2.Load<System.DateTime>("MinigameWait" + gameId.ToString());
        }
        else
            startTime = System.DateTime.Now;
        
        if(ES2.Exists("MinigamePlayCount" + gameId.ToString()))
        {
            playCount = ES2.Load<int>("MinigamePlayCount" + gameId.ToString());
        }

        UpdatePlayCount();
    }

    // Update is called once per frame
    void Update()
    {
        if(time > maxTimeUpdate)
        {
            time = 0;
            int t = (int)(600 - (System.DateTime.Now - startTime).TotalSeconds);
            int m = t / 60;
            timeText.text = m.ToString("00") + ":" + (t - m * 60).ToString("00");
            

            if ((System.DateTime.Now - startTime).TotalSeconds > 600)
            {
                playCount += ((int)(System.DateTime.Now - startTime).TotalSeconds)/600;
                if(playCount > maxPlayCount)
                {
                    playCount = maxPlayCount;
                }
                startTime = System.DateTime.Now;
                UpdatePlayCount();
            }
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    public void OnPlay()
    {

        playCount--;
        if(playCount == maxPlayCount - 1)
        {
            startTime = startTime = System.DateTime.Now;
            
        }
        UpdatePlayCount();
        MageManager.instance.PlaySoundName("BubbleButton", false);
        UIManager.instance.OnMinigame(gameId);
        if (UIManager.instance.eventPanel != null)
        {
            UIManager.instance.eventPanel.Close();
        }
    }

    void UpdatePlayCount()
    {
        if(playCount >= maxPlayCount)
        {
            timeText.gameObject.SetActive(false);
        }

        for(int i = 0; i < playCountIcon.Length; i++)
        {
            if (i >= playCount)
            {
                playCountIcon[i].color = Color.black;
            }
            else
                playCountIcon[i].color = Color.white;
        }

        if(playCount == 0)
        {
            playButton.SetActive(false);
            adButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(true);
            adButton.SetActive(false);
        }

        ES2.Save(startTime, "MinigameWait" + gameId.ToString());
        ES2.Save<int>(playCount, "MinigamePlayCount" + gameId.ToString());
    }


}
