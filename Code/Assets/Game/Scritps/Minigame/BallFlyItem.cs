using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFlyItem : MonoBehaviour
{
    public float speedX = 0;
    float speedY = 0;
    float maxY = 4;
    Minigame4 minigame;
    public int point = 0;
    Animator animator;
    bool isExplode = false;
    float time = 0;
    public BallFlyItemEffect effect = BallFlyItemEffect.None;
    public GameObject explosionEffect;
    CircleCollider2D circleCollider2D;
    // Start is called before the first frame update
    void Awake()
    {
        circleCollider2D = this.GetComponent<CircleCollider2D>();
        animator = this.GetComponent<Animator>();
        minigame = GameObject.FindObjectOfType<Minigame4>();
    }

    public void Load(float s,float y)
    {
        maxY = y;
        speedY = s;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isExplode)
        {
            time += Time.deltaTime;
            Vector3 v = new Vector3(speedX * Mathf.Sin(time), speedY, 0);
            this.transform.position += v * Time.deltaTime;
            if (this.transform.position.y > maxY)
            {
                minigame.EndGame();
                minigame.RemoveBall(this);
            }
        }

            
    }

    IEnumerator ExplodeCoroutine()
    {
        isExplode = true;
        circleCollider2D.enabled = false;
        if (explosionEffect != null)
            GameObject.Instantiate(explosionEffect, this.transform.position, Quaternion.identity);
        MageManager.instance.PlaySound("punch_low_deep_impact_02", false);
        animator.Play("Explode", 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        minigame.bonus += 1;
        if (this.effect == BallFlyItemEffect.Explode)
            minigame.OnExplode();
        else if(this.effect == BallFlyItemEffect.Speed)
        {
            MageManager.instance.PlaySound("Item_Appear", false);
            minigame.OnReduceSpeed();
            minigame.RemoveBall(this);
        }
        else if(this.effect == BallFlyItemEffect.Coin)
        {
            MageManager.instance.PlaySound("collect_item_02", false);
            minigame.bonus += 9;
            minigame.SpawnCoin(this.transform.position, point);
            minigame.RemoveBall(this);
        }
        else
            minigame.RemoveBall(this);

       

    }

    public void Explode()
    {
        if (isExplode)
            return;
        StartCoroutine(ExplodeCoroutine());
    }

    public void ReduceSpeed()
    {
        speedX = speedX / 5;
        speedY = speedY / 5;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<ArrowItem>() != null)
        {
            Explode();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

    }
}

public enum BallFlyItemEffect {None,Explode,Speed,Coin};
