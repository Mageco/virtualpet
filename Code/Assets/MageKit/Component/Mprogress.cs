using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mprogress : MonoBehaviour {

	public Image fill;
    public Text tip;
	float progress = 0;
    public Sprite[] loadingImages;
    public Image loadingImage;

	// Use this for initialization
	void OnEnable () {
		progress = 0;
		fill.fillAmount = 0;
        int id = Random.Range(0,DataHolder.Dialogs().GetDataCount());
        tip.text = DataHolder.Dialog(id).GetDescription(MageManager.instance.GetLanguage());
        int n = Random.Range(0, loadingImages.Length);
        loadingImage.sprite = loadingImages[n];

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
