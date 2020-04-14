using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowItem : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    BoxCollider2D boxCollider2D;
    Vector3 lastPosition;
    int number = 0;
    int value = 0;
    public GameObject[] bonuses;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dir = rigidbody2D.velocity;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rigidbody2D.MoveRotation(angle);
        float minY = Camera.main.transform.position.y - Camera.main.orthographicSize + 0.5f;
        if (this.transform.position.x > 16 || this.transform.position.y < minY)
        {
            if (number == 1)
            {
                value = 0;
                GameObject bonus = Instantiate(bonuses[0]);
                bonus.transform.position = Vector3.zero;
            }
            else if (number == 2)
            {
                MageManager.instance.PlaySound("points_ticker_bonus_score_reward_single_02", false);
                value = 3;
                GameObject bonus = Instantiate(bonuses[1]);
                bonus.transform.position = Vector3.zero;
            }
            else if (number == 3)
            {
                MageManager.instance.PlaySound("points_ticker_bonus_score_reward_single_04", false);
                value = 10;
                GameObject bonus = Instantiate(bonuses[2]);
                bonus.transform.position = Vector3.zero;
            }
            else if (number > 3)
            {
                MageManager.instance.PlaySound("points_ticker_bonus_score_reward_single_06", false);
                value = 20;
                GameObject bonus = Instantiate(bonuses[3]);
                bonus.transform.position = Vector3.zero;
            }
            Minigame.instance.bonus += value;
            Destroy(this.gameObject);
        }
            
    }

    public void Load(float speed,float angle)
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        rigidbody2D.velocity = new Vector2(speed * Mathf.Cos(angle),speed * Mathf.Sin(angle));
        lastPosition = this.transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BallFlyItem>() != null)
        {
            number++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

    }
}
