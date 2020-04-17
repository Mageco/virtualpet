using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class LanguageMeshPro : MonoBehaviour {

	TextMeshPro text;
	public int dialogId = -1;

    private void Awake()
    {
		text = this.GetComponent<TextMeshPro>();
    }

    // Use this for initialization
    void Start () {
		ChangeLanguage ();
	}

	public void ChangeLanguage()
	{
        if(text != null)
		    text.text = DataHolder.Dialog(dialogId).GetName(MageManager.instance.GetLanguage());
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}


