﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public int gameLevel = 1;
    public static Minigame instance;
    public int maxLive;
    public int live;
    protected float time = 0;
    public float maxTime;

    public GameObject winPrefab;
    public GameObject losePrefab;
    public Vector2 boundX;
    public Vector2 boundY;
    public int exp = 1; 
    public int coin = 10;
    public int diamon = 0;

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
        gameLevel = GameManager.instance.myPlayer.minigameLevels[0];
    }

        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        time += Time.deltaTime;
        if(time >= maxTime && state == GameState.Run){
            EndGame();
            if(live == maxLive){
                OnWin(3);
                GameManager.instance.myPlayer.minigameLevels[0] ++;
                GameManager.instance.LogAchivement(AchivementType.Minigame_Level);
            }else if(live == maxLive - 1)
            {
                OnWin(2);
            }else{
                OnWin(1);
            }
            Debug.Log("Win");
        }
    }

    public virtual void UpdateLive(){

    }


    public virtual void OnWin(int star){
        if (winPanel == null)
        {
            var popup = Instantiate(winPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            winPanel = popup.GetComponent<WinPanel>();
            winPanel.Load(star,(gameLevel+1)*star*exp,(gameLevel+1)*star*diamon,(gameLevel+1)*star*coin);
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

    }
}
