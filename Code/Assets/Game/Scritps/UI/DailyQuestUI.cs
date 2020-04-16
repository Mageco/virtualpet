using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestUI : MonoBehaviour
{
    int iD = 0;
    public Image icon;
    public Text questName;
    public Text progress;
    public Image slider;
    public Button collect;
    public Text bonus;
    public GameObject collectIcon;
    void Awake()
    {

    }

    // Start is called before the first frame update
    public void Load(DailyQuestData a)
    {
        Achivement d = DataHolder.GetAchivement(a.achivementId);
        iD = a.achivementId;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        questName.text = d.GetName(MageManager.instance.GetLanguage()) + " " + a.requireValue.ToString() + " " + DataHolder.Dialog(138).GetName(MageManager.instance.GetLanguage());
        progress.text = Mathf.Clamp(a.value,0,a.requireValue).ToString() + "/" + a.requireValue.ToString();
        slider.fillAmount = a.value * 1f / a.requireValue;
        bonus.text = a.bonus.ToString();
        if (a.state == DailyQuestState.Ready)
        {
            collect.interactable = true;
        }
        else
            collect.interactable = false;

        if (a.state == DailyQuestState.Collected)
            collectIcon.SetActive(true);
        else
            collectIcon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Collect()
    {
        MageManager.instance.PlaySound("Collect_Achivement", false);
        GameManager.instance.CompleteDailyQuest(iD);
        collect.interactable = false;
        UIManager.instance.dailyQuestPanel.Load();
    }
}
