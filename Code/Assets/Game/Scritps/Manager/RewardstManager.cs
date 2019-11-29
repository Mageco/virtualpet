using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class RewardstManager : MonoBehaviour
{
    public static RewardstManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void LevelUp()
    {
        QuestPanel questPanel = UIManager.instance.OnQuestCompletePopup();
        questPanel.Load(GameManager.instance.GetPet(0).level);
        Debug.Log("Quest Complete");
    }

    public void AddReward()
    {
        if (DataHolder.Quest(GameManager.instance.GetPet(0).level).haveItem)
        {
            ApiManager.GetInstance().AddItem(DataHolder.Quest(GameManager.instance.GetPet(0).level).itemId);
            ApiManager.GetInstance().EquipItem(DataHolder.Quest(GameManager.instance.GetPet(0).level).itemId);
            ItemManager.instance.LoadItems();
        }

       ApiManager.GetInstance().AddCoin(DataHolder.Quest(GameManager.instance.GetPet(0).level).coinValue);
       ApiManager.GetInstance().AddDiamond(DataHolder.Quest(GameManager.instance.GetPet(0).level).diamondValue);
       GameManager.instance.GetPet(0).Exp += DataHolder.Quest(GameManager.instance.GetPet(0).level).expValue;
    }

    // Update is called once per frame
}
