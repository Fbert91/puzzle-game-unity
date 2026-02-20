using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// AnalyticsDashboard - In-game UI dashboard for viewing retention and engagement metrics
/// Provides quick snapshots of user retention, monetization, and feature engagement
/// 
/// FEATURES:
/// - Real-time retention metrics display
/// - Session summary view
/// - Monetization overview
/// - Level churn indicators
/// - Quick export button (CSV/JSON)
/// - Publisher-friendly summary for sharing
/// </summary>
public class AnalyticsDashboard : MonoBehaviour
{
    [SerializeField] private GameObject dashboardPanel;
    [SerializeField] private Text retentionSummaryText;
    [SerializeField] private Text sessionSummaryText;
    [SerializeField] private Text monetizationText;
    [SerializeField] private Text churnWarningsText;
    [SerializeField] private Button exportButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button publisherReportButton;
    [SerializeField] private bool autoUpdateInterval = false;
    [SerializeField] private float updateIntervalSeconds = 60f;

    private RetentionTracker retentionTracker;
    private DataExporter dataExporter;
    private float lastUpdateTime = 0f;
    private bool isDashboardVisible = false;

    private void Start()
    {
        retentionTracker = RetentionTracker.Instance;
        dataExporter = DataExporter.Instance;

        if (dashboardPanel != null)
            dashboardPanel.SetActive(false);

        if (exportButton != null)
            exportButton.onClick.AddListener(OnExportClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        if (publisherReportButton != null)
            publisherReportButton.onClick.AddListener(OnPublisherReportClicked);
    }

    private void Update()
    {
        if (autoUpdateInterval && isDashboardVisible)
        {
            if (Time.time - lastUpdateTime >= updateIntervalSeconds)
            {
                RefreshDashboard();
                lastUpdateTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Show the analytics dashboard
    /// </summary>
    public void ShowDashboard()
    {
        if (dashboardPanel != null)
        {
            dashboardPanel.SetActive(true);
            isDashboardVisible = true;
            RefreshDashboard();
            Debug.Log("[AnalyticsDashboard] Dashboard shown");
        }
    }

    /// <summary>
    /// Hide the analytics dashboard
    /// </summary>
    public void HideDashboard()
    {
        if (dashboardPanel != null)
        {
            dashboardPanel.SetActive(false);
            isDashboardVisible = false;
            Debug.Log("[AnalyticsDashboard] Dashboard hidden");
        }
    }

    /// <summary>
    /// Refresh all dashboard displays
    /// </summary>
    private void RefreshDashboard()
    {
        if (retentionTracker == null)
            return;

        UpdateRetentionMetrics();
        UpdateSessionMetrics();
        UpdateMonetizationMetrics();
        UpdateChurnWarnings();
    }

    /// <summary>
    /// Update retention metrics display
    /// </summary>
    private void UpdateRetentionMetrics()
    {
        if (retentionSummaryText == null)
            return;

        var userProfile = retentionTracker.GetUserProfile();
        var cohortData = retentionTracker.GetCohortAnalysis();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== RETENTION METRICS ===");
        sb.AppendLine($"Install Date: {userProfile.installDate}");
        sb.AppendLine($"Days Active: {userProfile.loginDates.Count}");
        sb.AppendLine($"Login Streak: {userProfile.currentLoginStreak}");
        sb.AppendLine($"Status: {(userProfile.isChurned ? "CHURNED" : "ACTIVE")}");
        sb.AppendLine();

        // Show cohort retention rates
        if (cohortData.Count > 0)
        {
            var latestCohort = cohortData.OrderByDescending(c => c.cohortDate).First();
            sb.AppendLine("Latest Cohort Retention:");
            sb.AppendLine($"D1: {latestCohort.d1RetentionRate:P1} ({latestCohort.d1Retained}/{latestCohort.totalUsersInCohort})");
            sb.AppendLine($"D7: {latestCohort.d7RetentionRate:P1} ({latestCohort.d7Retained}/{latestCohort.totalUsersInCohort})");
            sb.AppendLine($"D30: {latestCohort.d30RetentionRate:P1} ({latestCohort.d30Retained}/{latestCohort.totalUsersInCohort})");
        }

        retentionSummaryText.text = sb.ToString();
    }

    /// <summary>
    /// Update session metrics display
    /// </summary>
    private void UpdateSessionMetrics()
    {
        if (sessionSummaryText == null)
            return;

        var userProfile = retentionTracker.GetUserProfile();
        var currentSessionMetrics = retentionTracker.GetCurrentSessionMetrics();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== SESSION STATS ===");
        sb.AppendLine($"Total Sessions: {userProfile.totalSessions}");
        sb.AppendLine($"Total Playtime: {userProfile.totalPlaytimeSeconds / 3600f:F1}h");
        sb.AppendLine($"Avg Session: {(userProfile.totalSessions > 0 ? userProfile.totalPlaytimeSeconds / userProfile.totalSessions : 0):F0}s");
        sb.AppendLine();

        if (currentSessionMetrics.Count > 0)
        {
            sb.AppendLine("Current Session:");
            sb.AppendLine($"Duration: {(float)currentSessionMetrics["duration_seconds"]:F0}s");
            sb.AppendLine($"Levels: {currentSessionMetrics["levels_completed"]}/{currentSessionMetrics["levels_played"]}");
            sb.AppendLine($"Completion: {((float)currentSessionMetrics["completion_rate"]):P0}");
        }

        sessionSummaryText.text = sb.ToString();
    }

    /// <summary>
    /// Update monetization metrics display
    /// </summary>
    private void UpdateMonetizationMetrics()
    {
        if (monetizationText == null)
            return;

        var userProfile = retentionTracker.GetUserProfile();
        var cohortData = retentionTracker.GetCohortAnalysis();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== MONETIZATION ===");
        sb.AppendLine($"Revenue (User): ${userProfile.totalRevenue:F2}");
        sb.AppendLine($"Purchases: {userProfile.iapPurchaseCount}");
        sb.AppendLine($"Status: {(userProfile.hasEverSpent ? "SPENDER" : "F2P")}");
        sb.AppendLine($"Ad Watches: {userProfile.adWatchCount}");
        sb.AppendLine();

        if (cohortData.Count > 0)
        {
            var latestCohort = cohortData.OrderByDescending(c => c.cohortDate).First();
            sb.AppendLine("Cohort ARPPU (All Users):");
            sb.AppendLine($"${latestCohort.cohortARPPU:F2}");
            sb.AppendLine($"Total Cohort Revenue: ${latestCohort.totalRevenue}");
        }

        monetizationText.text = sb.ToString();
    }

    /// <summary>
    /// Update churn warnings display
    /// </summary>
    private void UpdateChurnWarnings()
    {
        if (churnWarningsText == null)
            return;

        var levelChurnData = retentionTracker.GetLevelChurnData();
        var userProfile = retentionTracker.GetUserProfile();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== CHURN SIGNALS ===");

        // User churn status
        if (userProfile.isChurned)
        {
            sb.AppendLine($"⚠️ USER CHURNED ({userProfile.daysSinceLastSession} days)");
        }
        else if (userProfile.daysSinceLastSession >= 3)
        {
            sb.AppendLine($"⚠️ WARNING: {userProfile.daysSinceLastSession} days no play");
        }
        else
        {
            sb.AppendLine("✓ User Active");
        }

        sb.AppendLine();

        // Level churn hotspots
        var problemLevels = levelChurnData.Values
            .Where(l => l.abandonmentRate > 0.3)
            .OrderByDescending(l => l.abandonmentRate)
            .Take(3);

        if (problemLevels.Any())
        {
            sb.AppendLine("High-Churn Levels:");
            foreach (var level in problemLevels)
            {
                sb.AppendLine($"  Level {level.levelId}: {level.abandonmentRate:P0} abandon");
            }
        }
        else
        {
            sb.AppendLine("✓ No critical churn levels");
        }

        churnWarningsText.text = sb.ToString();
    }

    /// <summary>
    /// Export data on button click
    /// </summary>
    private void OnExportClicked()
    {
        if (dataExporter == null)
        {
            Debug.LogError("[AnalyticsDashboard] DataExporter not found");
            return;
        }

        // Export all reports
        dataExporter.ExportAllReports();
        ShowNotification("Analytics exported to persistent data folder");
        Debug.Log("[AnalyticsDashboard] Export completed");
    }

    /// <summary>
    /// Generate and show publisher report
    /// </summary>
    private void OnPublisherReportClicked()
    {
        if (dataExporter == null)
        {
            Debug.LogError("[AnalyticsDashboard] DataExporter not found");
            return;
        }

        string publisherReport = dataExporter.ExportPublisherDashboard();
        
        // Copy to clipboard (or display in a new window)
        GUIUtility.systemCopyBuffer = publisherReport;
        ShowNotification("Publisher report copied to clipboard!");
        Debug.Log(publisherReport);
    }

    /// <summary>
    /// Close dashboard on button click
    /// </summary>
    private void OnCloseClicked()
    {
        HideDashboard();
    }

    /// <summary>
    /// Show brief notification to user
    /// </summary>
    private void ShowNotification(string message)
    {
        Debug.Log($"[AnalyticsDashboard] {message}");
        // TODO: Implement toast notification or UI alert
    }

    /// <summary>
    /// Get summary for publishing (returns formatted string)
    /// </summary>
    public string GetPublisherSummary()
    {
        if (dataExporter == null)
            return "";

        return dataExporter.ExportPublisherDashboard();
    }

    /// <summary>
    /// Get CSV export of retention metrics
    /// </summary>
    public string GetRetentionCSV()
    {
        if (dataExporter == null)
            return "";

        return dataExporter.ExportRetentionMetricsCSV();
    }

    /// <summary>
    /// Get detailed metrics as dictionary (for programmatic access)
    /// </summary>
    public Dictionary<string, object> GetMetricsSummary()
    {
        var userProfile = retentionTracker.GetUserProfile();
        var cohortData = retentionTracker.GetCohortAnalysis();

        var summary = new Dictionary<string, object>
        {
            { "total_sessions", userProfile.totalSessions },
            { "total_playtime_hours", userProfile.totalPlaytimeSeconds / 3600f },
            { "login_days", userProfile.loginDates.Count },
            { "login_streak", userProfile.currentLoginStreak },
            { "is_churned", userProfile.isChurned },
            { "total_revenue", userProfile.totalRevenue },
            { "iap_purchases", userProfile.iapPurchaseCount },
            { "has_spent", userProfile.hasEverSpent }
        };

        if (cohortData.Count > 0)
        {
            var latestCohort = cohortData.OrderByDescending(c => c.cohortDate).First();
            summary["d1_retention"] = latestCohort.d1RetentionRate;
            summary["d7_retention"] = latestCohort.d7RetentionRate;
            summary["d30_retention"] = latestCohort.d30RetentionRate;
            summary["arppu"] = latestCohort.cohortARPPU;
        }

        return summary;
    }
}
