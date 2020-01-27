using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatItem : MonoBehaviour
{

    public GameObject[] levelSprites;
    // Start is called before the first frame update
    void Start()
    {
        int n = GameManager.instance.myPlayer.minigameLevels[1] / 5;
        for(int i = 0; i < levelSprites.Length; i++)
        {
            if (i < n)
            {
                levelSprites[i].SetActive(true);
            }
            else
                levelSprites[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
