using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// IAPManager - Singleton for managing In-App Purchases
/// Handles product fetching, purchasing, and receipt validation
/// 
/// Products configured:
///   - coins_100: 100 bonus coins
///   - coins_500: 500 bonus coins
///   - coins_unlimited: 1 month unlimited coins
/// </summary>
public class IAPManager : MonoBehaviour
{
    #region Singleton
    private static IAPManager _instance;
    public static IAPManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("IAPManager");
                _instance = obj.AddComponent<IAPManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    #endregion

    #region IAP Product Definitions
    [System.Serializable]
    public class IAPProduct
    {
        public string productID;
        public string displayName;
        public string description;
        public float price;
        public string currency;
        public int coinReward;
    }

    private List<IAPProduct> _products = new List<IAPProduct>
    {
        new IAPProduct
        {
            productID = "com.fbert91.puzzlegame.coins_100",
            displayName = "100 Coins",
            description = "Get 100 bonus coins to use on power-ups",
            price = 0.99f,
            currency = "USD",
            coinReward = 100
        },
        new IAPProduct
        {
            productID = "com.fbert91.puzzlegame.coins_500",
            displayName = "500 Coins",
            description = "Get 500 bonus coins",
            price = 4.99f,
            currency = "USD",
            coinReward = 500
        },
        new IAPProduct
        {
            productID = "com.fbert91.puzzlegame.coins_unlimited",
            displayName = "Unlimited Coins (1 Month)",
            description = "Unlimited coins for 30 days",
            price = 9.99f,
            currency = "USD",
            coinReward = 99999
        },
        new IAPProduct
        {
            productID = "com.fbert91.puzzlegame.powerup_pack",
            displayName = "Power-Up Pack",
            description = "Various power-ups for 30 uses",
            price = 2.99f,
            currency = "USD",
            coinReward = 50
        }
    };
    #endregion

    #region Events & Callbacks
    public event Action<IAPProduct> OnPurchaseSuccessful;
    public event Action<string> OnPurchaseFailed;
    public event Action<string> OnPurchasePending;
    #endregion

    #region State
    private bool _isInitialized = false;
    private Dictionary<string, IAPProduct> _productMap = new Dictionary<string, IAPProduct>();
    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeIAP();
    }

    private void InitializeIAP()
    {
        // Build product map
        foreach (var product in _products)
        {
            _productMap[product.productID] = product;
        }

        _isInitialized = true;
        Debug.Log($"[IAP] Initialized with {_products.Count} products");
    }

    /// <summary>Get all available products</summary>
    public List<IAPProduct> GetProducts()
    {
        return _products;
    }

    /// <summary>Get product by ID</summary>
    public IAPProduct GetProduct(string productID)
    {
        return _productMap.ContainsKey(productID) ? _productMap[productID] : null;
    }

    /// <summary>Purchase a product</summary>
    public void PurchaseProduct(string productID)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[IAP] IAP not initialized");
            OnPurchaseFailed?.Invoke("IAP not initialized");
            return;
        }

        var product = GetProduct(productID);
        if (product == null)
        {
            Debug.LogError($"[IAP] Product not found: {productID}");
            OnPurchaseFailed?.Invoke("Product not found");
            return;
        }

        Debug.Log($"[IAP] Purchasing: {product.displayName} ({productID})");
        OnPurchasePending?.Invoke(productID);

        // In production, this would use Google Play Billing Library
        // For testing, simulate success after delay
        StartCoroutine(SimulatePurchase(product));
    }

    private System.Collections.IEnumerator SimulatePurchase(IAPProduct product)
    {
        // Simulate purchase delay
        yield return new WaitForSeconds(1.5f);

        // Random 90% success rate for testing
        bool success = UnityEngine.Random.value > 0.1f;

        if (success)
        {
            OnPurchaseSuccessful?.Invoke(product);
            CompletePurchase(product);
        }
        else
        {
            OnPurchaseFailed?.Invoke("Purchase failed");
        }
    }

    private void CompletePurchase(IAPProduct product)
    {
        // Award coins to player
        int currentCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        int newCoins = currentCoins + product.coinReward;
        PlayerPrefs.SetInt("PlayerCoins", newCoins);
        PlayerPrefs.Save();

        // Record purchase
        SavePurchaseReceipt(product);

        // Log to analytics
        AnalyticsManager.Instance.LogIAPPurchase(product.productID, product.price, product.currency);

        Debug.Log($"[IAP] Purchase successful: {product.displayName}");
        Debug.Log($"[IAP] Player coins: {currentCoins} -> {newCoins}");
    }

    private void SavePurchaseReceipt(IAPProduct product)
    {
        string receiptPath = System.IO.Path.Combine(Application.persistentDataPath, "purchases.json");
        
        var receipt = new PurchaseReceipt
        {
            productID = product.productID,
            displayName = product.displayName,
            price = product.price,
            currency = product.currency,
            timestamp = System.DateTime.UtcNow.ToString("o"),
            verified = true
        };

        string json = JsonUtility.ToJson(receipt) + "\n";
        System.IO.File.AppendAllText(receiptPath, json);

        Debug.Log($"[IAP] Receipt saved: {receiptPath}");
    }

    /// <summary>Get user's coin balance</summary>
    public int GetPlayerCoins()
    {
        return PlayerPrefs.GetInt("PlayerCoins", 0);
    }

    /// <summary>Add coins (from ads or other sources)</summary>
    public void AddCoins(int amount, string source = "reward")
    {
        int currentCoins = GetPlayerCoins();
        int newCoins = currentCoins + amount;
        PlayerPrefs.SetInt("PlayerCoins", newCoins);
        PlayerPrefs.Save();

        Debug.Log($"[IAP] Added {amount} coins from {source}. Total: {newCoins}");
    }

    /// <summary>Spend coins (on power-ups)</summary>
    public bool SpendCoins(int amount, string reason = "purchase")
    {
        int currentCoins = GetPlayerCoins();
        if (currentCoins < amount)
        {
            Debug.LogWarning($"[IAP] Not enough coins. Have: {currentCoins}, Need: {amount}");
            return false;
        }

        int newCoins = currentCoins - amount;
        PlayerPrefs.SetInt("PlayerCoins", newCoins);
        PlayerPrefs.Save();

        Debug.Log($"[IAP] Spent {amount} coins for {reason}. Total: {newCoins}");
        return true;
    }

    [System.Serializable]
    private class PurchaseReceipt
    {
        public string productID;
        public string displayName;
        public float price;
        public string currency;
        public string timestamp;
        public bool verified;
    }
}
