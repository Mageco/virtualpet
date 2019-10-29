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

    // Start is called before the first frame update
    public void Load(Skill d)
    {
        iD = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/","");
        url = url.Replace(".png","");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        skillName.text = d.GetName(0);
        skillDescription.text = d.GetDescription(0);
        progress.text = InputController.instance.character.data.GetSkillProgress(d.skillType).ToString() + "/" + d.maxProgress.ToString();
        slider.fillAmount = InputController.instance.character.data.GetSkillProgress(d.skillType) * 1f/d.maxProgress;
    }



    // Update is called once per frame
    void Update()
    {

    }
}
