# Retention Metrics Guide

## What Is Retention and Why Does It Matter?

**Retention** is the percentage of players who return to play your game after installing it. It's the single most important metric for game success because:

- **High retention** = Players love your game
- **Low retention** = Something is wrong (difficulty, bugs, boredom)
- **Growing retention** = You're improving
- **Declining retention** = Red alert - investigate immediately

Publishers care deeply about retention because it directly predicts:
- Game lifetime value (how much money it will make)
- Ability to monetize (need players to stick around)
- Update success (did new content help retention?)
- Whether the game is good or just had good marketing

## Core Retention Metrics

### Day 1 Retention (D1)

**Definition**: Percentage of players who return the day after they install the game.

```
D1 Retention = (Players who played on Day 2) / (Total players who installed) × 100%
```

**Why it matters:**
- First impression test
- Shows if core loop is compelling
- If D1 is bad (<30%), your tutorial or first level is broken
- If D1 is good (>50%), you have a strong core game

**Healthy targets:**
- Casual games: 30-40%
- Mid-core games: 40-50%
- Hardcore games: 50-60%+

**What to do if D1 is low:**
- Check your onboarding/tutorial (too confusing?)
- Are first levels too hard? (Difficulty spike!)
- Is your game clear about what to do?
- Check for bugs or crashes on first play

**Example:**
```
Day 1: 1,000 players install
Day 2: 350 return
D1 Retention = 35%
```

---

### Day 7 Retention (D7)

**Definition**: Percentage of players who return 7 days after installation.

```
D7 Retention = (Players who played on Day 8) / (Total players who installed) × 100%
```

**Why it matters:**
- Tests if game has staying power
- One week out = players are comparing you to other games
- Shows if progression/rewards system works
- Indicates whether you have enough content

**Healthy targets:**
- Casual games: 15-25%
- Mid-core games: 20-35%
- Hardcore games: 30-50%+

**D7 vs D1 ratio tells you about content depth:**
```
If D1 = 35% and D7 = 20% → You're losing 43% of returners (content??)
If D1 = 35% and D7 = 28% → You're retaining 80% of returners (good depth)
```

**What to do if D7 is low:**
- Add more levels/content (players ran out of things to do)
- Improve progression rewards (not enough dopamine)
- Check level difficulty curve (difficulty spike = quit)
- Test social features (friend competition helps retention)

---

### Day 30 Retention (D30)

**Definition**: Percentage of players who return 30 days after installation.

```
D30 Retention = (Players who played on Day 31) / (Total players who installed) × 100%
```

**Why it matters:**
- **THE** metric publishers care about
- Shows long-term viability
- Predicts lifetime value (LTV)
- Indicates whether updates keep players engaged

**Healthy targets:**
- Casual games: 5-15%
- Mid-core games: 10-20%
- Hardcore games: 15-40%+

**Why D30 drops so much:**
```
Day 1:  35% (360 people)
Day 7:  20% (200 people) ← Players who don't find staying power leave
Day 30: 8%  (80 people)  ← Only loyal core remains
```

This is NORMAL. Not everyone will stay 30 days.

**What to do if D30 is growing:**
- New update worked!
- New feature is engaging
- You're retaining the right players

---

### Cohort Analysis

**Definition**: Group players by install date and track their retention curves.

```
Example Cohort (Players who installed on Feb 10):
  1,000 players installed
  D1: 350 returned (35%)
  D7: 200 returned (20%)
  D30: 80 returned (8%)
  ARPPU: $0.85
  
Example Cohort (Players who installed on Feb 20):
  1,200 players installed
  D1: 480 returned (40%) ← BETTER!
  D7: 276 returned (23%) ← BETTER!
  D30: 120 returned (10%) ← BETTER!
  ARPPU: $1.10 ← BETTER!
```

**Why it matters:**
- See if updates improved retention
- Identify which update helped or hurt
- Compare marketing campaigns
- Track progression over time

