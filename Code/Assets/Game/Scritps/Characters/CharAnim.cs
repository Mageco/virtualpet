using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnim : MonoBehaviour
{
	public static CharAnim instance;
	CharController character;
	Animator anim;
	string lastAnim = "";
	string curretAnim = "";
	AnimType animType = AnimType.Idle;

    void Awake()
	{
		if (instance == null)
			instance = this;
		character = this.GetComponent <CharController> ();
		anim = this.GetComponent <Animator> ();
	}

	void Update()
	{
		anim.Play (curretAnim, 0);
	}

	public void SetAnimType(AnimType type)
	{
		Direction d = character.direction;
		switch (AnimType)
		{
						
			default:
		}


	}

}

public enum AnimType {Idle,Rest,Sit,Lay,Sleep,Eat,Drink,Crawl,Walk,Run,Jump,Burping,Pee,Fart,Shit,Itchi,Sick,Bath,Soap,Shower,Shake,Caress,Bark,Hold,Fall,Smell,Tired,Sad,Happy};
