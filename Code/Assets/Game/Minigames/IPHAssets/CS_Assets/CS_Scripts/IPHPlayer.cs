﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace InfiniteHopper
{
	/// <summary>
	/// This script defines the player, its movement, as well as its death
	/// </summary>
	public class IPHPlayer:MonoBehaviour 
	{
		internal Transform thisTransform;
		Vector3 originalScale;
		internal GameObject gameController;
		Animator animator;
		
		//How fast the player's jump power increases when we are holding the jump button
		public float jumpChargeSpeed = 30;
		
		//The current jump power of the player
		internal float jumpPower = 2;
		
		//The maximum jump power of the player
		public float jumpChargeMax = 20;

		//Should the player automatically jump when reaching the maximum power?
		internal bool autoJump = true;

		//A power bar that shows how high the player will jump.
		public Transform powerBar;
		
		//The *horizontal* movement speed of the player when it is jumping
		public float moveSpeed = 4;
		
		//The particle effects that will play when the player jumps and lands on a column
		public ParticleSystem jumpEffect;
		public ParticleSystem landEffect;
		public ParticleSystem perfectEffect;
		
		//Various animations for the player

		//Various sounds and their source
		public AudioClip soundStartJump;
		public AudioClip soundEndJump;
		public AudioClip soundLand;
		public AudioClip soundCrash;
		public AudioClip soundPerfect;
		
		//Did the player start the jump process ( powering up and then releasing )
		internal bool  startJump = false;
		
		//Is the player jumping now ( is in mid-air )
		internal bool  isJumping = false;
		
		//Has the player landed on a column?
		internal bool  isLanded = false;

		// Is the player falling now?
		internal bool  isFalling = false;

		// Is the player dead?
		internal bool isDead = false;

        private void Awake()
        {
			animator = this.GetComponent<Animator>();
        }

        void  Start()
		{
			thisTransform = transform;
			originalScale = this.transform.localScale;
			//Assign the game controller for easier access
			gameController = GameObject.FindGameObjectWithTag("GameController");

			//Set the particle effects to the "RenderInFront" sorting layer so that the effects appear in front of the player object
			if ( jumpEffect )    jumpEffect.GetComponent<Renderer>().sortingLayerName = "RenderInFront";
			if ( landEffect )    landEffect.GetComponent<Renderer>().sortingLayerName = "RenderInFront";
			if ( perfectEffect )    perfectEffect.GetComponent<Renderer>().sortingLayerName = "RenderInFront";
			
		}
		
		void  Update()
		{
			if ( isDead == false )
			{
				//If we are starting to jump, charge up the jump power as long as we are holding the jump button down
				if ( startJump == true )
				{
					//Charge up the jump power
					if ( jumpPower < jumpChargeMax )
					{
						//Add to the jump power based on charge speed
						jumpPower += Time.deltaTime * jumpChargeSpeed;

						//Update the power bar fill amount
						powerBar.Find("Base/FillAmount").GetComponent<Image>().fillAmount = jumpPower/jumpChargeMax;
						
						//Play the charge sound and change the pitch based on the jump power

						//if ( soundSource )    soundSource.GetComponent<AudioSource>().pitch = 0.3f + jumpPower * 0.1f;
					}
					else if ( autoJump == true )
					{
						//If the jump power exceeds the maximum allowed jump power, end charging, and launch the player
						EndJump();
					}
					else
					{
						animator.Play("FullPower", 0);
					}
				}
				
				//If the player is moving down, then he is falling
				if ( isFalling == false && GetComponent<Rigidbody2D>().velocity.y < 0 )
				{
					isFalling = true;
					animator.Play("Falling", 0);
				}
			}
		}
		
		//This function adds score to the gamecontroller
		void  ChangeScore(Transform landedObject)
		{
			gameController.SendMessage("ChangeScore", landedObject);
		}
		
		//This function destroys the player, and triggers the game over event
		void  Die()
		{
			if ( isDead == false )
			{
				//Call the game over function from the game controller
				gameController.SendMessage("GameOver", 0.5f);
				
				//Play the death sound
				MageManager.instance.PlaySoundClip(soundCrash);
				
				// The player is dead
				isDead = true;
			}
		}

		// This function resets the player's dead status
		public void NotDead()
		{
			isDead = false;
		}
		
		//This function starts the jumping process, allowing the player to charge up the jump power as long as he is holding the jump button down
		void  StartJump( bool playerAutoJump )
		{
			if ( isDead == false )
			{
				//Set the player auto jump state based on the GameController playerAutoJump value
				autoJump = playerAutoJump;

				//You can only jump if you are on land
				if ( isLanded == true )
				{	
					startJump = true;
					
					//Reset the jump power
					jumpPower = 0;

					animator.Play("JumpStart", 0);

					//Align the power bar to the player and activate it
					if ( powerBar )
					{
						powerBar.position = thisTransform.position;

						powerBar.gameObject.SetActive(true);
					}

					MageManager.instance.PlaySoundClip(soundStartJump);
				}
			}
		}
		
		//This function ends the jump process, and launches the player with the jump power we charged
		void  EndJump()
		{
			if ( isDead == false )
			{
				//You can only jump if you are on land, and you already charged up the jump power ( jump start )
				if ( isLanded == true && startJump == true )
				{
					thisTransform.parent = null;

					startJump = false;
					isJumping = true;
					isLanded = false;
					isFalling = false;
					
					//Give the player velocity based on jump power and move speed
					GetComponent<Rigidbody2D>().velocity = new Vector2( moveSpeed, jumpPower);

					//Play the jump ( launch ) animation
					animator.Play("JumpEnd", 0);
                    

					//Deactivate the power bar
					if ( powerBar )    powerBar.gameObject.SetActive(false);

					//Play the jump particle effect
					if ( jumpEffect )   jumpEffect.Play(); 

					//Play the jump sound ( launch )
					MageManager.instance.PlaySoundClip(soundEndJump);

				}
			}
		}
		
		//This function runs when the player succesfully lands on a column
		void  PlayerLanded()
		{
			isLanded = true;

			//Play the landing animation
			animator.Play("Landed", 0);
			
			//Play the landing particle effect
			if ( landEffect )    landEffect.Play();

			//Play the landing sound
			MageManager.instance.PlaySoundClip(soundLand);

		}
		
		//This function runs when the player executes a perfect landing ( closest to the middle )
		void  PerfectLanding(int streak)
		{
			//Play the perfect landing particle effect
			if ( perfectEffect )    perfectEffect.Play();

			//If there is a sound source and a sound assigned, play it from the source
			MageManager.instance.PlaySoundClip(soundPerfect);
		}

		//This function rescales this object over time
		IEnumerator Rescale( float targetScale )
		{
			//Perform the scaling action for 1 second
			float scaleTime = 1;
			
			while ( scaleTime > 0 )
			{
				thisTransform.localScale = Vector3.Lerp(thisTransform.localScale, originalScale * targetScale, Time.deltaTime);
				scaleTime -= Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}
			
			//Rescale the object to the target scale instantly, so we make sure that we got the the target
			thisTransform.localScale = originalScale * targetScale;
		}
	}
}