**What to look for:**
- ✅ Retention rates increasing week-over-week = success
- ❌ Retention rates decreasing = problem with recent update
- 📊 ARPPU increasing = monetization improving
- 📈 More players installing = marketing working

---

## Engagement Metrics

These predict retention - they tell you WHY players stick around.

### Session Metrics

**Session Length** (Average playtime per session)
```
Good: 10-20 minutes per session
Too short: <5 min (not enough to engage)
Too long: >30 min (maybe too addictive? Watch churn)
```

**Session Frequency** (How often players return)
```
Good: 1-3 sessions per day
Growing frequency: Players getting more engaged
Decreasing frequency: Engagement dropping - warning sign!
```

**Total Playtime** (Cumulative time)
```
Correlates with:
  - Retention (more playtime = more likely to return)
  - Monetization (more playtime = more IAP opportunities)
  - Difficulty (easy games have longer sessions)
```

### Activity Metrics

**Daily Active Users (DAU)**
```
DAU = How many unique players logged in today

Healthy:
  - Monday-Friday: ~60% of user base
  - Weekends: ~80% of user base
  - Trending up = retention improving
  - Trending down = retention declining
```

**Monthly Active Users (MAU)**
```
MAU = How many unique players logged in this month

MAU/DAU Ratio tells you about stickiness:
  - Ratio > 3.0 = Players log in multiple times (good)
  - Ratio < 2.0 = Players only log in once or twice (bad)
  - Example: 1000 MAU, 500 DAU = Ratio 2.0 (each player ~2 logins/month)
```

### Progression Metrics

**Level Progression Speed** (Levels per day)
```
If players do 2 levels/day:
  - Day 1: Level 2
  - Day 7: Level 14
  - Day 30: Level 60

Fast progression = Content consumption faster = Need more content
Slow progression = Players stuck on hard levels = Rebalance difficulty
```

---

## Feature Engagement Metrics

### Why Feature Engagement Predicts Retention

When players use certain features, they're more likely to return:

```
Feature Usage → Player Engagement → Player Retention → Player Spending
```

### Key Features to Track

**Hint Usage**
```
High hint usage = Players struggling (difficulty too hard)
Rising hint usage = Difficulty spike incoming (watch churn)
Action: If hints spike on level N, rebalance that level
```

**Power-up Usage**
```
Players who use power-ups are 30-50% more likely to return
Why? They're investing in the game (progression investment)
Monetization tie-in: Power-ups lead to IAP purchases
```

**Shop Visits**
```
Shop visits = Monetization signal (player interested in spending)
More visits + no purchases = Prices too high or wrong items
Action: Track which items are viewed vs bought
```

**Time in Shop**
```
Long shop sessions = Price comparing (player wants to spend!)
Short visits = Just browsing (not ready to buy)
```

**Ad Watches**
```
Willing to watch ads = Engaged (wants to continue playing)
No ad watches = Unengaged (doesn't care about ad reward)
Track this over time: Rising = engagement up, Falling = engagement down
```

### Retention Bonus

**Feature Engagement → Retention Multiplier**

Example:
```
Base retention (D7): 20%

Players who use hints:      20% × 1.5 = 30% (50% boost!)
Players who use power-ups:  20% × 2.0 = 40% (100% boost!)
Players who watch ads:      20% × 1.2 = 24% (20% boost)
Players who visit shop:     20% × 1.8 = 36% (80% boost)

Players who do ALL:         20% × 3.5 = 70% (350% boost!)
```

This means: **Get players to use features → They stay longer**

---

## Churn Analysis

### What Is Churn?

**Churn** = Players who stop playing. Measured as:

```
Churn Rate = (Players who quit) / (Total players) × 100%
             OR (1 - Retention Rate)

If D30 retention is 8%, churn is 92%
```

### Level-Based Churn

**Where do players quit?**

