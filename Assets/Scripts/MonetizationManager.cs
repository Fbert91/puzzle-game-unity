using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Monetization system - Handles IAP (in-app purchases) and ads
/// Supports multiple platforms and monetization strategies
/// </summary>
public class MonetizationManager : MonoBehaviour
{
    [System.Serializable]
    public class IAPProduct
    {
        public string productId;
        public string displayName;
        public string description;
        public float price;
        public string category; // "powerup", "cosmetic", "booster", "starter_pack"
        public int quantity; // For consumables
    }

    [System.Serializable]
    public class PlayerCurrency
    {
        public int coins = 0;
        public int gems = 0; // Premium currency
        public int hints = 3;
        public Dictionary<string, int> unlockedCosmetics = new Dictionary<string, int>();
        public Dictionary<string, int> activeBoosters = new Dictionary<string, int>();
    }

    public static MonetizationManager Instance { get; private set; }

    [SerializeField] private List<IAPProduct> iapProducts = new List<IAPProduct>();
    [SerializeField] private string admobAppId = "ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy";
    [SerializeField] private string admobBannerId = "ca-app-pub-3940256099942544/6300978111"; // Test ad unit
    [SerializeField] private string admobRewardedId = "ca-app-pub-3940256099942544/5224354917"; // Test ad unit

    private PlayerCurrency playerCurrency = new PlayerCurrency();
    private bool isInitialized = false;

