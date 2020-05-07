using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;
using TMPro;


public class CharController : MonoBehaviour
{

    #region Declair
    //Data
    public Pet data;
    public GameObject petPrefab;
    public CharType charType = CharType.Dog;

   [HideInInspector]
    public Direction direction = Direction.L;

    //Think
    protected float dataTime = 0;
    protected float maxDataTime = 1f;

    float saveTime = 0;
    float maxSaveTime = 1;

    float timeLove = 0;
    float maxTimeLove = 10;

    //Movement
    [HideInInspector]
    public Vector3 target;

    [HideInInspector]
    public PolyNavAgent agent;
    [HideInInspector]
    public bool isArrived = true;
    [HideInInspector]
    public bool isAbort = false;
    [HideInInspector]
    public bool isAction = false;
    [HideInInspector]
    public bool isMoving = false;


    //Action
    public ActionType actionType = ActionType.None;
    public ActionType lastActionType = ActionType.None;
    
    [HideInInspector]
    public Animator anim;

    //Interact
    [HideInInspector]
    public CharInteract charInteract;
    [HideInInspector]
    public CharScale charScale;
    [HideInInspector]
    public CharCollider charCollider;
    [HideInInspector]
    public CharStatus charStatus;

    //Pee,Sheet
    public Transform peePosition;
    public Transform shitPosition;
    EmotionStatus lastEmotionStatus;
 


    //[HideInInspector]
    public GameObject shadow;
    [HideInInspector]
    public Vector3 originalShadowScale;

    TextMeshPro petNameText;
    public SpriteRenderer petEmotion;
    EmotionStatus emotionStatus = EmotionStatus.Happy;
    Sprite[] emotionIcons = new Sprite[3];

    #endregion

    //Skill
    [HideInInspector]
    public SkillType currentSkill = SkillType.NONE;
    MouseController mouse;

    [HideInInspector]
    public Vector3 dropPosition = Vector3.zero;

    public SpriteRenderer iconStatusObject;
    [HideInInspector]
    public IconStatus iconStatus = IconStatus.None;
    IconStatus lastIconStatus = IconStatus.None;
    Vector3 originalStatusScale;

    public BaseFloorItem equipment;
    protected BaseFloorItem lastEquipment;
    [HideInInspector]
    public List<GameObject> dirties = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> dirties_L = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> dirties_LD = new List<GameObject>();
    public List<GameObject> skinPrefabs = new List<GameObject>();
    GameObject charObject;

    #region Load

    void Awake()
    {
        charInteract = this.GetComponent<CharInteract>();
        charScale = this.GetComponent<CharScale>();
        charCollider = this.GetComponentInChildren<CharCollider>(true);
    }

    protected virtual void Start()
    {
        if (actionType != ActionType.None)
            DoAction();
    }

    public void LoadData(PlayerPet pet)
    {

        if (!GameManager.instance.isGuest && ES2.Exists(DataHolder.GetPet(pet.iD).GetName(0) + pet.realId.ToString()))
        {
            if (float.Parse(GameManager.instance.myPlayer.version) < 2.0f)
            {
                Pet p = new Pet(pet.iD);
                p.realId = pet.realId;
                p.level = pet.level;
                this.data = p;
            }
            else
            {
                Pet p = ES2.Load<Pet>(DataHolder.GetPet(pet.iD).GetName(0) + pet.realId.ToString());
                p.realId = pet.realId;
                p.level = pet.level;
                this.data = p;
            }
        }
        else
        {
            Pet p = new Pet(pet.iD);
            p.realId = pet.realId;
            p.level = pet.level;
            this.data = p;
        }


    }

    public void LoadPrefab()
    {
        

        LoadCharObject();

        GameObject go1 = Instantiate(Resources.Load("Prefabs/Pets/Agent")) as GameObject;
        //go1.transform.parent = GameManager.instance.transform;
        agent = go1.GetComponent<PolyNavAgent>();
        agent.OnDestinationReached += OnArrived;
        agent.transform.position = this.transform.position;

        agent.maxSpeed = data.speed;

        SetDirection(Direction.R);


    }

