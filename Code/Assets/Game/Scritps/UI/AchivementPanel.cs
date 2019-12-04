using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementPanel : MonoBehaviour
{
public Transform anchor;
    List<AchivementUI> items = new List<AchivementUI>();
    public GameObject achivementUIPrefab;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Load(){
        List<PlayerAchivement> items = new List<PlayerAchivement>();

        ClearItems();
        foreach(PlayerAchivement a in GameManager.instance.myPlayer.achivements){
            items.Add(a);
        }   

        items.Sort((p1,p2)=>(p1.order).CompareTo(p2.order));

        for(int i=0;i<items.Count;i++){
            LoadItem(items[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    void LoadItem(PlayerAchivement data){
        GameObject go = Instantiate(achivementUIPrefab);      
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        AchivementUI item = go.GetComponent<AchivementUI>();
        items.Add(item);
        item.Load(data);
    }

    void ClearItems(){
        foreach(AchivementUI s in items){
            Destroy(s.gameObject);
        }
        items.Clear();
    }
}
