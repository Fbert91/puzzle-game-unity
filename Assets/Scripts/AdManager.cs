using UnityEngine;
using GoogleMobileAds.Api;
using System;

/// <summary>
/// AdManager - Rewarded + Interstitial ad system (banners removed)
/// Rewarded ads: after 3+ fails, daily bonus 2x, streak freeze
/// Interstitial ads: every 5 levels, max 3/session, never first session
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
    private string _rewardedAdUnitID = "ca-app-pub-3940256099942544/5224354917";     // Test Rewarded
    private string _interstitialAdUnitID = "ca-app-pub-3940256099942544/1033173712"; // Test Interstitial
    #endregion

    #region Ad Objects
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;
    #endregion

    #region Events
    public event Action OnRewardedAdClosed;
    public event Action<Reward> OnRewardEarned;
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
        isFirstSession = PlayerPrefs.GetInt(FIRST_SESSION_KEY, 0) == 0;
        InitializeAdMob();
        LoadRewardedAd();
        LoadInterstitialAd();
    }

    #region Initialization
    public void InitializeAdMob()
    {
        if (MobileAds.IsInitialized) return;

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("[AdManager] AdMob initialized (no banners)");
        });
    }

    public void SetAdUnitIDs(string appID, string rewardedID, string interstitialID)
    {
        _appID = appID;
        _rewardedAdUnitID = rewardedID;
        _interstitialAdUnitID = interstitialID;
    }
    #endregion

    #region Rewarded Ad
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

        _rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdManager] Rewarded Ad Closed");
            OnRewardedAdClosed?.Invoke();
            LoadRewardedAd();
        };

        _rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
        {
            Debug.LogWarning($"[AdManager] Rewarded Ad Failed: {err}");
            LoadRewardedAd();
        };

        Debug.Log("[AdManager] Rewarded Ad Loaded");
    }

    /// <summary>
    /// Show rewarded ad with callback
    /// </summary>
    public void ShowRewardedAd(System.Action<Reward> onRewardCallback)
    {
        if (IsRewardedLoaded && _rewardedAd != null)
        {
            _rewardedAd.Show(reward =>
            {
                Debug.Log($"[AdManager] Reward earned: {reward.Amount} {reward.Type}");
                OnRewardEarned?.Invoke(reward);
                onRewardCallback?.Invoke(reward);

                // Log analytics
                if (Analytics.Instance != null)
                    Analytics.Instance.LogAdReward("admob", reward.Type, (int)reward.Amount);
            });
        }
        else
        {
            Debug.LogWarning("[AdManager] Rewarded Ad not loaded");
            LoadRewardedAd();
        }
    }
    #endregion

    #region Interstitial Ad
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
            Debug.LogWarning($"[AdManager] Interstitial Failed: {error.GetMessage()}");
            return;
        }

        _interstitialAd = ad;
        IsInterstitialLoaded = true;

        _interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            OnInterstitialClosed?.Invoke();
            LoadInterstitialAd();
        };

        _interstitialAd.OnAdFullScreenContentFailed += (AdError err) =>
        {
            LoadInterstitialAd();
        };

        Debug.Log("[AdManager] Interstitial Loaded");
    }

    /// <summary>
    /// Try to show interstitial ad (respects session limits)
    /// Only between level sets (every 5 levels), max 3/session, never first session
    /// </summary>
    public bool TryShowInterstitial()
    {
        // Never on first session
        if (isFirstSession)
        {
            Debug.Log("[AdManager] First session - no interstitials");
            return false;
        }

        // Max per session
        if (interstitialAdsShownThisSession >= maxInterstitialsPerSession)
        {
            Debug.Log("[AdManager] Max interstitials reached for session");
            return false;
        }

        // Only every 5 levels
        if (levelsCompletedThisSession % 5 != 0 || levelsCompletedThisSession == 0)
        {
            return false;
        }

        if (IsInterstitialLoaded && _interstitialAd != null)
        {
            _interstitialAd.Show();
            interstitialAdsShownThisSession++;

            if (Analytics.Instance != null)
                Analytics.Instance.LogAdImpression("admob", "interstitial");

            Debug.Log($"[AdManager] Interstitial shown ({interstitialAdsShownThisSession}/{maxInterstitialsPerSession})");
            return true;
        }

        return false;
    }
    #endregion

    #region Integration Points

    /// <summary>
    /// Called when a level is completed (tracks for interstitial timing)
    /// </summary>
    public void OnLevelCompleted()
    {
        levelsCompletedThisSession++;
        failCountForCurrentLevel = 0;
    }

    /// <summary>
    /// Called when player fails a level
    /// </summary>
    public void OnLevelFailed()
    {
        failCountForCurrentLevel++;
    }

    /// <summary>
    /// Check if "Watch ad for hint" should be offered (after 3+ fails)
    /// </summary>
    public bool ShouldOfferHintAd()
    {
        return failCountForCurrentLevel >= 3 && IsRewardedLoaded;
    }

    /// <summary>
    /// Show "Watch ad for hint" rewarded ad
    /// </summary>
    public void ShowHintRewardAd(System.Action onHintGranted)
    {
        ShowRewardedAd(reward =>
        {
            onHintGranted?.Invoke();
            Debug.Log("[AdManager] Hint reward granted from ad");
        });
    }

    /// <summary>
    /// Show "Watch ad for 2x daily reward"
    /// </summary>
    public void ShowDoubleDailyRewardAd(System.Action onDoubleReward)
    {
        ShowRewardedAd(reward =>
        {
            onDoubleReward?.Invoke();
            Debug.Log("[AdManager] 2x daily reward granted");
        });
    }

    /// <summary>
    /// Show "Watch ad to save streak"
    /// </summary>
    public void ShowStreakFreezeAd(System.Action onStreakFreezed)
    {
        ShowRewardedAd(reward =>
        {
            onStreakFreezed?.Invoke();
            Debug.Log("[AdManager] Streak freeze granted from ad");
        });
    }

    /// <summary>
    /// Mark first session as complete (call at end of session)
    /// </summary>
    public void MarkFirstSessionComplete()
    {
        if (isFirstSession)
        {
            isFirstSession = false;
            PlayerPrefs.SetInt(FIRST_SESSION_KEY, 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Reset session counters
    /// </summary>
    public void ResetSessionCounters()
    {
        interstitialAdsShownThisSession = 0;
        levelsCompletedThisSession = 0;
        failCountForCurrentLevel = 0;
    }
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        if (_rewardedAd != null)
            _rewardedAd.Destroy();

        if (_interstitialAd != null)
            _interstitialAd.Destroy();

        Debug.Log("[AdManager] Cleaned up ads");
    }

    private void OnApplicationQuit()
    {
        MarkFirstSessionComplete();
    }
    #endregion

    #region Getters
    public int GetInterstitialsShown() => interstitialAdsShownThisSession;
    public int GetLevelsCompletedThisSession() => levelsCompletedThisSession;
    public int GetFailCount() => failCountForCurrentLevel;
    public bool IsFirstSession() => isFirstSession;
    #endregion
}
