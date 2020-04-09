using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using InfiniteHopper.Types;

namespace InfiniteHopper
{
	/// <summary>
	/// This script defines an item which can be picked up. When picked up, it can run a function or update a PlayerPrefs value.
	/// </summary>
	public class IPHItem:MonoBehaviour 
	{
		Animator animator;
		CircleCollider2D col;
		//The tag of the object that can touch this item
		public string hitTargetTag = "Player";
		
		//A list of functions that run when this item is touched by the target
		public TouchFunction[] touchFunctions;
		
		//The sound that plays when this object is touched
		public AudioClip soundHit;
		public int coin = 0;

        private void Awake()
        {
			animator = this.GetComponent<Animator>();
			col = this.GetComponent<CircleCollider2D>();
        }

        //This function runs when this obstacle touches another object with a trigger collider
        void  OnTriggerEnter2D ( Collider2D other  ){	
			//Check if the object that was touched has the correct tag
			if ( other.tag == hitTargetTag )
			{
                if(animator != null)
				    animator.Play("Active", 0);

				if (col != null)
					col.enabled = false;

				//Go through the list of functions and runs them on the correct targets
				foreach( TouchFunction touchFunction in touchFunctions )
				{
					//Check that we have a target tag and function name before running
					if ( touchFunction.targetTag != string.Empty && touchFunction.functionName != string.Empty )
					{
						//Run the function
						GameObject.FindGameObjectWithTag(touchFunction.targetTag).SendMessage(touchFunction.functionName, touchFunction.functionParameter);
					}
				}

				if (coin != 0)
                {
					Minigame.instance.bonus += coin;
					Minigame.instance.SpawnCoin(this.transform.position, coin);
				}
					

				MageManager.instance.PlaySoundClip(soundHit);
			}
		}

        IEnumerator DestroyCoroutine()
        {
			yield return new WaitForSeconds(1);
			//Destroy the item
			Destroy(gameObject);
		}
	}
}