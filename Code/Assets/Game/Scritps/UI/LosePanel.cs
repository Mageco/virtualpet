using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LosePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
        MageManager.instance.LoadScene(SceneManager.GetActiveScene().name,0.5f);
        this.GetComponent<Popup>().Close();
    }
}
