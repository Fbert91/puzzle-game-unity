# Sample Analytics Reports

## Publisher Dashboard Report
Generated: 2024-02-20 14:30:45

```
=== PUZZLE GAME - RETENTION ANALYTICS DASHBOARD ===
Report Generated: 2024-02-20 14:30:45

--- KEY METRICS ---
Total Users: 5,200
D1 Retention: 38.2%
D7 Retention: 22.1%
D30 Retention: 9.8%
Avg Playtime (hours): 12.5
Total Revenue: $4,156.00
ARPPU: $0.80

--- MONETIZATION ---
% Players Spending: 4.3%
IAP Purchases: 223
Ad Watches: 2,341

--- CHURN SIGNALS ---
High-Churn Levels (>30% abandonment):
  Level 8: 35.2% abandonment (18 times)
  Level 15: 31.5% abandonment (15 times)

--- RECOMMENDATIONS ---
1. Monitor Level Churn: Check highlighted levels for difficulty balance
2. Feature Engagement: Track which features drive retention
3. Monetization: Test new IAP bundles to increase conversion
4. Ad Strategy: Optimize ad placement and frequency
5. Daily Retention: Focus on D1 and D7 retention improvements
```

## Retention Metrics Report (CSV Format)

```
Retention Metrics Report
Generated: 2024-02-20 14:30:45

USER PROFILE
Property,Value
Install Date,2024-01-20
Cohort,2024-01-20
Total Sessions,45
Total Playtime (hours),12.5
Login Days,23
Current Streak,3
Last Session,2024-02-20
Days Since Last Session,0
Status,ACTIVE

MONETIZATION
Property,Value
Total Revenue,$18.50
IAP Purchases,5
Has Spent,true
Ad Watches,34

LOGIN HISTORY (Last 30 Days)
Date
2024-01-20
2024-01-21
2024-01-22
2024-01-24
2024-01-26
2024-01-28
2024-02-01
2024-02-05
2024-02-10
2024-02-15
2024-02-18
2024-02-19
2024-02-20
```

## Level Churn Analysis (CSV Format)

```
Level Churn Analysis
Generated: 2024-02-20 14:30:45

Level ID,Times Started,Times Completed,Completion Rate,Times Abandoned,Abandonment Rate,Avg Time (s)
1,45,45,100.00%,0,0.00%,45.2
2,45,45,100.00%,0,0.00%,52.3
3,44,44,100.00%,0,0.00%,58.1
4,44,42,95.45%,2,4.55%,61.5
5,42,40,95.24%,2,4.76%,67.2
6,40,39,97.50%,1,2.50%,63.8
7,39,37,94.87%,2,5.13%,71.4
8,37,24,64.86%,13,35.14%,89.2 ← CHURN HOTSPOT!
9,24,23,95.83%,1,4.17%,58.9
10,23,22,95.65%,1,4.35%,65.3
15,15,11,73.33%,4,26.67%,98.5 ← DIFFICULTY SPIKE
16,11,10,90.91%,1,9.09%,62.4
```

## Cohort Analysis Report (CSV Format)

```
Cohort Analysis (Retention & Monetization)
Generated: 2024-02-20 14:30:45

Cohort Date,Users,D1 Retained,D1 Rate,D7 Retained,D7 Rate,D30 Retained,D30 Rate,ARPPU,LTV
2024-01-20,1000,350,35.00%,180,18.00%,65,6.50%,$0.68,$20.40
2024-01-27,850,310,36.47%,195,22.94%,98,11.53%,$0.78,$24.34
2024-02-03,1200,480,40.00%,276,23.00%,156,13.00%,$0.82,$25.62
2024-02-10,950,380,40.00%,220,23.16%,125,13.16%,$0.85,$26.28
2024-02-17,1200,456,38.00%,246,20.50%,72,6.00%,$0.82,$20.28

Observations:
- D1 retention: Stable 35-40% (good initial appeal)
- D7 retention: Trending up to 23% (improved progression!)
- D30 retention: Improving (latest cohort at 13% vs 6.5% first)
- ARPPU: Steady growth from $0.68 to $0.85 (8% improvement)
- Latest cohort (Feb 17) shows early churn on D30 (need 7+ more days of data)

Conclusion: Game is improving! Latest update helped retention.
```

## Feature Engagement Report (CSV Format)

```
Feature Engagement Analysis
Generated: 2024-02-20 14:30:45

Feature Name,Total Usage,Users,Per Session Avg,Retention Bonus,Conversion Bonus
level_completion,156,45,3.47,15.00%,0.00%
shop_visit,32,18,0.71,35.00%,20.00%
powerup_usage,45,28,1.00,45.00%,15.00%
hint_usage,67,35,1.49,-10.00%,5.00%
ad_watch,34,22,0.76,10.00%,25.00%
iap_purchase,5,5,0.11,0.00%,100.00%

Key Insights:
- Hint usage is high but shows negative retention bonus
  → Suggests difficulty is too high
  → Recommendation: Rebalance levels 8 and 15
  
- Shop visits show strong retention bonus (35%)
  → Players who visit shop are 35% more likely to return
  → Recommendation: Optimize shop UI and pricing
  
- Power-ups have highest retention bonus (45%)
  → Players who use power-ups are engaged
  → Recommendation: Make power-ups more discoverable
  
- Ad watching shows good conversion bonus (25%)
  → Players who watch ads are willing to spend
  → Recommendation: Increase ad reward value
```

## Trend Analysis

### 📈 Positive Trends
```
Week 1:  D1=35%, D7=18%, D30=6.5%
Week 2:  D1=36%, D7=21%, D30=10%    ← Improving!
Week 3:  D1=40%, D7=23%, D30=13%    ← Strong improvement!
         ARPPU: $0.68 → $0.82 (+20%)
```

