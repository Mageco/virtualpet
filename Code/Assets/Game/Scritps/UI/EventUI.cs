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

    // Start is called before the first frame update
    void Start()
    {
        levelText.text = "Level " + (GameManager.instance.myPlayer.minigameLevels[0] + 1).ToString();
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
