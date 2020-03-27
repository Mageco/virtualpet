using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : MonoBehaviour
{
    public Transform anchor;
    public GameObject itemPrefab;
    public Text coinText;
    public Text diamonText;
    public Text levelText;
    int coin = 0;
    int diamond = 0;
    List<GameObject> icons = new List<GameObject>();
    Animator anim;


    void Awake()
    {
        anim = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Load()
    {
        MageManager.instance.PlaySound("Win", false);
        int level = GameManager.instance.myPlayer.level;
        levelText.text = level.ToString();
        coin = 100;
        diamond = 5;
        coinText.text = coin.ToString();
        diamonText.text = diamond.ToString();
        List<Item> items = new List<Item>();

        for(int i = 0; i < DataHolder.Items().GetDataCount(); i++){
            if(DataHolder.Item(i).isAvailable && DataHolder.Item(i).levelRequire == level)
            {
                GameObject go = GameObject.Instantiate(itemPrefab) as GameObject;
                go.transform.SetParent(this.anchor);
                go.transform.localScale = Vector3.one;
                string url = DataHolder.Item(i).iconUrl.Replace("Assets/Game/Resources/", "");
                url = url.Replace(".png", "");
                go.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(url) as Sprite;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Close()
    {
        MageManager.instance.PlaySound("Collect_Achivement", false);
        GameManager.instance.AddCoin(coin);
        GameManager.instance.AddDiamond(diamond);
        this.GetComponent<Popup>().Close();
    }
}
