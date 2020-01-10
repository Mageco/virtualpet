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
    // Start is called before the first frame update
    void Start()
    {
        MageManager.instance.PlaySoundName("Win", false);
    }

    Animator animator;

    void Awake(){
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(int d, int c){
        animator = this.GetComponent<Animator>();
        animator.Play("Win",0);
        if((GameManager.instance.myPlayer.minigameLevels[0] + 1)% 5 == 0 || GameManager.instance.myPlayer.minigameLevels[0] == 0)
        {
            exp.transform.parent.gameObject.SetActive(true);
            GameManager.instance.AddItem(72);
            GameManager.instance.EquipItem(72);
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
        MageManager.instance.LoadScene(SceneManager.GetActiveScene().name,0.5f);
        this.GetComponent<Popup>().Close();
    }

    public void Close(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }
}
