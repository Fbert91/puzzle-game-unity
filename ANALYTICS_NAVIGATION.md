# Analytics System - Navigation Guide

## Start Here 👇

### For the Impatient (5 minutes)
1. Read: **QUICK_REFERENCE.md** - Code snippets to copy/paste
2. Copy: 3 scripts to Assets/Scripts/
3. Go: Follow integration checklist
4. Done: Export data and see results

### For the Thorough (1 hour)
1. Read: **README_ANALYTICS.md** - System overview
2. Read: **RETENTION_GUIDE.md** - Understand metrics
3. Read: **INTEGRATION_GUIDE.md** - Step-by-step integration
4. Copy: Scripts and add to game
5. Test: Play game and export data

### For the Deep Dive (2-3 hours)
1. Read: **README_ANALYTICS.md** - Overview
2. Read: **RETENTION_GUIDE.md** - All metrics explained
3. Read: **INTEGRATION_GUIDE.md** - Integration
4. Read: **ANALYTICS_SETUP.md** - Firebase setup
5. Read: **SAMPLE_REPORTS.md** - Example output
6. Review: **QUICK_REFERENCE.md** - Keep for reference
7. Implement: Full integration with all features

---

## File Navigation

### 📋 Core Documentation (READ THESE)

| File | Size | Time | Purpose |
|------|------|------|---------|
| **README_ANALYTICS.md** | 15K | 10 min | Start here - System overview |
| **RETENTION_GUIDE.md** | 15K | 20 min | ⭐ Understanding every metric |
| **INTEGRATION_GUIDE.md** | 16K | 30 min | Step-by-step code integration |
| **QUICK_REFERENCE.md** | 8K | 5 min | Code snippets & checklists |

### 🚀 Setup & Configuration

| File | Size | Time | Purpose |
|------|------|------|---------|
| **ANALYTICS_SETUP.md** | 12K | 15 min | Firebase + BigQuery setup |
| **SAMPLE_REPORTS.md** | 9K | 10 min | Example outputs & analysis |

### 📚 Reference

| File | Size | Time | Purpose |
|------|------|------|---------|
| **ANALYTICS_IMPLEMENTATION_COMPLETE.md** | 11K | 5 min | Summary of what was built |

---

## Code Files

### Required Scripts (Copy to Assets/Scripts/)

```
RetentionTracker.cs (22K)
├─ Core retention tracking system
├─ Tracks D1, D7, D30 retention
├─ Level churn analysis
├─ Feature engagement metrics
└─ Cohort analysis

DataExporter.cs (18K)
├─ CSV/JSON export
├─ Publisher dashboard generation
├─ File saving with timestamps
└─ Multiple export formats

AnalyticsDashboard.cs (12K) [OPTIONAL]
├─ In-game UI dashboard
├─ Real-time metrics display
├─ One-click exports
└─ Publisher report button
```

### Enhanced Scripts (Already Updated)

```
Analytics.cs (14K)
├─ Sync methods for RetentionTracker
├─ Integration with new system
└─ Maintains backward compatibility
```

---

## Which File Should I Read?

### "I need to integrate this NOW"
→ **QUICK_REFERENCE.md**

### "I don't understand what retention means"
→ **RETENTION_GUIDE.md**

### "How do I add this to my game?"
→ **INTEGRATION_GUIDE.md**

### "What are retention metrics?"
→ **RETENTION_GUIDE.md** (also SAMPLE_REPORTS.md)

### "How do I use Firebase?"
→ **ANALYTICS_SETUP.md**

### "What gets exported?"
→ **SAMPLE_REPORTS.md**

### "What was actually built?"
→ **README_ANALYTICS.md** or **ANALYTICS_IMPLEMENTATION_COMPLETE.md**

### "Where do I start?"
→ **README_ANALYTICS.md**

### "I need code examples"
→ **QUICK_REFERENCE.md** or **INTEGRATION_GUIDE.md**

---

## Reading Order (Recommended)

### If you have 30 minutes:
1. README_ANALYTICS.md (10 min)
2. QUICK_REFERENCE.md (5 min)
3. INTEGRATION_GUIDE.md (15 min)

### If you have 1 hour:
1. README_ANALYTICS.md (10 min)
2. RETENTION_GUIDE.md (20 min)
3. QUICK_REFERENCE.md (5 min)
4. INTEGRATION_GUIDE.md (25 min)

### If you have 2+ hours:
1. README_ANALYTICS.md (10 min)
2. RETENTION_GUIDE.md (20 min)
3. INTEGRATION_GUIDE.md (25 min)
4. ANALYTICS_SETUP.md (15 min)
5. SAMPLE_REPORTS.md (10 min)
6. QUICK_REFERENCE.md (5 min)

---

## Quick Links by Question

### "How do I..."

