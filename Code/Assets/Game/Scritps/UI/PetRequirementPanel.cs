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
    public Text description;

    public Transform anchor;
    List<PetRequirementUI> items = new List<PetRequirementUI>();
    public GameObject petRequirementUIPrefab;
    int price = 0;


    // Start is called before the first frame update
    void Start()
    {
        MageManager.instance.PlaySound("happy_collect_item_01", false);
    }

    public void Load(Pet pet)
    {

        petId = pet.iD;
        string url = pet.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        petAvatar.sprite = Resources.Load<Sprite>(url) as Sprite;
        
        OffAllIcon();

        /*
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
        }*/
        
        petName.text = pet.GetName(0);
        Debug.Log(pet.GetDescription(MageManager.instance.GetLanguage()));
        description.text = pet.GetDescription(MageManager.instance.GetLanguage());
        //petDescription.text = pet.GetDescription(MageManager.instance.GetLanguage(0));
        requireText.text = DataHolder.Dialog(15).GetDescription(MageManager.instance.GetLanguage());
        price  = pet.requireValue;
        petPrice.text = price.ToString();

        /*
        for (int i = 0; i < pet.requirePets.Length; i++)
        {
            LoadPet(pet.requirePets[i]);
        }*/

        for (int i = 0; i < pet.requireEquipments.Length; i++)
        {
            LoadEquipment(pet.requireEquipments[i],pet.requireNumber[i]);
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
        //petPrice.gameObject.SetActive(false);
        //happyIcon.SetActive(false);
        diamonIcon.SetActive(false);
        coinIcon.SetActive(false);
    }

    public void ReLoad()
    {
        
        canBuy = true;
        Pet pet = DataHolder.GetPet(petId);
        Load(pet);
        /*
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
            */
    }

    void LoadEquipment(int id,int number)
    {
        GameObject go = Instantiate(petRequirementUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        PetRequirementUI item = go.GetComponent<PetRequirementUI>();
        Item i = DataHolder.GetItem(id);
        string url = i.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        int n = GameManager.instance.GetItemNumber(id);
        item.Load(id,url,n,number);

        if (n < number)
            canBuy = false;
        
        items.Add(item);
        
    }

    void LoadPet(int id)
    {
        
    }

    public void OnCollect()
    {
        Pet pet = DataHolder.GetPet(petId);

        if (price > GameManager.instance.GetHappy())
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
            return;
        }

        if (!isBuy)
        {
            isBuy = true;
            GameManager.instance.AddHappy(-price, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
            for (int i = 0; i < pet.requireEquipments.Length; i++)
            {
                GameManager.instance.AddItem(pet.requireEquipments[i],-pet.requireNumber[i], Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
            }

            int realId = GameManager.instance.AddPet(petId, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
            PlayerPet p = GameManager.instance.GetPet(realId);
            p.isNew = true;
            GameManager.instance.EquipPet(realId);
            
            if (ItemManager.instance != null && ItemManager.instance.GetCharCollector(petId) != null)
                ItemManager.instance.GetCharCollector(petId).DeActive();

            if (ForestManager.instance != null)
                ForestManager.instance.CheckCollector(p.iD);
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
