using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChestItem : MonoBehaviour
{
    [HideInInspector]
    public ItemSaveDataType itemSaveDataType = ItemSaveDataType.Chest;
    public int id = 0;
    RewardType rewardType = RewardType.Chest;
    PriceType priceType = PriceType.Coin;
    int value = 0;
    Animator animator;
    bool isActive = false;

    void Awake()
    {
        int n = Random.Range(0, 100);

        if (n < 50)
        {
            priceType = PriceType.Coin;
            value = Random.Range(10, 20);
        }
        else if (n < 95)
        {
            priceType = PriceType.Happy;
            value = Random.Range(20, 30);
        }
        else
        {
            priceType = PriceType.Diamond;
            value = 1;
        }

        animator = this.GetComponent<Animator>();
            
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Load()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

    }

    void OnMouseUp()
    {
        Debug.Log("click");
        if (IsPointerOverUIObject())
            return;

        if(!isActive)
            Pick();
    }

    void Pick()
    {
        MageManager.instance.PlaySoundName("collect_item_03", false);
        UIManager.instance.OnRewardItemPanel(rewardType, this);
    }

    public void OnActive()
    {
        isActive = true;
        StartCoroutine(ActiveCoroutine());
    }

    IEnumerator ActiveCoroutine()
    {
        MageManager.instance.PlaySoundName("Tinerbell", false);
        animator.Play("Active");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        if(priceType == PriceType.Coin)
        {
            GameManager.instance.AddCoin(value);
        }else if(priceType == PriceType.Happy)
        {
            GameManager.instance.AddHappy(value);
        }else if(priceType == PriceType.Diamond)
        {
            GameManager.instance.AddDiamond(value);
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
}
