using UnityEngine;
using GoogleMobileAds.Api;
using System;

/// <summary>
/// AdManager - Singleton for managing all advertisements (Banner, Rewarded, Interstitial)
/// Integrates Google Mobile Ads SDK for Unity (AdMob)
/// 
/// Usage:
///   - Add to scene as singleton
///   - Initialize with your ad unit IDs
///   - Show ads via AdManager.Instance.Show*() methods
/// </summary>
public class AdManager : MonoBehaviour
{
    #region Singleton
    private static AdManager _instance;
    public static AdManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("AdManager");
                _instance = obj.AddComponent<AdManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    #endregion

    #region Ad Unit IDs (Configuration)
    // TODO: Replace with your actual AdMob app ID and ad unit IDs
    private string _appID = "ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy"; // Your AdMob App ID
    private string _bannerAdUnitID = "ca-app-pub-3940256099942544/6300978111"; // Test Banner ID
    private string _rewardedAdUnitID = "ca-app-pub-3940256099942544/5224354917"; // Test Rewarded ID
    private string _interstitialAdUnitID = "ca-app-pub-3940256099942544/1033173712"; // Test Interstitial ID
    #endregion

    #region Ad Objects
    private BannerView _bannerView;
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;
    #endregion

    #region Events
    public event Action OnRewardedAdClosed;
    public event Action<Reward> OnRewardEarned;
    public event Action OnInterstitialClosed;
    public event Action OnBannerLoaded;
    #endregion

