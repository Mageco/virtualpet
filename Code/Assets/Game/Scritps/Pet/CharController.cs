using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;


public class CharController : MonoBehaviour
{

    #region Declair
    //Data
    public Pet data;
    public GameObject petPrefab;
    //[HideInInspector]

    //[HideInInspector]
    public EnviromentType enviromentType = EnviromentType.Room;
    //[HideInInspector]
    public Direction direction = Direction.D;

    //Think
    protected float dataTime = 0;
    protected float maxDataTime = 1f;

    //Movement
    public Vector3 target;

    public PolyNavAgent agent;
    public bool isArrived = true;
    public bool isAbort = false;
    public bool isAction = false;

    //Action
    public ActionType actionType = ActionType.None;
    //public bool isEndAction = false;
    //Anim
    //CharAnim charAnim;
    protected Animator anim;
    public GameObject body;

    //Interact
    public CharInteract charInteract;
    public CharScale charScale;
    public bool isTouch = false;
    //Pee,Sheet
    public Transform peePosition;
    public Transform shitPosition;
    public GameObject peePrefab;
    public GameObject shitPrefab;
    public GameObject touchObject;
    #endregion

    //Skill
    public SkillType currentSkill = SkillType.NONE;
    FoodBowlItem foodItem;
    DrinkBowlItem drinkItem;
    MouseController mouse;
    public GameObject growUpTimeline;
    System.DateTime playTime = System.DateTime.Now;

    public Vector3 dropPosition = Vector3.zero;

    public SpriteRenderer iconStatusObject;
    public IconStatus iconStatus = IconStatus.None;
    IconStatus lastIconStatus = IconStatus.None;

    public List<GameObject> dirties = new List<GameObject>();

    #region Load

    void Awake()
    {

    }


    public void LoadPrefab(){
        GameObject go = Instantiate(petPrefab) as GameObject;
        go.transform.parent = this.transform;
		go.transform.localPosition = Vector3.zero;    

        anim = go.transform.GetComponent<Animator>();
        charInteract = this.GetComponent<CharInteract>();
        charScale = this.GetComponent<CharScale>();

        GameObject go1 = Instantiate(Resources.Load("Prefabs/Pets/Agent")) as GameObject;
        go1.transform.parent = GameManager.instance.transform;
		agent = go1.GetComponent<PolyNavAgent>();
        agent.OnDestinationReached += OnArrived;
        agent.transform.position = this.transform.position;
        
        agent.maxSpeed = data.speed;
        
        if(this.transform.GetComponentInChildren<TouchPoint>(true) != null)
            touchObject = this.transform.GetComponentInChildren<TouchPoint>(true).gameObject;
        if(touchObject != null)
            touchObject.SetActive(false);

        if(ES2.Exists("PlayTime")){
            playTime = ES2.Load<System.DateTime>("PlayTime");
        }

        if(iconStatusObject != null)
            iconStatusObject.gameObject.SetActive(false);


        //Load Dirty Effect
        //grab all the kids and only keep the ones with dirty tags
     
         Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
     
         foreach (Transform child in allChildren) {
             if (child.gameObject.tag == "Dirty") {
                 dirties.Add(child.gameObject);
             }
         }

        Load();
    }

    public void LoadTime(float t){

        Debug.Log("Load Time " + t);
        int n = (int)(t/10);
        data.Sleep = data.maxSleep - (t%28800)*0.005f; 

    }