    public void LoadCharObject()
    {
        iconStatusObject.transform.parent = this.transform;

        if (charObject != null)
            Destroy(charObject);

        if (petNameText != null)
            Destroy(petNameText.gameObject);

        if(petEmotion != null)
            Destroy(petEmotion.gameObject);

        if (charStatus != null)
            Destroy(charStatus.gameObject);

        

        charObject = Instantiate(skinPrefabs[GameManager.instance.GetPet(data.realId).accessoryId]) as GameObject;
        charObject.transform.parent = this.transform;
        charObject.transform.localScale = skinPrefabs[GameManager.instance.GetPet(data.realId).accessoryId].transform.localScale;
        charObject.transform.localPosition = Vector3.zero;
        
        anim = charObject.transform.GetComponent<Animator>();

        //Load Dirty Effect
        //grab all the kids and only keep the ones with dirty tags
        dirties.Clear();
        dirties_L.Clear();
        dirties_LD.Clear();
       

        Transform[] allChildren = charObject.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            if (child.gameObject.tag == "Dirty")
            {
                dirties.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
            else if (child.gameObject.tag == "Shadow")
            {
                shadow = child.gameObject;
                originalShadowScale = shadow.transform.localScale;
            }
            else if (child.gameObject.tag == "Dirty_L")
            {
                dirties_L.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
            else if (child.gameObject.tag == "Dirty_LD")
            {
                dirties_LD.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }

        GameObject go1 = GameObject.Instantiate(Resources.Load("Prefabs/Pets/PetEmotion")) as GameObject;
        petEmotion = go1.GetComponent<SpriteRenderer>();
        go1.transform.parent = this.transform;
        go1.transform.localPosition = new Vector3(-4f, iconStatusObject.transform.localPosition.y - 2.5f, -10);
        petEmotion.gameObject.SetActive(false);

        iconStatusObject.transform.parent = charObject.transform.GetChild(0);
        data.petName = GameManager.instance.GetPet(data.realId).petName;
        GameObject nameObject = GameObject.Instantiate(Resources.Load("Prefabs/Pets/PetNamePrefab")) as GameObject;

        nameObject.transform.position = iconStatusObject.transform.position - new Vector3(0, 2.5f, 10);
        petNameText = nameObject.GetComponent<TextMeshPro>();
        petNameText.text = data.petName;

 
        nameObject.transform.SetParent(this.transform);
        if (iconStatusObject != null)
        {
            iconStatusObject.enabled = false;
            iconStatusObject.gameObject.SetActive(false);
            originalStatusScale = iconStatusObject.transform.localScale;

            GameObject go2 = Instantiate(Resources.Load("Prefabs/Pets/PetStatusPrefab")) as GameObject;
            charStatus = go2.GetComponent<CharStatus>();
            go2.transform.parent = iconStatusObject.transform;
            go2.transform.localPosition = new Vector3(0, 0, -10);

            emotionIcons[0] = Resources.Load<Sprite>("Icons/Status/Happy") as Sprite;
            emotionIcons[1] = Resources.Load<Sprite>("Icons/Status/Normal") as Sprite;
            emotionIcons[2] = Resources.Load<Sprite>("Icons/Status/Sad") as Sprite;

        }


    }


    public void SetName()
    {
        data.petName = GameManager.instance.GetPet(data.realId).petName;
        Debug.Log(data.petName);
        petNameText.text = data.petName;
    }



    // Use this for initialization

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        if (agent == null)
            return;


        //petNameText.gameObject.SetActive(true);
        //Debug.Log(actionType.ToString() + "  " + charInteract.interactType.ToString());
        if (actionType == ActionType.None)
        {
            Think();
            DoAction();
        }
        else if(actionType == ActionType.Toy)
        {
            petNameText.transform.position = iconStatusObject.transform.position - new Vector3(0, 2.5f, 10);
            petNameText.transform.rotation = Quaternion.identity;
            //petNameText.gameObject.SetActive(false);
            if (equipment != null && equipment.IsActive())
            {
                int value = (int)DataHolder.GetItem(equipment.itemID).value;
                data.Toy += 2 * value * Time.deltaTime;
            }
        }

        if(emotionStatus != EmotionStatus.Sad)
        {
            if(timeLove > maxTimeLove)
            {
                StartCoroutine(OnEmotion());
                ItemManager.instance.SpawnPetHappy(this.transform.position,data.RateHappy + data.level / 5);
                timeLove = 0;
            }else
                timeLove += Time.deltaTime;
        }

        if(emotionStatus == EmotionStatus.Happy)
        {
            petEmotion.sprite = emotionIcons[0];
        }else if(emotionStatus == EmotionStatus.Normal)
        {
            petEmotion.sprite = emotionIcons[1];
        }else
        {
            petEmotion.sprite = emotionIcons[2];
        }

        CalculateDirection();

        //if(charInteract.interactType == InteractType.None)
        this.transform.position = agent.transform.position;

        //Calculate Attribue Data
        if (dataTime > maxDataTime)
        {
            CalculateData();
            dataTime = 0;
        }
        else
            dataTime += Time.deltaTime;

        CalculateStatus();

        //Save
        if (saveTime > maxSaveTime)
        {
            saveTime = 0;
            if (!GameManager.instance.isGuest)
                ES2.Save(this.data, DataHolder.GetPet(data.iD).GetName(0) + data.realId.ToString());

        }
        else
            saveTime += Time.deltaTime;
    }



    #endregion

    #region Data
    protected virtual void CalculateData()
    {

        if (data.Food > 0 && data.Water > 0)
        {
            data.Food -= 0.4f;
            data.Water -= 0.4f;
            data.Shit += 0.2f;
            data.Pee += 0.25f;
        }


        data.Dirty += 0.2f;
        data.Sleep -= 0.1f;

        if(actionType != ActionType.Toy)
            data.Toy -= 0.5f;

        float deltaHealth = 0;

        if (data.Health > 0.1f * data.MaxHealth)
        {
            if (data.Dirty > data.MaxDirty * 0.7f)
                deltaHealth -= (data.Dirty - data.MaxDirty * 0.7f) * 0.005f;

            if (data.Pee > data.MaxPee * 0.95f)
                deltaHealth -= (data.Pee - data.MaxPee * 0.95f) * 0.005f;

            if (data.Shit > data.MaxShit * 0.95f)
                deltaHealth -= (data.Shit - data.MaxShit * 0.95f) * 0.005f;

            if (data.Food < data.MaxFood * 0.05f)
                deltaHealth -= (data.MaxFood * 0.05f - data.Food) * 0.005f;

            if (data.Water < data.MaxWater * 0.05f)
                deltaHealth -= (data.MaxWater * 0.05f - data.Water) * 0.005f;

            if (data.Sleep < data.MaxSleep * 0.05f)
                deltaHealth -= (data.MaxSleep * 0.05f - data.Sleep) * 0.005f;
        }
        //Debug.Log(deltaHealth);

        data.Health += deltaHealth;

        //CheckDirty

        if(charObject != null)
        {
            if (data.dirty > data.MaxDirty * 0.5f)
            {
                int n = (int)((data.dirty - data.MaxDirty * 0.5f) / (data.MaxDirty * 0.5f) * dirties.Count);
                for (int i = 0; i < dirties.Count; i++)
                {
                    if (i < n)
                        dirties[i].SetActive(true);
                    else
                        dirties[i].SetActive(false);
                }

                int n1 = (int)((data.dirty - data.MaxDirty * 0.5f) / (data.MaxDirty * 0.5f) * dirties_L.Count);
                for (int i = 0; i < dirties_L.Count; i++)
                {
                    if (i < n)
                        dirties_L[i].SetActive(true);
                    else
                        dirties_L[i].SetActive(false);
                }

                int n2 = (int)((data.dirty - data.MaxDirty * 0.5f) / (data.MaxDirty * 0.5f) * dirties_LD.Count);
                for (int i = 0; i < dirties_LD.Count; i++)
                {
                    if (i < n)
                        dirties_LD[i].SetActive(true);
                    else
                        dirties_LD[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < dirties.Count; i++)
                {
                    dirties[i].SetActive(false);
                }
                for (int i = 0; i < dirties_L.Count; i++)
                {
                    dirties_L[i].SetActive(false);
                }
                for (int i = 0; i < dirties_LD.Count; i++)
                {
                    dirties_LD[i].SetActive(false);
                }
            }
        }
        

        //Save data
        data.actionType = this.actionType;
        data.position = this.transform.position;
        data.scalePosition = charScale.scalePosition;
        if (equipment != null)
            data.equipmentId = equipment.itemID;
        else
            data.equipmentId = 0;
                
    }



    #region Thinking
    protected virtual void Think()
    {
        if (charInteract.interactType != InteractType.None)
            return;

        if (data.Damage > data.MaxDamage * 0.8f)
        {
            actionType = ActionType.Injured;
            return;
        }

        if (data.Health < data.MaxHealth * 0.1f)
        {
            actionType = ActionType.Sick;
            return;
        }

        if (data.Shit > data.MaxShit * 0.9f)
        {
            actionType = ActionType.Shit;
            return;
        }

        if (data.Pee > data.MaxPee * 0.9f)
        {
            actionType = ActionType.Pee;
            return;
        }

        if (data.Dirty > data.MaxDirty * 0.9f)
        {
            if(data.level >= 15)
            {
                actionType = ActionType.OnBath;
                return;
            }
            else
            {
                actionType = ActionType.Itchi;
                return;
            }
  
        }

        if (data.Food < data.MaxFood * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Eat;
                return;
            }
        }


        if (data.Water < data.MaxWater * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Drink;
                return;
            }
        }

        if (data.Sleep < data.MaxSleep * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Sleep;
                return;
            }
        }

        if (data.Toy < data.MaxToy * 0.1f && data.level >= 10) 
        {
            actionType = ActionType.Discover;
            return;
        }

        /*
        if (data.Energy < data.MaxEnergy * 0.1f)
        {
            actionType = ActionType.Tired;
            return;
        }*/

        

