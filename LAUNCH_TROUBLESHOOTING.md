# Launch Troubleshooting Guide - Common Issues & Solutions

**Target**: Quick reference for solving common problems during launch and post-launch period.  
**Time**: 5-10 minutes per issue  
**Format**: Issue → Diagnosis → Solution  

---

## ANDROID BUILD ERRORS

### "JDK Not Found"

**Error Message**:
```
Error: JDK not found
Could not locate the JDK directory
```

**Diagnosis**:
- Java Development Kit path not set in Unity
- JDK not installed

**Solution**:
1. **Download JDK 11**: https://www.oracle.com/java/technologies/downloads/
2. **Install JDK**
3. **In Unity**: Edit → Preferences → External Tools → Android
4. **Click Browse** next to "JDK"
5. **Navigate to**: `C:\Program Files\Java\jdk-11.x.x` (Windows)
   - Or: `/Library/Java/JavaVirtualMachines/jdk-11.x.x/Contents/Home` (Mac)
6. **Click Apply**
7. **Restart Unity** and rebuild

---

### "Android SDK Not Found"

**Error Message**:
```
Error: Android SDK not found
Unable to locate Android SDK
```

**Diagnosis**:
- Android SDK not installed
- Path not set correctly

**Solution**:
1. **Try auto-install** (easiest):
   - Edit → Preferences → External Tools → Android
   - Check boxes next to: Android SDK, Android NDK, OpenJDK
   - Click "Install" and wait
2. **If that fails, manual install**:
   - Download Android Studio: https://developer.android.com/studio
   - Install it (includes SDK)
   - Point Unity to SDK location:
     - Edit → Preferences → External Tools → Android
     - Set Android SDK Path: `C:\Users\YourName\AppData\Local\Android\Sdk`
3. **Restart Unity** and rebuild

---

### "Build Failed - Gradle Error"

**Error Message**:
```
Gradle build failed
Unable to build project
```

**Diagnosis**:
- Build system corruption
- Gradle cache outdated
- Unity version incompatibility

**Solution**:
1. **Clean Gradle cache**:
   - Delete folder: `ProjectName/Temp/Gradle`
   - Or: Right-click project → Clean
2. **Update Build Tools**:
   - Edit → Preferences → External Tools → Android
   - Update to latest Android SDK Build-Tools
3. **Restart Unity**
4. **Rebuild APK**

---

### "Out of Memory During Build"

**Error Message**:
```
java.lang.OutOfMemoryError
Java heap space exceeded
```

**Diagnosis**:
- Computer running out of RAM
- Too many programs open

**Solution**:
1. **Close other programs**:
   - Close browsers, Discord, Slack, etc.
   - Restart computer if RAM still low
2. **Increase Java heap size** (advanced):
   - Edit → Preferences → External Tools → Android
   - Find "Java Heap Size" option
   - Increase to 4GB or 6GB (if you have RAM available)
3. **Try build again**

---

## ADMOB & ADS ISSUES

### "Ads Don't Load - Black Screen"

**Error Message** (in Logcat):
```
E/Ads: Failed to initialize Google Mobile Ads SDK
E/AdRequest: Could not load ad unit
```

**Diagnosis**:
- Ad unit ID is wrong or blank
- App ID not set in Unity
- No internet connection
- Device registered as test device (shows test ads instead)

**Solution**:
1. **Verify Ad Unit IDs in script**:
   - Open AdManager.cs
   - Check: `_bannerAdUnitId = "ca-app-pub-xxxxx..."`
   - Must match AdMob dashboard exactly
2. **Set App ID in Unity**:
   - Window → Google Mobile Ads → Settings
   - Enter your AdMob App ID
3. **Check internet**:
   - Device must have Wi-Fi or mobile data
   - Emulator: Can't show real ads, use test IDs
4. **If testing, use test ad unit IDs** (Step 3 of ADMOB_SETUP_GUIDE.md)

---

### "Ad Unit ID Invalid"

