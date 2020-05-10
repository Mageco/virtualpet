using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideUI : MonoBehaviour
{
    public GameObject[] pages;
    int currentPage = 0;
    // Start is called before the first frame update
    void Start()
    {
        OnPage(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPage(int id){
        for(int i=0;i<pages.Length;i++){
            if(i==id){
                pages[i].SetActive(true);
            }else
                pages[i].SetActive(false);
        }
        
    }

    public void Next(){
        MageManager.instance.PlaySound("BubbleButton", false);
        currentPage ++;
        if(currentPage > pages.Length - 1){
            Close();
            if(Minigame.instance != null)
                Minigame.instance.StartGame();
        }else
            OnPage(currentPage);
    }

    public void Close(){
        this.GetComponent<Popup>().Close();
    }
}
