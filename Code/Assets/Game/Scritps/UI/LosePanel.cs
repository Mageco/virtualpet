using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LosePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MageManager.instance.PlaySoundName("Lose", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(){

    }

    public void Close(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }

    public void OnHome(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }

    public void Replay(){
        MageManager.instance.PlaySoundName("BubbleButton", false);
        int price = 10; 
        if(GameManager.instance.GetHappy() >= price){
            GameManager.instance.AddHappy(-price);
            MageManager.instance.LoadScene(SceneManager.GetActiveScene().name,0.5f);
            this.GetComponent<Popup>().Close();
        }else
        {
            MageManager.instance.OnNotificationPopup ("Bạn không đủ tim để chơi trò chơi này hãy chăm sóc thú cưng để thu thập thêm tim nhé.");
        }
    }
}
