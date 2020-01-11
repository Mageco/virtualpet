using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;
using UnityEngine.UI;

public class PetCollectionPanel : MonoBehaviour
{
    public Text pageNumber;
    int currentPage = 1;
    PetCollectionUI[] petCollectionUIs;
    public SimpleScrollSnap simpleScrollSnap;

    private void Awake()
    {
        
       
    }

    // Start is called before the first frame update
    public void Load()
    {
        petCollectionUIs = this.transform.GetComponentsInChildren<PetCollectionUI>(true);
        for (int i = 0; i < DataHolder.Pets().GetDataCount(); i++)
        {
            petCollectionUIs[i].Load(DataHolder.Pet(i).iD);
        }
        //simpleScrollSnap.startingPanel = 0;
        //simpleScrollSnap.startingPanel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        pageNumber.text = (simpleScrollSnap.CurrentPanel + 1).ToString() + "/3";
    }


    public void OnActive(int id)
    {
        for(int i = 0; i < petCollectionUIs.Length; i++)
        {
            if(petCollectionUIs[i].petId == id)
            {
                simpleScrollSnap.startingPanel = i / 4;
                simpleScrollSnap.GoToPanel(i / 4);
                Debug.Log(i / 4);
                petCollectionUIs[i].OnActive();
            }
        }
    }




    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
