using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

public class ToyCarItem : ToyItem
{
	public float speed = 20;
	Vector3[] paths;
	public GameObject body;
	Direction direction = Direction.R;


    protected override void OnClick()
    {
		OnActive();
    }


	public override void OnActive(){
		if (state == EquipmentState.Active)
			return;
		int round = Random.Range(2, 4);
        MageManager.instance.PlaySound3D("Item_Car", false,this.transform.position);
		state = EquipmentState.Active;
        animator.speed = 2;
		List<Vector3> pointRandoms = ItemManager.instance.GetRandomPoints (AreaType.All,round);

		if(pointRandoms == null || pointRandoms.Count == 0){
			return;
		}
		//Debug.Log(round);
		paths = new Vector3[round];
		for (int i=0;i< round;i++){
			paths [i] = pointRandoms[Random.Range(0,pointRandoms.Count)];
		}

		iTween.MoveTo (this.gameObject, iTween.Hash ("name","car","path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompleteSeek"));
		animator.Play("Run_" + direction.ToString(),0);
	}

	void CompleteSeek()
	{
		body.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		body.transform.localScale = new Vector3 (body.transform.localScale.x, body.transform.localScale.y, 1);
		animator.Play("Idle_" + direction.ToString(),0);
		Debug.Log ("Complete Run");
		state = EquipmentState.Idle;
	}

    protected override void Update()
    {
		lastPosition = this.transform.position;
		base.Update();
	}

    protected override void LateUpdate()
	{
		if (state == EquipmentState.Active)
		{
			if (this.transform.position.x > lastPosition.x)
			{
				direction = Direction.R;
			}
			else
			{
				direction = Direction.L;
			}
			animator.Play("Run_" + direction.ToString(), 0);
		}
		base.LateUpdate();

		
	}
}

