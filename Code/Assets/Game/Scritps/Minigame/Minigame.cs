using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public int gameLevel = 1;
    public static Minigame instance;
    public int maxLive;
    public int live;
    public float time = 0;
    public float maxTime;

    public GameObject winPrefab;
    public GameObject losePrefab;
    public Vector2 boundX;
    public Vector2 boundY;
    public int exp = 1; 
    public int coin = 10;
    public int diamon = 0;
    int zIndex = 0;

    public GameState state = GameState.Ready;

    WinPanel winPanel;
    LosePanel losePanel;

    void Awake(){
        if(instance == null){
            instance = this;
        }
        float d = Camera.main.orthographicSize * (float)Screen.width / (float)Screen.height;
        boundX.x = -d;
        boundX.y = d;
        gameLevel = GameManager.instance.GetPlayer().minigameLevels[0];
    }

        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public virtual void UpdateLive(){

    }


    public virtual void OnWin(){
        if (winPanel == null)
        {
            var popup = Instantiate(winPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            winPanel = popup.GetComponent<WinPanel>();
            winPanel.Load((gameLevel+1)*diamon,(gameLevel+1)*3*coin);
        }
    }

    public virtual void OnLose(){
        if (losePanel == null)
        {
            var popup = Instantiate(losePrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            losePanel = popup.GetComponent<LosePanel>();
        }
    }

    public bool IsInBound(Vector3 pos){
        if(pos.x > boundX.x && pos.x < boundX.y && pos.y > boundY.x && pos.y < boundY.y){
            return true;
        }else
            return false;
    }

    public Vector3 GetPointInBound(){
        float x = Random.Range(Minigame.instance.boundX.x, Minigame.instance.boundX.y);
        float y = Random.Range(Minigame.instance.boundY.x, Minigame.instance.boundY.y);
        return new Vector3(x, y, 0);
    }

    public virtual void StartGame(){

    }

    public void OnHome(){
        UIManager.instance.OnHome();
    }

    public virtual void EndGame(){
        GameManager.instance.LogAchivement(AchivementType.Play_MiniGame);
    }

    public int GetZindex()
    {
        zIndex++;
        return zIndex;
    }
}
