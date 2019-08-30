using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBBowlItem : MonoBehaviour
{
	float foodAmount;
	public float maxfoodAmount = 200;
	public SpriteRenderer image;
	public Sprite[] foodSprites;

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
		Fill ();
	}

	void Fill()
	{
		foodAmount = maxfoodAmount-1;
	}
}