**...set up analytics?**
→ INTEGRATION_GUIDE.md + QUICK_REFERENCE.md

**...understand D1 retention?**
→ RETENTION_GUIDE.md (section: Core Retention Metrics)

**...know if retention is good?**
→ RETENTION_GUIDE.md (section: Healthy Indicators)

**...export data?**
→ QUICK_REFERENCE.md + INTEGRATION_GUIDE.md

**...share with publisher?**
→ SAMPLE_REPORTS.md + QUICK_REFERENCE.md

**...use Firebase?**
→ ANALYTICS_SETUP.md

**...interpret churn data?**
→ RETENTION_GUIDE.md (section: Churn Analysis)

**...improve retention?**
→ RETENTION_GUIDE.md (section: What to do if XX is low)

**...debug issues?**
→ INTEGRATION_GUIDE.md (Troubleshooting section)

**...see example reports?**
→ SAMPLE_REPORTS.md

---

## Integration Checklist

### Phase 1: Setup (Day 1)
- [ ] Copy RetentionTracker.cs to Assets/Scripts/
- [ ] Copy DataExporter.cs to Assets/Scripts/
- [ ] Copy AnalyticsDashboard.cs to Assets/Scripts/ (optional)
- [ ] Read RETENTION_GUIDE.md
- [ ] Read QUICK_REFERENCE.md

### Phase 2: Integration (Day 2-3)
- [ ] Follow INTEGRATION_GUIDE.md
- [ ] Add analytics init to GameInitializer
- [ ] Add LogLevelStart/Complete calls
- [ ] Add LogIAPPurchase calls
- [ ] Add shop/ad tracking calls
- [ ] Test with debug commands

### Phase 3: Verification (Day 4)
- [ ] Play game for full session
- [ ] Export data
- [ ] Check CSV files created
- [ ] Verify data looks correct
- [ ] Compare to SAMPLE_REPORTS.md

### Phase 4: Optimization (Day 5+)
- [ ] Monitor retention daily
- [ ] Fix identified level churn
- [ ] Share metrics with publisher
- [ ] Plan data-driven improvements

---

## Key Metrics Locations

Where to find explanations for each metric:

| Metric | File | Section |
|--------|------|---------|
| D1 Retention | RETENTION_GUIDE.md | Day 1 Retention |
| D7 Retention | RETENTION_GUIDE.md | Day 7 Retention |
| D30 Retention | RETENTION_GUIDE.md | Day 30 Retention |
| Cohort Analysis | RETENTION_GUIDE.md | Cohort Analysis |
| Session Length | RETENTION_GUIDE.md | Session Metrics |
| DAU/MAU | RETENTION_GUIDE.md | Activity Metrics |
| ARPPU | RETENTION_GUIDE.md | Monetization Section |
| Level Churn | RETENTION_GUIDE.md | Churn Analysis |
| Feature Impact | RETENTION_GUIDE.md | Feature Engagement |
| LTV | RETENTION_GUIDE.md | Monetization Section |

---

## Common Scenarios

### Scenario 1: "My boss wants retention metrics ASAP"
1. Read: QUICK_REFERENCE.md (5 min)
2. Do: Add 4 logging calls to LevelManager (15 min)
3. Play: Game for 1 hour
4. Export: `DataExporter.Instance.ExportAllReports()`
5. Share: Files in Application.persistentDataPath/Analytics/
**Time: 30 minutes** ✓

### Scenario 2: "I need to understand what all this data means"
1. Read: README_ANALYTICS.md (10 min)
2. Read: RETENTION_GUIDE.md (20 min)
3. Review: SAMPLE_REPORTS.md (10 min)
**Time: 40 minutes** ✓

### Scenario 3: "I want to integrate everything properly"
1. Read: README_ANALYTICS.md (10 min)
2. Read: RETENTION_GUIDE.md (20 min)
3. Follow: INTEGRATION_GUIDE.md (30 min)
4. Test: Debug commands (15 min)
5. Verify: Check exported CSV (10 min)
**Time: 85 minutes** ✓

### Scenario 4: "I need to show data to publishers"
1. Read: SAMPLE_REPORTS.md (10 min)
2. Read: RETENTION_GUIDE.md→Healthy Indicators (10 min)
3. Export: All reports (2 min)
4. Share: Publisher dashboard + CSV files
**Time: 22 minutes** ✓

### Scenario 5: "I want Firebase cloud backup"
1. Read: ANALYTICS_SETUP.md (15 min)
2. Setup: Firebase in console (10 min)
3. Code: Uncomment Firebase lines (5 min)
4. Verify: Events in Firebase Console (5 min)
**Time: 35 minutes** ✓

---

## FAQ (By Document)

