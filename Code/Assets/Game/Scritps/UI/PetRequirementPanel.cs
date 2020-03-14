using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetRequirementPanel : MonoBehaviour
{
    int petId = 0;
    public Image petAvatar;
    public Text petName;
    public Text petPrice;
    public Text requireText;
    public Button buyButton;
    public bool canBuy = true;
    bool isBuy = false;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject happyIcon;

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

        OffAllIcon();
        if(pet.requireValue > 0)
        {
            petPrice.gameObject.SetActive(true);
            petPrice.text = pet.requireValue.ToString();
            if(pet.requireValueType == PriceType.Coin)
            {
                coinIcon.SetActive(true);
            }else if(pet.requireValueType == PriceType.Happy)
            {
                happyIcon.SetActive(true);
            }
            else if (pet.requireValueType == PriceType.Diamond)
            {
                diamonIcon.SetActive(true);
            }
        }
        
        petName.text = pet.GetName(0);
        //petDescription.text = pet.GetDescription(MageManager.instance.GetLanguage(0));
        requireText.text = DataHolder.Dialog(15).GetDescription(MageManager.instance.GetLanguage());

        for (int i = 0; i < pet.requirePets.Length; i++)
        {
            LoadPet(pet.requirePets[i]);
        }

        for (int i = 0; i < pet.requireEquipments.Length; i++)
        {
            LoadEquipment(pet.requireEquipments[i]);
        }

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

    void OffAllIcon()
    {
        petPrice.gameObject.SetActive(false);
        happyIcon.SetActive(false);
        diamonIcon.SetActive(false);
        coinIcon.SetActive(false);
    }

    public void ReLoad()
    {
        canBuy = true;
        Pet pet = DataHolder.GetPet(petId);
        ClearItems();
        for (int i = 0; i < pet.requirePets.Length; i++)
        {
            LoadPet(pet.requirePets[i]);
        }

        for (int i = 0; i < pet.requireEquipments.Length; i++)
        {
            LoadEquipment(pet.requireEquipments[i]);
        }

        if (canBuy)
        {
            buyButton.interactable = true;
        }
        else
            buyButton.interactable = false;
    }

    void LoadEquipment(int id)
    {
        GameObject go = Instantiate(petRequirementUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        PetRequirementUI item = go.GetComponent<PetRequirementUI>();
        Item i = DataHolder.GetItem(id);
        string url = i.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        if (GameManager.instance.IsHaveItem(id) || GameManager.instance.IsEquipItem(id))
        {
            if(i.itemType == ItemType.Toy)
                item.Load(url, 1, 1,4);
            else if(i.itemType == ItemType.Food || i.itemType == ItemType.Drink)
                item.Load(url, 1, 1, 2);
            else if (i.itemType == ItemType.Fruit)
                item.Load(url, 1, 1, 5);
            else
                item.Load(url, 1, 1, 3);
        }
        else
        {
            if (i.itemType == ItemType.Toy)
                item.Load(url, 0, 1, 4);
            else if (i.itemType == ItemType.Food || i.itemType == ItemType.Drink)
                item.Load(url, 0, 1, 2);
            else if (i.itemType == ItemType.Fruit)
                item.Load(url, 0, 1, 5);
            else
                item.Load(url, 0, 1, 3);

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
        string url = DataHolder.GetPet(id).iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        if (GameManager.instance.IsEquipPet(id))
        {
            item.Load(url, 1, 1,1);
        }
        else
        {
            item.Load(url, 0, 1,1);
            canBuy = false;
        }
        items.Add(item);
        
    }

    public void OnCollect()
    {
        Pet pet = DataHolder.GetPet(petId);
        if(pet.requireValueType == PriceType.Coin && pet.requireValue > GameManager.instance.GetCoin())
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
            return;
        }else if (pet.requireValueType == PriceType.Happy && pet.requireValue > GameManager.instance.GetHappy())
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
            return;
        }
        else if (pet.requireValueType == PriceType.Diamond && pet.requireValue > GameManager.instance.GetDiamond())
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(7).GetDescription(MageManager.instance.GetLanguage()));
            return;
        }

 
        if (!isBuy)
        {
            GameManager.instance.AddPet(petId);
            Pet p = GameManager.instance.GetPet(petId);
            p.isNew = true;
            GameManager.instance.EquipPet(petId);
            isBuy = true;
            if (ItemManager.instance != null && ItemManager.instance.GetCharCollector(petId) != null)
                ItemManager.instance.GetCharCollector(petId).DeActive();

            if (ForestManager.instance != null)
                ForestManager.instance.CheckCollector();
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
