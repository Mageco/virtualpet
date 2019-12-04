using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
     public ScrollRect scroll;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEvent(int id){
        if(id > 0)
        {
             MageManager.instance.OnNotificationPopup ("Trò chơi này chưa ra mắt");
        }else{
            if(GameManager.instance.GetPet(0).level > 0){
                this.Close();
                UIManager.instance.OnMinigame(1);
            }else
            {
                MageManager.instance.OnNotificationPopup ("Bạn cần tiến hoá lên chó trưởng thành");
            }
        }
       
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
