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
		public MageEventType eventName = MageEventType.OpenScreen;
		public string eventDetail = "";

		public DateTime eventDate = DateTime.Now;

		public MageEvent() : base () {
		}

		public MageEvent(MageEventType type, string eventDetail = "") {
			this.eventName = type;
			this.eventDetail = eventDetail;
			eventDate = DateTime.Now;

		}

	}

	public enum MageEventType {

		///<summary>Activation.OpenActivationWindow
		OpenActivationWindow,
		///<summary>Activation.ActivateFailed
		ActivateFailed,
		///<summary>Activation.ActivateSuccessful
		ActivateSuccessful,
		///<summary>Activation.DiscardActivationWindow
		DiscardActivationWindow,
		///<summary>Activation.OpenExternalLink
		OpenExternalLink,
		///<summary>Admob.VideoAdLoaded
		VideoAdLoaded,
		///<summary>Admob.VideoAdStarted
		VideoAdStarted,
		///<summary>Admob.VideoAdOpened
		VideoAdOpened,
		///<summary>Admob.VideoAdClosed
		VideoAdClosed,
		///<summary>Admob.VideoAdRewarded
		VideoAdRewarded,
		///<summary>Admob.VideoAdFailtoLoaded
		VideoAdFailtoLoaded,
		///<summary>Admob.InterstitialAdLoaded
		InterstitialAdLoaded,
		///<summary>Admob.InterstitialAdShow
		InterstitialAdShow,
		///<summary>Admob.InterstitialAdOpened
		InterstitialAdOpened,
		///<summary>Admob.InterstitialAdClosed
		InterstitialAdClosed,
		///<summary>Admob.InterstitialAdFailtoLoaded
		InterstitialAdFailtoLoaded,
		///<summary>Admob.BannerAdLoaded
		BannerAdLoaded,
		///<summary>Admob.BannerAdShow
		BannerAdShow,
		///<summary>Admob.BannerAdClosed
		BannerAdClosed,
		///<summary>Admob.BannerAdOpened
		BannerAdOpened,
		///<summary>UserAction.OpenApplication
		OpenApplication,
		///<summary>UserAction.QuitApplication
		QuitApplication,
		///<summary>UserAction.OpenCreateUserProfile
		OpenCreateUserProfile,
		///<summary>UserAction.CancelCreateUserProfile
		CancelCreateUserProfile,
		///<summary>UserAction.CreateUserProfileFailed
		CreateUserProfileFailed,
		///<summary>UserAction.CreateUserProfileSuccesful
		CreateUserProfileSuccesful,
		///<summary>UserAction.OpenLogin
		OpenLogin,
		///<summary>UserAction.LoginFailed
		LoginFailed,
		///<summary>UserAction.LoginSuccessful
		LoginSuccessful,
		///<summary>UserAction.AddCharacter
		AddCharacter,
		///<summary>UserAction.EquipItem
		EquipItem,
		///<summary>UserAction.UseItem
		UseItem,
		///<summary>UserAction.OpenScreen
		OpenScreen,
		///<summary>UserAction.ClickObject
		ClickObject,
		///<summary>Store.OpenStore
		OpenStore,
		///<summary>Store.OpenStoreCategory
		OpenStoreCategory,
		///<summary>Store.SearchItem
		SearchItem,
		///<summary>Store.OpenItem
		OpenItem,
		///<summary>Store.CheckOutItem
		CheckOutItem,
		///<summary>Store.ConfirmPaymentItem
		ConfirmPaymentItem,
		///<summary>Store.CancelPaymentItem
		CancelPaymentItem,
		///<summary>InAppPurchase.OpenIAPWindow
		OpenIAPWindow,
		///<summary>InAppPurchase.CloseIAPWindow
		CloseIAPWindow,
		///<summary>InAppPurchase.CheckoutIAPWindow
		CheckoutIAPWindow
	}
}
