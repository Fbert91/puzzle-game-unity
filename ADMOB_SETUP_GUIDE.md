# AdMob Setup Guide - Complete 30-Minute Setup

**Target**: Create your Google AdMob account, set up ad units (banner, rewarded, interstitial), and integrate Google Mobile Ads SDK into your puzzle game.  
**Time**: ~30 minutes  
**Prerequisites**: Google account, Unity project with Android build settings  
**Result**: Ready to test ads with test unit IDs before going live  

---

## TABLE OF CONTENTS
1. [Google AdMob Account Setup](#step-1-google-admob-account-setup)
2. [Create Ad Units](#step-2-create-ad-units)
3. [Import Google Mobile Ads SDK](#step-3-import-google-mobile-ads-sdk-into-unity)
4. [Code Integration](#step-4-code-integration)
5. [Testing with Test Ad Unit IDs](#step-5-testing-with-test-ad-unit-ids)
6. [Going Live Checklist](#step-6-going-live-checklist)
7. [Troubleshooting](#troubleshooting)

---

## STEP 1: Google AdMob Account Setup

### 1.1 Create an AdMob Account

1. **Go to Google AdMob**: https://admob.google.com
2. **Sign in** with your Google account (create one if needed)
3. **Click "Sign Up"** if you don't have an AdMob account yet

### 1.2 Accept Terms & Enter Phone Number

1. Check **"I agree to the AdMob terms..."**
2. Enter your **phone number** for verification
3. Click **"Create"**

### 1.3 Account Information

**Fill in your account details:**
- **Account Type**: Select "Individual" (unless you represent a company)
- **Email Address**: Your Google email
- **Website** (optional): Leave blank for now
- Click **"Finish setup"**

✅ **Result**: You now have an active AdMob account.

---

## STEP 2: Create Ad Units

### 2.1 Create Your First App in AdMob

1. **In AdMob Dashboard**: Click **"Apps"** (left sidebar)
2. Click **"Add app"** button
3. **App Name**: `PuzzleGame Canada` (or your game name)
4. **App Type**: Select **"Android"**
5. **Click "Add"**

### 2.2 Register Your App

After clicking "Add":
- **Package Name**: `com.yourname.puzzlegame` (or your unique package name)
  - Format: `com.firstname.lastname.appname`
  - Example: `com.bert.puzzle.game`
  - ⚠️ **Important**: You'll use this in your Unity Android build settings
- **App Store**: Select **"Google Play Store"** (even though you haven't published yet)
- **Click "Create"**

### 2.3 Create Ad Units (3 types)

Now you'll create three ad units for banner, rewarded, and interstitial ads:

#### **AD UNIT #1: BANNER AD**

1. Click **"Create Ad Unit"** button
2. **Ad Unit Name**: `Banner_Main` (or `Banner_Bottom`)
3. **Ad Format**: Select **"Banner"**
   - Size: 320x50 (standard banner)
4. Click **"Create Ad Unit"**
5. **Copy and save the Ad Unit ID** (looks like: `ca-app-pub-xxxxxxxxxxxxxxxx/yyyyyyyyyy`)
   - Store in notepad for reference

✅ **Banner Ad Unit ID saved**

#### **AD UNIT #2: REWARDED AD**

1. Click **"Create Ad Unit"** button again
2. **Ad Unit Name**: `Rewarded_Main`
3. **Ad Format**: Select **"Rewarded"**
4. Click **"Create Ad Unit"**
5. **Copy and save the Ad Unit ID**

✅ **Rewarded Ad Unit ID saved**

#### **AD UNIT #3: INTERSTITIAL AD**

1. Click **"Create Ad Unit"** button
2. **Ad Unit Name**: `Interstitial_Levels`
3. **Ad Format**: Select **"Interstitial"**
4. Click **"Create Ad Unit"**
5. **Copy and save the Ad Unit ID**

✅ **Interstitial Ad Unit ID saved**

### 2.4 Reference Your IDs

**Save these IDs in a safe location (notepad, GitHub, etc.):**

```
APP ID: ______________________
(Found in App Settings)

BANNER AD UNIT ID: ______________________
REWARDED AD UNIT ID: ______________________
INTERSTITIAL AD UNIT ID: ______________________

TEST DEVICE ID: ______________________
(You'll add this in testing section)
```

---

## STEP 3: Import Google Mobile Ads SDK into Unity

### 3.1 Download Google Mobile Ads SDK

1. **In Unity**, go to **Window → TextMesh Pro → Import TMP Essential Resources** (if prompted)
2. Go to **Window → Google Mobile Ads → Settings**
3. If you see this option, you already have the plugin installed
4. If NOT, download it manually:
   - Download: https://github.com/googleads/googleads-mobile-unity-plugin/releases
   - Latest version (e.g., `GoogleMobileAdsPlugin-v9.1.0.unitypackage`)

### 3.2 Import the Plugin

1. **In Unity**: Go to **Assets → Import Package → Custom Package**
2. **Select** the `GoogleMobileAdsPlugin-v*.*.*.unitypackage` file
3. Click **"Import"**
4. Wait for import to complete (may take a minute)

### 3.3 Configure Ad Settings

1. Go to **Window → Google Mobile Ads → Settings**
2. **iOS Settings**:
   - **Ad Mob App ID**: (Skip for now, you can add iOS later)
3. **Android Settings**:
   - **Ad Mob App ID**: Enter your **APP ID** from Step 2.4
   - ✅ Click **"Update"**

### 3.4 Verify Installation

After import, you should see a new folder: **Assets → GoogleMobileAds**

If you don't see it, try:
1. **Window → Google Mobile Ads → Settings** (opens settings again)
2. This confirms the plugin is installed

✅ **Google Mobile Ads SDK is now ready to use**

---

## STEP 4: Code Integration

### 4.1 Create AdManager Script

Create a new C# script: **Assets → Scripts → AdManager.cs**

```csharp
using GoogleMobileAds.Client;
using GoogleMobileAds.Common;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    // Ad Unit IDs - Replace with YOUR ids from AdMob
    private string _bannerAdUnitId = "ca-app-pub-xxxxxxxxxxxxxxxx/yyyyyyyyyy";
    private string _rewardedAdUnitId = "ca-app-pub-xxxxxxxxxxxxxxxx/yyyyyyyyyy";
    private string _interstitialAdUnitId = "ca-app-pub-xxxxxxxxxxxxxxxx/yyyyyyyyyy";

    // Test Device IDs (for testing)
    private string[] _testDeviceIds = new string[] { "EMULATOR" };

    private BannerView _bannerView;
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;

    public static AdManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize Google Mobile Ads SDK
        MobileAds.Initialize();
        
        // Load ads
        LoadBannerAd();
        LoadRewardedAd();
        LoadInterstitialAd();
    }

    // ===== BANNER ADS =====
    public void LoadBannerAd()
    {
        if (_bannerView != null)
            _bannerView.Destroy();

        var adRequest = new AdRequest();
        _bannerView = new BannerView(_bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        
        _bannerView.OnAdLoaded += HandleBannerLoaded;
        _bannerView.OnAdFailedToLoad += HandleBannerFailedToLoad;
        
        _bannerView.LoadAd(adRequest);
    }

    private void HandleBannerLoaded()
    {
        Debug.Log("Banner ad loaded successfully");
    }

    private void HandleBannerFailedToLoad(LoadAdError error)
    {
        Debug.LogError("Banner ad failed to load: " + error.GetMessage());
    }

    public void HideBannerAd()
    {
        if (_bannerView != null)
            _bannerView.Hide();
    }

    public void ShowBannerAd()
    {
        if (_bannerView != null)
            _bannerView.Show();
    }

    public void DestroyBannerAd()
    {
        if (_bannerView != null)
            _bannerView.Destroy();
    }

    // ===== REWARDED ADS =====
    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        var adRequest = new AdRequest();
        RewardedAd.Load(_rewardedAdUnitId, adRequest, HandleRewardedAdLoaded);
    }

    private void HandleRewardedAdLoaded(RewardedAd ad, LoadAdError error)
    {
        if (error != null || ad == null)
        {
            Debug.LogError("Rewarded ad failed to load: " + error?.GetMessage());
            return;
        }

        _rewardedAd = ad;
        _rewardedAd.OnAdPaid += HandleRewardedAdPaid;
        _rewardedAd.OnAdClicked += HandleRewardedAdClicked;
        _rewardedAd.OnAdFullScreenContentOpened += HandleRewardedAdOpened;
        _rewardedAd.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
        _rewardedAd.OnAdFullScreenContentFailed += HandleRewardedAdFailedToShow;
        
        Debug.Log("Rewarded ad loaded successfully");
    }

    public void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("User earned reward: " + reward.Amount + " " + reward.Type);
                // Give player reward here (coins, hints, etc.)
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready to show");
            LoadRewardedAd(); // Try to load a new one
        }
    }

    private void HandleRewardedAdPaid(AdValue value)
    {
        Debug.Log("Rewarded ad paid: " + value.Value);
    }

    private void HandleRewardedAdClicked()
    {
        Debug.Log("Rewarded ad clicked");
    }

    private void HandleRewardedAdOpened()
    {
        Debug.Log("Rewarded ad opened");
    }

    private void HandleRewardedAdClosed()
    {
        Debug.Log("Rewarded ad closed");
        LoadRewardedAd(); // Load next ad
    }

    private void HandleRewardedAdFailedToShow(AdError error)
    {
        Debug.LogError("Rewarded ad failed to show: " + error.GetMessage());
        LoadRewardedAd();
    }

    // ===== INTERSTITIAL ADS =====
    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(_interstitialAdUnitId, adRequest, HandleInterstitialAdLoaded);
    }

    private void HandleInterstitialAdLoaded(InterstitialAd ad, LoadAdError error)
    {
        if (error != null || ad == null)
        {
            Debug.LogError("Interstitial ad failed to load: " + error?.GetMessage());
            return;
        }

        _interstitialAd = ad;
        _interstitialAd.OnAdPaid += HandleInterstitialAdPaid;
        _interstitialAd.OnAdClicked += HandleInterstitialAdClicked;
        _interstitialAd.OnAdFullScreenContentOpened += HandleInterstitialAdOpened;
        _interstitialAd.OnAdFullScreenContentClosed += HandleInterstitialAdClosed;
        _interstitialAd.OnAdFullScreenContentFailed += HandleInterstitialAdFailedToShow;
        
        Debug.Log("Interstitial ad loaded successfully");
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready to show");
            LoadInterstitialAd();
        }
    }

    private void HandleInterstitialAdPaid(AdValue value)
    {
        Debug.Log("Interstitial ad paid: " + value.Value);
    }

    private void HandleInterstitialAdClicked()
    {
        Debug.Log("Interstitial ad clicked");
    }

    private void HandleInterstitialAdOpened()
    {
        Debug.Log("Interstitial ad opened");
    }

    private void HandleInterstitialAdClosed()
    {
        Debug.Log("Interstitial ad closed");
        LoadInterstitialAd(); // Load next ad
    }

    private void HandleInterstitialAdFailedToShow(AdError error)
    {
        Debug.LogError("Interstitial ad failed to show: " + error.GetMessage());
        LoadInterstitialAd();
    }

    void OnDestroy()
    {
        DestroyBannerAd();
    }
}
```

### 4.2 Add Ad Manager to Your Scene

1. **Create an empty GameObject** in your main menu/game scene
2. Name it **"AdManager"**
3. **Drag the AdManager.cs script** onto this GameObject (as a component)
4. ✅ The script will now initialize ads when the game starts

### 4.3 Update Ad Unit IDs

**IMPORTANT**: Replace the placeholder ad unit IDs in the script:

```csharp
// Replace these with YOUR ids from AdMob (Step 2.4)
private string _bannerAdUnitId = "YOUR_BANNER_AD_UNIT_ID";
private string _rewardedAdUnitId = "YOUR_REWARDED_AD_UNIT_ID";
private string _interstitialAdUnitId = "YOUR_INTERSTITIAL_AD_UNIT_ID";
```

### 4.4 Call Ads from Your Game Code

**Show banner ad (always visible at bottom):**
```csharp
AdManager.Instance.ShowBannerAd();
```

**Show rewarded ad (player earns reward for watching):**
```csharp
AdManager.Instance.ShowRewardedAd();
// Modify HandleRewardedAdClosed() to give reward (coins, hints, etc.)
```

**Show interstitial ad (between levels):**
```csharp
// Example: After level complete
AdManager.Instance.ShowInterstitialAd();
```

**Hide banner ad temporarily:**
```csharp
AdManager.Instance.HideBannerAd();
```

---

## STEP 5: Testing with Test Ad Unit IDs

### 5.1 Get Your Test Device ID

Before publishing, test ads on your Android device:

1. **Build APK and install on device** (see Android Build Guide)
2. **Run the game with AdManager** in the scene
3. **Open Android Logcat**:
   - Window → TextMesh Pro → TextMeshPro (or open Android Studio)
   - Run the game
   - Look for a message like:
     ```
     Use RequestConfiguration.Builder().setTestDeviceIds(Arrays.asList("33BE2250B43518CCDA7DE426D04EE232")) to get test ads on this device.
     ```
4. **Copy this device ID** (the long hex string)

### 5.2 Add Test Device to AdMob Settings

1. **Go to AdMob Dashboard** → Settings → General
2. Scroll to **"Test Devices"**
3. **Add Device** → Paste your device ID
4. Click **"Add"**

### 5.3 Use Google Test Ad Unit IDs (ALWAYS SAFE)

For testing purposes, Google provides special test ad unit IDs that always show test ads:

**Replace your IDs with these test IDs:**

```csharp
// TEST Ad Unit IDs (Safe for testing - shows Google test ads)
private string _bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";      // Banner
private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";    // Rewarded
private string _interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // Interstitial
```

⚠️ **IMPORTANT**: Only use these test IDs for testing! Switch back to your real IDs before publishing.

### 5.4 Test on Device

1. **Build and run your game on Android device**
2. **Test each ad type**:
   - Banner should appear at bottom
   - Rewarded ad should show full-screen video
   - Interstitial should show between levels
3. **Check logcat** for any errors
4. All ads should say **"Test Ad"** or show Google ads

✅ **All ads tested successfully**

---

## STEP 6: Going Live Checklist

When you're ready to publish to Google Play:

### Before Publishing:

- [ ] All three ad unit IDs are added to AdManager.cs (your REAL ids, not test ids)
- [ ] App ID is set in Window → Google Mobile Ads → Settings
- [ ] AdManager.cs is in your scene
- [ ] All ad calls are working (LogCat shows "loaded successfully")
- [ ] You've tested all three ad types on a device
- [ ] No LogCat errors related to ads
- [ ] Test device ID removed from AdMob settings (only during testing)
- [ ] Game is playable with ads enabled

### Ads Placement Strategy:

For your puzzle game, recommend:

**Banner Ad**: 
- Show at bottom of screen during gameplay
- Hide during loading screens or menus (optional)

**Rewarded Ad**:
- "Watch ad to get bonus coins"
- "Watch ad for extra hint"
- "Watch ad for continue after game over"

**Interstitial Ad**:
- Between level completions (after every 3-5 levels)
- Optional: Before starting a level
- Don't show on every level (players get annoyed)

### Going Live:

1. Build final APK with real ad unit IDs
2. Test on device one more time
3. Upload APK to Google Play Console (see Google Play Setup Guide)
4. Submit app for review
5. Once approved, your app goes live with ads

✅ **Ready to monetize!**

---

## TROUBLESHOOTING

### Issue: "Google Mobile Ads SDK not found"

**Solution**:
1. Import the plugin again: Assets → Import Package → Custom Package
2. Select GoogleMobileAdsPlugin-v*.*.*.unitypackage
3. Click Import
4. Restart Unity

### Issue: Ads fail to load (LogCat shows errors)

**Common causes**:
1. **Wrong Ad Unit IDs** - Verify IDs match your AdMob dashboard
2. **App ID not set** - Go to Window → Google Mobile Ads → Settings and enter App ID
3. **Test device not registered** - Add device ID to AdMob test devices
4. **No internet** - Ensure device has internet connection
5. **Ad Unit is disabled** - Check AdMob dashboard if ad unit is still active

**Quick fix**:
- Use Google test Ad Unit IDs (from Step 5.3) to verify setup works
- If test ads load, your setup is correct
- Then switch back to your real ad unit IDs

### Issue: "RequestConfiguration not found" error

**Solution**: Update Google Mobile Ads SDK to latest version
1. Delete Assets/GoogleMobileAds folder
2. Re-import latest GoogleMobileAdsPlugin package

### Issue: App crashes on Android with AdMob

**Solution**:
1. Ensure Android API level 21+ is selected in Build Settings
2. Check that dependencies are imported correctly
3. Make sure AdManager script is on a persistent GameObject
4. Check LogCat for full error message

### Issue: Test ads don't show

**Solution**:
1. Verify using Google test Ad Unit IDs (Step 5.3)
2. Check LogCat for "loaded successfully" messages
3. Ensure app has internet permission in AndroidManifest.xml
4. Wait 15-20 seconds for ad to load (may be slow on emulator)

### Issue: Real ads show during testing

⚠️ **PROBLEM**: You're showing users real ads during beta testing!

**Solution**:
1. Go to AdMob Dashboard → Settings → Test devices
2. Add your device ID as a test device
3. Now your device will show test ads only
4. Remove from test devices before final release

---

## NEXT STEPS

1. ✅ Complete all steps above
2. ✅ Test on your Android device
3. ✅ Move to **ANDROID_BUILD_GUIDE.md** to build final APK
4. ✅ Then **GOOGLE_PLAY_SETUP_GUIDE.md** to publish app

---

## QUICK REFERENCE: Ad Implementation Summary

| Ad Type | Best For | Frequency | Example |
|---------|----------|-----------|---------|
| **Banner** | Constant revenue | Always visible | Bottom of screen |
| **Rewarded** | Player choice | Optional | "Watch ad for bonus" |
| **Interstitial** | Full-screen | Between actions | After level complete |

**Revenue Estimate**: 
- Banner: $0.50-$2 CPM
- Rewarded: $2-$5 CPM
- Interstitial: $1-$3 CPM
- *CPM = earnings per 1000 impressions*

---

**✅ AdMob setup complete! You're ready for Android build & launch.**

For next steps, follow: **ANDROID_BUILD_GUIDE.md**