    #region Properties
    public bool IsBannerLoaded { get; private set; }
    public bool IsRewardedLoaded { get; private set; }
    public bool IsInterstitialLoaded { get; private set; }
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
    }

    private void Start()
    {
        InitializeAdMob();
        LoadBannerAd();
        LoadRewardedAd();
        LoadInterstitialAd();
    }

    #region Initialization
    /// <summary>Initialize Google Mobile Ads SDK</summary>
    public void InitializeAdMob()
    {
        if (MobileAds.IsInitialized) return;

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("[AdManager] AdMob initialized");
            Debug.Log($"[AdManager] Status: {initStatus}");
        });
    }

    /// <summary>Set AD Unit IDs (Production)</summary>
    public void SetAdUnitIDs(string appID, string bannerID, string rewardedID, string interstitialID)
    {
        _appID = appID;
        _bannerAdUnitID = bannerID;
        _rewardedAdUnitID = rewardedID;
        _interstitialAdUnitID = interstitialID;
        Debug.Log("[AdManager] Ad unit IDs updated");
    }
    #endregion

    #region Banner Ad
    /// <summary>Load Banner Ad at bottom of screen</summary>
    private void LoadBannerAd()
    {
        // Create banner view
        _bannerView = new BannerView(_bannerAdUnitID, AdSize.Banner, AdPosition.Bottom);

        // Register callback events
        _bannerView.OnBannerAdLoaded += BannerView_OnBannerAdLoaded;
        _bannerView.OnBannerAdLoadFailed += BannerView_OnBannerAdLoadFailed;

        // Create ad request
        var adRequest = new AdRequest();

        // Load ad
        _bannerView.LoadAd(adRequest);
        Debug.Log("[AdManager] Loading Banner Ad...");
    }

    private void BannerView_OnBannerAdLoaded()
    {
        IsBannerLoaded = true;
        OnBannerLoaded?.Invoke();
        Debug.Log("[AdManager] Banner Ad Loaded Successfully");
    }

    private void BannerView_OnBannerAdLoadFailed(LoadAdError error)
    {
        IsBannerLoaded = false;
        Debug.LogWarning($"[AdManager] Banner Ad Failed: {error.GetMessage()}");
    }

    public void ShowBannerAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Show();
            Debug.Log("[AdManager] Showing Banner Ad");
        }
    }

    public void HideBannerAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Hide();
            Debug.Log("[AdManager] Hiding Banner Ad");
        }
    }

    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
            Debug.Log("[AdManager] Banner Ad Destroyed");
        }
    }
    #endregion

    #region Rewarded Ad
    /// <summary>Load Rewarded Video Ad (watch for coins)</summary>
    private void LoadRewardedAd()
    {
        var adRequest = new AdRequest();

        RewardedAd.Load(_rewardedAdUnitID, adRequest, OnRewardedAdLoaded);
        Debug.Log("[AdManager] Loading Rewarded Ad...");
    }

    private void OnRewardedAdLoaded(RewardedAd ad, LoadAdError error)
    {
        if (error != null)
        {
            IsRewardedLoaded = false;
            Debug.LogWarning($"[AdManager] Rewarded Ad Failed: {error.GetMessage()}");
            return;
        }

        _rewardedAd = ad;
        IsRewardedLoaded = true;

        // Register callbacks
        _rewardedAd.OnAdFullScreenContentOpened += RewardedAd_OnAdFullScreenContentOpened;
        _rewardedAd.OnAdFullScreenContentClosed += RewardedAd_OnAdFullScreenContentClosed;
        _rewardedAd.OnAdFullScreenContentFailed += RewardedAd_OnAdFullScreenContentFailed;
        _rewardedAd.OnAdClicked += RewardedAd_OnAdClicked;
        _rewardedAd.OnAdImpressionRecorded += RewardedAd_OnAdImpressionRecorded;

        Debug.Log("[AdManager] Rewarded Ad Loaded");
    }

    private void RewardedAd_OnAdFullScreenContentOpened()
    {
        Debug.Log("[AdManager] Rewarded Ad Opened");
    }

    private void RewardedAd_OnAdFullScreenContentClosed()
    {
        Debug.Log("[AdManager] Rewarded Ad Closed");
        OnRewardedAdClosed?.Invoke();
        LoadRewardedAd(); // Load next one
    }

    private void RewardedAd_OnAdFullScreenContentFailed(AdError error)
    {
        Debug.LogWarning($"[AdManager] Rewarded Ad Failed: {error}");
        LoadRewardedAd();
    }

    private void RewardedAd_OnAdClicked()
    {
        Debug.Log("[AdManager] Rewarded Ad Clicked");
    }

    private void RewardedAd_OnAdImpressionRecorded()
    {
        Debug.Log("[AdManager] Rewarded Ad Impression Recorded");
    }

    /// <summary>Show Rewarded Ad and execute callback on reward</summary>
    public void ShowRewardedAd(System.Action<Reward> onRewardCallback)
    {
        if (IsRewardedLoaded && _rewardedAd != null)
        {
            _rewardedAd.Show(reward =>
            {
                Debug.Log($"[AdManager] User earned reward: {reward.Amount} {reward.Type}");
                OnRewardEarned?.Invoke(reward);
                onRewardCallback?.Invoke(reward);
            });
        }
        else
        {
            Debug.LogWarning("[AdManager] Rewarded Ad not loaded yet");
            LoadRewardedAd();
        }
    }
    #endregion

    #region Interstitial Ad
    /// <summary>Load Interstitial Ad (full screen)</summary>
    private void LoadInterstitialAd()
    {
        var adRequest = new AdRequest();

        InterstitialAd.Load(_interstitialAdUnitID, adRequest, OnInterstitialAdLoaded);
        Debug.Log("[AdManager] Loading Interstitial Ad...");
    }

    private void OnInterstitialAdLoaded(InterstitialAd ad, LoadAdError error)
    {
        if (error != null)
        {
            IsInterstitialLoaded = false;
            Debug.LogWarning($"[AdManager] Interstitial Ad Failed: {error.GetMessage()}");
            return;
        }

        _interstitialAd = ad;
        IsInterstitialLoaded = true;

        // Register callbacks
        _interstitialAd.OnAdFullScreenContentOpened += InterstitialAd_OnAdFullScreenContentOpened;
        _interstitialAd.OnAdFullScreenContentClosed += InterstitialAd_OnAdFullScreenContentClosed;
        _interstitialAd.OnAdFullScreenContentFailed += InterstitialAd_OnAdFullScreenContentFailed;

        Debug.Log("[AdManager] Interstitial Ad Loaded");
    }

    private void InterstitialAd_OnAdFullScreenContentOpened()
    {
        Debug.Log("[AdManager] Interstitial Ad Opened");
    }

    private void InterstitialAd_OnAdFullScreenContentClosed()
    {
        Debug.Log("[AdManager] Interstitial Ad Closed");
        OnInterstitialClosed?.Invoke();
        LoadInterstitialAd(); // Load next one
    }

    private void InterstitialAd_OnAdFullScreenContentFailed(AdError error)
    {
        Debug.LogWarning($"[AdManager] Interstitial Ad Failed: {error}");
        LoadInterstitialAd();
    }

    /// <summary>Show Interstitial Ad (after level or in menu)</summary>
    public void ShowInterstitialAd()
    {
        if (IsInterstitialLoaded && _interstitialAd != null)
        {
            _interstitialAd.Show();
            Debug.Log("[AdManager] Showing Interstitial Ad");
        }
        else
        {
            Debug.LogWarning("[AdManager] Interstitial Ad not loaded");
            LoadInterstitialAd();
        }
    }
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        DestroyBannerAd();
        
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
        }

        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
        }

        Debug.Log("[AdManager] Cleaned up all ads");
    }
    #endregion
}
