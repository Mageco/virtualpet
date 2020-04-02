using System.Collections;
using System.Collections.Generic;
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
        Close();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
