using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEvent : MonoBehaviour
{
    CharController character;
    public ItemEventType itemEventType;
    // Start is called before the first frame update
    void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player") {

		}
	}

	void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {

		}
	}
}
