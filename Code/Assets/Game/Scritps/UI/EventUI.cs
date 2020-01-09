using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    public int gameId = 0;
    public Text levelText;
    public GameObject[] stars;
    public Animator progress;

    // Start is called before the first frame update
    void Start()
    {
        levelText.text = "Level " + (GameManager.instance.myPlayer.minigameLevels[0] + 1).ToString();
        int n = (GameManager.instance.myPlayer.minigameLevels[0]+1) % 5;
        for (int i = 0; i < stars.Length; i++)
        {
            if(i > n)
            {

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
