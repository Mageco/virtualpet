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
    public TextMesh valueText;

    void Awake()
    {
        int n = Random.Range(0, 100);

        if (n < 70)
        {
            priceType = PriceType.Coin;
            value = Random.Range(20, 30);
        }else if(n < 90)
        {
            priceType = PriceType.Coin;
            value = Random.Range(30, 50);
        }
        else
        {
            priceType = PriceType.Diamond;
            value = Random.Range(1, 3);
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
        if(!isActive)
            StartCoroutine(ActiveCoroutine());
    }

    IEnumerator ActiveCoroutine()
    {
        isActive = true;
        valueText.text = "+" + value.ToString();
        MageManager.instance.PlaySoundName("Tinerbell", false);
        if (priceType == PriceType.Coin)
        {
            animator.Play("Active_Coin",0);
            GameManager.instance.AddCoin(value);
        }
        else if (priceType == PriceType.Happy)
        {
            animator.Play("Active_Happy",0);
            GameManager.instance.AddHappy(value);
        }
        else if (priceType == PriceType.Diamond)
        {
            animator.Play("Active_Diamond", 0);
            GameManager.instance.AddDiamond(value);
        }
        
        
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
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
