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
    
    float maxTimeLove = 30;

    //Movement
    //[HideInInspector]
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

    //Tease
    public CharController petTarget;

    //[HideInInspector]
    public GameObject shadow;
    [HideInInspector]
    public Vector3 originalShadowScale;

    TextMeshPro petNameText;
    //public SpriteRenderer petEmotion;
    //public EmotionStatus emotionStatus = EmotionStatus.Happy;
    //Sprite[] emotionIcons = new Sprite[3];

    #endregion

    //Skill
    [HideInInspector]
    public SkillType currentSkill = SkillType.NONE;
    public List<Skill> skills = new List<Skill>();
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

    //Balance
    float eatRate = 30;
    float drinkRate = 15;
    float peeRate = 15;
    float shitRate = 30;
    float sleepRate = 180;
    float bathRate = 45;
    float toyRate = 60;


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
            if (GameManager.instance.IsPreviousData())
            {
                Pet p = new Pet(pet.iD);
                p.realId = pet.realId;
                p.level = pet.level;
                this.data = p;
                this.ResetData();
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
            this.ResetData();
        }

        LoadSkill();
        if(data.actionType == ActionType.OnBath)
        {
            data.Dirty = Random.Range(0, data.MaxDirty * 0.3f);
        }else if(data.actionType == ActionType.Sleep)
        {
            data.Sleep = Random.Range(data.MaxSleep * 0.7f, data.MaxSleep);
        }
        else if (data.actionType == ActionType.Toy)
        {
            data.Sleep = Random.Range(data.MaxToy * 0.7f, data.MaxToy);
        }
        else if (data.actionType == ActionType.Pee)
        {
            data.Dirty = Random.Range(0, data.MaxPee * 0.3f);
        }
        else if (data.actionType == ActionType.Shit)
        {
            data.Dirty = Random.Range(0, data.MaxShit * 0.3f);
        }
        else if (data.actionType == ActionType.Eat)
        {
            data.Sleep = Random.Range(data.MaxFood * 0.7f, data.MaxFood);
        }
        else if (data.actionType == ActionType.Drink)
        {
            data.Sleep = Random.Range(data.MaxWater * 0.7f, data.MaxWater);
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

        //if(petEmotion != null)
        //    Destroy(petEmotion.gameObject);

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
                shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.3f);
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

        //GameObject go1 = GameObject.Instantiate(Resources.Load("Prefabs/Pets/PetEmotion")) as GameObject;
        //petEmotion = go1.GetComponent<SpriteRenderer>();
        //go1.transform.parent = this.transform;
        //go1.transform.localPosition = new Vector3(-4f, iconStatusObject.transform.localPosition.y - 2.5f, -10);
        //petEmotion.gameObject.SetActive(true);


        iconStatusObject.transform.parent = charObject.transform.GetChild(0);
        data.petName = GameManager.instance.GetPet(data.realId).petName;
        GameObject nameObject = GameObject.Instantiate(Resources.Load("Prefabs/Pets/PetNamePrefab")) as GameObject;

        nameObject.transform.position = iconStatusObject.transform.position - new Vector3(0, 2.5f, 10);
        petNameText = nameObject.GetComponent<TextMeshPro>();
        //petNameText.text = "<color=green>" + data.level.ToString() + " </color>" + data.petName;
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

            //emotionIcons[0] = Resources.Load<Sprite>("Icons/Status/Happy") as Sprite;
            //emotionIcons[1] = Resources.Load<Sprite>("Icons/Status/Normal") as Sprite;
            //emotionIcons[2] = Resources.Load<Sprite>("Icons/Status/Sad") as Sprite;
            //petEmotion.gameObject.SetActive(false);
        }
    }

    void LoadSkill()
    {
        skills.Clear();
        for(int i = 0; i < 5; i++)
        {
            Skill s = new Skill();
            if (i == 0)
            {
                s.type = SkillType.Toy;
                s.isLearn = true;
            }
            else if (i == 1)
            {
                s.type = SkillType.Happy;
                s.isLearn = true;
            }
            else if (i == 2)
            {
                s.type = SkillType.Toilet;
                if (data.level >= 5 * (i + 1))
                    s.isLearn = true;
                else
                    s.isLearn = false;
            }
            else if (i == 3)
            {
                s.type = SkillType.Sleep;
                if (data.level >= 5 * (i + 1))
                    s.isLearn = true;
                else
                    s.isLearn = false;
            }
                
            else if (i == 4)
            {
                s.type = SkillType.Bath;
                if (data.level >= 5 * (i + 1))
                    s.isLearn = true;
                else
                    s.isLearn = false;
            }
                


            skills.Add(s);
        }
    }

    public bool IsLearnSkill(SkillType type)
    {
        foreach(Skill s in skills)
        {
            if(s.type == type)
            {
                return s.isLearn;
            }
        }
        return false;
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

        if (actionType == ActionType.None)
        {
            Think();
            DoAction();
        }
        else if(actionType == ActionType.Toy)
        {

            petNameText.transform.rotation = Quaternion.identity;

            if (equipment != null && equipment.IsActive())
            {
                data.Toy +=  data.MaxToy/toyRate * Time.deltaTime;
            }
        }

        petNameText.transform.position = iconStatusObject.transform.position - new Vector3(0, 2.5f, 10);
        //petEmotion.transform.position = iconStatusObject.transform.position - new Vector3(4f, 2.3f, 10);

        /*
        if (emotionStatus != EmotionStatus.Sad)
        {
            if(data.timeLove > maxTimeLove && actionType != ActionType.Hold)
            {
                //StartCoroutine(OnEmotion());
                if(!IsLearnSkill(SkillType.Happy))
                    ItemManager.instance.SpawnPetHappy(this.charScale.scalePosition,3 * (data.RateHappy + data.level / 5));
                else
                    ItemManager.instance.SpawnHeart(3*(data.RateHappy + data.level / 5), this.transform.position);
                data.timeLove = 0;
            }else
                data.timeLove += Time.deltaTime;
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
        }*/

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
            data.Food -= 0.2f;
            data.Water -= 0.3f;
            data.Shit += 0.2f;
            data.Pee += 0.3f;
        }


        data.Dirty += 0.15f;
        data.Sleep -= 0.1f;

        if(actionType != ActionType.Toy)
            data.Toy -= 0.4f;

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
            if(IsLearnSkill(SkillType.Bath))
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

        if (data.Toy < data.MaxToy * 0.1f && IsLearnSkill(SkillType.Toy)) 
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Discover;
                return;
            }
        }

        if (data.Shit > data.MaxShit * 0.7f && IsLearnSkill(SkillType.Toilet))
        {
            int ran = Random.Range(0, 100);
            if (ran > 70)
            {
                actionType = ActionType.Shit;
                return;
            }
        }

        if (data.Pee > data.MaxPee * 0.7f && IsLearnSkill(SkillType.Toilet))
        {
            int ran = Random.Range(0, 100);
            if (ran > 70)
            {
                actionType = ActionType.Pee;
                return;
            }
        }

        if (data.Dirty > data.MaxDirty * 0.7f && IsLearnSkill(SkillType.Bath))
        {
            int ran = Random.Range(0, 100);
            if (ran > 70)
            {
                actionType = ActionType.OnBath;
                return;
            }
        }

        if (data.Food < data.MaxFood * 0.3f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 70)
            {
                actionType = ActionType.Eat;
                return;
            }
        }


        if (data.Water < data.MaxWater * 0.3f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 70)
            {
                actionType = ActionType.Drink;
                return;
            }
        }

        if (data.Sleep < data.MaxSleep * 0.3f && IsLearnSkill(SkillType.Sleep))
        {
            int ran = Random.Range(0, 100);
            if (ran > 70)
            {
                actionType = ActionType.Sleep;
                return;
            }
        }

        if (data.Toy < data.MaxToy * 0.3f && IsLearnSkill(SkillType.Toy))
        {
            int ran = Random.Range(0, 100);
            if (ran > 70)
            {
                actionType = ActionType.Discover;
                return;
            }
        }

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
        else if (actionType == ActionType.Love)
        {
            StartCoroutine(Love());
        }
        else if (actionType == ActionType.Tease)
        {
            StartCoroutine(Tease());
        }
        else if (actionType == ActionType.Teased)
        {
            StartCoroutine(Teased());
        }
    }

    #endregion

    protected virtual void CalculateStatus()
    {
        //lastEmotionStatus = emotionStatus;
        lastIconStatus = iconStatus;

        if (actionType == ActionType.Injured)
        {
            iconStatus = IconStatus.Bandage;
            charStatus.SetProgress((data.MaxDamage - data.Damage) / data.MaxDamage);
        }
        else if (actionType == ActionType.Sick)
        {
            iconStatus = IconStatus.MedicineBox;
            charStatus.SetProgress(data.Health / data.MaxHealth);
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
        }
        else if (data.Health < 0.1f * data.MaxHealth)
        {
            iconStatus = IconStatus.MedicineBox;
            charStatus.SetProgress(data.Health / data.MaxHealth);
        }
        else if (data.Pee > 0.9f * data.MaxPee)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxPee - data.Pee) / data.MaxPee);
        }
        else if (data.Pee > 0.9f * data.MaxShit)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxShit - data.Shit) / data.MaxShit);
        }
        else if (data.Food < 0.1f * data.MaxFood)
        {
            iconStatus = IconStatus.Food;
            charStatus.SetProgress(data.Food / data.MaxFood);
        }
        else if (data.Water < 0.1f * data.MaxWater)
        {
            iconStatus = IconStatus.Drink;
            charStatus.SetProgress(data.Water / data.MaxWater);
        }
        else if (data.Sleep < 0.1f * data.MaxSleep)
        {
            iconStatus = IconStatus.Bed;
            charStatus.SetProgress(data.Sleep / data.MaxSleep);
        }
        else if (data.Toy < 0.1f * data.MaxToy)
        {
            iconStatus = IconStatus.Toy;
            charStatus.SetProgress(data.Toy / data.MaxToy);
        }
        else if (data.Dirty > 0.9f * data.MaxDirty)
        {
            iconStatus = IconStatus.Bath;
            charStatus.SetProgress((data.MaxDirty - data.Dirty) / data.MaxDirty);
        }
        else if (data.Damage > 0.7f * data.MaxDamage)
        {
            iconStatus = IconStatus.Bandage;
            charStatus.SetProgress((data.MaxDamage - data.Damage) / data.MaxDamage);
        }
        else if (data.Health < 0.3f * data.MaxHealth)
        {
            iconStatus = IconStatus.MedicineBox;
            charStatus.SetProgress(data.Health / data.MaxHealth);
        }
        else if (data.Pee > 0.7f * data.MaxPee)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxPee - data.Pee) / data.MaxPee);
        }
        else if (data.Pee > 0.7f * data.MaxShit)
        {
            iconStatus = IconStatus.Toilet;
            charStatus.SetProgress((data.MaxShit - data.Shit) / data.MaxShit);
        }
        else if (data.Food < 0.3f * data.MaxFood)
        {
            iconStatus = IconStatus.Food;
            charStatus.SetProgress(data.Food / data.MaxFood);
        }
        else if (data.Water < 0.3f * data.MaxWater)
        {
            iconStatus = IconStatus.Drink;
            charStatus.SetProgress(data.Water / data.MaxWater);
        }
        else if (data.Sleep < 0.3f * data.MaxSleep)
        {
            iconStatus = IconStatus.Bed;
            charStatus.SetProgress(data.Sleep / data.MaxSleep);
        }
        else if (data.Toy < 0.3f * data.MaxToy)
        {
            iconStatus = IconStatus.Toy;
            charStatus.SetProgress(data.Toy / data.MaxToy);
        }
        else if (data.Dirty > 0.7f * data.MaxDirty)
        {
            iconStatus = IconStatus.Bath;
            charStatus.SetProgress((data.MaxDirty - data.Dirty) / data.MaxDirty);
        }
        else
        {
            iconStatus = IconStatus.None;
        }

        /*
        if (data.Damage > 0.9f * data.MaxDamage || data.Health < 0.1f * data.MaxHealth || data.Pee > 0.9f * data.MaxPee || data.Pee > 0.9f * data.MaxShit
            || data.Food < 0.1f * data.MaxFood || data.Water < 0.1f * data.MaxWater || data.Sleep < 0.1f * data.MaxSleep || data.Toy < 0.1f * data.MaxToy
            || data.Dirty > 0.9f * data.MaxDirty)
        {
            emotionStatus = EmotionStatus.Sad;
        }else if (data.Damage > 0.7f * data.MaxDamage || data.Health < 0.3f * data.MaxHealth || data.Pee > 0.7f * data.MaxPee || data.Pee > 0.7f * data.MaxShit
            || data.Food < 0.3f * data.MaxFood || data.Water < 0.3f * data.MaxWater || data.Sleep < 0.3f * data.MaxSleep || data.Toy < 0.3f * data.MaxToy
            || data.Dirty > 0.7f * data.MaxDirty)
        {
            emotionStatus = EmotionStatus.Normal;
        }else
            emotionStatus = EmotionStatus.Happy;
        */

        if (iconStatusObject != null)
        {
            LoadIconStatus();
        }

        //if(emotionStatus != lastEmotionStatus)
        //{
            //StartCoroutine(OnEmotion());
        //}
        //iconStatusObject.transform.localScale = originalStatusScale / charScale.scaleAgeFactor;

    }

    IEnumerator OnEmotion()
    {
        //petEmotion.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        //petEmotion.gameObject.SetActive(false);
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
        if (charInteract.interactType != InteractType.Drag) {
            if (agent.transform.eulerAngles.z < 180f && agent.transform.eulerAngles.z > 0f || (agent.transform.eulerAngles.z > -360f && agent.transform.eulerAngles.z < -180f))
                direction = Direction.L;
            else
                direction = Direction.R;
        } 

        if (direction == Direction.R)
        {
            this.charObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            this.charObject.transform.localScale = new Vector3(this.charObject.transform.localScale.x, this.charObject.transform.localScale.y, -1);

        }
        else
        {
            this.charObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            this.charObject.transform.localScale = new Vector3(this.charObject.transform.localScale.x, this.charObject.transform.localScale.y, 1);
        }
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

    public virtual void OnLove()
    {
        Abort();
        actionType = ActionType.Love;
    }

    public virtual void OnTease(CharController c)
    {
        if (actionType == ActionType.Hold || actionType == ActionType.Sick || actionType == ActionType.Injured || equipment != null || actionType == ActionType.Teased | actionType == ActionType.Tease)
            return;
        petTarget = c;
        Abort();
        actionType = ActionType.Tease;
    }

    public virtual void OnTeased()
    {
        if (actionType == ActionType.Hold || actionType == ActionType.Sick || actionType == ActionType.Injured || equipment != null || actionType == ActionType.Teased | actionType == ActionType.Tease)
            return;
        Abort();
        actionType = ActionType.Teased;
    }


    public virtual void OnSupprised()
    {
        if (actionType == ActionType.Hold)
            return;

        Abort();
        actionType = ActionType.Supprised;
    }

    public virtual void OnEat()
    {
        if (actionType == ActionType.Eat)
            return;
        actionType = ActionType.Eat;
        isAbort = true;        
    }

    public virtual void OnDrink()
    {
        if (actionType == ActionType.Drink)
            return;
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
        DoAction();
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

        if (actionType == ActionType.Patrol || actionType == ActionType.Discover)
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
        SetName();
        LoadSkill();
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
        charScale.speedFactor = 1;
        if (Vector2.Distance(this.transform.position,target) > 1)
        {
            agent.SetDestination(target);
            while (!isArrived && !isAbort)
            {
                anim.Play("Run_L", 0);
                yield return new WaitForEndOfFrame();
            }
            
        }
        else
        {
            isArrived = true;
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
            anim.Play("Walk_L", 0);
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
            anim.Play("Run_Angry_L", 0);
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

        charCollider.CheckHighlight();
       
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

 

            pos.z = -50;
            agent.transform.position = Vector3.Lerp(this.transform.position,pos,Time.deltaTime * 5);
            if(agent.transform.position.x >= lastPosition.x)
            {
                SetDirection(Direction.R);
            }else
                SetDirection(Direction.L);
            lastPosition = this.transform.position;
            yield return new WaitForEndOfFrame();
        }

        charCollider.OffAllItem();
        //this.charObject.transform.rotation = Quaternion.identity;
        dropPosition = charScale.scalePosition;
        bool dropOutSide = false;
        //Start Drop
        
        if (charCollider.items.Count > 0)
        {
            equipment = charCollider.items[0];
            if (!equipment.IsBusy())
            {
                if (equipment.itemType != ItemType.Bath && equipment.itemType != ItemType.Bed && equipment.itemType != ItemType.Toilet && equipment.itemType != ItemType.Table && (data.Health < data.MaxHealth * 0.1f || data.Damage > data.MaxDamage * 0.9f))
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
                    if (ran < 10)
                    {
                        MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false, this.transform.position);
                        yield return StartCoroutine(DoAnim("Speak_L"));
                    }
                    else if (ran < 60)
                    {
                        anim.Play("Idle_L", 0);
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
        if (ran < 40)
        {
            yield return StartCoroutine(DoAnim("Standby"));
        }
        else if(ran < 60)
        {
            MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false,this.transform.position);
            yield return DoAnim("Speak_L");
        }
        else
        {
            yield return DoAnim("Love");
        }

        float t = 0;
        float maxTime = Random.Range(3,5);
        while (t < maxTime && !isAbort)
        {
            anim.Play("Idle_L", 0);
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
        yield return StartCoroutine(DoAnim("Idle_L"));
        CheckAbort();
    }

    protected virtual IEnumerator Love()
    {
        yield return StartCoroutine(DoAnim("Love"));
        CheckAbort();
    }

    protected virtual IEnumerator Bath()
    {
        float startValue = data.Dirty;
        float value = Random.Range(0, 0.3f * data.MaxDirty);
        if (IsLearnSkill(SkillType.Bath) && (equipment == null || equipment.itemType != ItemType.Bath))
        {
            BaseFloorItem item = ItemManager.instance.FindFreeRandomItem(ItemType.Bath);
            if (item != null && item.startPoint != null)
            {
                target = item.startPoint.position;
                yield return StartCoroutine(RunToPoint());
                JumpIn(item);   
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Bath)
        {
            charInteract.interactType = InteractType.Equipment;
            this.transform.position = equipment.GetAnchorPoint(this).position;
            agent.transform.position = equipment.GetAnchorPoint(this).position;
            equipment.OnActive();
            int soundId = MageManager.instance.PlaySound3D("Pee", false, this.transform.position);

            float maxTime = Random.Range(1, 5);
            float t = 0;
            int n = 0;
            anim.Play("Soap", 0);
            while (this.data.Dirty > value && !isAbort)
            {
                if (data.Health < 0.1f * data.MaxHealth)
                    anim.Play("Sick", 0);
                else if (data.Damage > 0.9f * data.MaxDamage)
                    anim.Play("Injured_L", 0);
                else
                {
                    if (t > maxTime)
                    {
                        t = 0;
                        maxTime = Random.Range(1, 5);
                        n = Random.Range(0, 100);

                        if (n < 30)
                            anim.Play("Soap", 0);
                        else if(n < 70)
                            anim.Play("Shower", 0);
                        else
                        {
                            anim.Play("Shake", 0);
                            maxTime = 1;
                        }
                    }
                    else
                        t += Time.deltaTime;
                    
                }
                    
                this.data.Dirty -= data.MaxDirty / bathRate * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            MageManager.instance.StopSound(soundId);



            if (equipment != null && equipment.itemType == ItemType.Bath)
            {
                if (startValue - data.Dirty > data.MaxDirty * 0.1f)
                {
                    ItemManager.instance.SpawnHeart(data.RateHappy + data.level/5, this.transform.position);
                    GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.OnBath);
                }

                yield return StartCoroutine(DoAnim("Shake"));
                JumpOut();
            }
        }



        CheckAbort();
    }


    protected virtual IEnumerator Pee()
    {
        float pee = data.Pee;
        float value = Random.Range(0, 0.3f * data.MaxPee);

        if (IsLearnSkill(SkillType.Toilet) && (equipment == null || equipment.itemType != ItemType.Toilet))
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
            ItemManager.instance.SpawnPee(pos, pee);
            while (data.Pee > value && !isAbort)
            {
                if (equipment != null && equipment.itemType == ItemType.Toilet && !isAbort)
                {
                    this.transform.position = equipment.GetAnchorPoint(this).position;
                }
                data.Pee -= data.MaxPee/peeRate * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Toilet)
        {
            if (pee - data.Pee > data.MaxPee * 0.1f)
            {
                ItemManager.instance.SpawnHeart(data.RateHappy + data.level/5, this.transform.position);
                GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Pee);
            }
            JumpOut();
        }

        CheckAbort();
    }

    protected virtual IEnumerator Shit()
    {
        float shit = data.Shit;
        float value = Random.Range(0, 0.3f * data.MaxShit);
        if (IsLearnSkill(SkillType.Toilet) && (equipment == null || equipment.itemType != ItemType.Toilet))
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
            while (data.Shit > value && !isAbort)
            {
                data.Shit -= data.MaxShit/shitRate * Time.deltaTime;
                if (equipment != null && equipment.itemType == ItemType.Toilet && !isAbort)
                {
                    this.transform.position = equipment.GetAnchorPoint(this).position;
                }
                yield return new WaitForEndOfFrame();
            }
            if (!isAbort)
            {
                Vector3 pos = peePosition.position;
                pos.z = pos.y * 10 + 10;
                ItemManager.instance.SpawnShit(shitPosition.position, shit);
            }
            else
            {
                data.Shit = shit;
            }
        }

        if (equipment != null && equipment.itemType == ItemType.Toilet && !isAbort)
        {
            if (shit - data.Shit > 0.1f * data.MaxShit)
            {
                ItemManager.instance.SpawnHeart(data.RateHappy + data.level/5, this.transform.position);
                GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Pee);
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
        float startValue = data.Food;
        float value = Random.Range(0.7f * data.MaxFood,data.MaxFood);
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
            if (item != null && item.CanEat() && Vector2.Distance(this.transform.position, item.GetAnchorPoint(this).position) <= 1f)
            {
                bool isContinue = true;
                SetDirection(Direction.L);

                MageManager.instance.PlaySound3D("Eat", false, this.transform.position);
                anim.Play("Eat", 0);
               //yield return StartCoroutine(Wait(0.1f));
                while (item != null && data.Food < value && !isAbort && isContinue)
                {
                    data.Food += data.MaxFood/eatRate * Time.deltaTime;
                    item.Eat(data.MaxFood / eatRate * Time.deltaTime);
                    if (!item.CanEat())
                    {
                        isContinue = false;
                    }
                    if (Vector2.Distance(this.transform.position, item.GetAnchorPoint(this).position) > 1f)
                        isContinue = false;
                    yield return new WaitForEndOfFrame();
                }
                if (data.Food - startValue >= 0.1f * data.MaxFood)
                {
                    ItemManager.instance.SpawnHeart(data.RateHappy + data.level/5, this.transform.position);
                    GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Eat);
                }
                
                JumpOut();
            }
            else
            {
                if (equipment != null)
                {
                    equipment.RemovePet(this);
                    equipment = null;
                }
                int ran = Random.Range(0, 100);
                if (ran < 30)
                {
                    MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false, this.transform.position);
                    yield return StartCoroutine(DoAnim("Speak_L"));
                }
                else
                {
                    yield return StartCoroutine(DoAnim("Standby"));
                }

            }
            
        }

        CheckAbort();
    }

    protected virtual IEnumerator Drink()
    {
        float startValue = data.Water;
        float value = Random.Range(0.7f * data.MaxWater, data.MaxWater);
        if (equipment == null || equipment.itemType != ItemType.Drink)
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
            if (item != null && item.CanEat() && Vector2.Distance(this.transform.position, item.GetAnchorPoint(this).position) <= 1f)
            {
                bool isContinue = true;

                SetDirection(Direction.L);
                int soundid = MageManager.instance.PlaySound3D("Drink", false, this.transform.position);
                anim.Play("Drink", 0);
                yield return StartCoroutine(Wait(0.1f));
                while (item != null && data.Water < value && !isAbort && isContinue)
                {
                    data.Water += data.MaxWater / drinkRate * Time.deltaTime;
                    item.Eat(data.MaxWater / drinkRate * Time.deltaTime);
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
                if (data.Water - startValue > 0.1f * data.MaxWater)
                {
                    ItemManager.instance.SpawnHeart(data.RateHappy + data.level/5, this.transform.position);
                    GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Drink);
                }

                JumpOut();
            }
            else
            {
                if(equipment != null)
                {
                    equipment.RemovePet(this);
                    equipment = null;
                }

                int ran = Random.Range(0, 100);
                if (ran < 30)
                {
                    MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false, this.transform.position);
                    yield return StartCoroutine(DoAnim("Speak_L"));
                }
                else
                {
                    yield return StartCoroutine(DoAnim("Standby"));
                }
            }
           
        }
        CheckAbort();
    }

    protected virtual IEnumerator Sleep()
    {
        float startValue = data.Sleep;
        float value = Random.Range(0.7f * data.MaxSleep, data.MaxSleep);
        float sleepValue = 0;

        if (IsLearnSkill(SkillType.Sleep) && (equipment == null || equipment.itemType != ItemType.Bed))
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
            charInteract.interactType = InteractType.Equipment;
            this.transform.position = equipment.GetAnchorPoint(this).position;
            agent.transform.position = equipment.GetAnchorPoint(this).position;
            sleepValue = data.MaxSleep / sleepRate;
        }
        else
        {
            value = Random.Range(0.3f * data.MaxSleep, 0.5f * data.MaxSleep);
            sleepValue = data.MaxSleep / sleepRate / 2;
        }

        while (data.Sleep < value && !isAbort)
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
                data.Sleep += sleepValue * Time.deltaTime;
                data.Health += 0.1f * sleepValue * Time.deltaTime;
                data.Damage -= 0.1f * sleepValue * Time.deltaTime;
            }

            yield return new WaitForEndOfFrame();
        }

        if (equipment != null && equipment.itemType == ItemType.Bed)
        {
            if (data.Sleep - startValue > data.MaxSleep * 0.1f)
            {
                ItemManager.instance.SpawnHeart(data.RateHappy + data.level/5, this.transform.position);
                GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Sleep);
            }
            if(!isAbort)
                JumpOut();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Patrol()
    {
        int d = Random.Range(0, 100);
        if (d > 50)
            SetDirection(Direction.L);
        else
            SetDirection(Direction.R);
        int n = 0;
        int maxCount = Random.Range(3, 7);
        while (!isAbort && n < maxCount)
        {
            int ran = Random.Range(0, 100);
            if (ran < 60)
            {
                target = ItemManager.instance.GetPatrolPoint(this.transform.position);
                yield return StartCoroutine(RunToPoint());
            }
            else if (ran < 70)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 5)));
            }
            else if (ran < 90)
            {
                anim.Play("Idle_L", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 5)));
            }
            else
            {
                MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false,this.transform.position);
                yield return DoAnim("Speak_L");
            }

            n++;
        }
        CheckAbort();
    }


    protected virtual IEnumerator Listening()
    {
        yield return StartCoroutine(DoAnim("Idle_L"));
        CheckAbort();
    }

    protected virtual IEnumerator Tease()
    {
        yield return StartCoroutine(DoAnim("Tease"));
        if (petTarget != null)
        {
            charScale.speedFactor = 2f;
            anim.speed = 1.5f;
            yield return StartCoroutine(RunToPoint());
            int n = Random.Range(100, 500);
            while (petTarget != null && n > 0 && !isAbort)
            {
                agent.Stop();
                Vector3 pos = petTarget.transform.position;
                pos.z = 0;
                agent.SetDestination(pos);
                anim.Play("Run_L", 0);
                
               if (Vector2.Distance(this.transform.position, petTarget.transform.position) < 5)
                {
                    int r = Random.Range(0, 100);
                    petTarget.OnTeased();
                    agent.Stop();
                    yield return StartCoroutine(DoAnim("Speak_L"));
                    if(r > 70)
                        MageManager.instance.PlaySound3D(charType.ToString() + "_Speak", false, this.transform.position);
                }

                n--;
                yield return StartCoroutine(Wait(0.1f));
            }
        }
        charScale.speedFactor = 1f;
        anim.speed = 1;
        CheckAbort();
    }

    protected virtual IEnumerator Teased()
    {
        yield return StartCoroutine(DoAnim("Teased"));
        target = ItemManager.instance.GetRandomPoint(AreaType.All);
        charScale.speedFactor = 1.5f;
        anim.speed = 1.5f;
        yield return StartCoroutine(RunToPoint());
        charScale.speedFactor = 1f;
        anim.speed = 1;
        CheckAbort();
    }

    protected virtual void JumpOut()
    {
        if (equipment != null)
        {
            agent.transform.position = equipment.endPoint.position;
            equipment.RemovePet(this);
            equipment.DeActive();
        }
        charInteract.interactType = InteractType.None;
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
        /*
        if (data.level >= 30)
        {
            BaseFloorItem item = ItemManager.instance.GetRandomItem(ItemType.MedicineBox);
            if (item != null)
            {
                target = item.GetAnchorPoint(this).position;
                yield return StartCoroutine(RunToPoint());
                OnHealth(SickType.Sick, data.MaxHealth);
            }
        }*/

        anim.Play("Sick", 0);
        Debug.Log("Sick");
        while (data.Health < data.MaxHealth * 0.7f && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Injured()
    {
        /*
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
        }*/

        anim.Play("Injured_L", 0);
        Debug.Log("Injured");
        while (data.Damage > data.MaxDamage * 0.3f && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
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
        float startValue = data.Toy;
        float value = Random.Range(0.7f * data.MaxToy, data.MaxToy);
        
        if (equipment.toyType == ToyType.Jump)
        {
            
            dropPosition = equipment.GetAnchorPoint(this).position + new Vector3(0, Random.Range(-1f, 1f), 0);
            agent.transform.position = dropPosition;
            yield return new WaitForEndOfFrame();

            while (equipment != null && !isAbort && data.Toy < value)
            {
                int ran = Random.Range(0, 100);
                if (ran > 50)
                    SetDirection(Direction.L);
                else
                    SetDirection(Direction.R);
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

                    if (data.Toy == value)
                    {
                        pos1.x += 15 * Time.deltaTime;
                    }
                    agent.transform.position = pos1;

                    if (ySpeed < 0 && this.transform.position.y < dropPosition.y)
                    {
                        if (data.Toy == value)
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
            while (equipment != null && data.Toy < value && !isAbort)
            {
                anim.speed = 1.5f;
                    
                if (equipment.state != EquipmentState.Active && Vector2.Distance(this.transform.position,equipment.transform.position) < 5)
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
            //SetDirection(Direction.L);
            equipment.OnActive();
            while (equipment != null && data.Toy < value && !isAbort)
            {
                agent.transform.position = equipment.GetAnchorPoint(this).position;
                anim.Play("Run_L", 0);
                yield return new WaitForEndOfFrame();
            }
            charInteract.interactType = InteractType.None;
        }
        else if (equipment.toyType == ToyType.Slider || equipment.toyType == ToyType.Circle)
        {
            
            while (equipment != null && data.Toy < value && !isAbort)
            {
                SetDirection(Direction.L);
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
        else if (equipment.toyType == ToyType.Spring || equipment.toyType == ToyType.Swing)
        {
            equipment.OnActive();
            charInteract.interactType = InteractType.Equipment;
            while (equipment != null && data.Toy < value && !isAbort)
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
        else if (equipment.toyType == ToyType.Dance || equipment.toyType == ToyType.Fun)
        {
            equipment.OnActive();
            charInteract.interactType = InteractType.Equipment;
            float time = 0;
            float maxTime = Random.Range(3, 7);
            while (equipment != null && data.Toy < value && !isAbort)
            {
                if (equipment.GetAnchorPoint(this) != null)
                {
                    agent.transform.position = equipment.GetAnchorPoint(this).position;
                }
                anim.Play("Play_" + equipment.toyType.ToString(), 0);
                if(time > maxTime)
                {
                    time = 0;
                    maxTime = Random.Range(3, 7);
                    if (direction == Direction.L)
                        SetDirection(Direction.R);
                    else if (direction == Direction.R)
                        SetDirection(Direction.L);
                }
                time += Time.deltaTime;
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
            float t = 0;
            ToyCarrier carrier = equipment.GetComponent<ToyCarrier>();
            charInteract.interactType = InteractType.Equipment;
            if (equipment.toyType != ToyType.Sprinkler)
                shadow.GetComponent<SpriteRenderer>().enabled = false;

            int n = Random.Range(0, 2);
            if(equipment.toyType == ToyType.Carrier || equipment.toyType == ToyType.Flying)
            {
                n = 0;
            }

            Transform anchorPoint = equipment.GetAnchorPoint(this);
            while (anchorPoint != null && !isAbort && data.Toy < value)
            {
                agent.transform.position = anchorPoint.position;
                if(equipment.toyType == ToyType.Seesaw)
                {
                    this.transform.rotation = anchorPoint.rotation;
                    this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, anchorPoint.localScale.z);
                }else if(equipment.toyType == ToyType.Carrier || equipment.toyType == ToyType.Flying)
                {
                    SetDirection(carrier.direction);
                }

                if (equipment.pets.Count == equipment.anchorPoints.Length)
                {
                    anim.Play("Play_" + equipment.toyType.ToString(), 0);
                    equipment.OnActive();
                }
                else
                {
                    if (t > 15)
                        OnCall();
                    anim.Play("Wait_" + equipment.toyType.ToString(), 0);
                    equipment.DeActive();
                    t += Time.deltaTime;
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

        if (data.Toy - startValue > data.MaxToy * 0.1f)
        {
            ItemManager.instance.SpawnHeart(data.RateHappy + data.level/5, this.transform.position);
            GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Toy);
        }
            

        if (!isAbort)
        {
            yield return StartCoroutine(DoAnim("Love"));
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
        anim.Play("Itching_L", 0);
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

        if (equipment != null)
        {
            equipment.RemovePet(this);
            equipment.DeActive();
            equipment = null;
        }

        charCollider.CheckHighlight();

        MageManager.instance.PlaySound("Drag", false);
        charInteract.interactType = InteractType.Touch;

        ItemManager.instance.SetCameraTarget(this.gameObject);
        anim.Play("Hold", 0);
        if (shadow != null)
            shadow.SetActive(true);
        Vector3 lastPosition = this.transform.position;
        while (charInteract.interactType == InteractType.Touch)
        {
            Vector3 pos = target;

            pos.z = 0;
            if (pos.y > charScale.maxHeight)
                pos.y = charScale.maxHeight;
            else if (pos.y < ItemManager.instance.gardenBoundY.x)
                pos.y = ItemManager.instance.gardenBoundY.x;

            if (pos.x > ItemManager.instance.gardenBoundX.y)
                pos.x = ItemManager.instance.gardenBoundX.y;
            else if (pos.x < ItemManager.instance.gardenBoundX.x)
                pos.x = ItemManager.instance.gardenBoundX.x;

            pos.z = -50;
            agent.transform.position = pos;
            if (agent.transform.position.x >= lastPosition.x)
            {
                SetDirection(Direction.R);
            }
            else
                SetDirection(Direction.L);
            lastPosition = this.transform.position;
            yield return new WaitForEndOfFrame();
        }

        charCollider.OffAllItem();
        //this.charObject.transform.rotation = Quaternion.identity;
        dropPosition = charScale.scalePosition;
        bool dropOutSide = false;
        //Start Drop

        if (charCollider.items.Count > 0)
        {
            equipment = charCollider.items[0];
            if (!equipment.IsBusy())
            {
                if (equipment.itemType != ItemType.Bath && equipment.itemType != ItemType.Bed && equipment.itemType != ItemType.Toilet && equipment.itemType != ItemType.Table && (data.Health < data.MaxHealth * 0.1f || data.Damage > data.MaxDamage * 0.9f))
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
                if (equipment.anchorPoints.Length > 0)
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

        /*
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


        CheckAbort();*/
    }

    protected virtual IEnumerator Gift()
    {
        yield return StartCoroutine(JumpDown(0, 20, 30));
        yield return StartCoroutine(DoAnim("Love"));
        actionType = ActionType.None;
        isAbort = false;
        CheckAbort();
    }

    protected virtual IEnumerator Fall()
    {
        MageManager.instance.PlaySound3D("Fall", false,this.transform.position);

        data.Damage += Random.Range(10, 30);
        MageManager.instance.PlaySound3D("Drop", false, this.transform.position);
        yield return StartCoroutine(DoAnim("Fall_L"));

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

    public void ResetData()
    {
        data.Health = data.MaxHealth * 0.7f;
        data.Damage = data.MaxDamage * 0.3f;
        data.Food = data.MaxFood * 0.5f;
        data.Water = data.MaxWater * 0.5f;
        data.Dirty = data.MaxDirty * 0.6f;
        data.Pee = data.MaxPee * 0.3f;
        data.Shit = data.MaxShit * 0.3f;
        data.Sleep = data.MaxSleep * 0.7f;
        data.Toy = data.MaxToy * 0.7f;
    }

}



