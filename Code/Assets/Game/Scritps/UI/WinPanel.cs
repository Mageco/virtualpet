using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    public Text exp;
    public Text coin;
    public Text diamon;
    public Sprite nextButton;
    public Sprite replayButton;
    public Sprite homeButton;
    int star = 0;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        MageManager.instance.PlaySoundName("Win", false);
        for(int i = 0; i < star; i++)
        {
            yield return new WaitForSeconds(0.3f);
            MageManager.instance.PlaySoundName("collect_item_11", false);
        }
       
    }

    Animator animator;

    void Awake(){
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(int star,int e, int d, int c){
        animator = this.GetComponent<Animator>();
        animator.SetInteger("star",star);
        this.star = star;
        if(e > 0){
            exp.text = e.ToString();
            //GameManager.instance.AddExp(e,GameManager.instance.GetActivePet().iD);
        }
        else
            exp.transform.parent.gameObject.SetActive(false);
        
        if(c > 0){
            coin.text = c.ToString();
            GameManager.instance.AddCoin(c);
        }
        else
            coin.transform.parent.gameObject.SetActive(false);

        if(d > 0){
            diamon.text = d.ToString();
            GameManager.instance.AddDiamond(d);
        }
        else
            diamon.transform.parent.gameObject.SetActive(false);        
    }

    public void OnHome(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }

    public void Replay(){
        MageManager.instance.PlaySoundName("BubbleButton", false);
        int price = Mathf.Min(GameManager.instance.myPlayer.minigameLevels[0], 10); 
        if(GameManager.instance.GetHappy() >= price){
            GameManager.instance.AddHappy(-price);
            MageManager.instance.LoadScene(SceneManager.GetActiveScene().name,0.5f);
            this.GetComponent<Popup>().Close();
        }else
        {
            MageManager.instance.OnNotificationPopup ("Bạn không đủ tim để chơi trò chơi này hãy chăm sóc thú cưng để thu thập thêm tim nhé.");
        }
    }

    public void Close(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }
}
