using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetCollectionUI : MonoBehaviour
{
    public int petId = 0;
    public Image BG;
    public Image petAvatar;
    public Text petName;
    public Image petFrame;
    Animator animator;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    public void Load(int id)
    {
        petId = id;
        Pet pet = DataHolder.GetPet(petId);
        string url = pet.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        petAvatar.sprite = Resources.Load<Sprite>(url) as Sprite;
        petName.text = pet.GetName(MageManager.instance.GetLanguage());

        int n = petId%5 + 1;
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-15f, 15f)));
        if (GameManager.instance.IsEquipPet(petId))
        {
            petAvatar.color = Color.white;
            BG.sprite = Resources.Load<Sprite>("Icons/Background/BG_" + n.ToString()) as Sprite;

        }
        else
        {
            petAvatar.color = Color.black;
            BG.sprite = Resources.Load<Sprite>("Icons/Background/BG_" + n.ToString()+"_Black") as Sprite;
        }    
    }

    public void OnActive()
    {
        StartCoroutine(ActiveCoroutine());
    }

    IEnumerator ActiveCoroutine()
    {
        Pet pet = DataHolder.GetPet(petId);
        petAvatar.color = Color.black;
        int n = petId % 5 + 1;
        BG.sprite = Resources.Load<Sprite>("Icons/Background/BG_" + n.ToString()) as Sprite;
        animator.Play("Active");
        yield return new WaitForSeconds(0.3f);
        petAvatar.color = Color.white;
        BG.sprite = Resources.Load<Sprite>("Icons/Background/BG_" + n.ToString() + "_Black") as Sprite;
        animator.Play("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