```
Level 1:   100 players, 95 complete (5% abandon)
Level 2:   95 players, 90 complete (5% abandon)
Level 5:   82 players, 60 complete (27% abandon) ← SPIKE!
Level 6:   60 players, 55 complete (8% abandon)
```

**Level 5 has a difficulty spike!** Fix it or lose players.

### Churn Signals to Watch

| Signal | Meaning | Action |
|--------|---------|--------|
| Hint usage rising | Frustration increasing | Check difficulty |
| Session length dropping | Engagement declining | Add content/features |
| Playtime decreasing | Boredom setting in | Update needed |
| Shop visits dropping | Monetization interest down | Price review |
| No login for 3+ days | At-risk player | Send push notification |
| No login for 7+ days | Churned (likely lost) | Win-back campaign |

### Identifying Churn Patterns

**Time-based churn:**
```
Many quits on level 10 = Specific difficulty spike
Many quits on day 3 = Content runs out or progression too fast
Many quits after seeing ads = Ad frequency/placement issue
```

**Cohort-based churn:**
```
Cohort A (old version): 8% D30 retention
Cohort B (new version): 12% D30 retention
New version is 50% better! Keep the changes.
```

---

## Monetization-Retention Link

### Revenue Per User (ARPPU)

**Definition**: Average money earned per player

```
ARPPU = Total Revenue / Total Players

Example:
  200 players
  50 spend money (IAP)
  Total spent: $200
  ARPPU = $200 / 200 = $1.00
```

**Why it correlates with retention:**
- Players who spend are invested = More likely to return
- Monetization features need engagement = Keeps players playing
- Good retention allows more monetization

### Conversion Rate

**Definition**: Percentage of players who spend money

```
Conversion = (Players who spent) / (Total players) × 100%

Healthy targets:
  Casual games: 2-5%
  Mid-core games: 5-10%
  Hardcore games: 10-20%+
```

### Lifetime Value (LTV)

**Definition**: Total money a player will spend over their lifetime

```
LTV = ARPPU × (Average days played)

Example:
  ARPPU: $1.00
  Average player stays 30 days
  LTV = $1.00 × 30 = $30 per user

If you can spend $5 to acquire, that's a 6x return!
```

**LTV grows with retention:**
```
If D30 retention is 8% (players stay ~30 days):
  LTV = $1.00

If you improve D30 to 15% (players stay ~45 days):
  LTV = $1.50 (50% increase!)

Better retention = More revenue per player
```

### Cohort LTV

**Definition**: Average LTV for players installed on same day

```
Cohort Feb 10 (old version):
  ARPPU: $0.85
  Avg playtime: 25 days
  LTV: $21.25

Cohort Feb 20 (new version):
  ARPPU: $1.10
  Avg playtime: 35 days
  LTV: $38.50 (81% increase!)
```

---

## Publisher Report Metrics

This is what publishers look at to decide if your game is good:

```
EXECUTIVE SUMMARY
================
Total Players: 5,000
D1 Retention: 38%          ← Is first experience good?
D7 Retention: 22%          ← Is progression compelling?
D30 Retention: 10%         ← Is it sustainable?

ARPPU: $1.20               ← Can it monetize?
% Spending: 4.2%           ← Do players buy?
Ad Watch Rate: 45%         ← Do players watch ads?

CHURN HOTSPOTS
===============
Level 8: 35% abandon       ← Too hard? Fix it.
Level 15: 28% abandon      ← Difficulty spike
Day 3: 40% drop            ← Content runs out? Add levels.

TREND
=====
Retention improving week-over-week ✓
Revenue per user growing ✓
Engagement increasing ✓

RECOMMENDATION: Ship it!
```

---

## How to Use This Data

### Daily (Check your dashboard)

```
✓ Is DAU trending up or down?
✓ Are new features being used?
✓ Any churn spikes on specific levels?
```

### Weekly (Review metrics)

