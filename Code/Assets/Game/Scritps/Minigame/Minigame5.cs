using System.Collections;
using System.Collections.Generic;
using InfiniteHopper.Types;
using UnityEngine;
using UnityEngine.UI;

public class Minigame5 : Minigame
{
	public GameObject guideUIPrefab;
	//The player object, and the current player
	public GameObject playerObject;
	Animator animator;

	//A list of background elements that loop
	public LoopingBackground[] loopingBackground;

	//A list of columns that that randomly appear as the player moves forward
	public GameObject[] topColumn;
	public GameObject[] downColumn;

	//The range of the horizontal and vertical gap between each two columns
	public Vector2 columnGapRange = new Vector2(3, 7);
	public Vector2 columnHeightRange = new Vector2(-4, 0);
	public float deathLineHeight = -10;

	//A list of items that can appear on columns
	public Transform[] items;

	public float force = 100;
    public float speedRate = 1;
	float speed = 1;
	public GameObject fallEffect;
	GuideUI guildUI;
	public AudioClip[] sounds;
	GameObject scoreColumn;
	bool isDead = false;
	int count = 0;

	public List<GameObject> columns = new List<GameObject>();


	protected override void Start()
	{
		base.Start();
		MageManager.instance.PlayMusicName("Minigame2", true);
		animator = playerObject.GetComponent<Animator>();

		//MageManager.instance.PlayMusic(music, 0, true);

		//If the player object is not already assigned, Assign it from the "Player" tag
		//if (cameraObject == null) cameraObject = GameObject.FindGameObjectWithTag("MainCamera").transform;

		for (int index = 0; index < loopingBackground.Length; index++)
		{
			//Choose a random time from the animation
			//loopingBackground[index].backgroundObject.animation[loopingBackground[index].backgroundObject.animation.clip.name].time = Random.Range(0, loopingBackground[index].backgroundObject.animation.clip.length);
			loopingBackground[index].backgroundObject.GetComponent<Animation>()[loopingBackground[index].backgroundObject.GetComponent<Animation>().clip.name].time = loopingBackground[index].animationOffset;

			//Enable the animation
			loopingBackground[index].backgroundObject.GetComponent<Animation>()[loopingBackground[index].backgroundObject.GetComponent<Animation>().clip.name].enabled = true;

			//Sample the animation at the current time
			loopingBackground[index].backgroundObject.GetComponent<Animation>().Sample();

			//Disable the animation
			loopingBackground[index].backgroundObject.GetComponent<Animation>()[loopingBackground[index].backgroundObject.GetComponent<Animation>().clip.name].enabled = false;

			//Play the animation from the new time we sampled
			loopingBackground[index].backgroundObject.GetComponent<Animation>().Play();
		}

		speed = 0;

		playerObject.SetActive(false);

        for(int i = 0; i < 10; i++)
        {
			createColumn();
        }

		scoreColumn = columns[0];

		if (GameManager.instance.myPlayer.minigameLevels.Length > minigameId && GameManager.instance.myPlayer.minigameLevels[minigameId] == 0)
			OnGuildPanel();
		else
			StartGame();
	}



