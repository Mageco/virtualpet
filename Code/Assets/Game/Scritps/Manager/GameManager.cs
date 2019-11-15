using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 	public static GameManager instance;

    
    public float gameTime = 0;
    public List<CharController> petObjects = new List<CharController>();
    public List<Pet> pets = new List<Pet>();
    public CameraController camera;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            GameObject.Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        Application.targetFrameRate = 50;
        Load();
        camera = Camera.main.GetComponent<CameraController>();
    }
 

    public void LoadNewUserData()
    {
        ApiManager.instance.AddItem(56);
        ApiManager.instance.AddItem(8);
        ApiManager.instance.AddItem(17);
        ApiManager.instance.AddDiamond(50000);
        ApiManager.instance.AddCoin(1000);
        ApiManager.instance.AddPet(0);
        ApiManager.instance.EquipItem(56);
        ApiManager.instance.EquipItem(8);
        ApiManager.instance.EquipItem(17);
        ApiManager.instance.EquipPet(0);
    }

    private void Start()
    {
        if (!ES2.Exists("User"))
        {
            LoadNewUserData();
        }
        
        if(ItemManager.instance != null)
            ItemManager.instance.LoadItems();

        LoadPets();
        
        if(camera != null){
            camera.SetTarget(petObjects[0].gameObject);
        }
       
    }


    


    public void LoadPets()
    {
        List<int> data = ApiManager.instance.GetEquipedPets();
        for (int i = 0; i < data.Count; i++)
        {
            AddPet(data[i]);
        }
    }

    void AddPet(int itemId)
    {
        Pet p = new Pet(itemId);
       
        CharController c = p.Load();
        p.AddExp(2000);
        pets.Add(p);
        petObjects.Add(c);
    }

    public void EquipPet(int itemId)
    {
        AddPet(itemId);
    }

	void Update(){
		gameTime += Time.deltaTime;
	}

    void Load(){
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		DataHolder.Instance();
		DataHolder.Instance().Init();
		//Debug.Log(DataHolder.Items().GetDataCount());
    }





	
}
