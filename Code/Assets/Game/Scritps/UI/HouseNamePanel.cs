using System.Collections;
using System.Collections.Generic;
using Mage.Models.Users;
using MageSDK.Client;
using UnityEngine;
using UnityEngine.UI;

public class HouseNamePanel : MonoBehaviour
{
    public InputField input;
    
    // Start is called before the first frame update
    void Start()
    {
        input.text = GameManager.instance.myPlayer.playerName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSubmit()
    {
        GameManager.instance.myPlayer.playerName = input.text;
        User u = MageEngine.instance.GetUser();
        u.fullname = GameManager.instance.myPlayer.playerName;
        MageEngine.instance.UpdateUserProfile(u);
        Close();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
