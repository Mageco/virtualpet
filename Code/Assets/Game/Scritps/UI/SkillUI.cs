using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    int iD = 0;
    public Image icon;
    public Text skillName;
    public Text skillDescription;
    public Text progress;
    public Image slider;
    public Image rewardIcon;
    public Sprite coinIcon;
    public Sprite diamonIcon;
    public Text rewardValue;
    
    public Button collectButton;

    public GameObject rewardPanel;


    void Awake(){

    }

    // Start is called before the first frame update
    public void Load(PetSkill skill)
    {
        Skill d = DataHolder.Skills().GetSkill(skill.skillId);
        iD = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/","");
        url = url.Replace(".png","");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        skillName.text = d.GetName(0);
        skillDescription.text = d.GetDescription(0);
        progress.text = skill.level.ToString() + "/" + d.maxProgress.ToString();
        slider.fillAmount = skill.level * 1f/d.maxProgress;
        if(d.coinValue != 0){
            rewardIcon.sprite = coinIcon;
            rewardValue.text = d.coinValue.ToString();
        }else if(d.diamondValue != 0){
            rewardIcon.sprite = diamonIcon;
            rewardValue.text = d.diamondValue.ToString();           
        }

        if(skill.rewardState == RewardState.Received){
            rewardPanel.SetActive(false);
        }else
        {
            rewardPanel.SetActive(true);
            if(skill.rewardState == RewardState.Ready){
                collectButton.interactable = true;
            }else
                collectButton.interactable = false;
        }

    }

    public void Collect(){
        GameManager.instance.CollectSkillRewards(iD);
        StartCoroutine(CollectCoroutine());
    }

    IEnumerator CollectCoroutine(){
        collectButton.interactable = false;
        rewardPanel.GetComponent<Animator>().Play("Active");
        yield return new WaitForSeconds(2);
        rewardPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
