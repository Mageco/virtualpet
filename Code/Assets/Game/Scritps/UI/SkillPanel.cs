using System.Collections;
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

    public void Load(){
        List<Skill> items = new List<Skill>();

        ClearItems();
        for(int i=0;i<DataHolder.Skills().GetDataCount();i++){
            items.Add(DataHolder.Skill(i));
        }   

        items.Sort((p1,p2)=>(p1.skillOrder).CompareTo(p2.skillOrder));

        for(int i=0;i<items.Count;i++){
            LoadItem(items[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    void LoadItem(Skill data){
        GameObject go = Instantiate(skillUIPrefab);
       
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        SkillUI skill = go.GetComponent<SkillUI>();
        skills.Add(skill);
        skill.Load(data);
    }

    void ClearItems(){
        foreach(SkillUI s in skills){
            Destroy(s.gameObject);
        }
        skills.Clear();
    }

}