### About Retention (See RETENTION_GUIDE.md)
- What is D1 retention?
- Is my retention good?
- Why is retention dropping?
- What's normal for puzzle games?
- How do I interpret this metric?

### About Integration (See INTEGRATION_GUIDE.md)
- Where do I add code?
- What methods do I call?
- How do I test?
- What if something breaks?
- How do I debug?

### About Exports (See SAMPLE_REPORTS.md)
- What do CSV files look like?
- How do I interpret the data?
- What's a healthy ARPPU?
- How do I share with publisher?
- What can I do with this data?

### About Setup (See ANALYTICS_SETUP.md)
- How do I enable Firebase?
- How do I query BigQuery?
- What's the setup process?
- How do I test Firebase?
- What if Firebase doesn't work?

### About Features (See README_ANALYTICS.md)
- What can this system do?
- What metrics are tracked?
- Is it performant?
- Does it work offline?
- What's included?

---

## File Sizes & Reading Time

```
README_ANALYTICS.md         15K  ~10 min  ⭐ START HERE
RETENTION_GUIDE.md          15K  ~20 min  📚 LEARN METRICS
INTEGRATION_GUIDE.md        16K  ~30 min  🔧 IMPLEMENT
ANALYTICS_SETUP.md          12K  ~15 min  ☁️  FIREBASE
SAMPLE_REPORTS.md           9K   ~10 min  📊 EXAMPLES
QUICK_REFERENCE.md          8K   ~5 min   ⚡ REFERENCE
ANALYTICS_IMPLEMENTATION_   11K  ~5 min   📋 SUMMARY
COMPLETE.md
```

---

## Document Cross-References

**README_ANALYTICS.md** links to:
- RETENTION_GUIDE.md (understand metrics)
- INTEGRATION_GUIDE.md (add to game)
- QUICK_REFERENCE.md (code snippets)
- ANALYTICS_SETUP.md (Firebase)
- SAMPLE_REPORTS.md (example output)

**INTEGRATION_GUIDE.md** links to:
- QUICK_REFERENCE.md (code snippets)
- ANALYTICS_SETUP.md (Firebase)
- RETENTION_GUIDE.md (metric definitions)

**RETENTION_GUIDE.md** is standalone but referenced by:
- README_ANALYTICS.md
- INTEGRATION_GUIDE.md
- SAMPLE_REPORTS.md (metrics interpretation)

**SAMPLE_REPORTS.md** links to:
- RETENTION_GUIDE.md (metric definitions)
- ANALYTICS_SETUP.md (BigQuery)

**QUICK_REFERENCE.md** links to:
- RETENTION_GUIDE.md (metric targets)
- ANALYTICS_SETUP.md (Firebase)

---

## Checklist for Success

### Week 1: Setup & Learning
- [ ] Read README_ANALYTICS.md
- [ ] Read RETENTION_GUIDE.md (key metrics section)
- [ ] Copy scripts to project
- [ ] Initialize analytics in game
- [ ] Test basic logging

### Week 2: Full Integration
- [ ] Add all logging calls
- [ ] Test with debug commands
- [ ] Export sample data
- [ ] Review CSV format
- [ ] Fix any issues

### Week 3-4: Data Collection
- [ ] Play game regularly (accumulate data)
- [ ] Monitor exports weekly
- [ ] Check for issues
- [ ] Identify retention problems
- [ ] Plan fixes

### Week 5: Publisher Ready
- [ ] Generate clean export
- [ ] Create publisher dashboard
- [ ] Write summary analysis
- [ ] Share with stakeholders
- [ ] Get feedback

---

## Print This Page!

This navigation guide is useful to print and keep by your desk:
- Quick reference to which file answers which question
- Integration checklist to track progress
- Reading order for your available time
- Links between documents

---

## Support

**Problem:** Can't find something  
**Solution:** Use Ctrl+F to search this file for the topic

**Problem:** Don't know where to start  
**Solution:** Read "Start Here" section above

**Problem:** Need code to copy/paste  
**Solution:** Go to QUICK_REFERENCE.md

**Problem:** Don't understand a metric  
**Solution:** Go to RETENTION_GUIDE.md

**Problem:** Need to integrate  
**Solution:** Go to INTEGRATION_GUIDE.md

**Problem:** Want examples  
**Solution:** Go to SAMPLE_REPORTS.md

**Problem:** Need Firebase setup  
**Solution:** Go to ANALYTICS_SETUP.md

---

## Summary

You have:
- ✅ 4 production scripts
- ✅ 6 comprehensive docs
- ✅ 66 KB of documentation
- ✅ Multiple example outputs
- ✅ Integration guides
- ✅ Troubleshooting help
- ✅ Navigation (this file!)

**Next step:** Pick your reading time above and start with the first file.

**Estimated time to working analytics: 1-2 hours**

Let's go! 🚀
