# Game Integration Guide

## Overview

This guide explains how to integrate the Analytics System into your Puzzle Game Unity project to track player behavior, monetization, and engagement metrics.

## Setup (5 minutes)

### 1. Copy Analytics Scripts

Copy these files to your Unity project:

```
Assets/Scripts/Analytics/
├── AnalyticsManager.cs        # Main manager (singleton)
├── AnalyticsEvent.cs          # Event model
├── AnalyticsEventQueue.cs     # Offline queue
└── AnalyticsNetworkClient.cs  # Network communication
```

### 2. Configure Server URL

In `AnalyticsManager.cs`:

```csharp
[SerializeField] private string serverUrl = "http://localhost:5000/api";
```

**Development:** `http://localhost:5000/api`
**Production:** `https://your-api-domain.com/api`

### 3. Initialize in Game

In your game startup (e.g., `GameManager.cs`):

```csharp
public class GameManager : MonoBehaviour
{
    void Start()
    {
        // Initialize analytics
        AnalyticsManager.Instance.LogSessionStart();
    }

    void OnApplicationQuit()
    {
        // This is automatic, but you can add cleanup
    }
}
```

## Usage

### Track Level Events

```csharp
// When level starts
AnalyticsManager.Instance.LogLevelStarted(5);

// When level completes
AnalyticsManager.Instance.LogLevelCompleted(
    levelNumber: 5,
    moves: 20,
    stars: 3
);

// When level fails
AnalyticsManager.Instance.LogLevelFailed(
    levelNumber: 5,
    attempts: 2
);
```

### Track Monetization

```csharp
// IAP purchase
AnalyticsManager.Instance.LogIAPPurchase(
    productId: "coins_100",
    amount: 4.99m,
    currency: "USD"
);

// Ad watch
AnalyticsManager.Instance.LogAdWatch(
    adNetwork: "AdMob",
    adPlacement: "level_complete_rewarded"
);
```

### Track Sessions

```csharp
// Session start (automatic on first update)
AnalyticsManager.Instance.LogSessionStart();

// Session end (automatic on quit)
int sessionDurationSeconds = (int)(Time.time);
AnalyticsManager.Instance.LogSessionEnd(sessionDurationSeconds);
```

### Custom Events

```csharp
// Log any custom event
AnalyticsManager.Instance.LogEvent("feature_unlock", new Dictionary<string, object>
{
    { "feature_name", "power_ups" },
    { "unlock_level", 10 }
});
```

## Integration Examples

### Example 1: Level Manager

```csharp
public class LevelManager : MonoBehaviour
{
    private int _currentLevel;
    private int _moveCount = 0;
    private float _levelStartTime;

    void Start()
    {
        _currentLevel = 5;
        _moveCount = 0;
        _levelStartTime = Time.time;
        
        // Track level start
        AnalyticsManager.Instance.LogLevelStarted(_currentLevel);
    }

    public void MakeMove()
    {
        _moveCount++;
    }

    public void LevelComplete(int stars)
    {
        // Track completion
        AnalyticsManager.Instance.LogLevelCompleted(
            _currentLevel,
            _moveCount,
            stars
        );
        
        // Show reward
        ShowCompletionScreen();
    }

    public void LevelFailed()
    {
        // Count failures
        _failureCount++;
        
        // Track failure
        AnalyticsManager.Instance.LogLevelFailed(_currentLevel, _failureCount);
    }
}
```

### Example 2: Shop Manager

```csharp
public class ShopManager : MonoBehaviour
{
    public void BuyCoins(string productId)
    {
        // Process purchase via IAP provider
        IAP.Purchase(productId, (success) =>
        {
            if (success)
            {
                // Track the purchase
                decimal price = GetProductPrice(productId);
                string currency = GetProductCurrency(productId);
                
                AnalyticsManager.Instance.LogIAPPurchase(
                    productId,
                    price,
                    currency
                );
            }
        });
    }

    public void WatchRewardedAd()
    {
        // Show ad
        AdsManager.ShowRewardedAd(() =>
        {
            // Track ad watch
            AnalyticsManager.Instance.LogAdWatch(
                "Google AdMob",
                "reward_chest"
            );
            
            // Give reward
            GiveCoins(50);
        });
    }
}
```

### Example 3: Progression Tracking

```csharp
public class ProgressionManager : MonoBehaviour
{
    private int _levelUnlocksCount = 0;

    void Update()
    {
        // Check for milestones
        if (PlayerProgress.GetUnlockedLevelCount() > _levelUnlocksCount)
        {
            _levelUnlocksCount = PlayerProgress.GetUnlockedLevelCount();
            
            AnalyticsManager.Instance.LogEvent("progression_milestone", 
                new Dictionary<string, object>
                {
                    { "milestone_type", "level_unlock" },
                    { "total_unlocked", _levelUnlocksCount }
                });
        }
    }
}
```

## Event Tracking Best Practices

### ✅ DO Track

- Level starts/completions/failures
- In-app purchases
- Ad watches
- Session starts/ends
- Feature unlocks
- Progression milestones
- Tutorial completion
- Settings changes

### ❌ DON'T Track

- Every frame update (too verbose)
- Full object serialization
- Sensitive user data
- Personally identifiable information

### Data Privacy

**Never send:**
- Real names or emails
- Real addresses or phone numbers
- Device identifiers
- Payment card information

