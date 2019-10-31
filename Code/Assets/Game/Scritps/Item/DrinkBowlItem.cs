using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkBowlItem : MonoBehaviour
{
	float foodAmount;
	public float maxfoodAmount = 200;
	public SpriteRenderer image;
	public Sprite[] foodSprites;
	public Transform anchor;

	float time;
	float maxDoubleClickTime = 0.4f;
	bool isClick = false;

    // Start is called before the first frame update
    void Start()
    {
		foodAmount = maxfoodAmount - 1;   
    }

    // Update is called once per frame
    void Update()
    {
		int id = (int)(foodAmount/(maxfoodAmount/foodSprites.Length));
		image.sprite = foodSprites [id];
		
    }

	public void Eat(float f)
	{
		foodAmount -= f;
		if (foodAmount < 0)
			foodAmount = 0;
	}

	public bool CanEat()
	{
		if (foodAmount <= 0)
			return false;
		else
			return true;
	}

	void OnMouseUp()
	{
		if (isClick) {
			if (time > maxDoubleClickTime) {
				time = 0;
			} else {
				Fill ();
				time = 0;
				isClick = false;
				return;
			}
		} else {
			time = 0;
			isClick = true;
		}
	}

	void Fill()
	{
		foodAmount = maxfoodAmount-1;
	}
}
