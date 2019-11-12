using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSpawner : MonoBehaviour
{
    public GameObject petPrefab;

    public GameObject agentPrefabs;
    CharController character;
    PolyNavAgent agent;


    public CharController LoadPet(){
        GameObject go = Instantiate(petPrefab) as GameObject;
        go.transform.parent = this.transform;
        character = go.GetComponent<CharController>();

		GameObject go1 = Instantiate(agentPrefabs) as GameObject;
		agent = go1.GetComponent<PolyNavAgent>();
        go1.transform.parent = transform;
		agent.LoadCharacter(character);
        character.agent = agent;
        return character;
    }
}
