using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarFruitGame : MonoBehaviour
{
    public float originalSpeed = 10;
    public float speed = 0;
    float time = 0;
    AnimalState state = AnimalState.Idle;
    public Animator animator;
    Direction direction = Direction.R;
    bool isPower = true;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        speed = originalSpeed;
        animator.Play("Head_Idle", 1);
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
    }

    public void OnIncreaseSpeed()
    {
        isPower = true;
        animator.Play("Head_PowerUp", 1);
        speed = originalSpeed * 1.5f;
    }

    public void ResetSpeed()
    {

        isPower = false;
        speed = originalSpeed;
    }

    public void Eat()
    {
        animator.Play("Head_Eat", 1);
    }

    public void OnLose()
    {
        state = AnimalState.Cached;
        animator.Play("Head_Fail", 1);
        animator.Play("Fail_" + direction.ToString(), 0);
    }

    public void OnRunLeft()
    {
        if (state == AnimalState.Idle)
        {
            animator.Play("Head_Run", 1);
            direction = Direction.L;
            state = AnimalState.Run;
        }

    }

    public void OnRunRight()
    {
        if(state == AnimalState.Idle)
        {
            animator.Play("Head_Run", 1);
            direction = Direction.R;
            state = AnimalState.Run;
        }
            
    }

    public void OnStop()
    {
        animator.Play("Head_Idle", 1);
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

