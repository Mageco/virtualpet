using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetCollectionPanel : MonoBehaviour
{
    public Text pageNumber;
    int currentPage = 1;
    PetCollectionUI[] petCollectionUIs;

    private void Awake()
    {
        petCollectionUIs = this.transform.GetComponentsInChildren<PetCollectionUI>(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < DataHolder.Pets().GetDataCount(); i++)
        {
            petCollectionUIs[i].Load(DataHolder.Pet(i).iD);
        }
    }

    // Update is called once per frame
    void Update()
    {
        pageNumber.text = currentPage.ToString() + "/4";
    }

    public void OnNext()
    {
        currentPage++;
        if (currentPage > 4)
            currentPage = 4;
    }

    public void OnPrevious()
    {
        currentPage--;
        if (currentPage < 1)
            currentPage = 1;
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
