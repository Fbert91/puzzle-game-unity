# Monetization Guide - Puzzle Game Unity

Complete guide to implementing and scaling monetization (IAP, ads, analytics).

## Monetization Strategy Overview

### Phase 1: Launch (Minimal)
- **Ads:** Optional (watch for coins)
- **IAP:** Power-ups & cosmetics only (no P2W)
- **Focus:** Great game experience, build DAU

### Phase 2: Growth (Scale)
- **Ads:** Banner ads + rewarded videos
- **IAP:** Expand cosmetics, seasonal booster packs
- **Analytics:** Monitor retention, optimize placement

### Phase 3: Mature (Optimize)
- **Ads:** Scale across best-performing placements
- **IAP:** High-value offers based on user cohorts
- **Analytics:** Predict LTV, refine pricing

## In-App Purchases (IAP)

### Product Categories & Pricing

#### Power-ups (Consumable)
```
hint_pack_5           $0.99  (5 hints)
reveal_tile_3         $0.99  (reveal 3 tiles)
skip_level            $1.99  (skip current level)
```

**Strategy:**
- Low entry price ($0.99) for first-time buyers
- Consumable (repurchase encourages revenue)
- Non-essential (game playable without)

#### Cosmetics (Non-consumable)
```
character_skin_*      $1.99-2.99
theme_neon            $1.99
tile_design_*         $0.99-1.99
```

**Strategy:**
- Appeal to players who love the game
- No gameplay advantage
- Encourage self-expression

#### Boosters (Limited-time)
```
booster_2x_score_7d   $4.99
unlimited_hints_3d    $2.99
```

**Strategy:**
- Urgency (limited time)
- High perceived value
- Works for mid-core players

#### Starter Pack (Discounted)
```
starter_pack          $4.99  (100 coins + hints)
```

**Strategy:**
- Best value (30% discount)
- First purchase incentive
- Builds confidence in spending

#### Currency Packs (Gems)
```
gems_100              $0.99
gems_550              $4.99  (best value)
gems_2500             $19.99
```

**Strategy:**
- Tiered pricing (economies of scale)
- Premium currency for special items
- Drives higher ARPU

### Setting Up IAP

#### Android (Google Play Billing)

1. **Create Products in Google Play Console:**
   - Go to Google Play Console → Select app
   - Monetization → Products → In-app products
   - Create each product:
     - Product ID: `hint_pack_5` (no spaces/special chars)
     - Name: "Hint Pack (5)"
     - Description: "Get 5 extra hints"
     - Price: $0.99
     - Type: Consumable (for power-ups)

2. **Integrate Google Play Billing Library:**
   ```csharp
   // In MonetizationManager.cs
   using Google.Play.Billing;
   
   BillingClient billingClient = BillingClient.newBuilder(context)
       .setListener(purchaseUpdateListener)
       .enablePendingPurchases()
       .build();
   ```

3. **Handle Purchases:**
   ```csharp
   BillingFlowParams billingParams = BillingFlowParams.newBuilder()
       .setSkuDetails(skuDetails)
       .build();
   billingClient.launchBillingFlow(activity, billingParams);
   ```

#### iOS (StoreKit 2)

1. **Create Products in App Store Connect:**
   - Go to App Store Connect → Select app
   - In-App Purchases → Create new
   - Reference Name: `hint_pack_5`
   - Product ID: `com.yourcompany.puzzlegame.hint_pack_5`
   - Type: Consumable
   - Price: $0.99

2. **Integrate StoreKit 2:**
   ```csharp
   // iOS IAP with StoreKit 2
   using StoreKit;
   
   Product? product = try await Product.products(for: ["com.yourcompany.puzzlegame.hint_pack_5"])
   var result = try await product.purchase()
   ```

#### Cross-Platform Implementation

The project already has `MonetizationManager.cs` with abstraction:

```csharp
// Usage (platform-agnostic)
MonetizationManager.Instance.PurchaseProduct("hint_pack_5");

// Listen to success/failure
MonetizationManager.Instance.OnPurchaseSuccess += (productId, quantity) => {
    Debug.Log($"Purchased {productId}");
};
```

### Testing IAP