**Error Message** (in Logcat):
```
E/Ads: Invalid ad unit ID: ca-app-pub-3940256099942544/1234567890
```

**Diagnosis**:
- Ad unit ID format is wrong
- Copied wrong ID from AdMob
- Typo in ID

**Solution**:
1. **Go to AdMob dashboard**: https://admob.google.com
2. **Find correct Ad Unit ID**:
   - Apps → Your App → Ad Units
   - Copy ID exactly (long string like `ca-app-pub-xxxxxxxxxxxxxxxx/yyyyyyyyyy`)
3. **Paste in AdManager.cs**:
   - Must match exactly
   - No extra spaces or characters
4. **Rebuild APK** and test

---

### "Ads Show During Testing But Not in Production"

**Diagnosis**:
- Using test ad unit IDs in production
- Device registered as test device

**Solution**:
1. **Switch to real ad unit IDs**:
   - Remove test IDs from AdManager.cs
   - Add real ad unit IDs from AdMob
   - Rebuild APK
2. **Remove from test devices**:
   - AdMob Dashboard → Settings → Test Devices
   - Remove your device ID from list
3. **Rebuild and upload to Google Play**

---

### "No Revenue - Ads Aren't Earning"

**Diagnosis**:
- Ads aren't being shown enough
- Ad placement is poor (users don't see them)
- Low-value region (Canada has lower CPM)

**Solution**:
1. **Check ad impressions**:
   - AdMob Dashboard → Home → Impressions
   - If <1000 impressions/day, not enough views
2. **Increase ad frequency**:
   - Show banner ad always (not just sometimes)
   - Add more rewarded ad opportunities
   - Show interstitial every 3 levels (not every level)
3. **Improve ad placement**:
   - Banner at bottom (where user sees it)
   - Rewarded ad: "Get bonus coins" (incentivized)
   - Interstitial: Between natural pauses
4. **Wait for more users**:
   - Revenue grows as user base grows
   - First day, revenue might be $0.10-$0.50
   - By day 7, should see $1-5/day

---

## GOOGLE PLAY ISSUES

### "App Rejected for Policy Violation"

**Email from Google**:
```
Your app doesn't comply with Google Play policies
See details: [link]
```

**Common Violations**:

**1. Deceptive/Misleading**
- Description doesn't match game
- Screenshots show different game
- Title is misleading

**Solution**:
- Rewrite description to match actual game
- Update screenshots to show your game
- Resubmit with accurate info

**2. Too Many Ads / Intrusive Ads**
- Ads interrupt gameplay constantly
- Ads block important UI
- Ads appear during loading

**Solution**:
- Reduce ad frequency
- Move ads to bottom/sides (not center)
- Show ads only between natural pauses
- Rebuild APK and resubmit

**3. Privacy Policy Missing/Inadequate**
- No privacy policy
- Privacy policy doesn't mention ads/data collection

**Solution**:
- Create simple privacy policy
- Mention: Google AdMob, analytics, no personal data
- Upload to Google Play
- Resubmit app

**4. Crashes / Technical Issues**
- App crashes on test device
- Won't run on emulator
- Too many reported crashes

**Solution**:
- Test app thoroughly on emulator
- Fix any crashing issues
- Review LogCat for errors
- Rebuild APK, test on multiple devices
- Resubmit

---

### "App Takes Too Long to Approve"

**Issue**: Review pending for >3 hours

**Normal timing**: 30 min - 2 hours
**Sometimes**: 3-5 hours
**Rare**: 12-24 hours

**What to do**:
- Wait, it will eventually process
- No need to resubmit multiple times
- Check status in Play Console → Releases

---

### "App Removed - High Crash Rate"

**Email from Google**:
```
Your app was removed: Crash rate exceeds policy limits
Current crash rate: 5%
```

**Diagnosis**:
- Game crashing for many users
- Critical bug needs immediate fix

**Solution** (Emergency):
1. **Identify crash**:
   - Google Play Console → Vitals → Crashes
   - See which line of code is crashing
2. **Fix the bug**:
   - Most common: Null reference error, missing sprite, memory leak
   - Make fix to code
   - Test thoroughly on device
3. **Rebuild APK** with fix
4. **Upload Version 1.1**:
   - Increment version: 1.0 → 1.1
   - Build Settings → Player → Android → Version Name
5. **Submit for review** (emergency review usually approved within 1 hour)

---

## DEVICE TESTING ISSUES

### "App Crashes on Real Device but Not Emulator"

**Diagnosis**:
- Device-specific bug
- Memory issue on low-end device
- Permission missing

**Solution**:
1. **Check LogCat**:
   - Connect device via USB
   - adb logcat | grep "E/"
   - Look for error messages
2. **Common fixes**:
   - Low memory: reduce number of sprites in memory
   - Permission: check AndroidManifest.xml
   - Device: test on different Android version
3. **Test on multiple devices**:
   - High-end and low-end
   - Different Android versions (6.0, 10, 12, etc.)
   - Different screen sizes

---

### "Game Runs at 30 FPS (Slow)"

**Diagnosis**:
- Too much processing
- Ads loading every frame
- Memory leak

**Solution**:
1. **Profile performance** (Unity Profiler):
   - Window → Analysis → Profiler
   - Watch CPU/Memory while playing
   - Identify bottleneck
2. **Common fixes**:
   - Don't create new objects every frame
   - Cache sprites (don't reload)
   - Load ads once, not every frame
   - Reduce particles/effects
3. **Test on low-end device**:
   - If slow on expensive device, problem is with code
   - If slow on cheap device, acceptable (optimize if possible)

---

### "Buttons Too Small to Tap"

**Diagnosis**:
- UI scale doesn't match screen size
- Text too small to read

**Solution**:
1. **Use relative layout** (Canvas scaling):
   - Canvas → Render Mode: Screen Space - Overlay
   - Canvas Scaler → Scale with Screen Size
2. **Minimum button size**: 48x48 DP (density-independent pixels)
3. **Test on different screen sizes**:
   - Small phone (5")
   - Large phone (6.5")
   - Tablet (10")

---

## SPRITES & GRAPHICS ISSUES

### "Sprites Missing or Pink/Magenta"

**Error**: Game shows pink/magenta instead of sprite

**Diagnosis**:
- Sprite not imported correctly
- Missing shader/material
- Wrong texture format

**Solution**:
1. **Check import settings**:
   - Select sprite in Project
   - Inspector → Texture Type: "Sprite (2D and UI)"
   - Click Apply
2. **Check shader**:
   - Select object in scene
   - Inspector → Sprite Renderer → Material: "Sprites/Default"
3. **Verify file exists**:
   - Assets/Sprites/Tiles/tile_1.png (should exist)
   - If missing, re-download from Kenney

---

### "Sprites Look Blurry"

**Diagnosis**:
- Wrong filter mode
- Pixels per unit mismatch

**Solution**:
1. **Set filter mode**:
   - Select sprite
   - Inspector → Filter Mode: "Point (no filter)"
   - Click Apply
2. **Set pixels per unit**:
   - Inspector → Pixels Per Unit: 100 (for 64x64 sprites)
   - Click Apply

---

## POST-LAUNCH ISSUES

### "Day 1 Retention is 10% (Very Low)"

**Diagnosis**:
- Game too hard (players can't progress)
- Game too easy (boring)
- Game has bugs
- Game doesn't match description

**Solution**:
1. **Analyze where players quit**:
   - Use Firebase Analytics
   - Or check Google Play Analytics → Retention → Time played
   - See if players quit at specific level
2. **Fix the issue**:
   - Too hard: Make first 10 levels easier
   - Too easy: Add challenge earlier
   - Buggy: Fix crashes
   - Misleading: Update description
3. **Upload Version 1.1**:
   - Rebuild with fixes
   - Note in release: "Improved difficulty balance"
   - Resubmit for review

---

### "Negative Reviews - Game Buggy"

**Example Review**:
```
"Game keeps crashing on level 5"
⭐⭐ (2 stars)
```

**Solution**:
1. **Reproduce the bug**:
   - Try to play to level 5
   - See if you can crash it
2. **Fix the bug**:
   - Debug the issue in code
   - Test thoroughly
3. **Upload fix**:
   - Version 1.1
   - Note: "Fixed crash on level 5"
   - Submit for review
4. **Respond to review**:
   - Click reply button on review
   - "Thanks for reporting! We fixed this in version 1.1. Please update and try again."

---

### "Users Saying Game Too Hard"

**Example Review**:
```
"Can't beat level 10. Too difficult!"
⭐⭐ (2 stars)
```

**Solution**:
1. **Play the level yourself**:
   - Is it genuinely difficult?
   - Or is it strategy-based (requires thinking)?
2. **If too hard**:
   - Reduce required tiles to match
   - Add power-ups at earlier levels
   - Plan version 1.1 with easier difficulty
3. **If fair difficulty**:
   - Respond: "Thanks! Level 10 is meant to be challenging. Try using the power-up strategically!"
   - Add tutorial/hints for that level

---

## MONETIZATION ISSUES

### "AdMob Account Suspended"

**Email from Google**:
```
Your AdMob account has been suspended
Reason: Invalid activity or policy violation
```

**Causes** (rare):
- You clicked your own ads
- Users clicking ads excessively
- Artificially inflating impressions
- Suspicious activity

**Prevention**:
- NEVER click your own ads
- Don't incentivize users to click ads
- Don't use bot/automation
- Don't share app on "ad clicking" websites

**If happens**:
- Email Google: appeals@google.com
- Explain situation
- Wait 5-10 business days for response
- Usually resolved if honest mistake

---

## SUPPORT & COMMUNICATION

### "How to Respond to Negative Reviews"

**Bad Response**:
```
"Your review is stupid. Our game is great."
```

**Good Response**:
```
"Thanks for your feedback! We're sorry you experienced issues.
Could you tell us more about the problem so we can fix it?
Please email us at: support@yourgame.com"
```

**Tips**:
- Be polite and professional
- Don't get defensive
- Acknowledge the problem
- Offer solution
- Ask for more details

---

### "How to Respond to Crash Reports"

**When someone reports: "Game crashes on level 5"**

**Your Response**:
```
"Thanks for reporting! We're investigating.
Please try the following:
1. Update to the latest version (if available)
2. Clear app cache: Settings → Apps → [Your Game] → Storage → Clear Cache
3. If still crashing, please email us: support@yourgame.com"
```

---

## QUICK DECISION TREE

```
⚠️ PROBLEM ENCOUNTERED?

Is it a CRASH?
├─ YES → Check "Android Build Errors" section above
└─ NO → Next

Are APPS NOT LOADING?
├─ YES → Check "AdMob & Ads Issues" section above
└─ NO → Next

Is it a STORE LISTING issue?
├─ YES → Check "Google Play Issues" section above
└─ NO → Next

Is it a SPRITE/GRAPHICS issue?
├─ YES → Check "Sprites & Graphics Issues" section above
└─ NO → Next

Is it POST-LAUNCH (low retention/ratings)?
├─ YES → Check "Post-Launch Issues" section above
└─ NO → Check "Device Testing Issues" section
```

---

## WHEN TO ESCALATE

**Contact Support if**:
- Problem not in this guide
- ApK won't build despite trying all solutions
- Google rejected app for unknown reason
- AdMob account suspended
- Critical crash you can't identify

**Support Contacts**:
- Unity Issues: unity.com/support
- Google Play: google.com/play/console/support
- AdMob Issues: admob.google.com/support
- Android Issues: stackoverflow.com (tag: android)

---

**✅ Troubleshooting guide complete! Most issues have quick solutions above.**

For each problem, follow the diagnosis and solution steps. If stuck, reach out to support contacts.

