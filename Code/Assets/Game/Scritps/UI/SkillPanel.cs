﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    public Transform anchor;
    List<SkillUI> skills = new List<SkillUI>();
    public GameObject skillUIPrefab;
   
    // Start is called before the first frame update
    void Start()
    {
        Load();
    }

    void Load(){
        ClearItems();
        for(int i=0;i<DataHolder.Skills().GetDataCount();i++){
            LoadItem(DataHolder.Skill(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(UIManager.instance.notification == NotificationType.SkillLevelUp){
            Load();
            UIManager.instance.notification = NotificationType.None;
        }
    }

    

    void LoadItem(Skill data){
        GameObject go = GameObject.Instantiate(skillUIPrefab);
       
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        SkillUI skill = go.GetComponent<SkillUI>();
        skills.Add(skill);
        skill.Load(data);
    }

    void ClearItems(){
        foreach(SkillUI s in skills){
            GameObject.Destroy(s.gameObject);
        }
        skills.Clear();
    }

}

