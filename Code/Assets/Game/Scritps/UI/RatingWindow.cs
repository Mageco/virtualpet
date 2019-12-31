using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingWindow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Rate()
	{
        #if UNITY_ANDROID
                Application.OpenURL("https://play.google.com/store/apps/details?id=vn.com.mage.virtualpet");

#elif UNITY_IOS

#endif
        ES2.Save(true, "RateUs");
        Close();
        
    }

    void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
