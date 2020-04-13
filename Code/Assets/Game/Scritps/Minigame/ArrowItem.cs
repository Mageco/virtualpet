using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowItem : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    BoxCollider2D boxCollider2D;
    Vector3 lastPosition;

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
        if (this.transform.position.y < -20)
            Destroy(this.gameObject);
    }

    public void Load(float speed,float angle)
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        rigidbody2D.velocity = new Vector2(speed * Mathf.Cos(angle),speed * Mathf.Sin(angle));
        lastPosition = this.transform.position;
    }
}
