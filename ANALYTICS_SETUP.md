# Analytics Setup Guide

## Overview

The PuzzleGame analytics system provides comprehensive retention tracking, engagement metrics, and monetization insights. It consists of four main components:

1. **Analytics.cs** - High-level event logging and Firebase integration
2. **RetentionTracker.cs** - Detailed user lifecycle and retention calculations
3. **DataExporter.cs** - Export and reporting (CSV/JSON)
4. **AnalyticsDashboard.cs** - In-game UI for viewing metrics

## Quick Start

### 1. Add Components to Your Game

Add these scripts to your game scene:

```csharp
// Add to your game manager or startup script
public void InitializeAnalytics()
{
    // Create Analytics singleton
    GameObject analyticsObj = new GameObject("Analytics");
    Analytics analytics = analyticsObj.AddComponent<Analytics>();
    
    // Create RetentionTracker
    GameObject retentionObj = new GameObject("RetentionTracker");
    RetentionTracker tracker = retentionObj.AddComponent<RetentionTracker>();
    
    // Create DataExporter
    GameObject exporterObj = new GameObject("DataExporter");
    DataExporter exporter = exporterObj.AddComponent<DataExporter>();
    
    // Optional: Create Dashboard
    GameObject dashboardObj = new GameObject("AnalyticsDashboard");
    AnalyticsDashboard dashboard = dashboardObj.AddComponent<AnalyticsDashboard>();
}
```

### 2. Log Events from Your Game

#### Log Level Events

```csharp
// When player starts a level
Analytics.Instance.LogLevelStart(levelId);
Analytics.Instance.SyncLevelStartToRetention(levelId);

// When player completes a level
Analytics.Instance.LogLevelComplete(levelId, score, stars, moveCount, timeSeconds);
Analytics.Instance.SyncLevelCompleteToRetention(levelId, moveCount, timeSeconds, false);

// When player uses a hint
Analytics.Instance.LogHintUsed(levelId, hintCount);
Analytics.Instance.SyncHintUsageToRetention(levelId);

// When player retries a level
Analytics.Instance.LogLevelRetry(levelId);
```

#### Log Monetization Events

```csharp
// When player makes a purchase
Analytics.Instance.LogIAPPurchase(productId, price, "USD", purchaseToken);
Analytics.Instance.SyncIAPToRetention(productId, price);

// When player watches an ad
Analytics.Instance.LogAdImpression("admob", "rewarded_video");
Analytics.Instance.LogAdReward("admob", "coins", 50);
Analytics.Instance.SyncAdWatchToRetention("admob");

// When player visits shop
Analytics.Instance.SyncShopVisitToRetention();
```

#### Log Engagement Events

```csharp
// Custom events
Analytics.Instance.LogCustomEvent("feature_used", new Dictionary<string, object>
{
    { "feature_name", "power_ups" },
    { "count", 3 }
});
```

### 3. Export Data

```csharp
// Export all analytics to files
DataExporter.Instance.ExportAllReports();

// Or export specific reports
string retentionCSV = DataExporter.Instance.ExportRetentionMetricsCSV();
string levelChurnCSV = DataExporter.Instance.ExportLevelChurnCSV();
string publisherReport = DataExporter.Instance.ExportPublisherDashboard();

// Save to file
DataExporter.Instance.SaveExportToFile(retentionCSV, "retention_report.csv");
```

### 4. View Dashboard (Optional)

```csharp
// Show analytics dashboard in-game
AnalyticsDashboard.Instance.ShowDashboard();

// Get metrics programmatically
var metrics = AnalyticsDashboard.Instance.GetMetricsSummary();
```

## Firebase Integration (Advanced)

### Setup Firebase Analytics

1. **Import Firebase SDK**
   ```
   Add Firebase for Unity from Asset Store
   ```

2. **Initialize Firebase in your game**
   ```csharp
   public void InitializeFirebase()
   {
       Firebase.FirebaseApp.CheckAndFixAsync().ContinueWith(task => {
           if (task.IsCompleted)
           {
               Debug.Log("Firebase initialized");
               // Now you can use Firebase Analytics
           }
       });
   }
   ```

