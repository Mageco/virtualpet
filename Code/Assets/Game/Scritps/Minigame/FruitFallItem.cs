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
                MageManager.instance.PlaySound("Collect_Item", false);        
                other.GetComponent<BoarFruitGame>().OnIncreaseSpeed();
            }else if(effect == BallFlyItemEffect.Explode){
                MageManager.instance.PlaySound("Fox_Hurt", false);
                other.GetComponent<BoarFruitGame>().OnLose();
                Minigame.instance.live = 0;
            }else
            {
                MageManager.instance.PlaySound("collect_item_02", false);
                other.GetComponent<BoarFruitGame>().Eat();
                Minigame.instance.SpawnCoin(this.transform.position, point);
                Minigame.instance.bonus += point;
            }
            GameObject.Destroy(this.gameObject);
        }
        else if(other.tag == "Floor")
        {
            if(effect != BallFlyItemEffect.Explode && effect != BallFlyItemEffect.Speed)
            {
                MageManager.instance.PlaySound("Item_Dissapear", false);
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

