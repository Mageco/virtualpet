using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSpawner : MonoBehaviour
{
    public GameObject petPrefab;

    public GameObject agentPrefabs;
    CharController character;
    PolyNavAgent agent;
    
    // Start is called before the first frame update
    void Awake()
    {
        LoadPet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadPet(){
        GameObject go = GameObject.Instantiate(petPrefab) as GameObject;
        go.transform.parent = this.transform;
        character = go.GetComponent<CharController>();

		GameObject go1 = GameObject.Instantiate(agentPrefabs) as GameObject;
		PolyNavAgent agent = go1.GetComponent<PolyNavAgent>();
		agent.LoadCharacter(character);
        character.agent = agent;
    }
}
