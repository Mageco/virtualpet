using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame4 : Minigame
{
    public Text scoreText;
    public int score = 0;
    public Animator animator;
    public GameObject arrowPrefab;
    public GameObject arrow;
    public Transform arrowAnchor;
    ArrowItem arrowItem;
    public float speed = 10;
    public BowState bowState = BowState.Idle;
    public GameObject[] ballPrefabs;
    List<BallFlyItem> balls = new List<BallFlyItem>();
    public GameObject guideUIPrefab;
    float timeDuration = 0;
    float timeSpawn = 0;
    float maxY = 3;
    float minY = 0;
    GuideUI guildUI;
    public Animator bow;


    protected override void Start()
    {
        base.Start();
        float ratio = Screen.height * 1f / Screen.width;
        Debug.Log(ratio);
        Camera.main.orthographicSize = 15 * ratio;
        Camera.main.transform.position += new Vector3(0, Camera.main.orthographicSize - 7, 0);
        maxY = Camera.main.orthographicSize + 1 + Camera.main.transform.position.y;
        minY = Camera.main.transform.position.y - Camera.main.orthographicSize - 1;
        if (GameManager.instance.myPlayer.minigameLevels[minigameId] == 0)
            OnGuildPanel();
        else
            StartGame();

        MageManager.instance.PlayMusicName("Minigame4", true);
    }

    public void Arm()
    {
        if (state != GameState.Run)
            return;

        if (bowState != BowState.Idle)
            return;
        bowState = BowState.Arm;
    }

    public void Shoot()
    {
        if (state != GameState.Run)
            return;

        if (bowState != BowState.Arm)
            return;

        StartCoroutine(ShotCoroutine());
    }

    IEnumerator ShotCoroutine()
    {
        bowState = BowState.Shoot;
        yield return new WaitForEndOfFrame();
        arrow.GetComponent<SpriteRenderer>().enabled = false;
        GameObject go = GameObject.Instantiate(arrowPrefab) as GameObject;
        go.transform.position = arrowAnchor.position;
        go.transform.rotation = arrowAnchor.rotation;
        arrowItem = go.GetComponent<ArrowItem>();
        Debug.Log(go.transform.rotation.eulerAngles.z);
        arrowItem.Load(speed, go.transform.rotation.eulerAngles.z / 180 * Mathf.PI);
        animator.speed = 0;
        bow.Play("Active", 0);
        MageManager.instance.PlaySound("Throw", false);
        yield return new WaitForSeconds(0.5f);
        animator.speed = 1;
        bowState = BowState.Idle;
        arrow.GetComponent<SpriteRenderer>().enabled = true;
    }

    protected override void Update()
    {
        base.Update();
        scoreText.text = score.ToString();
        if(bowState == BowState.Idle)
        {
            animator.Play("Idle", 0);
        }else if(bowState == BowState.Arm)
        {
            animator.Play("Arm", 0);
        }else if(bowState == BowState.Shoot)
        {
            
        }

        time += Time.deltaTime;

        if(state == GameState.Run)
        {
            if (timeSpawn > timeDuration)
            {
                timeSpawn = 0;
                SpawnBall();
                timeDuration = Random.Range(0.5f,2f);
            }
            else
                timeSpawn += Time.deltaTime;
        }

    }

    public void SpawnBall()
    {
        int id = Random.Range(0, ballPrefabs.Length);
        GameObject go = GameObject.Instantiate(ballPrefabs[id]) as GameObject;
        go.transform.position = new Vector3(Random.Range(-1, 10), minY, 0);
        BallFlyItem ball = go.GetComponent<BallFlyItem>();
        float speed = Random.Range(2,3) + time/50;
        ball.Load(speed,maxY);
        balls.Add(ball);
    }

    public void RemoveBall(BallFlyItem ball)
    {
        if (balls.Contains(ball))
        {
            balls.Remove(ball);
            Destroy(ball.gameObject);
        }
    }

    public void OnExplode()
    {
        foreach(BallFlyItem ball in balls)
        {
            ball.Explode();
        }
    }

    public void OnReduceSpeed()
    {
        foreach (BallFlyItem ball in balls)
        {
            ball.ReduceSpeed();
        }
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

public enum BowState {Idle,Arm,Shoot}
