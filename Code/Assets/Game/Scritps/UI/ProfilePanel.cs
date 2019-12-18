using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel : MonoBehaviour
{
    public Transform anchor;
    List<ProfileUI> items = new List<ProfileUI>();
    public GameObject itemUIPrefab;
    
    
    // Start is called before the first frame update

    void Awake(){

       

    }
    void Start()
    {   
        ClearItems();
        foreach(Pet p in GameManager.instance.GetPets()){
            LoadItem(p);
        }
    }

    
    // Update is called once per frame
    void Update()
    {

    }



    void LoadItem(Pet data){
        GameObject go = Instantiate(itemUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        ProfileUI item = go.GetComponent<ProfileUI>();
        items.Add(item);
        item.Load(data.iD);
    }

    void ClearItems(){
        foreach(ProfileUI item in items){
            Destroy(item.gameObject);
        }
        items.Clear();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
