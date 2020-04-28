using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitFallItem : MonoBehaviour
{
    public int point = 1;
    public BallFlyItemEffect effect = BallFlyItemEffect.None;
    public GameObject explosionEffect;
    Rigidbody2D rigid;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Eat()
    {
        Minigame.instance.bonus += point;
        GameObject.Destroy(this.gameObject);
    }

    public void Load(float gravity)
    {
        rigid.gravityScale = gravity;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<BoarFruitGame>() != null)
        {
            if (effect == BallFlyItemEffect.Speed)
            {
                Eat();
                other.GetComponent<BoarFruitGame>().OnIncreaseSpeed();
            }else if(effect == BallFlyItemEffect.Explode){
                other.GetComponent<BoarFruitGame>().OnLose();
                Minigame.instance.live = 0;
            }else
            {
                other.GetComponent<BoarFruitGame>().Eat();
                Eat();
            }
                
            
        }else if(other.tag == "Floor")
        {
            if(effect != BallFlyItemEffect.Explode && effect != BallFlyItemEffect.Speed)
            {
                GameObject.FindObjectOfType<BoarFruitGame>().ResetSpeed();
                Minigame.instance.live--;
                GameObject.Destroy(this.gameObject);
            }

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

    }
}

