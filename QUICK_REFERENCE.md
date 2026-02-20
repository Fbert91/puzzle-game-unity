# Quick Reference Card - Retention Analytics

## Initialize Analytics
```csharp
// In GameInitializer or your startup code
new GameObject("Analytics").AddComponent<Analytics>();
new GameObject("RetentionTracker").AddComponent<RetentionTracker>();
new GameObject("DataExporter").AddComponent<DataExporter>();
```

## Log Level Events
```csharp
// When level starts
Analytics.Instance.LogLevelStart(levelId);
Analytics.Instance.SyncLevelStartToRetention(levelId);

// When level completes
Analytics.Instance.LogLevelComplete(levelId, score, stars, moveCount, timeSeconds);
Analytics.Instance.SyncLevelCompleteToRetention(levelId, moveCount, timeSeconds, hadHint);

// When player quits
Analytics.Instance.SyncSessionQuitToRetention(currentLevelId);

// When hint is used
Analytics.Instance.LogHintUsed(levelId, hintCount);
Analytics.Instance.SyncHintUsageToRetention(levelId);
```

## Log Monetization Events
```csharp
// Purchase
Analytics.Instance.LogIAPPurchase(productId, price, "USD", token);
Analytics.Instance.SyncIAPToRetention(productId, price);

// Ad impression
Analytics.Instance.LogAdImpression("admob", "rewarded");

// Ad reward
Analytics.Instance.LogAdReward("admob", rewardType, amount);
Analytics.Instance.SyncAdWatchToRetention("admob");

// Shop visit
Analytics.Instance.SyncShopVisitToRetention();
```

## Export Data
```csharp
// Export all reports (CSV + JSON + Dashboard)
DataExporter.Instance.ExportAllReports();

// Or individual exports
string retention = DataExporter.Instance.ExportRetentionMetricsCSV();
string churn = DataExporter.Instance.ExportLevelChurnCSV();
string cohorts = DataExporter.Instance.ExportCohortAnalysisCSV();
string features = DataExporter.Instance.ExportFeatureEngagementCSV();
string publisher = DataExporter.Instance.ExportPublisherDashboard();
```

## Get Metrics
```csharp
// User retention metrics
var metrics = RetentionTracker.Instance.GetRetentionMetricsSummary();

// Current session metrics
var session = RetentionTracker.Instance.GetCurrentSessionMetrics();

// Level churn data (where players quit)
var churnData = RetentionTracker.Instance.GetLevelChurnData();

// Cohort analysis (D1/D7/D30 by install date)
var cohorts = RetentionTracker.Instance.GetCohortAnalysis();

// Feature engagement (what keeps players?)
var features = RetentionTracker.Instance.GetFeatureEngagement();
```

## Show Dashboard (Optional)
```csharp
// Show in-game analytics dashboard
AnalyticsDashboard.Instance.ShowDashboard();

// Get metrics for your own UI
var summary = AnalyticsDashboard.Instance.GetMetricsSummary();
```

---

## Key Metrics at a Glance

| Metric | Good | Target |
|--------|------|--------|
| **D1 Retention** | 30-50% | Check weekly |
| **D7 Retention** | 15-30% | Improving = success |
| **D30 Retention** | 5-15% | Publisher metric! |
| **ARPPU** | $0.80-2.00 | Growing = good |
| **Conversion** | 3-5% | Track spending % |
| **Level Abandon** | <30% each | >30% = fix that level |
| **Session Length** | 10-20 min | Too short = boring |
| **DAU/MAU Ratio** | >1.8 | Stickiness metric |

---

## Debug Keys (Add to Your Update Loop)
```csharp
// Show dashboard
if (Input.GetKeyDown(KeyCode.L))
    AnalyticsDashboard.Instance?.ShowDashboard();

// Export data
if (Input.GetKeyDown(KeyCode.E))
    DataExporter.Instance?.ExportAllReports();

// Print metrics
if (Input.GetKeyDown(KeyCode.D))
{
    var m = RetentionTracker.Instance?.GetRetentionMetricsSummary();
    foreach (var metric in m)
        Debug.Log($"{metric.Key}: {metric.Value}");
}
```

---

## Files to Update in Your Game

### GameInitializer.cs (New or Existing)
Add analytics initialization in Awake()

### LevelManager.cs  
Add 4 calls: `LogLevelStart()`, `LogLevelComplete()`, `SyncLevelStartToRetention()`, `SyncLevelCompleteToRetention()`

### MonetizationManager.cs
Add 2 calls: `LogIAPPurchase()`, `SyncIAPToRetention()`

### UIManager.cs (Optional)
Add: `SyncShopVisitToRetention()` when shop opens

### Any Hint Usage Code
Add: `SyncHintUsageToRetention()`