        int n = Random.Range(1, 100);
        if (n > 20)
            actionType = ActionType.Patrol;
        else
            actionType = ActionType.OnCall;
    }




    protected virtual void DoAction()
    {
        lastActionType = actionType;
        if (isAction)
        {
           
            Debug.Log("Action is doing " + actionType);
            StopAllCoroutines();
            isAction = false;
        }
        //Debug.Log("DoAction " + actionType);
        isAbort = false;
        agent.Stop();
        isAction = true;

        if (actionType == ActionType.Patrol)
        {
            StartCoroutine(Patrol());
        }
        else if (actionType == ActionType.Pee)
        {
            StartCoroutine(Pee());
        }
        else if (actionType == ActionType.Shit)
        {
            StartCoroutine(Shit());
        }
        else if (actionType == ActionType.Eat)
        {
            StartCoroutine(Eat());
        }
        else if (actionType == ActionType.Drink)
        {
            StartCoroutine(Drink());
        }
        else if (actionType == ActionType.Sleep)
        {
            StartCoroutine(Sleep());
        }
        else if (actionType == ActionType.Itchi)
        {
            StartCoroutine(Itchi());
        }
        else if (actionType == ActionType.Sick)
        {
            StartCoroutine(Sick());
        }
        else if (actionType == ActionType.Discover)
        {
            StartCoroutine(Discover());
        }
        else if (actionType == ActionType.Hold)
        {
            StartCoroutine(Hold());
        }
        else if (actionType == ActionType.Mouse)
        {
            StartCoroutine(Mouse());
        }
        else if (actionType == ActionType.OnBath)
        {
            StartCoroutine(Bath());
        }
        else if (actionType == ActionType.OnTable)
        {
            StartCoroutine(Table());
        }
        else if (actionType == ActionType.Tired)
        {
            StartCoroutine(Tired());
        }
        else if (actionType == ActionType.Happy)
        {
            StartCoroutine(Happy());
        }
        else if (actionType == ActionType.Fall)
        {
            StartCoroutine(Fall());
        }
        else if (actionType == ActionType.Supprised)
        {
            StartCoroutine(Supprised());
        }
        else if (actionType == ActionType.Stop)
        {
            StartCoroutine(Stop());
        }
        else if (actionType == ActionType.Toy)
        {
            StartCoroutine(Toy());
        }
        else if (actionType == ActionType.Injured)
        {
            StartCoroutine(Injured());
        }
        else if (actionType == ActionType.OnCall)
        {
            StartCoroutine(Call());
        }
        else if (actionType == ActionType.OnControl)
        {
            StartCoroutine(Control());
        }
        else if (actionType == ActionType.OnGift)
        {
            StartCoroutine(Gift());
        }
    }

    #endregion

    protected virtual void CalculateStatus()
    {
        lastEmotionStatus = emotionStatus;
        lastIconStatus = iconStatus;

        if (actionType == ActionType.Injured)
        {
            iconStatus = IconStatus.Bandage;
            charStatus.SetProgress((data.MaxDamage - data.Damage) / data.MaxDamage);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (actionType == ActionType.Sick)
        {
            iconStatus = IconStatus.MedicineBox;
            charStatus.SetProgress(data.Health / data.MaxHealth);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (actionType == ActionType.OnBath)
        {
            iconStatus = IconStatus.Bath;
            charStatus.SetProgress((data.MaxDirty - data.Dirty) / data.MaxDirty);
        }
        else if (actionType == ActionType.Sleep)
        {
            iconStatus = IconStatus.Bed;
            charStatus.SetProgress(data.Sleep / data.MaxSleep);
        }
        else if (actionType == ActionType.Eat)
        {
            iconStatus = IconStatus.Food;
            charStatus.SetProgress(data.Food / data.MaxFood);
        }
        else if (actionType == ActionType.Drink)
        {
            iconStatus = IconStatus.Drink;
            charStatus.SetProgress(data.Water / data.MaxWater);
        }
        else if (actionType == ActionType.Pee)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxPee - data.Pee) / data.MaxPee);
        }
        else if (actionType == ActionType.Shit)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxShit - data.Shit) / data.MaxShit);
        }
        else if (actionType == ActionType.Toy)
        {
            iconStatus = IconStatus.Toy;
            charStatus.SetProgress(data.Toy / data.MaxToy);
        }
        else if (data.Damage > 0.9f * data.MaxDamage)
        {
            iconStatus = IconStatus.Bandage;
            charStatus.SetProgress((data.MaxDamage - data.Damage) / data.MaxDamage);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (data.Health < 0.1f * data.MaxHealth)
        {
            iconStatus = IconStatus.MedicineBox;
            charStatus.SetProgress(data.Health / data.MaxHealth);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (data.Pee > 0.9f * data.MaxPee)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxPee - data.Pee) / data.MaxPee);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (data.Pee > 0.9f * data.MaxShit)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxShit - data.Shit) / data.MaxShit);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (data.Food < 0.1f * data.MaxFood)
        {
            iconStatus = IconStatus.Food;
            charStatus.SetProgress(data.Food / data.MaxFood);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (data.Water < 0.1f * data.MaxWater)
        {
            iconStatus = IconStatus.Drink;
            charStatus.SetProgress(data.Water / data.MaxWater);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (data.Sleep < 0.1f * data.MaxSleep)
        {
            iconStatus = IconStatus.Bed;
            charStatus.SetProgress(data.Sleep / data.MaxSleep);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (data.Toy < 0.1f * data.MaxToy)
        {
            iconStatus = IconStatus.Toy;
            charStatus.SetProgress(data.Toy / data.MaxToy);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (data.Dirty > 0.9f * data.MaxDirty)
        {
            iconStatus = IconStatus.Bath;
            charStatus.SetProgress((data.MaxDirty - data.Dirty) / data.MaxDirty);
            emotionStatus = EmotionStatus.Sad;
        }
        else if (data.Damage > 0.7f * data.MaxDamage)
        {
            iconStatus = IconStatus.Bandage;
            charStatus.SetProgress((data.MaxDamage - data.Damage) / data.MaxDamage);
            emotionStatus = EmotionStatus.Normal;
        }
        else if (data.Health < 0.3f * data.MaxHealth)
        {
            iconStatus = IconStatus.MedicineBox;
            charStatus.SetProgress(data.Health / data.MaxHealth);
            emotionStatus = EmotionStatus.Normal;
        }
        else if (data.Pee > 0.7f * data.MaxPee)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxPee - data.Pee) / data.MaxPee);
            emotionStatus = EmotionStatus.Normal;
        }
        else if (data.Pee > 0.7f * data.MaxShit)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxShit - data.Shit) / data.MaxShit);
            emotionStatus = EmotionStatus.Normal;
        }
        else if (data.Food < 0.3f * data.MaxFood)
        {
            iconStatus = IconStatus.Food;
            charStatus.SetProgress(data.Food / data.MaxFood);
            emotionStatus = EmotionStatus.Normal;
        }
        else if (data.Water < 0.3f * data.MaxWater)
        {
            iconStatus = IconStatus.Drink;
            charStatus.SetProgress(data.Water / data.MaxWater);
            emotionStatus = EmotionStatus.Normal;
        }
        else if (data.Sleep < 0.3f * data.MaxSleep)
        {
            iconStatus = IconStatus.Bed;
            charStatus.SetProgress(data.Sleep / data.MaxSleep);
            emotionStatus = EmotionStatus.Normal;
        }
        else if (data.Toy < 0.3f * data.MaxToy)
        {
            iconStatus = IconStatus.Toy;
            charStatus.SetProgress(data.Toy / data.MaxToy);
            emotionStatus = EmotionStatus.Normal;
        }
        else if (data.Dirty > 0.7f * data.MaxDirty)
        {
            iconStatus = IconStatus.Bath;
            charStatus.SetProgress((data.MaxDirty - data.Dirty) / data.MaxDirty);
            emotionStatus = EmotionStatus.Normal;
        }
        else
        {
            iconStatus = IconStatus.None;
            emotionStatus = EmotionStatus.Happy;
        }


        if (iconStatusObject != null)
        {
            LoadIconStatus();
        }

        if(emotionStatus != lastEmotionStatus)
        {
            StartCoroutine(OnEmotion());
        }
        //iconStatusObject.transform.localScale = originalStatusScale / charScale.scaleAgeFactor;

    }

    IEnumerator OnEmotion()
    {
        petEmotion.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        petEmotion.gameObject.SetActive(false);
    }

    void LoadIconStatus()
    {

        if (iconStatus != IconStatus.None && lastIconStatus != iconStatus)
        {
            iconStatusObject.gameObject.SetActive(true);
            charStatus.Load(iconStatus);
        }

        if (iconStatus == IconStatus.None)
        {
            iconStatusObject.gameObject.SetActive(false);
        }
        
    }

    protected virtual void CalculateDirection()
    {
        if (agent.transform.eulerAngles.z < 180f && agent.transform.eulerAngles.z > 0f || (agent.transform.eulerAngles.z > -360f && agent.transform.eulerAngles.z < -180f))
            direction = Direction.L;
        else
            direction = Direction.R;
    }



    #endregion


    #region Interact


    public void SetActionType(ActionType action)
    {
        Abort();
        actionType = action;
    }

    public virtual void OnHold()
    {
        /*
        if (actionType == ActionType.Sick)
        {
            UIManager.instance.OnTreatmentPopup(this.data,SickType.Sick);
            return;
        }

        if (actionType == ActionType.Injured)
        {
            UIManager.instance.OnTreatmentPopup(this.data, SickType.Injured);
            return;
        }*/

        if (charInteract.interactType == InteractType.Drop || charInteract.interactType == InteractType.Jump || actionType == ActionType.OnControl || actionType == ActionType.OnGift)
            return;

        Abort();
        charInteract.interactType = InteractType.Drag;
        actionType = ActionType.Hold;
    }

    public virtual void OnToy(BaseFloorItem item)
    {
        if (equipment != null)
            return;

        if (item == null)
            return;

        //Debug.Log("Toy2");
        if (item.state == EquipmentState.Drag || item.state == EquipmentState.Busy || item.state == EquipmentState.Active)
            return;


        if(item.itemType == ItemType.Toy)
        {
            if (actionType != ActionType.Sick && actionType != ActionType.Injured && actionType != ActionType.OnControl && charInteract.interactType != InteractType.Drag //actionType != ActionType.Hold
                && actionType != ActionType.Toy && actionType != ActionType.Supprised && actionType != ActionType.OnCall && actionType != ActionType.OnGift)
            {
                actionType = ActionType.Toy;
                isAbort = true;
                lastEquipment = item;
                equipment = item;
                //Debug.Log(item.toyType);            
            }
        }
    }

    public virtual void OnCall()
    {
        if (actionType == ActionType.Sick || actionType == ActionType.Injured || actionType == ActionType.Hold)
        {
            return;
        }

        if(equipment != null)
        {
            agent.transform.position = equipment.endPoint.position;
        }

        Abort();
        actionType = ActionType.OnCall;
    }

    public virtual void OnSupprised()
    {
        if (actionType == ActionType.Hold || actionType == ActionType.Sick || actionType == ActionType.Injured || actionType == ActionType.Toy)
            return;

        Abort();
        actionType = ActionType.Supprised;
    }

    public virtual void OnEat()
    {
        actionType = ActionType.Eat;
        isAbort = true;        
    }

    public virtual void OnDrink()
    {
        actionType = ActionType.Drink;
        isAbort = true;   
    }

    protected void OnSleep()
    {
        Abort();
        actionType = ActionType.Sleep;
    }

    protected void OnToilet()
    {
        Abort();
        actionType = ActionType.Shit;
    }

    protected void OnBath()
    {
        Abort();
        actionType = ActionType.OnBath;
    }

    public void OnGift()
    {
        Abort();
        actionType = ActionType.OnGift;
    }


    public void OnHealth(SickType type, float value)
    {
        if (type == SickType.Injured)
        {
            if (data.Damage > data.MaxDamage * 0.7f)
            {
                GameManager.instance.LogAchivement(AchivementType.Injured);
            }
            data.Damage -= value;
            ItemManager.instance.SpawnBandageEffect(this, 10);
            GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Injured);
        }
        else if (type == SickType.Sick)
        {
            if (data.Health < data.MaxHealth * 0.3f)
            {
                GameManager.instance.LogAchivement(AchivementType.Sick);
            }
            data.Health += value;
            ItemManager.instance.SpawnPillEffect(this, 10);
            GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Sick);
        }
       
    }


    protected void OnTable()
    {
        Abort();
        actionType = ActionType.OnTable;
    }

    public virtual void OnFall()
    {
        if (actionType == ActionType.Sick || actionType == ActionType.Injured || actionType == ActionType.Hold || actionType == ActionType.Toy || data.Health == 0 || data.Damage == data.MaxDamage)
            return;


        if (!isArrived)
        {
            int ran = Random.Range(0, 100);
            if (ran > 20)
            {
                Abort();
                isArrived = true;
                actionType = ActionType.Fall;
            }
        }
    }

    /*
    public virtual void OnSoap()
    {
        if (actionType == ActionType.OnBath)
        {
            anim.Play("Soap", 0);
        }
    }

    public virtual void OffSoap()
    {
        if (actionType == ActionType.OnBath)
        {
            anim.Play("Standby", 0);
        }
    }

    public virtual void OnShower()
    {
        if (actionType == ActionType.OnBath)
            anim.Play("Shower", 0);
    }

    public virtual void OffShower()
    {
        if (actionType == ActionType.OnBath)
        {
            anim.Play("Shake", 0);
            MageManager.instance.PlaySound3D("Shake", false,this.transform.position);
        }

    }*/


    public virtual void OnControl()
    {
        actionType = ActionType.OnControl;
        isAbort = true;
    }

    public virtual void OnStop()
    {
        if (actionType != ActionType.Stop && !isArrived)
        {
            //Debug.Log(lastActionType);
            agent.Stop();
            actionType = ActionType.Stop;
            isAbort = true;
        }
    }

    public virtual void OnMouse()
    {
        if (charInteract.interactType != InteractType.None)
            return;

        if (actionType == ActionType.Patrol || actionType == ActionType.Discover || actionType == ActionType.Drink || actionType == ActionType.Eat)
        {
            if (charType == CharType.Cat || charType == CharType.Dog || charType == CharType.Shamoyed || charType == CharType.Chihuhu)
            {
                Abort();
                actionType = ActionType.Mouse;
            }
        }
    }

 
    public virtual void OnArrived()
    {
        if (actionType == ActionType.Mouse)
        {
            actionType = ActionType.None;
            anim.speed = 1;
        }

        isArrived = true;
    }


    public virtual void OnLevelUp()
    {
        
    }

    public virtual void OnTreatment(SickType sickType)
    {
        if (sickType == SickType.Sick)
            data.Health = data.MaxHealth;
        else if (sickType == SickType.Injured)
            data.Damage = 0;
    }



    #endregion




    #region Basic Action

    //Basic Action
    protected void Abort()
    {
        if (anim != null)
            anim.speed = 1;
        isAbort = true;
    }



    protected void SetDirection(Direction d)
    {
        direction = d;
        if (d == Direction.D)
        {
            agent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        else if (d == Direction.U)
            agent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -180));
        else if (d == Direction.RD)
            agent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -140));
        else if (d == Direction.RU)
            agent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -40));
        else if (d == Direction.LD)
            agent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 140));
        else if (d == Direction.LU)
            agent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 40));
        else if (d == Direction.L)
            agent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        else if (d == Direction.R)
            agent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
    }

    protected void LookToTarget(Vector3 target)
    {
        if (target.x >= this.transform.position.x)
        {
            direction = Direction.R;
            SetDirection(Direction.R);
        }
        else
        {
            direction = Direction.L;
            SetDirection(Direction.L);
        }
    }

    protected virtual IEnumerator DoAnim(string a)
    {
        float time = 0;
        anim.Play(a, 0);
        //yield return new WaitForEndOfFrame();
        while (time < anim.GetCurrentAnimatorStateInfo(0).length && !isAbort)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    protected virtual IEnumerator DoForceAnim(string a)
    {
        float time = 0;
        anim.Play(a, 0);
        while (time < anim.GetCurrentAnimatorStateInfo(0).length)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    protected virtual IEnumerator RunToPoint()
    {
        agent.Stop();
        isMoving = true;
        isArrived = false;
        agent.SetDestination(target);
        charScale.speedFactor = 1;

        while (!isArrived && !isAbort)
        {
            anim.Play("Run_" + this.direction.ToString(), 0);
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
    }

    protected IEnumerator WalkToPoint()
    {
        isMoving = true;
        isArrived = false;
        agent.SetDestination(target);
        charScale.speedFactor = 0.4f;

        while (!isArrived && !isAbort)
        {
            anim.Play("Walk_" + this.direction.ToString(), 0);
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
        charScale.speedFactor = 1;
    }

    protected IEnumerator Wait(float maxT)
    {
        float time = 0;

        while (time < maxT && !isAbort)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }


    protected virtual IEnumerator Mouse()
    {
        isMoving = true;
        charScale.speedFactor = 1.2f;
        while (GetMouse() != null && GetMouse().state != MouseState.Idle && !isAbort)
        {
            agent.SetDestination(GetMouse().transform.position);
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
        charScale.speedFactor = 1;
        CheckAbort();
    }

    protected virtual IEnumerator Hold()
    {
        if (equipment != null)
        {
            equipment.RemovePet(this);
            equipment.DeActive();
            equipment = null;
        }
       
        MageManager.instance.PlaySound("Drag", false);
        if (charInteract.isDrag)
        {
            charInteract.interactType = InteractType.Drag;
        }
        
        ItemManager.instance.SetCameraTarget(this.gameObject);
        anim.Play("Hold", 0);
        if (shadow != null)
            shadow.SetActive(true);
        Vector3 lastPosition = this.transform.position;
        Quaternion targetRotation;
        while (charInteract.interactType == InteractType.Drag && !isAbort)
        {

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - charInteract.dragOffset;
            
            pos.z = 0;
            if (pos.y > charScale.maxHeight)
                pos.y = charScale.maxHeight;
            else if (pos.y < ItemManager.instance.gardenBoundY.x)
                pos.y = ItemManager.instance.gardenBoundY.x;

            if (pos.x > ItemManager.instance.gardenBoundX.y)
                pos.x = ItemManager.instance.gardenBoundX.y;
            else if (pos.x < ItemManager.instance.gardenBoundX.x)
                pos.x = ItemManager.instance.gardenBoundX.x;

            targetRotation = Quaternion.Euler(new Vector3(0, 0, -(this.transform.position.x - lastPosition.x) * 50));
            this.charObject.transform.rotation = Quaternion.Lerp(this.charObject.transform.rotation, targetRotation, Time.deltaTime * 5);
            pos.z = -50;
            agent.transform.position = Vector3.Lerp(this.transform.position,pos,Time.deltaTime * 5);
            lastPosition = this.transform.position;
            yield return new WaitForEndOfFrame();
        }

        this.charObject.transform.rotation = Quaternion.identity;
        dropPosition = charScale.scalePosition;
        bool dropOutSide = false;
        //Start Drop
        
        if (charCollider.items.Count > 0)
        {
            equipment = charCollider.items[0];
            if (!equipment.IsBusy())
            {
                if (equipment.itemType != ItemType.Bed && equipment.itemType != ItemType.Toilet && equipment.itemType != ItemType.Table && (data.Health < data.MaxHealth * 0.1f || data.Damage > data.MaxDamage * 0.9f))
                {
                    dropPosition = equipment.endPoint.transform.position;
                    equipment = null;
                    dropOutSide = true;
                }
                else
                    dropPosition = equipment.GetAnchorPoint(this).position;
            }
            else
            {
                if(equipment.anchorPoints.Length > 0)
                {
                    dropPosition = equipment.endPoint.transform.position;
                    equipment = null;
                    dropOutSide = true;
                    UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(157).GetName(MageManager.instance.GetLanguage()));
                }
            }
        }

        
        float fallSpeed = 0;

        while (charInteract.interactType == InteractType.Drop)
        {
            if (agent.transform.position.y > dropPosition.y && charScale.height > 0)
            {
                fallSpeed += 100f * Time.deltaTime;
                if (fallSpeed > 50)
                    fallSpeed = 50;
                Vector3 pos1 = agent.transform.position;
                pos1.y -= fallSpeed * Time.deltaTime;
                pos1.x = Mathf.Lerp(pos1.x, dropPosition.x, Time.deltaTime * 5);
                pos1.z = charScale.scalePosition.z;
                agent.transform.position = pos1;
            }
            else
            {
                this.transform.rotation = Quaternion.identity;
                Vector3 pos3 = agent.transform.position;
                pos3.y = dropPosition.y;
                pos3.x = dropPosition.x;
                agent.transform.position = pos3;
                charInteract.interactType = InteractType.None;
                if (dropOutSide || (equipment != null && (equipment.itemType == ItemType.Food || equipment.itemType == ItemType.Drink)))
                {
                    charScale.height = 0;
                    charScale.scalePosition = agent.transform.position;
                }                   
                else
                    charScale.height = dropPosition.y - charScale.scalePosition.y;
                
            }
            yield return new WaitForEndOfFrame();
        }
        ItemManager.instance.ResetCameraTarget();
        charInteract.interactType = InteractType.None;

        CheckEnviroment();
        

        MageManager.instance.PlaySound3D("whoosh_swish_med_03", false, this.transform.position);
        yield return StartCoroutine(DoAnim("Drop"));

        CheckAbort();
    }

    void CompleteDrop()
    {
        charInteract.interactType = InteractType.None;
    }

    protected virtual IEnumerator Table()
    {

        if (equipment != null)
        {
            charInteract.interactType = InteractType.Equipment;
            agent.transform.position = equipment.GetAnchorPoint(this).position;

            while (!isAbort)
            {
                if(data.Health < data.MaxHealth * 0.1f) 
                {
                    anim.Play("Sick", 0);
                    yield return new WaitForEndOfFrame();
                }
                else if(data.Damage > data.MaxDamage * 0.9f)
                {
                    anim.Play("Injured_L", 0);
                    yield return new WaitForEndOfFrame();
                }
                else
                {

                    int ran = Random.Range(0, 100);
                    if (ran < 30)
                    {
                        MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false, this.transform.position);
                        yield return StartCoroutine(DoAnim("Speak_" + direction.ToString()));
                    }
                    else if (ran < 60)
                    {
                        anim.Play("Idle_" + this.direction.ToString(), 0);
                        yield return StartCoroutine(Wait(Random.Range(5, 15)));
                    }
                    else
                    {
                        anim.Play("Sleep", 0);
                        while (data.Sleep < data.MaxSleep && !isAbort)
                        {
                            data.Sleep += 1 * Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                
            }

            if (equipment != null)
            {
                equipment.RemovePet(this);
                equipment = null;
            }

            charInteract.interactType = InteractType.None;
        }
        CheckAbort();
    }

    protected virtual IEnumerator Call()
    {
        Vector3 pos = Camera.main.transform.position;
        pos.y = ItemManager.instance.roomBoundY.x + Random.Range (2,8f);
        pos.x += Random.Range(-15f, 15f);
        target = pos;
        if(charType == CharType.Parrot)
            yield return StartCoroutine(WalkToPoint());
        else
            yield return StartCoroutine(RunToPoint());
        int ran = Random.Range(0, 100);
        if (ran < 30)
        {
            yield return StartCoroutine(DoAnim("Standby"));
        }
        else if(ran < 60)
        {
            MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false,this.transform.position);
            yield return DoAnim("Speak_" + direction.ToString());
        }
        else
        {
            yield return DoAnim("Love");
        }

        float t = 0;
        float maxTime = Random.Range(3,5);
        while (t < maxTime && !isAbort)
        {
            anim.Play("Idle_" + this.direction.ToString(), 0);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Discover()
    {
        BaseFloorItem toyItem = ItemManager.instance.FindFreeRandomItem(ItemType.Toy);
        if (toyItem != null)
        {
            target = toyItem.startPoint.transform.position;
            yield return StartCoroutine(RunToPoint());
            if (toyItem != null && !toyItem.IsActive() && !isAbort)
            {
                equipment = toyItem;
                actionType = ActionType.Toy;
                isAbort = true;
            }
        }
        CheckAbort();
    }

    protected virtual IEnumerator Stop()
    {
        yield return StartCoroutine(Wait(anim.GetCurrentAnimatorStateInfo(0).length));
        anim.Play("Idle_" + direction.ToString());
        yield return StartCoroutine(Wait(Random.Range(1f, 2f)));
        CheckAbort();
    }

    protected virtual IEnumerator Bath()
    {
        float value = 0;

        if (data.level >= 20 && (equipment == null || equipment.itemType != ItemType.Bath))
        {
            BaseFloorItem item = ItemManager.instance.FindFreeRandomItem(ItemType.Bath);
            if (item != null && item.startPoint != null)
            {
                target = equipment.startPoint.position;
                yield return StartCoroutine(RunToPoint());
                JumpIn(item);   
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Bath)
        {
            value = DataHolder.GetItem(equipment.itemID).value;
            charInteract.interactType = InteractType.Equipment;
            this.transform.position = equipment.GetAnchorPoint(this).position;
            agent.transform.position = equipment.GetAnchorPoint(this).position;
            Debug.Log(equipment.GetAnchorPoint(this).position);
            equipment.OnActive();
            MageManager.instance.PlaySound3D("Pee", false, this.transform.position);
        }

        while (this.data.Dirty > 0 && !isAbort)
        {
            anim.Play("Soap", 0);
            this.data.Dirty -= value/60 * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (equipment != null && equipment.itemType == ItemType.Bath)
        {
            equipment.DeActive();
            if (data.Dirty <= 1 && !isAbort)
            {
                GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.OnBath);
            }

            yield return StartCoroutine(DoAnim("Shake"));
            charInteract.interactType = InteractType.None;
            equipment.RemovePet(this);
            equipment.DeActive();
            equipment = null;
            yield return StartCoroutine(JumpOut());

        }

        CheckAbort();
    }


    protected virtual IEnumerator Pee()
    {
        float value = data.Pee;

        if (data.level >= 5 && (equipment == null || equipment.itemType != ItemType.Toilet))
        {
            BaseFloorItem item = ItemManager.instance.FindFreeRandomItem(ItemType.Toilet);
            if (item != null && item.startPoint != null)
            {
                target = item.startPoint.position;
                yield return StartCoroutine(RunToPoint());
                JumpIn(item);   
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Toilet)
        {
            charInteract.interactType = InteractType.Equipment;
            this.transform.position = equipment.GetAnchorPoint(this).position;
            agent.transform.position = equipment.GetAnchorPoint(this).position;
            equipment.OnActive();
        }

        if (!isAbort)
        {
            anim.Play("Pee", 0);
            MageManager.instance.PlaySound3D("PeeDrop", false,this.transform.position);
            Debug.Log("Pee");
           
            Vector3 pos = peePosition.position;
            pos.z = 10;
            ItemManager.instance.SpawnPee(pos, value);
            while (data.Pee > 0 && !isAbort)
            {
                data.Pee -= 10 * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Toilet)
        {
            if (data.pee <= 1 && !isAbort)
            {
                GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Pee);
            }
            equipment.RemovePet(this);
            equipment.DeActive();
            charInteract.interactType = InteractType.None;
            yield return StartCoroutine(JumpOut());
        }

        CheckAbort();
    }

    protected virtual IEnumerator Shit()
    {
        float value = data.Shit;
        if (data.level >= 5 && (equipment == null || equipment.itemType != ItemType.Toilet))
        {
            BaseFloorItem item = ItemManager.instance.FindFreeRandomItem(ItemType.Toilet);
            if (item != null && item.startPoint != null)
            { 
                target = item.startPoint.position;
                yield return StartCoroutine(RunToPoint());
                JumpIn(item);                
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Toilet)
        {
            charInteract.interactType = InteractType.Equipment;
            this.transform.position = equipment.GetAnchorPoint(this).position;
            agent.transform.position = equipment.GetAnchorPoint(this).position;
            equipment.OnActive();
        }

        if (!isAbort)
        {
            anim.Play("Shit", 0);
            MageManager.instance.PlaySound3D("Shit", false,this.transform.position);
            while (data.Shit > 0 && !isAbort)
            {
                data.Shit -= 5 * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if (!isAbort)
            {
                Vector3 pos = peePosition.position;
                pos.z = pos.y * 10 + 10;
                ItemManager.instance.SpawnShit(shitPosition.position, value);
            }
            else
            {
                data.Shit = value;
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Toilet && !isAbort)
        {
            if (data.shit <= 1 && !isAbort)
            {
                GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Shit);
            }
            equipment.RemovePet(this);
            equipment.DeActive();
            charInteract.interactType = InteractType.None;
            actionType = ActionType.Pee;
            isAbort = true;
        }

        CheckAbort();
    }

    protected virtual IEnumerator Eat()
    {
        if (equipment == null || equipment.itemType != ItemType.Food)
        {
            BaseFloorItem item = ItemManager.instance.FindFreeRandomItem(ItemType.Food);
            if(item != null && item.startPoint != null)
            {
                target = item.startPoint.position;
                yield return StartCoroutine(RunToPoint());
                JumpIn(item);
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Food)
        {
            EatItem item = equipment.GetComponent<EatItem>();
            if (item != null)
            {
                if (data.level >= 25)
                {
                    item.Fill();
                }
                bool isContinue = true;

                if (item != null && isContinue)
                {
                    MageManager.instance.PlaySound3D("Eat", false, this.transform.position);
                    anim.Play("Eat", 0);
                    yield return StartCoroutine(Wait(0.1f));
                    while (item != null && data.Food < data.MaxFood && !isAbort && isContinue)
                    {
                        data.Food += 2 * Time.deltaTime;
                        item.Eat(2 * Time.deltaTime);
                        if (!item.CanEat())
                        {
                            isContinue = false;
                        }
                        if (Vector2.Distance(this.transform.position, item.GetAnchorPoint(this).position) > 1f)
                            isContinue = false;
                        yield return new WaitForEndOfFrame();
                    }
                    if (item != null)
                    {
                        equipment.RemovePet(this);
                    }
                    if (data.Food >= data.MaxFood - 10)
                    {
                        GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Eat);
                    }
                }
                else
                {
                    int ran = Random.Range(0, 100);
                    if (ran < 30)
                    {
                        MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false, this.transform.position);
                        yield return StartCoroutine(DoAnim("Speak_" + direction.ToString()));
                    }
                    else
                    {
                        yield return StartCoroutine(DoAnim("Standby"));
                    }
                }
            }
        }
        CheckAbort();
    }

    protected virtual IEnumerator Drink()
    {
        if(equipment == null || equipment.itemType != ItemType.Drink)
        {
            BaseFloorItem item = ItemManager.instance.FindFreeRandomItem(ItemType.Drink);
            if (item != null && item.startPoint != null)
            {
                target = item.startPoint.position;
                yield return StartCoroutine(RunToPoint());
                JumpIn(item);
            }
        }
        
        if (equipment != null && equipment.itemType == ItemType.Drink)
        {
            EatItem item = equipment.GetComponentInChildren<EatItem>();
            if (item != null)
            {
                if (data.level >= 25)
                {
                    item.Fill();
                }
                bool isContinue = true;

                if (item != null && isContinue)
                {
                    int soundid = MageManager.instance.PlaySound3D("Drink", false, this.transform.position);
                    anim.Play("Drink", 0);
                    yield return StartCoroutine(Wait(0.1f));
                    while (item != null && data.Water < data.MaxWater && !isAbort && isContinue)
                    {
                        data.Water += 5 * Time.deltaTime;
                        item.Eat(5 * Time.deltaTime);
                        if (!item.CanEat())
                        {
                            isContinue = false;
                        }
                        if (Vector2.Distance(this.transform.position, item.GetAnchorPoint(this).position) > 1f)
                            isContinue = false;
                        yield return new WaitForEndOfFrame();
                    }
                    MageManager.instance.StopSound(soundid);
                    if(item != null)
                    {
                        equipment.RemovePet(this);
                    }
                    if (data.Water >= data.MaxWater - 10)
                    {
                        GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Drink);
                    }
                }
                else
                {
                    int ran = Random.Range(0, 100);
                    if (ran < 30)
                    {
                        MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false, this.transform.position);
                        yield return StartCoroutine(DoAnim("Speak_" + direction.ToString()));
                    }
                    else
                    {
                        yield return StartCoroutine(DoAnim("Standby"));
                    }
                }
            }
        }
        CheckAbort();
    }

    protected virtual IEnumerator Sleep()
    {

        float value = 0;

        if (data.level >= 15 && (equipment == null || equipment.itemType != ItemType.Bed))
        {
            BaseFloorItem bed = ItemManager.instance.FindFreeRandomItem(ItemType.Bed);
            if (bed != null)
            {
                if (bed.startPoint != null)
                {
                    target = bed.startPoint.position;
                    yield return StartCoroutine(RunToPoint());
                    JumpIn(bed);
                }
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Bed)
        {
            value = DataHolder.GetItem(equipment.itemID).value;
            charInteract.interactType = InteractType.Equipment;
            this.transform.position = equipment.GetAnchorPoint(this).position;
            agent.transform.position = equipment.GetAnchorPoint(this).position;
        }


        

        while (data.Sleep < data.MaxSleep && !isAbort)
        {
            if (data.Health < data.MaxHealth * 0.1f)
            {
                anim.Play("Sick", 0);
            }
            else if (data.Damage > data.MaxDamage * 0.9f)
            {
                anim.Play("Injured_L", 0);
            }
            else
            {
                anim.Play("Sleep", 0);
                data.Sleep += (1 + value) * Time.deltaTime;
                data.Health += 0.1f * Time.deltaTime;
                data.Damage -= 0.1f * Time.deltaTime;
            }

            yield return new WaitForEndOfFrame();
        }

        if (equipment != null && equipment.itemType == ItemType.Bed && !isAbort)
        {
            if (data.Sleep > data.MaxSleep - 1)
            {
                GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Sleep);
            }
            equipment.RemovePet(this);
            equipment.DeActive();
            charInteract.interactType = InteractType.None;
            yield return StartCoroutine(JumpOut());
        }
        CheckAbort();
    }

    protected virtual IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(1, 3);
        while (!isAbort && n < maxCount)
        {
            int ran = Random.Range(0, 100);
            if (ran < 30)
            {
                SetTarget(AreaType.All);
                yield return StartCoroutine(RunToPoint());
            }
            else if (ran < 60)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            else if (ran < 80)
            {
                anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            else
            {
                MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false,this.transform.position);
                yield return DoAnim("Speak_" + direction.ToString());
            }

            n++;
        }
        CheckAbort();
    }


    protected virtual IEnumerator Listening()
    {
        yield return StartCoroutine(DoAnim("Idle_" + direction.ToString()));
        CheckAbort();
    }



    protected virtual IEnumerator JumpOut()
    {
        if (equipment != null)
            agent.transform.position = equipment.endPoint.position;
        yield return new WaitForEndOfFrame();
        equipment = null;
    }

    protected void JumpIn(BaseFloorItem item)
    {
        if (isAbort)
            return;

        if (item != null && !item.IsBusy())
        {
            equipment = item;
            equipment.AddPet(this);
            agent.transform.position = equipment.GetAnchorPoint(this).position;
        }
        else
        {
            equipment = null;
            isAbort = true;
        }
    }

    protected IEnumerator JumpDown(float zSpeed, float ySpeed, float accelerator)
    {

        anim.Play("Hold", 0);
        float speed = ySpeed;
        //charScale.scalePosition = new Vector3(this.transform.position.x, this.transform.position.y - height, 0);
        charInteract.interactType = InteractType.Jump;
        MageManager.instance.PlaySound("Drag", false);
        while (charInteract.interactType == InteractType.Jump && !isAbort)
        {
            speed -= accelerator * Time.deltaTime;
            if (speed < -50)
                speed = -50;
            Vector3 pos1 = agent.transform.position;
            pos1.y += speed * Time.deltaTime + zSpeed * Time.deltaTime;
            charScale.scalePosition.y += zSpeed * Time.deltaTime;
            charScale.height += speed * Time.deltaTime;
            agent.transform.position = pos1;

            if (charScale.height <= 0)
            {
                this.transform.rotation = Quaternion.identity;
                charInteract.interactType = InteractType.None;
            }
            yield return new WaitForEndOfFrame();
        }
        charInteract.interactType = InteractType.None;

        yield return StartCoroutine(DoAnim("Drop"));

    }


    protected virtual IEnumerator Sick()
    {
        if (data.level >= 30)
        {
            BaseFloorItem item = ItemManager.instance.GetRandomItem(ItemType.MedicineBox);
            if (item != null)
            {
                target = item.GetAnchorPoint(this).position;
                yield return StartCoroutine(RunToPoint());
                OnHealth(SickType.Sick, data.MaxHealth);
            }
        }

        anim.Play("Sick", 0);
        Debug.Log("Sick");
        //while ((System.DateTime.Now - data.timeSick).TotalSeconds < data.MaxTimeSick && !isAbort)
        while (data.Health < data.MaxHealth * 0.7f && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        //yield return StartCoroutine(DoAnim("Love"));
        //timeWait.gameObject.SetActive(false);
        //GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Sick);
        //CheckEnviroment();
        CheckAbort();
    }

    protected virtual IEnumerator Injured()
    {
        if(data.level >= 30)
        {
            BaseFloorItem item = ItemManager.instance.GetRandomItem(ItemType.MedicineBox);
            if (item != null)
            {
                target = item.GetAnchorPoint(this).position;
                yield return StartCoroutine(RunToPoint());
                Debug.Log(data.MaxDamage);
                OnHealth(SickType.Injured, data.MaxDamage);
            }
        }

        anim.Play("Injured_L", 0);
        Debug.Log("Injured");
        while (data.Damage > data.MaxDamage * 0.3f && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }

        GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Injured);
        CheckEnviroment();
        CheckAbort();
    }

    protected virtual IEnumerator Tired()
    {
        anim.Play("Tired", 0);
        while (data.energy < data.MaxEnergy && !isAbort)
        {
            float delta = 10 * Time.deltaTime;
            data.Energy += delta;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Toy()
    {
        
        if (equipment.toyType == ToyType.Jump)
        {
            dropPosition = equipment.GetAnchorPoint(this).position + new Vector3(0, Random.Range(-1f, 1f), 0);
            agent.transform.position = dropPosition;
            yield return new WaitForEndOfFrame();
            while (equipment != null && !isAbort && data.Toy < data.MaxToy)
            {
                equipment.OnActive();

                int r = Random.Range(0, 100);
                if(r > 80)
                    MageManager.instance.PlaySound3D(charType.ToString() + "_Supprised", false,this.transform.position);
                MageManager.instance.PlaySound3D("Drag", false,this.transform.position);
                //anim.Play("Teased", 0);
                anim.Play("Play_" + equipment.toyType.ToString());
                shadow.GetComponent<SpriteRenderer>().enabled = false;
                yield return new WaitForEndOfFrame();
                float ySpeed = 30 * anim.GetCurrentAnimatorStateInfo(0).length / 2;
                if (anim.GetCurrentAnimatorStateInfo(0).length < 2)
                {
                    anim.speed = 0.5f;
                    ySpeed = 60 * anim.GetCurrentAnimatorStateInfo(0).length / 2;
                }
                charInteract.interactType = InteractType.Equipment;
                while (charInteract.interactType == InteractType.Equipment && !isAbort)
                {
                        
                    ySpeed -= 30 * Time.deltaTime;
                    Vector3 pos1 = agent.transform.position;
                    pos1.y += ySpeed * Time.deltaTime;

                    if (data.Toy == data.MaxToy)
                    {
                        pos1.x += 15 * Time.deltaTime;
                    }
                    agent.transform.position = pos1;

                    if (ySpeed < 0 && this.transform.position.y < dropPosition.y)
                    {
                        if (data.Toy == data.MaxToy)
                        {
                            MageManager.instance.PlaySound3D("whoosh_swish_med_03", false,this.transform.position);
                            yield return StartCoroutine(DoAnim("Drop"));

                        }
                        else
                        {
                            agent.transform.position = dropPosition;
                        }
                        charInteract.interactType = InteractType.None;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
            shadow.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (equipment.toyType == ToyType.Ball || equipment.toyType == ToyType.Car)
        {
            charScale.speedFactor = 1.5f;
            while (equipment != null && data.Toy < data.MaxToy && !isAbort)
            {
                anim.speed = 1.5f;
                    
                if (equipment.state != EquipmentState.Active)
                {
                    agent.Stop();
                    equipment.OnActive();
                    yield return StartCoroutine(DoAnim("Love"));
                        
                }

                if (equipment.startPoint != null)
                {
                    target = equipment.startPoint.transform.position;
                    yield return StartCoroutine(RunToPoint());
                }
                yield return new WaitForEndOfFrame();
            }               
        }
        else if (equipment.toyType == ToyType.Wheel)
        {
            charInteract.interactType = InteractType.Equipment;
            MageManager.instance.PlaySound3D("Wheel", false,this.transform.position);
            charScale.speedFactor = 2f;
            anim.speed = 2f;
            SetDirection(Direction.L);
            equipment.OnActive();
            while (equipment != null && data.Toy < data.MaxToy && !isAbort)
            {
                agent.transform.position = equipment.GetAnchorPoint(this).position;
                anim.Play("Run_" + this.direction.ToString(), 0);
                yield return new WaitForEndOfFrame();
            }
            charInteract.interactType = InteractType.None;
        }
        else if (equipment.toyType == ToyType.Slider || equipment.toyType == ToyType.Circle)
        {
            while (equipment != null && data.Toy < data.MaxToy && !isAbort)
            {
                equipment.OnActive();
                charInteract.interactType = InteractType.Equipment;
                agent.transform.position = equipment.startPoint.position;
                yield return StartCoroutine(DoAnim("Play_" + equipment.toyType.ToString()));
                if (equipment.endPoint != null && !isAbort)
                {
                    agent.transform.position = equipment.endPoint.position;
                    this.transform.position = agent.transform.position;
                    yield return new WaitForEndOfFrame();
                }
                charInteract.interactType = InteractType.None;
                target = equipment.startPoint.position;
                Debug.Log(target);
                yield return StartCoroutine(RunToPoint());
            }
            charInteract.interactType = InteractType.None;
        }
        else if (equipment.toyType == ToyType.Spring || equipment.toyType == ToyType.Swing || equipment.toyType == ToyType.Dance || equipment.toyType == ToyType.Fun)
        {
            equipment.OnActive();
            charInteract.interactType = InteractType.Equipment;
            while (equipment != null && data.Toy < data.MaxToy && !isAbort)
            {
                if (equipment.GetAnchorPoint(this) != null)
                {
                    agent.transform.position = equipment.GetAnchorPoint(this).position;
                }
                anim.Play("Play_" + equipment.toyType.ToString(), 0);
                yield return new WaitForEndOfFrame();
            }
            charInteract.interactType = InteractType.None;
            if (equipment.endPoint != null && !isAbort)
            {
                agent.transform.position = equipment.endPoint.position;
            }
        }
        else if (equipment.toyType == ToyType.Seesaw || equipment.toyType == ToyType.Sprinkler || equipment.toyType == ToyType.Carrier || equipment.toyType == ToyType.Flying)
        {
            
            charInteract.interactType = InteractType.Equipment;
            if (equipment.toyType != ToyType.Sprinkler)
                shadow.GetComponent<SpriteRenderer>().enabled = false;

            int n = Random.Range(0, 2);
            if(equipment.toyType == ToyType.Carrier || equipment.toyType == ToyType.Flying)
            {
                n = 0;
            }

            Transform anchorPoint = equipment.GetAnchorPoint(this);
            while (anchorPoint != null && !isAbort && data.Toy < data.MaxToy)
            {
                agent.transform.position = anchorPoint.position;
                if(n == 0)
                {
                    this.transform.rotation = anchorPoint.rotation;
                    this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, anchorPoint.localScale.z);
                }else
                {
                    this.transform.rotation = Quaternion.Euler(new Vector3(anchorPoint.rotation.x,anchorPoint.rotation.y + 180,anchorPoint.rotation.z));
                    this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, -anchorPoint.localScale.z);
                }
                if (equipment.pets.Count == equipment.anchorPoints.Length)
                {
                    anim.Play("Play_" + equipment.toyType.ToString(), 0);
                    equipment.OnActive();
                }
                else
                {
                    anim.Play("Wait_" + equipment.toyType.ToString(), 0);
                    equipment.DeActive();
                }
                yield return new WaitForEndOfFrame();
            }

                
            charInteract.interactType = InteractType.None;
            agent.transform.position = equipment.endPoint.position + new Vector3(Random.Range(-2f,2f),Random.Range(-1f,1f),0);
            shadow.GetComponent<SpriteRenderer>().enabled = true;
            this.transform.rotation = Quaternion.identity;
               
        }

        if (equipment != null)
        {
            equipment.RemovePet(this);
            equipment.DeActive();
        }


        if (!isAbort)
        {
            yield return StartCoroutine(DoAnim("Love"));
            int value = (int)DataHolder.GetItem(equipment.itemID).value;
        }

        charScale.speedFactor = 1f;
        anim.speed = 1f;
        equipment = null;

        
        
        CheckAbort();
    }

    protected virtual IEnumerator Happy()
    {
        yield return StartCoroutine(DoAnim("Love"));
        CheckAbort();
    }

    protected virtual IEnumerator Itchi()
    {
        anim.Play("Itching_" + direction.ToString(), 0);
        Debug.Log("Itchi");
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Supprised()
    {
        MageManager.instance.PlaySound3D(charType.ToString() + "_Supprised", false,this.transform.position);
        yield return StartCoroutine(DoAnim("Teased"));
        CheckAbort();
    }

    protected virtual IEnumerator City()
    {
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Control()
    {
        MageManager.instance.PlaySound("Drag", false);
        charInteract.interactType = InteractType.Touch;
        ItemManager.instance.SetCameraTarget(this.gameObject);
        anim.Play("Hold", 0);
        if (shadow != null)
            shadow.SetActive(true);
        while (charInteract.interactType == InteractType.Touch)
        {
            Vector3 pos = target;
            pos.z = 0;
            if (pos.y > charScale.maxHeight)
                pos.y = charScale.maxHeight;
            else if (pos.y < -24)
                pos.y = -24;

            if (pos.x > 52)
                pos.x = 52;
            else if (pos.x < -49)
                pos.x = -49;

            pos.z = -50;
            agent.transform.position = pos;

            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.identity, Time.deltaTime * 2);
            yield return new WaitForEndOfFrame();
        }

        float fallSpeed = 0;

        while (charInteract.interactType == InteractType.Drop)
        {
            if (agent.transform.position.y > dropPosition.y)
            {
                fallSpeed += 100f * Time.deltaTime;
                if (fallSpeed > 50)
                    fallSpeed = 50;
                Vector3 pos1 = agent.transform.position;
                pos1.y -= fallSpeed * Time.deltaTime;
                pos1.x = Mathf.Lerp(pos1.x, dropPosition.x, Time.deltaTime * 5);
                pos1.z = charScale.scalePosition.z;
                agent.transform.position = pos1;
            }
            else
            {
                this.transform.rotation = Quaternion.identity;
                Vector3 pos3 = agent.transform.position;
                pos3.y = dropPosition.y;
                pos3.x = dropPosition.x;
                agent.transform.position = pos3;
                charScale.height = dropPosition.y - charScale.scalePosition.y;
                charInteract.interactType = InteractType.None;
            }
            yield return new WaitForEndOfFrame();
        }
        ItemManager.instance.ResetCameraTarget();
        charInteract.interactType = InteractType.None;

        CheckEnviroment();
        MageManager.instance.PlaySound3D("whoosh_swish_med_03", false,this.transform.position);
        yield return StartCoroutine(DoAnim("Drop"));


        CheckAbort();
    }

    protected virtual IEnumerator Gift()
    {
        yield return StartCoroutine(JumpDown(0, 20, 30));
        //syield return StartCoroutine(JumpOut());
        yield return StartCoroutine(DoAnim("Love"));
        CheckAbort();
    }

    protected virtual IEnumerator Fall()
    {
        MageManager.instance.PlaySound3D("Fall", false,this.transform.position);

        data.Damage += Random.Range(2, 10)/data.level;
        MageManager.instance.PlaySound3D("Drop", false, this.transform.position);
        yield return StartCoroutine(DoAnim("Fall_" + direction.ToString()));

        CheckAbort();
    }


    

    protected void CheckAbort()
    {
        agent.maxSpeed = data.speed;
        isAction = false;
        if (!isAbort)
            actionType = ActionType.None;
        else
        {
            DoAction();
        }
    }

    protected void CheckEnviroment()
    {
        if (equipment != null)
        {
            
            Debug.Log(equipment.itemType);
            if (equipment.itemType == ItemType.Bed)
            {
                equipment.AddPet(this);
                OnSleep();
            }
            else if (equipment.itemType == ItemType.Bath)
            {
                equipment.AddPet(this);
                OnBath();
            }
            else if (equipment.itemType == ItemType.Toilet)
            {
                equipment.AddPet(this);
                OnToilet();
            }
            else if (equipment.itemType == ItemType.Table)
            {
                equipment.AddPet(this);
                OnTable();
            }
            else if (equipment.itemType == ItemType.Food)
            {
                equipment.AddPet(this);
                OnEat();
            }
            else if (equipment.itemType == ItemType.Drink)
            {
                equipment.AddPet(this);
                OnDrink();
            }
            else if (equipment.itemType == ItemType.Toy)
            {
                equipment.AddPet(this);
                actionType = ActionType.Toy;
                isAbort = true;
                lastEquipment = equipment;
            }
            else
            {
                equipment = null;
            }
        }
    }

    #endregion

    #region Effect



    #endregion

    public MouseController GetMouse()
    {
        if (mouse == null)
            mouse = FindObjectOfType<MouseController>();
        return mouse;
    }

    #region  getpoint


    public void SetTarget(AreaType type)
    {
        target = ItemManager.instance.GetRandomPoint(type);       
    }

    
    #endregion

    void OnDestroy()
    {
        if (agent != null)
            GameObject.Destroy(agent.gameObject);
    }

}



