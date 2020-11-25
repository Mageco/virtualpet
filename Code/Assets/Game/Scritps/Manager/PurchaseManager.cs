#if USE_UNITY_PURCHASE
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using MageSDK.Client;
using Mage.Models.Users;
using MageApi;
using UnityEngine.Purchasing.Security;

public class PurchaseManager : MonoBehaviour, IStoreListener
{

    public static PurchaseManager instance;
    [HideInInspector]
    public bool isSubscripted = false;
    public bool IsRemoveAd = false;

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.
    public string[] consumableIds;
    public string[] nonConsumableIds;
    public string[] subScriptionIds;

    [HideInInspector]
    public int currentLesson = 0;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            GameObject.Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    string GetKey()
    {
        return Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013");
    }

    bool IsOK(string key)
    {
        if (key == GetKey())
        {
            GameManager.instance.count++;
            return true;
        }
        return false;
    }

    #region IN App action
    void OnPurchaseConsumableComplete(int id, string key)
    {

        if (IsOK(key))
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(76).GetName(MageManager.instance.GetLanguage()));
            if (id == 0)
            {
                GameManager.instance.AddDiamond(DataHolder.GetItem(3).sellPrice, GetKey());
            }
            else if (id == 1)
            {
                GameManager.instance.AddDiamond(DataHolder.GetItem(19).sellPrice, GetKey());
            }
            else if (id == 2)
            {
                GameManager.instance.AddDiamond(DataHolder.GetItem(20).sellPrice, GetKey());
            }
            else if (id == 3)
            {
                GameManager.instance.AddDiamond(DataHolder.GetItem(21).sellPrice, GetKey());
            }
            else if (id == 4)
            {
                GameManager.instance.AddDiamond(100, GetKey());
                GameManager.instance.AddRandomPet(RareType.Common, GetKey());
                GameManager.instance.AddItem(128, GetKey());
            }
            else if (id == 5)
            {
                GameManager.instance.AddDiamond(300, GetKey());
                GameManager.instance.AddRandomPet(RareType.Rare, GetKey());
                GameManager.instance.AddItem(129, GetKey());
            }
            else if (id == 6)
            {
                GameManager.instance.AddDiamond(1200, GetKey());
                GameManager.instance.AddRandomPet(RareType.Epic, GetKey());
                GameManager.instance.AddItem(130, GetKey());
            }
            MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.ConfirmPaymentItem, consumableIds[id]);
        }

    }

    void OnPurchaseComplete(int id)
    {

    }

    void OnSuscriptionComplete(int id)
    {
        isSubscripted = true;
    }

    public void OnCheckSubscriptionComplete(List<string> items)
    {


    }



    #endregion

    #region purchase
    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        //builder.AddProduct(kProductIDConsumable, ProductType.Consumable);
        for (int i = 0; i < consumableIds.Length; i++)
        {
            builder.AddProduct(consumableIds[i], ProductType.Consumable);
        }
        // Continue adding the non-consumable product.
        for (int i = 0; i < nonConsumableIds.Length; i++)
        {
            builder.AddProduct(nonConsumableIds[i], ProductType.NonConsumable);
        }
        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 

        for (int i = 0; i < subScriptionIds.Length; i++)
        {
            builder.AddProduct(subScriptionIds[i], ProductType.Subscription, new IDs() {
                { subScriptionIds [i], AppleAppStore.Name },
                { subScriptionIds [i], GooglePlay.Name },
            });
        }


        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyConsumable(int id)
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(consumableIds[id]);

    }


    public void BuyNonConsumable(int id)
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(nonConsumableIds[id]);
        MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.CheckOutItem, nonConsumableIds[id]);
    }


    public void BuySubscription(int id)
    {
        // Buy the subscription product using its the general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        // Notice how we use the general product identifier in spite of this ID being mapped to
        // custom store-specific identifiers above.
        BuyProductID(subScriptionIds[id]);
    }


    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            InitializePurchasing();
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public bool IsProductPurchased(string productId)
    {
        Product product = m_StoreController.products.WithID(productId);
        if (product != null && !product.availableToPurchase)
        {
            return true;
        }
        else
            return false;
    }

    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {

        // if any receiver consumed this purchase we return the status
        bool validPurchase = true;

#if UNITY_ANDROID && !SKIP_IAP_VALIDATION
        validPurchase = false;
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

        try
        {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(args.purchasedProduct.receipt);
            string storedTransactionIDs = MageEngine.instance.GetUser().GetUserData(UserBasicData.StoredIAPTransactionIDs);
            // For informational purposes, we list the receipt(s)
            ApiUtils.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {

                GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                if (null != google)
                {
                    // This is Google's Order ID.
                    // Note that it is null when testing in the sandbox
                    // because Google's sandbox does not provide Order IDs.
                    // check if Transaction ID has been used
                    if (productReceipt.productID == args.purchasedProduct.definition.id &&
                            !IsTransactionIDAlreadyUsed(google.transactionID, storedTransactionIDs))
                    {
                        validPurchase = true;
                        // update purchased transaction id
                        storedTransactionIDs += storedTransactionIDs + "#/#" + google.transactionID;
                        UserData purchasedId = new UserData(UserBasicData.StoredIAPTransactionIDs.ToString(), storedTransactionIDs, "MageEngine");
                        MageEngine.instance.UpdateUserData(purchasedId);
                        break;
                    }
                }
            }


        }
        catch (IAPSecurityException)
        {
            ApiUtils.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#endif

        if (validPurchase)
        {
            // A consumable product has been purchased by this user.
            for (int i = 0; i < consumableIds.Length; i++)
            {
                if (String.Equals(args.purchasedProduct.definition.id, consumableIds[i], StringComparison.Ordinal))
                {
                    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
                    OnPurchaseConsumableComplete(i, GetKey());
                }
            }

            // Or ... a subscription product has been purchased by this user.

            for (int i = 0; i < subScriptionIds.Length; i++)
            {
                if (String.Equals(args.purchasedProduct.definition.id, subScriptionIds[i], StringComparison.Ordinal))
                {
                    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    OnSuscriptionComplete(i);
                }
            }
            // Or ... a non-consumable product has been purchased by this user.

            for (int i = 0; i < nonConsumableIds.Length; i++)
            {
                if (String.Equals(args.purchasedProduct.definition.id, nonConsumableIds[i], StringComparison.Ordinal))
                {
                    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
                    OnPurchaseComplete(i);
                }
            }
        }


        /*
		// Or ... an unknown product has been purchased by this user. Fill in additional products here....
		else 
		{
			Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
		}*/

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    bool IsTransactionIDAlreadyUsed(string transactionID, string storedTransactionIDs)
    {
        string[] storedTransactionIdsSplits = storedTransactionIDs.Split(new string[] { "#/#" }, StringSplitOptions.None);

        foreach (string alreadyUsedTransactionID in storedTransactionIdsSplits)
        {
            if (transactionID == alreadyUsedTransactionID)
            {
                return true;
            }
        }
        return false;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    #endregion

}

#endif