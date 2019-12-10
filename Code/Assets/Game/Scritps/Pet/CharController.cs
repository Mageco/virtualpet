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


    #region Load

    void Awake()
    {

    }


    public void LoadPrefab(){
        GameObject go = Instantiate(petPrefab) as GameObject;
        go.transform.parent = this.transform;

        anim = go.transform.GetComponent<Animator>();
        charInteract = this.GetComponent<CharInteract>();
        charScale = this.GetComponent<CharScale>();

        GameObject go1 = Instantiate(Resources.Load("Prefabs/Pets/Agent")) as GameObject;
        go1.transform.parent = GameManager.instance.transform;
		agent = go1.GetComponent<PolyNavAgent>();
        agent.OnDestinationReached += OnArrived;
        
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

        Load();
    }

    public void LoadTime(float t){

        Debug.Log("Load Time " + t);
        int n = (int)(t/10);
        data.Sleep = data.maxSleep - (t%28800)*0.005f; 
        
        for(int i=0;i<n;i++){
            data.actionEnergyConsume = 3;
            data.Energy -= data.basicEnergyConsume * 10 + data.actionEnergyConsume;
            data.Happy -= data.happyConsume * 10;
 
            if (data.Food > 0)
            {
                data.Food -= 1;
                data.Energy += 1;
                data.Shit += 1;
            }else{
                if(GetFoodItem() != null){
                    if(GetFoodItem().foodAmount >= data.maxFood){
                        data.Food = data.maxFood;
                        GetFoodItem().foodAmount -= data.maxFood;
                    }else{
                        data.Food += GetFoodItem().foodAmount;
                        GetFoodItem().foodAmount = 0;
                    }
                }
            }

            if (data.Water > 0)
            {
                data.Water -= 1;
                data.Energy += 1;
                data.Pee += 1;
            }else{
                if(GetDrinkItem() != null){
                    if(GetDrinkItem().foodAmount >= data.maxWater){
                        data.Water = data.maxWater;
                        GetDrinkItem().foodAmount -= data.maxWater;
                    }else{
                        data.Water += GetFoodItem().foodAmount;
                        GetDrinkItem().foodAmount = 0;
                    }
                }
            }

            if(data.Shit > data.maxShit){
                float y = Random.Range(-20,20);
                SpawnShit( new Vector3(Random.Range(-40,40),y,y));
                data.Shit = 0;
            }

            if(data.Pee > data.maxPee){
                SpawnPee( new Vector3(Random.Range(-40,40),Random.Range(-20,20),50));
                data.Pee = 0;
            }

            data.Dirty += data.dirtyFactor * 10;
            data.Itchi += data.Dirty * 0.01f;
            
            float deltaHealth = data.healthConsume;

            deltaHealth += (data.Happy - data.maxHappy * 0.3f) * 0.01f;

            if (data.Dirty > data.maxDirty * 0.8f)
                deltaHealth -= (data.Dirty - data.maxDirty * 0.8f) * 0.03f;

            if (data.Food < data.maxFood * 0.1f)
                deltaHealth -= (data.maxFood * 0.1f - data.Food) * 0.01f;

            if (data.Water < data.maxWater * 0.1f)
                deltaHealth -= (data.maxWater * 0.1f - data.Water) * 0.01f;

            data.Health += deltaHealth;
        }

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

    }

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
        }else if(data.energy < 0.1f*data.maxEnergy){
            iconStatus = IconStatus.Tired_2;
        }else if(data.energy < 0.3f*data.maxEnergy){
            iconStatus = IconStatus.Tired_1;
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
         if (agent.transform.eulerAngles.z < 30f && agent.transform.eulerAngles.z > -30f || (agent.transform.eulerAngles.z > 330f && agent.transform.eulerAngles.z < 390f) || (agent.transform.eulerAngles.z < -330f && agent.transform.eulerAngles.z > -390f))
            direction = Direction.U;
        else if ((agent.transform.eulerAngles.z > 30f && agent.transform.eulerAngles.z < 80f) || (agent.transform.eulerAngles.z > -330f && agent.transform.eulerAngles.z < -280f))
            direction = Direction.LU;
        else if ((agent.transform.eulerAngles.z >= 80f && agent.transform.eulerAngles.z <= 150f) || (agent.transform.eulerAngles.z >= -280f && agent.transform.eulerAngles.z <= -210f))
            direction = Direction.LD;
        else if ((agent.transform.eulerAngles.z <= -30f && agent.transform.eulerAngles.z >= -80f) || (agent.transform.eulerAngles.z >= 280f && agent.transform.eulerAngles.z <= 330f))
            direction = Direction.RU;
        else if ((agent.transform.eulerAngles.z <= -80 && agent.transform.eulerAngles.z >= -150) || (agent.transform.eulerAngles.z >= 210f && agent.transform.eulerAngles.z <= 280f))
            direction = Direction.RD;
        else
            direction = Direction.D; 



        //if (agent.transform.eulerAngles.z < 180f && agent.transform.eulerAngles.z > 0f || (agent.transform.eulerAngles.z > -360f && agent.transform.eulerAngles.z < -180f))
        //    direction = Direction.LD;
        //else 
        //    direction = Direction.RD;
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

        if(ran < 50 + data.GetSkillProgress(SkillType.Call) * 5){
            target = pos;
            actionType = ActionType.Call;
        }else{
            actionType = ActionType.Listening;
            OnLearnSkill(SkillType.Call);
        }
            
       
        //touchObject.SetActive(true);
    }

    public virtual void OnListening(float sound)
    {
        if (actionType == ActionType.Bath || actionType == ActionType.Fear || actionType == ActionType.Listening || actionType == ActionType.Hold || actionType == ActionType.Sick || actionType == ActionType.Sleep)
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
        Abort();
        charInteract.interactType = InteractType.Drag;
        actionType = ActionType.Hold;
    }

    public virtual void OnEat(){
 
    }

    protected void OnBath()
    {
        if(data.GetSkillProgress(SkillType.Bath) == 0){
            UIManager.instance.OnQuestNotificationPopup("Bạn có thể dùng xà bông và vòi hoa sen để tắm cho thú cưng");
        }
        Abort();
        SetDirection(Direction.D);
        actionType = ActionType.Bath;
    }

    public void OnHealth(SickType type,float value){
        if(type == SickType.Injured){
            data.Damage += value * Time.deltaTime;
        }else if(type ==SickType.Sick){
            data.Health += value * Time.deltaTime;
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
        if(direction ==  Direction.LD || direction == Direction.RD || direction == Direction.LU || direction == Direction.RU){
            if(!isArrived){
                Abort();
                isArrived = true;
                actionType = ActionType.Fall;
            }
        }
    }

    public virtual void OnSoap()
    {
        if (actionType == ActionType.Bath)
        {
            anim.Play("Soap_D", 0);
        }
    }

    public virtual void OnShower()
    {
        if (actionType == ActionType.Bath)
            anim.Play("Shower_LD", 0);
    }

    public virtual void OffShower()
    {
        if (actionType == ActionType.Bath)
            anim.Play("Shake_D", 0);
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
        
        if(GameManager.instance.gameType != GameType.Minigame1)
        {
            Abort();
            actionType = ActionType.LevelUp;
        }
    }

    #endregion
    #region Thinking
    protected virtual void Think()
    {

    }

    protected virtual void DoAction(){
        
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
    }

    protected IEnumerator DoAnim(string a)
    {
        float time = 0;
        anim.Play(a, 0);
        yield return new WaitForEndOfFrame();
        while (time < anim.GetCurrentAnimatorStateInfo(0).length && !isAbort)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveToPoint()
    {
        isArrived = false;
        //if (Vector2.Distance(target, agent.transform.position) > 0.5f)
        //{

            agent.SetDestination(target);
            

            while (!isArrived && !isAbort)
            {
                if(GameManager.instance.gameType == GameType.Minigame1){
                    anim.Play("Run_Angry_" + this.direction.ToString(), 0);
                }else if(actionType == ActionType.Fear ){
                    anim.Play("RunScared_" + this.direction.ToString(), 0);
                }else
                    anim.Play("Run_" + this.direction.ToString(), 0);
                yield return new WaitForEndOfFrame();
            }
        //}
        //else
        //{
        //    agent.transform.position = target;
        //    isArrived = true;
        //    agent.Stop();
        //}
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

    protected IEnumerator JumpDown(float downSpeed,float upSpeed,float accelerator){

        anim.Play("Jump_D", 0);
        float speed = upSpeed;
        //charScale.scalePosition = new Vector3(this.transform.position.x, this.transform.position.y - height, 0);
        charInteract.interactType = InteractType.Jump;
        while (charInteract.interactType == InteractType.Jump && !isAbort)
        {
            speed -= accelerator * Time.deltaTime;
            if (speed < -50)
                speed = -50;
            Vector3 pos1 = agent.transform.position;
            pos1.y += speed * Time.deltaTime + downSpeed * Time.deltaTime;                
            charScale.scalePosition.y += downSpeed * Time.deltaTime;
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
        enviromentType = EnviromentType.Room;
        
    }

    protected IEnumerator JumpUp(float ySpeed,float zSpeed, Vector3 dropPosition,float height){
        if(!isAbort){
            anim.Play("Jump_U", 0);            
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
            enviromentType = EnviromentType.Room;
        }
    }

    protected virtual IEnumerator Hold()
    {
        charInteract.interactType = InteractType.Drag;
        enviromentType = EnviromentType.Room;
        GameManager.instance.SetCameraTarget(this.gameObject);
        SetDirection(Direction.D);
        if (data.Health < data.maxHealth * 0.1f)
        {
            anim.Play("Hold_Sick_D", 0);
        }
        else
        {
            anim.Play("Hold_D", 0);
        }
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
        CheckDrop(0);

        float fallSpeed = 0;
        float maxTime = 1;
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


                if (data.Health < data.maxHealth * 0.1f)
                {
                    SetDirection(Direction.D);
                    anim.Play("Drop_Sick_D", 0);
                    maxTime = 3;
                }
                else
                {
                    if(fallSpeed > 50 && enviromentType != EnviromentType.Room){
                        SetDirection(Direction.D);
                        anim.Play("Drop_Hard_D", 0);
                        maxTime = 2;
                    }
                    else
                    {
                        SetDirection(Direction.D);
                        anim.Play("Drop_Light_D", 0);
                        maxTime = 1;
                    }
                }

                charInteract.interactType = InteractType.None;
            }
            yield return new WaitForEndOfFrame();
        }
        GameManager.instance.ResetCameraTarget();
        yield return StartCoroutine(Wait(maxTime));
        if (data.Health < data.maxHealth * 0.1f)
        {
            actionType = ActionType.None;
        }
        else
        {
            CheckEnviroment();            
        }
       
        CheckAbort();
    }

    protected virtual IEnumerator Bath()
    {
        int ran = Random.Range(0,100);
        if(ran < 70 + data.GetSkillProgress(SkillType.Bath) * 3){
            anim.Play("Idle_" + direction.ToString(),0);
            while(!isAbort){   
                yield return new WaitForEndOfFrame();
            }
        }
        else{
            yield return StartCoroutine(JumpDown(-5,25,35));
            OnLearnSkill(SkillType.Bath);            
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
                yield return StartCoroutine(MoveToPoint());
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Toilet);
                yield return StartCoroutine(JumpUp(10,5,col.transform.position + new Vector3(0,col.height,0),col.height));
                enviromentType = EnviromentType.Toilet;
            }else{
                OnLearnSkill(SkillType.Toilet);
            }
        }

        SetDirection(Direction.D);
        anim.Play("Pee_D", 0);
        Debug.Log("Pee");
        SpawnPee(peePosition.position + new Vector3(0, 0, 50));
        while (data.Pee > 1 && !isAbort)
        {
            data.Pee -= 0.5f;
            yield return new WaitForEndOfFrame();
        }

        if(enviromentType == EnviromentType.Toilet){
            GameManager.instance.AddExp(5,data.iD);
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
                yield return StartCoroutine(MoveToPoint());
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Toilet);
                yield return StartCoroutine(JumpUp(10,5,col.transform.position + new Vector3(0,col.height,0),col.height));
                enviromentType = EnviromentType.Toilet;
            }else{
                OnLearnSkill(SkillType.Toilet);
            }
        }

        SetDirection(Direction.D);
        anim.Play("Poop_D", 0);
        SpawnShit(shitPosition.position);
        while (data.Shit > 1 && !isAbort)
        {
            data.Shit -= 0.5f;
            yield return new WaitForEndOfFrame();
        }

        if(enviromentType == EnviromentType.Toilet){
            GameManager.instance.AddExp(5,data.iD);
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
            yield return StartCoroutine(MoveToPoint());
            bool canEat = true;
            if (GetFoodItem().CanEat())
            {
                direction = Direction.LD;
                anim.Play("Eat_LD", 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Food < data.maxFood && !isAbort && canEat)
                {
                    data.Food += 0.3f;
                    GetFoodItem().Eat(0.3f);
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
                if(GetFoodItem() != null && GetFoodItem().GetComponent<ItemObject>() != null)
 			        GameManager.instance.LogAchivement(AchivementType.Eat,ActionType.None,GetFoodItem().GetComponent<ItemObject>().itemID);

            }else{
                int ran = Random.Range(0,100);
                if(ran < 20)
                    yield return DoAnim("Bark_D");
                else if(ran < 40)
                {
                    SetTarget(PointType.Patrol);
                    yield return StartCoroutine(MoveToPoint());
                    yield return DoAnim("Bark_D");
                    SetTarget(PointType.Eat);
                    yield return StartCoroutine(MoveToPoint());
                }else if(ran < 50)
                    yield return DoAnim("Bark_Sit_D");
                else if(ran < 70) 
                    yield return DoAnim("Idle_D");
                else if(ran < 80){
                    yield return DoAnim("Eat_LD");
                }else if(ran < 90){
                    yield return DoAnim("Smell_LD");
                }else{
                    yield return DoAnim("Smell_Bark_LD");
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
            yield return StartCoroutine(MoveToPoint());
            

            bool canDrink = true;

            if (GetDrinkItem().CanEat())
            {
                direction = Direction.LD;
                anim.Play("Drink_LD", 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Water < data.maxWater && !isAbort && canDrink)
                {
                    data.Water += 0.5f;
                    GetDrinkItem().Eat(0.5f);
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
                if(GetDrinkItem() != null && GetDrinkItem().GetComponent<ItemObject>() != null)
 			        GameManager.instance.LogAchivement(AchivementType.Drink,ActionType.None,GetDrinkItem().GetComponent<ItemObject>().itemID);
            }else{
                int ran = Random.Range(0,100);
                if(ran < 20)
                    yield return DoAnim("Bark_D");
                else if(ran < 40)
                {
                    SetTarget(PointType.Patrol);
                    yield return StartCoroutine(MoveToPoint());
                    yield return DoAnim("Bark_D");
                    SetTarget(PointType.Drink);
                    yield return StartCoroutine(MoveToPoint());
                }else if(ran < 50)
                    yield return DoAnim("Bark_Sit_D");
                else if(ran < 70) 
                    yield return DoAnim("Idle_D");
                else if(ran < 80){
                    yield return DoAnim("Drink_LD");
                }else if(ran < 90){
                    yield return DoAnim("Smell_LD");
                }else{
                    yield return DoAnim("Smell_Bark_LD");
                }

            }
        }
        CheckAbort();
    }

    protected virtual IEnumerator Bed()
    {
        int ran = Random.Range(0,100);
        if(ran < 50 + data.GetSkillProgress(SkillType.Sleep) * 5){
            if(data.sleep < 0.3f*data.maxSleep){
                actionType = ActionType.Sleep;
                Abort();
            }else{                    
                anim.Play("Idle_" + direction.ToString(),0);
                yield return StartCoroutine(Wait(Random.Range(2,6)));
                yield return StartCoroutine(JumpDown(-7,10,30));
            }
        }
        else{
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
                yield return StartCoroutine(MoveToPoint());
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Bed);
                yield return StartCoroutine(JumpUp(10,5,col.transform.position,col.height));
                enviromentType = EnviromentType.Bed;
            }else{
                OnLearnSkill(SkillType.Sleep);
            }
        }

       
        direction = Direction.LD;
        anim.Play("Sleep_LD", 0);

        while (data.Sleep < data.maxSleep && !isAbort)
        {
            data.Sleep += 0.01f;
            yield return new WaitForEndOfFrame();
        }
        
        yield return StartCoroutine(DoAnim("Stretch_L"));  

        if(enviromentType == EnviromentType.Bed){
            GameManager.instance.AddExp(5,data.iD);
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
            yield return StartCoroutine(MoveToPoint());
            int ran = Random.Range(0, 100);
            if (ran < 30)
            {
                SetDirection(Direction.D);
                anim.Play("StandBy", 0);
            }
            else
                anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(1, 3)));
            n++;
        }
        CheckAbort();
    }

    protected void CheckDrop(float y){
       // RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + new Vector3(0, y, 0), -Vector2.up, charScale.maxHeight);
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

    protected virtual IEnumerator LevelUp()
    {
        Debug.Log("Level Up" + data.level);
        yield return StartCoroutine(DoAnim("LevelUp_LD"));
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

	public void SetTarget(PointType type)
	{
        int n = 0;
        Vector3 pos = this.GetRandomPoint (type).position;
        while(pos == target && n<10)
        {
            pos = this.GetRandomPoint (type).position;
            n++;
        }
        target = pos;
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



