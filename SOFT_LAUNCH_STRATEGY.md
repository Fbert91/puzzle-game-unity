# Soft Launch Strategy - Canada Market Entry & Monitoring

**Target**: Execute soft launch to Canada market, monitor key metrics, optimize based on user feedback, and prepare for expansion to other countries.  
**Time**: Day 1-7 after launch  
**Focus**: Retention, bug fixing, monetization optimization  
**Result**: Validated game ready for broader release  

---

## TABLE OF CONTENTS
1. [Why Canada First?](#why-canada-first)
2. [Soft Launch Checklist](#soft-launch-checklist)
3. [Market Setup](#market-setup)
4. [Metrics Dashboard](#metrics-dashboard)
5. [Daily Monitoring](#daily-monitoring)
6. [Success Indicators](#success-indicators)
7. [Troubleshooting & Optimization](#troubleshooting--optimization)
8. [Expansion Timeline](#expansion-timeline)

---

## WHY CANADA FIRST?

### Strategic Advantages

**1. Low Competition**
- Fewer new puzzle games launching in Canada daily
- Less saturated market = better visibility
- Your app ranks higher in charts
- Better chance for organic growth

**2. English-Speaking Population**
- No localization needed (if US/UK English)
- Large audience to test with
- Easy feedback collection
- Forum discussions & reviews in English

**3. Similar to US Market**
- Canadian gamers have similar preferences to US
- If successful in Canada → likely successful in US
- Same ad networks, payment methods, laws
- Easier expansion path

**4. Manageable Scale**
- Canada population: 39 million
- ~70% own smartphones
- ~28 million potential users
- Manageable for monitoring/support

**5. Test Before Going Global**
- Small enough to see issues
- Large enough to get real data
- Can fix bugs before major launch
- Can optimize ads before scaling
- Can test retention mechanics

### Market Statistics

```
Canada Gaming Market
├─ Population: 39M (Android users: ~60%)
├─ Monthly Active Gamers: ~25M
├─ Casual Game Players: ~10M+
├─ Average Session: 15-30 minutes
├─ Revenue per user: $0.50-$3.00/month
└─ Growth rate: +5% year-over-year
```

---

## SOFT LAUNCH CHECKLIST

**Before going live, verify:**

- [ ] APK tested on real device (no crashes)
- [ ] AdMob account live with real ad unit IDs
- [ ] All ad formats working (banner, rewarded, interstitial)
- [ ] Google Play listing complete with screenshots
- [ ] Privacy policy published
- [ ] Content rating submitted
- [ ] App approved by Google Play
- [ ] App visible in Canada App Store
- [ ] Analytics tracking implemented (optional but recommended)
- [ ] Support email/contact set up
- [ ] Monitoring tools open and ready

---

## MARKET SETUP

### Step 1: Verify Canada Release

After approval, your app shows in **Google Play Store → Canada**:

1. **Go to**: Google Play Console → Release → Production
2. **Check Status**: Should show ✅ "LIVE" or "RELEASED"
3. **Verify Countries**: Canada should be listed
4. **Share link**:
   ```
   https://play.google.com/store/apps/details?id=com.yourname.puzzlegame
   ```

### Step 2: Testers & Early Access

**Invite people to test:**
1. Share Play Store link with 50-100 people:
   - Friends, family, online communities
   - Puzzle game forums
   - Mobile gaming communities
   - Discord/Reddit communities
2. **Gather feedback**:
   - How hard is the game?
   - Is difficulty progression good?
   - Do ads feel intrusive?
   - Are rewards compelling?
   - Any crashes/bugs?

### Step 3: Analytics Setup (Recommended)

**If using Firebase Analytics**:
1. Unity → Window → Google Cloud → Firebase
2. Enable **Analytics**
3. Track events:
   - Level completed
   - Ad impressions
   - Ad clicks
   - Session duration
   - Game overs

**Without analytics**:
- Monitor via Google Play Console only
- Less detailed but still useful

---

## METRICS DASHBOARD

### Key Metrics to Track

#### 1. **Installation Metrics**

| Metric | Target | Good Sign |
|--------|--------|-----------|
| Day 1 Installs | 10-100 | Growing |
| Day 7 Installs | 100-500 | Consistent growth |
| Total Installs | 1000+ | Strong initial uptake |
| Install Growth | Day-over-day +10% | Momentum |

**How to check**: Google Play Console → Acquisition → Installs (top chart)

#### 2. **Retention Metrics** (MOST IMPORTANT)

| Metric | Target | What It Means |
|--------|--------|---------------|
| Day 1 Retention | 30-50% | % who play again next day |
| Day 7 Retention | 10-20% | % still playing 1 week later |
| Day 30 Retention | 3-5% | % still playing 1 month later |

**How to check**: Google Play Console → Retention (chart by day)

**Rule of thumb**:
- **<20% D1 retention** = Game is too hard or boring (fix gameplay)
- **20-50% D1 retention** = Good (industry average ~20-40%)
- **>50% D1 retention** = Excellent (means game is addictive)

#### 3. **Crash Metrics**

| Metric | Target | What It Means |
|--------|--------|---------------|
| Crash-free users | >98% | % of users with no crashes |
| ANR (App Not Responding) | 0 | App freezes (bad) |
| Crash rate | <2% | Acceptable |

**How to check**: Google Play Console → Analytics → Crashes

**If crashes >5%**:
- 🚨 **URGENT**: Fix crashes immediately
- Upload new version ASAP
- Communicate fix in update notes

#### 4. **Revenue Metrics**

| Metric | Target | What It Means |
|--------|--------|---------------|
| Daily Active Users (DAU) | 50+ | People playing daily |
| Monthly Active Users (MAU) | 500+ | People playing monthly |
| Ad Impressions | 100+ per install | How many ads shown |
| Click-through Rate (CTR) | 1-3% | % who click ads |
| Revenue per 1000 impressions (RPM) | $1-5 | Ad earnings efficiency |

**Calculation**:
- If DAU = 100 users
- Average 10 ads per session = 1,000 ad impressions
- RPM = $2
- Daily revenue = (1,000 × $2) / 1,000 = **$2/day**

#### 5. **User Ratings**

| Metric | Target | What It Means |
|--------|--------|---------------|
| Average Rating | 4.0+ stars | User satisfaction |
| 5-star reviews | 40%+ | Happy players |
| 1-2 star reviews | <20% | Indicates issues |

**How to check**: Google Play Console → Ratings (star chart)

---

## DAILY MONITORING

### **Monitoring Checklist (Do This Every Day)**

#### **Morning Check (5 min)**
```
☐ Installs: Did game grow overnight?
☐ Crashes: Any new crash reports?
☐ Ratings: Any 1-2 star reviews? Read them
☐ Revenue: Any earnings in AdMob?
```

#### **Afternoon Check (10 min)**
```
☐ Engagement: How many DAU today?
☐ Retention: Day 1 retention on track?
☐ Support: Any user emails/messages?
☐ Feedback: Good user reviews to respond to
```

#### **Weekly Check (30 min, Friday)**
```
☐ Retention: Week 1 retention number?
☐ Monetization: Weekly revenue total
☐ Crashes: Fix any new crashes identified
☐ Features: Needed improvements identified?
```

### **Specific Dashboards to Check**

#### **Google Play Console:**

1. **Acquisition** (→ All users)
   - Installs: Growing?
   - Uninstalls: Low?
   - Rating changes

2. **Retention** (→ Retention curve)
   - D1, D7, D30 retention
   - Should decline gradually, not cliff-drop

3. **Crashes** (→ Vitals → Crashes)
   - Total crashes
   - Affected users
   - Top crash reasons
   - Fix immediately if >5%

4. **Ratings** (→ Reviews)
   - New reviews
   - Common complaints
   - Common praise

#### **AdMob Console:**

1. **Home** (→ Overview)
   - Total revenue (today/week/month)
   - Impressions
   - CTR (click-through rate)

2. **Apps** (→ Your app)
   - Revenue by ad unit
   - Which ads earn most?
   - Which ads have highest CTR?

#### **Firebase Analytics** (if using):

1. **Dashboard** (→ Analytics)
   - Active users
   - Top events
   - User demographics
   - Session duration

### **Example Daily Report Template**

```
═══════════════════════════════════════════════════
SOFT LAUNCH REPORT - Day 3
═══════════════════════════════════════════════════

📊 INSTALLS
  ├─ Total Installs: 127 ✅ (up from 89)
  ├─ Day 1 Installs: 45
  ├─ Uninstalls: 3
  └─ Active Users: 89

🎮 RETENTION  
  ├─ D1 Retention: 38% ✅ (Good!)
  └─ Crashes: 0 ✅

💰 REVENUE
  ├─ Ad Impressions: 543
  ├─ Ad Revenue: $1.32
  └─ Estimated Monthly: $40

⭐ RATINGS
  ├─ Average: 4.3 stars ✅
  ├─ 5 stars: 12 reviews ✅
  └─ 1 star: 1 review ⚠️ (Difficulty too easy)

🐛 ISSUES
  ├─ Feedback: "Game is boring after level 10"
  ├─ Action: Plan difficulty increase
  └─ Priority: Medium

✅ STATUS: On track for Canada soft launch!
```

---

## SUCCESS INDICATORS

### Green Light (Continue as planned)

**Your game is succeeding if:**

```
✅ Day 1 Retention: 25%+ 
   → Players like your game enough to come back

✅ Installs Growing: +10-20% daily
   → Word of mouth working, organic growth

✅ Crash-Free: >98%
   → Technical quality is good

✅ High Ratings: 4.0+ average
   → Users happy with game

✅ Revenue Growing: Daily revenue >$0
   → Monetization is working

✅ No Major Complaints: <20% 1-2 star reviews
   → Game meets expectations
```

**Action**: Continue monitoring. Plan broader expansion to US/UK after 1-2 weeks.

### Yellow Light (Investigate & Optimize)

**Consider optimizations if:**

```
⚠️ Day 1 Retention: 15-25%
   → Still okay but below ideal
   → Action: Analyze where players drop off
   → Improve early game engagement

⚠️ Installs Plateauing: <5% daily growth
   → Game needs visibility boost
   → Action: Reach out to YouTubers for reviews
   → Consider small ad spend

⚠️ Crashes: 2-5%
   → Technical issues present
   → Action: Review crash logs, deploy fix
   → Test thoroughly before next release

⚠️ Low Ratings: 3.0-3.5 average
   → Players have specific complaints
   → Action: Read reviews, identify themes
   → Fix most-mentioned issues

⚠️ Low Revenue: <$0.50/day
   → Monetization not optimized
   → Action: Check ad placement
   → Adjust frequency of ads
   → Test different ad networks
```

**Action**: Optimize specific areas, monitor for 3-5 more days.

### Red Light (Major Changes Needed)

**If you see these, game needs rework:**

```
🔴 Day 1 Retention: <15%
   → Players hate the game
   → Action: Review core gameplay, improve difficulty curve
   → Consider significant gameplay changes

🔴 Installs Declining: Fewer installs each day
   → Players are leaving in droves
   → Action: Analyze what's going wrong
   → Fix fundamental issues before continuing

🔴 Crash Rate: >5%
   → 🚨 CRITICAL: Game breaking for many users
   → Action: Emergency bug fixes
   → Deploy hotfix ASAP (test thoroughly first)

🔴 Average Rating: <3.0 stars
   → Game has serious problems
   → Action: Read all 1-2 star reviews carefully
   → Identify pattern (too hard? boring? buggy?)
   → Plan major update

🔴 High Uninstall Rate: >50% DAU
   → More people leaving than staying
   → Action: Major gameplay/difficulty overhaul needed
```

**Action**: Pause expansion plans. Focus on fixing core issues.

---

## TROUBLESHOOTING & OPTIMIZATION

### Issue: "Low day-1 retention"

**Possible Causes**:
1. **Game is too hard** - Players can't beat first level
2. **Game is too easy** - No challenge, boring
3. **Slow progression** - Level 1-10 are identical
4. **Poor onboarding** - Unclear how to play
5. **Too many ads** - Players annoyed by ads

**Solutions**:
- Play first 10 levels yourself
- Is progression interesting?
- Is difficulty ramping correctly?
- Check analytics: where do players drop off?
- Reduce ad frequency in early game
- Add tutorial/help text

### Issue: "Crashes/app breaking"

**How to debug**:
1. **Go to**: Google Play Console → Crashes
2. **Read crash logs** (stack traces)
3. **Identify common crashes**
4. **Rebuild APK** with fix
5. **Test thoroughly** on device before uploading
6. **Upload new version** (version 1.1)
7. **Submit for review** (usually approved <1 hour)

### Issue: "Low ad revenue"

**Possible Causes**:
1. **Ad placement wrong** - Ads not visible to players
2. **Ad frequency too low** - Not showing enough ads
3. **Ad frequency too high** - Players see ads and leave
4. **Rewarded ads not compelling** - Players don't watch
5. **Region low-value** - Canada has lower CPM than US

**Solutions**:
- Check banner ad is positioned well
- Track engagement before/after ads
- Test different ad frequencies
- Make rewards more compelling
- Add more rewarded ad opportunities
- Consider expanding to higher-value countries later

### Issue: "Bad reviews - game too hard/easy"

**Read reviews** to identify pattern:

```
Multiple 1-star reviews saying:
"Level 5 is impossible" 
→ Difficulty spike needed adjustment

vs.

"After level 20, all same" 
→ Need variety/new mechanics
```

**Actions**:
1. Identify the problem
2. Plan fix (easier levels? harder challenge?)
3. Implement in next version
4. Update description/store listing
5. Respond to reviews with ETA of fix

### Issue: "App removed for policy violation"

**Causes** (unlikely but possible):
1. **Misleading description** - Store listing doesn't match game
2. **Ads policy** - Too many ads or inappropriate placement
3. **Privacy policy** - Missing required privacy info
4. **Crashes** - Too many users experiencing crashes

**Resolution**:
1. Read Google's email for specifics
2. Fix the violation
3. Resubmit app for review
4. Usually re-approved within 24 hours

---

## EXPANSION TIMELINE

### Week 1: Canada Soft Launch (NOW)

**Days 1-7**:
- Monitor installs, retention, crashes
- Fix any bugs found
- Gather user feedback
- Run marketing to early adopters
- Target: 500-1000 installs, 25%+ D1 retention

### Week 2: Optimize (After 7 days)

**Actions**:
- Analyze retention data
- Fix most common complaints
- Upload version 1.1 (if needed)
- Approach YouTubers for reviews
- Reach out to gaming communities

**Metrics to hit**:
- 30%+ D1 retention
- <2% crash rate
- 3.8+ star average
- $0.50+/day revenue

### Week 3: Cautious Expansion (After 14 days)

If metrics are good:

**New regions to add**:
1. **United States** (largest market)
   - 8x larger than Canada
   - Higher ad rates
   - More competition
2. **United Kingdom** (English-speaking)
3. **Australia** (English-speaking)

**Action**: Go to Google Play Console → Pricing & Distribution → Countries
- Add USA, UK, Australia
- Resubmit app for review (minor release)
- Monitor metrics in each region separately

### Month 2: Broader Expansion (After 30 days)

If soft launch successful:

**Expand to**:
- Western Europe (Germany, France, Spain)
- Japan (large gaming market)
- Southeast Asia (Indonesia, Philippines)

**Consider**:
- Localization (translate game)
- Regional marketing
- Region-specific features

---

## SUCCESS METRICS SUMMARY

**By Day 7, You Should Have**:

| Metric | Target | Min Acceptable |
|--------|--------|-----------------|
| Total Installs | 500+ | 200+ |
| D1 Retention | 30%+ | 20%+ |
| D7 Retention | 15%+ | 10%+ |
| Crash-Free Rate | 99%+ | 98%+ |
| Avg Rating | 4.0+ | 3.5+ |
| Daily Revenue | $1.00+ | $0.30+ |

**If you hit these, game is ready to expand to new regions!** 🚀

---

## NEXT STEPS

1. ✅ Launch to Canada
2. ✅ Monitor daily for 7 days
3. ✅ Optimize based on feedback
4. ✅ Expand to new regions
5. Follow: **POST_LAUNCH_OPTIMIZATION.md** for growth strategies

---

**✅ Soft launch strategy defined! You now have a clear monitoring plan for Canada release.**