**Action**: Whatever you changed in Week 2 is working! Keep it up.

### 📉 Negative Trend (Hypothetical)
```
Week 1:  D1=38%, D7=22%, D30=10%
Week 2:  D1=36%, D7=20%, D30=8%     ← Declining
Week 3:  D1=32%, D7=18%, D30=6%     ← Getting worse
         ARPPU: $0.85 → $0.72 (-15%)
```

**Action**: Emergency investigation needed! What changed? Rollback recent update?

## Executive Summary for Publishers

```
PUZZLE GAME - BUSINESS METRICS
Generated: February 20, 2024

RETENTION TRAJECTORY
✓ D1 Retention: 38.2% (Target: 30-40%) - ON TARGET
✓ D7 Retention: 22.1% (Target: 15-25%) - ON TARGET
✓ D30 Retention: 9.8% (Target: 8-12%) - ON TARGET
✓ Retention trending UPWARD month-over-month

MONETIZATION HEALTH
✓ ARPPU: $0.80 (Target: $0.70-1.00) - ON TARGET
⚠ Conversion Rate: 4.3% (Target: 3-5%) - LOW END
  → Opportunity: Optimize IAP pricing/bundling
✓ Ad Revenue: Growing 10% week-over-week

ENGAGEMENT QUALITY
✓ DAU/MAU Ratio: 2.1 (Target: >1.8) - GOOD
✓ Session Length: 15 min average - HEALTHY
✓ Feature adoption: 85% of players use power-ups

IDENTIFIED ISSUES
⚠ Level 8 & 15 have high abandonment (35%, 32%)
  → Action: Rebalance difficulty on these levels
  → ETA impact: +2-3% D7 retention

RECOMMENDATION: Game is healthy. Focus on:
1. Difficulty balance (fix levels 8 & 15)
2. Monetization optimization (increase conversion)
3. Content expansion (add more levels for D7 retention)

FORECAST:
- If current trends continue: $50K+ monthly revenue (projected)
- With fixes: $60K+ monthly revenue (2+ year forecast)
- Confidence: HIGH (data-driven decisions)
```

## Sample BigQuery Analysis

```sql
-- Monthly Retention Cohort Table
SELECT
  DATE(PARSE_TIMESTAMP('%Y%m%d', event_date)) as install_date,
  COUNT(DISTINCT user_id) as cohort_size,
  
  COUNTIF(DATE(PARSE_TIMESTAMP('%Y%m%d', login_date)) = 
    DATE_ADD(DATE(PARSE_TIMESTAMP('%Y%m%d', event_date)), INTERVAL 1 DAY)) as d1_retained,
  COUNTIF(DATE(PARSE_TIMESTAMP('%Y%m%d', login_date)) = 
    DATE_ADD(DATE(PARSE_TIMESTAMP('%Y%m%d', event_date)), INTERVAL 7 DAY)) as d7_retained,
  COUNTIF(DATE(PARSE_TIMESTAMP('%Y%m%d', login_date)) = 
    DATE_ADD(DATE(PARSE_TIMESTAMP('%Y%m%d', event_date)), INTERVAL 30 DAY)) as d30_retained,
  
  SUM(revenue) as total_revenue,
  AVG(revenue) as arppu
  
FROM `project.analytics_XXXXXXX.events_*`
WHERE event_name IN ('install', 'dau')
GROUP BY install_date
ORDER BY install_date DESC;

Results:
install_date  | cohort_size | d1_retained | d7_retained | d30_retained | arppu
2024-02-17    | 1200        | 456         | 240         | 72           | $0.82
2024-02-10    | 950         | 380         | 220         | 125          | $0.85
2024-02-03    | 1200        | 480         | 276         | 156          | $0.82
2024-01-27    | 850         | 310         | 195         | 98           | $0.78
2024-01-20    | 1000        | 350         | 180         | 65           | $0.68
```

## Data Export Formats

### JSON Export Sample

```json
{
  "export_date": "2024-02-20T14:30:45Z",
  "user_profile": {
    "user_id": "device_12345",
    "install_date": "2024-01-20",
    "total_sessions": 45,
    "total_playtime_seconds": 45000,
    "login_days": 23,
    "login_streak": 3,
    "last_session": "2024-02-20",
    "is_churned": false,
    "total_revenue": 18.50,
    "iap_purchases": 5,
    "has_spent": true,
    "ad_watches": 34
  },
  "level_churn_analysis": [
    {
      "level_id": 8,
      "times_started": 37,
      "times_completed": 24,
      "completion_rate": 0.6486,
      "times_abandoned": 13,
      "abandonment_rate": 0.3514,
      "avg_time_seconds": 89.2
    }
  ],
  "cohort_analysis": [
    {
      "cohort_date": "2024-02-17",
      "users": 1200,
      "d1_retained": 456,
      "d1_rate": 0.38,
      "d7_retained": 240,
      "d7_rate": 0.20,
      "arppu": 0.82
    }
  ]
}
```

---

## How to Use These Reports

1. **Share with Publisher**
   - Export dashboard summary (TXT format)
   - Include cohort analysis CSV
   - Add notes on key findings

2. **Internal Team Review**
   - Analyze level churn data weekly
   - Check feature engagement metrics
   - Track ARPPU trends

3. **Data Analysis**
   - Export to BigQuery for advanced analysis
   - Compare cohorts to identify successful features
   - Build automated alerts for retention drops

4. **Decision Making**
   - Use D1/D7/D30 trends to guide updates
   - Use churn data to prioritize bug fixes
   - Use feature metrics to guide feature development

All reports are generated automatically by the analytics system every hour.
