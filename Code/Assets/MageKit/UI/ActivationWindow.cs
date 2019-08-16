using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivationWindow : MonoBehaviour {

	public InputField txtActiveCode;
	public InputField txtNumberCode;
	public Text[] codes;
	[HideInInspector]
	public string number = "";
	bool isActivate = false;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < codes.Length; i++) {
			codes [i].text = "";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (txtActiveCode.isFocused)
			CheckCapet ();
		else
			OffCapet();

	}

	public void Load()
	{
	}

	public void OnActivate()
	{
		if (!isActivate) {
			isActivate = true;
			StartCoroutine (SetActive ());

			number = txtNumberCode.text;
			string code = txtActiveCode.text.ToUpper();
			code = code.Replace (" ", "");

			if (number == "" || number.ToCharArray ().Length < 9) {
				MPopup popup = MageManager.instance.OnNotificationPopup ("Chú ý","Bạn vui lòng điền số điện thoại của bạn");
				return;
			}
			if (txtActiveCode.text == "" || code.Length != 12) {
				MPopup popup = MageManager.instance.OnNotificationPopup ("Chú ý","Bạn vui lòng điền mã kích hoạt gồm 12 chữ số không có dấu cách");
				return;
			} else {
				ES2.Save (code, "ActivationCode");
				ES2.Save (number, "PhoneNumber");
				ApiManager.instance.ValdiateActivationCode (code, number);
				MageManager.instance.OnWaiting ();
			}

		}
	}

	IEnumerator SetActive()
	{
		yield return new WaitForSeconds (1);
		isActivate = false;
	}

	public void Close()
	{
		this.GetComponent<Popup> ().Close ();
	}

	public void OnEnterCode()
	{
		string t = txtActiveCode.text;
		char[] c = t.ToCharArray ();
		for (int i = 0; i < codes.Length; i++) {
			if (i < c.Length)
				codes [i].text = c [i].ToString ().ToUpper();
			else
				codes [i].text = "";
		}

		CheckCapet ();

	}

	void CheckCapet()
	{
		bool isCapet = false;
		for (int i = 0; i < codes.Length; i++) {
			if (codes [i].text == "") {
				if (!isCapet) {
					codes [i].transform.parent.GetComponent<Animator> ().Play ("Active", 0);
					isCapet = true;
				}
				else
					codes [i].transform.parent.GetComponent<Animator> ().Play ("Idle", 0);

			}
			else
				codes [i].transform.parent.GetComponent<Animator> ().Play ("Idle", 0);
			
		}
	}

	void OffCapet()
	{
		for (int i = 0; i < codes.Length; i++) {
			codes [i].transform.parent.GetComponent<Animator> ().Play ("Idle", 0);
		}
	}

	public void ForGetActivationCode()
	{
		//MenuWindow.instance.OnForgetActivationWindow ();
	}

	public void OnWeb()
	{
		Application.OpenURL (ApiManager.instance.contactUrl);
	}
}
