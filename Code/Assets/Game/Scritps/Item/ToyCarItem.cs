﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

public class ToyCarItem : BaseFloorItem
{
	public float speed = 20;
	float time;
	float maxDoubleClickTime = 0.4f;
	bool isClick = false;
    int soundId = 0;

	Vector3[] paths;

	public bool isActive = false;

	public GameObject body;

	Direction direction = Direction.R;
	int round = 1;

	public GameObject energyBar;
	public GameObject energyProgress;


	protected override void Start(){
		base.Start();
		energyBar.SetActive(false);
		energyProgress.transform.localScale = new Vector3(0,1.5f,1);
	}
	Animator animator;

	protected override void Awake(){
		base.Awake();
		animator = this.GetComponent<Animator>();
	}

    // Update is called once per frame
    protected override void Update()
    {
		lastPosition = this.transform.position;
		base.Update();
		if(isActive){
			energyBar.SetActive(false);
			animator.speed = Mathf.Lerp(animator.speed,0,Time.deltaTime * 0.3f);
		}else
			animator.speed = 1;
		if(isDrag){
			animator.Play("Drag_" + direction.ToString());
			energyBar.SetActive(true);
			float t = Mathf.Clamp(dragTime/2,0,1.5f);
			
			energyProgress.transform.localScale = new Vector3(t,1.5f,1);
		}
    }

	public bool IsSupprised(){
		if(animator.speed > 1 && isActive){
			return true;
		}else
			return false;
	}

	protected override void OnMouseDown(){
		base.OnMouseDown();
		iTween.Stop(this.gameObject);
		isActive = false;
		animator.Play("Idle_"+direction.ToString());

        soundId = MageManager.instance.PlaySoundName("Item_Car_Charge", true);

    }

	protected override void OnMouseUp()
	{
		round = Mathf.Clamp((int) dragTime*2 + 1,1,6);
		base.OnMouseUp();
        MageManager.instance.StopSound(soundId);
        if (!isActive){
			if(round > 1){
				Run();
			}else{
				iTween.StopByName("car");
				isActive = false;
				animator.Play("Idle_"+direction.ToString());
			}
		}

	}

	void Run(){
        MageManager.instance.PlaySoundName("Item_Car", false);
		isActive = true;
        animator.speed = 2;
		GameManager.instance.LogAchivement(AchivementType.Use_Item,ActionType.None,this.item.itemID);
		List<Transform> points = ItemManager.instance.GetRandomPoints (PointType.MouseEat);
		List<Transform> pointRandoms = ItemManager.instance.GetRandomPoints (PointType.Mouse);

		if(points == null || pointRandoms == null || points.Count == 0 || pointRandoms.Count == 0){
			return;
		}
		Debug.Log(round);
		paths = new Vector3[round];
		for (int i=0;i< round;i++){
			paths [i] = pointRandoms[Random.Range(0,pointRandoms.Count)].position;
		}

		iTween.MoveTo (this.gameObject, iTween.Hash ("name","car","path", paths, "speed", speed, "orienttopath", false,"oncomplete", "CompleteSeek"));
		
		animator.Play("Run_" + direction.ToString(),0);
	}

	void CompleteSeek()
	{
		body.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		body.transform.localScale = new Vector3 (body.transform.localScale.x, body.transform.localScale.y, 1);
		animator.Play("Idle_" + direction.ToString(),0);
		Debug.Log ("Complete Run");
		isActive = false;
	}

	void LateUpdate()
	{
		
		//if (isActive) {
			Vector3 pos = this.transform.position;
			pos.z = this.transform.position.y * 10;
			this.transform.position = pos;

			if(isActive){
				if (pos.x > lastPosition.x) {
					direction = Direction.R;
				} else {
					direction = Direction.L;
				}
				animator.Play("Run_" + direction.ToString(),0);
			}

			float offset = initZ;

			if (this.transform.position.y < offset)
				this.transform.localScale = originalScale * (1 + (-this.transform.position.y + offset) * scaleFactor);
			else
				this.transform.localScale = originalScale;	

		//}



	}
}

