using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CharController : MonoBehaviour
{

    #region Declair
    //Data
    public CharData data;
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

    [HideInInspector]
    public PolyNavAgent agent;
    public bool isArrived = true;
    public bool isAbort = false;

    //Action
    public ActionType actionType = ActionType.None;
    //public bool isEndAction = false;
    //Anim
    //CharAnim charAnim;
    protected Animator anim;

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
    protected float skillTime;
    protected float maxSkillTime = 20;
    public GameObject skillLearnEffect;

    #region Load

    void Awake()
    {
        anim = this.GetComponent<Animator>();
        charInteract = this.GetComponent<CharInteract>();
        if(touchObject != null)
            touchObject.SetActive(false);
        if(skillLearnEffect != null)
            skillLearnEffect.SetActive(false);
    }
    // Use this for initialization
    void Start()
    {
        data.Init();
        anim.Play("Idle_" + direction.ToString(), 0);
    }
    #endregion

    #region Update

    // Update is called once per frame
    void FixedUpdate()
    {

        //Debug.Log(actionType.ToString() + "  " + charInteract.interactType.ToString());
        if (actionType == ActionType.None)
        {
            Think();
            DoAction();
        }


        CalculateDirection();

        this.transform.position = agent.transform.position;

        //Calculate Attribue Data
        if (dataTime > maxDataTime)
        {
            CalculateData();
            dataTime = 0;
        }
        else
            dataTime += Time.deltaTime;

        if(currentSkill != SkillType.NONE){
            if(skillTime > maxSkillTime){
                OffLearnSkill();
            }else
                skillTime += Time.deltaTime;
        }
    }
    #endregion

    #region Data
    protected virtual void CalculateData()
    {

    }

    protected virtual void CalculateDirection(){

    }

    #endregion


    #region Interact
    public virtual void OnCall()
    {
        if (actionType == ActionType.Hold || actionType == ActionType.Call || actionType == ActionType.Sick || actionType == ActionType.Sleep)
        {
            return;
        }
        Abort();
        actionType = ActionType.Call;
        touchObject.SetActive(true);
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

    public virtual void OnHold()
    {
        if(actionType == ActionType.SkillUp)
            return;

        Abort();
        actionType = ActionType.Hold;
    }

    public virtual void OnEat(){
        
    }

    protected void OnBath()
    {
        Abort();
        SetDirection(Direction.D);
        SetDirection(Direction.D);
        actionType = ActionType.Bath;
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

    #endregion
    #region Thinking
    protected virtual void Think()
    {

    }

    protected virtual void DoAction(){

    }
    #endregion

    #region Basic Action

    //Basic Action
    protected void Abort()
    {
        anim.speed = 1;
        isAbort = true;
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

    protected void CheckAbort()
    {
        if (!isAbort)
            actionType = ActionType.None;
        else
            DoAction();
    }

    #endregion

    #region Skill

    public void OnLearnSkill(SkillType type){
        currentSkill = type;
        skillTime = 0;
        skillLearnEffect.SetActive(true);
    }

    public void OffLearnSkill(){
        skillTime = 0;
        currentSkill = SkillType.NONE;
        skillLearnEffect.SetActive(false);
    }
    public void LevelUpSkill(SkillType type){
        OffLearnSkill();
        data.LevelUpSkill(type);
        UIManager.instance.OnNotify(NotificationType.Skill);
        Abort();
        actionType = ActionType.SkillUp;
    }

    protected IEnumerator SkillUp(){
        yield return StartCoroutine(DoAnim("JumpRound_SkillUp_D"));
        CheckAbort();
    }


    #endregion

    #region Effect
    protected void SpawnPee()
    {
        GameObject go = GameObject.Instantiate(peePrefab, peePosition.position + new Vector3(0, 0, 50), Quaternion.identity);
    }

    protected void SpawnShit()
    {
        GameObject go = GameObject.Instantiate(shitPrefab, shitPosition.position, Quaternion.identity);
    }

    protected void SpawnFly(){
        //GameObject go = GameObject.Instantiate(flyPrefab,Vector3.zero, Quaternion.identity); 
    }


    #endregion

}



