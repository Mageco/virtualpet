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
    float dataTime = 0;
    float maxDataTime = 1f;

    //Movement
    public Transform target;

    [HideInInspector]
    PolyNavAgent agent;
    float agentTime = 0;
    float maxAgentTime = 0.1f;
    Vector2 lastTargetPosition;
    public bool isArrived = true;
    public bool isAbort = false;

    //Action
    public ActionType actionType = ActionType.None;
    //public bool isEndAction = false;
    //Anim
    //CharAnim charAnim;
    Animator anim;

    //Interact
    CharInteract charInteract;
    public bool isTouch = false;
    //Pee,Sheet
    public Transform peePosition;
    public Transform shitPosition;
    public GameObject peePrefab;
    public GameObject shitPrefab;

    public GameObject touchObject;
    #endregion

    #region Load

    void Awake()
    {
        anim = this.GetComponent<Animator>();
        //charAnim = this.GetComponent<CharAnim> ();
        agent = GameObject.FindObjectOfType<PolyNavAgent>();
        charInteract = this.GetComponent<CharInteract>();
        touchObject.SetActive(false);
    }
    // Use this for initialization
    void Start()
    {
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

        this.transform.position = agent.transform.position;

        //Calculate Attribue Data
        if (dataTime > maxDataTime)
        {
            CalculateData();
            dataTime = 0;
        }
        else
            dataTime += Time.deltaTime;
    }
    #endregion

    #region Data
    void CalculateData()
    {
        data.actionEnergyConsume = 0;


        if (actionType == ActionType.Call)
            data.actionEnergyConsume = 0.2f;
        else if (actionType == ActionType.Mouse)
            data.actionEnergyConsume = 0.5f;
        else if (actionType == ActionType.Discover)
        {
            data.actionEnergyConsume = 0.5f;
        }
        else if (actionType == ActionType.Patrol)
        {
            data.actionEnergyConsume = 0.3f;
        }

        data.Energy -= data.basicEnergyConsume + data.actionEnergyConsume;

        data.Happy -= data.happyConsume;
        if (actionType == ActionType.Call)
            data.Happy += 0.01f;

        if (data.Food > 0 && data.Water > 0)
        {
            float delta = 0.1f + data.Health * 0.001f + data.Happy * 0.001f;
            data.Food -= delta;
            data.Water -= delta;
            data.Energy += delta;
            data.Shit += delta;
            data.Pee += delta * 2;
        }

        data.Dirty += data.dirtyFactor;
        data.Itchi += data.Dirty * 0.001f;
        data.Stamina -= data.staminaConsume;
        data.Stamina += data.actionEnergyConsume;

        data.Sleep -= data.sleepConsume;

        float deltaHealth = data.healthConsume;

        deltaHealth += (data.Happy - data.maxHappy * 0.3f) * 0.001f;

        if (data.Dirty > data.maxDirty * 0.8f)
            deltaHealth -= (data.Dirty - data.maxDirty * 0.8f) * 0.003f;

        if (data.Pee > data.maxPee * 0.9f)
            deltaHealth -= (data.Pee - data.maxPee * 0.9f) * 0.001f;

        if (data.Shit > data.maxShit * 0.9f)
            deltaHealth -= (data.Shit - data.maxShit * 0.9f) * 0.002f;

        if (data.Food < data.maxFood * 0.1f)
            deltaHealth -= (data.maxFood * 0.1f - data.Food) * 0.001f;

        if (data.Water < data.maxWater * 0.1f)
            deltaHealth -= (data.maxWater * 0.1f - data.Water) * 0.001f;

        if (data.Sleep < data.maxSleep * 0.05f)
            deltaHealth -= (data.maxSleep * 0.05f - data.Sleep) * 0.004f;

        data.Health += deltaHealth;



    }


    #endregion


    #region Interact
    public void OnCall()
    {
        if (actionType == ActionType.Call || actionType == ActionType.Sick || actionType == ActionType.Sleep)
        {
            return;
        }
        Abort();
        actionType = ActionType.Call;
        touchObject.SetActive(true);
    }

    public void OnListening()
    {
        Abort();
        actionType = ActionType.Listening;
    }

    public void OnHold()
    {
        Abort();
        actionType = ActionType.Hold;
    }


    void OnBath()
    {
        Abort();
        actionType = ActionType.Bath;
    }

    void OnTable()
    {
        Abort();
        actionType = ActionType.OnTable;
    }

    public void OnSoap()
    {
        if (actionType == ActionType.Bath)
        {
            anim.Play("BathStart_D", 0);
        }
    }

    public void OnShower()
    {
        if (actionType == ActionType.Bath)
            anim.Play("Shower_LD", 0);
    }

    public void OffShower()
    {
        if (actionType == ActionType.Bath)
            anim.Play("BathStart_D", 0);
    }

    public void OnTouch()
    {
        if (actionType == ActionType.Call)
        {
            isTouch = true;
        }
    }

    public void OffTouch()
    {
        //if(actionType == ActionType.Call){
        isTouch = false;
        //}
    }

    public void OnMouse(Transform t)
    {
        if (actionType == ActionType.Hold || actionType == ActionType.OnTable || actionType == ActionType.Bath || actionType == ActionType.Sick
        || actionType == ActionType.Sleep || actionType == ActionType.Call)
            return;

        int n = 0;

        if (actionType == ActionType.Pee || actionType == ActionType.Shit)
        {
            n = Random.Range(50, 100);
        }

        if (actionType == ActionType.Drink || actionType == ActionType.Eat)
        {
            n = Random.Range(25, 100);
        }

        if (n < 75)
        {
            Abort();
            actionType = ActionType.Mouse;
            target = t;
        }

    }

    public void OffMouse()
    {
        if (actionType == ActionType.Mouse)
            actionType = ActionType.None;
    }

    public void OnArrived()
    {
        Debug.Log("Arrived");

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
    void Think()
    {

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

        if (data.Food < data.maxFood * 0.2f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Eat;
                return;
            }
        }

        if (data.Water < data.maxWater * 0.2f)
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

        if (data.Energy < data.maxEnergy * 0.1f)
        {
            actionType = ActionType.Rest;
            return;
        }

        if (data.Stamina < data.maxStamina * 0.3f)
        {
            actionType = ActionType.Discover;
            return;
        }




        //Other Action
        int id = Random.Range(0, 100);
        if (id < 40)
        {
            actionType = ActionType.Rest;
        }
        else if (id < 60)
        {
            actionType = ActionType.Patrol;
        }
        else
        {
            actionType = ActionType.Discover;
        }

    }


    void DoAction()
    {
        Debug.Log("DoAction " + actionType);
        isAbort = false;
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
        else if (actionType == ActionType.Bath)
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
        }
    }
    #endregion


    #region Basic Action
    //Basic Action
    public void Abort()
    {
        anim.speed = 1;
        isAbort = true;
        touchObject.SetActive(false);
    }

    void RandomDirection()
    {

    }

    void SetDirection(Direction d)
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

    IEnumerator DoAnim(string a)
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

    IEnumerator MoveToPoint()
    {
        isArrived = false;
        if (Vector2.Distance(target.position, agent.transform.position) > 0.5f)
        {
            lastTargetPosition = target.position;
            agent.SetDestination(target.position);
            while (!isArrived && !isAbort)
            {
                anim.Play("Run_" + this.direction.ToString(), 0);
                yield return new WaitForEndOfFrame();
            }
            if (isAbort)
                agent.Stop();
        }
        else
        {
            isArrived = true;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Wait(float maxT)
    {
        float time = 0;

        while (time < maxT && !isAbort)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region Main Action


    IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        while (!isAbort && n < maxCount)
        {
            if (!isAbort)
            {
                InputController.instance.SetTarget(PointType.Favourite);
                yield return StartCoroutine(MoveToPoint());
            }
            if (!isAbort)
            {
                int ran = Random.Range(0, 100);
                if (ran < 30)
                {
                    anim.Play("BathStart_D", 0);
                }
                else
                    anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 3)));
            }

            n++;
        }
        CheckAbort();
    }

    IEnumerator Bath()
    {

        anim.Play("BathStart_D", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Table()
    {
        anim.Play("BathStart_D", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Discover()
    {
        int n = Random.Range(0, 4);
        int maxCount = Random.Range(4, 10);
        while (!isAbort && n < maxCount)
        {
            yield return new WaitForEndOfFrame();

        }
        CheckAbort();
    }

    IEnumerator Hold()
    {
        charInteract.interactType = InteractType.Drag;
        enviromentType = EnviromentType.Room;
        Vector3 dropPosition = Vector3.zero;
        SetDirection(Direction.D);
        if (data.Health < data.maxHealth * 0.1f)
        {
            anim.Play("Hold_Sick_D", 0);
        }
        else
            anim.Play("Hold_D", 0);
        while (charInteract.interactType == InteractType.Drag)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - charInteract.dragOffset;
            pos.z = 0;
            if (pos.y > 20)
                pos.y = 20;
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
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + new Vector3(0, -2, 0), -Vector2.up, 100);
        Vector3 pos2 = this.transform.position;
        pos2.y = pos2.y - 22;
        if (pos2.y < -20)
            pos2.y = -20;
        dropPosition = pos2;
        enviromentType = EnviromentType.Room;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.tag == "Table")
            {
                pos2.y = hit[i].collider.transform.position.y;
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
        }

        float fallSpeed = 0;
        float maxTime = 1;
        while (charInteract.interactType == InteractType.Drop)
        {
            fallSpeed += 100f * Time.deltaTime;
            if (fallSpeed > 50)
                fallSpeed = 50;
            Vector3 pos1 = agent.transform.position;
            pos1.y -= fallSpeed * Time.deltaTime;
            pos1.z = dropPosition.z;
            agent.transform.position = pos1;


            if (Vector2.Distance(agent.transform.position, dropPosition) < fallSpeed * Time.deltaTime * 2)
            {
                this.transform.rotation = Quaternion.identity;

                if (data.Health < data.maxHealth * 0.1f)
                {
                    anim.Play("Drop_Sick_D", 0);
                    maxTime = 3;
                }
                else
                {
                    if (fallSpeed < 50)
                    {
                        anim.Play("Drop_Light_D", 0);
                        maxTime = 1;
                    }
                    else
                    {
                        anim.Play("Drop_Hard_D", 0);
                        maxTime = 3;
                    }
                }

                charInteract.interactType = InteractType.None;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return StartCoroutine(Wait(maxTime));
        if (data.Health < data.maxHealth * 0.1f)
        {
            actionType = ActionType.None;
        }
        else
        {
            if (enviromentType == EnviromentType.Bath)
            {
                OnBath();
            }
            else if (enviromentType == EnviromentType.Table)
            {
                OnTable();
            }
        }

        CheckAbort();
    }

    IEnumerator Mouse()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            lastTargetPosition = target.position;
            agent.speed = 40;
            anim.Play("Run_" + this.direction.ToString(), 0);
            anim.speed = 1.3f;
            yield return StartCoroutine(Wait(0.2f));
        }
        CheckAbort();
    }

    IEnumerator Call()
    {

        yield return StartCoroutine(DoAnim("BathStart_D"));

        if (!isAbort)
        {
            InputController.instance.SetTarget(PointType.Call);
            yield return StartCoroutine(MoveToPoint());
        }

        float t = 0;
        float maxTime = 3f;
        int n = Random.Range(0, 9);
        bool isWait = true;

        while (!isAbort && isWait)
        {
            if (!isTouch)
            {
                if (t == 0)
                {
                    n = Random.Range(0, 9);
                }
                if (n == 0)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Lay_WaitPetBack_RD", 0);
                    else
                        anim.Play("Lay_WaitPetBack_LD", 0);
                }
                else if (n == 1)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Lay_WaitPetHead_RD", 0);
                    else
                        anim.Play("Lay_WaitPetHead_LD", 0);
                }
                else if (n == 2)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Lay_WaitPetStomatch_RD", 0);
                    else
                        anim.Play("Lay_WaitPetStomatch_LD", 0);
                }
                else if (n == 3)
                {
                    anim.Play("Sit_WaitPetUp_D");
                }
                else if (n == 4)
                {
                    anim.Play("Sit_WaitPetDown_D");
                }
                else if (n == 5)
                {
                    anim.Play("Sit_WaitPetLeft_D");
                }
                else if (n == 6)
                {
                    anim.Play("Sit_WaitPetRight_D");
                }
                else if (n == 7)
                {
                    anim.Play("WaitHandshake_D");
                }
                else if (n == 8)
                {
                    anim.Play("WaitJumpRound_D");
                }
                t += Time.deltaTime;
                if (t > maxTime)
                {
                    isWait = false;
                    n = Random.Range(0, 9);
                    maxTime = Random.Range(2f, 5f);
                }
            }
            else
            {

                if (n == 0)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Lay_PetBack_RD", 0);
                    else
                        anim.Play("Lay_PetBack_LD", 0);

                }
                else if (n == 1)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Lay_PetHead_RD", 0);
                    else
                        anim.Play("Lay_PetHead_LD", 0);
                }
                else if (n == 2)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Lay_PetStomatch_RD", 0);
                    else
                        anim.Play("Lay_PetStomatch_LD", 0);
                }
                else if (n == 3)
                {
                    anim.Play("Sit_Pet_Up_D", 0);
                }
                else if (n == 4)
                {
                    anim.Play("Sit_Pet_Down_D", 0);
                }
                else if (n == 5)
                {
                    anim.Play("Sit_Pet_Right_D", 0);
                }
                else if (n == 6)
                {
                    anim.Play("Sit_Pet_Left_D", 0);
                }
                else if (n == 7)
                {
                    anim.Play("Handshake_D", 0);
                }
                else if (n == 8)
                {
                    anim.Play("JumpRound_D", 0);
                }
                t = 0;


            }
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }


    IEnumerator Pee()
    {
        anim.Play("Pee_D", 0);
        Debug.Log("Pee");
        SpawnPee();
        while (data.Pee > 1 && !isAbort)
        {
            data.Pee -= 0.5f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Shit()
    {
        anim.Play("Poop_D", 0);
        SpawnShit();
        while (data.Shit > 1 && !isAbort)
        {
            data.Shit -= 0.5f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Eat()
    {
        Debug.Log("Eat");
        if (!isAbort)
        {
            InputController.instance.SetTarget(PointType.Eat);
            yield return StartCoroutine(MoveToPoint());
        }
        bool canEat = true;
        if (ItemController.instance.foodBowl.CanEat() && !isAbort)
        {
            anim.Play("Eat_LD", 0);
            yield return StartCoroutine(Wait(0.1f));
            while (data.Food < data.maxFood && !isAbort && canEat)
            {
                data.Food += 0.3f;
                ItemController.instance.foodBowl.Eat(0.3f);
                if (!ItemController.instance.foodBowl.CanEat())
                {
                    canEat = false;
                }
                if (Vector2.Distance(this.transform.position, ItemController.instance.foodBowl.anchor.position) > 0.5f)
                    canEat = false;
                yield return new WaitForEndOfFrame();
            }
        }
        CheckAbort();
    }

    IEnumerator Drink()
    {

        Debug.Log("Drink");
        if (!isAbort)
        {
            InputController.instance.SetTarget(PointType.Drink);
            yield return StartCoroutine(MoveToPoint());
        }

        bool canDrink = true;

        if (ItemController.instance.waterBowl.CanEat())
        {
            anim.Play("Eat_LD", 0);
            yield return StartCoroutine(Wait(0.1f));
            while (data.Water < data.maxWater && !isAbort && canDrink)
            {
                data.Water += 0.5f;
                ItemController.instance.waterBowl.Eat(0.5f);
                if (!ItemController.instance.waterBowl.CanEat())
                {
                    canDrink = false;
                }
                if (Vector2.Distance(this.transform.position, ItemController.instance.waterBowl.anchor.position) > 0.5f)
                    canDrink = false;
                yield return new WaitForEndOfFrame();
            }
        }
        CheckAbort();
    }

    IEnumerator Sleep()
    {

        Debug.Log("Sleep");
        InputController.instance.SetTarget(PointType.Sleep);
        yield return StartCoroutine(MoveToPoint());
        if (!isAbort)
            anim.Play("Sleep_LD", 0);
        while (data.Sleep < data.maxSleep && !isAbort)
        {
            data.Sleep += 0.01f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Rest()
    {
        float maxTime = Random.Range(2, 5);
        if (!isAbort)
        {
            int ran = Random.Range(0, 100);
            if (ran < 20)
            {
                anim.Play("Idle_Sit_D", 0);
            }
            else if (ran < 40)
            {
                if (this.direction == Direction.RD || this.direction == Direction.RU)
                    anim.Play("Lay_RD", 0);
                else
                    anim.Play("Lay_LD", 0);
            }else if(ran < 60){
                    
            }
        }

        Debug.Log("Rest");
        yield return StartCoroutine(Wait(maxTime));
        CheckAbort();
    }

    IEnumerator Itchi()
    {

        int i = Random.Range(0, 3);

        if (i == 0)
        {
            if (this.direction == Direction.RD || this.direction == Direction.RU)
                anim.Play("Itchy_RD", 0);
            else
                anim.Play("Itchy_LD", 0);
        }
        else if (i == 1)
        {
            anim.Play("Idle_ShedHair_D", 0);
        }
        else
        {
            anim.Play("Shake_Hair_D", 0);
        }



        Debug.Log("Itchi");
        while (data.itchi > 0.5 * data.maxItchi && !isAbort)
        {
            data.itchi -= 0.1f;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        CheckAbort();
    }

    IEnumerator Sick()
    {
        if (this.direction == Direction.RD || this.direction == Direction.RU)
            anim.Play("Lay_Sick_RD", 0);
        else
            anim.Play("Lay_Sick_LD", 0);
        Debug.Log("Sick");
        while (data.health < 0.1f * data.maxHealth && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        CheckAbort();
    }

    IEnumerator Fear()
    {
        anim.Play("Itchy_RD", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Tired()
    {
        anim.Play("Itchy_RD", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Sad()
    {
        anim.Play("Itchy_RD", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Happy()
    {
        anim.Play("Itchy_RD", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    void CheckAbort()
    {
        if (!isAbort)
            actionType = ActionType.None;
        else
            DoAction();
    }

    #endregion

    #region Effect
    void SpawnPee()
    {
        GameObject go = GameObject.Instantiate(peePrefab, peePosition.position + new Vector3(0, 0, 50), Quaternion.identity);
    }

    void SpawnShit()
    {
        GameObject go = GameObject.Instantiate(shitPrefab, shitPosition.position, Quaternion.identity);
    }


    #endregion

}


public enum EnviromentType { Room, Table, Bath };
public enum ActionType { None, Mouse, Rest, Sleep, Eat, Drink, Patrol, Discover, Pee, Shit, Itchi, Sick, Sad, Fear, Happy, Tired, Call, Hold, OnTable, Bath, Listening }
