using UnityEngine;
using System.Collections.Generic;
using System;
using System.Globalization;

/// <summary>
/// Analytics system for tracking user engagement, retention, and monetization
/// Supports Firebase Analytics, local logging, dashboard export, and integration with RetentionTracker
/// 
/// INTEGRATION: Works alongside RetentionTracker for comprehensive metrics:
/// - Analytics.cs: High-level event logging and Firebase integration
/// - RetentionTracker.cs: Detailed user lifecycle and retention calculations
/// - DataExporter.cs: Export and reporting
/// - AnalyticsDashboard.cs: UI visualization
/// </summary>
public class Analytics : MonoBehaviour
{
    [System.Serializable]
    public class SessionData
    {
        public string sessionId;
        public long sessionStartTime;
        public long sessionEndTime;
        public float sessionDurationSeconds;
        public int levelPlayedCount;
        public int levelCompletedCount;
        public int totalScore;
    }

    [System.Serializable]
    public class LevelAnalytics
    {
        public int levelId;
        public int playCount;
        public int completionCount;
        public float completionRate;
        public float averageScore;
        public float averageMoves;
        public float averageTime;
    }

    public static Analytics Instance { get; private set; }

    private SessionData currentSession;
    private List<LevelAnalytics> levelStats = new List<LevelAnalytics>();
    private Dictionary<string, object> customEvents = new Dictionary<string, object>();

    // Retention tracking
    private string lastLoginDate;
    private int loginStreak = 0;
    private List<string> allLoginDates = new List<string>();