    protected virtual void Load(){

    }
    // Use this for initialization
    void Start()
    {

    }
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
            if((System.DateTime.Now - playTime).Seconds > 10){
                LoadTime((System.DateTime.Now - playTime).Seconds);
            }
            playTime = System.DateTime.Now;
            ES2.Save(playTime,"PlayTime");
        }
        else
            dataTime += Time.deltaTime;


        
    }



    #endregion

    #region Data
    protected virtual void CalculateData()
    {
        float actionEnergyConsume = data.recoverEnergy;
        if (actionType == ActionType.Call)
            actionEnergyConsume = 0.2f;
        else if (actionType == ActionType.Mouse)
            actionEnergyConsume = 1.5f;
        else if (actionType == ActionType.Discover)
        {
            actionEnergyConsume = 1f;
        }
        else if (actionType == ActionType.Patrol)
        {
            actionEnergyConsume = 0.6f;
        }else if (actionType == ActionType.OnBath)
            actionEnergyConsume = 0.1f;
        else if (actionType == ActionType.Fear)
            actionEnergyConsume = 0.3f;
        
        data.Energy -= actionEnergyConsume;
        data.Food -= actionEnergyConsume/2;    
        data.Water -= actionEnergyConsume/2;
        data.Shit += actionEnergyConsume/2;
        data.Pee += actionEnergyConsume;

        data.Dirty += data.recoverDirty;
        if (data.Dirty > data.maxDirty * 0.7f)
            data.Itchi += (data.Dirty - data.maxDirty * 0.7f) * 0.005f;
        
        data.Sleep -= data.recoverSleep;

        float deltaHealth = data.recoverHealth;

        if (data.Dirty > data.maxDirty * 0.95f)
            deltaHealth -= (data.Dirty - data.maxDirty * 0.95f) * 0.005f;

        if (data.Pee > data.maxPee * 0.95f)
            deltaHealth -= (data.Pee - data.maxPee * 0.95f) * 0.005f;

        if (data.Shit > data.maxShit * 0.95f)
            deltaHealth -= (data.Shit - data.maxShit * 0.95f) * 0.005f;

        if (data.Food < data.maxFood * 0.05f)
            deltaHealth -= (data.maxFood * 0.05f - data.Food) * 0.005f;

        if (data.Water < data.maxWater * 0.05f)
            deltaHealth -= (data.maxWater * 0.05f - data.Water) * 0.005f;

        if (data.Sleep < data.maxSleep * 0.05f)
            deltaHealth -= (data.maxSleep * 0.05f - data.Sleep) * 0.01f;

        data.Health += deltaHealth;

        if(data.Health < 0.3f * data.maxHealth){
            data.Health -= (data.maxHealth * 0.3f - data.Health) * 0.001f;
        }
        data.curious += 0.1f;

        //CheckDirty
        if(data.dirty > data.maxDirty * 0.7f){
            int n = (int)((data.dirty - data.maxDirty * 0.7f)/(data.maxDirty * 0.3f) * dirties.Count);
            for(int i=0;i<dirties.Count;i++){
                if(i < n)
                    dirties[i].SetActive(true);
                else
                    dirties[i].SetActive(false);
            }
        }else
        {
            for(int i=0;i<dirties.Count;i++){
                dirties[i].SetActive(false);
            }
        }


    }



    #region Thinking
    protected virtual void Think()
    {
        if(charInteract.interactType != InteractType.None)
            return;

        if (data.Shit > data.maxShit * 0.9f)
        {
            actionType = ActionType.Shit;
            return;
        }

        if (data.Pee > data.maxPee * 0.9f)
        {
            actionType = ActionType.Pee;
            return;
        }

        if (data.Health < data.maxHealth * 0.1f)
        {
            actionType = ActionType.Sick;
            return;
        }

        if(data.Food < data.maxFood * 0.3f && GetFoodItem() != null && Vector2.Distance(this.transform.position,GetFoodItem().transform.position) < 3){
            actionType = ActionType.Eat;
            return;
        }

        if(data.Water < data.maxWater * 0.3f && GetDrinkItem() != null && Vector2.Distance(this.transform.position,GetDrinkItem().transform.position) < 3){
            actionType = ActionType.Drink;
            return;
        }

        if (data.Food < data.maxFood * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Eat;
                return;
            }
        }


        if (data.Water < data.maxWater * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Drink;
                return;
            }
        }



        if (data.Sleep < data.maxSleep * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Sleep;
                return;
            }
        }

        if (data.Itchi > data.maxItchi * 0.7f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 50)
            {
                actionType = ActionType.Itchi;
                return;
            }
        }

        if(data.Energy < data.maxEnergy * 0.1f){
            actionType = ActionType.Tired;
            return;
        }

        if (data.Energy < data.maxEnergy * 0.5f)
        {
            actionType = ActionType.Rest;
            return;
        }

        if (data.happy < data.maxHappy * 0.1f)
        {
            actionType = ActionType.Sad;
            return;
        }

        if (data.curious > data.maxCurious * 0.9f)
        {
            actionType = ActionType.Discover;
            return;
        }


        actionType = ActionType.Patrol;
        //Other Action
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

        if (actionType == ActionType.Rest)
        {
            StartCoroutine(Rest());
        }
        else if (actionType == ActionType.Patrol)
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
        else if (actionType == ActionType.Sad)
        {
            StartCoroutine(Sad());
        }
        else if (actionType == ActionType.Happy)
        {
           StartCoroutine(Happy());
        }
        else if (actionType == ActionType.Call)
        {
            StartCoroutine(Call());
        } else if (actionType == ActionType.Fall)
        {
            StartCoroutine(Fall());
        }else if(actionType == ActionType.Listening){
            StartCoroutine(Listening());
        }else if(actionType == ActionType.OnBed){
            StartCoroutine(Bed());
        }else if (actionType == ActionType.OnToilet)
        {
            StartCoroutine(Toilet());
        }else if (actionType == ActionType.JumpOut)
        {
            StartCoroutine(JumpOut());
        }else if (actionType == ActionType.Supprised)
        {
            StartCoroutine(Supprised());
        }      
    }

    #endregion

    protected virtual void CalculateStatus(){
       
        lastIconStatus = iconStatus;

        if(data.Health < 0.1f*data.maxHealth){
            iconStatus = IconStatus.Sick_2; 
        }else if(data.Health < 0.3f*data.maxHealth){
            iconStatus = IconStatus.Sick_1;
        }else if(data.Pee > 0.9f*data.maxPee || data.Shit > 0.9f*data.maxShit){
            iconStatus = IconStatus.Toilet_2; 
        }else if(data.Pee > 0.7f*data.maxPee || data.Shit > 0.7f*data.maxShit){
            iconStatus = IconStatus.Toilet_1;
        }else if(data.Food < 0.1f*data.maxFood){
            iconStatus = IconStatus.Hungry_2; 
        }else if(data.Food < 0.3f*data.maxFood){
            iconStatus = IconStatus.Hungry_1;
        }else if(data.Water < 0.1f*data.maxWater){
            iconStatus = IconStatus.Thirsty_2; 
        }else if(data.Water < 0.3f*data.maxWater){
            iconStatus = IconStatus.Thirsty_1;
        //}else if(data.energy < 0.1f*data.maxEnergy){
        //    iconStatus = IconStatus.Tired_2;
        //}else if(data.energy < 0.3f*data.maxEnergy){
        //    iconStatus = IconStatus.Tired_1;
        }else if(data.sleep < 0.1f*data.maxSleep){
            iconStatus = IconStatus.Sleeyp_2;
        }else if(data.sleep < 0.3f*data.maxSleep){
            iconStatus = IconStatus.Sleepy_1;
        }else if(data.dirty > 0.9f*data.maxDirty){
            iconStatus = IconStatus.Dirty_2; 
        }else if(data.dirty > 0.7f*data.maxDirty){
            iconStatus = IconStatus.Dirty_1;
        }else{
            iconStatus = IconStatus.None;
        }


        if(iconStatusObject != null){
            LoadIconStatus();
        }

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
    public virtual void OnCall(Vector3 pos)
    {
        if (actionType == ActionType.Hold || actionType == ActionType.Call || actionType == ActionType.Sick || actionType == ActionType.Sleep)
        {
            return;
        }
        Abort();
        int ran = Random.Range(0,100);

        if(ran < 70 + data.GetSkillProgress(SkillType.Call) * 3){
            target = pos;
            actionType = ActionType.Call;
        }else{
            actionType = ActionType.Listening;
            OnLearnSkill(SkillType.Call);
        }
    }

    public virtual void OnListening(float sound)
    {
        if (actionType == ActionType.OnBath || actionType == ActionType.Fear || actionType == ActionType.Listening || actionType == ActionType.Hold || actionType == ActionType.Sick || actionType == ActionType.Sleep)
        {
            return;
        }

        Abort();
        
        if(sound < 10){
            actionType = ActionType.Listening;
        }else {
            data.Fear += sound * 5;
            actionType = ActionType.Fear;
        } 
    }

    public void SetActionType(ActionType action){
        Abort();
        actionType = action;
    }

    public virtual void OnHold()
    {
        if(actionType == ActionType.Sick){
            UIManager.instance.OnQuestNotificationPopup("Bạn cần cho thú cưng uống thuốc");
            //return;
        }
            
        Abort();
        charInteract.interactType = InteractType.Drag;
        actionType = ActionType.Hold;
    }

    public virtual void OnSupprised(){
        if(actionType == ActionType.Hold || actionType == ActionType.Sick)
            return;

        Abort();
        actionType  = ActionType.Supprised;
    }

    public virtual void OnEat(){
 
    }

    protected void OnBath()
    {
        if(data.GetSkillProgress(SkillType.Bath) == 0){
            UIManager.instance.OnQuestNotificationPopup("Bạn có thể dùng xà bông và vòi hoa sen để tắm cho thú cưng");
        }
        Abort();
        
        actionType = ActionType.OnBath;
    }

    public void OnJumpOut()
    {  
        if(enviromentType == EnviromentType.Room)
            return;

        Abort();   
        actionType = ActionType.JumpOut;
    }

    public void OnHealth(SickType type,float value){
        if(type == SickType.Injured){
            data.Damage += value;
        }else if(type ==SickType.Sick){
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
        if(!isArrived){
            Abort();
            isArrived = true;
            actionType = ActionType.Fall;
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
        if (actionType == ActionType.OnBath)
            anim.Play("Shake", 0);
    }

    public virtual void OnTouch()
    {
        if (actionType == ActionType.Call)
        {
            isTouch = true;
        }
    }

    public virtual void OffTouch()
    {
        //if(actionType == ActionType.Call){
        isTouch = false;
        //}
    }


    public virtual void OnMouse()
    {
        int ran = Random.Range(0,100);
        if(ran < 50)
            return;

        if(actionType == ActionType.Patrol || actionType == ActionType.Rest || actionType == ActionType.Discover || actionType == ActionType.Drink || actionType == ActionType.Eat){
            Abort();
            actionType = ActionType.Mouse;
        }
    }

    public virtual void OffMouse()
    {
        //if (actionType == ActionType.Mouse)
        //    actionType = ActionType.None;
    }

    public virtual void OnToy(ToyType type){
        if(actionType == ActionType.Patrol){
            Abort();
            //actionType = ActionType.OnToy;
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
        if(touchObject != null)
            touchObject.SetActive(false);
    }



    protected void SetDirection(Direction d)
    {
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

    protected IEnumerator DoAnim(string a)
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

    protected IEnumerator RunToPoint()
    {
        isArrived = false;

        agent.SetDestination(target);
        

        while (!isArrived && !isAbort)
        {
            anim.Play("Run_" + this.direction.ToString(), 0);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator WalkToPoint()
    {
        isArrived = false;

        agent.SetDestination(target);
        

        while (!isArrived && !isAbort)
        {
            anim.Play("Walk_" + this.direction.ToString(), 0);
            yield return new WaitForEndOfFrame();
        }
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
            while (charInteract.interactType == InteractType.Jump && !isAbort)
            {
                ySpeed -= 30 * Time.deltaTime;
                if (ySpeed < -50)
                    ySpeed = -50;
                Vector3 pos1 = agent.transform.position;
                pos1.y += ySpeed * Time.deltaTime + zSpeed * Time.deltaTime;
                agent.transform.position = pos1;
                charScale.height += ySpeed * Time.deltaTime;
                charScale.scalePosition.y += zSpeed * Time.deltaTime;

                if (ySpeed < 0 && charScale.height < height)
                {
                    this.transform.rotation = Quaternion.identity;
                    charInteract.interactType = InteractType.None;
                    agent.transform.position = dropPosition;
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
        while(GetMouse() != null && GetMouse().state != MouseState.Idle && !isAbort){
            agent.SetDestination(GetMouse().transform.position);
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(0.1f));
        }
        CheckAbort();
    }

    protected virtual IEnumerator Hold()
    {
        charInteract.interactType = InteractType.Drag;
        enviromentType = EnviromentType.Room;
        GameManager.instance.SetCameraTarget(this.gameObject);
        if(data.Health < 0.1f * data.maxHealth)
            anim.Play("Sick",0);
        else
            anim.Play("Hold", 0);
        
        while (charInteract.interactType == InteractType.Drag)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - charInteract.dragOffset;
            pos.z = 0;
            if (pos.y > charScale.maxHeight)
                pos.y = charScale.maxHeight;
            else if (pos.y < -20)
                pos.y = -20;

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
        while (charInteract.interactType == InteractType.Drop && !isAbort)
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
        GameManager.instance.ResetCameraTarget();
        charInteract.interactType = InteractType.None; 
        

        if(data.Health < 0.1f * data.maxHealth)
        {
            actionType = ActionType.Sick;
            isAbort = true;
        }else{
            CheckEnviroment();
            yield return StartCoroutine(DoAnim("Drop"));
        }
            
    
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

    protected virtual IEnumerator Discover()
    {
        
        if (data.curious > data.maxCurious * 0.4f)
        {
            
            int ran1 = Random.Range(0,100);
            Vector3 t = GetRandomPoint(PointType.Patrol).position;
            if(ran1 < 5 && GetRandomPoint(PointType.Eat) != null){
                t = GetRandomPoint(PointType.Eat).position;
            }else if(ran1 < 10 && GetRandomPoint(PointType.Drink) != null){
                t = GetRandomPoint(PointType.Drink).position;                    
            }else if(ran1 < 15 && GetRandomPoint(PointType.Toilet) != null){
                t = GetRandomPoint(PointType.Toilet).position;                    
            }else if(ran1 < 20 && GetRandomPoint(PointType.Sleep) != null){
                t = GetRandomPoint(PointType.Sleep).position;                    
            }else if(ran1 < 25 && GetRandomPoint(PointType.Bath) != null){
                t = GetRandomPoint(PointType.Bath).position;                    
            }else if(ran1 < 30 && GetRandomPoint(PointType.Table) != null){
                t = GetRandomPoint(PointType.Table).position;                    
            }else if(ran1 < 35 && GetRandomPoint(PointType.Cleaner) != null){
                t = GetRandomPoint(PointType.Cleaner).position;                    
            }else if(ran1 < 40 && GetRandomPoint(PointType.Caress) != null){
                t = GetRandomPoint(PointType.Caress).position;                    
            }else if(ran1 < 45 && GetRandomPoint(PointType.Window) != null){
                t = GetRandomPoint(PointType.Window).position;                    
            }

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

    protected virtual IEnumerator Bath()
    {

        while(!isAbort){
            yield return new WaitForEndOfFrame();
        }   
        
        CheckAbort();
    }

    protected virtual IEnumerator Toilet()
    {
        if(data.shit > 0.7*data.maxShit){
            actionType = ActionType.Shit;
            isAbort = true;
        }else if(data.pee > 0.7f*data.maxPee){
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
        if(enviromentType != EnviromentType.Toilet)
        {
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

        
        anim.Play("Pee", 0);
        Debug.Log("Pee");
        SpawnPee(peePosition.position + new Vector3(0, 0, 50));
        while (data.Pee > 1 && !isAbort)
        {
            data.Pee -= data.ratePee * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if(enviromentType == EnviromentType.Toilet){
            GameManager.instance.AddExp(5,data.iD);
            GameManager.instance.AddHappy(10,data.iD);
            GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.OnToilet);
            LevelUpSkill(SkillType.Toilet);
            yield return StartCoroutine(JumpDown(-7,10,30));     
        }
        
        CheckAbort();
    }

    protected virtual IEnumerator Shit()
    {
        if(enviromentType != EnviromentType.Toilet)
        {
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

        
        anim.Play("Shit", 0);
        SpawnShit(shitPosition.position);
        while (data.Shit > 1 && !isAbort)
        {
            data.Shit -= data.rateShit * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if(enviromentType == EnviromentType.Toilet){
            GameManager.instance.AddExp(5,data.iD);
            GameManager.instance.AddHappy(10,data.iD);
            GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.OnToilet);
            LevelUpSkill(SkillType.Toilet);
            yield return StartCoroutine(JumpDown(-7,10,30));     
        }
        CheckAbort();
    }

    protected virtual IEnumerator Eat()
    {
        if (GetFoodItem() != null)
        {
            SetTarget(PointType.Eat);
            yield return StartCoroutine(RunToPoint());
            bool canEat = true;
            if (GetFoodItem().CanEat())
            {
                anim.Play("Eat", 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Food < data.maxFood && !isAbort && canEat)
                {
                    data.Food += data.rateFood*0.1f;
                    GetFoodItem().Eat(data.rateFood*0.1f);
                    if (!GetFoodItem().CanEat())
                    {
                        canEat = false;
                    }
                    if (Vector2.Distance(this.transform.position, GetFoodItem().anchor.position) > 1f)
                        canEat = false;
                    yield return new WaitForEndOfFrame();
                }
                GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Eat);
                GameManager.instance.AddExp(5,data.iD);
                GameManager.instance.AddHappy(5,data.iD);
                if(GetFoodItem() != null && GetFoodItem().GetComponent<ItemObject>() != null)
 			        GameManager.instance.LogAchivement(AchivementType.Eat,ActionType.None,GetFoodItem().GetComponent<ItemObject>().itemID);

            }else{
                int ran = Random.Range(0,100);
                if(ran < 20)
                    yield return DoAnim("Eat");
                else if(ran < 40)
                    yield return DoAnim("Standby");
                else{
                    SetTarget(PointType.Patrol);
                    yield return StartCoroutine(RunToPoint());
                    yield return DoAnim("Standby");
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

            SetTarget(PointType.Drink);
            yield return StartCoroutine(RunToPoint());
            

            bool canDrink = true;

            if (GetDrinkItem().CanEat())
            {
                
                anim.Play("Drink", 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Water < data.maxWater && !isAbort && canDrink)
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
                GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Drink);
                GameManager.instance.AddExp(5,data.iD);
                GameManager.instance.AddHappy(5,data.iD);
                if(GetDrinkItem() != null && GetDrinkItem().GetComponent<ItemObject>() != null)
 			        GameManager.instance.LogAchivement(AchivementType.Drink,ActionType.None,GetDrinkItem().GetComponent<ItemObject>().itemID);
            }else{
                int ran = Random.Range(0,100);
                if(ran < 20)
                    yield return DoAnim("Eat");
                else if(ran < 40)
                    yield return DoAnim("Standby");
                else{
                    SetTarget(PointType.Patrol);
                    yield return StartCoroutine(RunToPoint());
                    yield return DoAnim("Standby");
                }
            }
        }
        CheckAbort();
    }

    protected virtual IEnumerator Bed()
    {
        int ran = Random.Range(0,100);
        if(ran < 70 + data.GetSkillProgress(SkillType.Sleep) * 3){
            if(data.sleep < 0.3f*data.maxSleep){
                actionType = ActionType.Sleep;
                Abort();
            }else{                    
                anim.Play("Idle_" + direction.ToString(),0);
                yield return StartCoroutine(Wait(Random.Range(2,4)));
                yield return StartCoroutine(JumpDown(-7,10,30)); 
            }
        }
        else{
            yield return StartCoroutine(Wait(Random.Range(1,2)));
            yield return StartCoroutine(JumpDown(-7,10,30)); 
        }
        
        CheckAbort();
    }

    protected virtual IEnumerator Sleep()
    {
        if(enviromentType != EnviromentType.Bed)
        {
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

       
        
        anim.Play("Sleep", 0);

        while (data.Sleep < data.maxSleep && !isAbort)
        {
            data.Sleep += data.rateSleep;
            yield return new WaitForEndOfFrame();
        }
        
        if(enviromentType == EnviromentType.Bed){
            GameManager.instance.AddExp(20,data.iD);
            GameManager.instance.AddHappy(20,data.iD);
            GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Sleep);
            LevelUpSkill(SkillType.Sleep);
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
            SetTarget(PointType.Patrol);
            yield return StartCoroutine(RunToPoint());
            int ran = Random.Range(0, 100);
            if (ran < 30)
            {
                
                anim.Play("Standby", 0);
            }
            else
                anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(1, 10)));
            n++;
        }
        CheckAbort();
    }

    protected virtual IEnumerator Call()
    {
        yield return StartCoroutine(DoAnim("Idle_"+direction.ToString()));
        yield return StartCoroutine(RunToPoint());
        touchObject.SetActive(true);
        GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Call);
        anim.Play("Idle_" + direction.ToString(),0);
        yield return StartCoroutine(Wait(Random.Range(2f,5f)));
        CheckAbort();
    }

    protected virtual IEnumerator Listening(){
        yield return StartCoroutine(DoAnim("Idle_" + direction.ToString()));
        CheckAbort();
    }

    
    protected virtual IEnumerator Rest()
    {
        anim.Play("Idle_" + direction.ToString(),0);
        
        while(data.Food > 0 && data.Water > 0 && data.Sleep > 0 &&data.Energy < 0.9f * data.maxEnergy && !isAbort){
            data.Energy += 0.05f;
            data.Food -= 0.03f;
            data.Water -= 0.03f;
            data.Sleep -= 0.01f;
            yield return new WaitForEndOfFrame();
        }  
        yield return StartCoroutine(Wait(Random.Range(2f,5f)));
        CheckAbort();
    }

    protected virtual IEnumerator JumpOut()
    {
        if(enviromentType == EnviromentType.Bath){
            yield return StartCoroutine(JumpDown(-5,15,35));
        }else if(enviromentType == EnviromentType.Bed || enviromentType == EnviromentType.Toilet){
            yield return StartCoroutine(JumpDown(-7,10,30)); 
        }
        CheckAbort();
    }

    

    protected virtual IEnumerator Sick()
    {
        anim.Play("Sick", 0);
        Debug.Log("Sick");
        while (data.health < 0.5f * data.maxHealth && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Sick);
        CheckEnviroment();
        CheckAbort();
    }

    protected virtual IEnumerator Fear()
    {
        
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(DoAnim("Fear"));
        CheckAbort();
    }


    protected virtual IEnumerator Tired()
    {
        anim.Play("Tired", 0);
        while (data.Food > 0 && data.Water > 0 && data.Sleep > 0 && data.energy < data.maxEnergy * 0.1f && !isAbort)
        {
            data.Energy += 0.1f;
            data.Food -= 0.05f;
            data.Water -= 0.05f;
            data.Sleep += 0.01f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Sad()
    {
        yield return StartCoroutine(DoAnim("Sad"));
        CheckAbort();
    }

    protected virtual IEnumerator Happy()
    {
        yield return StartCoroutine(DoAnim("Happy"));
        CheckAbort();
    }

    protected virtual IEnumerator Itchi()
    {
        anim.Play("Itching", 0);
        Debug.Log("Itchi");
        while (data.itchi > 0.5 * data.maxItchi && !isAbort)
        {
            data.itchi -= 1f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Supprised(){
        anim.Play("Teased",0);
        while(!isAbort){
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected virtual IEnumerator Fall()
    {
        yield return StartCoroutine(DoAnim("Fall_" + direction.ToString()));
        CheckAbort();
    }

    protected void CheckDrop(){
        enviromentType = EnviromentType.Room;
        dropPosition = charScale.scalePosition;
        ItemCollider col = ItemManager.instance.GetItemCollider(dropPosition);

        if(col != null){
            if(col.tag == "Table"){
                enviromentType = EnviromentType.Table;
            }else if(col.tag == "Bath"){
                enviromentType = EnviromentType.Bath;
            }else if(col.tag == "Bed"){
                enviromentType = EnviromentType.Bed;
            }else if(col.tag == "Toilet"){
                enviromentType = EnviromentType.Toilet;
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
        if(!isAbort)
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
        if(data.GetSkillProgress(type) == 0){
            if(type == SkillType.Bath){
                UIManager.instance.OnQuestNotificationPopup("Thú cưng cửa bạn không phải lúc nào cũng muốn tắm, hãy kiên trì nhé, bạn thú cưng sẽ quen dần!");
            }else if(type == SkillType.Sleep){
                UIManager.instance.OnQuestNotificationPopup("Ngủ đúng chỗ sẽ có giấc ngủ ngon hơn và được nhiều điểm kinh nghiệm hơn, hãy huấn luyện thú cưng của bạn nhé!");
            }else if(type == SkillType.Toilet){
                UIManager.instance.OnQuestNotificationPopup("Đi ị và tè không đúng chỗ sẽ rất bẩn đấy, hãy huấn luyện chó của bạn dần nhé!");
            }else if(type == SkillType.Table){
                UIManager.instance.OnQuestNotificationPopup("Nhảy trên cao xuống có thể sẽ bị chấn thương đấy bạn cần huấn luyện nhé!");
            }
        }   
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
    protected void SpawnPee(Vector3 pos)
    {
        GameObject go = Instantiate(peePrefab, pos, Quaternion.identity);
    }

    protected void SpawnShit(Vector3 pos)
    {
        GameObject go = Instantiate(shitPrefab, pos, Quaternion.identity);
    }

    protected void SpawnFly(){
        //GameObject go = Instantiate(flyPrefab,Vector3.zero, Quaternion.identity); 
    }

    protected void GrowUp(){
         GameObject go = Instantiate(growUpTimeline, new Vector3(0, 0, -50), Quaternion.identity);
    }

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
        target = pos;

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



