using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;

/// <summary>
/// RetentionTracker - Dedicated retention analytics system
/// Tracks user lifecycle, retention cohorts, churn patterns, and monetization-retention links
/// 
/// KEY METRICS TRACKED:
/// - D1, D7, D30 Retention (% players returning after 1, 7, 30 days)
/// - Cohort Analysis (grouped by install date)
/// - Session patterns (frequency, duration, timing)
/// - Feature engagement (what keeps players engaged?)
/// - Churn signals (when and why do players quit?)
/// - Monetization (spending vs retention correlation)
/// </summary>
public class RetentionTracker : MonoBehaviour
{
    [System.Serializable]
    public class UserProfile
    {
        public string userId;
        public string installDate;
        public string cohortDate; // Date user was acquired (for cohort analysis)
        public int totalSessions = 0;
        public int totalPlaytimeSeconds = 0;
        public List<string> loginDates = new List<string>(); // All days user logged in
        public string lastSessionDate = "";
        public int daysSinceLastSession = 0;
        public int currentLoginStreak = 0;
        public float totalRevenue = 0f;
        public int iapPurchaseCount = 0;
        public bool hasEverSpent = false;
        public int adWatchCount = 0;
        public bool isChurned = false;
        public string churnDate = "";
    }

    [System.Serializable]
    public class SessionRecord
    {
        public string sessionId;
        public string sessionDate;
        public long sessionStartTime;
        public long sessionEndTime;
        public float sessionDurationSeconds;
        public int levelsPlayed;
        public int levelsCompleted;
        public float levelCompletionRate; // levels completed / levels played
        public int totalScore;
        public int totalMoves;
        public float totalGameTime;
        
        // Feature usage in this session
        public int hintsUsed;
        public int powerUpsUsed;
        public int shopVisits;
        public bool watchedAd;
        public float shopTimeSeconds;
        
        // Difficulty signals
        public int levelRetries;
        public int lastLevelPlayed;
        public bool quitOnLevel; // Did player quit while on a level?
    }

    [System.Serializable]
    public class RetentionCohort
    {
        public string cohortDate;
        public int totalUsersInCohort;
        public int d1Retained;
        public int d7Retained;
        public int d30Retained;
        public float d1RetentionRate;
        public float d7RetentionRate;
        public float d30RetentionRate;
        public float cohortARPPU; // Average revenue per user in cohort
        public float cohortLTV; // Lifetime value per user
        public int totalRevenue;
    }

    [System.Serializable]
    public class LevelChurnData
    {
        public int levelId;
        public int timesStarted;
        public int timesCompleted;
        public float completionRate;
        public int timesAbandonedHere; // Players quit while on this level
        public float abandonmentRate; // How many quit here vs started
        public int averageRetries;
        public float averageTimeSeconds;
    }

    [System.Serializable]
    public class FeatureEngagementMetrics
    {
        public string featureName;
        public int userCount; // How many users used this feature
        public int totalUsage;
        public float averagePerSession;
        public float retentionBonus; // D7 retention for users who use this feature
        public float conversionBonus; // IAP conversion for users who use this feature
    }

    public static RetentionTracker Instance { get; private set; }

    // Core data
    private UserProfile currentUserProfile;
    private SessionRecord currentSession;
    private List<SessionRecord> allSessions = new List<SessionRecord>();
    
    // Analytics data
    private List<RetentionCohort> cohortData = new List<RetentionCohort>();
    private Dictionary<int, LevelChurnData> levelChurnData = new Dictionary<int, LevelChurnData>();
    private Dictionary<string, FeatureEngagementMetrics> featureEngagementMetrics = new Dictionary<string, FeatureEngagementMetrics>();
    