    public event Action<string, int> OnPurchaseSuccess; // productId, quantity
    public event Action<string> OnPurchaseFailed;
    public event Action<int> OnCoinsEarned;
    public event Action<int> OnGemsEarned;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeMonetization();
        LoadPlayerCurrency();
    }

    /// <summary>
    /// Initialize monetization systems (IAP, Ads)
    /// </summary>
    private void InitializeMonetization()
    {
        // Initialize IAP products
        InitializeIAPProducts();

        // Initialize ad networks (AdMob, etc)
        // TODO: Initialize Firebase Ads
        // FirebaseApp.CheckAndFixAsync().ContinueWith(task => {
        //     MobileAds.Initialize("ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy");
        // });

        isInitialized = true;
        Debug.Log("[Monetization] System initialized");
    }

    /// <summary>
    /// Set up all IAP products
    /// </summary>
    private void InitializeIAPProducts()
    {
        iapProducts.Clear();

        // Power-ups
        iapProducts.Add(new IAPProduct
        {
            productId = "hint_pack_5",
            displayName = "Hint Pack (5)",
            description = "Get 5 extra hints",
            price = 0.99f,
            category = "powerup",
            quantity = 5
        });

        iapProducts.Add(new IAPProduct
        {
            productId = "reveal_tile_3",
            displayName = "Reveal Tile (3)",
            description = "Reveal 3 tiles automatically",
            price = 0.99f,
            category = "powerup",
            quantity = 3
        });

        iapProducts.Add(new IAPProduct
        {
            productId = "skip_level",
            displayName = "Skip Level",
            description = "Skip the current level",
            price = 1.99f,
            category = "powerup",
            quantity = 1
        });

        // Cosmetics
        iapProducts.Add(new IAPProduct
        {
            productId = "character_skin_astronaut",
            displayName = "Astronaut Character",
            description = "Cool astronaut character skin",
            price = 2.99f,
            category = "cosmetic",
            quantity = 1
        });

        iapProducts.Add(new IAPProduct
        {
            productId = "theme_neon",
            displayName = "Neon Theme",
            description = "Futuristic neon board theme",
            price = 1.99f,
            category = "cosmetic",
            quantity = 1
        });

        // Boosters
        iapProducts.Add(new IAPProduct
        {
            productId = "booster_2x_score_7days",
            displayName = "2x Score (7 days)",
            description = "Double score for 7 days",
            price = 4.99f,
            category = "booster",
            quantity = 1
        });

        iapProducts.Add(new IAPProduct
        {
            productId = "booster_unlimited_hints_3days",
            displayName = "Unlimited Hints (3 days)",
            description = "Use unlimited hints for 3 days",
            price = 2.99f,
            category = "booster",
            quantity = 1
        });

        // Starter Pack (discount)
        iapProducts.Add(new IAPProduct
        {
            productId = "starter_pack",
            displayName = "Starter Pack",
            description = "Great starter value: 100 coins + hint pack",
            price = 4.99f,
            category = "starter_pack",
            quantity = 1
        });

        // Gem packs
        iapProducts.Add(new IAPProduct
        {
            productId = "gems_100",
            displayName = "100 Gems",
            description = "100 premium gems",
            price = 0.99f,
            category = "currency",
            quantity = 100
        });

        iapProducts.Add(new IAPProduct
        {
            productId = "gems_550",
            displayName = "550 Gems (Best Value)",
            description = "550 premium gems with bonus",
            price = 4.99f,
            category = "currency",
            quantity = 550
        });

        Debug.Log($"[IAP] Initialized {iapProducts.Count} products");
    }

    /// <summary>
    /// Try to purchase a product
    /// </summary>
    public void PurchaseProduct(string productId)
    {
        if (!isInitialized)
        {
            Debug.LogError("[IAP] Monetization system not initialized");
            OnPurchaseFailed?.Invoke(productId);
            return;
        }

        IAPProduct product = iapProducts.Find(p => p.productId == productId);
        if (product == null)
        {
            Debug.LogError($"[IAP] Product {productId} not found");
            OnPurchaseFailed?.Invoke(productId);
            return;
        }

        // TODO: Call native IAP implementation
        // Platform-specific: Google Play Billing, App Store, etc.

        // For now, simulate purchase
        SimulatePurchase(product);
    }

    /// <summary>
    /// Simulate purchase (for testing)
    /// </summary>
    private void SimulatePurchase(IAPProduct product)
    {
        Debug.Log($"[IAP] Simulating purchase: {product.productId}");

        switch (product.category)
        {
            case "powerup":
                if (product.productId.Contains("hint"))
                    AddHints(product.quantity);
                else if (product.productId.Contains("reveal"))
                    AddCoins(product.quantity * 50); // Simulate coin cost
                else if (product.productId.Contains("skip"))
                    AddCoins(100);
                break;

            case "cosmetic":
                playerCurrency.unlockedCosmetics[product.productId] = 1;
                break;

            case "booster":
                // Booster active for X days
                int duration = product.productId.Contains("7days") ? 7 : 3;
                playerCurrency.activeBoosters[product.productId] = (int)(Time.time + (duration * 86400));
                break;

            case "starter_pack":
                AddCoins(100);
                AddHints(5);
                break;

            case "currency":
                AddGems(product.quantity);
                break;
        }

        // Log analytics
        Analytics.Instance.LogIAPPurchase(product.productId, product.price, "USD", System.Guid.NewGuid().ToString());

        OnPurchaseSuccess?.Invoke(product.productId, product.quantity);
        SavePlayerCurrency();
    }

    /// <summary>
    /// Show rewarded ad and give reward
    /// </summary>
    public void ShowRewardedAd(string rewardType, int rewardAmount)
    {
        // TODO: Show AdMob rewarded video
        // RewardedVideo.Instance.LoadAndShowRewardedVideo(...);

        // For now, simulate
        Debug.Log($"[Ads] Showing rewarded ad for {rewardType}");
        SimulateAdReward(rewardType, rewardAmount);
    }

    private void SimulateAdReward(string rewardType, int rewardAmount)
    {
        switch (rewardType)
        {
            case "coins":
                AddCoins(rewardAmount);
                break;
            case "gems":
                AddGems(rewardAmount);
                break;
            case "hints":
                AddHints(rewardAmount);
                break;
        }

        Analytics.Instance.LogAdReward("admob", rewardType, rewardAmount);
        Debug.Log($"[Ads] Reward granted: {rewardAmount} {rewardType}");
    }

    /// <summary>
    /// Currency management
    /// </summary>
    public void AddCoins(int amount)
    {
        playerCurrency.coins += amount;
        OnCoinsEarned?.Invoke(amount);
    }

    public void AddGems(int amount)
    {
        playerCurrency.gems += amount;
        OnGemsEarned?.Invoke(amount);
    }

    public void AddHints(int amount)
    {
        playerCurrency.hints += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (playerCurrency.coins >= amount)
        {
            playerCurrency.coins -= amount;
            SavePlayerCurrency();
            return true;
        }
        return false;
    }

    public bool SpendGems(int amount)
    {
        if (playerCurrency.gems >= amount)
        {
            playerCurrency.gems -= amount;
            SavePlayerCurrency();
            return true;
        }
        return false;
    }

    public bool UseHint()
    {
        if (playerCurrency.hints > 0)
        {
            playerCurrency.hints--;
            SavePlayerCurrency();
            return true;
        }
        return false;
    }

    public int GetCoins() => playerCurrency.coins;
    public int GetGems() => playerCurrency.gems;
    public int GetHints() => playerCurrency.hints;

    /// <summary>
    /// Get list of all IAP products
    /// </summary>
    public List<IAPProduct> GetIAPProducts() => new List<IAPProduct>(iapProducts);

    public IAPProduct GetProduct(string productId) => iapProducts.Find(p => p.productId == productId);

    /// <summary>
    /// Check if player has unlocked cosmetic
    /// </summary>
    public bool HasCosmetic(string cosmeticId) => playerCurrency.unlockedCosmetics.ContainsKey(cosmeticId);

    /// <summary>
    /// Check if booster is active
    /// </summary>
    public bool IsBoosterActive(string boosterId)
    {
        if (!playerCurrency.activeBoosters.ContainsKey(boosterId))
            return false;

        int expiryTime = playerCurrency.activeBoosters[boosterId];
        return (int)Time.time < expiryTime;
    }

    private void LoadPlayerCurrency()
    {
        string json = PlayerPrefs.GetString("PlayerCurrency", "");
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                playerCurrency = JsonUtility.FromJson<PlayerCurrency>(json);
            }
            catch
            {
                Debug.LogWarning("Failed to load player currency");
                playerCurrency = new PlayerCurrency();
            }
        }
    }

    private void SavePlayerCurrency()
    {
        string json = JsonUtility.ToJson(playerCurrency);
        PlayerPrefs.SetString("PlayerCurrency", json);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SavePlayerCurrency();
    }
}
