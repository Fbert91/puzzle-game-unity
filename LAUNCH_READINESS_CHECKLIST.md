# Launch Readiness Checklist - Final Quality Assurance

**Target**: Comprehensive pre-launch verification to ensure game is ready for Canada soft launch. Complete this 1 day before going live.  
**Time**: 2-3 hours  
**Success**: All items checked, game ready to launch  

---

## PRE-LAUNCH QUALITY ASSURANCE

### GAMEPLAY & MECHANICS

**Core Gameplay**
- [ ] Game is playable from start to finish
- [ ] All game mechanics work (tile matching, scoring, etc.)
- [ ] Difficulty progression is clear and reasonable
- [ ] No infinite loops or soft locks
- [ ] Game responds to all user inputs
- [ ] Controls are intuitive and responsive
- [ ] Game handles edge cases (quick-quit, low memory, etc.)

**Progression & Levels**
- [ ] First 5 levels are easy (tutorial-like)
- [ ] Difficulty increases gradually
- [ ] Level 1-10 are engaging
- [ ] Level 10-20 introduce new challenges
- [ ] Mix of easy, medium, hard levels
- [ ] No impossible-to-win levels
- [ ] Win/lose conditions are clear

**User Experience**
- [ ] Main menu is clear and inviting
- [ ] Settings screen works (if implemented)
- [ ] Pause/resume works during gameplay
- [ ] Game over screen is clear
- [ ] Level complete celebration is satisfying
- [ ] Sound/music works (if implemented)
- [ ] Game feels responsive (no lag/stuttering)

---

### TECHNICAL REQUIREMENTS

**Build & Installation**
- [ ] APK builds without errors
- [ ] APK file size is <100MB (if possible)
- [ ] APK installs on Android 6.0+ device
- [ ] App launches without crashing
- [ ] App doesn't require unknown permissions
- [ ] No error messages on startup
- [ ] App icon displays correctly

**Performance**
- [ ] Game runs smoothly (60 FPS or stable)
- [ ] No significant frame drops during gameplay
- [ ] No lag when loading levels
- [ ] No lag when showing ads
- [ ] Memory usage is reasonable
- [ ] Battery drain is acceptable (not excessive)
- [ ] Tested on at least 2 different devices

**Compatibility**
- [ ] Works on Android 6.0 (API 23)
- [ ] Works on Android 10+
- [ ] Works on both phones and tablets
- [ ] Landscape and portrait modes work
- [ ] Different screen sizes supported

**Crashing & Stability**
- [ ] No crashes during 30-minute gameplay session
- [ ] No crashes when pressing back button
- [ ] No crashes when pausing/resuming
- [ ] No crashes when switching apps and back
- [ ] No crashes on low memory device
- [ ] Game handles network loss gracefully
- [ ] App never shows "App has stopped" error

---

### MONETIZATION & ADS

**AdMob Setup**
- [ ] AdMob account fully created
- [ ] App ID set in Google Mobile Ads → Settings
- [ ] All 3 ad unit IDs created (banner, rewarded, interstitial)
- [ ] Ad unit IDs saved safely
- [ ] Using REAL ad unit IDs (not test IDs) for this version

**Banner Ads**
- [ ] Banner appears at bottom of screen during gameplay
- [ ] Banner doesn't cover important UI
- [ ] Banner loads without errors (check Logcat)
- [ ] Banner is not intrusive or annoying
- [ ] Banner disappears during menus (optional)

**Rewarded Ads**
- [ ] Rewarded ad button/trigger works
- [ ] Rewarded ad plays full video without crashing
- [ ] Player receives reward after watching
- [ ] Can refuse/close ad without crashing
- [ ] No reward given if ad closed early

**Interstitial Ads**
- [ ] Interstitial shows after level complete (if implemented)
- [ ] Interstitial doesn't interrupt gameplay
- [ ] Player can close interstitial
- [ ] Game continues smoothly after ad closes

**No Test Ads**
- [ ] NOT using Google test ad unit IDs
- [ ] Using real ad unit IDs (from AdMob dashboard)
- [ ] Device is NOT registered as test device (unless intentional)

---

### GOOGLE PLAY LISTING

**Store Listing Information**
- [ ] App name is clear and keyword-friendly
- [ ] Short description written (80 chars, catchy)
- [ ] Full description written (engaging, 4000 chars)
- [ ] Category set to "Games"
- [ ] Content rating completed
- [ ] Privacy policy uploaded/set

