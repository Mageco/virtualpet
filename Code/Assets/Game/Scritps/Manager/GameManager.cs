using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 	public static GameManager instance;

    private Hashtable variables;
    public List<ActionData> actions = new List<ActionData>();
    public float gameTime = 0;

    public List<ItemObject> items = new List<ItemObject>();
    public List<CharController> petObjects = new List<CharController>();
    public List<Pet> pets = new List<Pet>();

    void Awake()
    {
        if (instance == null)
            instance = this;

        Application.targetFrameRate = 50;
        Load();

    }
 

    public void LoadNewUserData()
    {
        ApiManager.instance.AddItem(56);
        ApiManager.instance.AddItem(8);
        ApiManager.instance.AddItem(17);
        ApiManager.instance.AddDiamond(50000);
        ApiManager.instance.AddCoin(50000);
        ApiManager.instance.AddPet(0);
        ApiManager.instance.UsePet(0);
    }

    private void Start()
    {
        if (!ES2.Exists("User"))
        {
            LoadNewUserData();
        }
        LoadItems();
        LoadPets();
    }


    public void EquipeItem()
    {
        StartCoroutine(UseItemCoroutine());
    }

    IEnumerator UseItemCoroutine()
    {
        List<int> data = ApiManager.instance.GetEquipedItems();
        List<ItemObject> removes = new List<ItemObject>();

        foreach (ItemObject item in items)
        {
            bool isRemove = true;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] == item.itemID)
                {
                    isRemove = false;
                }
            }
            if (isRemove)
                removes.Add(item);
        }

        foreach (ItemObject item in removes)
        {
            yield return StartCoroutine(RemoveItem(item.itemID));
        }


        List<int> adds = new List<int>();
        for (int i = 0; i < data.Count; i++)
        {
            bool isAdd = true;
            foreach (ItemObject item in items)
            {
                if (data[i] == item.itemID)
                {
                    isAdd = false;
                }
            }
            if (isAdd)
            {
                adds.Add(data[i]);
            }
        }

        for (int i = 0; i < adds.Count; i++)
        {
            AddItem(adds[i], true);
        }
    }

    public void LoadItems()
    {
        List<int> data = ApiManager.instance.GetEquipedItems();
        List<ItemObject> removes = new List<ItemObject>();

        foreach (ItemObject item in items)
        {
            bool isRemove = true;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] == item.itemID)
                {
                    isRemove = false;
                }
            }
            if (isRemove)
                removes.Add(item);
        }

        foreach (ItemObject item in removes)
        {
            items.Remove(item);
            Destroy(item.gameObject);
        }


        List<int> adds = new List<int>();
        for (int i = 0; i < data.Count; i++)
        {
            bool isAdd = true;
            foreach (ItemObject item in items)
            {
                if (data[i] == item.itemID)
                {
                    isAdd = false;
                }
            }
            if (isAdd)
            {
                adds.Add(data[i]);
            }
        }

        for (int i = 0; i < adds.Count; i++)
        {
            AddItem(adds[i], false);
        }
    }


    void AddItem(int itemId, bool isAnim)
    {
        string url = DataHolder.GetItem(itemId).prefabName.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".prefab", "");
        url = DataHolder.Items().GetPrefabPath() + url;
        GameObject go = Instantiate((Resources.Load(url) as GameObject), Vector3.zero, Quaternion.identity) as GameObject;
        ItemObject item = go.AddComponent<ItemObject>();
        item.itemType = DataHolder.GetItem(itemId).itemType;
        item.itemID = itemId;
        items.Add(item);
        go.transform.parent = this.transform;
        if (isAnim)
        {
            Animator anim = item.GetComponent<Animator>();
            if(anim != null)
                anim.Play("Appear", 0);
        }
    }


    IEnumerator RemoveItem(int itemId)
    {
        foreach (ItemObject item in items)
        {
            if (item.itemID == itemId)
            {
                Animator anim = item.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Play("Disaapear", 0);
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
                }

                items.Remove(item);
                Destroy(item.gameObject);
                break;
            }
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
        pets.Add(p);
        petObjects.Add(c);
    }

    public void UsePet(int itemId)
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

	public void LogAction(ActionType t){
		ActionData a = new ActionData();
		a.actionType = t;
		a.startTime = System.DateTime.Now;
		actions.Add(a);
		Debug.Log(a.actionType + "  " + a.startTime.ToShortTimeString());
		SaveAction();
	}

	public List<ActionData> GetActionLogs(System.DateTime t){
		List<ActionData> temp = new List<ActionData>();
		for(int i=0;i<actions.Count;i++){
			if(actions[i].startTime > t){
				temp.Add(actions[i]);
			}
		}
		return temp;
	}

	void SaveAction(){
		ES2.Save(actions,"ActionLog");
	}

	void LoadAction(){
		if(ES2.Exists("ActionLog")){
			ES2.LoadList<ActionData>("ActionLog");
		}
	}

	public void SaveVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();
		foreach (DictionaryEntry entry in variables) {
			keys.Add ((string)entry.Key);
			values.Add ((string)entry.Value);
		}
		ES2.Save(keys, "variablesKey");
		ES2.Save(values, "variablesValue");

	}

	public void LoadVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();

		if(ES2.Exists("variablesKey"))
			keys = ES2.LoadList<string>("variablesKey");
		if(ES2.Exists("variablesValue"))
			values = ES2.LoadList<string>("variablesValue");

		for (int i = 0; i < keys.Count;i++) {
			if(i < values.Count)
				variables.Add (keys [i], values [i]);

			Debug.Log ("Load Variables " + keys [i] + " " + values [i]);
		}
	}
}
