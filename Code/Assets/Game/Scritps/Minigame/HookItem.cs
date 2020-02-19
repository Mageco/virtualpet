using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class HookItem : MonoBehaviour
{

    public Vector3 target;
    Vector3 originalPosition;
    Quaternion originalRotation;
    Quaternion targetRotation;
    Vector3 velocity;
    public HookState state = HookState.Idle;
    public float speedThrow;
    List<FishController> fishes = new List<FishController>();
    public float speedDraw = 1;
    float speed = 0;
    float minWeight = 5;
    public Animator cat;
    public Animator turtle;
    public Animator parrot;
    float time;
    public GameObject body;
    public Transform hookAnchor;
    public Transform rodAnchor;
    public LineRenderer line;
    public GameObject timeGainPrefab;
    public GameObject[] bonuses;
    

    private void Awake()
    {
        originalPosition = this.transform.position;
        originalRotation = this.transform.rotation;
        line.startWidth = 0.07f;
        line.endWidth = 0.07f;
    }

    // Start is called before the first frame update
    void Start()
    {
        cat.Play("Idle", 0);
        this.body.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(state == HookState.Idle)
        {
            
        }
        else if (state == HookState.Active)
        {
            this.transform.position += velocity * Time.deltaTime;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * 20);

            if (!CheckPositionOutBound())
            {
                OnCatched();
            }

        }else if(state == HookState.Catched)
        {
            velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * 3);
            this.transform.position += velocity * Time.deltaTime;
            if(velocity.magnitude < 1f)
            {
                OnDraw();
            }
        }
        else if(state == HookState.Draw)
        {
            
            this.transform.position = Vector3.Lerp(this.transform.position, originalPosition, Time.deltaTime * speed);
            if(Vector2.Distance(this.transform.position,originalPosition) < 1)
            {
                OnHookOff();
            }
        }else if(state == HookState.Shock)
        {
            time += Time.deltaTime;
            if(time > 1f)
            {
                OnIdle();
            }
        }

        line.SetPosition(0, transform.position);
        line.SetPosition(1, rodAnchor.position);
    }

    public void OnHookOff()
    {
        state = HookState.Idle;
        this.transform.rotation = originalRotation;
        bool isShock = false;


        if(Minigame.instance.time > Minigame.instance.maxTime)
        {
            foreach (FishController fish in fishes)
            {
                fish.OnActive();
            }
            fishes.Clear();
            return;
        }

        foreach (FishController fish in fishes)
        {
            if(fish.fishType == FishType.JellyFish)
            {
                MageManager.instance.PlaySoundName("Jellyfish", false);
                isShock = true;
            }else if(fish.fishType == FishType.Squirt)
            {
                MageManager.instance.PlaySoundName("whoosh_swish_med_03", false);
                Minigame.instance.time -= 5;
                GameObject go = GameObject.Instantiate(timeGainPrefab);
                go.transform.position = fish.transform.position + new Vector3(0, -0.5f, -10);
            }
        }

        if (!isShock)
        {
            int number = 0;
            int count = 0;
            foreach (FishController fish in fishes)
            {
                if(fish.fishType == FishType.Fish)
                {
                    count++;
                    number++;
                }
                    
                else if(fish.fishType == FishType.YellowFish)
                {
                    count += 5;
                    number++;
                }
                else if(fish.fishType == FishType.SpecialFish)
                {
                    count += 10;
                    number++;
                }
                fish.OnDeactive();
            }

            int value = count;
            if (number == 1)
            {
                MageManager.instance.PlaySoundName("points_ticker_bonus_score_reward_single_01", false);
                value += 1;
                GameObject bonus = Instantiate(bonuses[0]);
                bonus.transform.position = hookAnchor.position + new Vector3(0, 0, -10);
            }
            else if (number == 2)
            {
                MageManager.instance.PlaySoundName("points_ticker_bonus_score_reward_single_02", false);
                value += 2;
                GameObject bonus = Instantiate(bonuses[1]);
                bonus.transform.position = hookAnchor.position + new Vector3(0, 0, -10);
            }
            else if (number == 3)
            {
                MageManager.instance.PlaySoundName("points_ticker_bonus_score_reward_single_04", false);
                value += 5;
                GameObject bonus = Instantiate(bonuses[2]);
                bonus.transform.position = hookAnchor.position + new Vector3(0, 0, -10);
            }
            else if (number > 3)
            {
                MageManager.instance.PlaySoundName("points_ticker_bonus_score_reward_single_06", false);
                value += 10;
                GameObject bonus = Instantiate(bonuses[3]);
                bonus.transform.position = hookAnchor.position + new Vector3(0, 0, -10);
            }

            if (count > 0)
            {
                cat.Play("Happy", 0);
                turtle.Play("Happy", 0);
                parrot.Play("Happy", 0);
            }
            else
            {
                cat.Play("Idle", 0);
                turtle.Play("Idle", 0);
                parrot.Play("Idle", 0);
            }

            if(value > 0)
            {
                Minigame.instance.SpawnCoin(this.transform.position + new Vector3(0, 0, -10), value);
                //GameManager.instance.AddCoin(value);
                Minigame.instance.bonus += value;
            }

            Minigame.instance.bonus += count;
            //GameManager.instance.AddCoin(count);
            OnIdle();
        }
        else
        {
            OnShock();
            foreach (FishController fish in fishes)
            {
                fish.OnActive();
            }
        }

        fishes.Clear();
        Minigame.instance.UpdateLive();
    }

    void OnIdle()
    {
        state = HookState.Idle;
        body.SetActive(false);
    }

    void OnShock()
    {
        body.SetActive(false);
        time = 0;
        state = HookState.Shock;
        cat.Play("Shock", 0);
        turtle.Play("Shock", 0);
        parrot.Play("Shock", 0);
    }

    public void OnTap(Vector3 pos)
    {
        if(state == HookState.Idle)
        {
            if(pos.y < 1.5f)
                StartCoroutine(OnThrow(pos));
        }
    }

    IEnumerator OnThrow(Vector3 pos)
    {
        MageManager.instance.PlaySoundName("Drag", false);
        
        
        target = pos;
        state = HookState.Active;
        cat.Play("Throw", 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(cat.GetCurrentAnimatorStateInfo(0).length);
        cat.Play("Throw_Idle", 0);
        MageManager.instance.PlaySoundName("Water_Sound01", false);
        yield return new WaitForEndOfFrame();
        body.SetActive(true);
        this.transform.position = hookAnchor.position;
        originalPosition = this.transform.position;
        Debug.Log(target);
        velocity = (target - this.transform.position).normalized;
        velocity.z = 0;
        this.velocity = speedThrow * SpeedRate() * velocity;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90;
        targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }



    public void OnCatched()
    {
        state = HookState.Catched;
    }

    public void OnDraw()
    {
        state = HookState.Draw;
        this.speed = speedDraw * SpeedRate();
        Debug.Log(speed);
    }

    bool CheckPositionOutBound()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(this.transform.position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(state == HookState.Active || state == HookState.Catched || state == HookState.Draw)
        {
            if (other.GetComponent<FishController>() != null)
            {
                FishController fish = other.GetComponent<FishController>();
                if (fish.state != FishState.Cached)
                {
                    this.OnCatched();
                    fish.transform.parent = this.transform;
                    fish.OnCached();
                    fishes.Add(fish);
                }
            }
        }
    }


    float SpeedRate()
    {
        float weight = 0;
        foreach(FishController fish in fishes)
        {
            weight += fish.weight;
        }
        return 10f/(minWeight + weight);
    }
}

public enum HookState {Idle,Active,Catched,Draw,DeActive,Shock}
