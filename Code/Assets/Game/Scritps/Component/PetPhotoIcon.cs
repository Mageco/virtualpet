using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetPhotoIcon : MonoBehaviour
{
    public GameObject[] petBasic;
    public List<GameObject> petAccessories = new List<GameObject>();
    int index = 0;

    private void Awake()
    {
        petAccessories = new List<GameObject>();
        for (int i = 0; i < petBasic.Length; i++)
        {
            for(int j = 0; j < 20; j++)
            {
                string name = petBasic[i].name + "_Accessory_" + (j + 1).ToString();
                GameObject go = GameObject.Find(name);
                if (go != null)
                {
                    petAccessories.Add(go);
                    //go.SetActive(false);
                    //Debug.Log(go.name);
                }
            }
        }

        Debug.Log(petAccessories[0].name);
    }

    public bool NextPet()
    {
        //Debug.Log(index);
        //Debug.Log(petAccessories[0].name);
        if (index >= petAccessories.Count)
            return false;
        for(int i = 0; i < petAccessories.Count; i++)
        {
            if (i == index)
                petAccessories[i].SetActive(true);
            else
                petAccessories[i].SetActive(false);
        }
        index++;

        return true;
    }
}
