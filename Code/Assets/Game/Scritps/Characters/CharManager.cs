using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharManager : MonoBehaviour
{

    public GameObject petSmallPrefab;
    public GameObject petNormalPrefab;
    public GameObject petMiddlePrefab;

    public CharAge age = CharAge.Small;

    
    CharController character;
    PolyNavAgent agent;
    
    // Start is called before the first frame update
    void Awake()
    {
        agent = this.GetComponentInChildren<PolyNavAgent>();
        LoadPet();
        agent.LoadCharacter(character);
        character.agent = agent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadPet(){
        if(age == CharAge.Small){
            GameObject go = GameObject.Instantiate(petSmallPrefab) as GameObject;
            go.transform.parent = this.transform;
            character = go.GetComponent<CharController>();
        }else if(age == CharAge.Middle){
            GameObject go = GameObject.Instantiate(petMiddlePrefab) as GameObject;
            go.transform.parent = this.transform;
            character = go.GetComponent<CharController>();
        }else if(age == CharAge.Big){
            GameObject go = GameObject.Instantiate(petNormalPrefab) as GameObject;
            go.transform.parent = this.transform;
            character = go.GetComponent<CharController>();
        }

    }
}