**Visual Assets**
- [ ] App icon uploaded (512x512 PNG, no transparency)
- [ ] App icon looks good and is recognizable
- [ ] At least 2 screenshots uploaded
- [ ] Maximum 8 screenshots uploaded
- [ ] Screenshots show actual gameplay
- [ ] Screenshots are in 1080x1920 or 1920x1080
- [ ] Feature graphic uploaded (optional but recommended)

**Metadata**
- [ ] Package name matches Android build settings
- [ ] Package name is unique (not taken)
- [ ] Version number set (1.0 recommended for first release)
- [ ] Target countries set (Canada selected)
- [ ] Pricing set to "Free"

---

### COMPLIANCE & POLICIES

**Privacy & Legal**
- [ ] Privacy policy exists (URL or Google's default)
- [ ] Privacy policy mentions AdMob/ads
- [ ] Privacy policy is accessible (URL works)
- [ ] Game doesn't collect personal data without consent
- [ ] No undisclosed data collection

**Permissions**
- [ ] Only requesting necessary permissions
- [ ] All requested permissions justified
- [ ] No excessive permissions (GPS, camera, etc. if not needed)
- [ ] Permissions match app functionality

**Content Appropriateness**
- [ ] Game content matches description
- [ ] No inappropriate language in game
- [ ] No violent/sexual content
- [ ] Screenshots match game content
- [ ] Content rating is accurate

**Google Play Compliance**
- [ ] App doesn't contain malware/viruses
- [ ] App doesn't violate Google policies
- [ ] Game doesn't impersonate other apps
- [ ] Game doesn't contain deceptive practices
- [ ] Game doesn't have backdoors or hidden features

---

### ANALYTICS & SUPPORT

**Monitoring Readiness**
- [ ] Google Play Console account accessed and ready
- [ ] Able to view installs, retention, crashes
- [ ] AdMob dashboard accessed and ready
- [ ] Can check daily revenue
- [ ] Firebase Analytics setup (optional)
- [ ] Can monitor via phone/computer during launch

**Support Readiness**
- [ ] Support email set (shown in app or store listing)
- [ ] Plan for responding to user reviews
- [ ] Plan for handling crash reports
- [ ] Contact info available in app (optional)
- [ ] Ready to release updates if needed

---

### DEVICE TESTING

**Physical Device Tests (Do on Real Device)**
- [ ] Game launches successfully
- [ ] UI is readable on phone screen
- [ ] Buttons are easy to tap (not too small)
- [ ] Game is fun and engaging
- [ ] No lag or stuttering
- [ ] Ads load properly
- [ ] Battery drain is reasonable
- [ ] No crashes during 30-minute session
- [ ] Tested on at least 2 different phones (if possible)

**Common Device Sizes to Test**
- [ ] Small phone (5 inches)
- [ ] Medium phone (6 inches)
- [ ] Large phone (6.5+ inches)
- [ ] Tablet (optional but recommended)

---

### FINAL CHECKS (Day Before Launch)

**24 Hours Before Launch**

- [ ] **Code Review**: Final code looks clean
- [ ] **No Debug Text**: Remove console.logs, debug UI
- [ ] **No Placeholders**: All graphics finalized
- [ ] **Test One More Time**: 30-minute gameplay session
- [ ] **Check Ads**: Verify ads load correctly
- [ ] **Verify Version**: Version 1.0 is correct
- [ ] **APK Built**: Latest APK ready to upload
- [ ] **APK Tested**: One final test on device
- [ ] **Store Listing**: All information filled and correct
- [ ] **Screenshots Current**: Show current game version
- [ ] **Icon Final**: App icon approved
- [ ] **No Pending Changes**: Everything committed

**Morning of Launch**

- [ ] Monitoring tools open (Google Play Console, AdMob)
- [ ] Support email monitored
- [ ] Discord/feedback channels ready
- [ ] Had good sleep (you'll be busy!)
- [ ] Set aside 2-3 hours for launch day monitoring

---

### SIGN-OFF CHECKLIST

**Before clicking "Submit":**

Please confirm:

**I have:**
- ✅ Tested game on real Android device (no crashes)
- ✅ Verified all ads load correctly
- ✅ Filled entire Google Play listing (no blanks)
- ✅ Uploaded at least 2 screenshots
- ✅ Set up AdMob with real ad unit IDs
- ✅ Created and saved app icon
- ✅ Set Canada as target country
- ✅ Set game as "Free"
- ✅ Completed content rating questionnaire
- ✅ Set up privacy policy
- ✅ Verified package name is unique
- ✅ Set version to 1.0
- ✅ Ready to monitor launch for 1 week
- ✅ Have plan for responding to user feedback
- ✅ Know how to fix bugs and upload updates

**Signature:**
```
Bert's Puzzle Game - Ready for Canada Launch
Version: 1.0
Date: ___________
```

---

## LAUNCH DAY TIMELINE

### **T-0 (Morning of Launch)**

```
08:00 - Open monitoring dashboards
08:15 - Verify store listing one more time
08:30 - Check APK upload (should be approved)
09:00 - Submit app to production (if not yet approved)
09:15 - Check app appears in Play Store (may take 20-30 min)
10:00 - Share link with friends/testers
10:30 - Monitor first installs coming in
```

### **T+1 Hour**

```
11:00 - Check for crashes (should be zero)
11:15 - Monitor ad impressions
11:30 - Read first user reviews
12:00 - If all good, share publicly
```

### **T+2-4 Hours**

```
12:00 - Game should have 20-50 installs
13:00 - Check retention numbers starting
14:00 - Monitor for any crashes
15:00 - First day report
```

### **T+24 Hours**

```
Next day 08:00 - Check Day 1 retention
08:30 - Read all reviews
09:00 - Check crash reports
10:00 - Decision: optimization needed or good?
```

---

## TROUBLESHOOTING DURING LAUNCH

### **"App not appearing in Play Store"**
- Wait 20-30 minutes after approval
- Refresh Play Store cache
- Search by exact name
- May take up to 1 hour to index

### **"App crashes on startup"**
- Emergency: remove from production immediately
- Identify crash from Logcat
- Fix bug, rebuild APK
- Upload version 1.1
- Resubmit for review (usually approved <1 hour)

### **"No ads are showing"**
- Check LogCat for ad errors
- Verify real ad unit IDs used (not test IDs)
- Device not registered as test device
- Check internet connection
- May take 30 seconds to load first ad

### **"Reviews are 1 star - game too hard"**
- Read review to confirm issue
- Adjust difficulty in next update
- Respond to reviews acknowledging feedback
- Plan version 1.1 for fix
- Don't panic, first day reviews can be random

---

## POST-LAUNCH DAY CHECKLIST

**After launching, during first week:**

**Daily (Every 24 hours)**
- [ ] Check new reviews
- [ ] Check crash reports
- [ ] Check installs growing
- [ ] Check ad revenue
- [ ] Respond to 1-2 star reviews

**After Day 3**
- [ ] Analyze retention (D1, D3 retention numbers)
- [ ] Check if any pattern in crashes
- [ ] Read all feedback for improvement ideas
- [ ] Plan any urgent fixes

**After Day 7**
- [ ] Full soft launch analysis (see SOFT_LAUNCH_STRATEGY.md)
- [ ] Decide: expand to US/UK or optimize more?
- [ ] Plan version 1.1 improvements
- [ ] Analyze monetization

---

## SUCCESS CRITERIA FOR LAUNCH

**Your launch was successful if:**

| Item | Success | Warning | Problem |
|------|---------|---------|---------|
| **Launches** | ✅ No crash | ⚠️ Rare crash | 🔴 Crashes always |
| **Day 1 Retention** | ✅ 25%+ | ⚠️ 15-25% | 🔴 <15% |
| **Rating** | ✅ 4.0+ | ⚠️ 3.5-4.0 | 🔴 <3.5 |
| **Crashes** | ✅ 0% | ⚠️ 1-2% | 🔴 >2% |
| **Ads Load** | ✅ 100% | ⚠️ 80-100% | 🔴 <80% |
| **Installs/Day** | ✅ 50+ | ⚠️ 10-50 | 🔴 <10 |

---

## FINAL DECISION POINTS

**After checking this list, decide:**

- ✅ **READY TO LAUNCH**: All items checked ✓
  → Go ahead and submit app!

- ⚠️ **ALMOST READY**: 1-2 items unchecked
  → Fix those items, then launch

- 🔴 **NOT READY**: 5+ items unchecked
  → Spend more time fixing
  → Don't rush, quality > speed

---

**🚀 Once complete, submit your app to Google Play!**

**Estimated path forward:**
1. ✅ Check this list (30 min)
2. ✅ Fix any issues (30 min - 2 hours)
3. ✅ Final device test (30 min)
4. ✅ Upload to Google Play (5 min)
5. ✅ Wait for approval (1-3 hours)
6. ✅ Submit for production release (5 min)
7. ✅ App goes LIVE (20-30 min)
8. ✅ Monitor launch (ongoing)

**Total time: 2-4 hours to live!** 🎉

