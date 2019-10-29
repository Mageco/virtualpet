using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{

	public static UIManager instance;
	[HideInInspector]
	public NotificationType notification = NotificationType.None;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnCall()
	{
		InputController.instance.OnCall ();
	}

	public void OnNotify(NotificationType type){
		notification = type;
	}
}

public enum NotificationType{None,ItemPurchase,SkillLevelUp}