3. **Enable Firebase Logging in Analytics.cs**

   In `Analytics.cs`, uncomment the Firebase logging code:

   ```csharp
   public void LogCustomEvent(string eventName, Dictionary<string, object> parameters = null)
   {
       if (parameters == null)
           parameters = new Dictionary<string, object>();

       Debug.Log($"[Analytics] Event: {eventName}");

       // Enable Firebase logging
       FirebaseAnalytics.LogEvent(eventName, parameters);
   }
   ```

4. **Send Retention Metrics to Firebase**

   ```csharp
   public void SendRetentionToFirebase()
   {
       var metrics = RetentionTracker.Instance.GetRetentionMetricsSummary();
       
       foreach (var metric in metrics)
       {
           // Convert to Firebase-compatible format
           FirebaseAnalytics.SetUserProperty(metric.Key, metric.Value.ToString());
       }
   }
   ```

### BigQuery Integration

Once data is in Firebase Analytics, it automatically syncs to BigQuery for advanced analysis:

1. **Enable BigQuery Export in Firebase Console**
   - Go to Firebase Console > Project Settings > Integration > BigQuery
   - Enable "BigQuery Integration"

2. **Query Your Retention Data**

   ```sql
   -- Get D1 retention rates by cohort
   SELECT
       PARSE_DATE('%Y%m%d', event_date) as event_date,
       COUNT(DISTINCT user_id) as users,
       SUM(CASE WHEN user_returned_day_1 THEN 1 ELSE 0 END) as d1_retained,
       SUM(CASE WHEN user_returned_day_1 THEN 1 ELSE 0 END) / COUNT(DISTINCT user_id) as d1_retention
   FROM `project.analytics_XXXXXXX.events_*`
   WHERE event_name = 'dau'
   GROUP BY event_date
   ORDER BY event_date DESC
   ```

## Data Storage

### Local Storage (Mobile/All Platforms)

Analytics data is stored locally using PlayerPrefs:

- **User Profile**: `RetentionTracker_UserProfile`
- **Session History**: `RetentionTracker_SessionHistory`
- **Last Login**: `LastLoginDate`
- **Login Streak**: `LoginStreak`

Data syncs to backend when available.

### File Export Locations

- **Editor/Standalone**: `<persistentDataPath>/Analytics/`
- **Mobile**: App-specific documents folder
- **Files are timestamped**: `retention_2024-02-20_14-30-45.csv`

## Metrics Reference

### Core Retention Metrics

| Metric | Definition | Why It Matters |
|--------|-----------|-----------------|
| **D1 Retention** | % players returning day after install | Measures first impression and core appeal |
| **D7 Retention** | % players returning after 7 days | Shows engagement staying power |
| **D30 Retention** | % players returning after 30 days | Indicates long-term stickiness |
| **Cohort Analysis** | D1/D7/D30 grouped by install date | Identifies trends and update impacts |

### Engagement Metrics

| Metric | Definition | Target |
|--------|-----------|--------|
| **Session Length** | Average playtime per session | 10-20 minutes |
| **Session Frequency** | Sessions per day | 1-3+ |
| **Total Playtime** | Cumulative hours played | Correlates with retention |
| **Level Progression** | Levels completed per day | 2-4 typical |
| **DAU** | Daily Active Users | Trending up = good retention |
| **MAU** | Monthly Active Users | MAU/DAU ratio = stickiness |

### Feature Engagement

| Metric | What It Tells You |
|--------|------------------|
| **Hint Usage** | Difficulty/frustration level |
| **Power-up Usage** | Feature adoption and engagement |
| **Shop Visits** | Monetization interest |
| **IAP Conversion** | % who spend (target: 2-5%) |
| **Ad Watch Rate** | Alternative monetization success |

### Churn Indicators

| Signal | Action |
|--------|--------|
| **High abandonment on level N** | Difficulty spike - rebalance |
| **Hint usage spike** | Frustration increasing - help players |
| **No sessions for 7 days** | Marked as churned - re-engage |
| **Decreasing session length** | Engagement declining - update content |

