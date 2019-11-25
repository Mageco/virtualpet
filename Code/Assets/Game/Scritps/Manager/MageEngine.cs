using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using SimpleJSON;
using MageApi.Models;
using MageApi.Models.Request;
using MageApi.Models.Response;
using MageApi;
using Mage.Models.Users;
using Mage.Models.Game;
using Mage.Models.Application;
using Mage.Models;

namespace MageSDK.Client {
	public class MageEngine : MonoBehaviour {
		///<summary>isLocalApplicationData is using to indicate where to get Application Data.</summary>
		// this is used to test application in Editor mode, if this is true then Application Data will be load from local resources
		public bool isLocalApplicationData = true;
		///<summary>resetUserDataOnStart is used during test in Unity editor to reset user data whenever editor is running.</summary>
		// if this variable is false, then data will be save to local storage and server accordingly
		public bool resetUserDataOnStart = true;
		[HideInInspector]
		public static MageEngine instance;

		#region private variables
		private bool _isLogin = false;
		#endregion

		public void Awake() {
			if (instance == null)
				instance = this;
			else
				GameObject.Destroy (this.gameObject);

			#if UNITY_EDITOR
				if (resetUserDataOnStart) {
					this._isLogin = true;
				} else {

				}
			#else

			#endif

			Debug.Log("Login: " + this._isLogin.ToString());		
			
		}

		public void Start() {
			Debug.Log(IsLogin());
		}

		public bool IsLogin() {
			Debug.Log("Is logged in: " + this._isLogin.ToString());
			return this._isLogin;
		}

		
	}
}

