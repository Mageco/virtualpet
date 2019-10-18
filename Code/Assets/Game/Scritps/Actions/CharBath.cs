using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharBath : CharAction
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSoap()
	{
		if(character.actionType == ActionType.Bath)
			anim.Play("Soap_D");
	}

	public void OnShower()
	{
		if(character.actionType == ActionType.Bath)
			anim.Play("Shower_LD");
	}

	public void OffShower()
	{
		if(character.actionType == ActionType.Bath)
			anim.Play("BathStart_D");
	}
}
