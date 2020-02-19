using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetRequirementPanel : MonoBehaviour
{
    int petId = 0;
    public Image petAvatar;
    public Text petName;
    public Text petDescription;
    public Text petPrice;
    public Text requireText;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject happyIcon;
    public Button buyButton;
    public bool canBuy = true;
    bool isBuy = false;

    public Transform anchor;
    List<PetRequirementUI> items = new List<PetRequirementUI>();
    public GameObject petRequirementUIPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Load(Pet pet)
    {
        petId = pet.iD;
        string url = pet.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        petAvatar.sprite = Resources.Load<Sprite>(url) as Sprite;
        //petPrice.text = pet.buyPrice.ToString();
        petName.text = pet.GetName(MageManager.instance.GetLanguage());
        petDescription.text = pet.GetDescription(MageManager.instance.GetLanguage());
        requireText.text = DataHolder.Dialog(15).GetDescription(MageManager.instance.GetLanguage());

        for (int i = 0; i < pet.requirePets.Length; i++)
        {
            LoadPet(pet.requirePets[i]);
        }

        for (int i = 0; i < pet.requireEquipments.Length; i++)
        {
            LoadEquipment(pet.requireEquipments[i]);
        }


        coinIcon.SetActive(false);
        diamonIcon.SetActive(false);
        happyIcon.SetActive(false);
        /*
        if (pet.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            happyIcon.SetActive(false);
            if (GameManager.instance.GetCoin() < (DataHolder.GetPet(petId).buyPrice))
            {
                canBuy = false;
            }
            else
            {
                coinIcon.GetComponent<PetRequirementUI>().done.SetActive(true);
            }
            coinIcon.GetComponent<PetRequirementUI>().number.text = GameManager.instance.GetCoin().ToString() + "/" + pet.buyPrice.ToString();
            
        }
        else if (pet.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            happyIcon.SetActive(false);
            if (GameManager.instance.GetDiamond() < (DataHolder.GetPet(petId).buyPrice))
            {
                canBuy = false;
            }
            else
            {
                diamonIcon.GetComponent<PetRequirementUI>().done.SetActive(true);
            }
            diamonIcon.GetComponent<PetRequirementUI>().number.text = GameManager.instance.GetDiamond().ToString() + "/" + pet.buyPrice.ToString();
        }
        else if (pet.priceType == PriceType.Happy)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            happyIcon.SetActive(true);
            if (GameManager.instance.GetHappy() < (DataHolder.GetPet(petId).buyPrice))
            {
                canBuy = false;
            }
            else
            {
                happyIcon.GetComponent<PetRequirementUI>().done.SetActive(true);
            }
            happyIcon.GetComponent<PetRequirementUI>().number.text = GameManager.instance.GetHappy().ToString() + "/" + pet.buyPrice.ToString();


        }*/

        if (canBuy)
        {
            buyButton.interactable = true;
        }
        else
            buyButton.interactable = false;

    }

    // Update is called once per frame
    void Update()
    {

    }



    void LoadEquipment(int id)
    {
        GameObject go = Instantiate(petRequirementUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        PetRequirementUI item = go.GetComponent<PetRequirementUI>();
        string url = DataHolder.GetItem(id).iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        if (GameManager.instance.IsHaveItem(id) || GameManager.instance.IsEquipItem(id))
        {
            item.Load(url, 1, 1);
        }
        else
        {
            item.Load(url, 0, 1);
            canBuy = false;
        }
            
        
        items.Add(item);
        
    }

    void LoadPet(int id)
    {
        GameObject go = Instantiate(petRequirementUIPrefab);

        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        PetRequirementUI item = go.GetComponent<PetRequirementUI>();
        string url = DataHolder.GetItem(id).iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        if (GameManager.instance.IsEquipPet(id))
        {
            item.Load(url, 1, 1);
        }
        else
        {
            item.Load(url, 0, 1);
            canBuy = false;
        }
        items.Add(item);
        
    }

    public void OnCollect()
    {
        if (!isBuy)
        {
            GameManager.instance.AddPet(petId);
            GameManager.instance.EquipPet(petId);
            isBuy = true;
            if (ItemManager.instance.GetCharCollector(petId) != null)
                ItemManager.instance.GetCharCollector(petId).DeActive();
            this.Close();
        }

    }

    void ClearItems()
    {
        foreach (PetRequirementUI s in items)
        {
            Destroy(s.gameObject);
        }
        items.Clear();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
