using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCompletePanel : MonoBehaviour
{
    int skillId = 0;
    public Text coin;
    public Text diamond;
    public Text exp;
    public Image skillIcon;
    Animator anim;
   
    void Awake(){
        anim = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Load(SkillType type)
    {
        for(int i = 0;i<DataHolder.Skills().GetDataCount();i++){
            if(DataHolder.Skill(i).skillType == type){
                skillId = DataHolder.Skill(i).iD;
                break;
            }
        }
        

  
        string url = DataHolder.GetSkill(skillId).iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        skillIcon.sprite = Resources.Load<Sprite>(url) as Sprite;
        


        if (DataHolder.GetSkill(skillId).coinValue != 0)
        {
            coin.text = DataHolder.GetSkill(skillId).coinValue.ToString();
            GameManager.instance.AddCoin(DataHolder.GetSkill(skillId).coinValue);
        }
        else
        {
            coin.transform.parent.gameObject.SetActive(false);
        }

        if (DataHolder.GetSkill(skillId).expValue != 0)
        {
            exp.text = DataHolder.GetSkill(skillId).expValue.ToString();
            GameManager.instance.AddExp(DataHolder.GetSkill(skillId).expValue);
        }
        else
        {
            exp.transform.parent.gameObject.SetActive(false);
        }

        if (DataHolder.GetSkill(skillId).diamondValue != 0)
        {
            diamond.text = DataHolder.GetSkill(skillId).diamondValue.ToString();
            GameManager.instance.AddDiamon(DataHolder.GetSkill(skillId).diamondValue);
        }
        else
        {
            diamond.transform.parent.gameObject.SetActive(false);
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
        anim.Play("Close");    
        yield return new WaitForEndOfFrame();
        this.GetComponent<Popup>().Close();
        yield return new WaitForSeconds(0.1f);
    }
}
