using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mprogress : MonoBehaviour {

	public Image fill;
    public Text tip;
	float progress = 0;
    public Image icon;
    public Image loadingImage;
	public int[] tipIds;

	// Use this for initialization
	void OnEnable () {
		progress = 0;
		fill.fillAmount = 0;
        int id = Random.Range(0,tipIds.Length);
        tip.text = DataHolder.Dialog(tipIds[id]).GetName(MageManager.instance.GetLanguage());
        int n = Random.Range(0, DataHolder.Pets().GetDataCount());
		string url = DataHolder.Pet(n).iconUrl.Replace("Assets/Game/Resources/", "");
		url = url.Replace(".png", "");
		icon.sprite = Resources.Load<Sprite>(url) as Sprite;
	}


    void Update()
	{
		fill.fillAmount = Mathf.Lerp (fill.fillAmount, progress, Time.deltaTime);
	}
	
	// Update is called once per frame
	public void UpdateProgress (float p) {
		if (p > 1)
			progress = 1;
		else if (p < 0)
			progress = 0;
		else
			progress = p;
	}
}
