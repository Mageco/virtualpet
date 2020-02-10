﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;


public class CharController : MonoBehaviour
{

    #region Declair
    //Data
    public Pet data;
    public GameObject petPrefab;
    public CharType charType = CharType.Dog;
    public GameType gameType = GameType.House;
    //[HideInInspector]

    //[HideInInspector]
    public EnviromentType enviromentType = EnviromentType.Room;
    //[HideInInspector]
    public Direction direction = Direction.L;

    //Think
    protected float dataTime = 0;
    protected float maxDataTime = 1f;

    //Movement
    [HideInInspector]
    public Vector3 target;

    [HideInInspector]
    public PolyNavAgent agent;
    public bool isArrived = true;
    public bool isAbort = false;
    public bool isAction = false;
    public bool isMoving = false;

    //Action
    public ActionType actionType = ActionType.None;
    public Animator anim;

    //Interact
    [HideInInspector]
    public CharInteract charInteract;
    [HideInInspector]
    public CharScale charScale;
    
    //Pee,Sheet
    public Transform peePosition;
    public Transform shitPosition;
  

    [HideInInspector]
    public GameObject shadow;
    [HideInInspector]
    public Vector3 originalShadowScale;

    #endregion

    //Skill
    [HideInInspector]
    public SkillType currentSkill = SkillType.NONE;
    FoodBowlItem foodItem;
    DrinkBowlItem drinkItem;
    MouseController mouse;

    [HideInInspector]
    public Vector3 dropPosition = Vector3.zero;

    public SpriteRenderer iconStatusObject;
    public TextMesh timeWait;
    public IconStatus iconStatus = IconStatus.None;
    IconStatus lastIconStatus = IconStatus.None;
    Vector3 originalStatusScale;

    protected ToyItem toyItem;

    [HideInInspector]
    public List<GameObject> dirties = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> dirties_L = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> dirties_LD = new List<GameObject>();

    #region Load

    void Awake()
    {

    }

    protected virtual void Start()
    {
        if (actionType != ActionType.None)
            DoAction();
    }


    public void LoadPrefab(){
        data.petName = DataHolder.GetPet(this.data.iD).GetName(MageManager.instance.GetLanguage());
        GameObject go = Instantiate(petPrefab) as GameObject;
        go.transform.parent = this.transform;
		go.transform.localPosition = Vector3.zero;    

        anim = go.transform.GetComponent<Animator>();
        charInteract = this.GetComponent<CharInteract>();
        charScale = this.GetComponent<CharScale>();

        GameObject go1 = Instantiate(Resources.Load("Prefabs/Pets/Agent")) as GameObject;
        //go1.transform.parent = GameManager.instance.transform;
		agent = go1.GetComponent<PolyNavAgent>();
        agent.OnDestinationReached += OnArrived;
        agent.transform.position = this.transform.position;
        
        agent.maxSpeed = data.speed;

        SetDirection(Direction.R);

        if (iconStatusObject != null)
        {
            iconStatusObject.gameObject.SetActive(false);
            originalStatusScale = iconStatusObject.transform.localScale;
        }

        if (timeWait != null)
            timeWait.gameObject.SetActive(false);


        //Load Dirty Effect
        //grab all the kids and only keep the ones with dirty tags
     
         Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>(true);
     
         foreach (Transform child in allChildren) {
             if (child.gameObject.tag == "Dirty") {
                 dirties.Add(child.gameObject);
                 child.gameObject.SetActive(false);
             }else if(child.gameObject.tag == "Shadow"){
                 shadow = child.gameObject;
                 originalShadowScale = shadow.transform.localScale;
             }else if(child.gameObject.tag == "Dirty_L") {
                 dirties_L.Add(child.gameObject);
                 child.gameObject.SetActive(false);
             }else if(child.gameObject.tag == "Dirty_LD") {
                 dirties_LD.Add(child.gameObject);
                 child.gameObject.SetActive(false);
             }
         }

        Load();
    }

    public void LoadTime(float t)
    {
        int n = 0;
        if(data.Health > data.MaxHealth * 0.5f && data.Damage < data.MaxDamage * 0.5f && t > 3600)
        {
            n += data.level/2;
        }

        if(actionType == ActionType.Sleep)
            data.Sleep += 0.01f * t;
        data.Energy -= 0.01f * t;
        data.Food -= 0.01f * t;
        data.Water -= 0.01f * t;
        data.Pee += 0.01f * t;
        data.Shit += 0.01f * t;
        data.Dirty += data.recoverDirty*0.1f * t;

        for (int i = 0; i < n; i++)
        {
            int ran = Random.Range(0, 100);
            Quaternion rot = Quaternion.identity;
            if (ran > 50)
                rot = Quaternion.Euler(new Vector3(0, 180, -1));
            Vector3 pos = this.charScale.scalePosition + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
            ItemManager.instance.SpawnHeart(pos, rot, 1, true);
        }

    }

    protected virtual void Load(){

    }
    // Use this for initialization

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        if (agent == null)
            return;

        //Debug.Log(actionType.ToString() + "  " + charInteract.interactType.ToString());
        if (actionType == ActionType.None)
        {
            //Debug.Log("Think");
            if (enviromentType == EnviromentType.Bath)
            {
                OnBath();
            }
            else if (enviromentType == EnviromentType.Table)
            {
                OnTable();
            }
            else if (enviromentType == EnviromentType.Bed)
            {
                OnBed();
            }
            else if (enviromentType == EnviromentType.Toilet)
            {
                OnToilet();
            }else{
                Think();                
            }
            DoAction();
            LogAction();
        }else if(actionType == ActionType.Sick || actionType == ActionType.Injured)
        {
            float t = data.MaxTimeSick - (float)(System.DateTime.Now - data.timeSick).TotalSeconds;
            float m = (int)(t / 60);
            float s = (int)(t - m * 60);
            timeWait.text = m.ToString("00") + ":" + s.ToString("00");
            //Debug.Log(t);
        }


        CalculateDirection();
        
        this.transform.position = agent.transform.position;