    // Integration with RetentionTracker
    private RetentionTracker retentionTracker;

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
        retentionTracker = RetentionTracker.Instance;
        StartNewSession();
        LoadAnalyticsData();
        CheckDailyLogin();
    }

    /// <summary>
    /// Start a new session
    /// </summary>
    private void StartNewSession()
    {
        currentSession = new SessionData
        {
            sessionId = System.Guid.NewGuid().ToString(),
            sessionStartTime = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            levelPlayedCount = 0,
            levelCompletedCount = 0,
            totalScore = 0
        };

        Debug.Log($"[Analytics] New session started: {currentSession.sessionId}");
    }

    /// <summary>
    /// Log when user plays a level
    /// </summary>
    public void LogLevelStart(int levelId)
    {
        currentSession.levelPlayedCount++;

        // Ensure level stats exist
        if (!levelStats.Exists(l => l.levelId == levelId))
        {
            levelStats.Add(new LevelAnalytics { levelId = levelId });
        }

        var levelStat = levelStats.Find(l => l.levelId == levelId);
        levelStat.playCount++;

        Debug.Log($"[Analytics] Level {levelId} started");
    }

    /// <summary>
    /// Log when user completes a level
    /// </summary>
    public void LogLevelComplete(int levelId, int score, int stars, int moveCount, float timeSeconds)
    {
        currentSession.levelCompletedCount++;
        currentSession.totalScore += score;

        var levelStat = levelStats.Find(l => l.levelId == levelId);
        if (levelStat != null)
        {
            levelStat.completionCount++;
            levelStat.completionRate = (float)levelStat.completionCount / levelStat.playCount;
            levelStat.averageScore = (levelStat.averageScore + score) / 2f;
            levelStat.averageMoves = (levelStat.averageMoves + moveCount) / 2f;
            levelStat.averageTime = (levelStat.averageTime + timeSeconds) / 2f;
        }

        LogCustomEvent("level_complete", new Dictionary<string, object>
        {
            { "level_id", levelId },
            { "score", score },
            { "stars", stars },
            { "moves", moveCount },
            { "time_seconds", timeSeconds }
        });

        Debug.Log($"[Analytics] Level {levelId} completed with {stars} stars and {score} points");
    }

    /// <summary>
    /// Log when user fails/retries a level
    /// </summary>
    public void LogLevelRetry(int levelId)
    {
        LogCustomEvent("level_retry", new Dictionary<string, object>
        {
            { "level_id", levelId }
        });
    }

    /// <summary>
    /// Log when user uses a hint
    /// </summary>
    public void LogHintUsed(int levelId, int hintCount)
    {
        LogCustomEvent("hint_used", new Dictionary<string, object>
        {
            { "level_id", levelId },
            { "hint_count", hintCount }
        });
    }

    /// <summary>
    /// Log IAP transaction
    /// </summary>
    public void LogIAPPurchase(string productId, float price, string currency, string purchaseToken)
    {
        LogCustomEvent("iap_purchase", new Dictionary<string, object>
        {
            { "product_id", productId },
            { "price", price },
            { "currency", currency },
            { "purchase_token", purchaseToken },
            { "timestamp", System.DateTime.UtcNow.ToString("O") }
        });

        Debug.Log($"[Analytics] IAP Purchase: {productId} for {price} {currency}");
    }

    /// <summary>
    /// Log ad impression
    /// </summary>
    public void LogAdImpression(string adNetwork, string adFormat)
    {
        LogCustomEvent("ad_impression", new Dictionary<string, object>
        {
            { "network", adNetwork },
            { "format", adFormat },
            { "timestamp", System.DateTime.UtcNow.ToString("O") }
        });
    }

    /// <summary>
    /// Log ad reward (user watched ad and received reward)
    /// </summary>
    public void LogAdReward(string adNetwork, string rewardType, int rewardAmount)
    {
        LogCustomEvent("ad_reward", new Dictionary<string, object>
        {
            { "network", adNetwork },
            { "reward_type", rewardType },
            { "reward_amount", rewardAmount }
        });
    }

    /// <summary>
    /// Log custom event
    /// </summary>
    public void LogCustomEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        if (parameters == null)
            parameters = new Dictionary<string, object>();

        Debug.Log($"[Analytics] Event: {eventName}");
        foreach (var kvp in parameters)
        {
            Debug.Log($"  - {kvp.Key}: {kvp.Value}");
        }

        // TODO: Send to Firebase Analytics
        // FirebaseAnalytics.LogEvent(eventName, parameters);
    }

    /// <summary>
    /// Track daily login for retention metrics
    /// </summary>
    private void CheckDailyLogin()
    {
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        string lastLogin = PlayerPrefs.GetString("LastLoginDate", "");

        if (lastLogin != today)
        {
            allLoginDates.Add(today);
            PlayerPrefs.SetString("LastLoginDate", today);

            if (lastLogin != "")
            {
                System.DateTime lastDate = System.DateTime.ParseExact(lastLogin, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                System.DateTime todayDate = System.DateTime.ParseExact(today, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                if ((todayDate - lastDate).TotalDays == 1)
                {
                    loginStreak = PlayerPrefs.GetInt("LoginStreak", 0) + 1;
                }
                else
                {
                    loginStreak = 1;
                }
            }
            else
            {
                loginStreak = 1;
            }

            PlayerPrefs.SetInt("LoginStreak", loginStreak);
            PlayerPrefs.Save();

            LogCustomEvent("daily_active_user", new Dictionary<string, object>
            {
                { "login_date", today },
                { "login_streak", loginStreak }
            });

            Debug.Log($"[Analytics] Daily login recorded. Streak: {loginStreak}");
        }
    }

    /// <summary>
    /// Get session summary
    /// </summary>
    public string GetSessionSummary()
    {
        return $"Session {currentSession.sessionId}\n" +
               $"Duration: {(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - currentSession.sessionStartTime) / 1000}s\n" +
               $"Levels Played: {currentSession.levelPlayedCount}\n" +
               $"Levels Completed: {currentSession.levelCompletedCount}\n" +
               $"Total Score: {currentSession.totalScore}";
    }

    /// <summary>
    /// Get level statistics
    /// </summary>
    public LevelAnalytics GetLevelStats(int levelId)
    {
        return levelStats.Find(l => l.levelId == levelId);
    }

    /// <summary>
    /// Get all level statistics
    /// </summary>
    public List<LevelAnalytics> GetAllLevelStats()
    {
        return new List<LevelAnalytics>(levelStats);
    }

    /// <summary>
    /// Get retention data
    /// </summary>
    public Dictionary<string, int> GetRetentionMetrics()
    {
        int day1Retention = 0, day7Retention = 0, day30Retention = 0;

        // Calculate based on login dates
        if (allLoginDates.Count > 0)
        {
            string firstLogin = allLoginDates[0];
            foreach (var loginDate in allLoginDates)
            {
                System.DateTime first = System.DateTime.ParseExact(firstLogin, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                System.DateTime current = System.DateTime.ParseExact(loginDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                int daysDiff = (int)(current - first).TotalDays;

                if (daysDiff == 1) day1Retention++;
                if (daysDiff == 7) day7Retention++;
                if (daysDiff == 30) day30Retention++;
            }
        }

        return new Dictionary<string, int>
        {
            { "d1", day1Retention },
            { "d7", day7Retention },
            { "d30", day30Retention },
            { "login_streak", loginStreak }
        };
    }

    private void LoadAnalyticsData()
    {
        // Load from PlayerPrefs
        lastLoginDate = PlayerPrefs.GetString("LastLoginDate", "");
        loginStreak = PlayerPrefs.GetInt("LoginStreak", 0);
    }

    /// <summary>
    /// Integration with RetentionTracker - sync level start event
    /// </summary>
    public void SyncLevelStartToRetention(int levelId)
    {
        if (retentionTracker != null)
        {
            retentionTracker.LogLevelStart(levelId);
        }
    }

    /// <summary>
    /// Integration with RetentionTracker - sync level complete event
    /// </summary>
    public void SyncLevelCompleteToRetention(int levelId, int moveCount, float timeSeconds, bool withHint)
    {
        if (retentionTracker != null)
        {
            retentionTracker.LogLevelComplete(levelId, moveCount, timeSeconds, withHint);
        }
    }

    /// <summary>
    /// Integration with RetentionTracker - sync hint usage
    /// </summary>
    public void SyncHintUsageToRetention(int levelId)
    {
        if (retentionTracker != null)
        {
            retentionTracker.LogHintUsed(levelId);
        }
    }

    /// <summary>
    /// Integration with RetentionTracker - sync shop visit
    /// </summary>
    public void SyncShopVisitToRetention()
    {
        if (retentionTracker != null)
        {
            retentionTracker.LogShopVisit();
        }
    }

    /// <summary>
    /// Integration with RetentionTracker - sync ad watch
    /// </summary>
    public void SyncAdWatchToRetention(string adNetwork)
    {
        if (retentionTracker != null)
        {
            retentionTracker.LogAdWatch(adNetwork);
        }
    }

    /// <summary>
    /// Integration with RetentionTracker - sync IAP purchase
    /// </summary>
    public void SyncIAPToRetention(string productId, float price)
    {
        if (retentionTracker != null)
        {
            retentionTracker.LogIAPPurchase(productId, price);
        }
    }

    /// <summary>
    /// Integration with RetentionTracker - sync session quit
    /// </summary>
    public void SyncSessionQuitToRetention(int currentLevel)
    {
        if (retentionTracker != null)
        {
            retentionTracker.LogSessionQuit(currentLevel);
        }
    }

    private void OnApplicationQuit()
    {
        // End session and save data
        currentSession.sessionEndTime = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        currentSession.sessionDurationSeconds = (currentSession.sessionEndTime - currentSession.sessionStartTime) / 1000f;

        LogCustomEvent("session_end", new Dictionary<string, object>
        {
            { "duration_seconds", currentSession.sessionDurationSeconds },
            { "levels_played", currentSession.levelPlayedCount },
            { "levels_completed", currentSession.levelCompletedCount },
            { "total_score", currentSession.totalScore }
        });

        // Sync with retention tracker
        if (retentionTracker != null)
        {
            retentionTracker.EndSession();
            retentionTracker.CheckChurnStatus();
        }

        SaveAnalyticsData();
    }

    private void SaveAnalyticsData()
    {
        // Save to PlayerPrefs or JSON
        PlayerPrefs.SetString("LastSessionData", JsonUtility.ToJson(currentSession));
        PlayerPrefs.Save();
    }
}