---

## What Gets Stored

**Automatically Tracked:**
- ✅ User install date
- ✅ Login dates (every day)
- ✅ Session count and playtime
- ✅ Level attempts and completions
- ✅ Feature usage (hints, power-ups, shop)
- ✅ Monetization (purchases, ads)
- ✅ Churn date (if inactive 7+ days)

**Calculated Automatically:**
- ✅ D1, D7, D30 retention
- ✅ Cohort analysis
- ✅ Level churn/abandonment
- ✅ Feature engagement metrics
- ✅ ARPPU and LTV

---

## Common Problems & Fixes

**Problem: No data showing**
- Check: Are components initialized?
- Fix: Verify Analytics.Instance != null in console
- Verify: Play a full game session (start to quit)

**Problem: CSV files empty**
- Check: Did you play the game long enough?
- Fix: Play > 1 level, make a purchase, watch an ad
- Verify: Check PlayerPrefs has data

**Problem: Retention values look wrong**
- Check: Device clock is correct?
- Fix: Data needs 30 days minimum
- Verify: Compare to sample reports

**Problem: Export button does nothing**
- Check: Is DataExporter initialized?
- Fix: Look at Application.persistentDataPath
- Verify: Files should appear there

---

## What to Do Each Week

**Monday:**
- Export metrics: `DataExporter.Instance.ExportAllReports()`
- Check D1 retention (is it stable?)
- Review level churn (any new problem levels?)

**Wednesday:**
- Monitor DAU (is it trending up/down?)
- Check feature engagement (are players using new features?)
- Review ARPPU (is monetization improving?)

**Friday:**
- Analyze weekly trends
- Note any cohort improvements
- Plan next week's fixes based on data

---

## What to Show Publisher

**Every Month:**
```
D1 Retention: XX%
D7 Retention: XX%
D30 Retention: XX%
ARPPU: $XX
Cohort Trend: ↑ (improving!)
No Critical Issues ✓
```

**When Updating Game:**
```
Before Update:
  D1: 35%, D7: 20%, D30: 8%

After Update:
  D1: 38%, D7: 23%, D30: 10%
  
Result: +3% D1, +15% D7, +25% D30 retention!
```

---

## Firebase Integration (Optional)

To enable cloud backup:

1. Import Firebase SDK (Asset Store)
2. Uncomment Firebase lines in Analytics.cs
3. Initialize Firebase at app start
4. Data auto-syncs to Firebase Analytics
5. BigQuery queries available after 24-48 hours

See **ANALYTICS_SETUP.md** for full Firebase setup.

---

## Files to Copy to Project

1. `Assets/Scripts/RetentionTracker.cs` - Core tracking
2. `Assets/Scripts/DataExporter.cs` - Export/reporting
3. `Assets/Scripts/AnalyticsDashboard.cs` - Optional UI

**Already Updated:**
- `Assets/Scripts/Analytics.cs` - Enhanced with sync methods

---

## Export Path

Files saved to: `Application.persistentDataPath/Analytics/`

- **Editor**: `Assets/../PlayerPrefs.asset` or temp folder
- **Mobile**: App-specific documents folder
- **Standalone**: Game folder or `%APPDATA%`

All files timestamped: `retention_2024-02-20_14-30-45.csv`

---

## Documentation to Read

1. **ANALYTICS_SETUP.md** - How to set up Firebase
2. **RETENTION_GUIDE.md** - What every metric means (IMPORTANT!)
3. **INTEGRATION_GUIDE.md** - Exact code to add
4. **SAMPLE_REPORTS.md** - Example output

---

## 30-Day Checklist

- [ ] **Week 1**: Analytics running, data logging
- [ ] **Week 2**: Playing game, checking for errors
- [ ] **Week 3**: First cohort completing, D1/D7 visible
- [ ] **Week 4**: D30 retention data available!
- [ ] **Week 4**: Export and share with publisher
- [ ] **Week 4**: Identify retention problems
- [ ] **Week 5+**: Iterate based on data

---

## Success Metrics

**You're winning if:**
- D1 retention > 30% (strong first impression)
- D7/D1 ratio > 60% (good content depth)
- D30 retention > 5% (viable game)
- ARPPU growing month-over-month
- No critical level churn spikes
- DAU trending upward
- Publisher is happy with reports

**Red flags:**
- D1 < 20% (core game broken?)
- D30 < 2% (not viable)
- Cohort retention declining (recent update bad?)
- Sudden level abandonment spike
- ARPPU dropping

---

## One-Line Summary

**Bert now has:** A complete retention analytics system that tracks every player's journey, identifies where they quit, proves retention to publishers, and exports data for analysis. Ready to ship! 🚀

---

Keep this card near your desk. You'll reference it daily!