### Monetization Linking

| Metric | Definition | Benchmark |
|--------|-----------|-----------|
| **ARPPU** | Average Revenue Per User | $0.50-2.00 |
| **LTV** | Lifetime Value per user | ARPPU × lifetime days |
| **Conversion Rate** | % of players who spend | 2-5% for casual games |
| **Cohort LTV** | Average LTV by install cohort | Improve over time |

## Example: Making a Publisher Report

```csharp
// Generate complete publisher dashboard
string report = DataExporter.Instance.ExportPublisherDashboard();

// Save to file
DataExporter.Instance.SaveExportToFile(report, "publisher_report.txt");

// Or display in UI
AnalyticsDashboard.Instance.GetPublisherSummary();
```

## Debugging

### Enable Debug Logging

All components log to Debug console. Use `Debug.Log` to see:

```
[Analytics] Event: level_complete
[RetentionTracker] New session started...
[DataExporter] Exported to: /path/to/file.csv
```

### Check Stored Data

```csharp
// View user profile
var profile = RetentionTracker.Instance.GetUserProfile();
Debug.Log($"User: {profile.userId}, Sessions: {profile.totalSessions}");

// View session history
var sessions = RetentionTracker.Instance.GetAllSessions();
Debug.Log($"Total sessions recorded: {sessions.Count}");

// View level churn
var churnData = RetentionTracker.Instance.GetLevelChurnData();
foreach (var level in churnData)
{
    Debug.Log($"Level {level.Key}: {level.Value.abandonmentRate:P1} abandoned");
}
```

## Common Integration Points

### In LevelManager.cs

```csharp
public void StartLevel(int levelId)
{
    Analytics.Instance.LogLevelStart(levelId);
    Analytics.Instance.SyncLevelStartToRetention(levelId);
}

public void CompleteLevel(int levelId, int score, int stars, int moves, float time)
{
    Analytics.Instance.LogLevelComplete(levelId, score, stars, moves, time);
    Analytics.Instance.SyncLevelCompleteToRetention(levelId, moves, time, false);
}
```

### In MonetizationManager.cs

```csharp
public void ProcessPurchase(string productId, float price)
{
    Analytics.Instance.LogIAPPurchase(productId, price, "USD", "token");
    Analytics.Instance.SyncIAPToRetention(productId, price);
}

public void ShowRewardedAd(string rewardType)
{
    Analytics.Instance.LogAdImpression("admob", "rewarded");
    Analytics.Instance.SyncAdWatchToRetention("admob");
}
```

### In UIManager.cs

```csharp
public void OpenShop()
{
    Analytics.Instance.SyncShopVisitToRetention();
}
```

## Performance Considerations

- Analytics operations are lightweight and non-blocking
- Local storage is compressed and efficient
- Firebase sends data in batches (not real-time)
- Dashboard updates every 60 seconds by default
- All logging is asynchronous

## Troubleshooting

**Q: My retention data looks wrong**
A: Check that `CheckDailyLogin()` is being called on startup. Ensure device clock is correct.

**Q: Firebase data not showing up**
A: Verify Firebase is initialized. Check Firebase Console for events. Wait 24-48 hours for data to appear in BigQuery.

**Q: CSV export is empty**
A: Ensure you've played the game long enough to generate data. Check persistentDataPath permissions.

**Q: Dashboard not showing**
A: Verify AnalyticsDashboard is added to scene and has proper UI references assigned.

## Next Steps

1. ✅ Set up local analytics (Analytics + RetentionTracker)
2. ✅ Integrate event logging in your game
3. ✅ Test with a few play sessions
4. ✅ Export and verify CSV reports
5. → Enable Firebase (optional but recommended for publishers)
6. → Set up BigQuery queries for advanced analysis
7. → Create automated retention dashboards
8. → A/B test features and track retention impact

## Support

For issues or questions:
- Check component debug logs
- Review sample integration in each script's comments
- Export data and inspect CSV files
- Enable Firebase debugging in Firebase Console
