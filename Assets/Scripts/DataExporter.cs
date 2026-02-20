using UnityEngine;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_EDITOR
using System.IO;
#endif

/// <summary>
/// DataExporter - Export analytics and retention data to CSV and JSON formats
/// Supports various export formats for analysis, publisher reports, and data visualization
/// 
/// EXPORT FORMATS:
/// - CSV: Spreadsheet format for Excel/Sheets analysis
/// - JSON: Structured format for backend/API
/// - Dashboard: Pre-formatted summary report
/// </summary>
public class DataExporter : MonoBehaviour
{
    public static DataExporter Instance { get; private set; }

    [System.Serializable]
    public class ExportSettings
    {
        public bool includeSessionDetails = true;
        public bool includeLevelData = true;
        public bool includeCohortData = true;
        public bool includeFeatureMetrics = true;
        public bool excludePersonalInfo = false; // For sharing with publishers
    }

    private ExportSettings currentSettings = new ExportSettings();
    
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

    /// <summary>
    /// Export all analytics data to JSON
    /// </summary>
    public string ExportToJSON()
    {
        RetentionTracker tracker = RetentionTracker.Instance;
        Analytics analytics = Analytics.Instance;

        if (tracker == null || analytics == null)
        {
            Debug.LogError("[DataExporter] Required components not found");
            return "";
        }

        var userProfile = tracker.GetUserProfile();
        var sessionMetrics = tracker.GetCurrentSessionMetrics();
        var levelChurnData = tracker.GetLevelChurnData();
        var cohortData = tracker.GetCohortAnalysis();
        var featureMetrics = tracker.GetFeatureEngagement();

        var exportData = new Dictionary<string, object>
        {
            { "export_date", DateTime.UtcNow.ToString("O") },
            { "user_profile", ConvertToSerializable(userProfile) },
            { "current_session", sessionMetrics },
            { "level_churn_analysis", ConvertLevelChurnToSerializable(levelChurnData) },
            { "cohort_analysis", ConvertCohortsToSerializable(cohortData) },
            { "feature_engagement", ConvertFeatureMetricsToSerializable(featureMetrics) },
            { "all_sessions", ConvertSessionsToSerializable(tracker.GetAllSessions()) }
        };

        return JsonUtility.ToJson(new JsonWrapper { json = JsonUtility.ToJson(exportData, true) }, true);
    }