#### Android Sandbox Testing
1. Add test accounts in Google Play Console
2. Use test email in device settings
3. Purchases marked as "Test"

#### iOS Sandbox Testing
1. Create Test User in App Store Connect
2. Sign out App Store
3. Attempt purchase, sign in with test account
4. Purchases are free in sandbox

## Advertising (Ads)

### Ad Network Integration

#### Google AdMob (Recommended)

1. **Create AdMob Account:**
   - Go to https://admob.google.com/
   - Sign in with Google account
   - Create app

2. **Get Ad Unit IDs:**
   - Banner: `ca-app-pub-3940256099942544/6300978111` (test)
   - Rewarded: `ca-app-pub-3940256099942544/5224354917` (test)
   - Interstitial: `ca-app-pub-3940256099942544/1033173712` (test)

3. **Integrate Google Mobile Ads SDK:**
   ```csharp
   using GoogleMobileAds.Api;
   
   MobileAds.Initialize();
   
   bannerView = new BannerView("ca-app-pub-3940256099942544/6300978111",
       AdSize.Banner, AdPosition.Bottom);
   bannerView.LoadAd(new AdRequest.Builder().Build());
   ```

### Ad Placement Strategy

#### Rewarded Video Ads (Priority)
- **Where:** "Watch ad for coins" button in menu
- **Frequency:** Once per session minimum
- **Reward:** 50-100 coins
- **Why:** High CTR, good revenue, player benefits

#### Banner Ads (Optional)
- **Where:** Main menu bottom, level select bottom
- **Frequency:** Always visible (but can be hidden)
- **Size:** 320x50 (mobile) or 300x250
- **Why:** Passive revenue, low friction

#### Interstitial Ads (Careful)
- **Where:** After level completion (optional view)
- **Frequency:** Max 1-2 per session
- **Why:** High CPM but high exit risk
- **Warning:** Don't force, let player skip after 3 seconds

### Ad Mediation (Multiple Networks)

For better fill rates and revenue, use mediation:

```csharp
// Example with AdMob Mediation
MobileAds.Initialize();

// Configure networks (AdMob handles routing)
string[] networkIds = new string[] {
    "ca-app-pub-3940256099942544/1033173712", // AdMob
};
```

### Implementing Rewarded Ads

```csharp
private RewardedAd rewardedAd;

public void LoadRewardedAd()
{
    AdRequest request = new AdRequest.Builder().Build();
    
    RewardedAd.Load("ca-app-pub-3940256099942544/5224354917",
        request, (RewardedAd ad, LoadAdError error) =>
    {
        rewardedAd = ad;
        rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            // Ad closed - load next one
            LoadRewardedAd();
        };
    });
}

public void ShowRewardedAd(string rewardType)
{
    if (rewardedAd != null && rewardedAd.CanShowAd())
    {
        rewardedAd.Show((Reward reward) =>
        {
            // Grant reward
            int amount = int.Parse(reward.Amount.ToString());
            MonetizationManager.Instance.SimulateAdReward(rewardType, amount);
        });
    }
}
```

## Analytics Integration

### Firebase Analytics

1. **Setup Firebase:**
   ```bash
   # Already configured in Analytics.cs
   # Just enable in Firebase Console
   ```

2. **Track Key Events:**
   ```csharp
   // Level completion
   Analytics.Instance.LogLevelComplete(1, 1000, 3, 15, 120f);
   
   // IAP purchase
   Analytics.Instance.LogIAPPurchase("hint_pack_5", 0.99f, "USD", "token");
   
   // Ad impression
   Analytics.Instance.LogAdImpression("admob", "rewarded");
   ```

3. **Monitor Metrics in Firebase Console:**
   - Daily Active Users (DAU)
   - Retention Day 1/7/30
   - Session duration
   - Level completion rates
   - Revenue by country

### Key Metrics to Monitor

#### Engagement
- **DAU:** Daily Active Users
- **MAU:** Monthly Active Users
- **Session Length:** Avg playtime per session
- **Level Completion %:** % of players finishing level X

#### Retention
- **D1 Retention:** % returning after 1 day
- **D7 Retention:** % returning after 7 days
- **D30 Retention:** % returning after 30 days
- **Churn Rate:** % of players who quit

