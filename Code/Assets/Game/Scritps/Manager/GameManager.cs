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
    public bool isPause = false;


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
        ApiManager.GetInstance().AddItem(17);
        ApiManager.GetInstance().AddItem(56);
        ApiManager.GetInstance().AddDiamond(50000);
        ApiManager.GetInstance().AddCoin(1000);
        ApiManager.GetInstance().AddPet(0);
        ApiManager.GetInstance().EquipPet(0);
        ApiManager.GetInstance().EquipItem(17);        
        ApiManager.GetInstance().EquipItem(56);

         
        ApiManager.GetInstance().AddItem(2);
        ApiManager.GetInstance().AddItem(11);                
        ApiManager.GetInstance().AddItem(8);
        ApiManager.GetInstance().EquipItem(2);
        ApiManager.GetInstance().EquipItem(11);                
        ApiManager.GetInstance().EquipItem(8);
        ApiManager.GetInstance().AddItem(4);
        ApiManager.GetInstance().EquipItem(4); 
    }

    private void Start()
    {
        Debug.Log(ApiManager.GetInstance().GetUser().characters.Count);
        if(ApiManager.GetInstance().GetUser().characters.Count == 0){
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
        List<int> data = ApiManager.GetInstance().GetEquipedPets();
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
        if(camera == null)
            camera = Camera.main.GetComponent<CameraController>();
        if(camera != null)
            camera.SetTarget(t);
    }

    public void ResetCameraTarget(){
        if(camera == null)
            camera = Camera.main.GetComponent<CameraController>();
        if(camera != null)
            camera.FindTarget();
    }

    public void OffCameraFollow(){
        if(camera == null)
            camera = Camera.main.GetComponent<CameraController>();
        if(camera != null)
            camera.isFollow = false;
    }


    public void OnCameraFollow(){
        if(camera == null)
            camera = Camera.main.GetComponent<CameraController>();
        if(camera != null)
            camera.isFollow = true;
    }

    public void OnEvent(){
        MageManager.instance.LoadSceneWithLoading("Minigame1");
        gameType = GameType.Minigame1;
        pets[0].Load();
    }

    public void AddCoin(int c){
        ApiManager.GetInstance().AddCoin(c);
    }

    public void AddDiamon(int d){
        ApiManager.GetInstance().AddDiamond(d);
    }

    public void AddExp(int e){
         pets[0].Exp += e;
    }

	
    public void Pause(){
        isPause = true;
    }

    public void UnPause(){
        isPause = false;
    }

}
