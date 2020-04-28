using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarFruitGame : MonoBehaviour
{
    public float originalSpeed = 10;
    float speed = 0;
    float time = 0;
    public AnimalState state = AnimalState.Idle;
    public Animator animator;
    Direction direction = Direction.R;
    public bool isPower = false;
    public bool isMove = false;
    public bool isEat = false;
    

    private void Awake()
    {
        speed = originalSpeed;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(state == AnimalState.Idle)
        {
            animator.Play("Idle_" + direction.ToString(), 0);
            
        }
        else if(state == AnimalState.Run)
        {
            
            if (isPower)
                animator.Play("Run_PowerUp_" + direction.ToString(), 0);
            else
                animator.Play("Run_" + direction.ToString(), 0);

            if(direction == Direction.R)
            {
                animator.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
            }else
                animator.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);

            Vector3 pos = animator.transform.position;
            if(pos.x > 9)
            {
                pos.x = 9;
            }

            if(pos.x < -9)
            {
                pos.x = -9;
            }

            animator.transform.position = pos;
        }


        if(state != AnimalState.Cached)
        {

            if (isEat)
            {
                animator.Play("Head_Eat", 1);
            }
            else if (isPower)
            {
                animator.Play("Head_PowerUp", 1);
            }

            else if (isMove)
            {
                animator.Play("Head_Run", 1);
            }
            else
                animator.Play("Head_Idle", 1);
        }


    }

    public void OnIncreaseSpeed()
    {
        isPower = true; 
        speed = originalSpeed * 1.5f;
    }

    public void ResetSpeed()
    {

        isPower = false;
        speed = originalSpeed;
    }

    public void Eat()
    {
        isEat = true;
        Invoke("OffEat", 1f);
    }

    void OffEat()
    {
        isEat = false;
    }

    public void OnLose()
    {
        state = AnimalState.Cached;
        animator.Play("Head_Fail", 1);
        animator.Play("Fail_" + direction.ToString(), 0);
        StartCoroutine(LoseCoroutine());
    }

    IEnumerator LoseCoroutine()
    {
        yield return new WaitForSeconds(1);
        Minigame.instance.EndGame();
    }

    public void OnRunLeft()
    {
        Debug.Log("IsPointLeftDown");
        if (state != AnimalState.Cached)
        {
            isMove = true;
            direction = Direction.L;
            state = AnimalState.Run;
        }

    }

    public void OnRunRight()
    {
        if(state != AnimalState.Cached)
        {
            isMove = true;
            direction = Direction.R;
            state = AnimalState.Run;
        }
            
    }

    public void OnStop()
    {
        isMove = false;
        if (state == AnimalState.Run)
            state = AnimalState.Idle;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<FruitFallItem>() != null)
        {
            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

    }
}


