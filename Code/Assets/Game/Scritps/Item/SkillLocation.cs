﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLocation : MonoBehaviour
{
    public SkillType skillType;
    public GameObject skillEffect;
    bool isEnter = false;
    // Start is called before the first frame update
    void Start()
    {
        skillEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(InputController.instance.Character.currentSkill == skillType){
            skillEffect.SetActive(true);
            if(isEnter && InputController.instance.Character.charInteract.interactType != InteractType.Drop
            && InputController.instance.Character.charInteract.interactType != InteractType.Drag)
            {
                InputController.instance.Character.LevelUpSkill(skillType);
            }
        }else
            skillEffect.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player") {
            //Debug.Log("Enter");
            isEnter = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            //Debug.Log("Enter");
            isEnter = false;
		}
	}
}
