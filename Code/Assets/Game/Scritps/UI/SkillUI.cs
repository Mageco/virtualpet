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
    public AnimatedButton collectbutton;
    public GameObject collectedButton;

    void Awake(){
        collectbutton.gameObject.SetActive(false);
        collectedButton.SetActive(false);
    }

    // Start is called before the first frame update
    public void Load(Skill d)
    {
        iD = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/","");
        url = url.Replace(".png","");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        skillName.text = d.GetName(0);
        skillDescription.text = d.GetDescription(0);
        progress.text = GameManager.instance.GetPetObject(0).data.GetSkillProgress(d.skillType).ToString() + "/" + d.maxProgress.ToString();
        slider.fillAmount = GameManager.instance.GetPetObject(0).data.GetSkillProgress(d.skillType) * 1f/d.maxProgress;
        if(d.coinValue != 0){
            rewardIcon.sprite = coinIcon;
            rewardValue.text = d.coinValue.ToString();
        }else if(d.diamondValue != 0){
            rewardIcon.sprite = diamonIcon;
            rewardValue.text = d.diamondValue.ToString();           
        }else if(d.itemId != -1){
            string itemurl = DataHolder.Item(d.itemId).iconUrl.Replace("Assets/Game/Resources/","");
            itemurl = itemurl.Replace(".png","");
            rewardIcon.sprite = Resources.Load<Sprite>(itemurl) as Sprite;
            rewardValue.text = "1";
        }
    }

    public void Done(){
        collectbutton.gameObject.SetActive(true);
    }

    public void Collect(){
        collectbutton.gameObject.SetActive(false);
        collectedButton.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
