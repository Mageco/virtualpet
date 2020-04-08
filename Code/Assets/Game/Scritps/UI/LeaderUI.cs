using System.Collections;
using System.Collections.Generic;
using Mage.Models.Users;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(LeaderBoardItem data)
    {
        score.text = data.attr_value.ToString();
    }
}