```
✓ Is D1 in target range?
✓ Are levels causing abandonment?
✓ Are top features working?
✓ Is ARPPU improving?
```

### Monthly (Full analysis)

```
✓ D1/D7/D30 trends
✓ Cohort analysis (is latest cohort better?)
✓ LTV growth
✓ Feature impact (did new feature help retention?)
✓ Monetization optimization
```

### After Major Updates

```
✓ Did retention improve?
✓ Did ARPPU increase?
✓ Are new features engaged?
✓ Any new churn hotspots?
```

---

## Red Flags & Healthy Indicators

### 🚨 Red Flags

- D1 < 20% (something fundamentally broken)
- D7/D1 ratio < 50% (not enough content)
- D30 < 2% (game not viable)
- Cohort retention declining (recent update hurt)
- Sudden level abandonment spike (difficulty issue)
- DAU dropping rapidly (engagement problem)
- Session length dropping (boredom)
- Hint usage spiking (frustration)

### ✅ Healthy Indicators

- D1: 30-50% (strong core game)
- D7: 15-30% (good content depth)
- D30: 5-15% (viable game)
- D7/D1 ratio > 60% (good progression)
- ARPPU growing month-over-month
- Cohort retention stable or improving
- DAU trending up
- Feature engagement high
- Level progression smooth (no spikes)
- No critical churn hotspots

---

## Common Misinterpretations

### ❌ "Our D30 retention is 5%, so the game is bad"

✅ **Context matters:**
- Is this normal for game genre? (Yes for casual)
- What was it before? (Improving = good, declining = bad)
- What's your ARPPU? (High ARPPU can compensate)
- What's your DAU? (Growing retention > absolute numbers)

### ❌ "We had 10,000 installs but only 100 Day 1 logins (1% D1)"

✅ **Check:**
- Are installs real or bot traffic?
- Is your tutorial crashing?
- Is your game loading?
- Are players actually starting?

### ❌ "Our ARPPU is $0.10, we're doomed"

✅ **Not necessarily:**
- Genre matters (casual games have low ARPPU)
- Look at LTV instead
- Look at spending trend (growing?)
- Look at conversion (are spenders spending more?)

### ❌ "D1 is 35% but D7 is only 18%, something's wrong"

✅ **That's normal:**
- D1 to D7 drop ~45% is healthy
- This just means you need more content
- Add levels, not easier difficulty

---

## Setting Targets

### For Casual Puzzle Games

```
Target D1: 30-40%
Target D7: 18-25%
Target D30: 8-12%
Target ARPPU: $0.80-1.50
Target Conversion: 3-5%
```

### For Mid-Core Games

```
Target D1: 40-50%
Target D7: 25-35%
Target D30: 12-20%
Target ARPPU: $1.50-3.00
Target Conversion: 5-10%
```

### For Your Game

```
1. Play competitor games
2. Note their retention targets (estimate from market)
3. Set YOUR targets 10-20% higher
4. Build features to hit those targets
5. Measure and iterate
```

---

## Export and Share Data

```csharp
// Generate for publisher
string report = DataExporter.Instance.ExportPublisherDashboard();

// Generate for internal analysis
string retention = DataExporter.Instance.ExportRetentionMetricsCSV();
string churn = DataExporter.Instance.ExportLevelChurnCSV();
string cohorts = DataExporter.Instance.ExportCohortAnalysisCSV();

// All data for Big Query analysis
string json = DataExporter.Instance.ExportToJSON();
```

---

## Next Steps

1. **Get your first cohort data** (Wait 30 days)
2. **Calculate D1, D7, D30** (Use the dashboard)
3. **Compare to targets** (Are you on track?)
4. **Identify problems** (Where do players quit?)
5. **Fix and measure** (Did retention improve?)
6. **Repeat** (Never stop optimizing)

Remember: **Good retention is made, not found.** You build it through great game design, appropriate difficulty, engaging content, and smart monetization. Keep measuring, keep improving!