    // Configuration
    private const string USER_PROFILE_KEY = "RetentionTracker_UserProfile";
    private const string SESSION_HISTORY_KEY = "RetentionTracker_SessionHistory";
    private const int CHURN_THRESHOLD_DAYS = 7; // Player considered churned after 7 days of no play
    
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
        LoadUserProfile();
        LoadSessionHistory();
        UpdateRetentionMetrics();
        StartNewSession();
    }

    /// <summary>
    /// Create or load user profile
    /// </summary>
    private void LoadUserProfile()
    {
        string json = PlayerPrefs.GetString(USER_PROFILE_KEY, "");
        
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                currentUserProfile = JsonUtility.FromJson<UserProfile>(json);
                Debug.Log($"[RetentionTracker] Loaded user profile - Install: {currentUserProfile.installDate}, Sessions: {currentUserProfile.totalSessions}");
            }
            catch
            {
                CreateNewUserProfile();
            }
        }
        else
        {
            CreateNewUserProfile();
        }
    }

    private void CreateNewUserProfile()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        currentUserProfile = new UserProfile
        {
            userId = SystemInfo.deviceUniqueIdentifier,
            installDate = today,
            cohortDate = today,
            loginDates = new List<string> { today }
        };
        SaveUserProfile();
        Debug.Log($"[RetentionTracker] Created new user profile - ID: {currentUserProfile.userId}");
    }

    /// <summary>
    /// Start a new session and track it
    /// </summary>
    public void StartNewSession()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        
        // Check if this is a new login day
        if (!currentUserProfile.loginDates.Contains(today))
        {
            currentUserProfile.loginDates.Add(today);
            currentUserProfile.lastSessionDate = today;
            
            // Update login streak
            if (currentUserProfile.loginDates.Count > 1)
            {
                string yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                if (currentUserProfile.loginDates.Contains(yesterday))
                {
                    currentUserProfile.currentLoginStreak++;
                }
                else
                {
                    currentUserProfile.currentLoginStreak = 1;
                }
            }
            else
            {
                currentUserProfile.currentLoginStreak = 1;
            }
            
            LogDailyActiveUser();
        }

        currentUserProfile.totalSessions++;

        currentSession = new SessionRecord
        {
            sessionId = Guid.NewGuid().ToString(),
            sessionDate = today,
            sessionStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            levelsPlayed = 0,
            levelsCompleted = 0
        };

        Debug.Log($"[RetentionTracker] New session started - ID: {currentSession.sessionId}, Total sessions: {currentUserProfile.totalSessions}");
    }

    /// <summary>
    /// End current session and save metrics
    /// </summary>
    public void EndSession()
    {
        if (currentSession == null) return;

        currentSession.sessionEndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        currentSession.sessionDurationSeconds = (currentSession.sessionEndTime - currentSession.sessionStartTime) / 1000f;

        currentUserProfile.totalPlaytimeSeconds += (int)currentSession.sessionDurationSeconds;
        currentSession.levelCompletionRate = currentSession.levelsPlayed > 0 ? 
            (float)currentSession.levelsCompleted / currentSession.levelsPlayed : 0;

        allSessions.Add(currentSession);
        SaveUserProfile();
        SaveSessionHistory();

        Debug.Log($"[RetentionTracker] Session ended - Duration: {currentSession.sessionDurationSeconds}s, Levels: {currentSession.levelsComplayed}/{currentSession.levelsPlayed}");
    }

    /// <summary>
    /// Log level started
    /// </summary>
    public void LogLevelStart(int levelId)
    {
        if (currentSession == null) return;

        currentSession.levelsPlayed++;
        currentSession.lastLevelPlayed = levelId;

        // Initialize level churn data if needed
        if (!levelChurnData.ContainsKey(levelId))
        {
            levelChurnData[levelId] = new LevelChurnData { levelId = levelId };
        }
        levelChurnData[levelId].timesStarted++;
    }

    /// <summary>
    /// Log level completed
    /// </summary>
    public void LogLevelComplete(int levelId, int moveCount, float timeSeconds, bool withHint)
    {
        if (currentSession == null) return;

        currentSession.levelsCompleted++;
        currentSession.totalMoves += moveCount;

        if (levelChurnData.ContainsKey(levelId))
        {
            levelChurnData[levelId].timesCompleted++;
            levelChurnData[levelId].averageTimeSeconds = 
                (levelChurnData[levelId].averageTimeSeconds + timeSeconds) / 2f;
        }

        LogFeatureEngagement("level_completion", 1);
    }

    /// <summary>
    /// Log session quit (player quit while on a level)
    /// </summary>
    public void LogSessionQuit(int currentLevel)
    {
        if (currentSession == null) return;

        currentSession.quitOnLevel = true;
        currentSession.lastLevelPlayed = currentLevel;

        // Track which level causes abandonment
        if (levelChurnData.ContainsKey(currentLevel))
        {
            levelChurnData[currentLevel].timesAbandonedHere++;
        }

        LogChurnSignal($"Quit on level {currentLevel}");
    }

    /// <summary>
    /// Log hint usage
    /// </summary>
    public void LogHintUsed(int levelId)
    {
        if (currentSession == null) return;

        currentSession.hintsUsed++;
        LogFeatureEngagement("hint_usage", 1);

        // Hint usage is a churn signal - might indicate frustration
        LogChurnSignal("Hint used - possible difficulty spike");
    }

    /// <summary>
    /// Log power-up usage
    /// </summary>
    public void LogPowerUpUsed(string powerUpType)
    {
        if (currentSession == null) return;

        currentSession.powerUpsUsed++;
        LogFeatureEngagement("powerup_usage", 1);
    }

    /// <summary>
    /// Log shop visit (monetization signal)
    /// </summary>
    public void LogShopVisit()
    {
        if (currentSession == null) return;

        currentSession.shopVisits++;
        LogFeatureEngagement("shop_visit", 1);
    }

    /// <summary>
    /// Log time spent in shop
    /// </summary>
    public void LogShopTimeSpent(float seconds)
    {
        if (currentSession == null) return;

        currentSession.shopTimeSeconds += seconds;
    }

    /// <summary>
    /// Log ad watch
    /// </summary>
    public void LogAdWatch(string adNetwork)
    {
        if (currentSession == null) return;

        currentSession.watchedAd = true;
        currentUserProfile.adWatchCount++;
        LogFeatureEngagement("ad_watch", 1);
    }

    /// <summary>
    /// Log IAP purchase (monetization event)
    /// </summary>
    public void LogIAPPurchase(string productId, float price)
    {
        currentUserProfile.iapPurchaseCount++;
        currentUserProfile.totalRevenue += price;
        currentUserProfile.hasEverSpent = true;
        
        if (currentSession != null)
        {
            LogFeatureEngagement("iap_purchase", 1);
        }

        // Users who spend are more likely to retain - log this correlation
        LogFeatureEngagement("monetized_user", 1);
        SaveUserProfile();
    }

    /// <summary>
    /// Log feature engagement (for retention correlation analysis)
    /// </summary>
    private void LogFeatureEngagement(string featureName, int count = 1)
    {
        if (!featureEngagementMetrics.ContainsKey(featureName))
        {
            featureEngagementMetrics[featureName] = new FeatureEngagementMetrics
            {
                featureName = featureName,
                userCount = 1
            };
        }

        featureEngagementMetrics[featureName].totalUsage += count;
        if (currentSession != null)
        {
            featureEngagementMetrics[featureName].averagePerSession = 
                (float)featureEngagementMetrics[featureName].totalUsage / currentUserProfile.totalSessions;
        }
    }

    /// <summary>
    /// Log potential churn signals
    /// </summary>
    private void LogChurnSignal(string signal)
    {
        Debug.Log($"[RetentionTracker] Churn Signal: {signal}");
        // These could be used for predictive analytics in the future
    }

    /// <summary>
    /// Check and update churn status
    /// </summary>
    public void CheckChurnStatus()
    {
        if (currentUserProfile.isChurned) return;

        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (string.IsNullOrEmpty(currentUserProfile.lastSessionDate))
            return;

        DateTime lastSession = DateTime.ParseExact(currentUserProfile.lastSessionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime todayDate = DateTime.ParseExact(today, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        
        int daysSinceLastSession = (int)(todayDate - lastSession).TotalDays;
        currentUserProfile.daysSinceLastSession = daysSinceLastSession;

        if (daysSinceLastSession >= CHURN_THRESHOLD_DAYS)
        {
            currentUserProfile.isChurned = true;
            currentUserProfile.churnDate = today;
            Debug.Log($"[RetentionTracker] User marked as churned - {daysSinceLastSession} days since last session");
            SaveUserProfile();
        }
    }

    /// <summary>
    /// Log daily active user
    /// </summary>
    private void LogDailyActiveUser()
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>
        {
            { "login_date", DateTime.Now.ToString("yyyy-MM-dd") },
            { "login_streak", currentUserProfile.currentLoginStreak },
            { "total_sessions", currentUserProfile.totalSessions },
            { "total_playtime_hours", currentUserProfile.totalPlaytimeSeconds / 3600f },
            { "cohort_date", currentUserProfile.cohortDate }
        };

        Analytics.Instance.LogCustomEvent("dau", eventData);
    }

    /// <summary>
    /// Update retention metrics for cohort analysis
    /// </summary>
    private void UpdateRetentionMetrics()
    {
        string cohortDate = currentUserProfile.cohortDate;
        
        // Find or create cohort
        RetentionCohort cohort = cohortData.Find(c => c.cohortDate == cohortDate);
        if (cohort == null)
        {
            cohort = new RetentionCohort { cohortDate = cohortDate, totalUsersInCohort = 1 };
            cohortData.Add(cohort);
        }

        // Calculate retention based on login dates
        if (currentUserProfile.loginDates.Count > 0)
        {
            DateTime installDate = DateTime.ParseExact(cohortDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            foreach (string loginDate in currentUserProfile.loginDates)
            {
                DateTime date = DateTime.ParseExact(loginDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                int daysSinceInstall = (int)(date - installDate).TotalDays;

                if (daysSinceInstall == 1) cohort.d1Retained++;
                if (daysSinceInstall == 7) cohort.d7Retained++;
                if (daysSinceInstall == 30) cohort.d30Retained++;
            }
        }

        // Calculate retention rates
        if (cohort.totalUsersInCohort > 0)
        {
            cohort.d1RetentionRate = (float)cohort.d1Retained / cohort.totalUsersInCohort;
            cohort.d7RetentionRate = (float)cohort.d7Retained / cohort.totalUsersInCohort;
            cohort.d30RetentionRate = (float)cohort.d30Retained / cohort.totalUsersInCohort;
        }

        // Update cohort monetization
        cohort.totalRevenue = (int)currentUserProfile.totalRevenue;
        cohort.cohortARPPU = cohort.totalUsersInCohort > 0 ? cohort.totalRevenue / cohort.totalUsersInCohort : 0;
        // LTV would be calculated over longer term
        cohort.cohortLTV = cohort.cohortARPPU;
    }

    /// <summary>
    /// Get retention metrics summary
    /// </summary>
    public Dictionary<string, object> GetRetentionMetricsSummary()
    {
        return new Dictionary<string, object>
        {
            { "user_id", currentUserProfile.userId },
            { "install_date", currentUserProfile.installDate },
            { "cohort_date", currentUserProfile.cohortDate },
            { "total_sessions", currentUserProfile.totalSessions },
            { "total_playtime_hours", currentUserProfile.totalPlaytimeSeconds / 3600f },
            { "login_days", currentUserProfile.loginDates.Count },
            { "login_streak", currentUserProfile.currentLoginStreak },
            { "last_session", currentUserProfile.lastSessionDate },
            { "days_since_last_session", currentUserProfile.daysSinceLastSession },
            { "is_churned", currentUserProfile.isChurned },
            { "total_revenue", currentUserProfile.totalRevenue },
            { "iap_purchases", currentUserProfile.iapPurchaseCount },
            { "has_spent", currentUserProfile.hasEverSpent },
            { "ad_watches", currentUserProfile.adWatchCount }
        };
    }

    /// <summary>
    /// Get current session metrics
    /// </summary>
    public Dictionary<string, object> GetCurrentSessionMetrics()
    {
        if (currentSession == null)
            return new Dictionary<string, object>();

        return new Dictionary<string, object>
        {
            { "session_id", currentSession.sessionId },
            { "session_date", currentSession.sessionDate },
            { "duration_seconds", currentSession.sessionDurationSeconds },
            { "levels_played", currentSession.levelsPlayed },
            { "levels_completed", currentSession.levelsCompleted },
            { "completion_rate", currentSession.levelCompletionRate },
            { "total_score", currentSession.totalScore },
            { "hints_used", currentSession.hintsUsed },
            { "powerups_used", currentSession.powerUpsUsed },
            { "shop_visits", currentSession.shopVisits },
            { "watched_ad", currentSession.watchedAd }
        };
    }

    /// <summary>
    /// Get level churn analysis
    /// </summary>
    public Dictionary<int, LevelChurnData> GetLevelChurnData()
    {
        // Calculate abandonment rates
        foreach (var levelData in levelChurnData.Values)
        {
            if (levelData.timesStarted > 0)
            {
                levelData.completionRate = (float)levelData.timesCompleted / levelData.timesStarted;
                levelData.abandonmentRate = (float)levelData.timesAbandonedHere / levelData.timesStarted;
            }
        }
        return levelChurnData;
    }

    /// <summary>
    /// Get cohort analysis data
    /// </summary>
    public List<RetentionCohort> GetCohortAnalysis()
    {
        return new List<RetentionCohort>(cohortData);
    }

    /// <summary>
    /// Get feature engagement metrics
    /// </summary>
    public Dictionary<string, FeatureEngagementMetrics> GetFeatureEngagement()
    {
        return new Dictionary<string, FeatureEngagementMetrics>(featureEngagementMetrics);
    }

    /// <summary>
    /// Save user profile to persistent storage
    /// </summary>
    private void SaveUserProfile()
    {
        string json = JsonUtility.ToJson(currentUserProfile);
        PlayerPrefs.SetString(USER_PROFILE_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load session history
    /// </summary>
    private void LoadSessionHistory()
    {
        string json = PlayerPrefs.GetString(SESSION_HISTORY_KEY, "");
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                // Unity's JsonUtility doesn't support Lists directly, so we need a wrapper
                // For now, just log that we'd load this
                Debug.Log("[RetentionTracker] Session history would be loaded from persistent storage");
            }
            catch
            {
                Debug.LogWarning("Failed to load session history");
            }
        }
    }

    /// <summary>
    /// Save session history
    /// </summary>
    private void SaveSessionHistory()
    {
        // In a real implementation, save all sessions to file or backend
        // For now, just log
        Debug.Log($"[RetentionTracker] Saving {allSessions.Count} sessions to history");
    }

    private void OnApplicationQuit()
    {
        EndSession();
        CheckChurnStatus();
        SaveUserProfile();
    }

    public UserProfile GetUserProfile() => currentUserProfile;
    public List<SessionRecord> GetAllSessions() => new List<SessionRecord>(allSessions);
}
