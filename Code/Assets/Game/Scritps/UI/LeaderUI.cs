using System.Collections;
using System.Collections.Generic;
using Mage.Models.Users;
using MageSDK.Client;
using UnityEngine;
using UnityEngine.UI;

public class LeaderUI : MonoBehaviour
{
    [HideInInspector]
    public int iD = 0;
    public GameObject[] rankIcons;
    public Image avatar;
    public Text playerName;
    public GameObject[] scoreIcons;
    public Text score;
    public Color highlightColor = Color.white;
    int tabId;
    string userId = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(LeaderBoardItem data,int id)
    {
        OffAllIcon();
        tabId = id;
        userId = data.user_id;

        if(userId == MageEngine.instance.GetUser().id)
        {
            this.GetComponent<Image>().color = highlightColor;
        }
        score.text = data.score.ToString();
        playerName.text = data.fullname;
        if(data.avatar != "")
        {
            MageEngine.instance.LoadImage(
           data.avatar,
                       (texture2D) =>
                       {
                           if (texture2D != null && avatar != null)
                               avatar.sprite = Utils.instance.CreateSprite(texture2D);
                       });
        }



        if (data.rank == 1)
        {
            rankIcons[0].SetActive(true);
        }else if(data.rank == 2)
        {
            rankIcons[1].SetActive(true);
        }
        else if (data.rank == 3)
        {
            rankIcons[2].SetActive(true);
        }
        else if (data.rank > 3)
        {
            rankIcons[3].SetActive(true);
            rankIcons[3].GetComponent<Text>().text = data.rank.ToString();
        }

        if(tabId == 0)
        {
            scoreIcons[0].SetActive(true);
        }else if(tabId == 1 || tabId == 2)
        {
            scoreIcons[1].SetActive(true);
        }else if(tabId == 3)
        {
            scoreIcons[2].SetActive(true);
        }
    }

    void OffAllIcon()
    {
        for(int i = 0; i < rankIcons.Length; i++)
        {
            rankIcons[i].SetActive(false);
        }
        for (int i = 0; i < scoreIcons.Length; i++)
        {
            scoreIcons[i].SetActive(false);
        }
    }


}
