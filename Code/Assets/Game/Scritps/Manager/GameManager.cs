using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 	public static GameManager instance;

    
    public float gameTime = 0;
    List<CharController> petObjects = new List<CharController>();
    List<Pet> pets = new List<Pet>();
    CameraController camera;

    public int questId = 0;

    public GameType gameType = GameType.House;

    public int addExp = 0;
    public bool isLoad = false;

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
        ApiManager.instance.AddItem(17);
        ApiManager.instance.AddItem(56);
        ApiManager.instance.AddDiamond(50000);
        ApiManager.instance.AddCoin(1000);
        ApiManager.instance.AddPet(0);

        ApiManager.instance.EquipItem(17);
        ApiManager.instance.EquipPet(0);
         ApiManager.instance.EquipItem(56);

   /*      
        ApiManager.instance.AddItem(2);
        ApiManager.instance.AddItem(11);
                
        ApiManager.instance.AddItem(8);


        
        ApiManager.instance.EquipItem(2);
        ApiManager.instance.EquipItem(11); 
               
        ApiManager.instance.EquipItem(8);*/
        
    }

    private void Start()
    {
        if (!ES2.Exists("User"))
        {
            LoadNewUserData();
            isLoad = true;
        }
        
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
        p.Exp +=addExp;
        CharController c = p.Load();
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

    public CharController GetPetObject(int id){
        return petObjects[id];
    }

    public void UpdatePetObjects(){
        petObjects.Clear();
        for(int i=0;i<pets.Count;i++){
            petObjects.Add(pets[i].character);
        }
    }

    public Pet GetPet(int id){
        return pets[id];
    }

    void Load(){
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		DataHolder.Instance();
		DataHolder.Instance().Init();
		//Debug.Log(DataHolder.Items().GetDataCount());
    }

    public void SetCameraTarget(GameObject t){
        camera.SetTarget(t);
    }

    public void ResetCameraTarget(){
        camera.FindTarget();
    }

    public void OffCameraFollow(){
        camera.isFollow = false;
    }


    public void OnCameraFollow(){
        camera.isFollow = true;
    }

    public void OnEvent(){
        MageManager.instance.LoadSceneWithLoading("Minigame1");
        gameType = GameType.Minigame1;
        pets[0].Load();
    }

    public void AddCoin(int c){
        ApiManager.instance.AddCoin(c);
    }

    public void AddDiamon(int d){
        ApiManager.instance.AddDiamond(d);
    }

    public void AddExp(int e){
         pets[0].Exp += e;
    }
	
}
