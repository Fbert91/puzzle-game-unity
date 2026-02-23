using UnityEngine;
using System;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif

/// <summary>
/// AdManager - Rewarded + Interstitial ad system (banners removed)
/// Requires Google Mobile Ads SDK. Install via Unity Package Manager or .unitypackage,
/// then add GOOGLE_MOBILE_ADS to Player Settings > Scripting Define Symbols.
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

    #region Ad Unit IDs
    private string _appID = "ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy";
    private string _rewardedAdUnitID = "ca-app-pub-3940256099942544/5224354917";
    private string _interstitialAdUnitID = "ca-app-pub-3940256099942544/1033173712";
    #endregion

    #region Ad Objects
#if GOOGLE_MOBILE_ADS
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;
#endif
    #endregion

    #region Events
    public event Action OnRewardedAdClosed;
    public event Action OnInterstitialClosed;
    #endregion

    #region Properties
    public bool IsRewardedLoaded { get; private set; }
    public bool IsInterstitialLoaded { get; private set; }
    #endregion

    #region Session Tracking
    private int interstitialAdsShownThisSession = 0;
    private int maxInterstitialsPerSession = 3;
    private int levelsCompletedThisSession = 0;
    private int failCountForCurrentLevel = 0;
    private bool isFirstSession = false;
    private const string FIRST_SESSION_KEY = "AdManager_FirstSessionDone";
    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        isFirstSession = PlayerPrefs.GetInt(FIRST_SESSION_KEY, 0) == 0;
#if GOOGLE_MOBILE_ADS
        InitializeAdMob();
        LoadRewardedAd();
        LoadInterstitialAd();
#else
        Debug.LogWarning("[AdManager] Google Mobile Ads SDK not installed. Ads disabled.");
#endif
    }

#if GOOGLE_MOBILE_ADS
    #region Initialization
    public void InitializeAdMob()
    {
        if (MobileAds.IsInitialized) return;
        MobileAds.Initialize(initStatus => { Debug.Log("[AdManager] AdMob initialized"); });
    }
    #endregion

    #region Rewarded Ad
    private void LoadRewardedAd()
    {
        var adRequest = new AdRequest();
        RewardedAd.Load(_rewardedAdUnitID, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null) { IsRewardedLoaded = false; Debug.LogWarning($"[AdManager] Rewarded Ad Failed: {error.GetMessage()}"); return; }
            _rewardedAd = ad;
            IsRewardedLoaded = true;
            _rewardedAd.OnAdFullScreenContentClosed += () => { OnRewardedAdClosed?.Invoke(); LoadRewardedAd(); };
            _rewardedAd.OnAdFullScreenContentFailed += (AdError err) => { LoadRewardedAd(); };
            Debug.Log("[AdManager] Rewarded Ad Loaded");
        });
    }

    public void ShowRewardedAd(System.Action<Reward> onRewardCallback)
    {
        if (IsRewardedLoaded && _rewardedAd != null)
        {
            _rewardedAd.Show(reward =>
            {
                onRewardCallback?.Invoke(reward);
                if (Analytics.Instance != null) Analytics.Instance.LogAdReward("admob", reward.Type, (int)reward.Amount);
            });
        }
        else { Debug.LogWarning("[AdManager] Rewarded Ad not loaded"); LoadRewardedAd(); }
    }
    #endregion

    #region Interstitial Ad
    private void LoadInterstitialAd()
    {
        var adRequest = new AdRequest();
        InterstitialAd.Load(_interstitialAdUnitID, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null) { IsInterstitialLoaded = false; return; }
            _interstitialAd = ad;
            IsInterstitialLoaded = true;
            _interstitialAd.OnAdFullScreenContentClosed += () => { OnInterstitialClosed?.Invoke(); LoadInterstitialAd(); };
            _interstitialAd.OnAdFullScreenContentFailed += (AdError err) => { LoadInterstitialAd(); };
        });
    }
    #endregion
#else
    public void ShowRewardedAd(System.Action<object> onRewardCallback)
    {
        Debug.LogWarning("[AdManager] Ads not available - SDK not installed");
    }
#endif

    public bool TryShowInterstitial()
    {
        if (isFirstSession) return false;
        if (interstitialAdsShownThisSession >= maxInterstitialsPerSession) return false;
        if (levelsCompletedThisSession % 5 != 0 || levelsCompletedThisSession == 0) return false;
#if GOOGLE_MOBILE_ADS
        if (IsInterstitialLoaded && _interstitialAd != null)
        {
            _interstitialAd.Show();
            interstitialAdsShownThisSession++;
            if (Analytics.Instance != null) Analytics.Instance.LogAdImpression("admob", "interstitial");
            return true;
        }
#endif
        return false;
    }

    #region Integration Points
    public void OnLevelCompleted() { levelsCompletedThisSession++; failCountForCurrentLevel = 0; }
    public void OnLevelFailed() { failCountForCurrentLevel++; }
    public bool ShouldOfferHintAd() { return failCountForCurrentLevel >= 3 && IsRewardedLoaded; }

    public void ShowHintRewardAd(System.Action onHintGranted)
    {
#if GOOGLE_MOBILE_ADS
        ShowRewardedAd(reward => { onHintGranted?.Invoke(); });
#else
        Debug.LogWarning("[AdManager] Ads not available"); onHintGranted?.Invoke();
#endif
    }

    public void ShowDoubleDailyRewardAd(System.Action onDoubleReward)
    {
#if GOOGLE_MOBILE_ADS
        ShowRewardedAd(reward => { onDoubleReward?.Invoke(); });
#else
        Debug.LogWarning("[AdManager] Ads not available"); onDoubleReward?.Invoke();
#endif
    }

    public void ShowStreakFreezeAd(System.Action onStreakFreezed)
    {
#if GOOGLE_MOBILE_ADS
        ShowRewardedAd(reward => { onStreakFreezed?.Invoke(); });
#else
        Debug.LogWarning("[AdManager] Ads not available"); onStreakFreezed?.Invoke();
#endif
    }

    public void MarkFirstSessionComplete()
    {
        if (isFirstSession) { isFirstSession = false; PlayerPrefs.SetInt(FIRST_SESSION_KEY, 1); PlayerPrefs.Save(); }
    }

    public void ResetSessionCounters() { interstitialAdsShownThisSession = 0; levelsCompletedThisSession = 0; failCountForCurrentLevel = 0; }
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
#if GOOGLE_MOBILE_ADS
        _rewardedAd?.Destroy();
        _interstitialAd?.Destroy();
#endif
    }
    private void OnApplicationQuit() { MarkFirstSessionComplete(); }
    #endregion

    public int GetInterstitialsShown() => interstitialAdsShownThisSession;
    public int GetLevelsCompletedThisSession() => levelsCompletedThisSession;
    public int GetFailCount() => failCountForCurrentLevel;
    public bool IsFirstSession() => isFirstSession;

    public void SetAdUnitIDs(string appID, string rewardedID, string interstitialID)
    { _appID = appID; _rewardedAdUnitID = rewardedID; _interstitialAdUnitID = interstitialID; }
}
