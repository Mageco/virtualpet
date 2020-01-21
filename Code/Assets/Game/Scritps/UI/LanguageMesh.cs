using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LanguageMesh : MonoBehaviour {

	public TextMesh text;
	public int dialogId = -1;

    private void Awake()
    {
		text = this.GetComponent<TextMesh>();
    }

    // Use this for initialization
    void Start () {
		ChangeLanguage ();
	}

	public void ChangeLanguage()
	{
        if(text != null)
		    text.text = DataHolder.GetDialog(dialogId).GetName(MageManager.instance.GetLanguage());
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}


