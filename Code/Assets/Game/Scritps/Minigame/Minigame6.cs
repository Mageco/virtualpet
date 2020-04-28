using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame6 : Minigame
{
    public Text liveText;
    public int score = 0;
    public float speed = 10;
    public GameObject[] fruitPrefabs;
    List<FruitFallItem> fruits = new List<FruitFallItem>();
    public GameObject guideUIPrefab;
    float timeDuration = 0;
    float timeSpawn = 0;
    float maxY = 3;
    GuideUI guildUI;



    protected override void Start()
    {
        base.Start();
        float ratio = Screen.height * 1f / Screen.width;
        Debug.Log(ratio);
        Camera.main.orthographicSize = 10f * ratio;
        Camera.main.transform.position += new Vector3(0, Camera.main.orthographicSize - 6, 0);
        maxY = Camera.main.orthographicSize + 1;
        //if (GameManager.instance.myPlayer.minigameLevels != null && GameManager.instance.myPlayer.minigameLevels[minigameId] == 0)
        //    OnGuildPanel();
        //else
            StartGame();

        MageManager.instance.PlayMusicName("Minigame4", true);
    }



    protected override void Update()
    {
        base.Update();
        liveText.text = live.ToString();

        time += Time.deltaTime;

        if (state == GameState.Run)
        {
            if (timeSpawn > timeDuration)
            {
                timeSpawn = 0;
                SpawnFruit();
                timeDuration = Random.Range(0.5f, 2f);
            }
            else
                timeSpawn += Time.deltaTime;
        }

    }

    public void SpawnFruit()
    {
        int id = Random.Range(0, fruitPrefabs.Length);
        GameObject go = GameObject.Instantiate(fruitPrefabs[id]) as GameObject;
        go.transform.position = new Vector3(Random.Range(-9f, 9f), maxY, 0);
        FruitFallItem fruit = go.GetComponent<FruitFallItem>();
        fruits.Add(fruit);
    }


    public void OnExplode()
    {
        EndGame();
    }


    public override void StartGame()
    {
        if (state == GameState.Ready)
        {
            state = GameState.Run;
        }
    }

    public override void EndGame()
    {
        state = GameState.End;
        MageManager.instance.StopMusic();
        OnEndGame(bonus);
    }

    public void OnGuildPanel()
    {
        if (guildUI == null)
        {
            var popup = Instantiate(guideUIPrefab) as GameObject;
            popup.SetActive(true);
            //popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            guildUI = popup.GetComponent<GuideUI>();
        }
    }
}
