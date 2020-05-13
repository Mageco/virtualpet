using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel : MonoBehaviour
{
    public Transform anchor;
    [HideInInspector]
    public List<ProfileUI> items = new List<ProfileUI>();
    public GameObject itemUIPrefab;
    
    
    // Start is called before the first frame update

    void Awake(){

       

    }
    void Start()
    {
        Load();
    }

    
    // Update is called once per frame
    void Update()
    {

    }

    public void Load()
    {
        ClearItems();
        foreach (PlayerPet p in GameManager.instance.GetPets())
        {
            if(p.itemState != ItemState.OnShop)
                LoadItem(p);
        }
    }


    void LoadItem(PlayerPet data){
        GameObject go = Instantiate(itemUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        ProfileUI item = go.GetComponent<ProfileUI>();
        items.Add(item);
        item.Load(data);
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
