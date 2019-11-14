using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestPanel : MonoBehaviour
{
    int questId = 0;
    public Image exp;
    public Image coin;
    public Image diamon;
    public Image item;
    public Animator anim;
    

    void Awake(){
        anim = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        anim.Play("Appear");
    }

    public void Load(int id)
    {
        questId = id;

        if (DataHolder.Quest(questId).haveItem)
        {
            string url = DataHolder.GetItem(DataHolder.Quest(questId).itemId).iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            item.sprite = Resources.Load<Sprite>(url) as Sprite;
        }else
        {
            item.transform.parent.gameObject.SetActive(false);
        }

        if (DataHolder.Quest(questId).expValue != 0)
        {
            exp.GetComponentInChildren<Text>().text = DataHolder.Quest(questId).expValue.ToString();
        }
        else
        {
            exp.transform.parent.gameObject.SetActive(false);
        }

        if (DataHolder.Quest(questId).coinValue != 0)
        {
            coin.GetComponentInChildren<Text>().text = DataHolder.Quest(questId).coinValue.ToString();
        }
        else
        {
            coin.transform.parent.gameObject.SetActive(false);
        }

        if (DataHolder.Quest(questId).diamondValue != 0)
        {
            diamon.GetComponentInChildren<Text>().text = DataHolder.Quest(questId).diamondValue.ToString();
        }
        else
        {
            diamon.transform.parent.gameObject.SetActive(false);
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close(){
        StartCoroutine(CloseCoroutine());        
    }

    IEnumerator CloseCoroutine()
    {
        Debug.Log("Close Quest");
        anim.Play("Disappear");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        QuestManager.instance.EndCompleteQuest();
        this.GetComponent<Popup>().Close();
    }
}
