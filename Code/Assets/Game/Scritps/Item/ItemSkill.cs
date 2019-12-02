using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkill : MonoBehaviour
{
    public SkillType skillType;
    public ActionType actionType;
    public InteractType interactType;
    public EnviromentType enviromentType;
    bool isEnter = false;
    bool isActive = false;
    CharController character;

    float time = 0;
    public float maxTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnter && isActive && character != null && skillType == character.currentSkill){

            bool done = true;
            if(actionType != ActionType.None && actionType != character.actionType){
                done = false;
                return;
            }

            if(actionType != ActionType.None && actionType != character.actionType){
                done = false;
                return;
            }

           if(interactType != InteractType.None && interactType != character.charInteract.interactType){
                done = false;
                return;
            }

            if(enviromentType != EnviromentType.Room && enviromentType != character.enviromentType){
                done = false;
                return;
            }
            
            if(done){
                CompleteSkill();
            }
        }else if(skillType == SkillType.Call && !isActive){
            if(time > maxTime){
                GameObject.Destroy(this.gameObject);
                return;
            }else
                time += Time.deltaTime;
        }else if(isActive){
            if(time > maxTime){
                if(character != null)
                    character.OffLearnSkill();
                DeActive();
                return;
            }else
                time += Time.deltaTime;
        }
    }

    public void OnActive(float t){
        maxTime = t;
        isActive = true;
    }

    public void DeActive(){
        isActive = false;
        isEnter = false;
        time = 0;
        if(skillType == SkillType.Call)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    void CompleteSkill(){
        DeActive();
        if(character != null){
            character.LevelUpSkill(this.skillType);
        }
    }

    void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player") {
            character = other.GetComponent<CharController>();
            isEnter = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            isEnter = false;
		}
	}
}
