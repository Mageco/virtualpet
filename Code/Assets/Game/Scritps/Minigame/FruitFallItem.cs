using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitFallItem : MonoBehaviour
{
    public int point = 1;
    public BallFlyItemEffect effect = BallFlyItemEffect.None;
    public GameObject explosionEffect;
    // Start is called before the first frame update
    void Awake()
    {
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



    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<BoarFruitGame>() != null)
        {
            if (effect == BallFlyItemEffect.Speed)
            {
                other.GetComponent<BoarFruitGame>().OnIncreaseSpeed();
            }else if(effect == BallFlyItemEffect.Explode){
                other.GetComponent<BoarFruitGame>().OnLose();
                Minigame.instance.live = 0;
            }
            other.GetComponent<BoarFruitGame>().Eat();
            Eat();
        }else if(other.tag == "Floor")
        {
            GameObject.FindObjectOfType<BoarFruitGame>().ResetSpeed();
            Minigame.instance.live--;
            GameObject.Destroy(this.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

    }
}

