using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLocation : MonoBehaviour
{
    public SkillType skillType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player") {
            Debug.Log("Enter");
            if(InputController.instance.character.currentSkill == skillType && InputController.instance.character.charInteract.interactType != InteractType.Drop
            && InputController.instance.character.charInteract.interactType != InteractType.Drag){
                InputController.instance.character.LevelUpSkill(skillType);
            }
		}
	}

	void OnTriggerExit2D(Collider2D other) {

	}
}
