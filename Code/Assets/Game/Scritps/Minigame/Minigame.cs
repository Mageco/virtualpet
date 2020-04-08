using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    public int gameLevel = 1;
    public static Minigame instance;
    public int maxLive;
    public int live;
    public float time = 0;
    public float maxTime;
    public GameObject endGamePrefab;
    public Vector2 boundX;
    public Vector2 boundY;
    public int exp = 1; 
    int zIndex = 0;
    public int minigameId = 0;
    public GameObject coinPrefab;
    public GameState state = GameState.Ready;

    public Text coinText;
    [HideInInspector]
    public WinPanel winPanel;
    public int bonus = 1;

    void Awake(){
        if(instance == null){
            instance = this;
        }
        float d = Camera.main.orthographicSize * (float)Screen.width / (float)Screen.height;
        boundX.x = -d;
        boundX.y = d;
        gameLevel = GameManager.instance.GetPlayer().minigameLevels[minigameId];
    }

        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //Debug.Log(bonus);
        coinText.text = bonus.ToString();
    }

    public virtual void UpdateLive(){

    }


    public virtual void OnEndGame(bool isWin){
        if (winPanel == null)
        {
            var popup = Instantiate(endGamePrefab) as GameObject;
            popup.SetActive(true);
            //popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            winPanel = popup.GetComponent<WinPanel>();
            if (isWin)
                bonus += 20 * (1 + gameLevel/5);

            if (bonus == 0)
                bonus = 1;
            winPanel.Load(bonus,minigameId, isWin);
            GameManager.instance.LogAchivement(AchivementType.Play_MiniGame, ActionType.None, minigameId);
        }

        if (isWin)
        {
            GameManager.instance.GetPlayer().minigameLevels[minigameId]++;
        }
    }

    public virtual void OnEndGame(int score)
    {
        if (winPanel == null)
        {
            var popup = Instantiate(endGamePrefab) as GameObject;
            popup.SetActive(true);
            //popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            winPanel = popup.GetComponent<WinPanel>();
            bonus += score/50;
            if(GameManager.instance.GetPlayer().minigameLevels[minigameId] < score)
                GameManager.instance.GetPlayer().minigameLevels[minigameId] = score;
            if (bonus == 0)
                bonus = 1;
            winPanel.Load(bonus, minigameId, false);
            GameManager.instance.LogAchivement(AchivementType.Play_MiniGame, ActionType.None, minigameId);
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

    public int GetZindex()
    {
        zIndex++;
        return zIndex;
    }

    public void SpawnCoin(Vector3 pos, int value, GameObject item = null)
    {
        GameObject go = Instantiate(coinPrefab, pos, Quaternion.identity);
        go.GetComponent<CoinItem>().Load(value);
        if (item != null)
        {
            go.transform.parent = item.transform;
        }
    }
}