**Safe to send:**
- Anonymized player IDs (UUID)
- Country/region (coarse)
- Age group (if required)
- Game-related metrics only

## Offline Support

Events are automatically queued locally if the server is unreachable:

```
Game Start → Queue Event → Check Server → Success?
                            ↓
                          YES: Send → Clear Queue
                          NO: Keep in Queue
                          ↓
                          Game Close → Save Queue
                          ↓
                          Next Session → Retry Send
```

**Storage Location:** `Application.persistentDataPath/analytics_queue.json`

You can manually trigger flush:

```csharp
// In AnalyticsManager, add public method
public void ForceFlush()
{
    FlushEvents();
}
```

## Configuration

### Batch Settings

Edit `AnalyticsManager.cs`:

```csharp
[SerializeField] private int batchSize = 10;        // Events per batch
[SerializeField] private float flushInterval = 30f; // Flush every 30 seconds
```

**Recommendations:**
- **Batchsize**: 10-50 events (lower = more server requests)
- **Flush**: 30-60 seconds (lower = fresher data)

### Network Settings

Edit `AnalyticsNetworkClient.cs`:

```csharp
request.timeout = 10; // 10 second timeout
```

Increase for slow networks, decrease for real-time requirements.

## Testing

### Local Testing

1. Start backend server: `dotnet run` (backend directory)
2. Run game in Unity Editor
3. Open browser developer tools (F12)
4. Check Network tab for POST requests to `/api/events`
5. Verify in backend logs

### Test Events

```csharp
void TestAnalytics()
{
    // Log test event
    AnalyticsManager.Instance.LogEvent("test_event", 
        new Dictionary<string, object>
        {
            { "timestamp", System.DateTime.UtcNow },
            { "test_value", 42 }
        });
    
    Debug.Log("Test event logged");
}
```

### Verify in Dashboard

1. Build and run game
2. Open dashboard: http://localhost:3000
3. Search for player ID in Player Search
4. Verify events appear in dashboard

## Debugging

### Enable Debug Logs

In `AnalyticsManager.cs`, logs are already there:

```csharp
Debug.Log($"[Analytics] Flushed {events.Count} events to server");
```

Check Console window (Ctrl+Shift+C).

### Common Issues

**Problem:** Events not sending
- ✓ Check server is running (`http://localhost:5000/api/health`)
- ✓ Check server URL is correct
- ✓ Check network connection
- ✓ Check firewall rules

**Problem:** Data missing
- ✓ Check event was logged
- ✓ Check flush interval (may not send immediately)
- ✓ Check Player ID consistency
- ✓ Check event properties format

**Problem:** High latency
- ✓ Reduce `batchSize` to send more frequently
- ✓ Increase `flushInterval` to batch more events
- ✓ Check network speed
- ✓ Move server closer (CDN/regional)

## Advanced Usage

### Custom Event Properties

```csharp
// Complex properties
AnalyticsManager.Instance.LogEvent("level_attempt", 
    new Dictionary<string, object>
    {
        { "level", 5 },
        { "difficulty", "hard" },
        { "power_ups_used", new int[] { 1, 3, 5 } },
        { "time_spent_seconds", 120 },
        { "matched_combos", 3 }
    });
```

### Multiple Sessions

Each session gets unique ID (automatic):

```csharp
// Session 1
AnalyticsManager.Instance.LogSessionStart();

// ... player plays ...
// Events have session_id A

// Session 2 (new game start)
AnalyticsManager.Instance.LogSessionStart();

// ... player plays ...
// Events have session_id B (different)
```

### Rate Limiting

Implement local rate limiting to avoid spam:

```csharp
private Dictionary<string, float> _lastEventTime = new();

public void LogEventRateLimited(string eventType, float minInterval = 1f)
{
    if (_lastEventTime.TryGetValue(eventType, out float lastTime))
    {
        if (Time.time - lastTime < minInterval)
            return; // Skip, too soon
    }
    
    LogEvent(eventType);
    _lastEventTime[eventType] = Time.time;
}
```

## Monitoring

### Dashboard Metrics

After integrating, check dashboard for:

1. **DAU/MAU:** Player engagement
2. **Retention:** Player retention
3. **Level Difficulty:** Which levels are problematic
4. **Monetization:** Revenue per player
5. **Session Duration:** Game engagement

### Alerts to Watch

- Sudden drop in DAU (game issue?)
- High failure rate on specific level (too hard?)
- Low revenue (pricing issue?)
- Short sessions (bad user experience?)

## Publishing

Before release:

- [ ] Change server URL to production
- [ ] Test with build (not Editor)
- [ ] Verify privacy compliance
- [ ] Test offline queue behavior
- [ ] Performance test with 100k+ events
- [ ] Set up monitoring/alerts
- [ ] Document analytics to users
- [ ] Ensure GDPR compliance

## Support

For issues:
1. Check console logs
2. Check network requests (browser F12)
3. Check backend logs
4. Review event properties format
5. Test with simple event first

## Further Reading

- API Reference: `/root/.openclaw/workspace/PuzzleGameAnalyticsServer/API_REFERENCE.md`
- Server Setup: `/root/.openclaw/workspace/PuzzleGameAnalyticsServer/SETUP.md`
- Dashboard Guide: `/root/.openclaw/workspace/PuzzleGameAnalyticsDashboard/SETUP.md`
