using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnim : MonoBehaviour
{
	public static CharAnim instance;
	CharController character;
	Animator anim;
	string lastAnim = "";
	string currentAnim = "";
	AnimType animType = AnimType.Idle;
	AnimType lastAnimType = AnimType.Idle;

    void Awake()
	{
		if (instance == null)
			instance = this;
		character = this.GetComponent <CharController> ();
		anim = this.GetComponent <Animator> ();
	}

	void Update()
	{
		
	}

	public AnimType GetAnimType()
	{
		return animType;
	}

	public void SetAnimType(AnimType type)
	{
		lastAnim = currentAnim;
		lastAnimType = animType;
		animType = type;
		Direction d = character.direction;
		switch (type)
		{
		case AnimType.Idle:
			currentAnim = "Idle_" + character.direction.ToString ();
			break;	
		case AnimType.Sit:
			currentAnim = "Idle_Sit_D";
			break;	
		case AnimType.Lay:
			if (character.direction == Direction.RD || character.direction == Direction.RU)
				currentAnim = "Lay_RD";
			else
				currentAnim = "Lay_LD";
			break;	
		case AnimType.Sleep:
			currentAnim = "Sleep_LD";
			break;	
		case AnimType.Eat:
			currentAnim = "Eat_LD";
			break;	
		case AnimType.Drink:
			currentAnim = "Eat_LD";
			break;	
		case AnimType.Crawl:
			break;	
		case AnimType.Walk:
			break;	
		case AnimType.Run:
			currentAnim = "Run_" + character.direction.ToString ();
			break;	
		case AnimType.Jump:
			break;	
		case AnimType.Burping:
			break;	
		case AnimType.Pee:
			currentAnim = "Pee_D";
			break;	
		case AnimType.Shit:
			currentAnim = "Poop_D";
			break;	
		case AnimType.Itchi:
			if (character.direction == Direction.RD || character.direction == Direction.RU)
				currentAnim = "Itchy_RD";
			else
				currentAnim = "Itchy_LD";
			break;	
		case AnimType.Sick:
			if (character.direction == Direction.RD || character.direction == Direction.RU)
				currentAnim = "Sick_RD";
			else
				currentAnim = "Sick_LD";
			break;	
		case AnimType.Bath:
			currentAnim = "BathStart_D";
			break;	
		case AnimType.Soap:
			currentAnim = "Soap_D";
			break;	
		case AnimType.Shower: 
			currentAnim = "Shower_LD";
			break;	
		case AnimType.Shake:
			currentAnim = "Shake_D";
			break;	
		case AnimType.Touch:
			if (lastAnimType == AnimType.Happy) {
				if (character.direction == Direction.RD || character.direction == Direction.RU)
					currentAnim = "Scratch_RD";
				else
					currentAnim = "Scratch_LD";
			} else {
				if (character.touchDirection == Direction.D)
					currentAnim = "Sit_Pet_Down_D";
				else if(character.touchDirection == Direction.L)
					currentAnim = "Sit_Pet_Left_D";
				else if(character.touchDirection == Direction.R)
					currentAnim = "Sit_Pet_Right_D";
				else if(character.touchDirection == Direction.U)
					currentAnim = "Sit_Pet_Up_D";
				
			}
			break;	
		case AnimType.Bark:
			if(lastAnimType == AnimType.Sit)
				currentAnim = "Bark_Sit_D";
			else
				currentAnim = "Bark_D";
			break;	
		case AnimType.Hold:
			currentAnim = "Hold_D";
			break;	
		case AnimType.Fall_Light:
			currentAnim = "Drop_Light_D";
			break;	
		case AnimType.Fall:
			currentAnim = "Drop_Hard_D";
			break;	
		case AnimType.Smell:
			currentAnim = "Smell_" + character.direction.ToString ();	
			break;	
		case AnimType.Tired:
			if (character.direction == Direction.RD || character.direction == Direction.RU)
				currentAnim = "Lay_RD";
			else
				currentAnim = "Lay_LD";
			break;
		case AnimType.Sad:
			break;
		case AnimType.Happy:
			if (character.direction == Direction.RD || character.direction == Direction.RU)
				currentAnim = "ScratchWait_RD";
			else
				currentAnim = "ScratchWait_LD";
			break;	
		case AnimType.Listening:
			if (character.direction == Direction.RD || character.direction == Direction.RU)
				currentAnim = "Idle_LookDown_RD";
			else
				currentAnim = "Idle_LookDown_LD";
			break;	
		}

		anim.Play (currentAnim, 0);
	}

}

public enum AnimType {Idle,Sit,Lay,Sleep,Eat,Drink,Crawl,Walk,Run,Jump,Burping,Pee,Fart,Shit,Itchi,Sick,Bath,Soap,Shower,Shake,Touch,Bark,Hold,Fall_Light,Fall,Smell,Tired,Sad,Happy,Listening};