    private void FixedUpdate()
    {
		if (state == GameState.Run)
		{
			time += Time.deltaTime;


			//If there is a player object, you can make it jump, the background moves in a loop.
			if (playerObject)
			{
				//If the player object moves below the death line, kill it.
				if (playerObject.transform.position.y < deathLineHeight)
					EndGame();
			}

			speed = Mathf.Lerp(speed, speedRate, Time.deltaTime * 2);

			playerObject.transform.rotation = Quaternion.Lerp(playerObject.transform.rotation, Quaternion.Euler(0,0,10), Time.deltaTime * 2);

			foreach (GameObject go in columns)
			{
				go.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
			}

			if (columns[0].transform.position.x < -10)
			{
				Debug.Log("Remove " + columns[0].name);
				columns[0].SetActive(false);
				GameObject.DestroyImmediate(columns[0]);
				columns.RemoveAt(0);
				createColumn();
			}

			if (scoreColumn.transform.position.x < playerObject.transform.position.x)
			{
				NextColumn();
				MageManager.instance.PlaySoundClip(sounds[0]);
				bonus += 3;
			}



		}
		else
		{
			speed = 0;
		}

		//Set the speed of the looping background based on the horizontal speed of the player
		for (int index = 0; index < loopingBackground.Length; index++)
		{
			loopingBackground[index].backgroundObject.GetComponent<Animation>()[loopingBackground[index].backgroundObject.GetComponent<Animation>().clip.name].speed = loopingBackground[index].animationSpeed * speed;
		}

		if (playerObject.transform.position.y < deathLineHeight && !isDead)
		{
			isDead = true;
			Vector3 pos = playerObject.transform.position;
			pos.y = -7;
			Instantiate(fallEffect, pos, Quaternion.identity);
			//If there is a source and a sound, play it from the source
			MageManager.instance.PlaySound("water_splash_object_body_01", false);
		}
	}


    void NextColumn()
    {
        for(int i = 0; i < columns.Count; i++)
        {
            if(columns[i] == scoreColumn)
            {
				scoreColumn = columns[i + 1];
				return;
            }
        }
    }

	//This function creates a column
	void createColumn()
	{
		count++;
		int id1 = Random.Range(0, 2);
		int id2 = Random.Range(0, 2);
		GameObject go1 = GameObject.Instantiate(topColumn[id1]) as GameObject;
		GameObject go2 = GameObject.Instantiate(downColumn[id2]) as GameObject;
		Vector3 pos1 = Vector3.zero;
		Vector3 pos2 = Vector3.zero;


		if (columns.Count == 0)
        {
			pos1.x = 20;
			pos2.x = 20;
        }
        else
        {
			pos1 = columns[columns.Count - 1].transform.position;
			pos1.x += Random.Range(columnGapRange.x, columnGapRange.y);
			pos2 = pos1;
		}

		pos1.y = Random.Range(columnHeightRange.x, columnHeightRange.y);
		pos2.y = pos1.y - Random.Range(3.5f, 4.5f);

		if (pos2.y > -1)
			pos2.y = -1;


		go1.transform.position = pos1;
		go2.transform.position = pos2;
		go2.transform.parent = go1.transform;
		go1.name = "Column " + count.ToString();
		columns.Add(go1);
	}

	public void OnJump()
    {
        if(state == GameState.Run)
        {
			MageManager.instance.PlaySound("Throw",false);
			speed = 1.5f * speedRate;
			playerObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			playerObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, force));
			playerObject.transform.rotation = Quaternion.Euler(0, 0, 30);
			animator.Play("Jump", 0);
		}
    }

	public override void StartGame()
	{
		if (state == GameState.Ready)
		{
			playerObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-100, -500));
			state = GameState.Run;
			playerObject.SetActive(true);
			speed = speedRate;
		}
	}

	public override void EndGame()
	{
		MageManager.instance.PlaySound("Punch",false);
		animator.Play("Fall");
		state = GameState.End;
		StartCoroutine(EndGameCoroutine());
	}

    IEnumerator EndGameCoroutine()
    {
		yield return new WaitForSeconds(0.2f);
		MageManager.instance.PlaySound("Whistle_slide_up_01", false); 
		yield return new WaitForSeconds(2);
		MageManager.instance.StopMusic();
		OnEndGame(bonus);
	}

	public void OnGuildPanel()
	{
		if (guildUI == null)
		{
			var popup = Instantiate(guideUIPrefab) as GameObject;
			popup.SetActive(true);
			//popup.transform.localScale = Vector3.zero;
			popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
			popup.GetComponent<Popup>().Open();
			guildUI = popup.GetComponent<GuideUI>();
		}
	}
}
