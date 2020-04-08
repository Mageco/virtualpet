using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePhotoUI : MonoBehaviour
{
    public int defaultCropSize = 256;
    public int minCropSize = 128;
    public int cropSize = 256;
    bool isDragging;
    Vector3 dragOffset;
    public Image crop;
	public Button takePhoto;
	public Slider slide;
	float size;
    float scaleX;
    

	void Awake()
	{
		//scaleX = 600.0f / Screen.width;
		Debug.Log(scaleX);
		Debug.Log("Screen " + Screen.width + "  " + Screen.height);
        if (ES2.Exists("TakePhotoPosition"))
        {
			crop.transform.localPosition = ES2.Load<Vector3>("TakePhotoPosition");
        }

        if (ES2.Exists("TakePhotoScale"))
        {
			defaultCropSize = ES2.Load<int>("TakePhotoScale");
        }
	}
	void Start()
	{
		Load();
	}


	void Load()
	{
		defaultCropSize = minCropSize * 2;
		if (ES2.Exists("TakePhotoPosition"))
		{
			crop.transform.localPosition = ES2.Load<Vector3>("TakePhotoPosition");
		}

		if (ES2.Exists("TakePhotoScale"))
		{
			defaultCropSize = ES2.Load<int>("TakePhotoScale");
			slide.value = (defaultCropSize * 1f / minCropSize - 1) / 7f;
		}
		crop.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultCropSize, defaultCropSize);
	}

	// Update is called once per frame
	void Update()
	{
		if (isDragging)
		{
			Vector3 pos = Input.mousePosition - dragOffset;
			float deltaX = (Screen.width/2.0f - size / 2.0f) / scaleX;
			float deltaY = (Screen.height / 2.0f - size / 2.0f) / scaleX;
			//(image.GetComponent<RectTransform>().sizeDelta.x / 2.0f - radiusX / 2.0f) / scaleX;
			//float deltaY = //(image.GetComponent<RectTransform>().sizeDelta.y / 2.0f - radiusY / 2.0f) / scaleX;
			if (pos.x <  - deltaX)
				pos.x =  - deltaX;
			if (pos.x >  + deltaX)
				pos.x =  + deltaX;
			if (pos.y <  - deltaY)
				pos.y =  - deltaY;
			if (pos.y >  + deltaY)
				pos.y =  + deltaY;
			crop.transform.position = pos;
		}
	}

	public void ChangeCropSize()
	{
		defaultCropSize = Mathf.Clamp((int)(minCropSize * (1 + slide.value * 7)),128,1080);
		crop.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultCropSize, defaultCropSize);
		ES2.Save(defaultCropSize, "TakePhotoScale");
	}

	public void TakePhoto()
	{
		StartCoroutine(TakePhotoCoroutine());
	}

    IEnumerator TakePhotoCoroutine()
    {
        //Deactive
		crop.gameObject.SetActive(false);
		slide.gameObject.SetActive(false);
		takePhoto.gameObject.SetActive(false);


		int resWidth = Screen.width;
		int resHeight = Screen.height;
		float scale = Screen.height / 1080f;
		Debug.Log(scale);
		Debug.Log(defaultCropSize);
		yield return new WaitForEndOfFrame();
		var texture = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, true);
		texture.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		texture.Apply();
		yield return new WaitForEndOfFrame();

	    Vector2 pos = new Vector2(texture.width / 2.0f + crop.transform.localPosition.x*scale, texture.height / 2.0f + crop.transform.localPosition.y * scale);
		texture = Utils.instance.CropSquare(texture, pos, (int)(defaultCropSize * scale));
		if (texture.height > cropSize)
			texture = Utils.instance.Resize(cropSize, texture);
		UIManager.instance.avatarUI.LoadAvatar(Utils.instance.CreateSprite(texture));


        //Reactive
		//ServerManager.instance.UploadCustomerAvatar(editTexture);
		crop.gameObject.SetActive(true);
		slide.gameObject.SetActive(true);
		takePhoto.gameObject.SetActive(true);

		Resources.UnloadUnusedAssets();
		UIManager.instance.OffTakePhotoPanel();
	}

	public void OnDrag()
	{
		isDragging = true;
		dragOffset = Input.mousePosition - crop.transform.position;
	}

	public void OnDrop()
	{
		isDragging = false;
		ES2.Save(crop.transform.localPosition, "TakePhotoPosition");

	}

    public void Close()
    {
		UIManager.instance.OffTakePhotoPanel();
    }
}
