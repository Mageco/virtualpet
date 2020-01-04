using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class MageEvent : BaseModel
	{
		public string eventName = MageEventType.OpenScreen.ToString();
		public string eventDetail = "";

		public string eventDate = String.Format("{0:s}", DateTime.Now);

		public MageEvent() : base () {
		}

		public MageEvent(MageEventType type, string eventDetail = "") {
			this.eventName = type.ToString();
			this.eventDetail = eventDetail;
			eventDate = String.Format("{0:s}", DateTime.Now);

		}

	}

	public enum MageEventType {

		///<summary>
		///Activation.OpenActivationWindow
		///</summary>
		OpenActivationWindow,
		///<summary>
		///Activation.ActivateFailed
		///</summary>
		ActivateFailed,
		///<summary>
		///Activation.ActivateSuccessful
		///</summary>
		ActivateSuccessful,
		///<summary>
		///Activation.DiscardActivationWindow
		///</summary>
		DiscardActivationWindow,
		///<summary>
		///Activation.OpenExternalLink
		///</summary>
		OpenExternalLink,
		///<summary>
		///Admob.VideoAdLoaded
		///</summary>
		VideoAdLoaded,
		///<summary>
		///Admob.VideoAdStarted
		///</summary>
		VideoAdStarted,
		///<summary>
		///Admob.VideoAdOpened
		///</summary>
		VideoAdOpened,
		///<summary>
		///Admob.VideoAdClosed
		///</summary>
		VideoAdClosed,
		///<summary>
		///Admob.VideoAdRewarded
		///</summary>
		VideoAdRewarded,
		///<summary>
		///Admob.VideoAdFailtoLoaded
		///</summary>
		VideoAdFailtoLoaded,
		///<summary>
		///Admob.InterstitialAdLoaded
		///</summary>
		InterstitialAdLoaded,
		///<summary>
		///Admob.InterstitialAdShow
		///</summary>
		InterstitialAdShow,
		///<summary>
		///Admob.InterstitialAdOpened
		///</summary>
		InterstitialAdOpened,
		///<summary>
		///Admob.InterstitialAdClosed
		///</summary>
		InterstitialAdClosed,
		///<summary>
		///Admob.InterstitialAdFailtoLoaded
		///</summary>
		InterstitialAdFailtoLoaded,
		///<summary>
		///Admob.BannerAdLoaded
		///</summary>
		BannerAdLoaded,
		///<summary>
		///Admob.BannerAdShow
		///</summary>
		BannerAdShow,
		///<summary>
		///Admob.BannerAdClosed
		///</summary>
		BannerAdClosed,
		///<summary>
		///Admob.BannerAdOpened
		///</summary>
		BannerAdOpened,
		///<summary>
		///UserAction.OpenApplication
		///</summary>
		OpenApplication,
		///<summary>
		///UserAction.QuitApplication
		///</summary>
		QuitApplication,
		///<summary>
		///UserAction.OpenCreateUserProfile
		///</summary>
		OpenCreateUserProfile,
		///<summary>
		///UserAction.CancelCreateUserProfile
		///</summary>
		CancelCreateUserProfile,
		///<summary>
		///UserAction.CreateUserProfileFailed
		///</summary>
		CreateUserProfileFailed,
		///<summary>
		///UserAction.CreateUserProfileSuccesful
		///</summary>
		CreateUserProfileSuccesful,
		///<summary>
		///UserAction.UpdateUserData
		///</summary>
		UpdateUserData,
		///<summary>
		///UserAction.OpenLogin
		///</summary>
		OpenLogin,
		///<summary>
		///UserAction.LoginFailed
		///</summary>
		LoginFailed,
		///<summary>
		///UserAction.LoginSuccessful
		///</summary>
		LoginSuccessful,
		///<summary>
		///UserAction.AddCharacter
		///</summary>
		AddCharacter,
		///<summary>
		///UserAction.EquipItem
		///</summary>
		EquipItem,
		///<summary>
		///UserAction.UseItem
		///</summary>
		UseItem,
		///<summary>
		///UserAction.OpenScreen
		///</summary>
		OpenScreen,
		///<summary>
		///UserAction.ClickObject
		///</summary>
		ClickObject,
		///<summary>
		///Store.OpenStore
		///</summary>
		OpenStore,
		///<summary>
		///Store.OpenStoreCategory
		///</summary>
		OpenStoreCategory,
		///<summary>
		///Store.SearchItem
		///</summary>
		SearchItem,
		///<summary>
		///Store.OpenItem
		///</summary>
		OpenItem,
		///<summary>
		///Store.CheckOutItem
		///</summary>
		CheckOutItem,
		///<summary>
		///Store.ConfirmPaymentItem
		///</summary>
		ConfirmPaymentItem,
		///<summary>
		///Store.CancelPaymentItem
		///</summary>
		CancelPaymentItem,
		///<summary>
		///InAppPurchase.OpenIAPWindow
		///</summary>
		OpenIAPWindow,
		///<summary>
		///InAppPurchase.CloseIAPWindow
		///</summary>
		CloseIAPWindow,
		///<summary>
		///InAppPurchase.CheckoutIAPWindow
		///</summary>
		CheckoutIAPWindow

	}
}
