﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;


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

    //Action
    public ActionType actionType = ActionType.None;
    //public bool isEndAction = false;
    //Anim
    //CharAnim charAnim;
    protected Animator anim;
    public GameObject body;

    //Interact
    public CharInteract charInteract;
    public bool isTouch = false;
    //Pee,Sheet
    public Transform peePosition;
    public Transform shitPosition;
    public GameObject peePrefab;
    public GameObject shitPrefab;
    public GameObject dirtyEffect;

    public GameObject touchObject;
    #endregion

    //Skill
    public SkillType currentSkill = SkillType.NONE;
    public GameObject skillLearnEffect;

    public float dragOffset = 20f;

    protected Vector3 dropPosition;
    public float cameraSize = 24;

    FoodBowlItem foodItem;
    DrinkBowlItem drinkItem;
    MouseController mouse;

    public GameObject growUpTimeline;
    System.DateTime playTime = System.DateTime.Now;

    #region Load

    void Awake()
    {

    }


    public void LoadPrefab(){
        GameObject go = Instantiate(petPrefab) as GameObject;
        go.transform.parent = this.transform;

        anim = go.transform.GetComponent<Animator>();
        charInteract = this.GetComponent<CharInteract>();

        GameObject go1 = Instantiate(Resources.Load("Prefabs/Pets/Agent")) as GameObject;
        go1.transform.parent = GameManager.instance.transform;
		agent = go1.GetComponent<PolyNavAgent>();
		agent.LoadCharacter(this);

        touchObject = this.transform.GetComponentInChildren<TouchPoint>(true).gameObject;
        if(touchObject != null)
            touchObject.SetActive(false);
        if(skillLearnEffect != null)
            skillLearnEffect.SetActive(false);

        if(ES2.Exists("PlayTime")){
            playTime = ES2.Load<System.DateTime>("PlayTime");
        }

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
    void FixedUpdate()
    {
        if(GameManager.instance.isPause)
            return;

        if (agent == null)
            return;

        //Debug.Log(actionType.ToString() + "  " + charInteract.interactType.ToString());
        if (actionType == ActionType.None)
        {
            Think();
            DoAction();
            LogAction();
        }


        CalculateDirection();

        this.transform.position = agent.transform.position;

        //Calculate Attribue Data
        if (dataTime > maxDataTime)
        {
            CalculateData();
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

        if(ran < data.GetSkillProgress(SkillType.Call) * 10){
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
        if(actionType == ActionType.SkillUp)
            return;

        Abort();
        actionType = ActionType.Hold;
    }

    public virtual void OnEat(){
        if(actionType == ActionType.Patrol || actionType == ActionType.Rest){
            actionType = ActionType.Eat;
            isAbort = true;
        }
    }

    public virtual void OnDrink(){
        actionType = ActionType.Drink;
        isAbort = true;
    }

    protected void OnBath()
    {
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
        if (actionType == ActionType.Mouse || actionType == ActionType.Hold || actionType == ActionType.OnTable || actionType == ActionType.Bath || actionType == ActionType.Sick
        || actionType == ActionType.Sleep || actionType == ActionType.Call || actionType == ActionType.Listening || actionType == ActionType.Fear)
            return;

        if (actionType == ActionType.Pee || actionType == ActionType.Shit || actionType == ActionType.SkillUp)
        {
            return;
        }

        int n = 0;

        if (actionType == ActionType.Drink || actionType == ActionType.Eat)
        {
            n = Random.Range(25, 100);
        }

        if (n < 75)
        {
            Abort();
            actionType = ActionType.Mouse;
        }

    }

    public virtual void OffMouse()
    {
        if (actionType == ActionType.Mouse)
            actionType = ActionType.None;
    }

    public virtual void OnArrived()
    {
        //Debug.Log("Arrived");

        if (actionType == ActionType.Mouse)
        {
            actionType = ActionType.None;
            agent.speed = 30;
            anim.speed = 1;
        }

        isArrived = true;
    }

    public virtual void OnEvent(ItemEventType e){
        if(e == ItemEventType.Eat){
            if(data.food < 50)
                OnEat();    
        }else if(e == ItemEventType.Drink){
            if(data.water < 50){
                OnDrink();
            }
        }
    }

    public virtual void OnLevelUp()
    {
        Abort();
        if(actionType == ActionType.None)
        {
            actionType = ActionType.LevelUp;
            DoAction();
        }else
            actionType = ActionType.LevelUp;
    }

    #endregion
    #region Thinking
    protected virtual void Think()
    {

    }

    protected virtual void DoAction(){

    }

    void LogAction(){
        ApiManager.GetInstance().LogAction(actionType);
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
        if (Vector2.Distance(target, agent.transform.position) > 0.5f)
        {
            if(!isAbort){
                agent.SetDestination(target);
            }

            while (!isArrived && !isAbort)
            {
                if(actionType == ActionType.Fear ){
                    anim.Play("RunScared_" + this.direction.ToString(), 0);
                }else
                    anim.Play("Run_" + this.direction.ToString(), 0);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            agent.transform.position = target;
            isArrived = true;
            agent.Stop();
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

    protected IEnumerator JumpDown(float height,float upSpeed,float accelerator){
        if(!isAbort){
            anim.Play("Jump_D", 0);
            float speed = upSpeed;
            dropPosition = new Vector3(this.transform.position.x, this.transform.position.y - height, 0);
            charInteract.interactType = InteractType.Drop;
            while (charInteract.interactType == InteractType.Drop && !isAbort)
            {
                speed -= accelerator * Time.deltaTime;
                if (speed < -50)
                    speed = -50;
                Vector3 pos1 = agent.transform.position;
                pos1.y += speed * Time.deltaTime;
                pos1.x = agent.transform.position.x;
                
                if(speed < 0)
                    pos1.z = dropPosition.y;
                else 
                    pos1.z = agent.transform.position.y;
                agent.transform.position = pos1;

                if (Mathf.Abs(agent.transform.position.y - dropPosition.y) < 2f)
                {
                    this.transform.rotation = Quaternion.identity;
                    charInteract.interactType = InteractType.None;
                }
                yield return new WaitForEndOfFrame();
            }
            enviromentType = EnviromentType.Room;
        }
    }

    protected IEnumerator JumpUp(float height,float upSpeed){
        if(!isAbort){
            anim.Play("Jump_U", 0);
            float speed = upSpeed;
            CheckDrop(5);
            charInteract.interactType = InteractType.Drop;
            while (charInteract.interactType == InteractType.Drop && !isAbort)
            {
                speed -= 30 * Time.deltaTime;
                if (speed < -50)
                    speed = -50;
                Vector3 pos1 = agent.transform.position;
                pos1.y += speed * Time.deltaTime;
                pos1.x = agent.transform.position.x;
                pos1.z = dropPosition.y;
                agent.transform.position = pos1;

                if (speed < 0 && Mathf.Abs(agent.transform.position.y - dropPosition.y) < 1f)
                {
                    this.transform.rotation = Quaternion.identity;
                    charInteract.interactType = InteractType.None;
                    pos1.y = dropPosition.y;
                    pos1.x = agent.transform.position.x;
                    pos1.z = dropPosition.y;
                    agent.transform.position = pos1;
                }
                yield return new WaitForEndOfFrame();
            }
            enviromentType = EnviromentType.Room;
        }
    }

    protected void CheckDrop(float y){
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + new Vector3(0, y, 0), -Vector2.up, 100);
        Vector3 pos2 = this.transform.position;
        pos2.y = pos2.y - dragOffset - 2;
        if (pos2.y < -20)
            pos2.y = -20;
        dropPosition = pos2;
        enviromentType = EnviromentType.Room;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.tag == "Table")
            {
                pos2.y = hit[i].collider.transform.position.y;
                pos2.z = hit[i].collider.transform.position.z;
                dropPosition = pos2;
                enviromentType = EnviromentType.Table;
                break;
            }
            else if (hit[i].collider.tag == "Bath")
            {
                pos2.y = hit[i].collider.transform.position.y;
                pos2.z = hit[i].collider.transform.position.z;
                dropPosition = pos2;
                enviromentType = EnviromentType.Bath;
                break;
            }
            else if (hit[i].collider.tag == "Bed")
            {
                pos2.y = hit[i].collider.transform.position.y;
                pos2.z = hit[i].collider.transform.position.z;
                dropPosition = pos2;
                enviromentType = EnviromentType.Bed;
                break;
            }
            else if (hit[i].collider.tag == "Toilet")
            {
                pos2.y = hit[i].collider.transform.position.y;
                pos2.z = hit[i].collider.transform.position.z;
                dropPosition = pos2;
                enviromentType = EnviromentType.Toilet;
                break;
            }
        }
    }

    protected void CheckAbort()
    {
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
        skillLearnEffect.SetActive(true);
        ItemManager.instance.ActivateSkillItems(type);
        if(data.GetSkillProgress(type) == 1)
            UIManager.instance.OnQuestNotificationPopup("Bạn có thể dậy cho thú cưng kỹ năng rồi đấy, Hãy bế chó vào vị trí như chỉ dẫn");
    }

    public void OffLearnSkill(){
        ItemManager.instance.DeActivateSkillItems(currentSkill);
        currentSkill = SkillType.NONE;
        skillLearnEffect.SetActive(false);
    }
    public void LevelUpSkill(SkillType type){
        data.LevelUpSkill(type);
        if(data.SkillLearned(currentSkill))
            UIManager.instance.OnSkillCompletePanel(currentSkill);
        
        if(data.GetSkillProgress(type) == 1)
            UIManager.instance.OnQuestNotificationPopup("Bạn đã hoàn thành được 1 điểm kỹ năng hãy tiếp tục nhé");
        else if(data.GetSkillProgress(type) == 5)
            UIManager.instance.OnQuestNotificationPopup("Cố lên bạn sắp hoàn thành rồi");
       
        OffLearnSkill();
        Abort();
        actionType = ActionType.SkillUp;
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



