using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetHappyItem : MonoBehaviour
{
    public int id = 0;
    [HideInInspector]
    public int value = 0;
    MSpriteButton button;
    Animator animator;

    private void Awake()
    {
        button = this.GetComponentInChildren<MSpriteButton>();
        animator = this.GetComponent<Animator>();
    }

    public void Load(int number)
    {
        value = number;
        int ran = Random.Range(0, 100);
        if (ran > 50)
        {
            animator.Play("Drop_L", 0);
        }
        else
            animator.Play("Drop_R", 0);
    }

    public void OnClick()
    {
        MageManager.instance.PlaySound3D("happy_collect_item_01", false, this.transform.position);
        ItemManager.instance.SpawnHeart(value, button.transform.position);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        Vector3 pos = this.transform.position;
        pos.z = button.transform.position.y * 10 - 10;
        this.transform.position = pos;
    }
}
