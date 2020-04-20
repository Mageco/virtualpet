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
    public Button visitButton;
    int tabId;
    string userId = "";
    Texture2D texture;
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
        if(data.avatar != null || data.avatar != "")
        {
            MageEngine.instance.LoadImage(
           data.avatar,
                       (texture2D) =>
                       {
                           texture = texture2D;
                           if ( texture2D != null && texture2D.width > 10 && avatar != null)
                           {
                             
                               avatar.sprite = Utils.instance.CreateSprite(texture2D);
                           }
                               
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
        }else if(tabId == 3 || tabId == 4)
        {
            scoreIcons[2].SetActive(true);
        }
        /*
        if (data.rank <= 50 && userId != MageEngine.instance.GetUser().id)
        {
            visitButton.gameObject.SetActive(true);
        }
        else
            visitButton.gameObject.SetActive(false);*/

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

    public void OnAvatar()
    {
        if(avatar.sprite != null)
            UIManager.instance.OnRemotePlayerPanel(avatar.sprite);
    }

    public void OnVisit()
    {
        UIManager.instance.OnFriendHouse(userId,avatar.sprite);
    }

    public void OnClose()
    {
        Texture2D.Destroy(texture);
    }
}