    /// <summary>
    /// Export user retention metrics to CSV
    /// </summary>
    public string ExportRetentionMetricsCSV()
    {
        RetentionTracker tracker = RetentionTracker.Instance;
        if (tracker == null) return "";

        var userProfile = tracker.GetUserProfile();
        var sb = new StringBuilder();

        // Header
        sb.AppendLine("Retention Metrics Report");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();

        // User Profile Summary
        sb.AppendLine("USER PROFILE");
        sb.AppendLine("Property,Value");
        sb.AppendLine($"Install Date,{userProfile.installDate}");
        sb.AppendLine($"Cohort,{userProfile.cohortDate}");
        sb.AppendLine($"Total Sessions,{userProfile.totalSessions}");
        sb.AppendLine($"Total Playtime (hours),{userProfile.totalPlaytimeSeconds / 3600f:F2}");
        sb.AppendLine($"Login Days,{userProfile.loginDates.Count}");
        sb.AppendLine($"Current Streak,{userProfile.currentLoginStreak}");
        sb.AppendLine($"Last Session,{userProfile.lastSessionDate}");
        sb.AppendLine($"Days Since Last Session,{userProfile.daysSinceLastSession}");
        sb.AppendLine($"Status,{(userProfile.isChurned ? "CHURNED" : "ACTIVE")}");
        sb.AppendLine();

        // Monetization Summary
        sb.AppendLine("MONETIZATION");
        sb.AppendLine("Property,Value");
        sb.AppendLine($"Total Revenue,${userProfile.totalRevenue:F2}");
        sb.AppendLine($"IAP Purchases,{userProfile.iapPurchaseCount}");
        sb.AppendLine($"Has Spent,{userProfile.hasEverSpent}");
        sb.AppendLine($"Ad Watches,{userProfile.adWatchCount}");
        sb.AppendLine();

        // Login History
        sb.AppendLine("LOGIN HISTORY (Last 30 Days)");
        sb.AppendLine("Date");
        DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
        foreach (var loginDate in userProfile.loginDates.OrderBy(d => d))
        {
            DateTime date = DateTime.ParseExact(loginDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (date >= thirtyDaysAgo)
            {
                sb.AppendLine(loginDate);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Export level churn analysis to CSV
    /// </summary>
    public string ExportLevelChurnCSV()
    {
        RetentionTracker tracker = RetentionTracker.Instance;
        if (tracker == null) return "";

        var levelChurnData = tracker.GetLevelChurnData();
        var sb = new StringBuilder();

        sb.AppendLine("Level Churn Analysis");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();

        sb.AppendLine("Level ID,Times Started,Times Completed,Completion Rate,Times Abandoned,Abandonment Rate,Avg Time (s)");

        foreach (var levelData in levelChurnData.Values.OrderBy(l => l.levelId))
        {
            sb.AppendLine(
                $"{levelData.levelId}," +
                $"{levelData.timesStarted}," +
                $"{levelData.timesCompleted}," +
                $"{levelData.completionRate:P2}," +
                $"{levelData.timesAbandonedHere}," +
                $"{levelData.abandonmentRate:P2}," +
                $"{levelData.averageTimeSeconds:F1}"
            );
        }

        return sb.ToString();
    }

    /// <summary>
    /// Export cohort analysis to CSV
    /// </summary>
    public string ExportCohortAnalysisCSV()
    {
        RetentionTracker tracker = RetentionTracker.Instance;
        if (tracker == null) return "";

        var cohortData = tracker.GetCohortAnalysis();
        var sb = new StringBuilder();

        sb.AppendLine("Cohort Analysis (Retention & Monetization)");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();

        sb.AppendLine("Cohort Date,Users,D1 Retained,D1 Rate,D7 Retained,D7 Rate,D30 Retained,D30 Rate,ARPPU,LTV");

        foreach (var cohort in cohortData.OrderBy(c => c.cohortDate))
        {
            sb.AppendLine(
                $"{cohort.cohortDate}," +
                $"{cohort.totalUsersInCohort}," +
                $"{cohort.d1Retained}," +
                $"{cohort.d1RetentionRate:P2}," +
                $"{cohort.d7Retained}," +
                $"{cohort.d7RetentionRate:P2}," +
                $"{cohort.d30Retained}," +
                $"{cohort.d30RetentionRate:P2}," +
                $"${cohort.cohortARPPU:F2}," +
                $"${cohort.cohortLTV:F2}"
            );
        }

        return sb.ToString();
    }

    /// <summary>
    /// Export feature engagement metrics to CSV
    /// </summary>
    public string ExportFeatureEngagementCSV()
    {
        RetentionTracker tracker = RetentionTracker.Instance;
        if (tracker == null) return "";

        var featureMetrics = tracker.GetFeatureEngagement();
        var sb = new StringBuilder();

        sb.AppendLine("Feature Engagement Analysis");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();

        sb.AppendLine("Feature Name,Total Usage,Users,Per Session Avg,Retention Bonus,Conversion Bonus");

        foreach (var feature in featureMetrics.Values.OrderByDescending(f => f.totalUsage))
        {
            sb.AppendLine(
                $"{feature.featureName}," +
                $"{feature.totalUsage}," +
                $"{feature.userCount}," +
                $"{feature.averagePerSession:F2}," +
                $"{feature.retentionBonus:P2}," +
                $"{feature.conversionBonus:P2}"
            );
        }

        return sb.ToString();
    }

    /// <summary>
    /// Export publisher dashboard summary
    /// </summary>
    public string ExportPublisherDashboard()
    {
        RetentionTracker tracker = RetentionTracker.Instance;
        if (tracker == null) return "";

        var userProfile = tracker.GetUserProfile();
        var cohortData = tracker.GetCohortAnalysis();
        var levelChurnData = tracker.GetLevelChurnData();

        var sb = new StringBuilder();

        sb.AppendLine("=== PUZZLE GAME - RETENTION ANALYTICS DASHBOARD ===");
        sb.AppendLine($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();

        // Key Metrics
        sb.AppendLine("--- KEY METRICS ---");
        sb.AppendLine($"Total Users: {(cohortData.Count > 0 ? cohortData.Sum(c => c.totalUsersInCohort) : 1)}");
        
        if (cohortData.Count > 0)
        {
            var latestCohort = cohortData.OrderByDescending(c => c.cohortDate).First();
            sb.AppendLine($"D1 Retention: {latestCohort.d1RetentionRate:P1}");
            sb.AppendLine($"D7 Retention: {latestCohort.d7RetentionRate:P1}");
            sb.AppendLine($"D30 Retention: {latestCohort.d30RetentionRate:P1}");
        }

        sb.AppendLine($"Avg Playtime (hours): {userProfile.totalPlaytimeSeconds / 3600f:F1}");
        sb.AppendLine($"Total Revenue: ${(cohortData.Count > 0 ? cohortData.Sum(c => c.totalRevenue) : userProfile.totalRevenue):F2}");
        sb.AppendLine($"ARPPU: ${(cohortData.Count > 0 ? cohortData.Average(c => c.cohortARPPU) : 0):F2}");
        sb.AppendLine();

        // Monetization
        sb.AppendLine("--- MONETIZATION ---");
        int totalUsers = cohortData.Count > 0 ? cohortData.Sum(c => c.totalUsersInCohort) : 1;
        float spendingPercentage = userProfile.hasEverSpent ? 100f : 0f; // Single user case
        sb.AppendLine($"% Players Spending: {spendingPercentage:F1}%");
        sb.AppendLine($"IAP Purchases: {userProfile.iapPurchaseCount}");
        sb.AppendLine($"Ad Watches: {userProfile.adWatchCount}");
        sb.AppendLine();

        // Churn Analysis
        sb.AppendLine("--- CHURN SIGNALS ---");
        var problemLevels = levelChurnData.Values
            .Where(l => l.abandonmentRate > 0.3)
            .OrderByDescending(l => l.abandonmentRate)
            .Take(5);

        if (problemLevels.Any())
        {
            sb.AppendLine("High-Churn Levels (>30% abandonment):");
            foreach (var level in problemLevels)
            {
                sb.AppendLine($"  Level {level.levelId}: {level.abandonmentRate:P1} abandonment ({level.timesAbandonedHere} times)");
            }
        }
        else
        {
            sb.AppendLine("No critical churn issues detected");
        }

        sb.AppendLine();
        sb.AppendLine("--- RECOMMENDATIONS ---");
        sb.AppendLine("1. Monitor Level Churn: Check highlighted levels for difficulty balance");
        sb.AppendLine("2. Feature Engagement: Track which features drive retention");
        sb.AppendLine("3. Monetization: Test new IAP bundles to increase conversion");
        sb.AppendLine("4. Ad Strategy: Optimize ad placement and frequency");
        sb.AppendLine("5. Daily Retention: Focus on D1 and D7 retention improvements");

        return sb.ToString();
    }

    /// <summary>
    /// Save export to file (Editor/Desktop builds only)
    /// </summary>
    public bool SaveExportToFile(string content, string filename, string folder = "Analytics")
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_EDITOR
        try
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, folder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fullPath = System.IO.Path.Combine(path, filename);
            File.WriteAllText(fullPath, content);
            Debug.Log($"[DataExporter] Exported to: {fullPath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataExporter] Failed to save file: {e.Message}");
            return false;
        }
#else
        Debug.LogWarning("[DataExporter] File export not supported on this platform");
        return false;
#endif
    }

    /// <summary>
    /// Export all analytics reports
    /// </summary>
    public void ExportAllReports(string folder = "Analytics")
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        SaveExportToFile(ExportRetentionMetricsCSV(), $"retention_{timestamp}.csv", folder);
        SaveExportToFile(ExportLevelChurnCSV(), $"level_churn_{timestamp}.csv", folder);
        SaveExportToFile(ExportCohortAnalysisCSV(), $"cohort_analysis_{timestamp}.csv", folder);
        SaveExportToFile(ExportFeatureEngagementCSV(), $"feature_engagement_{timestamp}.csv", folder);
        SaveExportToFile(ExportPublisherDashboard(), $"publisher_dashboard_{timestamp}.txt", folder);
        SaveExportToFile(ExportToJSON(), $"full_analytics_{timestamp}.json", folder);

        Debug.Log("[DataExporter] All reports exported successfully");
    }

    // Helper methods for conversion
    private Dictionary<string, object> ConvertToSerializable(RetentionTracker.UserProfile profile)
    {
        return new Dictionary<string, object>
        {
            { "user_id", profile.userId },
            { "install_date", profile.installDate },
            { "cohort_date", profile.cohortDate },
            { "total_sessions", profile.totalSessions },
            { "total_playtime_seconds", profile.totalPlaytimeSeconds },
            { "login_days", profile.loginDates.Count },
            { "login_streak", profile.currentLoginStreak },
            { "last_session", profile.lastSessionDate },
            { "days_since_last_session", profile.daysSinceLastSession },
            { "is_churned", profile.isChurned },
            { "churn_date", profile.churnDate },
            { "total_revenue", profile.totalRevenue },
            { "iap_purchases", profile.iapPurchaseCount },
            { "has_spent", profile.hasEverSpent },
            { "ad_watches", profile.adWatchCount }
        };
    }

    private List<Dictionary<string, object>> ConvertLevelChurnToSerializable(Dictionary<int, RetentionTracker.LevelChurnData> data)
    {
        var result = new List<Dictionary<string, object>>();
        foreach (var level in data.Values)
        {
            result.Add(new Dictionary<string, object>
            {
                { "level_id", level.levelId },
                { "times_started", level.timesStarted },
                { "times_completed", level.timesCompleted },
                { "completion_rate", level.completionRate },
                { "times_abandoned", level.timesAbandonedHere },
                { "abandonment_rate", level.abandonmentRate },
                { "avg_time_seconds", level.averageTimeSeconds }
            });
        }
        return result;
    }

    private List<Dictionary<string, object>> ConvertCohortsToSerializable(List<RetentionTracker.RetentionCohort> cohorts)
    {
        var result = new List<Dictionary<string, object>>();
        foreach (var cohort in cohorts)
        {
            result.Add(new Dictionary<string, object>
            {
                { "cohort_date", cohort.cohortDate },
                { "users", cohort.totalUsersInCohort },
                { "d1_retained", cohort.d1Retained },
                { "d1_rate", cohort.d1RetentionRate },
                { "d7_retained", cohort.d7Retained },
                { "d7_rate", cohort.d7RetentionRate },
                { "d30_retained", cohort.d30Retained },
                { "d30_rate", cohort.d30RetentionRate },
                { "arppu", cohort.cohortARPPU },
                { "ltv", cohort.cohortLTV }
            });
        }
        return result;
    }

    private Dictionary<string, object> ConvertFeatureMetricsToSerializable(Dictionary<string, RetentionTracker.FeatureEngagementMetrics> metrics)
    {
        var result = new Dictionary<string, object>();
        foreach (var feature in metrics)
        {
            result[feature.Key] = new Dictionary<string, object>
            {
                { "name", feature.Value.featureName },
                { "user_count", feature.Value.userCount },
                { "total_usage", feature.Value.totalUsage },
                { "avg_per_session", feature.Value.averagePerSession },
                { "retention_bonus", feature.Value.retentionBonus },
                { "conversion_bonus", feature.Value.conversionBonus }
            };
        }
        return result;
    }

    private List<Dictionary<string, object>> ConvertSessionsToSerializable(List<RetentionTracker.SessionRecord> sessions)
    {
        var result = new List<Dictionary<string, object>>();
        foreach (var session in sessions)
        {
            result.Add(new Dictionary<string, object>
            {
                { "session_id", session.sessionId },
                { "date", session.sessionDate },
                { "duration_seconds", session.sessionDurationSeconds },
                { "levels_played", session.levelsPlayed },
                { "levels_completed", session.levelsCompleted },
                { "completion_rate", session.levelCompletionRate },
                { "score", session.totalScore },
                { "hints_used", session.hintsUsed },
                { "shop_visits", session.shopVisits },
                { "ad_watched", session.watchedAd }
            });
        }
        return result;
    }

    [System.Serializable]
    private class JsonWrapper
    {
        public string json;
    }
}
