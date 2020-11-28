using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChestItem : MonoBehaviour
{
    public int id = 0;
    RewardType rewardType = RewardType.Chest;
    [HideInInspector]
    public PriceType priceType = PriceType.Coin;
    [HideInInspector]
    public int value = 0;
    Animator animator;
    bool isActive = false;
    public TextMesh valueText;
    Vector3 clickPosition;
    int itemId = 0;
    void Awake()
    {
        /*
        int n = Random.Range(0, 100);

        int ran = Random.Range(0, 100);
        if (ran < 40)
        {
            itemId = 0;
            value = Random.Range(2,4);
        }
        else if (ran < 60)
        {
            itemId = 239;
            value = Random.Range(1, 3);
        }
        else if (ran < 80)
        {
            itemId = 240;
            value = Random.Range(1, 3);
        }
        else
        {
            itemId = 241;
            value = Random.Range(1, 3);
        }*/
        animator = this.GetComponent<Animator>();

        itemId = 0;
        value = Random.Range(1,4);
    }
    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = this.transform.position;
        pos.z = 10 * pos.y;
        this.transform.position = pos;
    }

    public void Load()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnMouseDown()
    {
        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        Debug.Log("click");
        if (IsPointerOverUIObject())
            return;

        if(!isActive && Vector3.Distance(clickPosition, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 1f)
            Pick();
    }

    void Pick()
    {
        MageManager.instance.PlaySound("collect_item_03", false);
        UIManager.instance.OnRewardItemPanel(rewardType, this);
    }

    public void OnActive()
    {
        if(!isActive)
            StartCoroutine(ActiveCoroutine());
    }

    IEnumerator ActiveCoroutine()
    {
        isActive = true;
        valueText.text = "+" + value.ToString();
        MageManager.instance.PlaySound3D("Tinerbell", false,this.transform.position);
        /*
        if (priceType == PriceType.Coin)
        {
            animator.Play("Active_Coin",0);
            GameManager.instance.AddCoin(value, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        }
        else if (priceType == PriceType.Happy)
        {
            animator.Play("Active_Happy",0);
            GameManager.instance.AddHappy(value, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        }
        else if (priceType == PriceType.Diamond)
        {
        }*/
        animator.Play("Active_Diamond", 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        if (itemId == 0)
            GameManager.instance.AddDiamond(value, GetKey());
        else
        {
            Item item = DataHolder.GetItem(itemId);
            string url = item.iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            UIManager.instance.OnSpinRewardPanel(Resources.Load<Sprite>(url), value.ToString(),item.GetName(MageManager.instance.GetLanguage()));
            GameManager.instance.AddItem(itemId, value, GetKey());
        }
        Destroy(this.gameObject);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    string GetKey()
    {
        return Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013");
    }
}
