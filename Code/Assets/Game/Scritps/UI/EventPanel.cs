using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
     public ScrollRect scroll;
     public GameObject playButton;
    public EventUI[] events;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(int id)
    {
        for (int i = 0; i < events.Length; i++)
        {
            if (i == id)
            {
                events[i].gameObject.SetActive(true);
            }else
            {
                events[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnEvent(int id){
        this.Close();
        MageManager.instance.PlaySound("BubbleButton",false);
        UIManager.instance.OnMinigame(id);
    }

    public void Close(){
        this.GetComponent<Popup>().Close();
    }

     public void OnLeft(){
        if(scroll.horizontalNormalizedPosition > 0)
            scroll.horizontalNormalizedPosition -= 0.333f; 
    }

    public void OnRight(){
        if(scroll.horizontalNormalizedPosition < 1)
            scroll.horizontalNormalizedPosition += 0.333f; 
    }
}