#### Monetization
- **ARPU:** Average Revenue Per User ($)
- **ARPPU:** Average Revenue Per Paying User
- **Conversion %:** % who make any purchase
- **LTV:** Lifetime Value per player
- **CPI:** Cost per install (for paid ads)
- **Payback Period:** Days to recoup CPI

### Dashboard Setup

Create a simple dashboard to monitor:

```json
{
  "metrics": {
    "dau": 1000,
    "d1_retention": 0.45,
    "d7_retention": 0.15,
    "avg_session_length_min": 8,
    "level_completion_rate": 0.62,
    "conversion_rate_pct": 0.032,
    "arpu": 0.18,
    "revenue_today": 180
  }
}
```

## Pricing & Revenue Optimization

### Pricing Strategy

**Standard Mobile Game Pricing:**
- Entry ($0.99): Power-ups, cosmetics
- Mid ($2.99-4.99): Booster packs, cosmetics
- Premium ($19.99+): Currency bundles, limited items

**For Kids/Teens:**
- Lower prices overall ($0.99-2.99)
- More consumables (hints, power-ups)
- Fewer high-ticket items

### Price Testing

A/B test prices:
```
Version A: $0.99, $2.99, $4.99
Version B: $1.49, $3.99, $6.99
→ Measure conversion, revenue
→ Use winner for 30 days
→ Test next tier
```

### Seasonal Events

Increase revenue with limited-time offers:

**Seasonal Boosts:**
- Holiday packs (20-30% discount)
- Limited cosmetics (FOMO)
- Temporary 2x rewards events
- Battle pass alternative (cosmetic progression)

## P2W (Pay-to-Win) Prevention

### Ensure Fair Play

✅ **DO:**
- Power-ups provide convenience, not advantage
- Cosmetics are visual only
- All players can complete all levels without paying
- Hints are optional (puzzle is solvable without)
- Boosters increase reward, not difficulty reduction

❌ **DON'T:**
- Lock levels behind paywall
- Make paid character stronger
- Require gems to progress
- Create P2W mechanics

### Verify Balance

```csharp
// Test: Complete level without any purchases
// Result: Should take ~5-10 minutes
// If > 20 min, reduce difficulty or add hints
```

## Implementation Checklist

- [ ] **IAP Setup:**
  - [ ] Create products in Google Play Console
  - [ ] Create products in App Store Connect
  - [ ] Integrate billing libraries
  - [ ] Test purchases in sandbox
  - [ ] Test consumable/non-consumable correctly handled

- [ ] **Ads Setup:**
  - [ ] Create AdMob account
  - [ ] Configure ad units
  - [ ] Integrate SDK
  - [ ] Test ads in dev mode
  - [ ] Verify reward delivery

- [ ] **Analytics Setup:**
  - [ ] Create Firebase project
  - [ ] Enable analytics
  - [ ] Implement tracking events
  - [ ] Verify data in console
  - [ ] Create dashboards

- [ ] **Monetization Testing:**
  - [ ] IAP purchase flow works
  - [ ] Ad reward delivered correctly
  - [ ] Currency persists after app restart
  - [ ] Cosmetics/boosters work
  - [ ] No crashes during transactions

## Revenue Targets

### Conservative (Realistic)
- ARPU: $0.20-0.50
- Conversion: 2-5%
- D7 Retention: 15-25%

### Optimistic (With Polish)
- ARPU: $0.50-1.50
- Conversion: 5-15%
- D7 Retention: 30-50%

**Example Math:**
```
10,000 DAU × 20% D7 Retention = 2,000 D7 Users
2,000 × 3% Conversion = 60 payers
60 × $0.50 ARPU = $30/day
$30/day × 30 days = $900/month
```

## Compliance & Legal

- [ ] **Privacy Policy:** Disclose data collection (Analytics, ads)
- [ ] **Terms of Service:** Explain IAP, refunds
- [ ] **Age Compliance:** COPPA (US, <13) and GDPR (EU, any age)
- [ ] **Refund Policy:** 48-hour refund on purchases

---

**Last Updated:** 2026-02-20
**Next Review:** After soft launch analytics