        //Calculate Attribue Data
        if (dataTime > maxDataTime)
        {
            CalculateData();
            CalculateStatus();
            dataTime = 0;
//            Debug.Log((System.DateTime.Now - playTime).Seconds);

        }
        else
            dataTime += Time.deltaTime;


        
    }



    #endregion

    #region Data
    protected virtual void CalculateData()
    {

        if(data.Food > 0 && data.Water > 0){
            float delta = data.recoverEnergy/5;
            data.Energy += delta;
            data.Food -= delta;    
            data.Water -= delta;
            data.Shit += delta/4;
            data.Pee += delta/2;
        }


        data.Dirty += data.recoverDirty;
        if (data.Dirty > data.MaxDirty * 0.7f)
            data.Itchi += (data.Dirty - data.MaxDirty * 0.7f) * 0.005f;
        
        data.Sleep -= data.recoverSleep;

        float deltaHealth = data.recoverHealth;

        if(data.Health > 0.1f*data.MaxHealth){
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

        data.Health += deltaHealth;
        data.curious += 0.1f;

        //CheckDirty
        if(data.dirty > data.MaxDirty * 0.5f){
            int n = (int)((data.dirty - data.MaxDirty * 0.5f)/(data.MaxDirty * 0.5f) * dirties.Count);
            for(int i=0;i<dirties.Count;i++){
                if(i < n)
                    dirties[i].SetActive(true);
                else
                    dirties[i].SetActive(false);
            }

            int n1 = (int)((data.dirty - data.MaxDirty * 0.5f)/(data.MaxDirty * 0.5f) * dirties_L.Count);
            for(int i=0;i<dirties_L.Count;i++){
                if(i < n)
                    dirties_L[i].SetActive(true);
                else
                    dirties_L[i].SetActive(false);
            }

            int n2 = (int)((data.dirty - data.MaxDirty * 0.5f)/(data.MaxDirty * 0.5f) * dirties_LD.Count);
            for(int i=0;i<dirties_LD.Count;i++){
                if(i < n)
                    dirties_LD[i].SetActive(true);
                else
                    dirties_LD[i].SetActive(false);
            }
        }else
        {
            for(int i=0;i<dirties.Count;i++){
                dirties[i].SetActive(false);
            }
            for(int i=0;i<dirties_L.Count;i++){
                dirties_L[i].SetActive(false);
            }
            for(int i=0;i<dirties_LD.Count;i++){
                dirties_LD[i].SetActive(false);
            }
        }

        //Save data
        data.actionType = this.actionType;
        data.position = this.transform.position;
        data.enviromentType = this.enviromentType;
        data.scalePosition = charScale.scalePosition;
        data.height = charScale.height;

    }



    #region Thinking
    protected virtual void Think()
    {
        if(charInteract.interactType != InteractType.None)
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



        if(data.Food < data.MaxFood * 0.3f && GetFoodItem() != null && Vector2.Distance(this.transform.position,GetFoodItem().transform.position) < 3){
            actionType = ActionType.Eat;
            return;
        }

        if(data.Water < data.MaxWater * 0.3f && GetDrinkItem() != null && Vector2.Distance(this.transform.position,GetDrinkItem().transform.position) < 3){
            actionType = ActionType.Drink;
            return;
        }

        if (data.Food < data.MaxFood * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 0)
            {
                actionType = ActionType.Eat;
                return;
            }
        }


        if (data.Water < data.MaxWater * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 0)
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

        if (data.Itchi > data.MaxItchi * 0.7f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 50)
            {
                actionType = ActionType.Itchi;
                return;
            }
        }

        if(data.Energy < data.MaxEnergy * 0.1f){
            actionType = ActionType.Tired;
            return;
        }


        if (data.curious > data.MaxCurious * 0.9f)
        {
            actionType = ActionType.Discover;
            return;
        }


        actionType = ActionType.Patrol;
    }


    

    protected virtual void DoAction()
    {
        if(isAction){
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
        else if (actionType == ActionType.Fear)
        {
            StartCoroutine(Fear());
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
        }else if(actionType == ActionType.OnBed){
            StartCoroutine(Bed());
        }else if (actionType == ActionType.OnToilet)
        {
            StartCoroutine(Toilet());
        }else if (actionType == ActionType.Supprised)
        {
            StartCoroutine(Supprised());
        }else if (actionType == ActionType.Stop)
        {
            StartCoroutine(Stop());
        } else if (actionType == ActionType.Toy)
        {
            StartCoroutine(Toy());
        }else if (actionType == ActionType.Injured)
        {
            StartCoroutine(Injured());
        }
        else if (actionType == ActionType.JumpOut)
        {
            StartCoroutine(JumpOut());
        }
        else if (actionType == ActionType.OnCall)
        {
            StartCoroutine(Call());
        }
        else if (actionType == ActionType.OnControl)
        {
            StartCoroutine(Control());
        }
        else if (actionType == ActionType.OnGarden)
        {
            StartCoroutine(Garden());
        }
    }

    #endregion

    protected virtual void CalculateStatus(){
       
        lastIconStatus = iconStatus;

        if(data.Damage > 0.9f*data.MaxDamage || actionType == ActionType.Injured){
            iconStatus = IconStatus.Injured_2;
        }else if(data.Health < 0.1f*data.MaxHealth || actionType == ActionType.Sick){
            iconStatus = IconStatus.Sick_2; 
        }else if(data.Pee > 0.9f*data.MaxPee || data.Shit > 0.9f*data.MaxShit){
            iconStatus = IconStatus.Toilet_2; 
        }else if(data.Food < 0.1f*data.MaxFood){
            iconStatus = IconStatus.Hungry_2; 
        }else if(data.Water < 0.1f*data.MaxWater){
            iconStatus = IconStatus.Thirsty_2; 
        }else if(data.sleep < 0.1f*data.MaxSleep){
            iconStatus = IconStatus.Sleepy_2;
        }else if(data.dirty > 0.9f*data.MaxDirty){
            iconStatus = IconStatus.Dirty_2; 
        }else if(data.Damage > 0.7f*data.MaxDamage){
            iconStatus = IconStatus.Injured_1;
        }else if(data.Health < 0.3f*data.MaxHealth){
            iconStatus = IconStatus.Sick_1;
        }else if(data.Pee > 0.7f*data.MaxPee || data.Shit > 0.7f*data.MaxShit){
            iconStatus = IconStatus.Toilet_1;
        }else if(data.Food < 0.3f*data.MaxFood){
            iconStatus = IconStatus.Hungry_1;
        }else if(data.Water < 0.3f*data.MaxWater){
            iconStatus = IconStatus.Thirsty_1;
        }else if(data.sleep < 0.3f*data.MaxSleep){
            iconStatus = IconStatus.Sleepy_1;
        }else if(data.dirty > 0.7f*data.MaxDirty){
            iconStatus = IconStatus.Dirty_1;
        }else{
            iconStatus = IconStatus.None;
        }


        if(iconStatusObject != null){
            LoadIconStatus();
        }

        //iconStatusObject.transform.localScale = originalStatusScale / charScale.scaleAgeFactor;

    }

    void LoadIconStatus(){
        if(iconStatus != IconStatus.None && lastIconStatus != iconStatus){
            iconStatusObject.gameObject.SetActive(true);
            iconStatusObject.sprite = Resources.Load<Sprite>("Icons/Status/" + iconStatus.ToString()) as Sprite;
        }
        
        if(iconStatus == IconStatus.None){
            iconStatusObject.gameObject.SetActive(false);
        }
    }

    protected virtual void CalculateDirection(){
        if (agent.transform.eulerAngles.z < 180f && agent.transform.eulerAngles.z > 0f || (agent.transform.eulerAngles.z > -360f && agent.transform.eulerAngles.z < -180f))
            direction = Direction.L;
        else 
            direction = Direction.R;
    }



    #endregion


    #region Interact
   

    public void SetActionType(ActionType action){
        Abort();
        actionType = action;
    }

    public virtual void OnHold()
    {
        if(actionType == ActionType.Sick){
            UIManager.instance.OnTreatmentPopup(this.data,SickType.Sick);
            return;
        }

        if(actionType == ActionType.Injured){
            UIManager.instance.OnTreatmentPopup(this.data, SickType.Injured);
            return;
        }

        if(charInteract.interactType == InteractType.Drop || charInteract.interactType == InteractType.Jump || actionType == ActionType.OnControl)
            return;

        Abort();
        charInteract.interactType = InteractType.Drag;
        actionType = ActionType.Hold;
    }

    public virtual void OnCall()
    {
        if (actionType == ActionType.Sick)
        {
            UIManager.instance.OnTreatmentPopup(this.data, SickType.Sick);
            return;
        }

        if (actionType == ActionType.Injured)
        {
            UIManager.instance.OnTreatmentPopup(this.data, SickType.Injured);
            return;
        }

        if (actionType == ActionType.Patrol || actionType == ActionType.Discover || actionType == ActionType.OnGarden)
        {
            Abort();
            actionType = ActionType.OnCall;
        }
    }

    public virtual void OnSupprised(){
        if(actionType == ActionType.Hold || actionType == ActionType.Sick || actionType == ActionType.Injured || enviromentType != EnviromentType.Room || actionType == ActionType.Toy)
            return;

        Abort();
        actionType  = ActionType.Supprised;
    }

    public virtual void OnFear(){
        if(actionType == ActionType.Hold || actionType == ActionType.Sick || actionType == ActionType.Injured || enviromentType != EnviromentType.Room || actionType == ActionType.Toy)
            return;

        Abort();
        actionType  = ActionType.Fear;
    }

    public virtual void OnEat(){
        if(enviromentType == EnviromentType.Room && data.Food < 0.3f * data.MaxFood && 
            (actionType == ActionType.Patrol || actionType == ActionType.Discover || actionType == ActionType.OnCall)){
            actionType = ActionType.Eat;
            isAbort = true;
        }
    }

    public virtual void OnDrink(){
        if(enviromentType == EnviromentType.Room && data.Water < 0.3f * data.MaxWater && 
            (actionType == ActionType.Patrol || actionType == ActionType.Discover)){
            actionType = ActionType.Drink;
            isAbort = true;
        }

    }

    protected void OnBath()
    {
        Abort();
        actionType = ActionType.OnBath;
    }


    public void OnHealth(SickType type,float value){
        if(type == SickType.Injured){
            if(data.Damage > data.MaxDamage * 0.6f){
                GameManager.instance.AddExp(5,data.iD);
                GameManager.instance.LogAchivement(AchivementType.Injured);
            }
            data.Damage -= value;
        }else if(type ==SickType.Sick){
            if(data.Health < data.MaxHealth * 0.4f){
                GameManager.instance.AddExp(5,data.iD);
                GameManager.instance.LogAchivement(AchivementType.Sick);
            }
            data.Health += value;
        }
       
    }

    protected void OnBed()
    {
        Abort();
        actionType = ActionType.OnBed;
    }

    protected void OnToilet()
    {
        Abort();
        actionType = ActionType.OnToilet;
    }

    protected void OnTable()
    {
        Abort();
        actionType = ActionType.OnTable;
    }

    public virtual void OnFall(){
        if (actionType == ActionType.Sick || actionType == ActionType.Injured)
            return;


        if(!isArrived){
            int ran = Random.Range(0, 100);
            if(ran > 50)
            {
                Abort();
                isArrived = true;
                actionType = ActionType.Fall;
            }
        }
    }

    public virtual void OnSoap()
    {
        if (actionType == ActionType.OnBath)
        {
            anim.Play("Soap", 0);
        }
    }

    public virtual void OffSoap(){
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
        if (actionType == ActionType.OnBath){
            anim.Play("Shake", 0);
            MageManager.instance.PlaySoundName("Shake",false);
        }
            
    }

    public virtual void OnControl()
    {
        actionType = ActionType.OnControl;
        isAbort = true;
    }

    public virtual void OnStop(){
        if(actionType != ActionType.Stop && !isArrived){
            agent.Stop();
            actionType = ActionType.Stop;
            isAbort = true;
        }
    }

    public virtual void OnMouse()
    {
        if(charInteract.interactType != InteractType.None)
            return;
        if( actionType == ActionType.Patrol || actionType == ActionType.Discover || actionType == ActionType.Drink || actionType == ActionType.Eat){
            Abort();
            actionType = ActionType.Mouse;
        }
    }


    public virtual void OnJumpOut()
    {
        isAbort = true;
        //if(charType != CharType.Cat)
        actionType = ActionType.JumpOut;
    }

    public virtual void OnToy(ToyItem item){
        if(actionType != ActionType.Sick && actionType != ActionType.Injured && actionType != ActionType.OnControl && charInteract.interactType != InteractType.Drag //actionType != ActionType.Hold
         && actionType != ActionType.Toy && enviromentType == EnviromentType.Room && actionType != ActionType.Supprised){
            actionType = ActionType.Toy;
            isAbort = true;
            toyItem = item;
            Debug.Log(item.toyType);
        }
    }

    public virtual void OnArrived()
    {
        //Debug.Log("Arrived");

        if (actionType == ActionType.Mouse)
        {
            actionType = ActionType.None;
            //agent.speed = 30;
            anim.speed = 1;
        }

        isArrived = true;
    }


    public virtual void OnLevelUp()
    {

    }

    public virtual void OnTreatment(SickType sickType)
    {
        Abort();
        actionType = ActionType.Happy;
    }

    void LogAction(){
        //ApiManager.GetInstance().LogAc(actionType);
    }


    #endregion




    #region Basic Action

    //Basic Action
    protected void Abort()
    {
        if(anim != null)
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
        if(target.x >= this.transform.position.x){
            direction = Direction.R;
            SetDirection(Direction.R);
        }else{
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

    protected virtual IEnumerator RunToPoint()
    {
        isMoving = true;
        isArrived = false;
        agent.SetDestination(target);
        charScale.speedFactor = 1;
    
        while (!isArrived && !isAbort)
        {
            anim.Play("Run_" + this.direction.ToString(), 0);
            data.Energy -= Time.deltaTime;
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
            data.Energy -= 0.3f*Time.deltaTime;
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


    protected IEnumerator JumpDown(float zSpeed,float ySpeed,float accelerator){
        enviromentType = EnviromentType.Room;
        anim.Play("Hold", 0);
        float speed = ySpeed;
        //charScale.scalePosition = new Vector3(this.transform.position.x, this.transform.position.y - height, 0);
        charInteract.interactType = InteractType.Jump;
        MageManager.instance.PlaySoundName("Drag",false);
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

    protected IEnumerator JumpUp(float ySpeed,float zSpeed, Vector3 dropPosition,float height){
        if(!isAbort){
            anim.Play("Hold", 0);      
            charInteract.interactType = InteractType.Jump;
            MageManager.instance.PlaySoundName("Drag",false);
            while (charInteract.interactType == InteractType.Jump && !isAbort)
            {
                ySpeed -= 30 * Time.deltaTime;
                if (ySpeed < -50)
                    ySpeed = -50;
                Vector3 pos1 = agent.transform.position;
                pos1.y += ySpeed * Time.deltaTime + zSpeed * Time.deltaTime;
                pos1.x = Mathf.Lerp(pos1.x,dropPosition.x,Time.deltaTime * 5);
                agent.transform.position = pos1;
                charScale.height += ySpeed * Time.deltaTime;
                charScale.scalePosition.y = Mathf.Lerp(charScale.scalePosition.y,dropPosition.y - height,Time.deltaTime * 5);

                if (ySpeed < 0 && this.transform.position.y < dropPosition.y)
                {
                    this.transform.rotation = Quaternion.identity;
                    charInteract.interactType = InteractType.None;
                    agent.transform.position = dropPosition;
                    this.transform.position = dropPosition;
                    charScale.scalePosition.y = dropPosition.y - height;
                    charScale.height = height;
                }
                yield return new WaitForEndOfFrame();
            }
            yield return StartCoroutine(DoAnim("Drop"));
        }
    }

    protected virtual IEnumerator Mouse()
    {
        isMoving = true;
        charScale.speedFactor = 1.2f;
        while(GetMouse() != null && GetMouse().state != MouseState.Idle && !isAbort){
            agent.SetDestination(GetMouse().transform.position);
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);
            data.Energy -= 1.5f*Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
        charScale.speedFactor = 1;
        CheckAbort();
    }

    protected virtual IEnumerator Hold()
    {
        MageManager.instance.PlaySoundName("Drag",false);
        charInteract.interactType = InteractType.Drag;
        enviromentType = EnviromentType.Room;
        ItemManager.instance.SetCameraTarget(this.gameObject);
        anim.Play("Hold", 0);
        if(shadow != null)
            shadow.SetActive(true);
        while (charInteract.interactType == InteractType.Drag && !isAbort)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - charInteract.dragOffset;
            pos.z = 0;
            if (pos.y > charScale.maxHeight)
                pos.y = charScale.maxHeight;
            else if (pos.y < -24)
                pos.y = -24;

            if (pos.x > 52)
                pos.x = 52;
            else if (pos.x < -270)
                pos.x = -270;


            pos.z = -50;
            agent.transform.position = pos;

            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.identity, Time.deltaTime * 2);
            yield return new WaitForEndOfFrame();
        }

        //Start Drop
        CheckDrop();

        float fallSpeed = 0;
        
        while (charInteract.interactType == InteractType.Drop && actionType != ActionType.OnControl)
        {
            if (agent.transform.position.y > dropPosition.y)
            {
                fallSpeed += 100f * Time.deltaTime;
                if (fallSpeed > 50)
                    fallSpeed = 50;
                Vector3 pos1 = agent.transform.position;
                pos1.y -= fallSpeed * Time.deltaTime;
                pos1.x = Mathf.Lerp(pos1.x,dropPosition.x,Time.deltaTime * 5);
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
        MageManager.instance.PlaySoundName("whoosh_swish_med_03",false);
        yield return StartCoroutine(DoAnim("Drop"));
        
    
        CheckAbort();
    }

    protected virtual IEnumerator Table()
    {   
        anim.Play("Idle_" + this.direction.ToString(), 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Call()
    {
        Vector3 pos = ItemManager.instance.GetActiveCamera().transform.position;
        pos.y = -22;
        target = pos;
        yield return StartCoroutine(RunToPoint());
        int ran = Random.Range(0, 100);
        if(ran < 30)
        {
            yield return StartCoroutine(DoAnim("Standby"));
        }else if(ran < 60)
        {
            MageManager.instance.PlaySoundName(charType.ToString() + "_Speak", false);
            yield return DoAnim("Speak_" + direction.ToString());
        }
        else
        {
            yield return StartCoroutine(DoAnim("Love"));
        }
        
        CheckAbort();
    }

    protected virtual IEnumerator Discover()
    {
        
        if (data.curious > data.MaxCurious * 0.7f)
        {
            
            int ran1 = Random.Range(0,100);
            Vector3 t = GetRandomPoint(PointType.Patrol).position;
            target = t;
            yield return StartCoroutine(RunToPoint());
            yield return StartCoroutine(DoAnim("Idle_" + direction.ToString())) ;
            data.curious -= 10;                
        }

        /*
        CharController t = null;
        foreach(CharController p in GameManager.instance.petObjects){
            if(p != this && p.enviromentType == EnviromentType.Room){
                t = p;
            }
        }
        if(t != null){
            while(t != null && Vector2.Distance(this.transform.position,t.transform.position) > 3 && !isAbort){
                agent.SetDestination(t.transform.position + new Vector3(6,0,0));
                anim.Play("Run_" + this.direction.ToString(), 0);
                //charScale.speedFactor = 0.5f;
                yield return new WaitForEndOfFrame();
            }
            if(t != null)
                t.OnSupprised();
            yield return StartCoroutine(DoAnim("Tease"));
            data.curious -=40;        
            target = GetRandomPoint(PointType.Patrol).position;
            yield return StartCoroutine(RunToPoint());
        }*/
        
        CheckAbort();
    }

    protected virtual IEnumerator Stop(){
        yield return StartCoroutine(Wait(anim.GetCurrentAnimatorStateInfo(0).length));
        anim.Play("Idle_" + direction.ToString());
        yield return StartCoroutine(Wait(Random.Range(1f,2f)));
        CheckAbort();
    }

    protected virtual IEnumerator Bath()
    {

        while(!isAbort){
            yield return new WaitForEndOfFrame();
        }   
        
        CheckAbort();
    }

    protected virtual IEnumerator Toilet()
    {
        if(data.shit > 0.7*data.MaxShit){
            actionType = ActionType.Shit;
            isAbort = true;
        }else if(data.pee > 0.7f*data.MaxPee){
            actionType = ActionType.Pee;
            isAbort = true;
        }
        else{
            yield return StartCoroutine(Wait(Random.Range(1f,2f)));
            yield return StartCoroutine(JumpDown(-7,10,30));           
        }

        CheckAbort();
    }

    protected virtual IEnumerator Pee()
    {
        if (enviromentType != EnviromentType.Toilet)
        {
            if (data.GetSkillProgress(SkillType.Toilet) == 0)
            {
                if (GameManager.instance.IsEquipItem(ItemType.Toilet))
                {
                    //UIManager.instance.OnQuestNotificationPopup("Hold " + data.petName + " to the toilet and he will be happy");
                }
                else
                {
                    if (!GameManager.instance.IsEquipItem(ItemType.Clean))
                    {
                        //UIManager.instance.OnQuestNotificationPopup("You may need to buy a broom to clean your home");
                    }
                    else
                    {
                        //UIManager.instance.OnQuestNotificationPopup("You can buy a toilet for " + data.petName + " and " + data.petName + " will be happy");
                    }
                }
            }

            if (data.SkillLearned(SkillType.Toilet))
            {
                SetTarget(PointType.Toilet);
                yield return StartCoroutine(RunToPoint());
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Toilet);
                yield return StartCoroutine(JumpUp(10, 5, col.transform.position + new Vector3(0, col.height, 0), col.height));
                enviromentType = EnviromentType.Toilet;
            }
            else
            {
                OnLearnSkill(SkillType.Toilet);
            }
        }
        else
        {
            if (data.GetSkillProgress(SkillType.Toilet) == 1)
            {
                //UIManager.instance.OnQuestNotificationPopup("Good job " + data.petName + " will learn how to go to toilet soon!");
            }else if(data.GetSkillProgress(SkillType.Toilet) == 10)
            {
                //UIManager.instance.OnQuestNotificationPopup("Well done!! now " + data.petName + " can go to toilet by him self");
            }
        }

        
        anim.Play("Pee", 0);
        int soundid = MageManager.instance.PlaySoundName("Pee", true);
        MageManager.instance.PlaySoundName("PeeDrop", false);
        Debug.Log("Pee");
        float value = data.Pee;
        ItemManager.instance.SpawnPee(peePosition.position + new Vector3(0, 0, 50),value);
        while (data.Pee > 0 && !isAbort)
        {
            data.Pee -= data.ratePee * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        MageManager.instance.StopSound(soundid);
        

        if(enviromentType == EnviromentType.Toilet){
            if(data.pee <= 1){
                GameManager.instance.AddExp(10,data.iD);
                GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.OnToilet);
                LevelUpSkill(SkillType.Toilet);
                ItemManager.instance.SpawnHeart((int)ItemManager.instance.GetItemData(ItemType.Toilet).happy, this.transform.position);
                data.Health += ItemManager.instance.GetItemData(ItemType.Toilet).health;
                data.Damage -= ItemManager.instance.GetItemData(ItemType.Toilet).injured;
            }

            yield return StartCoroutine(JumpDown(-7,10,30));
        }

        

        CheckAbort();
    }

    protected virtual IEnumerator Shit()
    {
        if(enviromentType != EnviromentType.Toilet)
        {
            if (data.GetSkillProgress(SkillType.Toilet) == 0)
            {
                if (GameManager.instance.IsEquipItem(ItemType.Toilet))
                {
                    UIManager.instance.OnQuestNotificationPopup("Hold " + data.petName + " to the toilet and he will be happy");
                }
                else
                {
                    if (!GameManager.instance.IsEquipItem(ItemType.Clean))
                    {
                        //UIManager.instance.OnQuestNotificationPopup("You may need to buy a broom to clean your home");
                    }
                    else
                    {
                        //UIManager.instance.OnQuestNotificationPopup("You can buy a toilet for " + data.petName + " so that " + data.petName + " will be happy");
                    }
                }
            }

            if (data.SkillLearned(SkillType.Toilet) )
            {
                SetTarget(PointType.Toilet);
                yield return StartCoroutine(RunToPoint());
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Toilet);
                yield return StartCoroutine(JumpUp(10,5,col.transform.position + new Vector3(0,col.height,0),col.height));
                enviromentType = EnviromentType.Toilet;
            }else{
                OnLearnSkill(SkillType.Toilet);
            }
        }
        else
        {
            if (data.GetSkillProgress(SkillType.Toilet) == 1)
            {
                //UIManager.instance.OnQuestNotificationPopup("Good job " + data.petName + " will learn how to go to toilet soon!");
            }
            else if (data.GetSkillProgress(SkillType.Toilet) == 10)
            {
                //UIManager.instance.OnQuestNotificationPopup("Well done!! " + data.petName + " can go to toilet by him self");
            }
        }


        anim.Play("Shit", 0);
        MageManager.instance.PlaySoundName("Shit",false);
        float value = data.Pee;
        
        while (data.Shit > 0 && !isAbort)
        {
            data.Shit -= data.rateShit * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ItemManager.instance.SpawnShit(shitPosition.position,value);

        if(enviromentType == EnviromentType.Toilet){
            if(data.shit <= 1){
                GameManager.instance.AddExp(10,data.iD);
                GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.OnToilet);
                LevelUpSkill(SkillType.Toilet);
                ItemManager.instance.SpawnHeart((int)ItemManager.instance.GetItemData(ItemType.Toilet).happy, this.transform.position);
                data.Health += ItemManager.instance.GetItemData(ItemType.Toilet).health;
                data.Damage -= ItemManager.instance.GetItemData(ItemType.Toilet).injured;
            }
            yield return StartCoroutine(JumpDown(-7,10,30));     
        }
        CheckAbort();
    }

    protected virtual IEnumerator Eat()
    {
        if (GetFoodItem() != null)
        {
            if (Vector2.Distance(this.transform.position, GetFoodItem().anchor.position) > 0.5f)
            {
                SetTarget(PointType.Eat);
                yield return StartCoroutine(RunToPoint());
            }
            bool canEat = true;
            if (Vector2.Distance(this.transform.position, GetFoodItem().anchor.position) > 1f)
                canEat = false;
            if (GetFoodItem().CanEat() && canEat)
            {
                anim.Play("Eat", 0);
                int soundid = MageManager.instance.PlaySoundName("Eat", false);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Food < data.MaxFood && !isAbort && canEat)
                {
                    data.Food += data.rateFood * 0.1f;
                    GetFoodItem().Eat(data.rateFood * 0.1f);
                    if (!GetFoodItem().CanEat())
                    {
                        canEat = false;
                    }
                    if (Vector2.Distance(this.transform.position, GetFoodItem().anchor.position) > 1f)
                        canEat = false;
                    yield return new WaitForEndOfFrame();
                }
                MageManager.instance.StopSound(soundid);
                if (data.Food >= data.MaxFood - 10)
                {
                    GameManager.instance.LogAchivement(AchivementType.Do_Action, ActionType.Eat);
                    GameManager.instance.AddExp(5, data.iD);
                    ItemManager.instance.SpawnHeart((int)ItemManager.instance.GetItemData(ItemType.Food).happy, this.transform.position);
                    data.Health += ItemManager.instance.GetItemData(ItemType.Food).health;
                    data.Damage -= ItemManager.instance.GetItemData(ItemType.Food).injured;
                    if (GetFoodItem() != null && GetFoodItem().GetComponent<ItemObject>() != null)
                        GameManager.instance.LogAchivement(AchivementType.Eat, ActionType.None, GetFoodItem().GetComponent<ItemObject>().itemID);
                }
            }
            else
            {
               
                int ran = Random.Range(0, 100);
                if (ran < 30)
                {
                    MageManager.instance.PlaySoundName(charType.ToString() + "_Speak", false);
                    yield return StartCoroutine(DoAnim("Speak_" + direction.ToString()));
                }
                else
                {
                    yield return StartCoroutine(DoAnim("Standby"));
                }
            }
        }

        yield return new WaitForEndOfFrame();
        CheckAbort();
    }

    protected virtual IEnumerator Drink()
    {
        if (GetDrinkItem() != null)
        {
            //Debug.Log("Drink");

            if (Vector2.Distance(this.transform.position, GetDrinkItem().anchor.position) > 0.5f)
            {
                SetTarget(PointType.Drink);
                yield return StartCoroutine(RunToPoint());
            }

            

            bool canDrink = true;
            if (Vector2.Distance(this.transform.position, GetDrinkItem().anchor.position) > 1f)
                canDrink = false;

            if (GetDrinkItem().CanEat() && canDrink)
            {
                int soundid =  MageManager.instance.PlaySoundName("Drink",true);
                anim.Play("Drink", 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Water < data.MaxWater && !isAbort && canDrink)
                {
                    data.Water += data.rateWater*0.1f;
                    GetDrinkItem().Eat(data.rateWater*0.1f);
                    if (!GetDrinkItem().CanEat())
                    {
                        canDrink = false;
                    }
                    if (Vector2.Distance(this.transform.position, GetDrinkItem().anchor.position) > 1f)
                        canDrink = false;
                    yield return new WaitForEndOfFrame();
                }
                MageManager.instance.StopSound(soundid);
                if(data.Water >= data.MaxWater - 10){
                    GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Drink);
                    GameManager.instance.AddExp(5,data.iD);
                    ItemManager.instance.SpawnHeart((int)ItemManager.instance.GetItemData(ItemType.Drink).happy, this.transform.position);
                    data.Health += ItemManager.instance.GetItemData(ItemType.Drink).health;
                    data.Damage -= ItemManager.instance.GetItemData(ItemType.Drink).injured;
                    if (GetDrinkItem() != null && GetDrinkItem().GetComponent<ItemObject>() != null)
                        GameManager.instance.LogAchivement(AchivementType.Drink,ActionType.None,GetDrinkItem().GetComponent<ItemObject>().itemID);
                }
            }else{
                int ran = Random.Range(0, 100);
                if (ran < 30)
                {
                    MageManager.instance.PlaySoundName(charType.ToString() + "_Speak", false);
                    yield return StartCoroutine(DoAnim("Speak_" + direction.ToString()));
                }
                else
                {
                    yield return StartCoroutine(DoAnim("Standby"));
                }
            }
        }
        CheckAbort();
    }

    protected virtual IEnumerator Bed()
    {
        int ran = Random.Range(0,100);
        if(data.sleep < 0.3f*data.MaxSleep || data.Health < 0.3f * data.MaxHealth){
            actionType = ActionType.Sleep;
            Abort();
        }else{                    
            anim.Play("Idle_" + direction.ToString(),0);
            yield return StartCoroutine(Wait(Random.Range(2,4)));
            yield return StartCoroutine(JumpDown(-7,10,30)); 
        }
        
        CheckAbort();
    }

    protected virtual IEnumerator Sleep()
    {
        if(enviromentType != EnviromentType.Bed)
        {
            if (data.GetSkillProgress(SkillType.Sleep) == 0)
            {
                if (GameManager.instance.IsEquipItem(ItemType.Bed))
                {
                    //UIManager.instance.OnQuestNotificationPopup("Hold " + data.petName + " to the bed and he will be happy");
                }
                else
                {
                     //UIManager.instance.OnQuestNotificationPopup("You can buy a bed for " + data.petName + " and " + data.petName + " will be happy");
                }
            }


            if (data.SkillLearned(SkillType.Sleep) )
            {
                SetTarget(PointType.Sleep);
                yield return StartCoroutine(RunToPoint());
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Bed);
                 yield return StartCoroutine(JumpUp(10,5,col.transform.position,col.height));
                enviromentType = EnviromentType.Bed;
            }else{
                OnLearnSkill(SkillType.Sleep);
            }
        }
        else
        {
            if (data.GetSkillProgress(SkillType.Toilet) == 1)
            {
                //UIManager.instance.OnQuestNotificationPopup("Good job " + data.petName + " will learn how to go to bed soon!");
            }
            else if (data.GetSkillProgress(SkillType.Toilet) == 10)
            {
                //UIManager.instance.OnQuestNotificationPopup("Well done!! " + data.petName + " can go to bed by him self");
            }
        }




        anim.Play("Sleep", 0);

        while (data.Sleep < data.MaxSleep && !isAbort)
        {
            data.Sleep += data.rateSleep * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        if(enviromentType == EnviromentType.Bed){
            if(data.Sleep > data.MaxSleep - 1){
                GameManager.instance.AddExp(20,data.iD);
                GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Sleep);
                LevelUpSkill(SkillType.Sleep);
                //Debug.Log(ItemManager.instance.GetItemData(ItemType.Bed).GetName(0));
                ItemManager.instance.SpawnHeart((int)ItemManager.instance.GetItemData(ItemType.Bed).happy, this.transform.position);
                data.Health += ItemManager.instance.GetItemData(ItemType.Bed).health;
                data.Damage -= ItemManager.instance.GetItemData(ItemType.Bed).injured;
            }
            yield return StartCoroutine(JumpDown(-7,10,30)); 
        }

        CheckAbort();
    }

    protected virtual IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        while (!isAbort && n < maxCount)
        {
            int ran = Random.Range(0, 100);
            if(ran < 40){
                SetTarget(PointType.Patrol);
                yield return StartCoroutine(RunToPoint());
            }else if (ran < 60)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            else if(ran < 80){
                anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }else{
                MageManager.instance.PlaySoundName(charType.ToString() + "_Speak",false);
                yield return DoAnim("Speak_" + direction.ToString());
            }
            
            n++;
        }
        CheckAbort();
    }

    protected virtual IEnumerator Garden()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        SetTarget(PointType.Garden);
        yield return StartCoroutine(RunToPoint());
        while (!isAbort && n < maxCount)
        {
            int ran = Random.Range(0, 100);
            if (ran < 40)
            {
                SetTarget(PointType.Garden);
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
                MageManager.instance.PlaySoundName(charType.ToString() + "_Speak", false);
                yield return DoAnim("Speak_" + direction.ToString());
            }

            n++;
        }
        CheckAbort();
    }


    protected virtual IEnumerator Listening(){
        yield return StartCoroutine(DoAnim("Idle_" + direction.ToString()));
        CheckAbort();
    }



    protected virtual IEnumerator JumpOut()
    {
        if(enviromentType == EnviromentType.Bath){
            yield return StartCoroutine(JumpDown(-5,15,35));
        }else if(enviromentType == EnviromentType.Bed || enviromentType == EnviromentType.Toilet){
            yield return StartCoroutine(JumpDown(-7,10,30)); 
        }else if(enviromentType == EnviromentType.Table)
        {
            yield return StartCoroutine(JumpDown(-2, 10, 30));
        }
        CheckAbort();
    }

    

    protected virtual IEnumerator Sick()
    {
        timeWait.gameObject.SetActive(true);
        data.timeSick = System.DateTime.Now;
        SetTarget(PointType.Favourite);
        yield return StartCoroutine(WalkToPoint());
        anim.Play("Sick", 0);
        Debug.Log("Sick");
        while ((System.DateTime.Now - data.timeSick).TotalSeconds < data.MaxTimeSick && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        data.Health = data.MaxHealth;
        timeWait.gameObject.SetActive(false);
        GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Sick);
        CheckEnviroment();
        CheckAbort();
    }

    protected virtual IEnumerator Injured()
    {
        timeWait.gameObject.SetActive(true);
        data.timeSick = System.DateTime.Now;
        SetTarget(PointType.Favourite);
        yield return StartCoroutine(WalkToPoint());
        anim.Play("Injured", 0);
        Debug.Log("Injured");
        while ((System.DateTime.Now - data.timeSick).TotalSeconds < data.MaxTimeSick && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        data.Damage = data.MaxHealth;
        timeWait.gameObject.SetActive(false);
        GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Injured);
        CheckEnviroment();
        CheckAbort();
    }

    protected virtual IEnumerator Fear()
    {
        MageManager.instance.PlaySoundName(charType.ToString() + "_Supprised",false);
        yield return StartCoroutine(DoAnim("Teased"));
        data.Energy -= 2;
        CheckAbort();
    }


    protected virtual IEnumerator Tired()
    {
        anim.Play("Tired", 0);
        while (data.Food > 0 && data.Water > 0 && data.Sleep > 0 && data.energy < data.MaxEnergy * 0.5f && !isAbort)
        {
            float delta = data.recoverEnergy * Time.deltaTime;
            data.Energy += delta;
            data.Food -= delta * 0.1f;
            data.Water -= delta * 0.1f;
            data.Sleep -= delta * 0.1f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Toy()
    {
        if (toyItem != null)
        {
            if (toyItem.toyType == ToyType.WaterJet)
            {
                int n = Random.Range(3, 10);
                int count = 0;
                dropPosition = toyItem.anchorPoint.position + new Vector3(0, Random.Range(-2f, 2f), 0);
                agent.transform.position = dropPosition;

                yield return new WaitForEndOfFrame();
                while (!isAbort && count < n)
                {
                    toyItem.OnActive();
                    MageManager.instance.PlaySoundName(charType.ToString() + "_Supprised", false);
                    MageManager.instance.PlaySoundName("Drag", false);
                    anim.Play("Teased", 0);
                    shadow.SetActive(false);
                    charInteract.interactType = InteractType.Jump;
                    yield return new WaitForEndOfFrame();
                    float ySpeed = 30 * anim.GetCurrentAnimatorStateInfo(0).length / 2;
                    if (anim.GetCurrentAnimatorStateInfo(0).length < 2)
                    {
                        anim.speed = 0.5f;
                        ySpeed = 60 * anim.GetCurrentAnimatorStateInfo(0).length / 2;
                    }

                    while (charInteract.interactType == InteractType.Jump && !isAbort)
                    {
                        ySpeed -= 30 * Time.deltaTime;
                        Vector3 pos1 = agent.transform.position;
                        pos1.y += ySpeed * Time.deltaTime;

                        if (count == n - 1)
                        {
                            pos1.x += 15 * Time.deltaTime;
                            Debug.Log(pos1.x);
                        }
                        agent.transform.position = pos1;

                        if (ySpeed < 0 && this.transform.position.y < dropPosition.y)
                        {
                            if (count == n - 1)
                            {
                                MageManager.instance.PlaySoundName("whoosh_swish_med_03", false);
                                yield return StartCoroutine(DoAnim("Drop"));

                            }
                            else
                            {
                                agent.transform.position = dropPosition;
                            }
                            charInteract.interactType = InteractType.None;

                        }
                        data.Energy -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    count++;
                }
               
                GameManager.instance.AddExp(5, data.iD);
                
            }else if(toyItem.toyType == ToyType.Ball)
            {
                ToyBallItem ball = toyItem.GetComponent<ToyBallItem>();
                bool isPlay = false;
                bool isFear = false;
                if (charType == CharType.Cat)
                    isPlay = true;

                int ran1 = Random.Range(0, 100);
                if((charType == CharType.Dog || charType == CharType.Hamster) && ran1 > 70 && data.Energy > data.MaxEnergy * 0.1f)
                {
                    isPlay = true;
                }

                if (charType == CharType.Turtle || charType == CharType.Parrot || charType == CharType.Rabbit)
                    isFear = true;


                if (isPlay)
                {

                    charScale.speedFactor = 1.5f;
                    
                    while (ball != null && data.Energy > data.MaxEnergy * 0.1f && !isAbort)
                    {
                        
                        anim.speed = 1.5f;
                        agent.SetDestination(ball.transform.position + new Vector3(Random.Range(-2f,2f),Random.Range(-2f,2f),0));
                        anim.Play("Run_" + this.direction.ToString(), 0);
                        data.Energy -= 2f * Time.deltaTime;
                        if(Vector2.Distance(this.transform.position, ball.scalePosition) < 2 && ball.state != ItemDragState.Drag)
                        {
                            agent.Stop();
                            data.Energy -= 2;
                            ball.OnForce();
                            int ran = Random.Range(0, 100);
                            if(ran < 20)
                                GameManager.instance.AddExp(5,data.iD);
                            yield return StartCoroutine(DoAnim("Standby"));
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    charScale.speedFactor = 1f;
                }

                if(isFear)
                {
                    if (ball != null && Vector2.Distance(this.transform.position, ball.scalePosition) < 2 && ball.state == ItemDragState.Drop)
                    {
                        yield return StartCoroutine(DoAnim("Teased"));
                        Debug.Log("Supprised");
                    }
                }
            }
            else if (toyItem.toyType == ToyType.Wheel)
            {
                ToyWheelItem wheel = toyItem.GetComponent<ToyWheelItem>();
                bool isPlay = false;
                if ((charType == CharType.Cat || charType == CharType.Hamster) && data.Energy > 0.1f * data.MaxEnergy)
                    isPlay = true;

                

                if (isPlay)
                {
                    MageManager.instance.PlaySoundName("Wheel", false);
                    float time = 0;
                    charScale.speedFactor = 2f;
                    anim.speed = 2f;
                    SetDirection(Direction.L);
                    wheel.OnActive();
                    float maxTime = Random.Range(5, 10);
                    while (wheel != null && data.Energy > data.MaxEnergy * 0.1f && !isAbort && time < maxTime)
                    {
                        time += Time.deltaTime;
                        agent.transform.position = wheel.anchorPoint.position;
                        anim.Play("Run_" + this.direction.ToString(), 0);
                        data.Energy -= 2f * Time.deltaTime;                        
                        yield return new WaitForEndOfFrame();

                    }
                    int ran = Random.Range(0, 100);
                    if (charType == CharType.Hamster)
                        ran += 30;

                    if(ran < 50)
                    {
                        agent.transform.position = wheel.anchorPoint.position + new Vector3(0,3,0); 
                        time = 0;
                        while (time < 3 && !isAbort)
                        {
                            anim.Play("Idle_" + this.direction.ToString(), 0);
                            this.transform.parent = wheel.wheel.transform;
                            time += Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                        this.transform.rotation = Quaternion.identity;
                        this.transform.parent = null;
                        yield return StartCoroutine(DoAnim("Teased"));
                    }
                    else
                    {
                        
                        if (time > 5)
                        {
                            yield return StartCoroutine(DoAnim("Love"));
                            GameManager.instance.AddExp(5, data.iD);
                        }
  
                    }

                    //MageManager.instance.StopSound(soundId);
                    charScale.speedFactor = 1f;
                    wheel.DeActive();
                   
                }
                target = wheel.anchorPoint.position + new Vector3(0, 3, 0);
                yield return StartCoroutine(RunToPoint());
            }
        }
        anim.speed = 1f;

        CheckAbort();
    }

    protected virtual IEnumerator Happy()
    {
        yield return StartCoroutine(DoAnim("Love"));
        CheckAbort();
    }

    protected virtual IEnumerator Itchi()
    {
        anim.Play("Itching", 0);
        Debug.Log("Itchi");
        while (data.itchi > 0.5 * data.MaxItchi && !isAbort)
        {
            data.itchi -= 1f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Supprised(){
        int ran = Random.Range(0,100);
        if (ran > 50)
        {
            MageManager.instance.PlaySoundName(charType.ToString() + "_Supprised", false);
            yield return StartCoroutine(DoAnim("Teased"));
        }
        else
        {
            yield return StartCoroutine(DoAnim("Love"));
            GameManager.instance.AddExp(5, data.iD);
        }
        data.Energy -= 2;
        CheckAbort();
    }

    protected virtual IEnumerator Control()
    {
        MageManager.instance.PlaySoundName("Drag", false);
        charInteract.interactType = InteractType.Touch;
        enviromentType = EnviromentType.Room;
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

        //Start Drop
        CheckDrop();

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
        MageManager.instance.PlaySoundName("whoosh_swish_med_03", false);
        yield return StartCoroutine(DoAnim("Drop"));


        CheckAbort();
    }

    protected virtual IEnumerator Fall()
    {
        MageManager.instance.PlaySoundName("Fall",false);
       
        data.Damage += Random.Range(2,10);
        MageManager.instance.PlaySoundName("Drop", false,1);
        yield return StartCoroutine(DoAnim("Fall_" + direction.ToString()));
        
        CheckAbort();
    }

    protected void CheckDrop(){
        enviromentType = EnviromentType.Room;
        dropPosition = charScale.scalePosition;
        ItemCollider col = ItemManager.instance.GetItemCollider(dropPosition);

        
        if (col != null){
           
            if (col.tag == "Table"){
                enviromentType = EnviromentType.Table;
            }else if(col.tag == "Bath"){
                enviromentType = EnviromentType.Bath;
            }else if(col.tag == "Bed"){
                enviromentType = EnviromentType.Bed;
            }else if(col.tag == "Toilet"){
                enviromentType = EnviromentType.Toilet;
            }
            else if (col.tag == "Door")
            {
                Debug.Log(col.name + col.tag);
                enviromentType = EnviromentType.Door;
            }
            else if (col.tag == "ToHouse")
            {
                Debug.Log("ToHouse");
                enviromentType = EnviromentType.ToHouse;
            }
            //if(enviromentType != EnviromentType.Room)
            //    GameManager.instance.CheckEnviroment(this,enviromentType);
            dropPosition.y = charScale.scalePosition.y + col.height;
            if(this.transform.position.x > col.transform.position.x + col.width/2 - col.edge)
            {
                dropPosition.x = col.transform.position.x + col.width/2 - col.edge;
            }else if(this.transform.position.x < col.transform.position.x - col.width/2 + col.edge){
                dropPosition.x = col.transform.position.x - col.width/2 + col.edge;
            }
        }

        
    }

    protected void CheckAbort()
    {
        agent.maxSpeed = data.speed;
        isAction = false;
        if (!isAbort)
            actionType = ActionType.None;
        else
        {
            LogAction();
            DoAction();
        }
    }

    protected void CheckEnviroment(){
        Debug.Log("Check enviroment");
        Debug.Log(isAbort);
        if(isAbort)
            return;
        if (enviromentType == EnviromentType.Bath)
        {
            OnBath();
        }
        else if (enviromentType == EnviromentType.Table)
        {
            OnTable();
        }
        else if (enviromentType == EnviromentType.Bed)
        {
            OnBed();
        }
        else if (enviromentType == EnviromentType.Toilet)
        {
            OnToilet();
        }
    }

    #endregion

    #region Skill

    public void OnLearnSkill(SkillType type){
        currentSkill = type;
        //skillLearnEffect.SetActive(true);
        ItemManager.instance.ActivateSkillItems(type);
    }

    public void OffLearnSkill(){
        ItemManager.instance.DeActivateSkillItems(currentSkill);
        currentSkill = SkillType.NONE;
    }
    public void LevelUpSkill(SkillType type){
        data.LevelUpSkill(type);
        OffLearnSkill();
    }

    protected IEnumerator SkillUp(){
        yield return StartCoroutine(DoAnim("SkillUp"));
        
        if (enviromentType == EnviromentType.Bath)
        {
            OnBath();
        }
        else if (enviromentType == EnviromentType.Table)
        {
            OnTable();
        }
        else if (enviromentType == EnviromentType.Bed)
        {
            OnBed();
        }
        else if (enviromentType == EnviromentType.Toilet)
        {
            OnToilet();
        }
        CheckAbort();
    }

    #endregion

    #region Effect



    #endregion

    public FoodBowlItem GetFoodItem()
    {
        if(foodItem == null)
            foodItem = FindObjectOfType<FoodBowlItem>();
        return foodItem;
    }

    public DrinkBowlItem GetDrinkItem()
    {
        if (drinkItem == null)
            drinkItem = FindObjectOfType<DrinkBowlItem>();
        return drinkItem;
    }

    public MouseController GetMouse()
    {
        if (mouse == null)
            mouse = FindObjectOfType<MouseController>();
        return mouse;
    }

    #region  getpoint


	public void SetTarget(PointType type)
	{
        int n = 0;
        Vector3 pos = GetRandomPoint (type).position;
        while(pos == target && n<10)
        {
            pos = GetRandomPoint (type).position;
            n++;
        }
        if(type == PointType.Patrol){
            target = pos + new Vector3(Random.Range(-2f,2f),Random.Range(-2f,2f),0);
        }else
            target = pos + new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f),0);

	}

 List<GizmoPoint> GetPoints(PointType type)
	{
		List<GizmoPoint> temp = new List<GizmoPoint>();
		GizmoPoint[] points = GameObject.FindObjectsOfType <GizmoPoint> ();
		for(int i=0;i<points.Length;i++)
		{
			if(points[i].type == type)
				temp.Add(points[i]);
		}
		return temp;
	}

	public Transform GetRandomPoint(PointType type)
	{
		List<GizmoPoint> points = GetPoints (type);
		if(points != null && points.Count > 0){
			int id = Random.Range (0, points.Count);
			return points [id].transform;
		}else
			return null;

	}

	public List<Transform> GetRandomPoints(PointType type)
	{
		List<GizmoPoint> points = GetPoints (type);
		List<Transform> randomPoints = new List<Transform> ();
		for (int i = 0; i < points.Count; i++) {
			randomPoints.Add (points [i].transform);
		}

		for (int i = 0; i < randomPoints.Count; i++) {
			if (i < randomPoints.Count - 1) {
				int j = Random.Range (i, randomPoints.Count);
				Transform temp = randomPoints [i];
				randomPoints [i] = randomPoints [j];
				randomPoints [j] = temp;
			}
		}
		return randomPoints;
	}
    #endregion

    void OnDestroy(){
        if(agent != null)
            GameObject.Destroy(agent.gameObject);
    }

}



